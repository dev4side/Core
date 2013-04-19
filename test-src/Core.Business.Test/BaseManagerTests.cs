using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Business.Manager;
using Core.Data.BaseEntity;
using Core.Data.Interfaces.Entity;
using Core.Data.Interfaces.Repository;
using Core.Test.BaseFixtures;
using NUnit.Framework;
using Ninject;
using Ninject.MockingKernel.RhinoMock;

namespace Core.Business.Test
{
    [TestFixture]
    public class BaseManagerTests : BaseMockObjectFixture
    {
        protected override IKernel CreateKernel()
        {
            var kernel = new RhinoMocksMockingKernel(GetNinjectSettings());
            kernel.Bind<IManager>().To<ManagerConcrete>();
            return kernel;
        }

        [Test]
        public void Test()
        {
        
        }
    }

    /// <summary>
    /// Domain class for test manager
    /// </summary>
    public class ManagerEntity : IDomainEntity<Guid>
    {
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Interface for simulate a manager
    /// </summary>
    public interface IBaseManagerTest : IBaseManager<ManagerEntity> { }

    /// <summary>
    /// Concrete implementation of IManager interface
    /// </summary>
    public class BaseManagerTest : BaseManager<ManagerEntity, Guid>, IManager 
    {    
        public BaseManagerTest(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
