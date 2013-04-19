using System;
using Core.Business.Factory;
using Core.Business.Manager;
using Core.Data.Interfaces.Entity;
using Core.Data.Interfaces.Factory;
using Core.Data.Interfaces.Repository;
using Core.Log;
using Ninject;
using Ninject.Activation;

namespace Core.Services.Service
{
    /// <summary>
    /// Provides a base class for service layer
    /// </summary>
    /// <typeparam name="TEntity">The domain entity</typeparam>
    /// <typeparam name="TKey">The key for the relative domain entity</typeparam>
    public abstract class BaseService<TEntity, TKey> where TEntity : class, IDomainEntity<TKey>
    {
        [Inject]
        public virtual IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

        [Inject]
        public virtual IManagerFactory ManagerFactory { get; set; }

        [Inject]
        public virtual IContext CurrentContext { get; set; }

        [Inject]
        public virtual ILog<BaseService<TEntity, TKey>> Log { get; set; }
        
        public virtual TManager Manager<TManager>(IUnitOfWork unitOfWork) where TManager : IManagerMarker
        {
            return ManagerFactory.GetManager<TManager>(unitOfWork);
        }
    }
}
