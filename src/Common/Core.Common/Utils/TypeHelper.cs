using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Common.Utils
{
	public static class TypeHelperExtensions
	{
		public static IEnumerable<Type> GetExtendigConcreteTypes(this Type baseType)
		{
			if (baseType.IsInterface || baseType.IsGenericType)
				throw new NotSupportedException("This method works just with concrete non-generic types");

			var extendingTypes = from assembly in AppDomain.CurrentDomain.GetAssemblies()
								 from type in assembly.GetTypes()
								 where type.BaseType == baseType
								 where type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length > 0
								 where type.IsInterface == false
								 where type.IsAbstract == false
								 select type;
			return extendingTypes;
		}

		public static IEnumerable<Type> GetImplementingConcreteTypes(this Type interfaceType)
		{
			if (!interfaceType.IsInterface)
				throw new NotSupportedException("This method works just with interfaces");

			var implementingTypes = from assembly in AppDomain.CurrentDomain.GetAssemblies()
									from type in assembly.GetTypes()
									from iface in type.GetInterfaces()
									where iface == interfaceType
									where type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length > 0
									where type.IsInterface == false
									where type.IsAbstract == false
									select type;
			return implementingTypes;
		}
	}
}