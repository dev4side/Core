using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Transform;
using System.Collections;
using System.Runtime.CompilerServices;
using NHibernate.Properties;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate;

namespace Core.Data.NHibernate.Transform
{
    abstract public class BaseDistinctRootColumnResultTransformer<TDto> : IResultTransformer where TDto : class, new()
    {
        private static readonly object Hasher = new object();
        private readonly Type resultClass;
        private readonly IPropertyAccessor propertyAccessor;

        private ISetter[] setters;
        private ISetter[] settersList;
        private IGetter[] getterLists;

        private bool _isClassChecked = false;

        public delegate object ObjectActivator(params object[] args);
        private Dictionary<Type, ObjectActivator> ActivatorDic = new Dictionary<Type, ObjectActivator>();

        protected BaseDistinctRootColumnResultTransformer()
        {
            resultClass = typeof(TDto);
            propertyAccessor = new ChainedPropertyAccessor(new[]
				                            	{
				                            		PropertyAccessorFactory.GetPropertyAccessor(null),
				                            		PropertyAccessorFactory.GetPropertyAccessor("field")
				                            	});
        }

        virtual public object TransformTuple(object[] tuple, string[] aliases)
        {
            InitializeGettersAndSettersIfNeeded(aliases);
            var dto = GetDtoFromTupleAndAliases(aliases, tuple);
            return new Identity(tuple[0], dto, settersList, getterLists);
        }

        abstract public IList TransformList(IList list);

        private object GetDtoFromTupleAndAliases(string[] aliases, object[] tuple)
        {
            var dto = GetIstanceAsIListByType(resultClass);
            for (var i = 1; i < aliases.Length; i++)
            {
                int setterEnumerator = i - 1;
                var setterType = setters[setterEnumerator].Method.GetParameters()[0].ParameterType;
                //  handle if tuple is a list
                if (setterType.IsGenericType && setterType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    Type itemType = setterType.GetGenericArguments()[0];
                    var generic = typeof(List<>);
                    var specific = generic.MakeGenericType(itemType);
                    var list = GetIstanceAsIListByType(specific) as IList;
                    if (tuple[i] != null)
                    {
                        if (itemType == typeof(string))
                            list.Add(tuple[i].ToString());
                        else
                            list.Add(tuple[i]);
                    }
                    
                    setters[setterEnumerator].Set(dto, list);
                }
                // handle if tuple is enum
                //else if (tuple[i] != null && tuple[i].GetType().IsEnum)
                //{
                //    setters[setterEnumerator].Set(dto, (int)tuple[i]);
                //}
                else
                    setters[setterEnumerator].Set(dto, tuple[i]);
            }
            return dto;
        }

        private object GetIstanceAsIListByType(Type specific)
        {
            ObjectActivator candidateActivator;
            if (ActivatorDic.TryGetValue(specific, out candidateActivator))
                return candidateActivator(null);

            candidateActivator = GetActivator(specific.GetConstructors().First(x => !x.GetParameters().Any()));

            ActivatorDic.Add(specific, candidateActivator);
            return candidateActivator(null);
        }

        private void InitializeGettersAndSettersIfNeeded(string[] aliases)
        {
            if (_isClassChecked)
                return;

            var listSetters = new List<ISetter>();
            var listGetters = new List<IGetter>();

            if (aliases == null)
                throw new ArgumentNullException("aliases");

            try
            {
                setters = new ISetter[aliases.Length - 1];
                for (int i = 1; i < aliases.Length; i++)
                {
                    //var aliasesEnumerator = i - 1;
                    var setterEnumerator = i - 1;
                    string alias = aliases[i];
                    if (alias != null)
                    {
                        var setter = propertyAccessor.GetSetter(resultClass, alias);
                        setters[setterEnumerator] = setter;
                        var setterType = setter.Method.GetParameters()[0].ParameterType;
                        if (setterType.IsGenericType && setterType.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            listSetters.Add(setter);
                            listGetters.Add(propertyAccessor.GetGetter(resultClass, alias));
                        }
                    }

                }
            }
            catch (InstantiationException e)
            {
                throw new HibernateException("Could not instantiate result class: " + resultClass.FullName, e);
            }
            catch (MethodAccessException e)
            {
                throw new HibernateException("Could not instantiate result class: " + resultClass.FullName, e);
            }

            settersList = listSetters.ToArray();
            getterLists = listGetters.ToArray();

            _isClassChecked = true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return obj.GetHashCode() == Hasher.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Hasher.GetHashCode();
        }

        // http://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/
        public static ObjectActivator GetActivator(ConstructorInfo ctor)
        {
            // Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();
            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");
            Expression[] argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator), newExp, param);

            //compile it
            ObjectActivator compiled = (ObjectActivator)lambda.Compile();
            return compiled;
        }

        //############################################################ classes

        internal sealed class Identity
        {
            private ISetter[] _settersList;
            private IGetter[] _getterList;

            public object Id { get; private set; }
            public object Dto { get; private set; }

            internal Identity(object distinctKey, object dto, ISetter[] settersList, IGetter[] getterList)
            {
                Dto = dto;
                Id = distinctKey;
                _settersList = settersList;
                _getterList = getterList;
            }

            public override bool Equals(object other)
            {
                var that = (Identity)other;
                return ReferenceEquals(Id, that.Id);
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(Id);
            }

            internal void MergeTupleProperties(Identity identity)
            {
                for (var i = 0; i < _getterList.Length; i++)
                {
                    var currentListToAdd = _getterList[i].Get(this.Dto) as IList;
                    var listtoExtractItem = _getterList[i].Get(identity.Dto) as IList;
                    if (listtoExtractItem != null && listtoExtractItem.Count > 0)
                    {
                        var candidateItemToAdd = listtoExtractItem[0];
                        if (currentListToAdd != null)
                        {
                            //if (IsUnique(this.Dto, currentListToAdd, _settersList[i], _getterList[i]))
                            //{
                                currentListToAdd.Add(candidateItemToAdd);
                                _settersList[i].Set(this.Dto, currentListToAdd);
                            //}
                        }
                    }
                }
            }

            //private bool IsUnique(object dto, object candidate, ISetter setter, IGetter getter)
            //{

            //    var test = getter.Get(dto) as IList;

            //    return test.Contains(candidate);

            //}
        
        }        
    }
}
