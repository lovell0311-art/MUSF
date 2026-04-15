using System;
using NLog;

namespace CustomFrameWork
{
    public interface ILogger
    {
        void Log(string message, Exception exception, params object[] args);
        void Log(Exception exception);
        void Log(string message, params object[] args);
        LogEventInfo CreateLogEventInfo(LogLevel level);
        void SetLogLevel(LogLevel level);
    }
}
