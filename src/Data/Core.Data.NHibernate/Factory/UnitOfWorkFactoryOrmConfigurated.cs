using System;
using Core.Data.Configuration;
using Ninject;
using Ninject.Parameters;

namespace Core.Data.NHibernate.Factory
{

    /// <summary>
    /// Creatore di UnitOfWork tipicizzate in base alla configurazione (IOrmConfiguration)  definita nel config dell' applicazione
    /// </summary>
    public class UnitOfWorkFactoryOrmConfigurated : IUnitOfWorkFactoryOrmConfigurated
    {
        [Inject]
        public IKernel Kernel { get; set; }

        /// <summary>
        /// Crea un unitOfWork specifico per il database corretto
        /// risolve il problema relativo al fatto che le entità sono sparse su database diversi.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IUnitOfWork CreateUnitOfWork(Type type)
        {
            // todo: spostare questo nel costruttore. ninject lo risolve automaticamente
            var ormConfs = Kernel.Get<IOrmConfigurationCollection>();
            var ormConf = ormConfs.TryGetOrmConfigration(type);
            var result = Kernel.Get<IUnitOfWork>(new Parameter(ormConf.ConfigurationFileName, String.Empty, true));
            return result;
        }
    }
}
