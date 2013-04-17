using Ninject.Modules;

namespace Core.Business.NinjectModules
{
    public class MangerFactoryModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IManagerFactory>().To<ManagerFactory>();
        }
    }
}