using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using ETModel.Robot;
using NLog;
using ETHotfix;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using MongoDB.Bson;


namespace App
{
    internal static class Program
    {
        [DllImport("winmm")]
        static extern void timeBeginPeriod(int t);
        private static void Main(string[] args)
        {
            timeBeginPeriod(1);
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            try
            {
                TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs eventArgs) =>
                 {
                     //第一种是：
                     {
                         //eventArgs.SetObserved();
                         Log.Error(eventArgs.Exception);
                         LogManager.Shutdown();
                     }
                 };
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    Log.Fatal(e.ExceptionObject.ToString());
                    LogManager.Shutdown();
                };
                // 异步方法全部会回掉到主线程
                SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);
                NLogCustomizeObjectReflection();
                // 加载代码
                // 二选一
                CodeLoader.Instance.Start();
                // 性能调试模式，需要依赖hotfix项目
                //CodeLoader.Instance.StartDebug(typeof(HotfixHelper).Assembly);

                Root.MainFactory.AddCustomComponent<OptionComponent, string[]>(args);
                Root.MainFactory.AddCustomComponent<ConfigInfoComponent>();
                Root.MainFactory.AddCustomComponent<ReadWriteComponent>();
                Root.MainFactory.AddCustomComponent<LogToolComponent, bool, bool>(true, true);
                Root.MainFactory.AddCustomComponent<ServerManageComponent>();
                Root.MainFactory.AddCustomComponent<TimerComponent>();

                var startConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(OptionComponent.Options.AppId);
                if (!OptionComponent.Options.AppType.Is(startConfig.AppType))
                {
                    Log.Error("命令行参数apptype与配置不一致");
                    return;
                }
                CustomFrameWork.IdGenerater.AppId = startConfig.AppId;
                ETModel.IdGenerater.AppId = startConfig.AppId;

                // 初始化日志系统
                Log.logger.SetLogLevel(LogLevel.FromString(OptionComponent.Options.LogLevel));
                LogManager.Configuration.Variables["appType"] = $"{startConfig.AppType}";
                LogManager.Configuration.Variables["appId"] = $"{startConfig.AppId}";
                LogManager.Configuration.Variables["appTypeFormat"] = $"{startConfig.AppType,-8}";
                LogManager.Configuration.Variables["appIdFormat"] = $"{startConfig.AppId:0000}";

                Log.Info($"server start........................ {startConfig.AppId} {startConfig.AppType}");
           
