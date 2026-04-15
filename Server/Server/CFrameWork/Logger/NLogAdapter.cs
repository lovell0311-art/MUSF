using System;
using NLog;
using System.Runtime.CompilerServices;

namespace CustomFrameWork
{
	public class NLogAdapter: ILogger
    {
		private readonly Logger logger = null;

		private LogLevel logLevel = LogLevel.Trace;

		private LogEventInfo logEvent = null;

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="level">最小日志记录等级</param>
		public NLogAdapter(LogLevel level,string loggerName)
        {
			logLevel = level;
			logger= LogManager.GetLogger(loggerName);
		}

        public void SetLogLevel(LogLevel level)
        {
            logLevel = level;
        }

        public void Log(string message, Exception exception, params object[] args)
        {
			if (logEvent == null) return;
			logEvent.Exception = exception;
			logEvent.Message = message;
			logEvent.Parameters = args;
			logger.Log(GetType(), logEvent);
		}

        public void Log(string message, params object[] args)
        {
			if (logEvent == null) return;
            logEvent.Message = message;
            logEvent.Parameters = args;
            logger.Log(GetType(), logEvent);
        }
        public void Log(Exception exception)
        {
            if (logEvent == null) return;
            logEvent.Message = exception.ToString();
            logger.Log(GetType(), logEvent);
        }

        public LogEventInfo CreateLogEventInfo(LogLevel level)
        {
			if (logLevel > level)
            {
				logEvent = null;
				return null;
			}

			logEvent = LogEventInfo.Create(level, logger.Name, null);
			return logEvent;
		}


        public void Trace(string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
		{
			this.logger.Trace(message);
		}

	}
}