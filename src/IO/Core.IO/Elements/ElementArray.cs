using System;
using System.Reflection;

namespace Core.IO
{
	public class ElementArray : ElementSequence
	{
		public IElement LengthElement { get; set; }
		public Type ItemsType { get; set; }

		private MethodInfo _lengthGenericGetter;
		private MethodInfo _lengthGenericSetter;

		public int Length
		{
			get
			{
				return elements.Count;
			}
			set
			{
				//add if needed..
				while (elements.Count < value)
					elements.Add(ElementHelper.GetElementForPropertyType(ItemsType, "[" + elements.Count + "]", 0));

				//remove if needed..
				while (elements.Count > value)
					elements.RemoveAt(value);
			}
		}

		public ElementArray(string groupName,  int bufferStartPosition, Type itemsType, IElement lengthElement)
		    : base(groupName, bufferStartPosition)
		{
			if(itemsType == null) throw new ArgumentNullException("itemsType","Type of array items cannot be null.");
			if(lengthElement == null) throw new ArgumentNullException("lengthElement","Element for length retrieving cannot be null.");

			//get generic element value..
			var _lengthElementType = ElementHelper.ExtractValueTypeFromGenericElementType(lengthElement.GetType());

			//check invalid structs..
			if( (!_lengthElementType.IsValueType) || (_lengthElementType == typeof(bool)) || (_lengthElementType == typeof(char)) || (_lengthElementType == typeof(decimal)) || (_lengthElementType == typeof(Enum)) || (_lengthElementType == typeof(double)) || (_lengthElementType == typeof(float)))
				throw new NotSupportedException(String.Format("Type IElement<{0}> passed as LengthElement for the ElementArray \"{1}\" is not supported. Just value types like int, short, ushort..", _lengthElementType != null ? _lengthElementType.Name : "", Name));

			//prepare generic get/set delegates..
			_lengthGenericGetter = typeof(ElementArray).GetMethod("GetGenericElementValue", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(_lengthElementType);
			_lengthGenericSetter = typeof(ElementArray).GetMethod("SetGenericElementValue", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(_lengthElementType);

			ItemsType = itemsType;
			LengthElement = lengthElement;
		}
		
		public override void UpdateValueFromBuffer(byte[] buffer)
		{
			//cache length..
			var len = (int) _lengthGenericGetter.Invoke(this, new object[] { LengthElement } );

			//adjust elements..
			this.Length = len;

			//read items from buffer..
			base.UpdateValueFromBuffer(buffer);
		}

		public override void UpdateBufferFromValue(byte[] buffer)
		{
			//write array items on buffer..
			base.UpdateBufferFromValue(buffer);

			//update length element..
			_lengthGenericSetter.Invoke(this, new object[] { LengthElement, this.Length });

			//write length on buffer..
			LengthElement.UpdateBufferFromValue(buffer);

		}

		//
		//	WORKING FUNCTIONS
		//

		private int GetGenericElementValue<TStruct>(IElement<TStruct> element)
		{
			return Convert.ToInt32(element.Value);
		}

		private void SetGenericElementValue<TStruct>(IElement<TStruct> element, int intValue)
		{
			element.Value = (TStruct)Convert.ChangeType(intValue, typeof(TStruct));
		}
	}
}
