using System;

namespace Core.Data.Exceptions.Hql
{
    public class HqlException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HqlException"/> class.
        /// </summary>
        public HqlException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HqlException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public HqlException(string message) : base(message) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HqlException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception</param>
        public HqlException(string message, Exception innerException) : base(message, innerException) { }
    }
}
