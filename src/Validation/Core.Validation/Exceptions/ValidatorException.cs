using System;

namespace Core.Validation
{
    public class ValidatorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorException"/> class.
        /// </summary>
        public ValidatorException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ValidatorException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception</param>
        public ValidatorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
