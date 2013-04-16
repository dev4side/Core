using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.Kernel.Exceptions;

namespace Core.Kernel
{
    //nested type perche non ha un vero utilizzo al difuori dall autobinder
    public class ModuleBinder
    {
        public Type Interface { get; set; }
        public Type Concrete { get; set; }
    }

    public class AutoBinder
    {

        public static IEnumerable<ModuleBinder> GetBindingsBasedOnGenericInterface(Type interfaceAsGeneric)
        {
            IList<ModuleBinder> result = new List<ModuleBinder>();
            var assemblyToSearchIn = interfaceAsGeneric.Assembly;
            var concrateImplementationOfGenericInterface = GetConcrateImplementationOfGenericInterface(interfaceAsGeneric, assemblyToSearchIn);

            foreach (var concrateType in concrateImplementationOfGenericInterface)
            {

                var genericInterfacesToBind = (from interfaceAsType in concrateType.GetInterfaces()
                                               where
                                                   interfaceAsType.IsGenericType &&
                                                   interfaceAsType.GetGenericTypeDefinition() == interfaceAsGeneric
                                               select interfaceAsType);

                var interfacesFound = genericInterfacesToBind.Count();
                if (interfacesFound > 1)
                    throw new Exception("todo");
                if (interfacesFound == 1)
                    result.Add(new ModuleBinder() { Interface = genericInterfacesToBind.ElementAt(0), Concrete = concrateType });
            }
            return result;

        }



        //TODO: i tipi come parametri e non generics!!!!!!!
        /// <summary>
        ///  Trova tutte le associazioni interfaccia -implemtazione concreata, effettuando la ricerca filtrando (equalsTo) per il namespace di entrambi
        /// </summary>
        /// <typeparam name="TInterfaceSample">il tipo dell' interfaccia da utilizzare per cercare le interefaccie che iniziano con lo stesso namespace</typeparam>
        /// <typeparam name="TConcrateSample">il tipo della classe concreta da utilizzare per caricaricare i tipi che iniziano con lo stesso namespace</typeparam>
        /// <returns></returns>
        public static IEnumerable<ModuleBinder> GetBindingsBasedOnStartWithSameNamespaces<TInterfaceSample, TConcrateSample>() where TConcrateSample:TInterfaceSample
        {
            // questo metodo può essere fatto ancora piu genrerico:
            // in teoria non serve dover specificare anche il tipo della calsse concreta. in questo caso viene fatto perchè non so in qulae assembly cercare


            IList<ModuleBinder> result = new List<ModuleBinder>();
            var interfaceTypes = GetInterfaceTypesWithTheSameNamespece(typeof(TInterfaceSample));
            var concrateTypes = GetConcrateTypesWithTheSameNamespece(typeof(TConcrateSample));

            foreach (var interfaceType in interfaceTypes)
            {
                Type typeToSearch = interfaceType;
                var concreateImplementationsOfInterface =
                    (from concrateType in concrateTypes
                     where concrateType.GetInterfaces().Any(x => x == typeToSearch)
                     select concrateType);

                var concrateTypesWithCurrentInterface = concreateImplementationsOfInterface.Count();
                if (concrateTypesWithCurrentInterface > 1)
                    ThrowMultipleConcrateImplementations(concreateImplementationsOfInterface, typeToSearch);
                if (concrateTypesWithCurrentInterface == 1)
                    result.Add(new ModuleBinder()
                               {
                                   Interface = interfaceType,
                                   Concrete = concreateImplementationsOfInterface.ElementAt(0)
                               });
            }

            return result;
        }

        #region internal helpers
        private static List<Type> GetConcrateTypesWithTheSameNamespece(Type concreteSampleType)
        {

            Assembly concrateAssembly = concreteSampleType.Assembly;
            string concrateNamespaceToMatch = concreteSampleType.Namespace;
            var concrate = (from type in concrateAssembly.GetTypes()
                            where type.Namespace != null && 
                            (!type.Name.Contains("<>") && // escludo le classi proxy generate dinamicamente
                           //  type.Namespace.StartsWith(concrateNamespaceToMatch))
                             type.Namespace.Equals(concrateNamespaceToMatch))
                            select type).ToList();
            return concrate;
        }


        private static List<Type> GetInterfaceTypesWithTheSameNamespece(Type interfacesSampleType)
        {
            string interfaceNamespaceToMatch = interfacesSampleType.Namespace;
            var interfaceAssembly = interfacesSampleType.Assembly;
            var typesInAssembly = interfaceAssembly.GetTypes();

            var interfaces = (from type in typesInAssembly
                              where
                                  type.Namespace != null && 
                               //   (type.Namespace.StartsWith(interfaceNamespaceToMatch) &&
                                (type.Namespace.Equals(interfaceNamespaceToMatch) &&
                                  type.IsInterface)
                              select type);

            if (!interfaces.Any())
                throw new DependencyInjectionException(String.Format("Cannot find interfaces with namespace: {0} in assembly {1}. AutoBinder Failed using interface {2}!!",
                                  interfaceNamespaceToMatch, interfaceAssembly.FullName, interfacesSampleType.FullName));
            return interfaces.ToList();
        }

        private static void ThrowMultipleConcrateImplementations(IEnumerable<Type> concreateImplementationsOfInterface, Type type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Automatic bindings can be done only with 1 to 1 relationshipt between" +
                                        " conrate and interface trypes. for interface {0} have been found {1} concrate types:",
                                        type, concreateImplementationsOfInterface.Count()));
            foreach (var concrateType in concreateImplementationsOfInterface)
                sb.AppendLine(concrateType.FullName);
            throw new Exception(sb.ToString());
        }


        private static List<Type> GetConcrateImplementationOfGenericInterface(Type interfaceAsGeneric, Assembly assemblyToSearchIn)
        {
            var concrateImplementationOfGenericINterface = (from type in assemblyToSearchIn.GetTypes()
                                                            where
                                                                type.GetInterfaces().Any(
                                                                    x =>
                                                                    x.IsGenericType &&
                                                                    x.GetGenericTypeDefinition() == interfaceAsGeneric)
                                                            select type).ToList();
            return concrateImplementationOfGenericINterface;
        }
        #endregion
    }
}
