using System;

namespace Core.Data
{
    public class RepositoryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class.
        /// </summary>
        public RepositoryException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RepositoryException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception</param>
        public RepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}