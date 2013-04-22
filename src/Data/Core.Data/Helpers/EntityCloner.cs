using System;
using System.Linq;
using System.Reflection;
using Core.Data.Attributes;

namespace Core.Data.Helpers
{
    public static class EntityCloner
    {
        public static T CloneAsNewEntity<T>(T entity) where T : new()
        {
            if (entity.Equals(null))
            {
                throw new ArgumentNullException("entity");
            }

            var entityType = typeof(T);
            var entityIstanceToReturn = (T)Activator.CreateInstance(entityType);
            var props = entityType.GetProperties();

            foreach (PropertyInfo propertyInfo in props)
            {
                if (propertyInfo.GetCustomAttributes(true).Contains(new CloneExcludeAttribute()))
                    continue;
                var entityPropertyToFill = entityType.GetProperty(propertyInfo.Name);
                if (entityPropertyToFill == null)
                    continue;

                var pIstancePropertyValue = entityType.InvokeMember(propertyInfo.Name, BindingFlags.GetProperty, null, entity, null);
                entityPropertyToFill.SetValue(entityIstanceToReturn, pIstancePropertyValue, null);

            }
            return entityIstanceToReturn;
        }
    }
}
