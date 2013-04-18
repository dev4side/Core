using System.Configuration;

namespace Core.Data.OrmConfiguration
{
    public class OrmTypeMapElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }
    }
}
