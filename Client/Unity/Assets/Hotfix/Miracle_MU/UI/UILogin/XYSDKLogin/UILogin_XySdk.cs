using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System;
using UnityEngine.UI;



namespace ETHotfix
{
    [ObjectSystem]
    public class UILogin_XySdkAwake : AwakeSystem<UILogin_XySdk>
    {
        public override void Awake(UILogin_XySdk self)
        {


            SdkCallBackComponent.Instance.LoginCallBack = self.Login;

            self.sdkUtility = Component.Global.GetComponent<SdkUtility>();
#if UNITY_IPHONE
            self.sdkUtility.init("7a6cf46e5d6a73a5ce7d49590fbdc357", "9b45f6e9f44ba8a63bc1664a599d6f99");
#endif
            // self.sdkUtility.LoginInfo = $"1560217031058395137,eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwaG9uZSI6IjE3MzI4Nzk4OTkzIiwiYXBwSWQiOjAsImJ1c2luZXNzSWQiOjQ2LCJjbGllbnQiOjEsImV4cCI6MTY5MjU4ODQ5MCwidXNlcklkIjoxNTYwMjE3MDMxMDU4Mzk1MTM3LCJ1dWlkIjoiODhkYzgzYTEwYzE3NDA1NWE2NGE2MzBjZDFkZTg5NzIiLCJ1c2VybmFtZSI6IkgxUUNCMTY2MDgxOTcwMiJ9.a0pHtZPlj1on0gDWjrCiFmxIEK1AJHi_fbaj4i_eyvU";

            self.referenceCollector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.loginBtn = self.referenceCollector.GetButton("LoginBtn");
            self.loginBtn.onClick.AddSingleListener(self.LoginEvent);

            self.XieYiPanel = self.referenceCollector.GetImage("XieYiPanel").gameObject;

            self.InitXueKeXieYi();
            //隐私协议
            self.hyperlinkText = self.referenceCollector.GetText("Label").GetComponent<UIHyperlinkText>();
            self.hyperlinkText.RegisterClickCallback(self.Show);
            
            self.referenceCollector.GetToggle("Toggle").onValueChanged.AddSingleListener((value) => 
            {
               // if (value)
                {
                    if (ETModel.Init.instance.e_SDK == E_SDK.XY_SDK)
                    {

                        SdkCallBackComponent.Instance.sdkUtility.ShowUserAgreementPrivac();
                    }
                    
                    self.IsRead = value;
                    PlayerPrefs.SetInt("IsAgreen", value ? 1 : 0);
                }

            });
            if (PlayerPrefs.GetInt("IsAgreen") == 1)
            {
                self.IsRead = true;
                self.referenceCollector.GetToggle("Toggle").isOn = true;
            }
            else
            {
                self.IsRead = false;
                self.referenceCollector.GetToggle("Toggle").isOn = false;
            }


            //切换账号
            self.referenceCollector.GetButton("SwitchBtn").onClick.AddSingleListener(() => 
            {
                if (self.sdkUtility.IsLogin())
                { 
                self.sdkUtility.SwitchLogin();
                }
            });
            //退出登录
            self.referenceCollector.GetButton("LoginOutBtn").onClick.AddSingleListener(() =>
            {
                if (self.sdkUtility.IsLogin())
                {
                    self.sdkUtility.LoginOut();
                }
            });
        }
    }

    /// <summary>
    /// XYSDk 登录
    /// </summary>
    public partial class UILogin_XySdk : Component
    {

        public ReferenceCollector referenceCollector;

        public bool IsRead = false;
        public Button loginBtn;

       
        public SdkUtility sdkUtility;

        public long NewKey = 0;
        private int loginCount = 0;//登录次数

        public UIHyperlinkText hyperlinkText;

        public string ShouQInfo;

