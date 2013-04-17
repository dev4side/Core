using System;

namespace Core.Data
{
    public interface IRepositoryFactoryGuid : IRepositoryFactoryGeneric<Guid>
    {
        new IRepository<TEntity, Guid> GetRepository<TEntity>(IUnitOfWork untitOfWork) where TEntity : class, IDomainEntity<Guid>;
    }
}