                // 非核心去异步里面加载
                async void StartMainAsync()
                {
                    try
                    {
                        Root.MainFactory.AddCustomComponent<LanguageComponent>();

                        ReadConfigComponent mReadConfigComponent = Root.MainFactory.AddCustomComponent<ReadConfigComponent>();
                        await mReadConfigComponent.LoadConfigAsync();

                        //Game.EventSystem.Add(DLLType.Model, typeof(Game).Assembly);
                        //Game.EventSystem.Add(DLLType.Hotfix, DllHelper.GetHotfixAssembly());
                        //Game.EventSystem.Add(DLLType.Hotfix, Root.HotfixAssembly);
                        //Game.EventSystem.Add(DLLType.Hotfix, typeof(HotfixHelper).Assembly);

                        Root.MainFactory.AddCustomComponent<ConsoleComponent>();
                        //Game.Scene.AddComponent<ETModel.ETTimerComponent>();
                        Game.Scene.AddComponent<OpcodeTypeComponent>();
                        Game.Scene.AddComponent<MessageDispatcherComponent>();
                        Game.Scene.AddComponent<ETModel.ET.TimerComponent>();
                        Game.Scene.AddComponent<CoroutineLockComponent>();

                        // 根据不同的AppType添加不同的组件
                        switch (startConfig.AppType)
                        {
                            case AppType.Manager:
                                //Game.Scene.AddComponent<AppManagerComponent>();
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                Game.Scene.AddComponent<NetOuterComponent, string>(OptionComponent.Options.ServerOutIP);
                                break;
                            case AppType.DB:
                                var mDBComponent = Root.MainFactory.AddCustomComponent<DBComponent>();
                                Root.EventSystem.OnRun("DBComponent", mDBComponent);

                                //Root.MainFactory.AddCustomComponent<JointAreaSignalDetectionComponent>();
                                //Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                //Game.Scene.AddComponent<LocationProxyComponent>();

                                //Game.Scene.AddComponent<HttpComponent>();
                                break;

                            case AppType.Realm:
                                Root.MainFactory.AddCustomComponent<DBManagerComponent>();
                                Root.MainFactory.AddCustomComponent<DBProxyManagerComponent>();
                                Root.MainFactory.AddCustomComponent<AutoAreaComponent>();
                                //Game.Scene.AddComponent<MailboxDispatcherComponent>();
                                //Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                Game.Scene.AddComponent<NetOuterComponent, string>(OptionComponent.Options.ServerOutIP);
                                Root.MainFactory.AddCustomComponent<PingComponent>();
                                //Game.Scene.AddComponent<LocationProxyComponent>();
                                Root.MainFactory.AddCustomComponent<RealmGateAddressComponent>();

                                Root.MainFactory.AddCustomComponent<SMSMessageComponent>().Set(0, 1);

                                //Root.MainFactory.AddCustomComponent<ConsoleComponent>();
                                //Game.Scene.AddComponent<HttpComponent>();
                                break;
                            case AppType.Gate:
                                Root.MainFactory.AddCustomComponent<DBProxyManagerComponent>();

                                Root.MainFactory.AddCustomComponent<GateUserComponent>();
                                Root.MainFactory.AddCustomComponent<GatePlayerComponent>();
                                
                                //Game.Scene.AddComponent<MailboxDispatcherComponent>();
                                //Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                Game.Scene.AddComponent<NetOuterComponent, string>(OptionComponent.Options.ServerOutIP);
                                Root.MainFactory.AddCustomComponent<PingComponent>();
                                //Game.Scene.AddComponent<LocationProxyComponent>();
                                //Game.Scene.AddComponent<ActorMessageSenderComponent>();
                                //Game.Scene.AddComponent<ActorLocationSenderComponent>();
                                Root.MainFactory.AddCustomComponent<GateSessionKeyComponent>();
                                //Game.Scene.AddComponent<CoroutineLockComponent>();

                                //Root.MainFactory.AddCustomComponent<ConsoleComponent>();
                                break;
                            case AppType.Location:
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                //Game.Scene.AddComponent<LocationComponent>();
                                //Game.Scene.AddComponent<CoroutineLockComponent>();
                                break;
                            case AppType.Match:
                                Root.MainFactory.AddCustomComponent<DBProxyManagerComponent>();

                                Root.MainFactory.AddCustomComponent<GameAreaComponent>();


                                //Game.Scene.AddComponent<MailboxDispatcherComponent>();
                                //Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                //Game.Scene.AddComponent<LocationProxyComponent>();
                                //Game.Scene.AddComponent<ActorMessageSenderComponent>();
                                //Game.Scene.AddComponent<ActorLocationSenderComponent>();
                                //Game.Scene.AddComponent<CoroutineLockComponent>();

                                //Root.MainFactory.AddCustomComponent<ConsoleComponent>();
                                break;
                            case AppType.Game:

                                Root.MainFactory.AddCustomComponent<NameComponent>();
                                Root.MainFactory.AddCustomComponent<JointAreaSignalDetectionComponent>();

                                Root.MainFactory.AddCustomComponent<DBProxyManagerComponent>();
                                Root.MainFactory.AddCustomComponent<DBMongodbProxySaveManageComponent>();

                                Root.MainFactory.AddCustomComponent<ServerAreaManagerComponent>();
                                Root.MainFactory.AddCustomComponent<GameUserComponent>();
                                Root.MainFactory.AddCustomComponent<PlayerManageComponent>();
                                Root.MainFactory.AddCustomComponent<CombatSourceRecycleComponent>();

                                Root.MainFactory.AddCustomComponent<ItemUseRuleCreateBuilder>();
                                Root.MainFactory.AddCustomComponent<SynthesisRuleCreateBuilder>();
                                Root.MainFactory.AddCustomComponent<ItemConfigManagerComponent>();
                                Root.MainFactory.AddCustomComponent<ItemUpdatePropManagerComponent>();
                                Root.MainFactory.AddCustomComponent<ItemPriceComponent>();
                                Root.MainFactory.AddCustomComponent<ItemCustomAttrMethodCreateBuilder>();
                                Root.MainFactory.AddCustomComponent<ItemCustomDropComponent>();
                                Root.MainFactory.AddCustomComponent<ItemDefaultDropComponent>();
                                Root.MainFactory.AddCustomComponent<ItemSetManager>();
                                Root.MainFactory.AddCustomComponent<ItemSocketManager>();
                                Root.MainFactory.AddCustomComponent<TreasureChestManager>();
                                Root.MainFactory.AddCustomComponent<BackpackRouterRuleCreateBuilder>();

                                Root.MainFactory.AddCustomComponent<PropertyCreateBuilder>();
                                Root.MainFactory.AddCustomComponent<SkillCreateBuilder>();
                                Root.MainFactory.AddCustomComponent<BattleMasterCreateBuilder>();
                                Root.MainFactory.AddCustomComponent<PetsSkillCreateBuilder>();
                                //Root.MainFactory.AddCustomComponent<MapManageComponent>();
                                Root.MainFactory.AddCustomComponent<ExcAttrEntryManagerComponent>();
                                Root.MainFactory.AddCustomComponent<ItemAttrEntryManager>();
                                Root.MainFactory.AddCustomComponent<ChatManageComponent>();
                                Root.MainFactory.AddCustomComponent<TeamManageComponent>();

                                Root.MainFactory.AddCustomComponent<GameTaskConfigManager>();
                                Root.MainFactory.AddCustomComponent<GameTaskActionCreateBuilder>();
                                Root.MainFactory.AddCustomComponent<GameTaskRewardCreateBuilder>();
                                Root.MainFactory.AddCustomComponent<ShopMallComponent>();
                                Root.MainFactory.AddCustomComponent<CumulativeRechargeGiftComponent>();
                                Root.MainFactory.AddCustomComponent<LotteryPoolComponent>();


                                Root.MainFactory.AddCustomComponent<IpCacheComponent>();
#if !DEVELOP
                                //Root.MainFactory.AddCustomComponent<MySqlComponent>(); 
#endif
                                //Game.Scene.AddComponent<MailboxDispatcherComponent>();
                                //Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                //Game.Scene.AddComponent<HttpComponent>();
                                //Game.Scene.AddComponent<LocationProxyComponent>();
                                //Game.Scene.AddComponent<ActorMessageSenderComponent>();
                                //Game.Scene.AddComponent<ActorLocationSenderComponent>();
                                //Game.Scene.AddComponent<CoroutineLockComponent>();
                                //Root.MainFactory.AddCustomComponent<MailComponent>();
                                //Root.MainFactory.AddCustomComponent<ConsoleComponent>();
                                Root.MainFactory.AddCustomComponent<GameMasterComponent>();
                                break;
                            case AppType.Map:
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                //Game.Scene.AddComponent<UnitComponent>();
                                //Game.Scene.AddComponent<LocationProxyComponent>();
                                //Game.Scene.AddComponent<ActorMessageSenderComponent>();
                                //Game.Scene.AddComponent<ActorLocationSenderComponent>();
                                //Game.Scene.AddComponent<MailboxDispatcherComponent>();
                                //Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
                                //Game.Scene.AddComponent<PathfindingComponent>();
                                //Game.Scene.AddComponent<CoroutineLockComponent>();
                                break;
                            case AppType.Benchmark:
                                Game.Scene.AddComponent<NetOuterComponent>();
                                //Game.Scene.AddComponent<BenchmarkComponent, string>(clientConfig.Address);
                                break;
                            case AppType.BenchmarkWebsocketServer:
                                //Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
                                break;
                            case AppType.BenchmarkWebsocketClient:
                                Game.Scene.AddComponent<NetOuterComponent>();
                                //Game.Scene.AddComponent<WebSocketBenchmarkComponent, string>(clientConfig.Address);
                                break;
                            case AppType.MGMT:
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                Root.MainFactory.AddCustomComponent<DBProxyManagerComponent>();
                                Root.MainFactory.AddCustomComponent<DBMongodbProxySaveManageComponent>();
                                //Game.Scene.AddComponent<CoroutineLockComponent>();
                                Root.MainFactory.AddCustomComponent<JointAreaSignalDetectionComponent>();
                                Root.MainFactory.AddCustomComponent<WarAllianceComponent>();
                                Root.MainFactory.AddCustomComponent<RankComponent>();
                                Root.MainFactory.AddCustomComponent<MGMTTreasureHouse>();
                                Root.MainFactory.AddCustomComponent<LoginInfoRecordComponent>();
                                Root.MainFactory.AddCustomComponent<NameLockComponent>();
                                //Root.MainFactory.AddCustomComponent<ConsoleComponent>();
                                break;
                            case AppType.LoginCenter:
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                Root.MainFactory.AddCustomComponent<IpCacheComponent>();
                                Root.MainFactory.AddCustomComponent<LoginInfoRecordComponent>();
                                Root.MainFactory.AddCustomComponent<DBProxyManagerComponent>();
                                Root.MainFactory.AddCustomComponent<DBMongodbProxySaveManageComponent>();
                                Root.MainFactory.AddCustomComponent<PromotionComponent>();
                                break;
                            case AppType.UpdateDB:
                                var mDBComponent2 = Root.MainFactory.AddCustomComponent<DBComponent>();
                                Root.EventSystem.OnRun("DBComponent", mDBComponent2);

                                switch (OptionComponent.Options.AppendValue)
                                {
                                    case "t20230228": ETModel.t20230228.UpdateDB_ItemData.StartAsync().Coroutine(); break;
                                    case "t20230306_1": ETModel.t20230306.UpdateDB_GameTaskData.StartAsync().Coroutine(); break;
                                    case "t20230306_2": ETModel.t20230306.UpdateDB_GamePlayerData.StartAsync().Coroutine(); break;
                                    case "t20230306_3": ETModel.t20230306.UpdateDB_AccountInfo.StartAsync().Coroutine(); break;
                                    case "t20231214": ETModel.t20231214.UpdateDB_ItemData_SetId.StartAsync().Coroutine(); break;
                                }
                                
                                break;
                            case AppType.GM:
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);

                                Root.MainFactory.AddCustomComponent<DBProxyManagerComponent>();
                                Root.MainFactory.AddCustomComponent<DBMongodbProxySaveManageComponent>();

                                Root.MainFactory.AddCustomComponent<GameUserComponent>();
                                Root.MainFactory.AddCustomComponent<PlayerManageComponent>();

                                Root.MainFactory.AddCustomComponent<OnlineStatisticsComponent>();
                                //Root.MainFactory.AddCustomComponent<GMKeyComponent>();
                                Root.MainFactory.AddCustomComponent<ReadConfigComponent>();
                                Root.MainFactory.AddCustomComponent<ItemConfigManagerComponent>();
                                Game.Scene.AddComponent<HttpComponent>();
                                break;
                            case AppType.Robot:
                                Game.Scene.AddComponent<RobotManagerComponent>();
                                Game.Scene.AddComponent<ThreadPoolComponent>();
                                Game.Scene.AddComponent<NumericWatcherComponent>();

                                Game.Scene.AddComponent<AIDispatcherComponent>();
                                Game.Scene.AddComponent<AIConfigManager>();

                                Game.Scene.AddComponent<TransferPointManager>();

                                Root.MainFactory.AddCustomComponent<ItemConfigManagerComponent>();
                                break;
                            case AppType.PayServer:
                                Game.Scene.AddComponent<NetInnerComponent, string>(OptionComponent.Options.ServerInnerIP);
                                Root.MainFactory.AddCustomComponent<DBProxyManagerComponent>();
                                Root.MainFactory.AddCustomComponent<DBMongodbProxySaveManageComponent>();
                                Root.MainFactory.AddCustomComponent<GameUserComponent>();
                                Root.MainFactory.AddCustomComponent<PlayerManageComponent>();
                                Game.Scene.AddComponent<HttpComponent>();
                                break;
                            default:
                                throw new Exception($"命令行参数没有设置正确的AppType: {startConfig.AppType}");
                        }

                        Log.Console("初始化完毕!");
                    }
                    catch (Exception e)
                    {
                        Log.Fatal("加载服务器组件异常",e);
                        Log.Console("服务器启动失败!");
                    }
                }
                StartMainAsync();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }


            Stopwatch sw = new Stopwatch();
            while (true)
            {
                try
                {
                    sw.Restart();
                    Thread.Sleep(1);
                    OneThreadSynchronizationContext.Instance.Update();
                    Root.EventSystem.Update();
                    Game.EventSystem.Update();
                    sw.Stop();
                    if (OptionComponent.Options.AppType == AppType.Game || OptionComponent.Options.AppType == AppType.Gate)
                    {
                        Log.Info($"Frame time = {sw.ElapsedMilliseconds}");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        private static void Run()
        {

        }

        private static void NLogCustomizeObjectReflection()
        {
            // 注册序列化方法 '{@value}'
            LogManager.Setup().SetupSerialization(s => s.RegisterObjectTransformation<IMessage>(ex => Newtonsoft.Json.JsonConvert.SerializeObject(ex)));
        }

    }
}
