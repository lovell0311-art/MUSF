using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Realm)]
    public class C2R_XYSDKvalidatelogonRequestHandler : AMRpcHandler<C2R_XYSDKvalidatelogonRequest, R2C_XYSDKvalidatelogonResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, C2R_XYSDKvalidatelogonRequest request, R2C_XYSDKvalidatelogonResponse response, Action<IMessage> reply)
        {

            string url = "https://www.xy.com/xyapi/sdk/authtoken";
            int uid = request.Uid;
            int gid = ConstXYSDK.Gid;
            long Time = request.Time;
            string Token = request.Token;
            string jsonString = $"gid={gid}&time={Time}&token={Token}&uid={uid}{ConstXYSDK.Key}";
            string Chek = MD5Helper.GetMD5Hash(jsonString).ToLower();
            //string Chek = Help_MD5EncryptionHelper.MD5EncryptionGetString(jsonString);

            var Data = new Dictionary<string, string> { { "uid", $"{uid}" }, { "gid", $"{gid}" }, { "time", $"{Time}" }, { "token", $"{Token}" }, { "sign", $"{Chek}" } };
            //string jsonString2 = $"gid={gid}&time={Time}&token={Token}&uid={uid}&sign={Chek}";
            if (Data.Count > 0)
            {
                url += "?" + String.Join("&", Data.Select(p => $"{p.Key}={p.Value}"));
            }
            HttpClient httpClient = new HttpClient();
            var mrequest = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Content = new StringContent(jsonString, Encoding.UTF8, "application/x-www-form-urlencoded")
            };
            HttpResponseMessage mresponse = await httpClient.SendAsync(mrequest);
            ReturnSDK responseObject = new ReturnSDK();
            if (mresponse.IsSuccessStatusCode)
            {
                string responseBody = await mresponse.Content.ReadAsStringAsync();

                // 将字符串转换为相应的 JSON 对象
                responseObject = JsonConvert.DeserializeObject<ReturnSDK>(responseBody);
                if (responseObject == null)
                {
                    // 请求 API 接口出错
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(116);
                    reply(response);
                    return false;
                }

                if (responseObject.errno != "0")
                {
                    // 参数被篡改
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(115);
                    reply(response);
                    return false;
                }

            }
            else
            {
                // 请求 API 接口出错
                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(116);
                reply(response);
                return false;
            }

            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

            string address = session.RemoteAddress.Address.ToString();

            string str = request.Uid.ToString();
            DBAccountInfo dbLoginInfo = null;
            {// 获取或者创建一个账号

                var list = await mDBProxy.Query<DBAccountInfo>(p => p.XYAccountNumber == str);
                if (list.Count > 0)
                {
                    dbLoginInfo = list[0] as DBAccountInfo;
                }
                else
                {
                    {
                        DBManagerComponent mDBManagerComponent = Root.MainFactory.GetCustomComponent<DBManagerComponent>();
                        DBComponent db = mDBManagerComponent.GetZoneDB(DBType.Core, DBComponent.CommonDBId);
                        {
                            long Id = IdGeneraterNew.Instance.GenerateUnitId(DBComponent.CommonDBId);
                            DBAccountInfo registerInfo = new DBAccountInfo()
                            {
                                Id = Id,
                                //ShowId = dbvalue.ShowId,
                                Phone = "",
                                XYAccountNumber = str,
                                Password = "0",

                                RegisterTime = DateTime.UtcNow,
                                RegisterIP = address,

                                LastLoginType = request.OSType,
                                LastLoginTime = DateTime.UtcNow,
                                LastLoginIP = address,
                                DeviceType = request.DeviceType,
                                CPUType = request.CPUType,
                                ChannelId = request.ChannelId,
                            };

                            bool mSaveResult = await mDBProxy.Save(registerInfo);
                            if (mSaveResult == false)
                            {
                                Log.Error($"账号保存失败!  Phone:{registerInfo.XYAccountNumber}");
                                //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(103);
                                //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                                reply(response);
                                return false;
                            }
                            else
                            {
                                dbLoginInfo = registerInfo;
                            }

                            {
                                // 注册日志
                                DbLoginLog dbChangePasswordLog = new DbLoginLog()
                                {
                                    LoginLogType = ELoginLogType.Register,
                                    LoginType = ELoginType.XYUID,
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
            }

            if (dbLoginInfo == null)
            {
                // 账号注册失败
                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(103);
                reply(response);
                return false;
            }

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


            {
                // 登录日志
                DbLoginLog dbChangePasswordLog = new DbLoginLog()
                {
                    LoginLogType = ELoginLogType.Login,
                    LoginType = ELoginType.XYUID,
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


            // 随机分配一个Gate
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, str.Trim().GetHashCode()))
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                LoginCenter2Realm_LoginAccount l2rLoginAccount = (LoginCenter2Realm_LoginAccount)await loginCenterSession.Call(new Realm2LoginCenter_LoginAccount()
                {
                    UserId = dbLoginInfo.Id
                });
                if (l2rLoginAccount.Error == ErrorCode.ERR_AccountNotOffline)
                {
                    // 账号在线
                    // TODO 下线账号
                    var gateConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Gate, l2rLoginAccount.GateServerId);
                    Session gateSession2 = Game.Scene.GetComponent<NetInnerComponent>().Get(gateConfig.ServerInnerIP);
                    Gate2S_DisconnectGateUser g2SDisconneectGateUser = (Gate2S_DisconnectGateUser)await gateSession2.Call(new S2Gate_DisconnectGateUser() { UserId = dbLoginInfo.Id });
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

                var config = await Root.MainFactory.AddCustomComponent<RealmGateAddressComponent>().GetAddress(AppType.Gate, dbLoginInfo.Id);
                Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(config.ServerInnerIP);

                G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await gateSession.Call(new R2G_GetLoginKey() { Account = dbLoginInfo.Id.ToString() });

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
            reply(response);
            return false;
        }
    }
}