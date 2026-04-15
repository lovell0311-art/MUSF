using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Net.Sockets;
using System;
using UnityEngine.Analytics;
using System.Linq;

namespace ETHotfix
{
    [ObjectSystem]
    public class SessionHelperAwake : AwakeSystem<SessionHelper>
    {
        public override void Awake(SessionHelper self)
        {
            SessionHelper.Instance = self;

            SdkCallBackComponent.Instance.LogoutCallBack = self.LoginOut;
        }
    }
    /// <summary>
    /// 会话助手
    /// </summary>
    public class SessionHelper : Component
    {
        public enum ReconnectStatus
        {
            /// <summary> 重连成功 </summary>
            Success,
            /// <summary> 重连失败 </summary>
            Fail,
            /// <summary> 继续重连 </summary>
            Continue,
        }


        public static SessionHelper Instance;
        /// <summary>
        /// 当前重连的次数
        /// </summary>
        private int curReConnectTime = 1;
        /// <summary>
        /// 最大重连次数 默认是3次
        /// </summary>
        private int maxReConnectTime = 15;
        /// <summary>
        /// 重连工作完成 默认为true 开始重连为false 当连接建立成功 或超过最大重连次数时时返回true
        /// </summary>
        public bool ReconnectFinished = true;


        public void ForceTryReConnect()
        {
            UIComponent.Instance.VisibleUI(UIType.UIHint, "与服务器连接丢失，将重新与服务器建立连接");
            LogCollectionComponent.Instance.Info("#断线重连# 与服务器连接丢失，将重新与服务器建立连接");
            this.ReconnectFinished = false;
            //TryReconnectAfter30SecondIfFailed();
            StartReconnect().Coroutine();
        }

        /// <summary>
        /// 定时回调
        /// </summary>
        public void TryReconnectAfter30SecondIfFailed()
        {

            if (this.curReConnectTime >= this.maxReConnectTime || this.ReconnectFinished == true)
            {
                if (this.curReConnectTime >= this.maxReConnectTime)
                {

                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText("重连失败，请尝试重进游戏");
                    LogCollectionComponent.Instance.Info("#断线重连# 重连失败，请尝试重进游戏");
                    uIConfirmComponent.AddActionEvent(() => { uIConfirmComponent.QuitGame(); });
                }
                if (this.ReconnectFinished == true)
                {
                    Log.DebugWhtie("Reconnect succ ... STOP RECONNECT!!!");
                }
                this.ReconnectFinished = true;
                this.curReConnectTime = 0;
                return;
            }
            //重连开始

            UIComponent.Instance.VisibleUI(UIType.UIHint, $"自动重连 第[{this.curReConnectTime}/{this.maxReConnectTime}]尝试...");
            LogCollectionComponent.Instance.Info($"#断线重连# 自动重连 第[{this.curReConnectTime}/{this.maxReConnectTime}]尝试...");
            SendReEnterEnrollMessage().Coroutine();

            //30秒后重试判断是否重连成功
            TimerComponent.Instance.RegisterTimeCallBack(30 * 1000, TryReconnectAfter30SecondIfFailed);
            //重连计数加1
            this.curReConnectTime++;
        }


