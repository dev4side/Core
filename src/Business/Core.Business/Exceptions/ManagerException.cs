using System;

namespace Core.Business.Exceptions
{
    /// <summary>
    /// Represents errors that occur during execution of a manager into the business layer.
    /// </summary>
    public class ManagerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerException"/> class.
        /// </summary>
        public ManagerException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ManagerException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception</param>
        public ManagerException(string message, Exception innerException) : base(message, innerException) { }
    }
}