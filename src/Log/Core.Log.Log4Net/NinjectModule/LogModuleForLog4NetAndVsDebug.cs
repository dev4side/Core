namespace Core.Log.Log4Net.NinjectModule
{
    public class Log4NetAndVsModule : Ninject.Modules.NinjectModule
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
