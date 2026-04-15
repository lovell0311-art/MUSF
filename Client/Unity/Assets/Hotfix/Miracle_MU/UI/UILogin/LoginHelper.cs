using System;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 登录事件
    /// </summary>
    public static class LoginHelper
    {
        /// <summary>
        /// 登录事件 
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static async ETVoid OnLoginAsync(string account, string password)
        {
            try
            {
                //创建一个会话实体session
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalDataManager.LoginConnetIP);
                //热更层也创建一个Seesion,将Model层的session传递过去
                //热更层的Seesion创建后,会调用Awake方法,在内部关联了Model层的session
                //以后调用热更层的Seesion 就是调用间接的调用了主工程的 Seesion
                Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
                //await等待服务器响应 r2CLogin这个是响应后 解包->反序列化得到的对象 里面已经包含服务器发送过来的数据
                R2C_RegisterOrLoginResponse r2c_RegisterOrLogin = (R2C_RegisterOrLoginResponse)await realmSession.Call(new C2R_RegisterOrLoginRequest()
                {
                    Account = account,
                    Password = password,
                    VerificationCode = "",
                    LoginType = (int)LoginType.LOGIN,
                    TerminalIP = DeviceUtil.GetIP(),//登录ip
                    ChannelId = "123112414",//运营商id
                    DeviceNum = DeviceUtil.DeviceIdentifier,
                    DeviceType = DeviceUtil.DeviceModel,
                    OSType = DeviceUtil.DeviceVersionType,
                    BaseVersion = "1123",//版本号
                    CPUType = "12312"//cpu型号
                });
                //提示错误信息
                if (r2c_RegisterOrLogin.Error != 0)
                {
                    UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirm.SetTipText(r2c_RegisterOrLogin.Error.GetTipInfo(), true);
                    return;
                }
                realmSession.Dispose();

                LogCollectionComponent.Instance.UserId = r2c_RegisterOrLogin.UserId;
                LogCollectionComponent.Instance.IP = r2c_RegisterOrLogin.SelfIP;
                //服务器返回了网关地址
                //那么就根据网关地址创建一个新的Session 连接到网关去
                ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(r2c_RegisterOrLogin.Address);
                //在Scene实体中添加SessionComponent组件 并且缓存Session对象 以后就可以直接获取来发送消息
                ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = gateSession;

                //这里跟上面逻辑一样,创建热更层的Session,关联到主工程
                Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
                G2C_LoginGateResponse g2CLoginGate = (G2C_LoginGateResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginGateRequest()
                {
                    Key = r2c_RegisterOrLogin.Key,
                    ChannelId = "123112414",//运营商id
                });
               
                //登录成功 将账号保存到本地 方便下一次的登录 使用
                AccountConfigInfo config = LocalDataJsonComponent.Instance.LoadData<AccountConfigInfo>(LocalJsonDataKeys.UserAccount)??new AccountConfigInfo();
                config.account = account;
                config.password = password;
                config.IsRead = true;
                LocalDataJsonComponent.Instance.SavaData(config, LocalJsonDataKeys.UserAccount);
                //登录成功事件
                Game.EventSystem.Run(EventIdType.LoginFinish);

             
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

    }
  
}