using System;
using Core.Common.Converters;
using Core.Common.Mapper.Registry;
using Core.Common.Mappers;

namespace Core.Common.Mapper
{
    /// <summary>
    /// Loads and saves dto's that are mapped to registry values throught MapToRegistryKeyAttributes
    /// </summary>
    /// <typeparam name="TRegistryObject"></typeparam>
    public class RegistryMapper<TRegistryObject> where TRegistryObject : class
    {
        /// <summary>
        /// Returns T with mapped properties throught MapToRegistryKeyAttributes
        /// </summary>
        /// <returns></returns>
        public static TRegistryObject Get()
        {
            Type type = typeof(TRegistryObject);
            var result = Activator.CreateInstance(type);

            var customAttributes = type.GetCustomAttributes(false);
            foreach (var customAttribute in customAttributes)
            {
                if (customAttribute is MapToRegistryKeyAttribute)
                {
                    var properties = type.GetProperties();
                    foreach (var propertyInfo in properties)
                    {
                        var attributes = propertyInfo.GetCustomAttributes(false);
                        foreach (var attribute in attributes)
                        {
                            if (attribute is MapToRegistryKeyPropertyAttribute)
                            {
                                var registryKeyProperty = attribute as MapToRegistryKeyPropertyAttribute;
                                object registryKeyValue = RegistyHelper.Read(registryKeyProperty.Path, registryKeyProperty.Key);
                                if (registryKeyValue == null)
                                    throw new RegistryObjectMappingException(
                                        String.Format("no value in path:{0} and key:{1} in the windows registry. Check how you have mapped" +
                                                      "property {2} of type {3}",
                                                      registryKeyProperty.Path, registryKeyProperty.Key, propertyInfo.Name, result.GetType()));
                                propertyInfo.SetValue(result, Converter.ConvertToPropertyType(registryKeyValue, propertyInfo.PropertyType, registryKeyProperty.BooleanConversion), null);
                            }
                        }
                    }
                }
            }

            return result as TRegistryObject;
        }

        /// <summary>
        /// Saves T by writing properties marked with throught MapToRegistryKeyAttributes into Registry
        /// </summary>
        /// <param name="registryObject"></param>
        public static void Set(TRegistryObject registryObject)
        {
            Type type = typeof(TRegistryObject);
            var customAttributes = type.GetCustomAttributes(false);
            foreach (var customAttribute in customAttributes)
            {
                if (customAttribute is MapToRegistryKeyAttribute)
                {
                    var properties = type.GetProperties();
                    foreach (var propertyInfo in properties)
                    {
                        if (propertyInfo.CanRead)
                        {

                            object propertyValue = propertyInfo.GetValue(registryObject, null);
                            if (propertyValue != null)
                            {
                                var attributes = propertyInfo.GetCustomAttributes(false);
                                foreach (var attribute in attributes)
                                {
                                    if (attribute is MapToRegistryKeyPropertyAttribute)
                                    {
                                        var registryKeyProperty = attribute as MapToRegistryKeyPropertyAttribute;
                                        RegistyHelper.Write(registryKeyProperty.Path, registryKeyProperty.Key, Converter.TryInverseBooleanConvertion(propertyValue, registryKeyProperty.BooleanConversion));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        } 
    }
}