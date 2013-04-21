using System;
using Microsoft.Win32;

namespace Core.Common.Mapper.Registry
{
    ///
    /// taken from http://www.codeproject.com/KB/system/modifyregistry.aspx 
    /// 

    /// <summary>
    /// Reads and Writes Windows's RegistryKeys.
    /// </summary>
    internal static class RegistyHelper
    {
        // fix 32 and or 64bit
        private static RegistryKey BaseRegistryKey
        {
            get
            {
                if (IntPtr.Size == 4)
                    return Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE");
                return Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Wow6432Node");
            }
        }


        internal static object Read(string path, string keyName)
        {
            RegistryKey key = BaseRegistryKey.OpenSubKey(path);
            if (key == null)
            {
                return null;
            }
            // If the RegistryKey exists I get its value
            // or null is returned.
            return key.GetValue(keyName);
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
                throw e;
            }
        }
    }
}
