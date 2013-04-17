using System;

namespace Core.Data
{
    public interface IRepositoryFactoryGeneric<TKey>
    {
        IRepository<TEntity, TKey> GetRepository<TEntity>(IUnitOfWork untitOfWork) where TEntity : class, IDomainEntity<TKey>;
        IProjection GetProjection<TEntity>(string projectionClause, string projectionName, string projectionAlias);
        IProjection GetProjection<TEntity>(string projectionName, string projectionAlias);
        IRestriction GetRestriction<TEntity>(string restirction);
        IJoin GetJoin<TEntity>(string join);
        IOrdination GetOrdination<TEntity>(string ordinationName, string ordinationType);
        T GetManager<T>(IUnitOfWork untitOfWork);
    }
}
