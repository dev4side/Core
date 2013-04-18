using Core.Data.OrmConfiguration.Interfaces;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;

namespace Core.Data.NHibernate.Factory
{
    public class ConfigurationFactory
    {
        public Configuration CreateConfiguration(IOrmConfiguration ormConfiguration)
        {
            var configuration = new Configuration();
            configuration.Configure(ormConfiguration.ConfigurationFile.FullName);

            return configuration;
        }

        public Configuration CreateFluentlyConfiguration<TMappingType>(string connectionStringKey)
        {
            var configuration =
                    Fluently.Configure().Database(MsSqlConfiguration.MsSql2008.ConnectionString(
                        connStr => connStr.FromConnectionStringWithKey(connectionStringKey)))
                        .Mappings(
                                mapping =>
                                mapping.FluentMappings.AddFromAssemblyOf<TMappingType>()).BuildConfiguration();

            return configuration;
        }
    }
}
