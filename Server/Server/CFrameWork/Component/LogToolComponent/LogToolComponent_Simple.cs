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
    public partial class LogToolComponent
    {
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="b_Message">信息</param>
        /// <param name="b_IsStackTrace">是否需要堆栈信息</param>
        /// <param name="b_FontColor">打印信息字体颜色</param>
        public static void SimpleLog(object b_Message, bool b_IsStackTrace = true, ConsoleColor b_FontColor = ConsoleColor.White)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            LogPathCheck();
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                StackFrame mStackFrame = new StackFrame(1, true);
                if (b_IsStackTrace == false)
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "Log message is null!")}\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
                else
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "Log message is null!")}   {mStackFrame.GetFileName()}:line {mStackFrame.GetFileLineNumber()}   {mStackFrame.GetMethod().Name}({string.Join(",", mStackFrame.GetMethod().GetParameters().ToList())})方法      {OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
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
        public static void SimpleError(object b_Message, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            LogPathCheck(false, true);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                StackFrame mStackFrame = new StackFrame(1, true);
                if (b_IsStackTrace == false)
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "Error message is null!")}\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
                else
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "Error message is null!")}   {mStackFrame.GetFileName()}:line {mStackFrame.GetFileLineNumber()}   {mStackFrame.GetMethod().Name}({string.Join(",", mStackFrame.GetMethod().GetParameters().ToList())})方法      {OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
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
        public static void SimpleWarning(object b_Message, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            LogPathCheck(true, false);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                StackFrame mStackFrame = new StackFrame(1, true);
                if (b_IsStackTrace == false)
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "Warning message is null!")}\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
                else
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "Warning message is null!")}   {mStackFrame.GetFileName()}:line {mStackFrame.GetFileLineNumber()}   {mStackFrame.GetMethod().Name}({string.Join(",", mStackFrame.GetMethod().GetParameters().ToList())})方法      {OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
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
        public static void AssertSimpleLog(bool b_ConditionalStatement, bool b_IsStackTrace = true, ConsoleColor b_FontColor = ConsoleColor.White)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck();
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                StackFrame mStackFrame = new StackFrame(1, true);
                if (b_IsStackTrace == false)
                    Str = $"=>AssertLog is fail!\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
                else
                    Str = $"=>AssertLog is fail!   {mStackFrame.GetFileName()}:line {mStackFrame.GetFileLineNumber()}   {mStackFrame.GetMethod().Name}({string.Join(",", mStackFrame.GetMethod().GetParameters().ToList())})方法      {OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
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
        public static void AssertSimpleLog(bool b_ConditionalStatement, object b_Message, bool b_IsStackTrace = true, ConsoleColor b_FontColor = ConsoleColor.White)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck();
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                StackFrame mStackFrame = new StackFrame(1, true);
                if (b_IsStackTrace == false)
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "AssertLog message is null!")}\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
                else
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "AssertLog message is null!")}   {mStackFrame.GetFileName()}:line {mStackFrame.GetFileLineNumber()}   {mStackFrame.GetMethod().Name}({string.Join(",", mStackFrame.GetMethod().GetParameters().ToList())})方法      {OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
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
        public static void AssertSimpleWarning(bool b_ConditionalStatement, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck(true, false);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                StackFrame mStackFrame = new StackFrame(1, true);
                if (b_IsStackTrace == false)
                    Str = $"=>AssertWarning is fail!\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
                else
                    Str = $"=>AssertWarning is fail!   {mStackFrame.GetFileName()}:line {mStackFrame.GetFileLineNumber()}   {mStackFrame.GetMethod().Name}({string.Join(",", mStackFrame.GetMethod().GetParameters().ToList())})方法      {OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
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
        public static void AssertSimpleWarning(bool b_ConditionalStatement, object b_Message, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck(true, false);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                StackFrame mStackFrame = new StackFrame(1, true);
                if (b_IsStackTrace == false)
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "AssertWarning message is null!")}\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
                else
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "AssertWarning message is null!")}   {mStackFrame.GetFileName()}:line {mStackFrame.GetFileLineNumber()}   {mStackFrame.GetMethod().Name}({string.Join(",", mStackFrame.GetMethod().GetParameters().ToList())})方法      {OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
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
        public static void AssertSimpleError(bool b_ConditionalStatement, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck(false, true);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                StackFrame mStackFrame = new StackFrame(1, true);
                if (b_IsStackTrace == false)
                    Str = $"=>AssertError is fail!\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
                else
                    Str = $"=>AssertError is fail!   {mStackFrame.GetFileName()}:line {mStackFrame.GetFileLineNumber()}   {mStackFrame.GetMethod().Name}({string.Join(",", mStackFrame.GetMethod().GetParameters().ToList())})方法      {OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
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
        public static void AssertSimpleError(bool b_ConditionalStatement, object b_Message, bool b_IsStackTrace = true)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            if (b_ConditionalStatement) return;
            LogPathCheck(false, true);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                StackFrame mStackFrame = new StackFrame(1, true);
                if (b_IsStackTrace == false)
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "AssertError message is null!")}\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
                else
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "AssertError message is null!")}   {mStackFrame.GetFileName()}:line {mStackFrame.GetFileLineNumber()}   {mStackFrame.GetMethod().Name}({string.Join(",", mStackFrame.GetMethod().GetParameters().ToList())})方法      {OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
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


        public static void FileSimpleLog(string b_FileName, object b_Message, bool b_IsStackTrace = true, ConsoleColor b_FontColor = ConsoleColor.White)
        {
            if (OptionComponent.Options.PointLog == 0) return;
            LogPathCheck(b_FileName: b_FileName);
            //throw new NotImplementedException($"ERROR:\n   {new StackTrace().GetFrame(0).GetMethod().Name}方法没有重写!\n{Environment.StackTrace}");
            string Str;
            {
                StackFrame mStackFrame = new StackFrame(1, true);
                if (b_IsStackTrace == false)
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "Log message is null!")}\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
                else
                    Str = $"=>{(b_Message != null ? b_Message.ToString() : "Log message is null!")}   {mStackFrame.GetFileName()}:line {mStackFrame.GetFileLineNumber()}   {mStackFrame.GetMethod().Name}({string.Join(",", mStackFrame.GetMethod().GetParameters().ToList())})方法      {OptionComponent.Options.ServerTypeName}-{OptionComponent.Options.AppId.ToString()} {MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} \tLogPage:{m_LogPage.ToString()},LogIndex:{m_LogIndex.ToString()}\n";
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
