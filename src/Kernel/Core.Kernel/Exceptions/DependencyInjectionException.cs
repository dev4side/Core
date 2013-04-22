using System;
using System.Text;

namespace Core.Kernel.Exceptions
{
    public class DependencyInjectionException : Exception
    {
        private readonly string _message;

        private const string HeaderMessage = "## Exception has been reised in configuring the Depending Injection Container ##";

        public DependencyInjectionException(string message)
        {
            _message = message;
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>The error message that explains the reason for the exception, or a generic message.</returns>
        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(HeaderMessage);
                sb.AppendLine(_message);
                return sb.ToString();
            }
        }
    }
}