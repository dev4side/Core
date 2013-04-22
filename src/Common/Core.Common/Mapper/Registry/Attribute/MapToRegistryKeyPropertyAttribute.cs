using System;

namespace Core.Common.Mapper.Registry.Attribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MapToRegistryKeyPropertyAttribute: System.Attribute
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
