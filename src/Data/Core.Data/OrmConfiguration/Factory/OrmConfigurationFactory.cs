using System;
using System.Configuration;
using Core.Data.Exceptions;
using Core.Data.Mappers;

namespace Core.Data.Configuration
{
    public class OrmConfigurationFactory
    {
        private const string DEFAULT_CONFIGRATION_NAME = "orm";

        public static OrmConfigurationCollection GetAllConfirations()
        {
            return GetAllConfirations(DEFAULT_CONFIGRATION_NAME);
        }

        public static OrmConfigurationCollection GetAllConfirations(string sectionName)
        {
            try
            {
                var config = ConfigurationManager.GetSection(sectionName) as OrmConfigurationSectionHandler;
                if (config == null) throw new OrmConfigurationException(String.Format("There is no section [{0}] in the config section! Cannot create OrmConfiguration", sectionName));
                return OrmConfigurationMapper.GetConfigurations(config);
            }
            catch (Exception ex)
            {
                throw new OrmConfigurationException("Unable to retreive OrmConfigurations defined in the config section. Details:" + ex.Message, ex);
            }
        }

        public static IOrmConfiguration GetFirstOrDefaultConfiguration(string sectionName)
        {
            var result = GetAllConfirations(sectionName);
            if (result != null)
                if (result.Count > 0)
                    return result[0];
            throw new OrmConfigurationException("There is no OrmConfigurations defined in the config section.");
        }

        public static IOrmConfiguration GetFirstOrDefaultConfiguration()
        {
            var result = GetAllConfirations(DEFAULT_CONFIGRATION_NAME);
            if (result != null)
                if (result.Count > 0)
                    return result[0];
            throw new OrmConfigurationException("There is no OrmConfigurations defined in the config section.");
        }
    }
}
