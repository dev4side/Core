using System;
using System.Collections;
using System.Reflection;

namespace Core.Common.Util
{
	/// <summary>
	/// Helper class for working with 'extended' enums using <see cref="StringValueAttribute"/> attributes.
	/// </summary>
	public class StringEnum
	{
		private Type _enumType;
		private static Hashtable _stringValues = new Hashtable();

		/// <summary>
		/// Creates a new <see cref="StringEnum"/> instance.
		/// </summary>
		/// <param name="enumType">Enum type.</param>
		public StringEnum(Type enumType)
		{
			if (!enumType.IsEnum)
				throw new ArgumentException(String.Format("Supplied type must be an Enum.  Type was {0}", enumType.ToString()));

			_enumType = enumType;
		}

		/// <summary>
		/// Gets the string value associated with the given enum value.
		/// </summary>
		/// <param name="valueName">Name of the enum value.</param>
		/// <returns>String Value</returns>
		public string GetStringValue(string valueName)
		{
			Enum enumType;
			string stringValue = null;
			try
			{
				enumType = (Enum) Enum.Parse(_enumType, valueName);
				stringValue = GetStringValue(enumType);
			}
			catch (Exception) { }//Swallow!

			return stringValue;
		}

		/// <summary>
		/// Gets the string values associated with the enum.
		/// </summary>
		/// <returns>String value array</returns>
		public Array GetStringValues()
		{
			ArrayList values = new ArrayList();
			//Look for our string value associated with fields in this enum
			foreach (FieldInfo fi in _enumType.GetFields())
			{
				//Check for our custom attribute
				StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
				if (attrs.Length > 0)
					values.Add(attrs[0].Value);
				else
					values.Add(fi.Name);

			}

			return values.ToArray();
		}

		/// <summary>
		/// Gets the values as a 'bindable' list datasource.
		/// </summary>
		/// <returns>IList for data binding</returns>
		public IList GetListValues()
		{
			Type underlyingType = Enum.GetUnderlyingType(_enumType);
			ArrayList values = new ArrayList();
			//Look for our string value associated with fields in this enum
			foreach (FieldInfo fi in _enumType.GetFields())
			{
				//Check for our custom attribute
				StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
				if (attrs.Length > 0)
					values.Add(new DictionaryEntry(Convert.ChangeType(Enum.Parse(_enumType, fi.Name), underlyingType), attrs[0].Value));
				else
					values.Add(new DictionaryEntry(Convert.ChangeType(Enum.Parse(_enumType, fi.Name), underlyingType), fi.Name));
			}

			return values;

		}

		/// <summary>
		/// Return the existence of the given string value within the enum.
		/// </summary>
		/// <param name="stringValue">String value.</param>
		/// <returns>Existence of the string value</returns>
		public bool IsStringDefined(string stringValue)
		{
			return Parse(_enumType, stringValue) != null;
		}

		/// <summary>
		/// Return the existence of the given string value within the enum.
		/// </summary>
		/// <param name="stringValue">String value.</param>
		/// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
		/// <returns>Existence of the string value</returns>
		public bool IsStringDefined(string stringValue, bool ignoreCase)
		{
			return Parse(_enumType, stringValue, ignoreCase) != null;
		}

		/// <summary>
		/// Gets the underlying enum type for this instance.
		/// </summary>
		/// <value></value>
		public Type EnumType
		{
			get { return _enumType; }
		}


		/// <summary>
		/// Gets a string value for a particular enum value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>String Value associated via a <see cref="StringValueAttribute"/> attribute, or null if not found.</returns>
		public static string GetStringValue(Enum value)
		{
			string output = null;
			Type type = value.GetType();

			if (_stringValues.ContainsKey(value))
				output = (_stringValues[value] as StringValueAttribute).Value;
			else 
			{				
				//FieldInfo fi = type.GetField(value.ToString()); //tolto per con le enum che hanno [Flags] andava in errore
                FieldInfo fi = type.GetField(Enum.GetName(type, value));

                //Look for our 'StringValueAttribute' in the field's custom attributes
				StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
				if (attrs.Length > 0)
				{
					_stringValues.Add(value, attrs[0]);
					output = attrs[0].Value;
				}
				else
				{
					output = value.ToString();
				}
					
			}
			return output;

		}

		/// <summary>
		/// Parses the supplied enum and string value to find an associated enum value (case sensitive).
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="stringValue">String value.</param>
		/// <returns>Enum value associated with the string value, or null if not found.</returns>
		public static object Parse(Type type, string stringValue)
		{
			return Parse(type, stringValue, false);
		}

		/// <summary>
		/// Parses the supplied enum and string value to find an associated enum value.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="stringValue">String value.</param>
		/// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
		/// <returns>Enum value associated with the string value, or null if not found.</returns>
		public static object Parse(Type type, string stringValue, bool ignoreCase)
		{
			object output = null;
			string enumStringValue = null;

			if (!type.IsEnum)
				throw new ArgumentException(String.Format("Supplied type must be an Enum.  Type was {0}", type.ToString()));

			//Look for our string value associated with fields in this enum
			foreach (FieldInfo fi in type.GetFields())
			{
				//Check for our custom attribute
				StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
				if (attrs.Length > 0)
					enumStringValue = attrs[0].Value;

				//Check for equality then select actual enum value.
				if ((string.Compare(enumStringValue, stringValue, ignoreCase) == 0) || (string.Compare(fi.Name, stringValue, ignoreCase) == 0) )
				{
					output = Enum.Parse(type, fi.Name);
					break;
				}
			}

			return output;
		}

		/// <summary>
		/// Return the existence of the given string value within the enum.
		/// </summary>
		/// <param name="stringValue">String value.</param>
		/// <param name="enumType">Type of enum</param>
		/// <returns>Existence of the string value</returns>
		public static bool IsStringDefined(Type enumType, string stringValue)
		{
			return Parse(enumType, stringValue) != null;
		}

		/// <summary>
		/// Return the existence of the given string value within the enum.
		/// </summary>
		/// <param name="stringValue">String value.</param>
		/// <param name="enumType">Type of enum</param>
		/// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
		/// <returns>Existence of the string value</returns>
		public static bool IsStringDefined(Type enumType, string stringValue, bool ignoreCase)
		{
			return Parse(enumType, stringValue, ignoreCase) != null;
		}
	}
}