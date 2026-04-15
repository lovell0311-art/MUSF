
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [EventMethod(typeof(ConsoleComponent), EventSystemType.INIT)]
    public class ConsoleComponentEvent : ITEventMethodOnInit<ConsoleComponent>
    {
        public void OnInit(ConsoleComponent b_CustomComponent)
        {
            b_CustomComponent.OnInit();
        }
    }

    [EventMethod(typeof(ConsoleComponent), EventSystemType.LOAD)]
    public class ConsoleComponentLoadEvent : ITEventMethodOnLoad<ConsoleComponent>
    {
        public override void OnLoad(ConsoleComponent b_CustomComponent)
        {
            b_CustomComponent.OnInit();
        }
    }

    public static class ConsoleComponentSystem
    {
        public static void OnInit(this ConsoleComponent b_Component)
        {
            b_Component.OnlyRunUpdate();

            b_Component.RegisterCommandLine().Coroutine();

            b_Component.StartCommandLineAsync().Coroutine();
        }

        private async static Task RegisterCommandLine(this ConsoleComponent b_Component)
        {
            if (b_Component.ConsoleCommandLineDic.Count > 0)
            {
                b_Component.ConsoleCommandLineDic.Clear();
            }

            Type[] types = Root.HotfixAssembly.GetTypes();
            for (int i = 0, len = types.Length; i < len; i++)
            {
                Type type = types[i];

                if (type.IsDefined(typeof(ConsoleCommandLineAttribute), false))
                {
                    object[] mAttributes = type.GetCustomAttributes(typeof(ConsoleCommandLineAttribute), false);
                    if (mAttributes.Length > 0)
                    {
                        ConsoleCommandLineAttribute mCommandLineAttribute = mAttributes[0] as ConsoleCommandLineAttribute;
                        string commandLineName = mCommandLineAttribute.ConsoleCommandLineName;
                        if (b_Component.ConsoleCommandLineDic.ContainsKey(commandLineName))
                        {
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:已经存在对象的网络包对象,{mCommandLineAttribute.ConsoleCommandLineName} 注册失败\n{Environment.StackTrace}\n\n");
                        }
                        b_Component.ConsoleCommandLineDic[commandLineName] = type;
                    }
                }
            }
        }

        private async static Task StartCommandLineAsync(this ConsoleComponent b_Component)
        {
            b_Component.CancellationTokenSource = new CancellationTokenSource();
            bool IsQuit = false;
            while (b_Component.IsRunUpdate && b_Component.IsDisposeable == false)
            {
                if (IsQuit)
                {
                    Log.Console($"退出旧命令!");
                    return;
                }

                ModeContexCommandlineComponent modeContexComponent = b_Component.GetCustomComponent<ModeContexCommandlineComponent>();

                string reader = await Task.Run(() =>
                {
                    Console.Write($"{(modeContexComponent != null ? modeContexComponent.Contex : "")}> ");
                    return Console.ReadLine();
                }, b_Component.CancellationTokenSource.Token);

                if (reader == null)
                {
                    Log.Warning("控制台输入流不可用，跳过命令监听。");
                    return;
                }

                reader = reader.ToLower().Trim();

                switch (reader)
                {
                    case ConsoleCommandLinePath.Quit:
                        b_Component.RemoveCustomComponent<ModeContexCommandlineComponent>();
                        break;
                    case "load":
                        Log.Console($"命令 : 热重载 Hotfix");

                        CodeLoader.Instance.LoadHotfix();
                        // 有两种方案
                        // 1.清除对象池
                        // 2.禁止hotfix中写继承 ADataContextSource 的 class (正确的做法，后续会改为这种)

                        // TODO 清除对象池，有部分对象是hotfix中的
                        Root.CreateBuilder.Clear();
                        // 重新加载配置
                        Root.MainFactory.GetCustomComponent<ReadConfigComponent>().Clear();
                        Root.MainFactory.GetCustomComponent<ReadConfigComponent>().LoadConfig();


                        // ET框架运行Load事件
                        Game.EventSystem.Load();
                        // 自定义框架运行Load事件
                        Root.EventSystem.Load();

                        IsQuit = true;
                        break;
                    case "threadid":
                        Log.Console($"命令 : 线程id:{Thread.CurrentThread.ManagedThreadId}");
                        break;
                    case "Exit":
                        System.Environment.Exit(0);
                        break;
                    default:
                        {
                            //string[] readerLines = reader.Split(" ");
                            //string mConsoleCommandLineName = readerLines.Length > 0 ? readerLines[0] : "";

                            int subLen = reader.IndexOf(" ");
                            string mConsoleCommandLineName = modeContexComponent != null ? modeContexComponent.Contex
                                                                                : (subLen == -1 ? reader : reader.Substring(0, subLen));
                            if (b_Component.ConsoleCommandLineDic.TryGetValue(mConsoleCommandLineName, out Type type))
                            {
                                C_ConsoleCommandLine mCommandLine = Root.CreateBuilder.GetInstance<C_ConsoleCommandLine>(type);

                                if (subLen != -1 && modeContexComponent == null)
                                {
                                    modeContexComponent = b_Component.AddCustomComponent<ModeContexCommandlineComponent>();
                                    modeContexComponent.Contex = mConsoleCommandLineName;
                                }

                                string mConsoleCommandLineStr = subLen == -1 ? reader : reader.Substring(subLen).TrimStart();
                                await mCommandLine.Run(modeContexComponent, mConsoleCommandLineName, mConsoleCommandLineStr);

                                mCommandLine.Dispose();

                                if (modeContexComponent == null)
                                {
                                    modeContexComponent = b_Component.AddCustomComponent<ModeContexCommandlineComponent>();
                                    modeContexComponent.Contex = mConsoleCommandLineName;
                                }
                            }
                            else
                            {
                                Log.Console($"无效命令 : {reader}");
                            }
                        }
                        break;
                }
            }
        }
    }
}
