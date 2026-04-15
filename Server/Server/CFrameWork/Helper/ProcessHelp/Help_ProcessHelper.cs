
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CustomFrameWork.Component;
using System.IO;

namespace CustomFrameWork
{
    public class Help_ProcessHelper
    {
        public static Process StartUp(string b_FileName, string b_Arguments, string b_WorkingDirectory = ".", bool b_WaitExit = false)
        {
            bool mRedirectStandardOutput = true;
            bool mRedirectStandardError = true;
            //设置为false, 可以重定向输入、输出和错误流
            bool mUseShellExecute = false;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                mRedirectStandardOutput = false;
                mRedirectStandardError = false;
                // 获取或设置一个值，该值指示是否使用操作系统外壳程序启动进程。
                mUseShellExecute = true;
            }
            if (b_WaitExit)
            {
                mRedirectStandardOutput = true;
                mRedirectStandardError = true;
                //设置为false, 可以重定向输入、输出和错误流
                mUseShellExecute = false;
            }

            ProcessStartInfo StartInfo = new ProcessStartInfo
            {
                // 不创建窗口
                CreateNoWindow = true,
                FileName = b_FileName,
                // 启动命令参数集
                Arguments = b_Arguments,
                WorkingDirectory = b_WorkingDirectory,
                RedirectStandardOutput = mRedirectStandardOutput,
                RedirectStandardError = mRedirectStandardError,
                UseShellExecute = mUseShellExecute
            };

            try
            {
                Process process = Process.Start(StartInfo);
                if (b_WaitExit)
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        LogToolComponent.Error($"程序:{b_FileName}启动错误!\n错误为:\n{process.StandardError.ReadToEnd()}\n输出为:{process.StandardOutput.ReadToEnd()}");
                    }
                }
                return process;
            }
            catch (Exception e)
            {
                LogToolComponent.Error($"路径: {Path.GetFullPath(b_WorkingDirectory)}, Command: {b_FileName} {b_Arguments}\nError:{e.Message}");
                return null;
            }
        }
    }
}
