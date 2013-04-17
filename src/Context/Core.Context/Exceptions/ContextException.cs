using System;

namespace Core.Context.Exceptions
{
    public class ContextException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextException"/> class.
        /// </summary>
        public ContextException() : base() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ContextException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception</param>
        public ContextException(string message, Exception innerException) : base(message, innerException) {}
    }
}
