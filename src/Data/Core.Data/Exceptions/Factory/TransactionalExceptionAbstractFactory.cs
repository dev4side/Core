using System;
using System.Data.SqlClient;

namespace Core.Data.Exceptions.Factory
{
    public static class TransactionalExceptionAbstractFactory
    {
         public static TransactionException GetException(Exception exception)
         {
             var exceptionToParse = GetSqlException(exception);
             return GetTransactionExceptionFromSqlException(exceptionToParse);
         }

         private static SqlException GetSqlException(Exception exception)
         {
             if(exception == null)
                 throw new Exception("The provided exception does not contains a SqlException ");
             if (exception is SqlException)
                 return exception as SqlException;
              return GetSqlException(exception.InnerException);
         }

         private static TransactionException GetTransactionExceptionFromSqlException(SqlException exceptionToParse)
         {

             // http://msdn.microsoft.com/en-us/library/cc645611.aspx
             // specific to SQL SERVER 2008!!! farlo piu figo, leggere da file, astrarre la cioncia, etc...
             switch (exceptionToParse.Number)
             {
                 case 547:
                     return new TransactionDeleteException("Reference contraints conflit", exceptionToParse);
                 default: return new TransactionException("Unable commit transaction. A roll back will be tryied", exceptionToParse);
             }
         }
    }
}