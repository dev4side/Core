using System;
using System.Collections.Generic;

namespace Core.Common.Extension
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetAllBaseTypes(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}
