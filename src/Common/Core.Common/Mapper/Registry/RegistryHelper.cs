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
                var openSubKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE");
                if (openSubKey != null)
                    return openSubKey.OpenSubKey("Wow6432Node");
                else throw new Exception("Cannot read key SOFTWARE");
            }
        }


        internal static object Read(string path, string keyName)
        {
            return Microsoft.Win32.Registry.GetValue(path, keyName, null);
        }

        internal static void Write(string path, string keyName, object obj)
        {
            Microsoft.Win32.Registry.SetValue(path, keyName, obj);
        }
    }
}
