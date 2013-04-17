using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Services.Consinstency
{
    public class MessageException : Exception
    {
        public MessageException(string message) : base(message) { }
    }
}
