using Core.Business.Factory;
using Ninject.Modules;

namespace Core.Business.NinjectModules
{
    /// <summary>
    /// Ninject module for bind business interfaces to concrete implementations
    /// </summary>
    public class MangerFactoryModule : NinjectModule
    {
        /// <summary>
        /// 
        /// </summary>
        public override void Load()
        {
            Bind<IManagerFactory>().To<ManagerFactory>();
        }
    }
}