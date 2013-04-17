using System.Configuration;

namespace Core.Data.Configuration
{
    public class OrmConfigurationSectionHandler : ConfigurationSection
    {
        [ConfigurationProperty("ormConfigurations")]
        public OrmConfigurationElementCollection OrmConfigurations
        {
            get
            {
                return this["ormConfigurations"] as OrmConfigurationElementCollection;
            }
        }

    }
}
