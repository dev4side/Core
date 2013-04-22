using System;

namespace Core.Common.Cache.Exceptions
{
    /// <summary>
    /// Represents errors that occur during execution cache layer.
    /// </summary>
    public class CacheException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheException"/> class.
        /// </summary>
        public CacheException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public CacheException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception</param>
        public CacheException(string message, Exception innerException) : base(message, innerException) { }
    }
}
