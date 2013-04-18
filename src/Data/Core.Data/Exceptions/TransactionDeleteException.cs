using System;

namespace Core.Data.Exceptions
{
    public class TransactionDeleteException : TransactionException
    {
        public TransactionDeleteException(string message, Exception innerException) : base(message, innerException) { }
    }
}