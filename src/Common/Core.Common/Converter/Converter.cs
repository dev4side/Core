using Core.Common.Mapper.Registry;
using System;
using Core.Common.Mapper.Registry.Attribute;

namespace Core.Common.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Check if the given object is a boolean, and if true, applies inverse conversion with the given BooleanConversion rule
        /// This is the opposit of ConvertToBoolean
        /// </summary>
        /// <param name="value"></param>
        /// <param name="booleanConversion"></param>
        /// <returns></returns>
        public static object TryInverseBooleanConvertion(object value, RegistryConversion booleanConversion)
        {
            if (value.GetType().FullName.ToLower() == "system.boolean")
            {
                bool valueAsBool = (bool)value;
                switch (booleanConversion)
                {
                    case RegistryConversion.BoolEnabledDisabled:
                        return valueAsBool ? "Enabled" : "Disabled";
                    case RegistryConversion.BoolBit:
                        return valueAsBool ? 1 : 0;
                    default:
                        return Convert.ToBoolean(value);
                }
            }
            
            return value;
        }

        /// <summary>
        /// Converts an object into a requested type.
        /// </summary>
        public static object ConvertToPropertyType(object objectoToCOnvert, Type requestedResultType, RegistryConversion booleanConversionRule)
        {
            switch (requestedResultType.FullName.ToLower())
            {
                case "system.double":
                    return Double.Parse(objectoToCOnvert.ToString());
                case "system.boolean":
                    return ConvertToBoolean(objectoToCOnvert, booleanConversionRule);
                case "system.int32":
                    return Int32.Parse(objectoToCOnvert.ToString());
                default:
                    return objectoToCOnvert;
            }
        }

        /// <summary>
        /// Converts an object into a bool by follwoing the specified BooleanConversion rule
        /// This is the opposit of TryInverseBooleanConvertion
        /// </summary>
        public static bool ConvertToBoolean(object value, RegistryConversion booleanConversion)
        {
            switch (booleanConversion)
            {
                case RegistryConversion.BoolEnabledDisabled:
                    switch (value.ToString().ToLower())
                    {
                        case "enabled":
                            return true;
                        case "disabled":
                            return false;
                        case "":
                            return false;
                    }
                    throw new Exception(String.Format("Unable to convert {0} into bool", value));
                case RegistryConversion.BoolBit:
                    return Convert.ToBoolean(Convert.ToInt32(value));
                default:
                    return Convert.ToBoolean(value);
            }
        }
    }
}
