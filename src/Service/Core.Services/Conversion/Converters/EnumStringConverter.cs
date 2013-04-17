using System;
using Core.Common.Utils;

namespace Core.Services.Conversion
{
   
    public class EnumStringConverter : IConvertCommand
    {
        public object ConvertToDto(object toConvert, Type requestedType)
        {
            if (!toConvert.GetType().IsEnum)
                throw new ConvertCommandException(toConvert.GetType(), toConvert);
            if (requestedType != typeof(string))
                throw new ConvertCommandException(toConvert.GetType(), toConvert);

			return StringEnum.GetStringValue(toConvert as Enum);
        }

        public object ConvertToEntity(object toConvert, Type requestedType)
        {
            if (toConvert.GetType() != typeof(string))
                throw new ConvertCommandException(toConvert.GetType(), toConvert);
            if (!requestedType.IsEnum)
                throw new ConvertCommandException(toConvert.GetType(), toConvert);

            return StringEnum.Parse(requestedType, toConvert.ToString());

        }
    }
}
