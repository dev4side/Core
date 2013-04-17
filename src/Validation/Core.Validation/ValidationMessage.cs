using System;

namespace Core.Validation
{
    public sealed class ValidationMessage
    {
        public string Message { get; internal set; }

        internal ValidationMessage()
        {
        }
    }
}