        public async ETVoid StartReconnect()
        {
            bool reconnectingCancelToken = false;
            bool waitTimeCancelToken = false;

            int nextReconnectTime = 0;
            UIConfirmComponent uIConfirmComponent = null;

            void ShowConfirmUI(string tipText)
            {
                uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText(tipText);
                uIConfirmComponent.AddActionEvent(() =>
                {
                    // 立即重连
                    waitTimeCancelToken = true;
                });
                uIConfirmComponent.AddCancelEventAction(() =>
                {
                    reconnectingCancelToken = true;
                    waitTimeCancelToken = true;
                    uIConfirmComponent.QuitGame();
                });
            }

            async ETVoid Reconnecting()
            {
                for (int i = 0; i < 1000; ++i)
                {
                    string dots = new string('.', i % 3 + 1);
                    // 让窗口一直显示，防止点击确实按钮后，窗口消失
                    ShowConfirmUI($"重连中{dots}");
                    await TimerComponent.Instance.WaitAsync(300);
                    if (reconnectingCancelToken) return;
                }
            }


            for (curReConnectTime = 0; curReConnectTime < maxReConnectTime; ++curReConnectTime)
            {
                waitTimeCancelToken = false;
                for (int time = nextReconnectTime; time >= 0; --time)
                {
                    // 让窗口一直显示，防止点击确实按钮后，窗口消失
                    ShowConfirmUI($"与服务器连接中断，{time}秒后再次重连。");
                    // 有bug Wait无法取消
                    //await TimerComponent.Instance.WaitAsync(1000, waitTimeCancelToken.Token);
                    await TimerComponent.Instance.WaitAsync(1000);
                    if (waitTimeCancelToken) break;
                }
                if (nextReconnectTime == 0)
                {
                    nextReconnectTime = 2;
                }
                else
                {
                    nextReconnectTime += nextReconnectTime / 2;
                    if (nextReconnectTime > 30) nextReconnectTime = 30;
                }


                reconnectingCancelToken = false;

                Reconnecting().Coroutine();

                // 开始重连
                LogCollectionComponent.Instance.Info($"#断线重连# 开始第 {curReConnectTime} 次重连");

                ReconnectStatus status = await SendReEnterEnrollMessage();
                reconnectingCancelToken = true;

                LogCollectionComponent.Instance.Info($"#断线重连# 第 {curReConnectTime} 次重连结束 status:{status}");


                if (status == ReconnectStatus.Fail)
                {
                    // 重连失败
                    uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"重连失败，请尝试重进游戏!");
                    uIConfirmComponent.AddActionEvent(() =>
                    {
                        uIConfirmComponent.QuitGame();
                    });
                    uIConfirmComponent.AddCancelEventAction(() =>
                    {
                        uIConfirmComponent.QuitGame();
                    });

                    LogCollectionComponent.Instance.Info("#断线重连# 重连失败");
                    return;
                }
                else if (status == ReconnectStatus.Success)
                {
                    // 重连成功
                    this.ReconnectFinished = true;
                    this.curReConnectTime = 0;
                    GlobalDataManager.IsStartReConnect = false;

                    UIComponent.Instance.InVisibilityUI(UIType.UIConfirm);
                    LogCollectionComponent.Instance.Info("#断线重连# 重连成功");
                    return;
                }
                else if (status == ReconnectStatus.Continue)
                {
                    // 继续重连
                }
                else
                {
                    Log.Error("未知的重连状态");
                    LogCollectionComponent.Instance.Info("#断线重连# 未知的重连状态");
                }

            }

            uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
            uIConfirmComponent.SetTipText($"重连超时，请尝试重进游戏!");
            uIConfirmComponent.AddActionEvent(() =>
            {
                uIConfirmComponent.QuitGame();
            });
            uIConfirmComponent.AddCancelEventAction(() =>
            {
                uIConfirmComponent.QuitGame();
            });

            LogCollectionComponent.Instance.Info("#断线重连# 重连超时");
        }

