using System;
using System.Text;

namespace Core.Data
{
    public class TransactionException : RepositoryException
    {

        public TransactionException(string message, Exception innerException): base(
            message,innerException)
        {
            
        }

        public override string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("##################### TransactionException ##################");
                sb.AppendLine("An error occurred in the transaction. Always check the PREVIOUS log information for usefull hints about this kind of excpetions.");
                sb.AppendLine(String.Concat("The transaction reported: ",base.Message));
                sb.AppendLine(String.Concat("Inner Messages: ", FormatInnerMessage(base.InnerException)));
                return sb.ToString();
            }
        }

        private static string FormatInnerMessage(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            FormatInnerMessageAsRecursive(exception,ref sb, 0);
            return sb.ToString();
        }

        private static void FormatInnerMessageAsRecursive(Exception exception, ref StringBuilder sb, int level)
        {
			if (level >= 5) return;

            sb.AppendLine(exception.Message);
            if (exception.InnerException != null)
                FormatInnerMessageAsRecursive(exception,ref sb, ++level);
        }
    }
}
