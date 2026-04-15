using System;
using NLog;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;

namespace CustomFrameWork
{
    /// ==================================================================
    /// NLog.config 如何配置？
    /// https://github.com/NLog/NLog/wiki/Configuration-file
    /// ==================================================================


    /// <summary>
    /// 日志系统
    /// </summary>
    public static partial class Log
    {
        public static readonly ILogger logger = new NLogAdapter(LogLevel.Trace,"Logger");

        public static readonly ILogger playerLogger = new NLogAdapter(LogLevel.Debug, "PlayerLogger");
        public static readonly ILogger consoleLogger = new NLogAdapter(LogLevel.Debug, "ConsoleLogger");



        private static System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

        public static void Trace(string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Trace);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Log(message);
        }

        public static void Debug(string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Debug);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Log(message);
        }

        public static void Info(string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Info);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Log(message);
        }

        public static void Warning(string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Warn);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Log(message);
        }

        public static void Error(string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Error);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Log("{0}\n{1}",message, GetStacksInfo(2));
        }

        public static void Error(Exception exception,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Error);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Log("{0}\n{1}", exception.ToString(), GetStacksInfo(2));
        }

        public static void Error(string message,
            Exception exception,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Error);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Log("{0}\n{1}\n{2}", message, exception.ToString(), GetStacksInfo(2));
        }

        /// <summary>
        /// 致命的错误，无法恢复
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callerLineNumber"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        public static void Fatal(string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Fatal);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Log("{0}\n{1}", message, GetStacksInfo(2));
        }

        public static void Fatal(string message,
            Exception exception,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Fatal);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Log("{0}\n{1}\n{2}",message, exception.ToString(), GetStacksInfo(2));
        }

        /// <summary>
        /// 输出到控制台
        /// </summary>
        /// <param name="message"></param>
        public static void Console(string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = consoleLogger.CreateLogEventInfo(LogLevel.Info);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            consoleLogger.Log(message);
        }

        /// <summary>
        /// 玩家操作日志
        /// 日志输出位置: Log/AppType_AppId/AppType_AppId_PlayerLog_年月日.txt
        /// </summary>
        /// <param name="message"></param>
        public static void PLog(string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = playerLogger.CreateLogEventInfo(LogLevel.Info);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            playerLogger.Log(message);
        }

        /// <summary>
        /// 玩家操作日志
        /// 日志输出位置: Log/AppType_AppId/AppType_AppId_PlayerLog_年月日.txt
        /// 通过标签来过滤是哪个系统的日志，没必要重新保存个日志文件
        /// </summary>
        /// <param name="message"></param>
        public static void PLog(string tag,string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = playerLogger.CreateLogEventInfo(LogLevel.Info);
            if (logEvent == null) return;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            playerLogger.Log("#" + tag + "# " + message);
        }



        /// ===============================================================================
        /// 如何使用？ 
        /// Log.Info().Log("test message! name={0}",name);
        /// 将 string 格式化放到最后（需要时才会格式化），Trace,Debug 关闭时，性能损耗很小
        /// ===============================================================================


        public static ILogger Trace([CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Trace);
            if (logEvent == null) return logger;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            return logger;
        }

        public static ILogger Debug([CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Debug);
            if (logEvent == null) return logger;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            return logger;
        }

        public static ILogger Info([CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Info);
            if (logEvent == null) return logger;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            return logger;
        }

        public static ILogger Warning([CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Warn);
            if (logEvent == null) return logger;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            return logger;
        }

        public static ILogger Error([CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Error);
            if (logEvent == null) return logger;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            return logger;
        }

        public static ILogger Fatal([CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            LogEventInfo logEvent = logger.CreateLogEventInfo(LogLevel.Fatal);
            if (logEvent == null) return logger;
            logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
            return logger;
        }



        #region Core
        /// <summary>
        /// 获取堆栈信息
        /// </summary>
        /// <param name="skipFrame">跳过多少帧</param>
        /// <returns></returns>
        public static string GetStacksInfo(int skipFrame = 0)
        {
            stringBuilder.Clear();
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(skipFrame, true);
            /*
            for (int i = 0, n = st.FrameCount; i < n; ++i)
            {
                string info;
                System.Diagnostics.StackFrame frame = st.GetFrame(i);
                if(string.IsNullOrEmpty(frame.GetFileName()))
                {
                    info = string.Format("   at {0}",
                                        frame.GetMethod().Name);
                }
                else
                {
                    info = string.Format("   at {0} in {1}:line {2}",
                                        frame.GetMethod().Name,
                                        frame.GetFileName(),
                                        frame.GetFileLineNumber());
                }

                stringBuilder.AppendLine(info);
            }*/
            return st.ToString();
        }
        #endregion

    }
}
