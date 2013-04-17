using System;

namespace Core.Services.Conversion
{
   
    public class NoneConverter : IConvertCommand
    {
        public object ConvertToDto(object toConvert,Type requestedType)
        {
            return toConvert;
        }

        public object ConvertToEntity(object toConvert, Type requestedTyp)
        {
            return toConvert;
        }
    }
}

