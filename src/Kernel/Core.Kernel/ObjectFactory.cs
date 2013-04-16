using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Core.Kernel.Attributes;
using Core.Kernel.Exceptions;
using Ninject;
using System.Reflection;
using System.Diagnostics;
using Ninject.Modules;
using System.IO;
using Ninject.Parameters;

namespace Core.Kernel
{
    public class ObjectFactory
    {
        //private const string MODULE_ASSEMBLY_MATCH_PATTERN = "Empower.*.dll";

        private const string APP_SETTING_KEY_FOR_ASSEMBLIES_TO_SEARCH_MODULES = "DIModules";
        private const char ASSEMBLIES_SPLIT_CHAR = ',';

        private static IKernel _kernel;

        public static IKernel Kernel
        {
            get
            {
                if (_kernel == null)
                    BuildKernel();
                return _kernel;
            }
        }

        #region Public methods

        public static void ResolveDependencies(object ob)
        {
            Kernel.Inject(ob);
        }

        public static object Get(Type type)
        {
            return Kernel.Get(type);
        }

        public static object TryGet(Type type)
        {
            return Kernel.TryGet(type);
        }

        public static T Get<T>(params IParameter[] constructionParams)
        {
            return Kernel.Get<T>(constructionParams);
        }

        public static T Get<T>(string name, params IParameter[] constructionParams)
        {
            return Kernel.Get<T>(name, constructionParams);
        }

        public static T TryGet<T>()
        {
            return Kernel.TryGet<T>();
        }
        
        #endregion

        // questo metodo è indispensabile per il global asax dell' applicazione web
        //vedere Empower.Nexo.Web.Global
        public static IKernel GetNinjectKernelForGlobalAsax()
        {
            return Kernel;
        }

        private static void BuildKernel()
        {
            _kernel = new StandardKernel(GetSettings(), new INinjectModule[] { });
            var candidateAssembliesToLoad = GetAssembliesContainingModules();
            var rejectedAssembliesToLoad = new List<Assembly>();
            foreach (var candidateAssembly in candidateAssembliesToLoad)
            {
                try
                {
                    _kernel.Load(candidateAssembly);
                    WriteEntryLog(String.Format("{0} has been loaded", candidateAssembly.FullName), false);
                }
                catch (FileNotFoundException)
                {
                    rejectedAssembliesToLoad.Add(candidateAssembly);
                }
                catch (Exception ex)
                {
                    LogToEventViewer(ex, candidateAssembly);
                }
            }

            if (rejectedAssembliesToLoad.Count != 0)
                FormatUnableToLoadNinjectModulesException(rejectedAssembliesToLoad);
        }

        private static void LogToEventViewer(Exception ex, Assembly faultedAssembly = null)
        {
            var sb = new StringBuilder();
            if (faultedAssembly != null) sb.AppendLine("An error occured in loading ninject modules from assembly: " + faultedAssembly.FullName);
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);
            WriteInnerExceptionIfExists(sb, ex);
            WriteEntryLog(sb.ToString(), true);
        }

        private static void WriteInnerExceptionIfExists(StringBuilder sb, Exception ex)
        {
            if (ex.InnerException != null)
            {
                var innerEx = ex.InnerException;
                sb.AppendLine("Inner:");
                sb.AppendLine(innerEx.Message);
                sb.AppendLine(innerEx.StackTrace);
                WriteInnerExceptionIfExists(sb, innerEx);
            }
        }

        private static void FormatUnableToLoadNinjectModulesException(IEnumerable<Assembly> rejectedAssembliesToLoad)
        {
            var sb = new StringBuilder();
            sb.AppendLine("An error occured during the loading of some Ninject Assemblies deployed in your bin folder.");
            sb.AppendLine("Probably some references are missing!");
            sb.AppendLine("The following assemblies failed to load:");
            foreach (var assembly in rejectedAssembliesToLoad)
                sb.AppendLine(assembly.FullName);
            var excpetionToLaunch = new DependencyInjectionException(sb.ToString());
            LogToEventViewer(excpetionToLaunch);
            throw excpetionToLaunch;
        }

        /// <summary>
        /// Usato nei tests, per i mocks
        /// </summary>
        /// <param name="kernel"></param>
        public static void AssignKernel(IKernel kernel)
        {
            _kernel = kernel;
        }

        public static void ResetKernel()
        {
            _kernel = null;
        }

        protected static INinjectSettings GetSettings()
        {
            return new NinjectSettings() { LoadExtensions = true, InjectNonPublic = true};
        }

