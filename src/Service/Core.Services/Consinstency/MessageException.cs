using System;

namespace Core.Services.Consinstency
{
    public class MessageException : Exception
    {
        public MessageException(string message) : base(message) { }
    }
}
