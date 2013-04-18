using System;
using System.Collections.Generic;
using System.IO;
using Core.Data.OrmConfiguration.Interfaces;

namespace Core.Data.Objects
{
    public class OrmConfiguration : IOrmConfiguration
    {
        #region IOrmCOnfigration implementation
        
        public string Name { get; private set; }
        public string DatabaseName { get; private set; }
        public FileInfo ConfigurationFile { get; private set; }
        public IList<string> ControlTypes { get; set; }
        public string ConfigurationFileName
        {
            get { return ConfigurationFile.Name; }
        } 
        
        #endregion

        public OrmConfiguration(string name, string configrationFilePath, string databaseName)
        {
            ConfigurationFile = GetFileInfoIfExists(configrationFilePath);
            Name = name;
            DatabaseName = databaseName;
        }

        private FileInfo GetFileInfoIfExists(string configrationFilePath)
        {
            string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            if(!applicationPath.EndsWith(@"\"))
                applicationPath = String.Concat(applicationPath, @"\");
            return new FileInfo(String.Concat(applicationPath, configrationFilePath));
        }
    }
}
