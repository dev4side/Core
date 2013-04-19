using System;
using Core.Log.Helper;
using System.Threading;
using Core.Log.Log4Net.Managers;

namespace Core.Log.Log4Net
{
    public class VsDebugLogger<T> : ILog<T>
    {
        private const string DEBUG_INDICATOR = " [Core] ";
        private const string INFO_INDICATOR = DEBUG_INDICATOR + " [INFO] ";
        private const string WARNING_INDICATOR = DEBUG_INDICATOR + " [WARN] ";
        private const string ERROR_INDICATOR = DEBUG_INDICATOR + " [ERRR] ";
        private const string FATAL_INDICATOR = DEBUG_INDICATOR + " [SHIT] ";

        public void Debug(params string[] message)
        {
            WriteToDebugWindow(DEBUG_INDICATOR, message);
        }

        public void Debug(Exception exception, params string[] message)
        {
            WriteToDebugWindow(DEBUG_INDICATOR, message, exception);
        }
        
        public void Info(params string[] message)
        {
            WriteToDebugWindow(INFO_INDICATOR, message);
        }

        public void Info(Exception exception, params string[] message)
        {
            WriteToDebugWindow(INFO_INDICATOR, message, exception);
        }

        public void Warn(params string[] message)
        {
            WriteToDebugWindow(WARNING_INDICATOR, message);
        }

        public void Warn(Exception exception, params string[] message)
        {
            WriteToDebugWindow(WARNING_INDICATOR, message, exception);
        }

        public void Error(params string[] message)
        {
            WriteToDebugWindow(ERROR_INDICATOR, message);
        }

        public void Error(Exception exception, params string[] message)
        {
            WriteToDebugWindow(ERROR_INDICATOR, message, exception);
        }

        public void Fatal(params string[] message)
        {
            WriteToDebugWindow(FATAL_INDICATOR, message);
        }

        public void Fatal(Exception exception, params string[] message)
        {
            WriteToDebugWindow(FATAL_INDICATOR, message, exception);
        }

        private static void WriteExceptionDetails(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Exception Message: {0}", exception.Message));
            System.Diagnostics.Debug.WriteLine(String.Format("StackTrace: {0}", exception.StackTrace));
        }

        private void WriteToDebugWindow(string errorLelvelIndicatgor, string[] message)
        {
            System.Diagnostics.Debug.Indent();
            System.Diagnostics.Debug.WriteLine(String.Concat(errorLelvelIndicatgor, ThreadIdOrName(), LogFormatter.BuildMessageFromParams(message)));
            System.Diagnostics.Debug.Unindent();
        }

        private string ThreadIdOrName()
        {
            const string resultFormat = " [{0}] ";

            return Thread.CurrentThread.Name == null
                       ? string.Format(resultFormat, Thread.CurrentThread.ManagedThreadId.ToString())
                       : string.Format(resultFormat, Thread.CurrentThread.Name);
        }

        private void WriteToDebugWindow(string errorLelvelIndicatgor, string[] message, Exception exception)
        {
            System.Diagnostics.Debug.Indent();
            System.Diagnostics.Debug.WriteLine(String.Concat(errorLelvelIndicatgor, LogFormatter.BuildMessageFromParams(message)));
            WriteExceptionDetails(exception);
            System.Diagnostics.Debug.Unindent();
        }

        public bool IsDebugEnabled()
        {
            return true;
        }

        public void DumpObject(object objectToDump)
        {
            WriteToDebugWindow(DEBUG_INDICATOR, new[] { ObjectDumper.Dump(objectToDump) });
        }
    }
}
