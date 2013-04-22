using System.Configuration;

namespace Core.Data.OrmConfiguration
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
