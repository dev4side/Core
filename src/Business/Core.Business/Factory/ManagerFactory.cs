using System;
using Core.Business.Exceptions;
using Core.Business.Manager;
using Core.Common.Constants.NinjectConstants;
using Core.Data.Interfaces.Repository;
using Core.Kernel;
using Ninject.Parameters;

namespace Core.Business.Factory
{
    public class ManagerFactory : IManagerFactory
    {
        /// <summary>
        /// Get the correct manager for the <typeparamref name="TManger"/> type.
        /// </summary>
        /// <typeparam name="TManger">The type of manager.</typeparam>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <returns>The instance of <typeparamref name="TManger"/> type.</returns>
        public TManger GetManager<TManger>(IUnitOfWork unitOfWork) where TManger : IManagerMarker
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            if (!typeof (TManger).IsInterface)
            {
                throw new ManagerException(
                    String.Format(
                        "The MangerFactory must be used to retrive interface types, to allow mocking in tests. You have provided a concrete type: {0}. " +
                        "make sure you just did not just made a typo: GetManager<BodyPartManager> insted of GetManager<IBodyPartManager>",
                        typeof (TManger).FullName));
            }

            return ObjectFactory.Get<TManger>(new ConstructorArgument(NinjectConstructorParameters.UnitOfWorkParameterName, unitOfWork));
        }
    }
}