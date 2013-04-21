using System;

namespace Core.Common.Mappers.Registry
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MapToRegistryKeyAttribute : Attribute { }
}
