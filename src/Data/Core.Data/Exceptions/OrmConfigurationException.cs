using System;

namespace Core.Data.Exceptions
{
    public class OrmConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrmConfigurationException"/> class.
        /// </summary>
        public OrmConfigurationException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrmConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public OrmConfigurationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrmConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception</param>
        public OrmConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
