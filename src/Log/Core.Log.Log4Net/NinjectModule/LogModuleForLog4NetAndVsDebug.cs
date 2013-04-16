using System.Diagnostics;
using Ninject.Modules;

namespace Core.Log.Log4Net
{
    public class Log4NetAndVsModule : NinjectModule
    {
        public override void Load()
        {
#if DEBUG
            Bind(typeof(ILog<>)).To(typeof(Log4NetLogger<>)).When(l => !Debugger.IsAttached).InSingletonScope();
            Bind(typeof(ILog<>)).To(typeof(VsDebugLogger<>)).When(l => Debugger.IsAttached).InSingletonScope();
#else
              Bind(typeof(ILog<>)).To(typeof(Log4NetLogger<>)).InSingletonScope();
#endif

        }
    }
}
