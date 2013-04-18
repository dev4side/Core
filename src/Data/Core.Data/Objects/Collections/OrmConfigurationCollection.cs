using System;
using System.Collections.Generic;
using Core.Data.Exceptions;
using Core.Data.OrmConfiguration.Interfaces;

namespace Core.Data.Objects.Collections
{
    public class OrmConfigurationCollection : List<IOrmConfiguration>, IOrmConfigurationCollection
    {
        internal OrmConfigurationCollection()
        {
                
        }

       public IOrmConfiguration TryGetOrmConfigration(Type type)
       {
           var typeAsStrig = type.ToString();
           foreach (IOrmConfiguration ormConfiguration in this)
           {
               foreach (var compatibleType in ormConfiguration.ControlTypes)
               {
                   if (compatibleType.Equals(typeAsStrig))
                       return ormConfiguration;
               }
           }
           throw new OrmConfigurationException(String.Format("The [{0}] type has no defined configuration in the current config. " +
                                    "Please check that your config contains a defined rtype under <ormConfiguration><includeTypes> <type name=\"NAME\"/>", typeAsStrig));
       }
    }

   
}