        private static IEnumerable<Assembly> GetAssembliesContainingModules()
        {
            var result = new List<Assembly>();
            var assemblyNamesToLoad = GetAssemblyPatternsFromConfigToLoad();
            foreach (var assemblyPattern in assemblyNamesToLoad)
            {
                var currentAssemblyUri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                var currenyAssemblyDirectory = new FileInfo(currentAssemblyUri.LocalPath).Directory;
                if (currenyAssemblyDirectory != null)
                {
                    var files = currenyAssemblyDirectory.GetFiles(assemblyPattern);
                    foreach (var file in files)
						if (!file.Name.ToLower().EndsWith(".vshost.exe") && (file.Name.ToLower().EndsWith(".dll") || file.Name.ToLower().EndsWith(".exe")))
                            result.Add(Assembly.Load(file.Name.ToLower().Replace(".dll", String.Empty).Replace(".exe", String.Empty)));
                }
            }
            WriteFoundAssembliesInLog(result);
            result.Sort(new PriorityComparer());

            //var newList  = result.OrderBy(delegate(Assembly assembly)
            //                   {
            //                       var orderPriorityList = assembly.GetCustomAttributes(
            //                           typeof (ModuleLoadPriorityAttribute), false);
            //                       if (orderPriorityList.Length > 0)
            //                           return ((ModuleLoadPriorityAttribute) orderPriorityList[0]).Priority;
            //                       return 0;
            //                   });


            return result;
        }

        private static void WriteFoundAssembliesInLog(List<Assembly> result)
        {
            if(result == null || result.Count == 0)
            {
                var messageToThrow =
                    String.Format("Cannot configure ObjectFactory: no assemblies are found with the search pattern {0}",
                                  ConfigurationManager.AppSettings[APP_SETTING_KEY_FOR_ASSEMBLIES_TO_SEARCH_MODULES]);
                WriteEntryLog(messageToThrow, true);
                throw new DependencyInjectionException(messageToThrow);
            }
           
            var sb = new StringBuilder();
            sb.AppendLine("###### the kernel's object facory has started using the auto module loading ######");
            sb.AppendLine("the patter defined in config for searching modules in asseblies is: ");
            sb.AppendLine(ConfigurationManager.AppSettings[APP_SETTING_KEY_FOR_ASSEMBLIES_TO_SEARCH_MODULES]);
            sb.AppendLine("Assemblies that matched selected patters are listed below. Please note that lower is the priority," +
                          "earlier the module is loaded. ");
            foreach (var foundAssembly in result)
                sb.AppendLine(string.Format("- {0} with module loading priority: {1} ",foundAssembly.FullName, PriorityComparer.ReadPriority(foundAssembly)));
            WriteEntryLog(sb.ToString(), false);
        }

        private static string[] GetAssemblyPatternsFromConfigToLoad()
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(APP_SETTING_KEY_FOR_ASSEMBLIES_TO_SEARCH_MODULES))
                throw new DependencyInjectionException(
                    String.Format("Cannot configure ObjectFactory: your app.config must contain an app setting key [{0}] " +
                                  "and the value must contains assembly search patterns separated by [{1}]",
                                  APP_SETTING_KEY_FOR_ASSEMBLIES_TO_SEARCH_MODULES, ASSEMBLIES_SPLIT_CHAR));
            var assemblyNamesToLoad = ConfigurationManager.AppSettings[APP_SETTING_KEY_FOR_ASSEMBLIES_TO_SEARCH_MODULES].Split(ASSEMBLIES_SPLIT_CHAR);

            var assemblyNamesWithExeAndDllToLoad = new List<string>();
            for (int i = 0; i < assemblyNamesToLoad.Length; i++)
            {
                if (!assemblyNamesToLoad[i].ToLowerInvariant().EndsWith(".dll"))
                    assemblyNamesWithExeAndDllToLoad.Add(string.Format("{0}.dll", assemblyNamesToLoad[i]));
                    //assemblyNamesToLoad[i] += ".dll";
                if (!assemblyNamesToLoad[i].ToLowerInvariant().EndsWith(".exe"))
                    assemblyNamesWithExeAndDllToLoad.Add(string.Format("{0}.exe", assemblyNamesToLoad[i]));
            }
            return assemblyNamesToLoad;
        }

        private static void WriteEntryLog(string message, bool isError)
        {
            EventLog.WriteEntry("Empower", message, isError ? EventLogEntryType.Error : EventLogEntryType.Information);
        }
    }

    class PriorityComparer : IComparer<Assembly>
    {
        public int Compare(Assembly x, Assembly y)
        {
            var xPriority = ReadPriority(x);
            var yPriority = ReadPriority(y);
            return xPriority.CompareTo(yPriority);
        }

        public static int ReadPriority(Assembly assembly)
        {

            var orderPriorityList = assembly.GetCustomAttributes(
                typeof(ModuleLoadPriorityAttribute), false);
            if (orderPriorityList.Length > 0)
                return ((ModuleLoadPriorityAttribute)orderPriorityList[0]).Priority;
            return 0;
        }
    }


}
