using System;
using Core.Common.Constants.NinjectConstants;
using Core.Data;
using Core.Kernel;
using Ninject.Parameters;

namespace Core.Business
{
    public class ManagerFactory : IManagerFactory
    {
        public TManger GetManager<TManger>(IUnitOfWork unitOfWork) where TManger : IMangerMarker
        {
            if (!typeof(TManger).IsInterface)
                throw new Exception(String.Format("the MangerFactory must be used to retrive interface types," +
                                                  " to allow mocking in tests. You have provided a concrete type: {0}." +
                                                  "make sure you just did not just made a typo: GetManager<BodyPartManager> insted of GetManager<IBodyPartManager>", typeof(TManger).FullName));

            return ObjectFactory.Get<TManger>(new ConstructorArgument(NinjectConstructorParameters.UNIT_OF_WORK_PARAMENTER_NAME, unitOfWork));
        }
    }
}