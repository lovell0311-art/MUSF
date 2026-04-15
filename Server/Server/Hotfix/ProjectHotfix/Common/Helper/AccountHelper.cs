using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;


namespace ETHotfix
{
    public static class AccountHelper
    {
        /// <summary>
        /// 踢账号下线
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static async Task Kick(long userId, string reason)
        {
            // 查找玩家在哪个服务器
            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
            Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
            IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
            {
                UserId = userId
            });
            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
            if (l2sGetLoginRecord == null)
            {
                Log.Warning("LoginCenter 没开启");
                return;
            }

            if (l2sGetLoginRecord.UserId != userId)
            {
                // 玩家没在线
                return;
            }

            // 通知Gate,将玩家踢下线
            var gateInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Gate, l2sGetLoginRecord.GateServerId);
            Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gateInfo.ServerInnerIP);
            IResponse response2 = await gameSession.Call(new S2Gate_DisconnectGateUser()
            {
                DisconnectType = (int)DisconnectType.GMKickOffline,
                UserId = userId,
                Reason = reason,
            });
        }

        /// <summary>
        /// 封禁账号
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="banTillTime"></param>
        /// <param name="banReason"></param>
        /// <returns></returns>
        public static async Task Ban(long userId, long banTillTime ,string banReason)
        {
            if(userId == 0)
            {
                Log.Warning("userId == 0");
                return;
            }
            DBProxyComponent dbProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

            Dictionary<string,object> updates = new Dictionary<string,object>();
            updates.Add("BanTillTime", banTillTime);
            updates.Add("BanReason", banReason);
            bool result = await dbProxy.UpdateOneSet<DBAccountInfo>(p => p.Id == userId, updates);
            if(result == false)
            {
                Log.Warning($"封禁用户失败! userId:{userId} banTillTime:{banTillTime} banReason:{banReason}");
                return;
            }

            // 查找玩家在哪个服务器
            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
            Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
            IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
            {
                UserId = userId
            });
            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
            if (l2sGetLoginRecord == null)
            {
                Log.Warning("LoginCenter 没开启");
                return;
            }

            if(l2sGetLoginRecord.UserId != userId)
            {
                // 玩家没在线
                return;
            }

            // 通知Gate,将玩家踢下线
            var gateInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Gate, l2sGetLoginRecord.GateServerId);
            Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gateInfo.ServerInnerIP);
            IResponse response2 = await gameSession.Call(new S2Gate_DisconnectGateUser() { 
                DisconnectType = (int)DisconnectType.Ban,
                UserId = userId,
                BanTillTime = banTillTime,
                Reason = banReason,
            });
        }
    }
}
