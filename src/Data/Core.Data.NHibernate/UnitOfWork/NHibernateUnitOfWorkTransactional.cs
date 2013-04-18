using System;
using Core.Data.Exceptions;
using Core.Data.Exceptions.Factory;
using NHibernate;
using NHibernate.Exceptions;
using TransactionException = Core.Data.Exceptions.TransactionException;

namespace Core.Data.NHibernate
{
    public class NHibernateUnitOfWorkTransactional : NHibBaseUnitOfWork
    {
        private ITransaction _transaction;

        public NHibernateUnitOfWorkTransactional(ISessionFactory sessionFactory)
        {
            Session = sessionFactory.OpenSession();
            StartTransaction();
        }

        public override void Dispose()
        {
            try
            {
                CommitTransaction();
            }
            finally
            {
                base.Dispose();
            }
        }

        private void StartTransaction()
        {
            _transaction = Session.BeginTransaction();
        }

        private void CommitTransaction()
        {
            try
            {
                _transaction.Commit();
                _transaction.Dispose();
            }
            catch (GenericADOException exception)
            {
                TransactionException transactionException = TransactionalExceptionAbstractFactory.GetException(exception);
                Log.Warn(transactionException);
                RollBackTransactionAndSetDisposedToTrue();
                Log.Warn("Transaction has been rollbacked correctly");
                throw transactionException;
            }
            catch (PropertyValueException ex)
            {
                var exceptionToThrow = new RepositoryException(ex.Message, ex);
                Log.Warn(exceptionToThrow, "Missing required property");
                _transaction.Rollback();
                throw exceptionToThrow;
            }
            catch (Exception exception)
            {
                _transaction.Rollback();
                Log.Error(exception, "An unknow exception has been throw during UnitOfWork disposing.");
                throw;
            }
        }

        private void RollBackTransactionAndSetDisposedToTrue()
        {
            _transaction.Rollback();
        }
    }
}