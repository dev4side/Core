using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Core.Services.Conversion
{

	public static partial class ExtensionMethods
	{
		public static Dictionary<Type,PropertyInfo[]> propertyCache = new Dictionary<Type,PropertyInfo[]>();

		public static PropertyInfo[] GetPropertiesWithInheritance(this Type type)
		{
			if(propertyCache.ContainsKey(type))
				return propertyCache[type];

			if (type.IsInterface)
			{
				var propertyInfos = new List<PropertyInfo>();
				var considered = new List<Type>();
				var queue = new Queue<Type>();
				considered.Add(type);
				queue.Enqueue(type);

				while (queue.Count > 0)
				{
					var subType = queue.Dequeue();
					foreach (var subInterface in subType.GetInterfaces())
					{
						if (!considered.Contains(subInterface))
						{
							considered.Add(subInterface);
							queue.Enqueue(subInterface);
						}
					}

					var subTypeProperties = subType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance).Where(p => !propertyInfos.Contains(p));

					propertyInfos.InsertRange(0, subTypeProperties);
				}

				return propertyCache[type] = propertyInfos.ToArray();
			}
			else				
				return propertyCache[type] = type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
		}
	}


	public static class ListToDataTableConverter
	{

		public static DataTable GetDataTable<TGenericType>(IList<TGenericType> list)
		{
			//ottengo il tipo..
			Type entityType = typeof(TGenericType);

			//ottengo le proprietà del tipo..
			PropertyInfo[] entityProperties = entityType.GetPropertiesWithInheritance();

			//costruisco una table con le proprietà del tipo..
			DataTable table = new DataTable(entityType.Name);
			foreach(var prop in entityType.GetPropertiesWithInheritance())
				table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

			//ottimizzo..
			int count = table.Columns.Count;

			//fetch..
			foreach(var obj in list)
			{
				DataRow row = table.NewRow();
				for(int i=0; i<count; i++)
				{
					var prop  = entityProperties[i];
					var type  = prop.PropertyType;
					var value = prop.GetValue(obj, null);

					if( type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)) )
						row[i] = value ?? Activator.CreateInstance(Nullable.GetUnderlyingType(type));
					else
						row[i] = value;
				}
				table.Rows.Add(row);
			}
			table.AcceptChanges();

			return table;
		}

	}
}
