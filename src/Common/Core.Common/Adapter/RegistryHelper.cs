using System;
using Microsoft.Win32;

namespace Core.Common.Adapters
{
    /// <summary>
    /// Helper che permette di ottenere i valori delel chiavi di registro di Dev4Side
    /// </summary>
    internal static class RegistyHelper
    {


        // http://www.codeproject.com/KB/system/modifyregistry.aspx
        // fix 32 and or 64bit
        private static RegistryKey BaseRegistryKey
        {
            get
            {
                if (IntPtr.Size == 4)
                    return Registry.LocalMachine.OpenSubKey("SOFTWARE");
                else return Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Wow6432Node");
            }
        }


        internal static object Read(string path, string keyName)
        {
            RegistryKey key = BaseRegistryKey.OpenSubKey(path);
            if (key == null)
            {
               // Log.Warn(String.Concat(ConfigurationRegistryPath, " doesn't exist"));
                return null;
            }
            try
            {
                // If the RegistryKey exists I get its value
                // or null is returned.
                return key.GetValue(keyName);
            }
            catch (Exception e)
            {
                // Log.Error(String.Format("Cannot read {0} key from registry", keyName),e);
                throw e;
            }
        }

        internal static void Write(string path, string keyName, object obj)
        {
            if (obj == null)
                obj = "";
            RegistryKey key = BaseRegistryKey.OpenSubKey(path, true);
            try
            {
              
                key.SetValue(keyName, obj, RegistryValueKind.String);
            }
            catch (Exception e)
            {
               // Log.Error(String.Format("Cannot write {0} key from registry", keyName), e);
                throw e;
            }
        }
    }
}
