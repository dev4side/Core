using System;

namespace Core.Log
{
    public interface ILog<TType>
    {
        void Debug(params string[] message);
        void Debug(Exception exception, params string[] message);
        void Info(params string[] message);
        void Info(Exception exception, params string[] message);
        void Warn(params string[] message);
        void Warn(Exception exception, params string[] message);
        void Error(params string[] message);
        void Error(Exception exception, params string[] message);
        void Fatal(params string[] message);
        void Fatal(Exception exception, params string[] message);

        void DumpObject(object objectToDump);

        bool IsDebugEnabled();
    }
}