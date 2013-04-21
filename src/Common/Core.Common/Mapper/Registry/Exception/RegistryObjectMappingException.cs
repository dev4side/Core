using System;

namespace Core.Common.Mapper.Registry
{
    public class RegistryObjectMappingException : Exception
    {
        private readonly string _message;
        public RegistryObjectMappingException(string message)
        {
            _message = message;
        }
        public override string Message
        {
            get { return _message; }
        }
    }
}
