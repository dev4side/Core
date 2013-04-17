using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Services.Resolving
{
    public class WcfTypeResolverManager
    {
		public static List<Type> LoadKnowDtoTypesFromAssembly(Assembly assembly)
		{ 
			return LoadKnowDtoTypesFromAssembly(assembly, null);
		}

        public static List<Type> LoadKnowDtoTypesFromAssembly(Assembly assembly, string rootNamespace)
        {
            Type[] candidateTypes = assembly.GetTypes();
			List<Type> filteredTypes = new List<Type>();

            foreach (var candidateType in candidateTypes)
            {
                if (candidateType.Namespace != null)
                    if ( (rootNamespace == null) || candidateType.Namespace.StartsWith(rootNamespace))
                        if (candidateType.IsClass)
							filteredTypes.Add(candidateType);

            }
			return filteredTypes;
        }
    }
}
