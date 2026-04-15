using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Realm)]
    public class C2R_RegisterOrLoginRequestHandler : AMRpcHandler<C2R_RegisterOrLoginRequest, R2C_RegisterOrLoginResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, C2R_RegisterOrLoginRequest request, R2C_RegisterOrLoginResponse response, Action<IMessage> reply)
        {
            {// 校验账号是否为空，普通登录支持手机号和自定义账号
                if (string.IsNullOrWhiteSpace(request.Account))
                {
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(100);
                    //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号错误,请重新输入!");
                    reply(response);
                    return false;
                }
            }

            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

            if (request.LoginType == 1)
            {// 校验验证码
#if !DEVELOP
                if (!Root.MainFactory.GetCustomComponent<SMSMessageComponent>().Verify(ESMSCodeType.Register, request.Account, request.VerificationCode))
                {
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(105);
                    reply(response);
                    return false;
                }
#endif
            }
            string address = session.RemoteAddress.Address.ToString();
            //List<string> strings = new List<string>()
            //{ 
            //     "tx0412","xy8888","cg8888","XY8888",
            //     "kk7006","fs6666","mh0225","Xy8888",
            //     "aa3265","xx6666","geng01","js1234",
            //     "df6666","hh8888","Kk7006","qq1234",
            //     "sp8888","dnd888","KK7006",
            //};
           
            DBAccountInfo dbLoginInfo = null;
            {// 获取或者创建一个账号
                Log.Info($"#RealmLogin# QueryAccount start account={request.Account} remote={session.RemoteAddress} terminal={request.TerminalIP}");
                Task<List<ComponentWithId>> queryTask = mDBProxy.Query<DBAccountInfo>(p => p.Phone == request.Account);
                if (await Task.WhenAny(queryTask, Task.Delay(5000)) != queryTask)
                {
                    Log.Error($"#RealmLogin# QueryAccount timeout account={request.Account} dbProxy={mDBProxy?.DBId} dbType={mDBProxy?.DBType} area={mDBProxy?.AreaId}");
                    response.Error = 102001;
                    response.Message = "DBQueryTimeout";
                    reply(response);
                    return true;
                }

                var list = await queryTask;
                Log.Info($"#RealmLogin# QueryAccount finish account={request.Account} count={list.Count}");
                if (list.Count > 0)
                {
                    Log.Info($"#RealmLogin# QueryAccount branch=Exists account={request.Account} loginType={request.LoginType}");
                    if (request.LoginType == 0)
                    {
                        Log.Info($"#RealmLogin# QueryAccount itemType account={request.Account} type={list[0]?.GetType().FullName ?? "null"}");
                        dbLoginInfo = list[0] as DBAccountInfo;
                        if (dbLoginInfo == null)
                        {
                            Log.Error($"#RealmLogin# QueryAccount cast failed account={request.Account} rawType={list[0]?.GetType().FullName ?? "null"}");
                            response.Error = 102001;
                            response.Message = "AccountCastError";
                            reply(response);
                            return true;
                        }
                        Log.Info($"#RealmLogin# PasswordCheck start account={request.Account} userId={dbLoginInfo.Id}");
                        if (dbLoginInfo.Password != request.Password)
                        {
                            Log.Info($"#RealmLogin# PasswordCheck failed account={request.Account} userId={dbLoginInfo.Id}");
                            //response.Error = ErrorCodeHotfix.ERR_AccountAlreadyExists;
                            response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(100);
                            response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                            dbLoginInfo = null;
                            reply(response);
                            return true;
                        }
                        Log.Info($"#RealmLogin# PasswordCheck passed account={request.Account} userId={dbLoginInfo.Id}");
                    }
                    else
                    {
                        Log.Info($"#RealmLogin# QueryAccount branch=RegisterButExists account={request.Account}");
                        response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(101);
                        //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号已存在");
                        reply(response);
                        return true;
                    }
                }
                else
                {
                    Log.Info($"#RealmLogin# QueryAccount branch=NotExists account={request.Account} loginType={request.LoginType}");
                    if (request.LoginType == 0)
                    {
                        Log.Info($"#RealmLogin# QueryAccount loginButNotExists account={request.Account}");
                        response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                        //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在");
                        reply(response);
                        return true;
                    }
                    else
                    {
                        Log.Info($"#RealmLogin# QueryAccount registerStart account={request.Account}");
                        //if (!strings.Contains(request.Code))
                        //{
                        //    response.Error = 135;
                        //    reply(response);
                        //    return false;
                        //}
                        DBManagerComponent mDBManagerComponent = Root.MainFactory.GetCustomComponent<DBManagerComponent>();
                        DBComponent db = mDBManagerComponent.GetZoneDB(DBType.Core, DBComponent.CommonDBId);
                        //DBShowId dbvalue = await db.UpdateAtomicallyInc<DBShowId>(o => true, o => (o as DBShowId).ShowId, 1) as DBShowId;

                        //if (dbvalue == null)
                        //{
                        //    Log.Error($"账号创建失败!  Phone:{request.Account}");
                        //    response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                        //    response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号创建失败");
                        //}
                        //else
                        {
                            long Id = IdGeneraterNew.Instance.GenerateUnitId(DBComponent.CommonDBId);
                            DBAccountInfo registerInfo = new DBAccountInfo()
                            {
                                Id = Id,
                                //ShowId = dbvalue.ShowId,

                                Phone = request.Account,
                                Password = request.Password,

                                RegisterTime = DateTime.UtcNow,
                                RegisterIP = address,

                                LastLoginType = request.OSType,
                                LastLoginTime = DateTime.UtcNow,
                                LastLoginIP = address,
                                DeviceType = request.DeviceType,
                                CPUType = request.CPUType,
                                ChannelId = request.ChannelId,
                                Code = request.Code,
                                /**
                                 * 推广码调整，
                                 * 前端填写的邀请码(request.Code)将代替代理ID(request.ChannelId)
                                 * 实际推广功能不在使用,
                                 * 代理ID(request.ChannelId)也不在使用
                                 **/
                                //ChannelId = request.Code,
                                //Code = "",
                            };
                            //推广码人数更新
                            //if(!string.IsNullOrEmpty(request.Code))
                            //{
                            //    G2M_CheckExtensionCode g2M_CheckExtensionCode = new G2M_CheckExtensionCode();
                            //    g2M_CheckExtensionCode.Code = request.Code;
                            //    g2M_CheckExtensionCode.UserID = Id;
                            //    var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                            //    Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                            //    IResponse Message = await loginCenterSession.Call(g2M_CheckExtensionCode);
                            //    if (Message != null && Message.Error != 0)
                            //    {
                            //        response.Error = 135;
                            //        reply(response);
                            //        return false;
                            //    }
                            //}

                            bool mSaveResult = await mDBProxy.Save(registerInfo);
                            if (mSaveResult == false)
                            {
                                Log.Error($"账号保存失败!  Phone:{registerInfo.Phone}");
                                //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(103);
                                //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                            }
                            else
                            {
                                dbLoginInfo = registerInfo;
                                Log.Info($"#RealmLogin# QueryAccount registerSaved account={request.Account} userId={dbLoginInfo.Id}");
                            }
                        }


                        {
                            // 注册日志
                            DbLoginLog dbChangePasswordLog = new DbLoginLog()
                            {
                                LoginLogType = ELoginLogType.Register,
                                LoginType = ELoginType.Phone,
                                Id = IdGeneraterNew.Instance.GenerateId(),
                                UserId = dbLoginInfo.Id,
                                GameServerId = OptionComponent.Options.AppId,
                                CreateTime = TimeHelper.Now(),

                                TerminalIP = session.RemoteAddress.Address.ToString(),
                                DeviceType = request.DeviceType,
                                CPUType = request.CPUType,
                                BaseVersion = request.BaseVersion,
                                OSType = request.OSType,
                                ChannelId = request.ChannelId,

                            };
                            DBLogHelper.Write(dbChangePasswordLog);
                        }
                    }
                }
            }

#if !DEVELOP
            Log.Info($"#RealmLogin# AfterQuery check=IdInspect developFlag=false account={request.Account} userId={dbLoginInfo?.Id ?? 0}");
            // 是否已进行实名认证
            if (dbLoginInfo != null && dbLoginInfo.IdInspect == 0)
            {
                if (request.LoginType == 0)
                {
                    response.UserId = dbLoginInfo.Id;
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(132);
                    //C2G_NeedIdCardInspectMessage mNeedIdCardInspectMessage = new C2G_NeedIdCardInspectMessage();
                    //mNeedIdCardInspectMessage.Account = dbLoginInfo.Id;
                    //reply(mNeedIdCardInspectMessage);
                    reply(response);
                    return true;
                }
            }
#endif
            Log.Info($"#RealmLogin# AfterQuery check=RealName account={request.Account} userId={dbLoginInfo?.Id ?? 0} enable={RealName.EnableOrNot}");
            if (RealName.EnableOrNot)
            {
                if (dbLoginInfo != null && dbLoginInfo.IdInspect == 0)
                {
                    if (request.LoginType == 0)
                    {
                        response.UserId = dbLoginInfo.Id;
                        response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(134);
                        reply(response);
                        return true;
                    }
                }
            }
            Log.Info($"#RealmLogin# AfterQuery check=Maintain account={request.Account} userId={dbLoginInfo?.Id ?? 0} maintain={ConstServer.IsMaintain}");
            if (ConstServer.IsMaintain)
            {
                if(dbLoginInfo.Identity != AccountIdentity.Test)
                {
                    // 系统维护中，请稍后再试
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(119);
                    reply(response);
                    return true;
                }
            }

            // 随机分配一个Gate
            Log.Info($"#RealmLogin# AfterQuery check=EnterLoginFlow account={request.Account} userId={dbLoginInfo?.Id ?? 0} hasAccount={(dbLoginInfo != null)}");
            if (dbLoginInfo != null)
            {
                Log.Info($"#RealmLogin# AfterQuery check=Ban account={request.Account} userId={dbLoginInfo.Id} banTill={dbLoginInfo.BanTillTime}");
                if (dbLoginInfo.BanTillTime != 0)
                {
                    if (dbLoginInfo.BanTillTime > Help_TimeHelper.GetNow())
                    {
                        // 账号被封禁
                        response.BanTillTime = dbLoginInfo.BanTillTime;
                        response.BanReason = dbLoginInfo.BanReason;

                        response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(117);
                        reply(response);
                        return false;
                    }
                }

                try
                {
                    Log.Info($"#RealmLogin# LoginLog start account={request.Account} userId={dbLoginInfo.Id}");
                    DbLoginLog dbChangePasswordLog = new DbLoginLog()
                    {
                        LoginLogType = ELoginLogType.Login,
                        LoginType = ELoginType.Phone,
                        Id = IdGeneraterNew.Instance.GenerateId(),
                        UserId = dbLoginInfo.Id,
                        GameServerId = OptionComponent.Options.AppId,
                        CreateTime = TimeHelper.Now(),

                        TerminalIP = session.RemoteAddress.Address.ToString(),
                        DeviceType = request.DeviceType,
                        CPUType = request.CPUType,
                        BaseVersion = request.BaseVersion,
                        OSType = request.OSType,
                        ChannelId = request.ChannelId,

                    };
                    DBLogHelper.Write(dbChangePasswordLog);
                    Log.Info($"#RealmLogin# LoginLog finish account={request.Account} userId={dbLoginInfo.Id}");
                }
                catch (Exception ex)
                {
                    Log.Error($"#RealmLogin# LoginLog exception account={request.Account} userId={dbLoginInfo.Id} ex={ex}");
                    response.Error = 102001;
                    response.Message = "LoginLogException";
                    reply(response);
                    return true;
                }


                long loginLockKey = request.Account.Trim().GetHashCode();
                Log.Info($"#RealmLogin# WaitLock start account={request.Account} userId={dbLoginInfo.Id} key={loginLockKey}");
                CoroutineLock loginLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, loginLockKey);
                Log.Info($"#RealmLogin# WaitLock acquired account={request.Account} userId={dbLoginInfo.Id} key={loginLockKey} lockId={loginLock.InstanceId}");
                using (loginLock)
                {
                    var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                    Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                    Log.Info($"#RealmLogin# LoginCenter start account={request.Account} userId={dbLoginInfo.Id} target={loginCenterList[0].ServerInnerIP}");
                    Task<IResponse> loginCenterTask = loginCenterSession.Call(new Realm2LoginCenter_LoginAccount()
                    {
                        UserId = dbLoginInfo.Id
                    });
                    if (await Task.WhenAny(loginCenterTask, Task.Delay(5000)) != loginCenterTask)
                    {
                        Log.Error($"#RealmLogin# LoginCenter timeout account={request.Account} userId={dbLoginInfo.Id} target={loginCenterList[0].ServerInnerIP}");
                        response.Error = 102001;
                        response.Message = "LoginCenterTimeout";
                        reply(response);
                        return true;
                    }

                    LoginCenter2Realm_LoginAccount l2rLoginAccount = (LoginCenter2Realm_LoginAccount)await loginCenterTask;
                    Log.Info($"#RealmLogin# LoginCenter finish account={request.Account} userId={dbLoginInfo.Id} error={l2rLoginAccount?.Error ?? -1}");
                    if (l2rLoginAccount.Error == ErrorCode.ERR_AccountNotOffline)
                    {
                        // 账号在线
                        // TODO 下线账号
                        var gateConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Gate, l2rLoginAccount.GateServerId);
                        Session gateSession2 = Game.Scene.GetComponent<NetInnerComponent>().Get(gateConfig.ServerInnerIP);
                        Log.Info($"#RealmLogin# DisconnectOldGate start account={request.Account} userId={dbLoginInfo.Id} gate={gateConfig.ServerInnerIP}");
                        Task<IResponse> disconnectTask = gateSession2.Call(new S2Gate_DisconnectGateUser() { UserId = dbLoginInfo.Id });
                        if (await Task.WhenAny(disconnectTask, Task.Delay(5000)) != disconnectTask)
                        {
                            Log.Error($"#RealmLogin# DisconnectOldGate timeout account={request.Account} userId={dbLoginInfo.Id} gate={gateConfig.ServerInnerIP}");
                            response.Error = 102001;
                            response.Message = "DisconnectGateTimeout";
                            reply(response);
                            return true;
                        }

                        Gate2S_DisconnectGateUser g2SDisconneectGateUser = (Gate2S_DisconnectGateUser)await disconnectTask;
                        Log.Info($"#RealmLogin# DisconnectOldGate finish account={request.Account} userId={dbLoginInfo.Id} error={g2SDisconneectGateUser?.Error ?? -1}");
                        if (g2SDisconneectGateUser.Error != ErrorCode.ERR_Success)
                        {
                            response.Error = g2SDisconneectGateUser.Error;
                            reply(response);
                            return true;
                        }
                    }
                    else if (l2rLoginAccount.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = l2rLoginAccount.Error;
                        reply(response);
                        return true;
                    }

                    Log.Info($"#RealmLogin# GateAddress start account={request.Account} userId={dbLoginInfo.Id}");
                    var gateAddressTask = Root.MainFactory.AddCustomComponent<RealmGateAddressComponent>().GetAddress(AppType.Gate, dbLoginInfo.Id);
                    if (await Task.WhenAny(gateAddressTask, Task.Delay(5000)) != gateAddressTask)
                    {
                        Log.Error($"#RealmLogin# GateAddress timeout account={request.Account} userId={dbLoginInfo.Id}");
                        response.Error = 102001;
                        response.Message = "GateAddressTimeout";
                        reply(response);
                        return true;
                    }

                    var config = await gateAddressTask;
                    Log.Info($"#RealmLogin# GateAddress finish account={request.Account} userId={dbLoginInfo.Id} gateInner={config?.ServerInnerIP} gateOuter={config?.OutIP}");
                    Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(config.ServerInnerIP);

                    Log.Info($"#RealmLogin# GateLoginKey start account={request.Account} userId={dbLoginInfo.Id} gate={config.ServerInnerIP}");
                    Task<IResponse> loginKeyTask = gateSession.Call(new R2G_GetLoginKey() { Account = dbLoginInfo.Id.ToString() });
                    if (await Task.WhenAny(loginKeyTask, Task.Delay(5000)) != loginKeyTask)
                    {
                        Log.Error($"#RealmLogin# GateLoginKey timeout account={request.Account} userId={dbLoginInfo.Id} gate={config.ServerInnerIP}");
                        response.Error = 102001;
                        response.Message = "GateLoginKeyTimeout";
                        reply(response);
                        return true;
                    }

                    G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await loginKeyTask;
                    Log.Info($"#RealmLogin# GateLoginKey finish account={request.Account} userId={dbLoginInfo.Id} error={g2RGetLoginKey?.Error ?? -1}");

                    if (g2RGetLoginKey == null)
                    {
                        response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(109);
                        //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("获取Key时无返回");
                    }
                    else if (g2RGetLoginKey.Error != 0)
                    {
                        response.Error = g2RGetLoginKey.Error;
                        response.Message = g2RGetLoginKey.Message;
                    }
                    else
                    {
                        // 登录成功
                        Dictionary<string, object> updates = new Dictionary<string, object>();
                        updates.Add("LastLoginTime", DateTime.UtcNow);
                        updates.Add("LastLoginIP", address);
                        mDBProxy.UpdateOneSet<DBAccountInfo>(p => p.Id == dbLoginInfo.Id, updates).Coroutine();

                        response.Address = config.OutIP.ToString();
                        response.Key = g2RGetLoginKey.Key;
                        response.SelfIP = session.RemoteAddress.Address.ToString();
                        response.UserId = dbLoginInfo.Id;
                    }
                }
            }

            reply(response);
            return true;
        }
    }
}
