using System;
using System.Collections.Generic;

namespace Core.Services.Conversion
{
    public static class ConverterManager 
    {
        private static Dictionary<ConvertMechanism, IConvertCommand> _dictionary;

        static ConverterManager()
        {
            _dictionary = CreateDictionary();
        }

        public static object ConvertToEntity(object toConvert, ConvertMechanism convertMechanismToUse, Type requestedType)
        {
            if (toConvert == null)
                return null;
            return _dictionary[convertMechanismToUse].ConvertToEntity(toConvert, requestedType);
        }

        public static object ConvertToDto(object toConvert, ConvertMechanism convertMechanismToUse, Type requestedType)
        {
            if (toConvert == null)
                return null;
            return _dictionary[convertMechanismToUse].ConvertToDto(toConvert, requestedType);
        }

        public static Dictionary<ConvertMechanism, IConvertCommand> CreateDictionary()
        {
            var dictionary = new Dictionary<ConvertMechanism, IConvertCommand>
                                 {
                                     {ConvertMechanism.None, new NoneConverter()},
                                     {ConvertMechanism.CoBodyPart, new CoBodyPartConverter()},
                                     {ConvertMechanism.EnumString, new EnumStringConverter()},
                                     {ConvertMechanism.ObjectGuid, new ObjectGuidConverter()}
                                 };
            return dictionary;
        }
    }

    
}
