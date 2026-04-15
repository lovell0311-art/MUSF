using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix.Robot
{
    public static class LoginHelper
    {
        public static async Task<bool> LoginAsync(Scene clientScene,string address,string phone,string password)
        {
            var realmList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Realm);
            if (realmList.Length <= 0) return false;

            Session realmSession = clientScene.GetComponent<NetOuterComponent>().Create(realmList[0].ServerOutIP);
            R2C_RegisterOrLoginResponse r2C_RegisterOrLoginResponse = (R2C_RegisterOrLoginResponse)await realmSession.Call(new C2R_RegisterOrLoginRequest()
            {
                Account = phone,
                Password = MD5Helper.GetMD5Hash(password),
                ChannelId = "123112414",
                LoginType = 0,
            });
            if (r2C_RegisterOrLoginResponse.Error != ErrorCode.ERR_Success)
            {
                // 登录失败
                return false;
            }
            realmSession.AddComponent<ClientSceneComponent, Scene>(clientScene);
            AccountInfoComponent accountInfo = clientScene.GetComponent<AccountInfoComponent>();
            accountInfo.Phone = phone;
            accountInfo.GateAddress = r2C_RegisterOrLoginResponse.Address;
            accountInfo.GateId = r2C_RegisterOrLoginResponse.GateId;
            accountInfo.GateKey = r2C_RegisterOrLoginResponse.Key.ToString();

            return true;
        }

        public static async Task<bool> RegisterAsync(Scene clientScene, string address, string phone, string password)
        {
            var realmList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Realm);
            if (realmList.Length <= 0) return false;
            Session realmSession = clientScene.GetComponent<NetOuterComponent>().Create(realmList[0].ServerOutIP);
            R2C_RegisterOrLoginResponse r2C_RegisterOrLoginResponse = (R2C_RegisterOrLoginResponse)await realmSession.Call(new C2R_RegisterOrLoginRequest()
            {
                Account = phone,
                Password = MD5Helper.GetMD5Hash(password),
                ChannelId = "123112414",
                LoginType = 1,
            });
            if (r2C_RegisterOrLoginResponse.Error != ErrorCode.ERR_Success)
            {
                // 注册失败
                return false;
            }

            return true;
        }


        public static async Task<bool> EnterGameOrCreateAsync(Scene clientScene,int zoneId,int lineId)
        {
            AccountInfoComponent accountInfo = clientScene.GetComponent<AccountInfoComponent>();
            Session gateSession = clientScene.GetComponent<NetOuterComponent>().Create(accountInfo.GateAddress);
            G2C_LoginGateResponse g2C_LoginGateResponse = (G2C_LoginGateResponse)await gateSession.Call(new C2G_LoginGateRequest() { 
                ChannelId = "123112414",
                GateId = accountInfo.GateId,
                Key = long.Parse(accountInfo.GateKey)
            });
            if(g2C_LoginGateResponse.Error != ErrorCode.ERR_Success)
            {
                return false;
            }
            gateSession.AddComponent<ClientSceneComponent, Scene>(clientScene);
            gateSession.AddComponent<RobotDisconnectComponent>();

            G2C_LoginSystemEnterGameAreaMessage g2C_LoginSystemEnterGameAreaMessage = (G2C_LoginSystemEnterGameAreaMessage)await gateSession.Call(new C2G_LoginSystemEnterGameAreaMessage()
            {
                GameAreaId = zoneId,
                LineId = lineId
            });
            if(g2C_LoginSystemEnterGameAreaMessage.Error != ErrorCode.ERR_Success)
            {
                return false;
            }
            clientScene.GetComponent<SessionComponent>().session = gateSession;

            async Task<bool> RequestGetGamePlayerInfo()
            {
                G2C_LoginSystemGetGamePlayerInfoResponse g2C_LoginSystemGetGamePlayerInfoResponse = (G2C_LoginSystemGetGamePlayerInfoResponse)await gateSession.Call(new C2G_LoginSystemGetGamePlayerInfoRequest()
                {
                    GameId = g2C_LoginSystemEnterGameAreaMessage.GameIds
                });
                if(g2C_LoginSystemGetGamePlayerInfoResponse.Error != ErrorCode.ERR_Success) 
                {
                    Log.Warning($"C2G_LoginSystemGetGamePlayerInfoRequest:{g2C_LoginSystemGetGamePlayerInfoResponse.Error}");
                    return false;
                }
                RoleInfoComponent roleInfo = clientScene.GetComponent<RoleInfoComponent>();
                foreach (var info in g2C_LoginSystemGetGamePlayerInfoResponse.GameInfos)
                {
                    roleInfo.RoleInfoDict.Add(info.GameId, new RoleInfo()
                    {
                        GameUserId = info.GameId,
                        Name = info.NickName,
                        PlayerType = (E_GameOccupation)info.PlayerType,
                        Level = info.Level,
                        OccupationLevel = info.OccupationLevel,
                    });
                }
                return true;
            }
            if ((await RequestGetGamePlayerInfo()) == false) return false;

            RoleInfoComponent roleInfo = clientScene.GetComponent<RoleInfoComponent>();

            if (roleInfo.RoleInfoDict.Count == 0)
            {
                // 没有职业纯当
                // 创建一个角色
                E_GameOccupation[] playerTypeList = new E_GameOccupation[] {
                    E_GameOccupation.Spell,
                    E_GameOccupation.Swordsman,
                    E_GameOccupation.Archer,
                    /*E_GameOccupation.Spellsword,
                    E_GameOccupation.Holyteacher,
                    E_GameOccupation.SummonWarlock,*/
                };
                C2G_LoginSystemCreateGamePlayerRequest createRoleMsg = new C2G_LoginSystemCreateGamePlayerRequest();
                createRoleMsg.PlayerType = (int)playerTypeList[RandomHelper.RandomNumber(0, playerTypeList.Length)];
                createRoleMsg.NickName = "Robot_" + accountInfo.Phone.Substring(accountInfo.Phone.Length - 5);
                G2C_LoginSystemCreateGamePlayerResponse g2C_LoginSystemCreateGamePlayerResponse = (G2C_LoginSystemCreateGamePlayerResponse)await gateSession.Call(createRoleMsg);
                if(g2C_LoginSystemCreateGamePlayerResponse.Error != ErrorCode.ERR_Success)
                {
                    return false;
                }
                long gameUserId = g2C_LoginSystemCreateGamePlayerResponse.GameIds.array[0];
                roleInfo.RoleInfoDict.Add(gameUserId, new RoleInfo()
                {
                    GameUserId = gameUserId,
                    Name = createRoleMsg.NickName,
                    PlayerType = (E_GameOccupation)createRoleMsg.PlayerType,
                    Level = 1,
                    OccupationLevel = 0,
                });
            }
            RoleInfo gameInfo = roleInfo.RoleInfoDict.First().Value;
            G2C_StartGameGamePlayerResponse g2C_StartGameGamePlayerResponse = (G2C_StartGameGamePlayerResponse)await gateSession.Call(new C2G_StartGameGamePlayerRequest()
            {
                GameUserId = gameInfo.GameUserId
            });
            if(g2C_StartGameGamePlayerResponse.Error != ErrorCode.ERR_Success)
            {
                return false;
            }

            Unit localUnit = UnitFactory.CreatePlayer(clientScene, gameInfo.GameUserId, (E_GameOccupation)gameInfo.PlayerType);
            RobotRoleComponent role = localUnit.GetComponent<RobotRoleComponent>();
            role.Name = gameInfo.Name;
            role.OccupationLevel = gameInfo.OccupationLevel;
            role.Level = gameInfo.Level;
            clientScene.Name = gameInfo.Name;

            clientScene.GetComponent<PlayerComponent>().LocalUnit = localUnit;
            
            
            ETCancellationToken cancellationToken = new ETCancellationToken();

            async Task<bool> SendReadyMsg()
            {
                G2C_ReadyResponse g2C_ReadyResponse = (G2C_ReadyResponse)await gateSession.Call(new C2G_ReadyRequest() { });
                if (g2C_ReadyResponse.Error != ErrorCode.ERR_Success)
                {
                    cancellationToken.Cancel();
                    return false;
                }
                return true;
            }

            async Task<bool> WaitMapChangeFinish()
            {
                Wait_MapChangeFinish result = await clientScene.GetComponent<ObjectWait>().Wait<Wait_MapChangeFinish>(cancellationToken);
                if (result.Error != WaitTypeError.Success) return false;
                return true;
            }

            using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();
            tasks.Add(SendReadyMsg());
            tasks.Add(WaitMapChangeFinish());

            List<bool> rets = await TaskHelper.WaitAll(tasks);
            foreach(bool ret in rets)
            {
                if (ret == false) return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="clientScene"></param>
        /// <returns></returns>
        public static async Task<bool> InitComponent(Scene clientScene)
        {
            ETCancellationToken cancellationToken = new ETCancellationToken();
            RobotPetsWindowsComponent petsCom = clientScene.GetComponent<RobotPetsWindowsComponent>();
            bool ret = await petsCom.OpenPetsWindowsAsync(cancellationToken);
            if(ret == false)
            {
                Log.Warning("上线获取宠物信息失败");
                return false;
            }
            return true;
        }
    }
}
