using System;
using Core.Data.Interfaces.Entity;
using Core.Data.Interfaces.Factory;
using Core.Data.Interfaces.Repository;
using Ninject;
using Ninject.Parameters;

namespace Core.Data.NHibernate.Factory
{
    public class RepositoryFactoryInt : IRepositoryFactoryGeneric<int>
    {
        [Inject]
        public IKernel Kernel { get; set; }

        public IRepository<TEntity, int> GetRepository<TEntity>(IUnitOfWork untitOfWork) where TEntity : class, IDomainEntity<int>
        {
            return Kernel.Get<IRepository<TEntity, int>>(new ConstructorArgument("unitOfWork", untitOfWork));
        }

        public T GetManager<T>(IUnitOfWork untitOfWork)
        {
            return Kernel.Get<T>(new ConstructorArgument("unitOfWork", untitOfWork));
        }

        public IProjection GetProjection<TEntity>(string projectionClause, string projectionName, string projectionAlias)
        {
            return Kernel.Get<IProjection>(new ConstructorArgument("type", typeof(TEntity)),
                                           new ConstructorArgument("projectionClause", projectionClause),
                                           new ConstructorArgument("projectionName", projectionName),
                                           new ConstructorArgument("projectionAlias", projectionAlias));
        }

        //never run!!! test it!
        public IProjection GetProjection<TEntity>(string projectionName, string projectionAlias)
        {
            return Kernel.Get<IProjection>(new ConstructorArgument("type", typeof (TEntity)),
                                           new ConstructorArgument("projectionName", projectionName),
                                           new ConstructorArgument("projectionAlias", projectionAlias));
        }

        //never run! test it!
        public IRestriction GetRestriction<TEntity>(string restriction)
        {
            return Kernel.Get<IRestriction>(new ConstructorArgument("type", typeof (TEntity)),
                                            new ConstructorArgument("restriction", restriction));
        }

        public IJoin GetJoin<TEntity>(string requiredJoin)
        {
            return Kernel.Get<IJoin>(new ConstructorArgument("rootEntitiyName", typeof (TEntity).Name),
                                     new ConstructorArgument("requiredJoin", requiredJoin));
        }

        public IOrdination GetOrdination<TEntity>(string ordinationName, string ordinationType)
        {
            return Kernel.Get<IOrdination>(new ConstructorArgument("type", typeof(TEntity)),
                                           new ConstructorArgument("ordinationName", ordinationName),
                                           new ConstructorArgument("ordinationType", ordinationType));
        }
    }

    // questo lo commento perchè lo avevo creato per non dovermi portare dietro sempre un generic.
    // valutare se è possibile levarlo (penso proprio di si)
    // miro
    //public class CoRepositoryFactory : RepositoryFactoryInt, ICoRepositoryFactory
    //{

    //}
}
