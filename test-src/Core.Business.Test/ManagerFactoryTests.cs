using System;
using Core.Business.Exceptions;
using Core.Business.Factory;
using Core.Business.Manager;
using Core.Data.Interfaces.Repository;
using Core.Test.BaseFixtures;
using NUnit.Framework;
using Ninject;
using Ninject.MockingKernel.RhinoMock;
using Rhino.Mocks;

namespace Core.Business.Test
{
    [TestFixture]
    public class ManagerFactoryTests : BaseMockObjectFixture
    {
        protected override IKernel CreateKernel()
        {
            var kernel = new RhinoMocksMockingKernel(GetNinjectSettings());
            kernel.Bind<IManager>().To<ManagerConcrete>();
            return kernel;
        }

        [Test]
        [ExpectedException(typeof(ManagerException))]
        public void GetManager_fails_is_TManger_generics_is_not_an_interface()
        {
            var unitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
            Manager managerTestClass = new ManagerFactory().GetManager<Manager>(unitOfWork);
        }

        [Test]
        public void GetManager_fails_is_does_not_return_a_correct_manager_class()
        {
            var unitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
            var managerTestClass = new ManagerFactory().GetManager<IManager>(unitOfWork);

            Assert.AreEqual(typeof(ManagerConcrete), managerTestClass.GetType());
        }
    }

    /// <summary>
    /// Class for testing errors on manager
    /// </summary>
    public class Manager: IManagerMarker { }

    /// <summary>
    /// Interface for simulate a manager
    /// </summary>
    public interface IManager : IManagerMarker { }

    /// <summary>
    /// Concrete implementation of IManager interface
    /// </summary>
    public class ManagerConcrete : IManager { }
}
