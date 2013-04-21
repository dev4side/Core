using Core.Common.Mapper.Registry;
using System;

namespace Core.Common.Mappers.Registry
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MapToRegistryKeyPropertyAttribute: Attribute
    {
        public string Key { get; private set; }
        public string Path { get; private set; }

        public MapToRegistryKeyPropertyAttribute(string path, string key)
        {
            Path = path;
            Key = key;
        }

        public RegistryConversion RegistryConversion { get; set; }
    }
}
