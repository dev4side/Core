using System;
using Core.Business.Exceptions;
using Core.Business.Manager;
using Core.Common.Constants.NinjectConstants;
using Core.Data.Interfaces.Repository;
using Core.Kernel;
using Ninject.Parameters;

namespace Core.Business.Factory
{
    /// <summary>
    /// 
    /// </summary>
    public class ManagerFactory : IManagerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TManger"></typeparam>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public TManger GetManager<TManger>(IUnitOfWork unitOfWork) where TManger : IManagerMarker
        {
            if (!typeof (TManger).IsInterface)
            {
                throw new ManagerException(
                    String.Format(
                        "The MangerFactory must be used to retrive interface types, to allow mocking in tests. You have provided a concrete type: {0}. " +
                        "make sure you just did not just made a typo: GetManager<BodyPartManager> insted of GetManager<IBodyPartManager>",
                        typeof (TManger).FullName));
            }
            
            return ObjectFactory.Get<TManger>(new ConstructorArgument(NinjectConstructorParameters.UNIT_OF_WORK_PARAMENTER_NAME, unitOfWork));
        }
    }
}