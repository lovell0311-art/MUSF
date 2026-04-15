using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using Aop.Api.Util;
using Org.BouncyCastle.Asn1.Crmf;
using Aop.Api.Domain;
using CommandLine;
using System.Xml.Linq;
using ETModel.HttpProto;

//using MySqlX.XDevAPI;

namespace ETHotfix
{
    [HttpHandler(AppType.GM, "/api/")]
    public class Http_GM : AHttpHandler
    {
        /// <summary>
        /// 初始化后台
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post] // Url-> /api/Init
        public async Task<object> Init(HttpListenerRequest req, string postBody)
        {
            Dictionary<string, string> data =new Dictionary<string, string>();
            data = postBody.Split('&').Select(item => item.Split('=')).ToDictionary(parts => parts[0], parts => parts[1]);

            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = "获取数据库失败1"
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = "获取数据库失败2"
                };
            }
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
            if (dBProxy2 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = "获取数据库失败3"
                };
            }
            if (data.TryGetValue("Key", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = "获取密钥失败"
                };
            }
            GMKeyComponent GMKeyComponentInfo = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            await GMKeyComponentInfo.SetInitKey(Key);
            if (data.TryGetValue("Account", out var Account) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = "账号错误"
                };
            }
            if (data.TryGetValue("Password", out var Password) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = "密码错误"
                };
            }
            DBGMToolAccount mDBGMToolAccount = new DBGMToolAccount();
            mDBGMToolAccount.Account = Account;
            mDBGMToolAccount.Password = Password;
            mDBGMToolAccount.AccountLevel = 1;
            mDBGMToolAccount.Id = IdGeneraterNew.Instance.GenerateUnitId(1);
            await dBProxy2.Save(mDBGMToolAccount);

            return new
            {
                Succeed = true,
                Msg = ""
            };
        }
        /// <summary>
        /// 后台登录
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post] // Url-> /api/LogIn
        public async Task<object> LogIn(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            postBody = GMTool.Decrypt(postBody);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data = postBody.Split('&').Select(item => item.Split('=')).ToDictionary(parts => parts[0], parts => parts[1]);
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
            if (dBProxy2 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败3")
                };
            }
            if (data.TryGetValue("Account", out var Account) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }
            if (data.TryGetValue("Password", out var Password) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取密码失败")
                };
            }
            
           var Info =  await dBProxy2.Query<DBGMToolAccount>(P=>P.Account == Account);
            if (Info == null || Info.Count != 1)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("账号不存在")
                };
            }
            var DBInfo = Info[0] as DBGMToolAccount;
            if (DBInfo.Password != Password)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("账号密码错误")
                };
            }
            string key = GMTool.GetKey();
            GMTool.Add(key, DBInfo);
            return new
            {
                Succeed = true,
                Msg = GMTool.Encrypt($"AccountLevel={DBInfo.AccountLevel}&LogInKey={key}")
            };
        }
        /// <summary>
        /// 搜索账号
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post] // Url-> /api/SearchAccount
        public async Task<object> SearchAccount(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("SearchAccount", Key))
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }


            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
            if (dBProxy2 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败3")
                };
            }
            List<ComponentWithId> Info = null;
            if (data.TryGetValue("AccountId", out var Account) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }
            Info = await dBProxy2.Query<DBAccountInfo>(P => P.Phone == Account);
            if (Info == null || Info.Count != 1)
            {
                if (long.TryParse(Account, out var AccId))
                {
                    Info = await dBProxy2.Query<DBAccountInfo>(P => P.Id == AccId);
                    if (Info == null || Info.Count != 1)
                    {
                        return new
                        {
                            Succeed = false,
                            Msg = GMTool.Encrypt("账号不存在1")
                        };
                    }
                }
                else
                {
                    return new
                    {
                        Succeed = false,
                        Msg = GMTool.Encrypt("账号不存在2")
                    };
                }
            }
            var DBInfo = Info[0] as DBAccountInfo;
            string SanF = "";
            if(!string.IsNullOrEmpty(DBInfo.DouyinAccountNumber))
                SanF = DBInfo.DouyinAccountNumber;
            if (!string.IsNullOrEmpty(DBInfo.ShouQAccountNumber))
                SanF = DBInfo.ShouQAccountNumber;

            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                l2sGetLoginRecord = (LoginCenter2S_GetLoginRecord)await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = DBInfo.Id
                });
            }
            catch (Exception e)
            {
                return Error(msg: e.ToString());
            }

            string Start = "离线";
            if (l2sGetLoginRecord.GameUserId != 0)
                Start = "在线";

            if (data.TryGetValue("AreaDBIndex", out var AreaDBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败4")
                };
            }
            if (int.TryParse(AreaDBIndex, out var AreaDBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败5")
                };
            }
            var dBProxy3 = mDBProxyManager.GetZoneDB(DBType.Core, AreaDBId);
            if (dBProxy3 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败6")
                };
            }
            int MoJin = 0;
            var AreaInfo = await dBProxy3.Query<DBAccountZoneData>(P => P.Id == DBInfo.Id);
            if (AreaInfo != null || AreaInfo.Count == 1)
            {
                MoJin = (AreaInfo[0] as DBAccountZoneData).YuanbaoCoin;
            }
            string Str = $"Phone={DBInfo.Phone}&Id={DBInfo.Id}&Type={(int)DBInfo.Identity}&SanF={SanF}&QuDao={DBInfo.ChannelId}&Stata={Start}" +
                $"&LoginTime={DBInfo.LastLoginTime}&LoginIP={DBInfo.LastLoginIP}&CreatTime={DBInfo.RegisterTime}&CreatIP={DBInfo.RegisterIP}" +
                $"&UnsealTime={DBInfo.BanTillTime}&Banned={DBInfo.BanReason}&MoJin={MoJin}&IdCard={DBInfo.IdCard}&Name={DBInfo.Name}&GameUser={l2sGetLoginRecord.GameUserId}";
            
            Str = GMTool.Encrypt(Str);
            return new
            {
                Succeed = true,
                Msg = Str
            };
        }
        /// <summary>
        /// 修改账号属性
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post] // Url-> /api/EditAccount
        public async Task<object> EditAccount(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("EditAccount", Key))
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
            if (dBProxy2 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败3")
                };
            }
            if (data.TryGetValue("UserID", out var UserID) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败4")
                };
            }
            if (long.TryParse(UserID, out var Id) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败5")
                };
            }
            var Info = await dBProxy2.Query<DBAccountInfo>(P => P.Id == Id);
            if (Info == null || Info.Count != 1)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("账号不存在")
                };
            }
            var DBInfo = Info[0] as DBAccountInfo;
            if (data.TryGetValue("Type", out var Type))
            {
                if (int.TryParse(Type,out var Type32))
                {
                    DBInfo.Identity = (AccountIdentity)Type32;
                }
            }
            if (data.TryGetValue("QuDao", out var QuDao))
            {
                if (!string.IsNullOrEmpty(QuDao))
                {
                    DBInfo.ChannelId = QuDao;
                }
            }
            if (data.TryGetValue("Banned", out var Banned))
            {
                if (!string.IsNullOrEmpty(Banned))
                {
                    DBInfo.BanReason = Banned;
                }
            }
            if (data.TryGetValue("BanTillTime", out var Time))
            {
                if (long.TryParse(Time, out var Time64))
                {
                    if (Time64 != 0)
                    {
                        DBInfo.BanTillTime = Help_TimeHelper.GetNow() + Time64;
                        // 查找玩家在哪个服务器
                        var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                        Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                        IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                        {
                            UserId = DBInfo.Id
                        });
                        LoginCenter2S_GetLoginRecord l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
                        if (l2sGetLoginRecord != null)
                        {
                            if (l2sGetLoginRecord.UserId == DBInfo.Id)
                            {
                                // 通知Gate,将玩家踢下线
                                var gateInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Gate, l2sGetLoginRecord.GateServerId);
                                Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gateInfo.ServerInnerIP);
                                IResponse response2 = await gameSession.Call(new S2Gate_DisconnectGateUser()
                                {
                                    DisconnectType = (int)DisconnectType.Ban,
                                    UserId = DBInfo.Id,
                                    BanTillTime = DBInfo.BanTillTime,
                                    Reason = Banned,
                                });
                            }
                        }
                    }
                }
            }
           
            data.TryGetValue("BanTill", out var BanTill);
            if (BanTill == "True")
            {
                DBInfo.BanTillTime = 0;
                DBInfo.BanReason = "";
            }
            await dBProxy2.Save(DBInfo);
            return new
            {
                Succeed = true,
                Msg = GMTool.Encrypt("修改完成")
            };
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post] // Url-> /api/UpDataPassword
        public async Task<object> UpDataPassword(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("UpDataPassword", Key))
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
            if (dBProxy2 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败3")
                };
            }
            if (data.TryGetValue("UserID", out var UserID) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败4")
                };
            }
            if (long.TryParse(UserID, out var Id) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败5")
                };
            }
            var Info = await dBProxy2.Query<DBAccountInfo>(P => P.Id == Id);
            if (Info == null || Info.Count != 1)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("账号不存在")
                };
            }
            var DBInfo = Info[0] as DBAccountInfo;
            if (data.TryGetValue("UpDataPassword", out var UpDataPassword))
            {
                if (!string.IsNullOrEmpty(UpDataPassword))
                {
                    DBInfo.Password = MD5Helper.GetMD5Hash(UpDataPassword);
                }
            }   
            await dBProxy2.Save(DBInfo);
            return new
            {
                Succeed = true,
                Msg = GMTool.Encrypt("修改完成")
            };
        }
        /// <summary>
        /// 修改认证信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post] // Url-> /api/UpDataIdCard
        public async Task<object> UpDataIdCard(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("UpDataIdCard", Key))
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
            if (dBProxy2 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败3")
                };
            }
            if (data.TryGetValue("UserID", out var UserID) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败4")
                };
            }
            if (long.TryParse(UserID, out var Id) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败5")
                };
            }
            var Info = await dBProxy2.Query<DBAccountInfo>(P => P.Id == Id);
            if (Info == null || Info.Count != 1)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("账号不存在")
                };
            }
            var DBInfo = Info[0] as DBAccountInfo;
            if (data.TryGetValue("IdC", out var IdC))
            {
                if (!string.IsNullOrEmpty(IdC))
                {
                    DBInfo.IdCard = IdC;
                    if (data.TryGetValue("IdN", out var IdN))
                    {
                        if (!string.IsNullOrEmpty(IdN))
                        {
                            DBInfo.Name = IdN;
                            DBInfo.IdInspect = 1;
                            await dBProxy2.Save(DBInfo);
                            return new
                            {
                                Succeed = true,
                                Msg = GMTool.Encrypt("修改完成")
                            };
                        }
                    }
                }
            }
            return new
            {
                Succeed = false,
                Msg = GMTool.Encrypt("参数错误")
            };
        }
        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post] // Url-> /api/GetPlayer
        public async Task<object> GetPlayer(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("GetPlayer", Key))
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
            if (dBProxy2 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败3")
                };
            }
            if (data.TryGetValue("UserID", out var UserID) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败4")
                };
            }
            if (long.TryParse(UserID, out var Id) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败5")
                };
            }
            var InfoList = await dBProxy2.Query<DBGamePlayerData>(P => P.UserId == Id);
            if (InfoList == null || InfoList.Count == 0)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("账号不存在")
                };
            }
            string Str = "";
            foreach (var DBInfo in InfoList)
            {
                var Info = DBInfo as DBGamePlayerData;
                Str += $"{Info.NickName}={Info.Id}&";
            }
            Str = Str.Remove(Str.Length - 1);
            return new
            {
                Succeed = true,
                Msg = GMTool.Encrypt(Str)
            };
        }
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post] // Url-> /api/GetPlayerInfo
        public async Task<object> GetPlayerInfo(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("GetPlayerInfo", Key))
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
            if (dBProxy2 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败3")
                };
            }

            if (data.TryGetValue("GameUserID", out var GameUserID) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败4")
                };
            }
            if (long.TryParse(GameUserID, out var Id) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败5")
                };
            }
   
            var DBInfo = await dBProxy2.Query<DBGamePlayerData>(P => P.Id == Id);
            if (DBInfo == null || DBInfo.Count != 1)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("角色不存在")
                };
            }
            var Info = DBInfo[0] as DBGamePlayerData;

            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                l2sGetLoginRecord = (LoginCenter2S_GetLoginRecord)await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = Info.UserId
                });
            }
            catch (Exception e)
            {
                return Error(msg: e.ToString());
            }

            string Start = "离线";
            if (l2sGetLoginRecord.GameUserId == Info.Id)
                Start = "在线";

            string Str = $"UserId={Info.UserId}&GameUserId={Info.Id}&Start={Start}&Name={Info.NickName}&IsDel={Info.IsDisposePlayer}" +
                $"&Class={Info.PlayerTypeId}&Level={Info.Level}&ClassLv={Info.OccupationLevel}&LiLiang={Info.Strength}&MingJie={Info.Agility}" +
                $"&ZhiLi={Info.Willpower}&TiLi={Info.BoneGas}&TongShuai={Info.Command}&ShuXingDian={Info.FreePoint}&QiJiBi={Info.MiracleCoin}";
            
            return new
            {
                Succeed = true,
                Msg = GMTool.Encrypt(Str)
            };
        }
        /// <summary>
        /// 搜索角色信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post] // Url-> /api/SearchPlayerInfo
        public async Task<object> SearchPlayerInfo(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("SearchPlayerInfo", Key))
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
            if (dBProxy2 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败3")
                };
            }
            List<ComponentWithId> DBInfo= new List<ComponentWithId>();
            if (data.TryGetValue("PlayerKey", out var PlayerKey) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败4")
                };
            }
            if (long.TryParse(PlayerKey, out var Id))
            {
                if(Id <= int.MaxValue)
                    DBInfo = await dBProxy2.Query<DBGamePlayerData>(P => P.NickName == PlayerKey);
                else
                    DBInfo = await dBProxy2.Query<DBGamePlayerData>(P => P.Id == Id);
            }
            else
                DBInfo = await dBProxy2.Query<DBGamePlayerData>(P => P.NickName == PlayerKey);

            if (DBInfo == null || DBInfo.Count != 1)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("角色不存在")
                };
            }
            var Info = DBInfo[0] as DBGamePlayerData;

            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                l2sGetLoginRecord = (LoginCenter2S_GetLoginRecord)await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = Info.UserId
                });
            }
            catch (Exception e)
            {
                return Error(msg: e.ToString());
            }

            string Start = "离线";
            if (l2sGetLoginRecord.GameUserId == Info.Id)
                Start = "在线";
     
            string Str = $"UserId={Info.UserId}&GameUserId={Info.Id}&Start={Start}&Name={Info.NickName}&IsDel={Info.IsDisposePlayer}" +
                $"&Class={Info.PlayerTypeId}&Level={Info.Level}&ClassLv={Info.OccupationLevel}&LiLiang={Info.Strength}&MingJie={Info.Agility}" +
                $"&ZhiLi={Info.Willpower}&TiLi={Info.BoneGas}&TongShuai={Info.Command}&ShuXingDian={Info.FreePoint}&QiJiBi={Info.MiracleCoin}";

            return new
            {
                Succeed = true,
                Msg = GMTool.Encrypt(Str)
            };
        }
        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post] // Url-> /api/SearchPlayerInfo
        public async Task<object> SetPlayerInfo(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("SetPlayerInfo", Key))
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
            if (dBProxy2 == null)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败3")
                };
            }
            if (data.TryGetValue("GameUserId", out var GameUserId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败4")
                };
            }
            if (long.TryParse(GameUserId, out var Id) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("角色不存在1")
                };
            }
            var DBInfo = await dBProxy2.Query<DBGamePlayerData>(P => P.Id == Id);
            if (DBInfo == null || DBInfo.Count != 1)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("角色不存在2")
                };
            }
            var Info = DBInfo[0] as DBGamePlayerData;

            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                l2sGetLoginRecord = (LoginCenter2S_GetLoginRecord)await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = Info.UserId
                });
            }
            catch (Exception e)
            {
                return Error(msg: e.ToString());
            }

            if (l2sGetLoginRecord.GameUserId == Info.Id)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("角色在线无法修改")
                };
            }
            if (data.TryGetValue("ClassLv", out var ClassLv))
            {
                if (int.TryParse(ClassLv, out var Lv))
                {
                    if (Lv != Info.OccupationLevel)
                    {
                        Log.Warning($"Lv:{Lv}OccupationLevel:{Info.OccupationLevel}");
                        Info.OccupationLevel = Lv;
                    }
                }
            }
            if (data.TryGetValue("Level", out var Level))
            {
                if (int.TryParse(Level, out var mLevel))
                {
                    if (mLevel != Info.Level)
                    {
                        Info.Level = mLevel;
                    }
                }
            }
            if (data.TryGetValue("LiLiang", out var LiLiang))
            {
                if (int.TryParse(LiLiang, out var Strength))
                {
                    if (Strength != Info.Strength)
                    {
                        Info.Strength = Strength;
                    }
                }
            }
            if (data.TryGetValue("MingJie", out var MingJie))
            {
                if (int.TryParse(MingJie, out var Agility))
                {
                    if (Agility != Info.Agility)
                    {
                        Info.Agility = Agility;
                    }
                }
            }
            if (data.TryGetValue("ZhiLi", out var ZhiLi))
            {
                if (int.TryParse(ZhiLi, out var Willpower))
                {
                    if (Willpower != Info.Willpower)
                    {
                        Info.Willpower = Willpower;
                    }
                }
            }
            if (data.TryGetValue("TiLi", out var TiLi))
            {
                if (int.TryParse(TiLi, out var BoneGas))
                {
                    if (BoneGas != Info.BoneGas)
                    {
                        Info.BoneGas = BoneGas;
                    }
                }
            }
            if (data.TryGetValue("TongShuai", out var TongShuai))
            {
                if (int.TryParse(TongShuai, out var Command))
                {
                    if (Command != Info.Command)
                    {
                        Info.Command = Command;
                    }
                }
            }
            if (data.TryGetValue("ShuXingDian", out var ShuXingDian))
            {
                if (int.TryParse(ShuXingDian, out var FreePoint))
                {
                    if (FreePoint != Info.FreePoint)
                    {
                        Info.FreePoint = FreePoint;
                    }
                }
            }
            if (data.TryGetValue("QiJiBi", out var QiJiBi))
            {
                if (int.TryParse(QiJiBi, out var MiracleCoin))
                {
                    if (MiracleCoin != Info.MiracleCoin)
                    {
                        Info.MiracleCoin = MiracleCoin;
                    }
                }
            }
            await dBProxy2.Save(Info);
            return new
            {
                Succeed = true,
                Msg = GMTool.Encrypt("修改成功")
            };
        }
        /// <summary>
        /// 踢玩家下线
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post]  // Url-> /api/Kick
        public async Task<GMReturn> Kick(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new GMReturn
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("Kick", Key))
            {
                return new GMReturn
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            
            if (data.TryGetValue("UserID", out var UserID))
            {
                if (long.TryParse(UserID, out long Id))
                {
                    await AccountHelper.Kick(Id,"GM踢下线");
                    return new GMReturn
                    {
                        Succeed = true,
                        Msg = GMTool.Encrypt("操作成功")
                    };
                }
            }
            return new GMReturn
            {
                Succeed = false,
                Msg = GMTool.Encrypt("未知")
            };
        }
        /// <summary>
        /// 获取身上装备
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post]  // Url-> /api/GetPlayerEquipment
        public async Task<object> GetPlayerEquipment(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("GetPlayerEquipment", Key))
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }
            if (data.TryGetValue("GameUserId", out var UserID))
            {
                if (long.TryParse(UserID, out long Id))
                {
                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
                    if (dBProxy2 == null)
                    {
                        return new
                        {
                            Succeed = false,
                            Msg = GMTool.Encrypt("获取数据库失败3")
                        };
                    }
                    var ItemList = await dBProxy2.Query<DBItemData>(P => P.GameUserId == Id && P.InComponent == EItemInComponent.Equipment);
                    if (ItemList == null || ItemList.Count < 0)
                    {
                        return new
                        {
                            Succeed = false,
                            Msg = GMTool.Encrypt("装备异常")
                        };
                    }
                    string Msg = "";

                    var ConfigIfno = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>();
                   if(ItemList.Count >= 1)
                    {
                        foreach (var Item in ItemList)
                        {
                            var DBItem = Item as DBItemData;
                            var info = ConfigIfno.Get(DBItem.ConfigID);

                            Msg += $"Item{info.Slot}={DBItem.Id + "," + info.Name}&";
                        }
                        Msg = Msg.Remove(Msg.Length - 1);

                        return new
                        {
                            Succeed = true,
                            Msg = GMTool.Encrypt(Msg)
                        };
                    }
                    
                    return new
                    {
                        Succeed = false,
                        Msg = GMTool.Encrypt("没有穿戴装备")
                    };
                }
            }


            return new GMReturn
            {
                Succeed = false,
                Msg = GMTool.Encrypt("未知")
            };
        }
        /// <summary>
        /// 获取某件装备详细信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        [Post]  // Url-> /api/GetPlayerEquipment
        public async Task<object> GetEquipmentInfo(HttpListenerRequest req, string postBody)
        {
            var GMTool = Root.MainFactory.GetCustomComponent<GMKeyComponent>();
            var data = GMTool.GetRequest(postBody);
            if (data.TryGetValue("AccKey", out var Key) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取账号失败")
                };
            }

            if (!GMTool.CheckLevel("GetEquipmentInfo", Key))
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("权限不足")
                };
            }
            if (data.TryGetValue("DBIndex", out var DBIndex) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败1")
                };
            }
            if (int.TryParse(DBIndex, out var DBId) == false)
            {
                return new
                {
                    Succeed = false,
                    Msg = GMTool.Encrypt("获取数据库失败2")
                };
            }
            if (data.TryGetValue("ItemUId", out var ItemUId))
            {
                if (long.TryParse(ItemUId, out long Id))
                {
                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, DBId);
                    if (dBProxy2 == null)
                    {
                        return new
                        {
                            Succeed = false,
                            Msg = GMTool.Encrypt("获取数据库失败3")
                        };
                    }
                    var ItemList = await dBProxy2.Query<DBItemData>(P => P.GameUserId == Id);
                    if (ItemList == null || ItemList.Count != 1)
                    {
                        return new
                        {
                            Succeed = false,
                            Msg = GMTool.Encrypt("装备异常")
                        };
                    }
                    var DBItem = ItemList[0] as DBItemData;

                    var ConfigIfno = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>().Get(DBItem.ConfigID);
                    string Msg = "";
                    //string Msg = $"Name={}&EnhanceLv={}&ForgeLv={}&Cnt={}&SetId={}oPenLv={}&Item={}" +
                    //    $"Skill={}&Luck={}&Type={}Excellent={}&Rebirth={}&Embed={}&special={}";

                    return new
                    {
                        Succeed = true,
                        Msg = GMTool.Encrypt(Msg)
                    };
                }
            }


            return new GMReturn
            {
                Succeed = false,
                Msg = GMTool.Encrypt("未知")
            };
        }
        
    }
}