        /// <summary>
        /// 重连
        /// </summary>
        public async ETTask<ReconnectStatus> SendReEnterEnrollMessage()
        {

            //那么就根据网关地址创建一个新的Session 连接到网关去
            ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalDataManager.Address);
            //在Scene实体中添加SessionComponent组件 并且缓存Session对象 以后就可以直接获取来发送消息
            ETModel.Game.Scene.GetComponent<ETModel.SessionComponent>().Session = gateSession;
            //这里跟上面逻辑一样,创建热更层的Session,关联到主工程
            Game.Scene.GetComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
            //请求网关
            for (int i = 0; i < 10; i++)
            {
                IResponse request = await SessionComponent.Instance.Session.Call(new C2G_LoginGateRequest()
                {
                    Key = long.Parse(GlobalDataManager.GateLoginKey),
                    ChannelId = "123112414",//运营商id
                });
                if (request.Error != 0)
                {
                    LogCollectionComponent.Instance.Info($"#Session# LoginGate Error:{request.Error}");
                    if (Enum.IsDefined(typeof(System.Net.Sockets.SocketError), request.Error))
                    {
                        // 网络还未通
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "网络异常");
                        LogCollectionComponent.Instance.Info("#断线重连# 网络异常");
                        return ReconnectStatus.Continue;
                    }
                    else if (request.Error == 111)
                    {
                        // 正在下线角色，等1秒后重试
                        G2C_LoginGateResponse g2CLoginGate = request as G2C_LoginGateResponse;
                        if (g2CLoginGate != null)
                            GlobalDataManager.GateLoginKey = g2CLoginGate.NewKey.ToString();
                        UIComponent.Instance.VisibleUI(UIType.UIHint, request.Error.GetTipInfo());
                        LogCollectionComponent.Instance.Info("#断线重连# 正在下线角色，等1秒后重试");
                    }
                    else
                    {
                        // 重连失败，服务器拒绝重连请求
                        LogCollectionComponent.Instance.Info("#断线重连# 重连失败，服务器拒绝重连请求");
                        return ReconnectStatus.Fail;
                    }
                }
                else
                {
                    // 重连成功
                    G2C_LoginGateResponse g2CLoginGate = request as G2C_LoginGateResponse;
                    if (g2CLoginGate != null)
                        GlobalDataManager.GateLoginKey = g2CLoginGate.NewKey.ToString();
                    LogCollectionComponent.Instance.Info($"#Session# LoginGate 成功 session.Id:{SessionComponent.Instance.Session.session.Id}");
                    SessionComponent.Instance.Session.AddComponent<PingComponent>();
                    break;
                }
                await TimerComponent.Instance.WaitAsync(1000);
            }


            //清理实体数据

            RoleEntity localRole = UnitEntityComponent.Instance?.LocalRole;
            BufferComponent localRoleBuffer = localRole?.GetComponent<BufferComponent>();
            localRoleBuffer?.ClearBuffer();
            UnitEntityComponent.Instance?.Clear(false);
            //进入区服
            G2C_LoginSystemEnterGameAreaMessage g2C_EnterGameAreaMessage = (G2C_LoginSystemEnterGameAreaMessage)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemEnterGameAreaMessage
            {
                GameAreaId = GlobalDataManager.EnterZoneID,//大区id
                LineId = GlobalDataManager.EnterLineID//线路id
            });
           // Log.DebugBrown("G2C_LoginSystemEnterGameAreaMessage==>" + JsonHelper.ToJson(g2C_EnterGameAreaMessage.GameOccupation));
            //提示错误信息
            if (g2C_EnterGameAreaMessage.Error != 0)
            {
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText(g2C_EnterGameAreaMessage.Error.GetTipInfo(), true);
                LogCollectionComponent.Instance.Info($"#断线重连# G2C_LoginSystemEnterGameAreaMessage.Error:{g2C_EnterGameAreaMessage.Error}");
                return ReconnectStatus.Fail;
            }
            else
            {
                RoleArchiveInfoManager.Instance.CanCreatRoleList = g2C_EnterGameAreaMessage.GameOccupation.ToList();
            }
            if (SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>() == SceneName.ChooseRole)
            {
                // 正在切换角色
                return ReconnectStatus.Success;
            }

            G2C_StartGameGamePlayerResponse g2C_GamePlayerStartGameResponse = (G2C_StartGameGamePlayerResponse)await SessionComponent.Instance.Session.Call(new C2G_StartGameGamePlayerRequest
            {
                GameUserId = UnitEntityComponent.Instance.LocaRoleUUID
            });
            if (g2C_GamePlayerStartGameResponse.Error != 0)
            {
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText($"{g2C_GamePlayerStartGameResponse.Error.GetTipInfo()}", true);
                LogCollectionComponent.Instance.Info($"#断线重连# G2C_StartGameGamePlayerResponse.Error:{g2C_GamePlayerStartGameResponse.Error}");
                return ReconnectStatus.Fail;
            }
            else
            {
                //创建玩家
                LogCollectionComponent.Instance.GameUserId = UnitEntityComponent.Instance.LocaRoleUUID;
                // UnitEntityComponent.Instance.LocalRole = UnitEntityFactory.CreatLocalRole();
                // UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.OccupationLevel, RoleArchiveInfoManager.Instance.curSelectRoleArchiveInfo.ClassLev);//缓存 转职等级·

                //请求获取 玩家的场景 坐标信息 以及属性信息
                G2C_ReadyResponse g2C_Ready = (G2C_ReadyResponse)await SessionComponent.Instance.Session.Call(new C2G_ReadyRequest { });
                if (g2C_Ready.Error != 0)
                {
                    Log.DebugRed($"{g2C_Ready.Error.GetTipInfo()}");
                    LogCollectionComponent.Instance.Info($"#断线重连# G2C_ReadyResponse.Error:{g2C_Ready.Error}");
                    return ReconnectStatus.Fail;
                }

                G2C_EnterChatRoom g2C_EnterChatRoom = (G2C_EnterChatRoom)await SessionComponent.Instance.Session.Call(new C2G_EnterChatRoom
                {
                    ChatRoomID = GlobalDataManager.EnterZoneID
                });
                if (g2C_EnterChatRoom.Error != 0)
                {
                    Log.DebugRed($"请求进入聊天房间错误:{g2C_EnterChatRoom.Error.GetTipInfo()}");
                    LogCollectionComponent.Instance.Info($"#断线重连# G2C_EnterChatRoom.Error:{g2C_EnterChatRoom.Error}");
                    return ReconnectStatus.Fail;
                }
                else
                {

                    ChatMessageDataManager.valuePairs[E_ChatType.World] = GlobalDataManager.EnterZoneID;
                }
                this.ReconnectFinished = true;
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"重连成功");
                LogCollectionComponent.Instance.Info("#断线重连# 重连成功");
                TeamDatas.Clear();//清理队伍数据
                UIMainComponent.Instance.ResetTeamWhenChangeLine();
                if (RoleOnHookComponent.Instance.IsOnHooking)
                {
                    UIMainComponent.Instance.StopOnHook();
                    UIMainComponent.Instance.HookTog.isOn = true;
                }

                //G2C_GetBeginnerGuideStatus g2C_GetBeginner = (G2C_GetBeginnerGuideStatus)await SessionComponent.Instance.Session.Call(new C2G_GetBeginnerGuideStatus { });
                //if (g2C_GetBeginner.Error != 0)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetBeginner.Error.GetTipInfo());
                //    LogCollectionComponent.Instance.Info($"#断线重连# G2C_GetBeginnerGuideStatus.Error:{g2C_GetBeginner.Error}");
                //    return ReconnectStatus.Fail;
                //}
                //else
                //{
                //    BeginnerGuideData.BeginnerGuideSata = g2C_GetBeginner.Value;
                //    BeginnerGuideData.BeginnerGuideCountTime = true;
                //    //UIMainComponent.Instance.cheakBeginner();
                //    UIMainComponent.Instance.SetBeginnerGuide(BeginnerGuideData.IsCompleteTrigger(46, 45) || BeginnerGuideData.IsCompleteTrigger(49, 45) || BeginnerGuideData.IsCompleteTrigger(54, 53) || BeginnerGuideData.IsCompleteTrigger(59, 58));
                //}

                GuideComponent.Instance.CheckIsShowGuide();
            }
            return ReconnectStatus.Success;
        }

        /// <summary>
        /// 切换账号
        /// </summary>
        public void LoginOut(string st)
        {
            GlobalDataManager.IsLoginOut = true;//切换账号
            //BeginnerGuideData.BeginnerGuideCountTime = false;
            //BeginnerGuideData.BeginnerGuideSata = -1;
            GuideComponent.Instance.LoginOut();
            UIComponent.Instance.VisibleUI(UIType.UISceneLoading);//场景加载面板
            ChangeScene("Init_Login", SceneComponent.Instance.CurrentSceneName).Coroutine();


            FriendListData.Clear();//清理好友缓存数据
            TeamDatas.Clear();//清理队伍数据
            TaskDatas.ClearTask();//清理任务信息
            KnapsackItemsManager.ClearKnapsackItems();//清理背包数据
            SoundComponent.Instance.Clear();//清理当前场景的音效
            TreasureMapComponent.Instance.Clear();// 宝藏小地图icon

            if (UIMainComponent.Instance != null)
            {
                UIMainComponent.Instance.ClearTask();//清理当前角色的任务
                UIMainComponent.Instance.StopOnHook();
                UIMainComponent.Instance.medicineEntity_Hp.Num = 0;
                UIMainComponent.Instance.medicineEntity_Mp.Num = 0;
            }
            CameraFollowComponent.Instance.followTarget = null;
            //清理实体数据

            UnitEntityComponent.Instance.Clear();


            UIComponent.Instance.Remove(UIType.UIMainCanvas);
            UIComponent.Instance.Clean();
            GlobalDataManager.GCClear();
            UIComponent.Instance.VisibleUI(UIType.UILogin);//显示登录面板

            ETModel.Game.Scene.GetComponent<ETModel.SessionComponent>().Session.Dispose();
            ETModel.Game.Scene.RemoveComponent<ETModel.SessionComponent>();
            Game.Scene.GetComponent<SessionComponent>().Session.Dispose();
            Game.Scene.RemoveComponent<SessionComponent>();

            async ETVoid ChangeScene(string sceneName, string nowSceneName)
            {

                await AssetBundleComponent.Instance.LHLoadBundleAsync(sceneName.StringToAB());
                // 切换到map场景
                using (SceneChangeComponent sceneChangeComponent = ETModel.Game.Scene.AddComponent<SceneChangeComponent>())
                {

                    await sceneChangeComponent.ChangeSceneAsync(sceneName);
                }
                SceneComponent.Instance.CurrentSceneName = sceneName;
                CameraComponent.Instance.MainCamera.transform.position = new Vector3(4.05f, 39, -389);
                CameraComponent.Instance.MainCamera.transform.rotation = Quaternion.Euler(11, 0, 0); ;
                AssetBundleComponent.Instance.UnloadBundle(nowSceneName.StringToAB());//卸载之前场景资源
            }
        }
    }
}
