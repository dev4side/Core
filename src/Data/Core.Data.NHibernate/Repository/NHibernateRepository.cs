using Core.Data.Interfaces.Entity;
using Core.Data.Interfaces.Repository;

namespace Core.Data.NHibernate
{
    public class NHibernateRepository<TEntity, TKey> : BaseNHibernateRepository<TEntity, TKey> where TEntity : class, IDomainEntity<TKey>
    {
        public NHibernateRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
    }
}