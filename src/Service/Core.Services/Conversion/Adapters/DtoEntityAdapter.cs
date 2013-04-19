using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Common.Constants;

namespace Core.Services.Conversion.Adapters
{
    // refactor this!!!!!
    public class DtoEntityAdapter<TEntity, TDto, TIDto> where TDto : TIDto
    {

        #region helper class DtoEntityMappingProperties
        class  DtoEntityMappingProperties
        {
            public PropertyInfo DtoPropertyInfo { get; set; }
            public MapToEntityProperty DtoMapToEntityProperty { get; set; }
            public PropertyInfo EntityPropertyInfo { get; set; }

            public string DtoRegisterdResolverName 
            {
                get 
                {
                    return String.Format("{0}.{1}", DtoPropertyInfo.ReflectedType.Name, DtoPropertyInfo.Name);

                }
            }

            public string EntityRegisterdResolverName
            {
                get
                {
                    return String.Format("{0}.{1}", EntityPropertyInfo.ReflectedType.Name, EntityPropertyInfo.Name);

                }
            }

            public DtoEntityMappingProperties (MapToEntityProperty dtoMapToentityProperty, PropertyInfo dtoPropertyInfo,  PropertyInfo entityPropertyInfo)
	        {
                DtoPropertyInfo = dtoPropertyInfo;
                DtoMapToEntityProperty = dtoMapToentityProperty;
                EntityPropertyInfo = entityPropertyInfo;
	        }
        }
        #endregion

        #region fields
        Dictionary<Type, IList<DtoEntityMappingProperties>> _knowMappings = new Dictionary<Type,IList<DtoEntityMappingProperties>>();

        // tutti i resolver registrati tramite il TipoDto.ProperitaDto e la relativa funcion
        Dictionary<string, Func<object, object>> _knowEntityResolverMappings = new Dictionary<string, Func<object, object>>();
        // contiene tutte i resolver registrati con Entita.ProperitaEntita
        IList<string> _knowEntityResolver = new List<string>();
        #endregion

        public DtoEntityAdapter()
        {
            CheckIfIsEntityAndDtoAreValidMappings();
        }


        #region public Entity builders
        /// <summary>
        /// Crea un nuova entita da un dto, resettando quindi l'id a 0
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TEntity CreateNewEntityFromDto(TIDto dto)
        {
            return (TEntity)RecursivelyCreateEntityFromDto(dto, true, typeof(TEntity));
        }

        /// <summary>
        /// Crea un entita da un dto, mantenendo l'id se definito nel dto
        /// </summary>
        public TEntity CreateEntityFromDto(TIDto dto)
        {
            return (TEntity)RecursivelyCreateEntityFromDto(dto, false, typeof(TEntity));
        }

        /// <summary>
        /// Crea una collezione di entita da una collezione di dto, mantenendo l'id se definito nel dto
        /// </summary>
        public IList<TEntity> CreateEntityFromDto(IList<TIDto> entityDto)
        {
            return entityDto.Select(CreateEntityFromDto).ToList();
        }


        public TEntity UpdateEntityFromDto(TIDto dto, TEntity entityToUpdate)
        {
            return (TEntity)RecursivelyCreateEntityFromDto(dto, false, typeof(TEntity), entityToUpdate);

        }
        #endregion


        #region public Dto builder

        /// <summary>
        /// Crea un dto da un' entità, reseetando l'id a 0 
        /// </summary>
        public TIDto CreateNewDtoFromEntity(TEntity entity)
        {
            return (TIDto)RecursivelyCreateDtoFromEntity(entity, true, typeof(TDto));
        }

        /// <summary>
        /// Crea un dto da un' entità, mantenendo l'id se definito nell' entità
        /// </summary>
        public TIDto CreateDtoFromEntity(TEntity entity)
        {
            try
            {
                return (TIDto)RecursivelyCreateDtoFromEntity(entity, false, typeof(TDto));
            }
            catch (ObjectDisposedException)
            {
				throw new DtoEntityAdapterConversionException(entity.GetType(), typeof(TDto), ExceptionMessagesConstants.UNIT_OF_WORK_DISPOSED_MESSAGE);
            }
            
        }

