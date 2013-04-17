using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Core.Data.Configuration
{

    [ConfigurationCollection(typeof(OrmConfigurationElementCollection), AddItemName = "ormConfiguration")]
    public class OrmConfigurationElementCollection : ConfigurationElementCollection
    {


        public OrmConfigurationElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as OrmConfigurationElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                    base.BaseRemoveAt(index);
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
