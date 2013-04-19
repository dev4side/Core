using Core.Kernel;
using Core.Test.Constants;
using NUnit.Framework;
using Ninject;
using Ninject.MockingKernel;

namespace Core.Test.BaseFixtures
{
    [Category(NUnitCategories.MOCK_STUB_TEST)]
    public abstract class BaseMockObjectFixture
    {
        protected abstract IKernel CreateKernel();

        protected static NinjectSettings GetNinjectSettings()
        {
            return new NinjectSettings() {InjectNonPublic = true};
        }

        [TearDown]
        protected void ResetKernel()
        {
            ((MockingKernel)ObjectFactory.Kernel).Reset();
        }

        //[TestFixtureSetUp]
        public virtual void SetupKernel()
        {
            ObjectFactory.AssignKernel(CreateKernel());
            ObjectFactory.ResolveDependencies(this);
        }
    }
}