        public IList<TIDto> CreateDtoFromEntity(IEnumerable<TEntity> entities)
        {
            try
            {
                return entities.Select(CreateDtoFromEntity).ToList();
            }
            catch (ObjectDisposedException)
            {
                throw new DtoEntityAdapterConversionException(typeof(TEntity), typeof(TDto), ExceptionMessagesConstants.UNIT_OF_WORK_DISPOSED_MESSAGE);
            }
        }
        #endregion

        private object RecursivelyCreateDtoFromEntity(object entityToConvertInDto, bool ignoreId, Type convertToDtoTypeAsResult)
        {
            if (entityToConvertInDto == null)
                return null;
            RegisterKnowMapping(convertToDtoTypeAsResult, entityToConvertInDto.GetType());
            var result = Activator.CreateInstance(convertToDtoTypeAsResult);
            foreach (var mapping in GetRegistedMapping(convertToDtoTypeAsResult))
            {
                if (mapping.DtoMapToEntityProperty.EntityPropertyNameToMap.Equals("Id"))
                    if (ignoreId)
                        continue;

                if (!mapping.DtoMapToEntityProperty.ConvertToDto)
                    continue;
                try
                {
                    // prendo l' istanza presente nella proprietà corrente dell' entità da convertire
                    var pIstanceEntityPropertyValue = mapping.EntityPropertyInfo.GetValue(entityToConvertInDto, null);
                    //var pIstancePropertyValue = entityToConvertInDto.GetType().InvokeMember(mapping.DtoMapToEntityProperty.EntityPropertyNameToMap, BindingFlags.GetProperty, null, entityToConvertInDto, null);
                    if (pIstanceEntityPropertyValue == null)
                        continue;
                    var pIstanceEntityPropertyValueType = pIstanceEntityPropertyValue.GetType();
                    //... se è un generic... ed IList<IDto> che indica una relazione padre figlio 1;n ... vediamo se IList<T> oppure semplicmente un generic
                    if (IsAnIListOfGenericSpecification(pIstanceEntityPropertyValueType))
                    {
                        var dtoAsList = CreateIstanceFromGenericListType(mapping.DtoPropertyInfo.PropertyType);
                        // e ci aggiungo i figli
                        var istanceAsList = pIstanceEntityPropertyValue as IEnumerable;
                        foreach (var item in istanceAsList)
                        {
                            var dtoToAddToCollection = RecursivelyCreateDtoFromEntity(item, ignoreId, mapping.DtoMapToEntityProperty.GetTypeToUseFromMapToEntityProperty(convertToDtoTypeAsResult.Assembly));
                            dtoAsList.Add(dtoToAddToCollection);
                        }
                        mapping.DtoPropertyInfo.SetValue(result, dtoAsList, null);
                    }

                    else
                    {
                        if (pIstanceEntityPropertyValueType.IsGenericType)
                            ThrowEntityToDtoMappingException(String.Format("Entity property {0} is a generic type of {1}. Only IList<T> generics are supported.",
                                mapping.DtoMapToEntityProperty.EntityPropertyNameToMap, pIstanceEntityPropertyValueType.Name));


                        if (mapping.DtoMapToEntityProperty.HasEntityPropertyToInvoke())
                        {
                            object propertyValue = pIstanceEntityPropertyValueType.InvokeMember(mapping.DtoMapToEntityProperty.EntityPropertyToInvoke, BindingFlags.GetProperty, null, pIstanceEntityPropertyValue, null);
                            mapping.DtoPropertyInfo.SetValue(result, propertyValue, null);
                        }
                        else 
                        {
                            var convertedValue = ConverterManager.ConvertToDto(pIstanceEntityPropertyValue, mapping.DtoMapToEntityProperty.ConvertToDtoMechanism, mapping.DtoPropertyInfo.PropertyType);
                            mapping.DtoPropertyInfo.SetValue(result, convertedValue, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ThrowEntityToDtoMappingException(String.Format("cannot map Entity {0} into DTO {1}. A generic error occured!. Check MapToEntityProperty attribute of your Dto. Details: {2}",
                        mapping.DtoMapToEntityProperty.EntityPropertyNameToMap, mapping.DtoPropertyInfo.Name, ex.Message));
                }
            }

            return result;

        }

        private void RegisterKnowMapping(Type dtoType, Type entityType)
        {
            if (!_knowMappings.ContainsKey(dtoType))
                _knowMappings[dtoType] = GetDtoEntityMappingProperties(dtoType, entityType);
        }

        private IList<DtoEntityMappingProperties> GetRegistedMapping(Type dtoTypeAsKey)
        {
            return _knowMappings[dtoTypeAsKey];
        }

        /// <summary>
        /// Crea un' entita dato un Dto
        /// </summary>
        /// <param name="dtoToConvertInEntity">il dto che deve essere convertito in entita</param>
        /// <param name="ignoreId">indica se durante il mapping di ignorare la proprietà ID. in caso di true, non viene copiato l'id del dto nell' id dell' entità</param>
        /// <param name="convertToEntityTypeAsResult">il tipo di ritorno dell' entità</param>
        /// <param name="objectFromRepositoryToUpdate">Indica l' entità su cui si vuole fare l'update, invece di crearne una nuova.</param>
        /// <returns>Torna l'entità mappata secondo il dto. Se la prop objectFromRepositoryToUpdate è stata specificata,
        ///  allora torna objectFromRepositoryToUpdate con tutte le proprietà aggiornate come indicato nel dto.
        /// </returns>
        private object RecursivelyCreateEntityFromDto(object dtoToConvertInEntity, bool ignoreId, Type convertToEntityTypeAsResult, object objectFromRepositoryToUpdate = null)
        {
            if (dtoToConvertInEntity == null)
                return null;

            Type dtoType = dtoToConvertInEntity.GetType();
            RegisterKnowMapping(dtoType, convertToEntityTypeAsResult);


            object result;
            if(objectFromRepositoryToUpdate == null)
                 result = Activator.CreateInstance(convertToEntityTypeAsResult);
            else result = objectFromRepositoryToUpdate;

            foreach (var mapping in GetRegistedMapping(dtoType))
            {
                if (mapping.DtoMapToEntityProperty.EntityPropertyNameToMap.Equals("Id"))
                    if (ignoreId)
                        continue;
                if (!mapping.DtoMapToEntityProperty.ConvertToEntity)
                    continue;

                try
                {
                    // prendo l' istanza presente nella proprietà corrente del dto da convertire
                    // entityToConvertInDto.GetType().InvokeMember(mapping.DtoMapToEntityProperty.EntityPropertyNameToMap, BindingFlags.GetProperty, null, entityToConvertInDto, null);
                    var pIstanceDtoPropertyValue = mapping.DtoPropertyInfo.GetValue(dtoToConvertInEntity, null);
                    if (pIstanceDtoPropertyValue == null)
                        continue;
                    var pIstanceDtoPropertyValueType = pIstanceDtoPropertyValue.GetType();
                    // se è un IList<T>

                    if (mapping.DtoMapToEntityProperty.HasConvertChildsUsingConcreteClassSpecified())
                    {
                        IList entityAsList = CreateIstanceFromGenericListType(mapping.EntityPropertyInfo.PropertyType);
                        IEnumerable dtoAsList = pIstanceDtoPropertyValue as IEnumerable;
                        foreach (var dto in dtoAsList)
                        {
                            var entiityToAddToCollection = RecursivelyCreateEntityFromDto(dto, ignoreId, entityAsList.GetType().GetGenericArguments()[0]);
                            entityAsList.Add(entiityToAddToCollection);
                        }
                        if (mapping.DtoMapToEntityProperty.HasAddChildsToEntityUsingMethodSpecified())
                            mapping.EntityPropertyInfo.DeclaringType.InvokeMember(mapping.DtoMapToEntityProperty.EntityMethodToInvokeToAddChilds, BindingFlags.InvokeMethod, null, result, new[] {entityAsList});

                        else mapping.EntityPropertyInfo.SetValue(result, entityAsList, null);
                    }
                    else 
                    {
                        if(pIstanceDtoPropertyValueType.IsGenericType)
                            ThrowDtoToEntityMappingException(String.Format("Dto property {0} is a generic type of {1}. Only IList<T> generics are supported.",
                                mapping.DtoMapToEntityProperty.EntityPropertyNameToMap, pIstanceDtoPropertyValueType.Name));

                        // non ha miolto senso usare i resolvers se ha indicato quelsta prop
                        // deve piuttosto controllare se è presente l' associazione Entity.PropEntity con Dto.PropDto
                        if (_knowEntityResolver.Contains(mapping.EntityRegisterdResolverName) && _knowEntityResolverMappings.ContainsKey(mapping.DtoRegisterdResolverName))
                        //if (mapping.DtoMapToEntityProperty.HasEntityPropertyToInvoke())
                        {
                            var aa = GetEntityUsingKnownResolver(dtoToConvertInEntity, mapping);
                            mapping.EntityPropertyInfo.SetValue(result, aa, null);
                        }
                        else 
                        {
                            // vediamo se cè gia un resolver per questa proprietà.
                            // nel caso è presente, salto questo passaggio, perche verra fatto dal resolver
                            if (!_knowEntityResolver.Contains(mapping.EntityRegisterdResolverName))
                            {
                                var pInstanceDto = mapping.DtoPropertyInfo.GetValue(dtoToConvertInEntity, null);
                                var convertedValue = ConverterManager.ConvertToEntity(pInstanceDto, mapping.DtoMapToEntityProperty.ConvertToDtoMechanism, mapping.EntityPropertyInfo.PropertyType);
                                mapping.EntityPropertyInfo.SetValue(result, convertedValue, null);
                            }

                        } 
                    }
                }
                catch (Exception ex)
                {
                    ThrowDtoToEntityMappingException(String.Format("cannot map DTO {0} into Entity {1}. Check MapToEntityProperty attribute of your Dto. Details: {2}",
                        mapping.DtoPropertyInfo.Name,  mapping.DtoMapToEntityProperty.EntityPropertyNameToMap, ex.Message));
                }
            }

            return result;
        }

        private object GetEntityUsingKnownResolver(object dtoToConvertInEntity, DtoEntityMappingProperties mapping)
        {
            var pInstanceDto = mapping.DtoPropertyInfo.GetValue(dtoToConvertInEntity, null);
            string key = mapping.DtoRegisterdResolverName;
            if (!_knowEntityResolverMappings.ContainsKey(key))
                ThrowDtoToEntityMappingException(String.Format("Please add a resolver with key [{0}] to instruct the adapter in how to create/obtain an instance of type {1} from dto's property {2}",
                    key, mapping.EntityPropertyInfo.PropertyType.Name, mapping.DtoPropertyInfo.Name));
            return _knowEntityResolverMappings[key](pInstanceDto);

        }

        private IList CreateIstanceFromGenericListType(Type GenericListType)
        {
            var genericStrongTypedList = CreateGenericListUsingGenericsSpecification(GenericListType);
            var result = Activator.CreateInstance(genericStrongTypedList);
            return (IList)result;
        }

        private static void ThrowEntityToDtoMappingException(string message)
        {
            throw new Exception(String.Format("An Exception has been raised when trying to convert an entity of type [{0}] into a dto of type [{1}]. The error is: {2}", typeof(TEntity), typeof(TDto), message));
        }

        private static void ThrowDtoToEntityMappingException(string message)
        {
            throw new Exception(String.Format("An Exception has been raised when trying to convert an dto of type [{0}] into an entity of type [{1}]. The error is: {2}",typeof(TDto),typeof(TEntity), message));
        }
      
        private static bool IsAnIListOfGenericSpecification(Type pIstancePropertyValueType)
        {
            //creo un oggetto IList<T> di modo da poter valutare se il mio Generic<T> è un IList<T> 
            if (!pIstancePropertyValueType.IsGenericType)
                return false;
            var specific = CreateGenericIListUsingGenericsSpecification(pIstancePropertyValueType);
            return DoesTypeContainInterface(pIstancePropertyValueType, specific);
        }

        private static MapToEntityProperty GetMapToEntityProperyAttributeIfPresent(PropertyInfo propertyInfo)
        {
           var propertyAttributes = propertyInfo.GetCustomAttributes(typeof(MapToEntityProperty), true);
           if (propertyAttributes.Count() != 0)
           {
               var mapToEntityProperty = propertyAttributes[0] as MapToEntityProperty;
               if (mapToEntityProperty != null)
                   return mapToEntityProperty;
           }
           return null;
        }

        private static Type CreateGenericListUsingGenericsSpecification(Type tToUse)
        {
            if (!tToUse.IsGenericType)
                throw new Exception("il dto deve essere un generic!!!");
            var specification = tToUse.GetGenericArguments()[0];
            var dtoAsGenericList = typeof(List<>);
            return dtoAsGenericList.MakeGenericType(specification);
        }

        private static Type CreateGenericIListUsingGenericsSpecification(Type genericType)
        {
            if (!genericType.IsGenericType)
                throw new MissingMemberException("Its supposed to be a geeric!!!");
            Type itemType = genericType.GetGenericArguments()[0];
            var generic = typeof(IList<>);
            return generic.MakeGenericType(itemType);
        }

        private static bool DoesTypeContainInterface(Type typeTocheckAgainstInterface, Type interfaceToFind)
        {
            foreach (var candidateInterface in typeTocheckAgainstInterface.GetInterfaces())
            {
                if (candidateInterface == interfaceToFind)
                    return true;
            }
            return false;
        }

        private static void CheckIfIsEntityAndDtoAreValidMappings()
        {
#if DEBUG
            // controllo che il DTO contiene l' attributo MapToEntity
            var typeAsString = typeof (TEntity).ToString();
            var candidatesAttributes = typeof(TDto).GetCustomAttributes(typeof(MapToEntity), true);
            
            foreach (var candidatesAttribute in candidatesAttributes)
            {
                var att = candidatesAttribute as MapToEntity;
                if (att != null)
                    if (att.EntityName.Equals(typeAsString))
                        return;
            }
            
            throw new DtoEntityAdapterConversionException(typeof(TEntity), typeof(TDto),
                String.Format("Cannot map {0} with dto {1}. Unable to locate the MapToEntity attribute in the DTO", typeof(TEntity), typeof(TDto)));
#endif
        }

        private static IList<DtoEntityMappingProperties> GetDtoEntityMappingProperties(Type dtoType, Type entityType)
        {
            Type entityTypeToUse = ExtractOriginalTypeIfTypeIsProxyClass(entityType);
            IList<DtoEntityMappingProperties> result = new List<DtoEntityMappingProperties>();
            try
            {
                foreach (PropertyInfo dtoPropertyInfo in dtoType.GetProperties())
                {
                    MapToEntityProperty mapToEntityProperty = GetMapToEntityProperyAttributeIfPresent(dtoPropertyInfo);
                    if (mapToEntityProperty != null)
                    {
                        PropertyInfo entityPropertyInfo = entityTypeToUse.GetProperty(mapToEntityProperty.EntityPropertyNameToMap);
                        if (entityPropertyInfo == null) ThrowEntityToDtoMappingException(String.Format("The Dto [{0}] used for the mapping has a property [{1}] that is mapped to an non-existent entity [{2}] property [{3}].  Fix the Dto mapping!",
                            dtoType, dtoPropertyInfo.Name, entityType, mapToEntityProperty.EntityPropertyNameToMap));
                        var dtoentityMappingProperties = new DtoEntityMappingProperties(mapToEntityProperty, dtoPropertyInfo, entityPropertyInfo);
                        result.Add(dtoentityMappingProperties);
                    }
                }
            }
            catch (MissingMethodException ex)
            {
                ThrowDtoToEntityMappingException(String.Format("There is a wrong Mapping attribute(MapToEntityProperty) in your dto. Cannot find specified property in your Entity: {0}", ex.Message));
            }
            return result;
        }

        // evita di far tornare le classi proxy.
        private static Type ExtractOriginalTypeIfTypeIsProxyClass(Type entityType)
        {
            if (entityType.Name.ToLower().EndsWith("proxy"))
            {
                // implementazione originaria
                //return entityType.GetProperties()[0].DeclaringType;

                if (entityType.BaseType.Name.ToLower() + "proxy" != entityType.Name.ToLower())
                    ThrowEntityToDtoMappingException(string.Format("Cannot get basetype {0} from type {1}", entityType.BaseType.Name, entityType.Name));
                return entityType.BaseType;
            }
            return entityType;
        }

        //func<T, TResult>
        public void AddEntityResolver(string entityPropertyToResolve, string dtoPropertyToUse, Func<object, object> resolver)
        {
            _knowEntityResolver.Add(entityPropertyToResolve);
            _knowEntityResolverMappings[dtoPropertyToUse] = resolver;
        }
    }
}