using System;

namespace  Core.Services.Conversion
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class MapToEntity : Attribute
    {
        private readonly string _entityClassName;

        public MapToEntity(string entityClassName)
        {
            _entityClassName = entityClassName;
        }

        public string EntityName { get { return _entityClassName; } }  
    }
}
