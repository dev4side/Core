using Core.Business.Factory;
using Core.Data;
using Core.Data.Interfaces.Entity;
using Core.Data.Interfaces.Factory;
using Core.Data.Interfaces.Repository;
using Core.Kernel;
using Ninject;

namespace Core.Business.Manager
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class BaseManager<TEntity, TKey> where TEntity : class, IDomainEntity<TKey>
    {
        private IRepository<TEntity, TKey> _currentRepository;

        [Inject]
        protected IRepositoryFactoryGeneric<TKey> RepositoryFactory { get; private set; }

		[Inject]
		protected IManagerFactory ManagerFactory { get; private set; }

        [Inject]
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; private set; }

        protected IUnitOfWork CurrentUnitOfWork { get; private set; }
        
        protected IRepository<TEntity, TKey> Repository
        {
            get {
                return _currentRepository ?? (_currentRepository = RepositoryFactory.GetRepository<TEntity>(CurrentUnitOfWork));
            }
        }

        protected BaseManager(IUnitOfWork unitOfWork)
        {
            ObjectFactory.ResolveDependencies(this);
            CurrentUnitOfWork = unitOfWork;
        }

        protected IRepository<TOtherEntity, TKey> GetRepository<TOtherEntity>() where TOtherEntity : class, IDomainEntity<TKey>
        {
            return RepositoryFactory.GetRepository<TOtherEntity>(CurrentUnitOfWork);
        }
        
        public TEntity GetById(TKey id)
        {
            return Repository.GetById(id);
        }

        public TEntity GetByIdOrNull(TKey id)
        {
            return Repository.GetByIdOrNull(id);
        }
    }
}
