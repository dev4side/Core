using System;
using Core.Common.Utils;

namespace Core.Common.Mappers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MapToRegistryKeyProperty: Attribute
    {
        public string Key { get; private set; }
        public string Path { get; private set; }

        public MapToRegistryKeyProperty(string path, string key)
        {
            Path = path;
            Key = key;
        }

        public BooleanConversion BooleanConversion { get; set; }
    }
}
