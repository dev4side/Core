using System;
using Core.Business.Factory;
using Core.Data.Interfaces.Factory;
using Core.Kernel;
using Core.Test.Constants;
using NUnit.Framework;
using Ninject;
using Ninject.MockingKernel;

namespace Core.Test.BaseFixtures
{
    /// <summary>
    /// Questa classe base viene utilizzata nei tests in cui è neccessario testare il codice SQL che veine generato dall' ORM
    /// I test che ereditano la classe avranno un database nuovo nuovo con dati preparati su cui eseguire le query
    /// </summary>
    [NUnit.Framework.Category(NUnitCategories.NHIBERNATE_SLOW_TEST)]
    public abstract class BaseDbDependentFixture
    {
        #region Services

        [Inject]
        public IUnitOfWorkFactory UnitOfWorkfactory { get; set; }

        [Inject]
        public IRepositoryFactoryGeneric<Guid> RepositoryFactory { get; set; }

        [Inject]
        public IManagerFactory ManagerFactory { get; set; }

        #endregion

        /// <summary>
        /// Indica se è neccessario ricreare il db per ogni test.
        /// </summary>
        public virtual bool ReCreateDbForEachTest
        {
            get { return true; }
        }

        /// <summary>
        /// Indica se è neccessario ricreare i dummy data per ogni test.
        /// </summary>
        public virtual bool CreateDummyDataForEachTest
        {
            get { return true; }
        }

        public abstract void CreateDummyData();

        protected abstract IKernel CreateKernel();
       
        /// <summary>
        /// Parte prima di ciascun test 
        /// </summary>
        [SetUp]
        public virtual void Setup()
        {
            if (ReCreateDbForEachTest)
            {
                CreateSchema();
            }

            if(CreateDummyDataForEachTest)
            {
                CreateDummyData();
            }
        }
        
        /// <summary>
        /// Parte 1 volta, appena la classe viene creata
        /// </summary>
        [TestFixtureSetUp]
        public virtual void SetupFixture()
        {
            if (ReCreateDbForEachTest)
            {
                CreateSchema();
            }

            ObjectFactory.AssignKernel(CreateKernel());
            ObjectFactory.ResolveDependencies(this);
        }

        [TearDown]
        public virtual void Cleanup()
        {
            ((MockingKernel)ObjectFactory.Kernel).Reset();
        }

        /// <summary>
        /// Parte 1 volta, quando la classe viene distrutta
        /// </summary>
        [TestFixtureTearDown]
        public virtual void TearDownFixture() { }

        protected static void CreateSchema()
        {
        //    // cancello il data base e lo ricreo novo
        //    try
        //    {
        //        var cfg = new NHibernate.Cfg.Configuration();
        //        cfg.Configure(OrmConfigurationFactory.GetFirstOrDefaultConfiguration().ConfigurationFile.FullName);
        //        cfg.BuildSessionFactory();
        //        var schemaExport = new SchemaExport(cfg);
        //        schemaExport.Drop(false, true);
        //        schemaExport.Create(false, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Cannot create database tables! more info" + ex.Message);
        //    }
        }

        protected static NinjectSettings GetNinjectSettings()
        {
            return new NinjectSettings() { InjectNonPublic = true };
        }
    }
}