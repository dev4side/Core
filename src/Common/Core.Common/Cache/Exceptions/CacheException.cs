using System;

namespace Core.Common.Cache.Exceptions
{
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
