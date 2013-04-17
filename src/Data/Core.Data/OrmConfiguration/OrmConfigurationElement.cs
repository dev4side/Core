using System;
using System.Configuration;

namespace Core.Data.Configuration
{
    public class OrmConfigurationElement  : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return this["path"] as string;
            }
        }

        [ConfigurationProperty("databaseName", IsRequired = false, DefaultValue = "")]
        public string DatabaseName
        {
            get
            {
                return this["databaseName"] as string;
            }
        }

        [ConfigurationProperty("includeTypes")]
        public OrmTypeMapElementCollection IncludeTypes
        {
            get
            {
                return this["includeTypes"] as OrmTypeMapElementCollection;
            }
        }
    }
}
