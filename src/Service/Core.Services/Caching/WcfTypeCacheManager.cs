using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Common.Cache;
using Core.Kernel;
using Core.Services.Resolving;

namespace Core.Services.Caching
{
    public class WcfTypeCacheManager
    {
        public static IEnumerable<Type> LoadKnownDtoTypesFromCacheOrFromAssembly(Assembly assembly, string rootNamespace)
        {
            var currentContext = ObjectFactory.Get<ICache>();

            var rootNamespaceCached = currentContext.GetFromCache(rootNamespace);

            if (rootNamespaceCached == null)
            {
                rootNamespaceCached = WcfTypeResolverManager.LoadKnowDtoTypesFromAssembly(assembly, rootNamespace);
                DateTime expiration = DateTime.Now.AddDays(1);
                currentContext.AddToCache(rootNamespace, rootNamespaceCached, expiration, TimeSpan.Zero);
            }

            return rootNamespaceCached as List<Type>;
        }
    }
}
