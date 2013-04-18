using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data.Objects.Collections;
using Core.Data.OrmConfiguration;
using Core.Data.OrmConfiguration.Interfaces;

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
            var result = new Core.Data.Objects.OrmConfiguration(
                ormConfigurationElement.Name, ormConfigurationElement.Path, ormConfigurationElement.DatabaseName)
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