        public void LoginEvent() 
        {
            Log.Debug("LoginEvent-------------------1 ");
            if (IsRead == false && ETModel.Init.instance.e_SDK != E_SDK.HaXi)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请先阅读并同意 用户协议 和隐私协议");
                return;
            }
            Log.Debug("LoginEvent-------------------2 ");
            loginBtn.interactable = false;
            if (ETModel.Init.instance.e_SDK == E_SDK.SHOU_Q)
            {
                ShouQSdk_LoginAsync(sdkUtility.LoginInfo).Coroutine();
            }
            else
            {
                Log.Debug("LoginEvent-------------------3 ");
                sdkUtility.Login();
            }

        }

        public void Show(string s, string s1)
        {
            if (ETModel.Init.instance.e_SDK == E_SDK.SHOU_Q)
            {

                if (s.Contains("服务协议"))
                {
                    UIComponent.Instance.VisibleUI(UIType.UI_UserAgreement, "https://beefuncloud.game.androidscloud.com/H5-web/BY_SYXKXY.html");
                }
                else if (s.Contains("隐私政策"))
                {
                    UIComponent.Instance.VisibleUI(UIType.UI_UserAgreement, "https://beefuncloud.game.androidscloud.com/H5-web/BY_YSXY.html");
                }

            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UI_UserAgreement);
            }

        }
        /// <summary>
        /// 登录回调
        /// </summary>
        /// <param name="userInfo"></param>
     
        public void Login(string Info)
        {
            
            switch (ETModel.Init.instance.e_SDK)
            {
                case E_SDK.NONE:
                    break;
                case E_SDK.HAO_YI_SDK:
                    break;
                case E_SDK.TIKTOK_SDK:
                    TikTokSdk_LoginAsync(Info).Coroutine();
                    break;
                case E_SDK.HaXi:
                    HaXiSdk_LoginAsync(Info).Coroutine();
                    break;
                case E_SDK.XY_SDK:
                    XySdk_LoginAsync(Info).Coroutine();
                    break;
                case E_SDK.SHOU_Q:
                    ShouQInfo=Info;
                    UIComponent.Instance.VisibleUI(UIType.UIHint,$"{Info}");
                    ShouQSdk_LoginAsync(Info).Coroutine();
                    Log.DebugGreen($"{Info}");
                    break;
                default:
                    break;
            }
            if (Info.Contains("登录失败"))
            {
                loginBtn.interactable = true;
            }

        }

        async ETVoid XySdk_LoginAsync(string Info) 
        {

            UserInfo userInfo = LitJson.JsonMapper.ToObject<UserInfo>(Info);
            //创建一个会话实体session
            ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalDataManager.LoginConnetIP);
            //热更层也创建一个Seesion,将Model层的session传递过去
            //热更层的Seesion创建后,会调用Awake方法,在内部关联了Model层的session
            //以后调用热更层的Seesion 就是调用间接的调用了主工程的 Seesion
            Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
            R2C_XYSDKvalidatelogonResponse r2C_XYSD = (R2C_XYSDKvalidatelogonResponse)await realmSession.Call(new C2R_XYSDKvalidatelogonRequest
            {
                Uid = int.Parse(userInfo.id),
                Gid = 681,
                Time = int.Parse(userInfo.loginTime),
                Token = userInfo.token,
                ChannelId = AgentTool.agentstr,//运营商id
                DeviceNum = DeviceUtil.DeviceIdentifier,
                DeviceType = DeviceUtil.DeviceModel,
                OSType = DeviceUtil.DeviceVersionType,
                BaseVersion = "1123",//版本号
                CPUType = ""//cpu型号

            }); ;
            if (r2C_XYSD.Error != 0)
            {
                //提示错误信息

                Log.DebugRed($"{r2C_XYSD.Error}");
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();

                if (r2C_XYSD.Error == 117)
                {
                    uIConfirm.AddCancelEventAction(() => Application.Quit());
                    uIConfirm.AddActionEvent(() => Application.Quit());
                    TimeSpan timeSpan = GetSpacingTime_Seconds(r2C_XYSD.BanTillTime);
                    DateTime dateTime = TimeHelper.GetDateTime_Milliseconds(r2C_XYSD.BanTillTime);
                    var str = $"您的账号存在存在异常行为({r2C_XYSD.BanReason})\n被封禁{timeSpan.Days}天{timeSpan.Hours}小时{timeSpan.Minutes}分钟\n解封时间：{dateTime.Year}年{dateTime.Month}月{dateTime.Hour}:{dateTime.Minute}";
                    uIConfirm.SetTipText(str, true, "账号异常");

                    static TimeSpan GetSpacingTime_Seconds(long Seconds)
                    {
                        //获取时间戳 秒
                        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(Seconds);
                        DateTime curdateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
                        TimeSpan timeSpan = new TimeSpan(startTime.Ticks);
                        TimeSpan CurtimeSpan = new TimeSpan(curdateTime.Ticks);
                        return timeSpan.Subtract(CurtimeSpan).Duration();
                    }
                }
                else
                {

                    uIConfirm.SetTipText(r2C_XYSD.Error.GetTipInfo(), true);
                }
                return;

            }
            else
            {
                //登录成功事件
                // Game.EventSystem.Run(EventIdType.LoginFinish);
                GlobalDataManager.XYUUID = int.Parse(userInfo.id);

                LogCollectionComponent.Instance.UserId = r2C_XYSD.UserId;
                LogCollectionComponent.Instance.IP = r2C_XYSD.SelfIP;
                LogCollectionComponent.Instance.Info("#登录# 使用XYSDK登录成功");
            }

            //获取上一次的登录信息
            R2C_GetLastLoginToTheRegion r2C_GetLastLoginToTheRegion = (R2C_GetLastLoginToTheRegion)await realmSession.Call(new C2R_GetLastLoginToTheRegion
            {
                Uid = userInfo.id,
                Type = 1
            });
            if (r2C_GetLastLoginToTheRegion.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, r2C_GetLastLoginToTheRegion.Error.GetTipInfo());
            }
            else
            {
                GlobalDataManager.EnterZoneID = r2C_GetLastLoginToTheRegion.GameAreaId;
                GlobalDataManager.EnterLineID = r2C_GetLastLoginToTheRegion.Gameline;
            }


            realmSession.Dispose();

            //服务器返回了网关地址
            //那么就根据网关地址创建一个新的Session 连接到网关去
            ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(r2C_XYSD.Address);
            GlobalDataManager.Address = r2C_XYSD.Address;
            //在Scene实体中添加SessionComponent组件 并且缓存Session对象 以后就可以直接获取来发送消息
            ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = gateSession;

            //这里跟上面逻辑一样,创建热更层的Session,关联到主工程
            Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
            if (Game.Scene.GetComponent<SessionHelper>() == null)
            {
                Game.Scene.AddComponent<SessionHelper>();
            }
            G2C_LoginGateResponse g2CLoginGate = (G2C_LoginGateResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginGateRequest()
            {
                Key = this.NewKey == 0 ? r2C_XYSD.Key : this.NewKey,
                ChannelId = AgentTool.agentstr,//运营商id
            });
            if (g2CLoginGate.Error != 0)
            {
                LogCollectionComponent.Instance.Info($"#Session# LoginGate Error:{g2CLoginGate.Error}");
                if (g2CLoginGate.Error == 111)
                {
                    this.NewKey = g2CLoginGate.NewKey;
                    if (++loginCount >= 3)
                    {
                        this.NewKey = 0;
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2CLoginGate.Error.GetTipInfo());

                    }
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2CLoginGate.Error.GetTipInfo());
                    this.NewKey = 0;
                }
            }
            else
            {
                LogCollectionComponent.Instance.Info($"#Session# LoginGate 成功 session.Id:{SessionComponent.Instance.Session.session.Id}");
                SessionComponent.Instance.Session.AddComponent<PingComponent>();

#region 断线重连

                // 记录下次重连 GateServer 需要用的密钥
                GlobalDataManager.GateLoginKey = g2CLoginGate.NewKey.ToString();
                LogCollectionComponent.Instance.Info($"#Session# LoginGate 成功 session.Id:{SessionComponent.Instance.Session.session.Id}");
#endregion

                //登录成功事件
                Game.EventSystem.Run(EventIdType.LoginFinish);
            }
        }

        async ETVoid TikTokSdk_LoginAsync(string Info)
        {
            TikTokLoginResult userInfo = JsonUtility.FromJson<TikTokLoginResult>(Info);

            //创建一个会话实体session
            ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalDataManager.LoginConnetIP);
            //热更层也创建一个Seesion,将Model层的session传递过去
            //热更层的Seesion创建后,会调用Awake方法,在内部关联了Model层的session
            //以后调用热更层的Seesion 就是调用间接的调用了主工程的 Seesion
            Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);


            R2C_DouYinSDKLogonResponse r2C_TikTokSdk = (R2C_DouYinSDKLogonResponse)await realmSession.Call(new C2R_DouYinSDKLogonRequest
            {
                Uid = 1,
                Gid = 681,//问题
                Time = 1,
                Token = userInfo.accessToken,
                ChannelId = AgentTool.agentstr,//运营商id
                DeviceNum = DeviceUtil.DeviceIdentifier,
                DeviceType = DeviceUtil.DeviceModel,
                OSType = DeviceUtil.DeviceVersionType,
                BaseVersion = "1123",//版本号
                CPUType =""//cpu型号

            });
            //UIComponent.Instance.VisibleUI(UIType.UIHint, "登录回调:userId" + userInfo.userId + "||||code" + userInfo.code +"|||:r2C_TikTokSdk.UserId"+ r2C_TikTokSdk.UserId);
            if (r2C_TikTokSdk.Error != 0)
            {
                //提示错误信息
                //UIComponent.Instance.VisibleUI(UIType.UIHint, "请求登录抖音异常" + r2C_TikTokSdk.Error+ ":::r2C_TikTokSdk.UserId" + r2C_TikTokSdk.UserId+ "::userInfo.userId"+ userInfo.userId);
                Log.DebugRed($"{r2C_TikTokSdk.Error}");
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();

                if (r2C_TikTokSdk.Error == 117)
                {
                    uIConfirm.AddCancelEventAction(() => Application.Quit());
                    uIConfirm.AddActionEvent(() => Application.Quit());
                    TimeSpan timeSpan = GetSpacingTime_Seconds(r2C_TikTokSdk.BanTillTime);
                    DateTime dateTime = TimeHelper.GetDateTime_Milliseconds(r2C_TikTokSdk.BanTillTime);
                    var str = $"您的账号存在存在异常行为({r2C_TikTokSdk.BanReason})\n被封禁{timeSpan.Days}天{timeSpan.Hours}小时{timeSpan.Minutes}分钟\n解封时间：{dateTime.Year}年{dateTime.Month}月{dateTime.Hour}:{dateTime.Minute}";
                    uIConfirm.SetTipText(str, true, "账号异常");

                    static TimeSpan GetSpacingTime_Seconds(long Seconds)
                    {
                        //获取时间戳 秒
                        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(Seconds);
                        DateTime curdateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
                        TimeSpan timeSpan = new TimeSpan(startTime.Ticks);
                        TimeSpan CurtimeSpan = new TimeSpan(curdateTime.Ticks);
                        return timeSpan.Subtract(CurtimeSpan).Duration();
                    }
                }
                else
                {

                    uIConfirm.SetTipText(r2C_TikTokSdk.Error.GetTipInfo(), true);
                }
                return;

            }
            else
            {
                //登录成功事件
                // Game.EventSystem.Run(EventIdType.LoginFinish);
              //  GlobalDataManager.XYUUID = int.Parse(userInfo.id);

                LogCollectionComponent.Instance.UserId = r2C_TikTokSdk.UserId;
                LogCollectionComponent.Instance.IP = r2C_TikTokSdk.SelfIP;
                LogCollectionComponent.Instance.Info("#登录# 使用TIKTOK_SDK登录成功");
            }
            //获取上一次的登录信息
            R2C_GetLastLoginToTheRegion r2C_GetLastLoginToTheRegion = (R2C_GetLastLoginToTheRegion)await realmSession.Call(new C2R_GetLastLoginToTheRegion
            {
                // Uid = userInfo.userId.ToString(),
                Uid = r2C_TikTokSdk.UserId.ToString(),
                Type = 2
            });
            if (r2C_GetLastLoginToTheRegion.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, r2C_GetLastLoginToTheRegion.Error.GetTipInfo());
                //UIComponent.Instance.VisibleUI(UIType.UIHint, "获取上一次登录数据异常" + r2C_GetLastLoginToTheRegion.Error);
            }
            else
            {
                GlobalDataManager.EnterZoneID = r2C_GetLastLoginToTheRegion.GameAreaId;
                GlobalDataManager.EnterLineID = r2C_GetLastLoginToTheRegion.Gameline;
            }

            realmSession.Dispose();

            //服务器返回了网关地址
            //那么就根据网关地址创建一个新的Session 连接到网关去
            ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(r2C_TikTokSdk.Address);
            GlobalDataManager.Address = r2C_TikTokSdk.Address;
            //在Scene实体中添加SessionComponent组件 并且缓存Session对象 以后就可以直接获取来发送消息
            ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = gateSession;

            //这里跟上面逻辑一样,创建热更层的Session,关联到主工程
            Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
            if (Game.Scene.GetComponent<SessionHelper>() == null)
            {
                Game.Scene.AddComponent<SessionHelper>();
            }
            G2C_LoginGateResponse g2CLoginGate = (G2C_LoginGateResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginGateRequest()
            {
                Key = this.NewKey == 0 ? r2C_TikTokSdk.Key : this.NewKey,
                ChannelId = AgentTool.agentstr,//运营商id
            });
            if (g2CLoginGate.Error != 0)
            {
                //UIComponent.Instance.VisibleUI(UIType.UIHint, "登录网关异常" + g2CLoginGate.Error);
                LogCollectionComponent.Instance.Info($"#Session# LoginGate Error:{g2CLoginGate.Error}");
                if (g2CLoginGate.Error == 111)
                {
                    this.NewKey = g2CLoginGate.NewKey;
                    if (++loginCount >= 3)
                    {
                        this.NewKey = 0;
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2CLoginGate.Error.GetTipInfo());

                    }
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2CLoginGate.Error.GetTipInfo());
                    this.NewKey = 0;
                }
            }
            else
            {
                LogCollectionComponent.Instance.Info($"#Session# LoginGate 成功 session.Id:{SessionComponent.Instance.Session.session.Id}");
                SessionComponent.Instance.Session.AddComponent<PingComponent>();

#region 断线重连

                // 记录下次重连 GateServer 需要用的密钥
                GlobalDataManager.GateLoginKey = g2CLoginGate.NewKey.ToString();
                LogCollectionComponent.Instance.Info($"#Session# LoginGate 成功 session.Id:{SessionComponent.Instance.Session.session.Id}");
                #endregion
                //UIComponent.Instance.VisibleUI(UIType.UIHint, "登录网关成功");
                //登录成功事件
                Game.EventSystem.Run(EventIdType.LoginFinish);
            }
        }

        async ETVoid HaXiSdk_LoginAsync(string info)
        {
            Log.Info("HaXiSdk_LoginAsync " + GlobalDataManager.LoginConnetIP);
            Log.Info(info);
            HaXiLoginResult userInfo = JsonUtility.FromJson<HaXiLoginResult>(info);

            //创建一个会话实体session
            ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalDataManager.LoginConnetIP);
            //热更层也创建一个Seesion,将Model层的session传递过去
            //热更层的Seesion创建后,会调用Awake方法,在内部关联了Model层的session
            //以后调用热更层的Seesion 就是调用间接的调用了主工程的 Seesion
            Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);



            R2C_HaXiSDKLogonResponse r2C_HaXiSdk = (R2C_HaXiSDKLogonResponse)await realmSession.Call(new C2R_HaXiSDKLogonRequest
            {
                UserId = userInfo.uid,
                Token = userInfo.token,
                ChannelId = AgentTool.agentstr,//运营商id
                DeviceNum = DeviceUtil.DeviceIdentifier,
                DeviceType = DeviceUtil.DeviceModel,
                OSType = DeviceUtil.DeviceVersionType,
                BaseVersion = "1123",//版本号
                CPUType = ""//cpu型号
            });
            Log.Info("发送登录");
            Log.Info(JsonHelper.ToJson(r2C_HaXiSdk));

            if (r2C_HaXiSdk.Error != 0)
            {
                //提示错误信息

                Log.DebugRed($"{r2C_HaXiSdk.Error}");
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();

                if (r2C_HaXiSdk.Error == 117)
                {
                    uIConfirm.AddCancelEventAction(() => Application.Quit());
                    uIConfirm.AddActionEvent(() => Application.Quit());
                    TimeSpan timeSpan = GetSpacingTime_Seconds(r2C_HaXiSdk.BanTillTime);
                    DateTime dateTime = TimeHelper.GetDateTime_Milliseconds(r2C_HaXiSdk.BanTillTime);
                    var str = $"您的账号存在存在异常行为({r2C_HaXiSdk.BanReason})\n被封禁{timeSpan.Days}天{timeSpan.Hours}小时{timeSpan.Minutes}分钟\n解封时间：{dateTime.Year}年{dateTime.Month}月{dateTime.Hour}:{dateTime.Minute}";
                    uIConfirm.SetTipText(str, true, "账号异常");

                    static TimeSpan GetSpacingTime_Seconds(long Seconds)
                    {
                        //获取时间戳 秒
                        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(Seconds);
                        DateTime curdateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
                        TimeSpan timeSpan = new TimeSpan(startTime.Ticks);
                        TimeSpan CurtimeSpan = new TimeSpan(curdateTime.Ticks);
                        return timeSpan.Subtract(CurtimeSpan).Duration();
                    }
                }
                else
                {

                    uIConfirm.SetTipText(r2C_HaXiSdk.Error.GetTipInfo(), true);
                }
                return;

            }
            else
            {
                //登录成功事件
                // Game.EventSystem.Run(EventIdType.LoginFinish);
                //  GlobalDataManager.XYUUID = int.Parse(userInfo.id);
                //userInfo.userId = r2C_TikTokSdk.UserId;
                LogCollectionComponent.Instance.UserId = r2C_HaXiSdk.UserId;
                LogCollectionComponent.Instance.IP = r2C_HaXiSdk.SelfIP;
                LogCollectionComponent.Instance.Info("#登录# 使用TIKTOK_SDK登录成功");
            }

            //获取上一次的登录信息
            R2C_GetLastLoginToTheRegion r2C_GetLastLoginToTheRegion = (R2C_GetLastLoginToTheRegion)await realmSession.Call(new C2R_GetLastLoginToTheRegion
            {
                Uid = r2C_HaXiSdk.UserId.ToString(),
                Type = 2
            });

            Log.Info("获取上一次登录数据 " + r2C_HaXiSdk.UserId);
            Log.Info(JsonHelper.ToJson(r2C_GetLastLoginToTheRegion));


            if (r2C_GetLastLoginToTheRegion.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, r2C_GetLastLoginToTheRegion.Error.GetTipInfo());
            }
            else
            {
                GlobalDataManager.EnterZoneID = r2C_GetLastLoginToTheRegion.GameAreaId;
                GlobalDataManager.EnterLineID = r2C_GetLastLoginToTheRegion.Gameline;
            }

            realmSession.Dispose();
            string[] arr = r2C_HaXiSdk.Address.Split(':');
           // UIComponent.Instance.VisibleUI(UIType.UIHint, "登录的网关" + arr[1]);
            //服务器返回了网关地址
            //那么就根据网关地址创建一个新的Session 连接到网关去
            ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(r2C_HaXiSdk.Address);
            GlobalDataManager.Address = r2C_HaXiSdk.Address;
            //在Scene实体中添加SessionComponent组件 并且缓存Session对象 以后就可以直接获取来发送消息
            ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = gateSession;

            //这里跟上面逻辑一样,创建热更层的Session,关联到主工程
            Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
            if (Game.Scene.GetComponent<SessionHelper>() == null)
            {
                Game.Scene.AddComponent<SessionHelper>();
            }
            G2C_LoginGateResponse g2CLoginGate = (G2C_LoginGateResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginGateRequest()
            {
                Key = this.NewKey == 0 ? r2C_HaXiSdk.Key : this.NewKey,
                ChannelId = AgentTool.agentstr,//运营商id
            });


            Log.Info("获取登录数据查看数据是否满足");
            Log.Info(JsonHelper.ToJson(g2CLoginGate));

            if (g2CLoginGate.Error != 0)
            {
                LogCollectionComponent.Instance.Info($"#Session# LoginGate Error:{g2CLoginGate.Error}");
                if (g2CLoginGate.Error == 111)
                {
                    this.NewKey = g2CLoginGate.NewKey;
                    if (++loginCount >= 3)
                    {
                        this.NewKey = 0;
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2CLoginGate.Error.GetTipInfo());

                    }
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2CLoginGate.Error.GetTipInfo());
                    this.NewKey = 0;
                }
            }
            else
            {
                LogCollectionComponent.Instance.Info($"#Session# LoginGate 成功 session.Id:{SessionComponent.Instance.Session.session.Id}");
                SessionComponent.Instance.Session.AddComponent<PingComponent>();

                #region 断线重连

                // 记录下次重连 GateServer 需要用的密钥
                GlobalDataManager.GateLoginKey = g2CLoginGate.NewKey.ToString();
                LogCollectionComponent.Instance.Info($"#Session# LoginGate 成功 session.Id:{SessionComponent.Instance.Session.session.Id}");
                #endregion

                //登录成功事件
                Game.EventSystem.Run(EventIdType.LoginFinish);
            }

        }

        async ETVoid ShouQSdk_LoginAsync(string Info)
        {
            if (string.IsNullOrEmpty(Info))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"信息为空");
                return;
            }
            string[] infos = Info.Split(',');
         
            //创建一个会话实体session
            ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalDataManager.LoginConnetIP);
            //热更层也创建一个Seesion,将Model层的session传递过去
            //热更层的Seesion创建后,会调用Awake方法,在内部关联了Model层的session
            //以后调用热更层的Seesion 就是调用间接的调用了主工程的 Seesion
            Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);


            R2C_ShouQSDKLogonResponse r2C_TikTokSdk = (R2C_ShouQSDKLogonResponse)await realmSession.Call(new C2R_ShouQSDKLogonRequest
            {
                Uid = infos[0],
                ChannelId = AgentTool.agentstr,//运营商id
                DeviceNum = DeviceUtil.DeviceIdentifier,
                DeviceType = DeviceUtil.DeviceModel,
                OSType = DeviceUtil.DeviceVersionType,
                BaseVersion = "1123",//版本号
                CPUType = ""//cpu型号

            });

            if (r2C_TikTokSdk.Error != 0)
            {
                //提示错误信息

                Log.DebugRed($"{r2C_TikTokSdk.Error}");
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();

                if (r2C_TikTokSdk.Error == 117)
                {
                    uIConfirm.AddCancelEventAction(() => Application.Quit());
                    uIConfirm.AddActionEvent(() => Application.Quit());
                    TimeSpan timeSpan = GetSpacingTime_Seconds(r2C_TikTokSdk.BanTillTime);
                    DateTime dateTime = TimeHelper.GetDateTime_Milliseconds(r2C_TikTokSdk.BanTillTime);
                    var str = $"您的账号存在存在异常行为({r2C_TikTokSdk.BanReason})\n被封禁{timeSpan.Days}天{timeSpan.Hours}小时{timeSpan.Minutes}分钟\n解封时间：{dateTime.Year}年{dateTime.Month}月{dateTime.Hour}:{dateTime.Minute}";
                    uIConfirm.SetTipText(str, true, "账号异常");

                    static TimeSpan GetSpacingTime_Seconds(long Seconds)
                    {
                        //获取时间戳 秒
                        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(Seconds);
                        DateTime curdateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
                        TimeSpan timeSpan = new TimeSpan(startTime.Ticks);
                        TimeSpan CurtimeSpan = new TimeSpan(curdateTime.Ticks);
                        return timeSpan.Subtract(CurtimeSpan).Duration();
                    }
                }
                else
                {

                    uIConfirm.SetTipText(r2C_TikTokSdk.Error.GetTipInfo(), true);
                }
                return;

            }
            else
            {
                //登录成功事件
                // Game.EventSystem.Run(EventIdType.LoginFinish);
                 GlobalDataManager.ShouQUUID = infos[0];

                LogCollectionComponent.Instance.UserId = r2C_TikTokSdk.UserId;
                LogCollectionComponent.Instance.IP = r2C_TikTokSdk.SelfIP;
                LogCollectionComponent.Instance.Info("#登录# 使用TIKTOK_SDK登录成功");
            }

            //获取上一次的登录信息
            R2C_GetLastLoginToTheRegion r2C_GetLastLoginToTheRegion = (R2C_GetLastLoginToTheRegion)await realmSession.Call(new C2R_GetLastLoginToTheRegion
            {
                Uid = infos[0],
                Type = 3
            });
            if (r2C_GetLastLoginToTheRegion.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, r2C_GetLastLoginToTheRegion.Error.GetTipInfo());
            }
            else
            {
                GlobalDataManager.EnterZoneID = r2C_GetLastLoginToTheRegion.GameAreaId;
                GlobalDataManager.EnterLineID = r2C_GetLastLoginToTheRegion.Gameline;
            }

            realmSession.Dispose();

            //服务器返回了网关地址
            //那么就根据网关地址创建一个新的Session 连接到网关去
            ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(r2C_TikTokSdk.Address);
            GlobalDataManager.Address = r2C_TikTokSdk.Address;
            //在Scene实体中添加SessionComponent组件 并且缓存Session对象 以后就可以直接获取来发送消息
            ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = gateSession;

            //这里跟上面逻辑一样,创建热更层的Session,关联到主工程
            Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
            if (Game.Scene.GetComponent<SessionHelper>() == null)
            {
                Game.Scene.AddComponent<SessionHelper>();
            }
            G2C_LoginGateResponse g2CLoginGate = (G2C_LoginGateResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginGateRequest()
            {
                Key = this.NewKey == 0 ? r2C_TikTokSdk.Key : this.NewKey,
                ChannelId = AgentTool.agentstr,//运营商id
            });
            if (g2CLoginGate.Error != 0)
            {
                LogCollectionComponent.Instance.Info($"#Session# LoginGate Error:{g2CLoginGate.Error}");
                if (g2CLoginGate.Error == 111)
                {
                    this.NewKey = g2CLoginGate.NewKey;
                    if (++loginCount >= 3)
                    {
                        this.NewKey = 0;
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2CLoginGate.Error.GetTipInfo());

                    }
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2CLoginGate.Error.GetTipInfo());
                    this.NewKey = 0;
                }
            }
            else
            {
                LogCollectionComponent.Instance.Info($"#Session# LoginGate 成功 session.Id:{SessionComponent.Instance.Session.session.Id}");
                SessionComponent.Instance.Session.AddComponent<PingComponent>();

                #region 断线重连

                // 记录下次重连 GateServer 需要用的密钥
                GlobalDataManager.GateLoginKey = g2CLoginGate.NewKey.ToString();
                LogCollectionComponent.Instance.Info($"#Session# LoginGate 成功 session.Id:{SessionComponent.Instance.Session.session.Id}");
                #endregion

                //登录成功事件
                Game.EventSystem.Run(EventIdType.LoginFinish);
            }
        }

    }
}
