using System;

namespace Core.Data.Exceptions
{
    public class TransactionDeleteException : TransactionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionDeleteException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception</param>
        public TransactionDeleteException(string message, Exception innerException) : base(message, innerException) { }
    }
}