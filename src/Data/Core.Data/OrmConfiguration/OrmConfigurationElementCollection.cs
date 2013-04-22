using System.Configuration;

namespace Core.Data.OrmConfiguration
{
    [ConfigurationCollection(typeof(OrmConfigurationElementCollection), AddItemName = "ormConfiguration")]
    public class OrmConfigurationElementCollection : ConfigurationElementCollection
    {
        public OrmConfigurationElement this[int index]
        {
            get { return base.BaseGet(index) as OrmConfigurationElement; }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new OrmConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OrmConfigurationElement)element).Name;
        }
    }
}
