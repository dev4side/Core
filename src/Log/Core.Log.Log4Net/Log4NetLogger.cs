using System;
using Core.Log.Helper;
using log4net;
using Core.Log.Log4Net.Managers;

namespace Core.Log.Log4Net
{
    public class Log4NetLogger<TType> : ILog<TType>
    {
        public Log4NetLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public void Debug(params string[] message)
        {
            if (IsDebugEnabled())
                LogManager.GetLogger(typeof(TType)).Debug(LogFormatter.BuildMessageFromParams(message));
        }

        public void Debug(Exception exception, params string[] message)
        {
            if (IsDebugEnabled())
                LogManager.GetLogger(typeof(TType)).Debug(LogFormatter.BuildMessageFromParams(message), exception);
        }

        public void Info(params string[] message)
        {
            LogManager.GetLogger(typeof(TType)).Info(LogFormatter.BuildMessageFromParams(message));
        }

        public void Info(Exception exception, params string[] message)
        {
            LogManager.GetLogger(typeof(TType)).Info(LogFormatter.BuildMessageFromParams(message), exception);
        }

        public void Warn(params string[] message)
        {
            LogManager.GetLogger(typeof(TType)).Warn(LogFormatter.BuildMessageFromParams(message));
        }

        public void Warn(Exception exception, params string[] message)
        {
            LogManager.GetLogger(typeof(TType)).Warn(LogFormatter.BuildMessageFromParams(message), exception);
        }

        public void Error(params string[] message)
        {
            LogManager.GetLogger(typeof(TType)).Error(LogFormatter.BuildMessageFromParams(message));
        }

        public void Error(Exception exception, params string[] message)
        {
            LogManager.GetLogger(typeof(TType)).Error(LogFormatter.BuildMessageFromParams(message), exception);
        }

        public void Fatal(params string[] message)
        {
            LogManager.GetLogger(typeof(TType)).Fatal(LogFormatter.BuildMessageFromParams(message));
        }

        public void Fatal(Exception exception, params string[] message)
        {
            LogManager.GetLogger(typeof(TType)).Fatal(LogFormatter.BuildMessageFromParams(message), exception);
        }

        public bool IsDebugEnabled()
        {
            var lol = LogManager.GetLogger(typeof (TType));
            return lol.IsDebugEnabled;
        }

        public void DumpObject(object objectToDump)
        {
            if (IsDebugEnabled())
            {
                LogManager.GetLogger(typeof (TType)).Debug(ObjectDumper.Dump(objectToDump));
            }
        }
    }
}
