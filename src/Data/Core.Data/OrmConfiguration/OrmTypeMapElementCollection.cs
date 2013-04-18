using System.Configuration;

namespace Core.Data.OrmConfiguration
{
    [ConfigurationCollection(typeof(OrmTypeMapElementCollection), AddItemName = "type")]
    public class OrmTypeMapElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new OrmTypeMapElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OrmTypeMapElement)element).Name;
        }
    }
}
