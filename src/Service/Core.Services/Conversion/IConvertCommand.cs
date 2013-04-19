using System;

namespace Core.Services.Conversion
{
    public interface IConvertCommand
    {
        object ConvertToDto(object toConvert, Type requestedType);
        object ConvertToEntity(object toConvert, Type requestedType);
    }
}
