using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Data.Helpers
{
    public static class InverseTrueChildsHelper<T>
    {
        public static void AddChildsToEnumerable(IEnumerable<T> childList, T childEntityToAdd, object parentEntity, string propertyNameWhereParentBelongs)
        {
            var propertyInfo = GetPropertyToUse(childList, propertyNameWhereParentBelongs);
            var list = (IList<T>)childList;
            propertyInfo.SetValue(childEntityToAdd, parentEntity, null);
            list.Add(childEntityToAdd);
        }
        
        public static void AddChildsToEnumerable(IEnumerable<T> childList, IEnumerable<T> childEntities, object parentEntity, string propertyNameWhereParentBelongs)
        {
            var propertyInfo = GetPropertyToUse(childList, propertyNameWhereParentBelongs);
            var list = (IList<T>)childList;
            foreach (var entity in childEntities)
            {
                propertyInfo.SetValue(entity, parentEntity, null);
                list.Add(entity);
            }
        }

        public static void RemoveChildFromEnumerable(IEnumerable<T> childList, T childEntityToDelete, string propertyNameWhereParentBelongs)
        {
            var propertyInfo = GetPropertyToUse(childList, propertyNameWhereParentBelongs);
            var list = (IList<T>)childList;
            list.Remove(childEntityToDelete);
        }

        private static PropertyInfo GetPropertyToUse(IEnumerable<T> childList, string propertyNameWhereParentBelongs)
        {
            if (!(childList is IList<T>))
                throw new Exception(String.Format("To use InverseTrueChildsHelper the provided {0} list must implement IList<>", typeof(T)));
            var propertyInfo = typeof(T).GetProperty(propertyNameWhereParentBelongs);
            if (propertyInfo == null)
                throw new Exception(String.Format("The provided propertyNameWhereParentBelongs [{0}] does not exists in {1} entity", propertyNameWhereParentBelongs, typeof(T)));
            return propertyInfo;
        }
    }
}
