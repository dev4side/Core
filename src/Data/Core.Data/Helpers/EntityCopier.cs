using System;
using System.Reflection;

namespace Core.Data.Helpers
{
    /// <summary>
    /// Permette di copiare un' entita
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityCopier<T> where T : new()
    {
        /// <summary>
        /// Crea una copia dell' entità, mantenendo tutte le reference alle proprietà.
        /// L' id dell 'enetita NON viene mai copiata.
        /// </summary>
        public static T CopyEntity(T entityToCopy)
        {
            T result = new T();
            var resultType = result.GetType();
            foreach (var property in entityToCopy.GetType().GetProperties())
            {
                if (property.Name == "Id")
                    continue;
                resultType.InvokeMember(property.Name, BindingFlags.SetProperty, null, result, new[] { property.GetValue(entityToCopy, null) });
            }
            return result;
        }
    }
}
