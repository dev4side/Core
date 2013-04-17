using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Data.Configuration;

namespace Core.Data.Mappers
{
    internal static class OrmConfigurationMapper
    {
        internal static OrmConfigurationCollection GetConfigurations(OrmConfigurationSectionHandler configurationSectionHandler)
        {
            OrmConfigurationCollection result = new OrmConfigurationCollection();
            result.AddRange(from OrmConfigurationElement ormConfigurationElement in configurationSectionHandler.OrmConfigurations
                            select GetConfiguration(ormConfigurationElement));
            return result;
        }

        internal static IOrmConfiguration GetConfiguration(OrmConfigurationElement ormConfigurationElement)
        {
            var result =  new OrmConfiguration(ormConfigurationElement.Name, ormConfigurationElement.Path, ormConfigurationElement.DatabaseName)
                              {
                                  ControlTypes = GetSupportedTypes(ormConfigurationElement)
                              };
            return result;
        }

        internal static IList<string> GetSupportedTypes(OrmConfigurationElement ormConfigurationElement)
        {
            return (from OrmTypeMapElement suypportedType in ormConfigurationElement.IncludeTypes
                    select suypportedType.Name).ToList();
        }
    }
}
