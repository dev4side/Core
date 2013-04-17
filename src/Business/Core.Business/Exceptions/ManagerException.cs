using System;

namespace Core.Business.Exceptions
{
    public class ManagerException : Exception
    {
        private readonly string _message;

        public ManagerException(string message)
        {
            _message = message;
        }

        public override string Message
        {
            get { return _message; }
        }
    }
}