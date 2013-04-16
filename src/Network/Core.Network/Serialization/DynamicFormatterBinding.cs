using System;
using System.Runtime.Serialization;
using System.Reflection;

namespace Core.Network.Serialization
{
    /// <summary>
    /// Custom binder to help serialized object clone operation.
    /// Binary formatter loads types dynamically form the assemblies in the application directory and does not check loaded assemblies before. 
    /// Instead it tries the find the assemblies which information’s are saved in the serialized file in the startup directory 
    /// (or better AppDomain.CurrentDomain.BaseDirectory). 
    /// </summary>
    public class DynamicFormatterBinding : SerializationBinder 
    { 
        public override Type BindToType(string assemblyName, string typeName) 
        {
            Type tyType = null;
            string sShortAssemblyName = assemblyName.Split(',')[0]; 
            Assembly[] ayAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly ayAssembly in ayAssemblies) 
            { 
                if (sShortAssemblyName == ayAssembly.FullName.Split(',')[0]) 
                { 
                    tyType = ayAssembly.GetType(typeName); 
                    break; 
                } 
            }
            return tyType; 
        } 
    } 
}
