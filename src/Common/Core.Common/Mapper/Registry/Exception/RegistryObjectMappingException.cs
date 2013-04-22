namespace Core.Common.Mapper.Registry.Exception
{
    public class RegistryObjectMappingException : System.Exception
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
