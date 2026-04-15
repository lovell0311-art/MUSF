using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace CustomFrameWork.Component
{
    /// <summary>
    /// 日志工具 全局唯一  bool:是不是调用输出控制台操作  bool:是不是写入数据到文本
    /// </summary>
    public partial class LogToolComponent : TCustomComponent<MainFactory, bool, bool>
    {
        /// <summary>
        /// 是不是写入数据到文本
        /// </summary>
        private static bool m_IsWrite = false;
        /// <summary>
        /// 是不是调用输出控制台操作
        /// </summary>
        private static bool m_IsConsoleShow = false;
        /// <summary>
        /// 写入的路径 Log
        /// </summary>
        private static string m_LogTxtPath;
        /// <summary>
        /// 写入的路径 Log+Warning
        /// </summary>
        private static string m_LogWarningTxtPath;
        /// <summary>
        /// 写入的路径 Log+Error
        /// </summary>
        private static string m_LogErrorTxtPath;
        /// <summary>
        /// 写入的次数
        /// </summary>
        private static int m_LogIndex;
        /// <summary>
        /// 写入的页数
        /// </summary>
        private static int m_LogPage;
        /// <summary>
        /// 写入的天数
        /// </summary>
        private static int m_LogDay;


        /// <summary>
        /// 文件夹路径
        /// </summary>
        private static string mLogTxtDirectoryPath;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="b_ConsoleShow">是不是调用输出控制台操作</param>
        /// <param name="b_Write">是不是调用写操作</param>
        public override void Awake(bool b_ConsoleShow, bool b_Write)
        {
            DateTime mDateTime = DateTime.Now;
            mLogTxtDirectoryPath = $"{Root.MainFactory.GetCustomComponent<ConfigInfoComponent>().GetConfigInfo("LogTxtPath")}/{mDateTime.Year}_{mDateTime.Month}/{OptionComponent.Options.ServerTypeName}/";
            if (Directory.Exists(mLogTxtDirectoryPath) == false)
            {
                Directory.CreateDirectory(mLogTxtDirectoryPath);
            }
            LogPathCheck();
            if (m_LogTxtPath == null) Console.WriteLine("LogTxtPath 路径为null");

            //Console.WriteLine($"是否存在日志文件:{File.Exists(mLogTxtPath)}");
            //if (File.Exists(mLogTxtPath))
            //{
            //    File.Delete(mLogTxtPath);
            //    Console.WriteLine("LogTxtPath 已删除");
            //}
            m_IsConsoleShow = b_ConsoleShow;
            m_IsWrite = b_Write;
            m_LogIndex = 0;
            m_LogPage = 1;
            m_LogDay = 0;
        }
        /// <summary>
        /// 清理组件
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;
            m_IsWrite = false;
            m_IsConsoleShow = false;
            m_LogIndex = 0;
            m_LogPage = 0;
            base.Dispose();
        }

        /// <summary>
        /// 路径检查
        /// </summary>
        /// <param name="b_LogWarning">是不是Warning</param>
        /// <param name="b_LogError">是不是Error</param>
        /// <param name="b_FileName">文件名字</param>
        private static void LogPathCheck(bool b_LogWarning = false, bool b_LogError = false, string b_FileName = null)
        {
            DateTime mDateTime = DateTime.Now;
            m_LogIndex++;
            if (m_LogDay != mDateTime.Day)
            {
                m_LogDay = mDateTime.Day;
                m_LogIndex = 0;
                m_LogPage = 1;

                mLogTxtDirectoryPath = $"{Root.MainFactory.GetCustomComponent<ConfigInfoComponent>().GetConfigInfo("LogTxtPath")}/{mDateTime.Year}_{mDateTime.Month}/{OptionComponent.Options.ServerTypeName}/";
                if (Directory.Exists(mLogTxtDirectoryPath) == false)
                {
                    Directory.CreateDirectory(mLogTxtDirectoryPath);
                }
            }
            else if (m_LogIndex >= 250000)
            {
                m_LogIndex = 0;
                m_LogPage++;
            }
            m_LogTxtPath = $"{mLogTxtDirectoryPath}{OptionComponent.Options.ServerTypeName}_{OptionComponent.Options.AppId.ToString()}_Log_{(b_FileName != null ? b_FileName:"")}{mDateTime.Month.ToString()}-{mDateTime.Day.ToString()}-{m_LogPage.ToString()}.txt";
            if (b_LogWarning)
                m_LogWarningTxtPath = $"{mLogTxtDirectoryPath}{OptionComponent.Options.ServerTypeName}_{OptionComponent.Options.AppId.ToString()}_LogWarning_{mDateTime.Month.ToString()}-{mDateTime.Day.ToString()}-{m_LogPage.ToString()}.txt";
            if (b_LogError)
                m_LogErrorTxtPath = $"{mLogTxtDirectoryPath}{OptionComponent.Options.ServerTypeName}_{OptionComponent.Options.AppId.ToString()}_LogError_{mDateTime.Month.ToString()}-{mDateTime.Day.ToString()}-{m_LogPage.ToString()}.txt";

        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="b_Message">信息</param>
        /// <param name="b_IsStackTrace">是否需要堆栈信息</param>
        /// <param name="b_FontColor">打印信息字体颜色</param>
        public static void Log(object b_Message, bool b_IsStackTrace = true, ConsoleColor b_FontColor = ConsoleColor.White)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            LogPathCheck();
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                Str = $"=>{OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:{(b_Message != null ? b_Message.ToString() : "Log message is null!")}\n{(b_IsStackTrace ? $"{Environment.StackTrace}\n\n" : "\n")}";
            }
            if (m_IsConsoleShow)
            {
                if (b_FontColor != ConsoleColor.White)
                {
                    Console.ForegroundColor = b_FontColor;
                    Console.Write(Str);
                    Console.ResetColor();
                }
                else
                    Console.Write(Str);
            }
            if (m_IsWrite)
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOG, m_LogTxtPath, Str);
        }


        /// <summary>
        /// Error
        /// </summary>
        /// <param name="b_Message"></param>
        /// <param name="b_IsStackTrace">是否需要堆栈信息</param>
        public static void Error(object b_Message, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            LogPathCheck(false, true);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                Str = $"=>{OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:{(b_Message != null ? b_Message.ToString() : "Error message is null!")}\n{(b_IsStackTrace ? $"{Environment.StackTrace}\n\n" : "\n")}";
            }
            if (m_IsConsoleShow)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Str);
                Console.ResetColor();
            }
            if (m_IsWrite)
            {
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOG, m_LogTxtPath, Str);
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOGERROR, m_LogErrorTxtPath, Str);
            }
        }
        /// <summary>
        /// Wraning
        /// </summary>
        /// <param name="b_Message"></param>
        /// <param name="b_IsStackTrace">是否需要堆栈信息</param>
        public static void Warning(object b_Message, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            LogPathCheck(true, false);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                Str = $"=>{OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:{(b_Message != null ? b_Message.ToString() : "Warning message is null!")}\n{(b_IsStackTrace ? $"{Environment.StackTrace}\n\n" : "\n")}";
            }
            if (m_IsConsoleShow)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(Str);
                Console.ResetColor();
            }

            if (m_IsWrite)
            {
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOG, m_LogTxtPath, Str);
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOGWARNING, m_LogWarningTxtPath, Str);
            }
        }

        /// <summary>
        /// 断言 
        /// </summary>
        /// <param name="b_ConditionalStatement">条件语句</param>
        /// <param name="b_IsStackTrace">是否需要堆栈信息</param>
        /// <param name="b_FontColor">打印信息字体颜色</param>
        public static void AssertLog(bool b_ConditionalStatement, bool b_IsStackTrace = true, ConsoleColor b_FontColor = ConsoleColor.White)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck();
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                Str = $"=>{OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:AssertLog is fail!\n{(b_IsStackTrace ? $"{Environment.StackTrace}\n\n" : "\n")}";
            }
            if (m_IsConsoleShow)
            {
                if (b_FontColor != ConsoleColor.White)
                {
                    Console.ForegroundColor = b_FontColor;
                    Console.Write(Str);
                    Console.ResetColor();
                }
                else
                    Console.Write(Str);
            }
            if (m_IsWrite)
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOG, m_LogTxtPath, Str);
        }
        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="b_ConditionalStatement">条件语句</param>
        /// <param name="b_Message">自定义输出字段</param>
        /// <param name="b_IsStackTrace">是否需要堆栈信息</param>
        /// <param name="b_FontColor">打印信息字体颜色</param>
        public static void AssertLog(bool b_ConditionalStatement, object b_Message, bool b_IsStackTrace = true, ConsoleColor b_FontColor = ConsoleColor.White)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck();
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                Str = $"=>{OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:{(b_Message != null ? b_Message.ToString() : "AssertLog message is null!")}\n{(b_IsStackTrace ? $"{Environment.StackTrace}\n\n" : "\n")}";
            }
            if (m_IsConsoleShow)
            {
                if (b_FontColor != ConsoleColor.White)
                {
                    Console.ForegroundColor = b_FontColor;
                    Console.Write(Str);
                    Console.ResetColor();
                }
                else
                    Console.Write(Str);
            }
            if (m_IsWrite)
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOG, m_LogTxtPath, Str);
        }
        /// <summary>
        /// 断言 
        /// </summary>
        /// <param name="b_ConditionalStatement">条件语句</param>
        /// <param name="b_IsStackTrace">是否需要堆栈信息</param>
        public static void AssertWarning(bool b_ConditionalStatement, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck(true, false);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                Str = $"=>{OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:AssertWarning is fail!\n{(b_IsStackTrace ? $"{Environment.StackTrace}\n\n" : "\n")}";
            }
            if (m_IsConsoleShow)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(Str);
                Console.ResetColor();
            }
            if (m_IsWrite)
            {
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOG, m_LogTxtPath, Str);
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOGWARNING, m_LogWarningTxtPath, Str);
            }
        }
        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="b_ConditionalStatement">条件语句</param>
        /// <param name="b_Message">自定义输出字段</param>
        /// <param name="b_IsStackTrace">是否需要堆栈信息</param>
        public static void AssertWarning(bool b_ConditionalStatement, object b_Message, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck(true, false);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                Str = $"=>{OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:{(b_Message != null ? b_Message.ToString() : "AssertWarning message is null!")}\n{(b_IsStackTrace ? $"{Environment.StackTrace}\n\n" : "\n")}";
            }
            if (m_IsConsoleShow)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(Str);
                Console.ResetColor();
            }
            if (m_IsWrite)
            {
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOG, m_LogTxtPath, Str);
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOGWARNING, m_LogWarningTxtPath, Str);
            }
        }
        /// <summary>
        /// 断言 
        /// </summary>
        /// <param name="b_ConditionalStatement">条件语句</param>
        /// <param name="b_IsStackTrace">是否需要堆栈信息</param>
        public static void AssertError(bool b_ConditionalStatement, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck(false, true);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                Str = $"=>{OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:AssertError is fail!\n{(b_IsStackTrace ? $"{Environment.StackTrace}\n\n" : "\n")}";
            }
            if (m_IsConsoleShow)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Str);
                Console.ResetColor();
            }
            if (m_IsWrite)
            {
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOG, m_LogTxtPath, Str);
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOGERROR, m_LogErrorTxtPath, Str);
            }
        }
        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="b_ConditionalStatement">条件语句</param>
        /// <param name="b_Message">自定义输出字段</param>
        /// <param name="b_IsStackTrace">是否需要堆栈信息</param>
        public static void AssertError(bool b_ConditionalStatement, object b_Message, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck(false, true);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                Str = $"=>{OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:{(b_Message != null ? b_Message.ToString() : "AssertError message is null!")}\n{(b_IsStackTrace ? $"{Environment.StackTrace}\n\n" : "\n")}";
            }
            if (m_IsConsoleShow)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Str);
                Console.ResetColor();
            }
            if (m_IsWrite)
            {
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOG, m_LogTxtPath, Str);
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOGERROR, m_LogErrorTxtPath, Str);
            }
        }


        public static void FileLog(string b_FileName, object b_Message, bool b_IsStackTrace = true, ConsoleColor b_FontColor = ConsoleColor.White)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            LogPathCheck(b_FileName: b_FileName);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                Str = $"=>{OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:{(b_Message != null ? b_Message.ToString() : "Log message is null!")}\n{(b_IsStackTrace ? $"{Environment.StackTrace}\n\n" : "\n")}";
            }
            if (m_IsConsoleShow)
            {
                if (b_FontColor != ConsoleColor.White)
                {
                    Console.ForegroundColor = b_FontColor;
                    Console.Write(Str);
                    Console.ResetColor();
                }
                else
                    Console.Write(Str);
            }
            if (m_IsWrite)
                Root.MainFactory.GetCustomComponent<ReadWriteComponent>()?.AddLogWriteAppendAsync(E_ReadWriteLock.LOG, m_LogTxtPath, Str);
        }
    }
}
