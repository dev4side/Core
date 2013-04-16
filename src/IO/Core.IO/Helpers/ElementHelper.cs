using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace Core.IO
{
	public static class ElementHelper
	{
		private static Dictionary<Type, ElementSequence> _complexElements;
		public static Dictionary<Type, ElementSequence> ComplexElements
		{
			get
			{
				if (_complexElements == null)
					_complexElements = new Dictionary<Type, ElementSequence>();
				return _complexElements;
			}
		}

		private static Dictionary<Type, Type> _basicElements;
		public static Dictionary<Type, Type> BasicElements
		{
			get
			{
				if (_basicElements == null)
				{
					//PROBLEMA: si sostituiscono gli IElement<> con tipo interno uguale..
					var tmp = new IntElement32("", 0);	//load Core.IO assembly in the case it's not loaded yet..
					_basicElements = new Dictionary<Type, Type>();

					var tmpCollection = from type in typeof(IElement).Assembly.GetTypes()
										where !type.IsInterface
										where !type.IsAbstract
										from iface in type.GetInterfaces()
										where iface.IsGenericType
										where iface.GetGenericTypeDefinition() == typeof(IElement<>)
										where !iface.GetGenericArguments()[0].IsGenericParameter	//excludes definition IElement<TStruct>
										select new KeyValuePair<Type, Type>(iface.GetGenericArguments()[0], type);

					foreach (var tmpPair in tmpCollection)
						_basicElements[tmpPair.Key] = tmpPair.Value;
				}
				return _basicElements;
			}
		}

		public static bool IsBasicType(Type type)
		{
			return BasicElements.ContainsKey(type);
		}

		public static bool IsListType(Type type)
		{
			return type.GetInterfaces().Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IList<>)));
		}

		//public static bool IsComplexType(Type type)
		//{
		//    return ComplexElements.ContainsKey(type);
		//}

		public static ElementSequence GetComplexElementForType(Type type, string name, int position = 0)
		{
			ElementSequence seq = null;

			if (ComplexElements.ContainsKey(type))
			{
				seq = ComplexElements[type];
				if (seq != null)
				{
					seq.Name = name;
					seq.Position = position;
					return seq;
				}
			}

			seq = new ElementSequence(name, position);

			foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				Type elementType = null;
				IElement elementObj = null;
				string elementName = null;
				int? elementPosition = null;
				IElement lengthElement = null;

				//check if an ad-hoc attribute has been set..
				if(Attribute.IsDefined(property,typeof(ElementTypeAttribute)))
				{
					var attribute = Attribute.GetCustomAttribute(property, typeof(ElementTypeAttribute)) as ElementTypeAttribute;
					elementPosition = attribute.BufferPosition;
					elementName = attribute.ElementName;
					elementType = attribute.ElementType;
					if (attribute.IsArray && !String.IsNullOrWhiteSpace(attribute.ArrayLengthProperty))
						lengthElement = seq.Elements.Where(e => e.Name == attribute.ArrayLengthProperty).First();	//throws if the LengthElement is not been defined yet..

					//create element, just basic elements can be specified through attributes..
					if (elementType != null)
						elementObj = GetDefaultInstanceOfElementType(elementType, elementName ?? property.Name, elementPosition ?? 0);
				}

				//get the rigth element..
				if (elementObj == null)
				{
					elementObj = GetElementForPropertyType(property.PropertyType, elementName ?? property.Name, elementPosition ?? 0, lengthElement);
				}

				if (elementObj == null)
					throw new Exception("No IElement<> supports type: " + property.PropertyType.Name);

				seq.Elements.Add(elementObj);
			}

			//cache it for further use..
			//ComplexElements[type] = seq;
			//TODO: Commentato. Una cache statica non va bene perché è tutto multithread e a volte asincrono quindi si accavallano i valori negli elementi..
			//TODO: Implementare un metodo di clonazione e tenere in cache un prototipo da clonare ogni volta arriva una richiesta per lo stesso tipo..
			return seq;
		}

		//obsolete..
		public static Type GetBasicElementTypeForPropertyType(Type propertyType)
		{
			//seleziona il primo IElement<T> dove T è propertyType..
			var elementType = (from element in BasicElements.Values
							   from iface in element.GetInterfaces()
							   where iface.GetInterfaces().Contains(typeof(IElement))
							   where iface.IsGenericType
							   where iface.GetGenericArguments()[0] == propertyType
							   select element).FirstOrDefault();
			return elementType;
		}

		//to be dismissed..
		public static IElement GetDefaultInstanceOfElementType(Type elementType, string elementName, int elementPosition)
		{
			//controllo che l'elemento abbia un convenzionale costruttore(string,int) e ne istanzio uno..
			//TODO: sostituire con implementazione di un metodo astratto in BaseElement<TStruct> tipo "IElement<TStruct> GetDefaultInstance()"
			var standardTwoParamsCtor = (from ctor in elementType.GetConstructors()
										 let parameters = ctor.GetParameters()
										 where parameters.Length - (from p in parameters where p.IsOptional select p).Count() == 2
										 where parameters[0].ParameterType == typeof(string)
										 where parameters[1].ParameterType == typeof(int)
										 select ctor).FirstOrDefault();

			if (standardTwoParamsCtor == null)
				throw new NotSupportedException("At the moment, only IElement implementation with ctor(string,int) are supported");

			//make up parameters list..
			var ctorParameters = new List<object>();
			ctorParameters.Add(elementName);
			ctorParameters.Add(elementPosition);
			foreach (var val in standardTwoParamsCtor.GetParameters().Where(p => p.IsOptional).Select(p => p.DefaultValue))
				ctorParameters.Add(val);

			return standardTwoParamsCtor.Invoke(ctorParameters.ToArray()) as IElement;
		}

		public static ElementArray GetElementArrayForType(Type itemType, string elementName, int elementPosition = 0, IElement arrayLengthElement = null)
		{
			return new ElementArray(elementName, elementPosition, itemType, arrayLengthElement);
		}

		public static IElement GetElementForPropertyType(Type propertyType, string elementName, int elementPosition, IElement arrayLengthElement = null)
		{
			if (IsBasicType(propertyType))
			{
				return GetDefaultInstanceOfElementType(BasicElements[propertyType], elementName, elementPosition);
			}
			else if (IsListType(propertyType))
			{
				return GetElementArrayForType(ExtractItemTypeFromGenericListType(propertyType), elementName, elementPosition, arrayLengthElement);
			}
			else
			{
				return GetComplexElementForType(propertyType, elementName, elementPosition);
			}
		}

		public static Type ExtractValueTypeFromGenericElementType(Type genericElementType)
		{
			if(genericElementType == null) throw new ArgumentNullException("genericElementType");

			Type genericArgumentType = (from iface in genericElementType.GetInterfaces()
										where iface.IsGenericType
										where iface.GetGenericTypeDefinition() == typeof(IElement<>)
										select iface.GetGenericArguments()[0]).FirstOrDefault();

			if(genericArgumentType == null)
				throw new ArgumentException(String.Format("The type {0} does not implement IElement<> so can't extract generic argument type.",genericElementType.Name));

			return genericArgumentType;
		}

		public static Type ExtractItemTypeFromGenericListType(Type genericListType)
		{
			if (genericListType == null) throw new ArgumentNullException("genericElementType");

			Type genericArgumentType = (from iface in genericListType.GetInterfaces()
										where iface.IsGenericType
										where iface.GetGenericTypeDefinition() == typeof(IList<>)
										select iface.GetGenericArguments()[0]).FirstOrDefault();

			if (genericArgumentType == null)
				throw new ArgumentException(String.Format("The type {0} does not implement IList<> so can't extract generic argument type.", genericListType.Name));

			return genericArgumentType;
		}

	}
}
