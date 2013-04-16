using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Ninject;
using Ninject.Extensions.Interception;

namespace Core.Log.AOP
{
    public class LogInterceptor<T> : SimpleInterceptor
    {
        readonly Stopwatch _stopwatch = new Stopwatch();
        private ILog<T> _log;

        protected override void BeforeInvoke(IInvocation invocation)
        {
            _log = invocation.Request.Kernel.Get<ILog<T>>();
            LogBannerAndMethodAndInputParameters(invocation);
            _stopwatch.Start();
        }


        protected override void AfterInvoke(IInvocation invocation)
        {
            _stopwatch.Stop();
            LogReturnValueAndFooter(invocation);
            _stopwatch.Reset();
        }


        private void LogReturnValueAndFooter(IInvocation invocation)
        {
            if(_log.IsDebugEnabled())
            {
                var sb = new StringBuilder();
                _log.Debug("Method {0} took {1} ms.", invocation.Request.Method.Name,
                           _stopwatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
                DisplayObjectProperties(invocation.ReturnValue,ref sb);
                _log.Debug(sb.ToString());
                _log.Debug("### [End] #### Method [{0}].[{1}] invokation finished", invocation.Request.Target.ToString(), invocation.Request.Method.Name);
            }
        }

        private void LogBannerAndMethodAndInputParameters(IInvocation invocation)
        {
            if (_log.IsDebugEnabled())
            {
                _log.Debug("### [Start] #### Method [{0}].[{1}] invokation starts", invocation.Request.Target.ToString(),invocation.Request.Method.Name);
                _log.Debug("Method received {0} paramenters", invocation.Request.Arguments.Length.ToString(CultureInfo.InvariantCulture));
                _log.Debug(LogObjects(invocation.Request.Arguments));
            }
            else
            {
                _log.Info("Invoked method [{0}].[{1}]", invocation.Request.Target.ToString(), invocation.Request.Method.Name);
            }
        }


        #region ReflectionLoggers
        private static string LogObjects(object[] objectsToLog)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < objectsToLog.Length; i++)
            {
                sb.AppendLine(String.Format("Paramenter #{0}", i));
                DisplayObjectProperties(objectsToLog[i], ref sb);
            }
            return sb.ToString();
        }


        private static void DisplayObjectProperties(Object objectToLog, ref StringBuilder sb)
        {
            if (objectToLog == null)
            {
                sb.AppendLine("Object is null");
                return;
            }
      
            var type = objectToLog.GetType();
            sb.AppendLine("Object value report");
            sb.AppendLine(String.Format("  | Type {0} = {1} ", type.Name, objectToLog));
            foreach (PropertyInfo p in type.GetProperties())
            {
                if (p.CanRead && p.GetIndexParameters().Length == 0)
                {
                    // p.GetValue(objectToLog,)
                    var obj = p.GetValue(objectToLog, null);
                    if (obj != null)
                        sb.AppendLine(String.Format("  |-Property {0} = {1} ", p.Name, obj));
                    else sb.AppendLine(String.Format("  |-Property {0} = null ", p.Name));
                }
            }
        } 
        #endregion


    }
}
