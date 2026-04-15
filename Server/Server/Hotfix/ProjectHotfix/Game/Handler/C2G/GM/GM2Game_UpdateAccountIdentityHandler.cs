using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class GM2Game_UpdateAccountIdentityHandler : AMRpcHandler<GM2Game_UpdateAccountIdentity, Game2GM_UpdateAccountIdentity>
    {
        protected override async Task<bool> CodeAsync(Session session, GM2Game_UpdateAccountIdentity b_Request, Game2GM_UpdateAccountIdentity b_Response, Action<IMessage> b_Reply)
        {
            long gameUserId = b_Request.GameUserId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, gameUserId))
            {
                PlayerManageComponent playerManage = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();
                Player player = null;
                int zoneId = 0;
                foreach (var kv in playerManage.AllUserDic)
                {
                    if(kv.Value.TryGetValue(gameUserId,out player))
                    {
                        zoneId = kv.Key;
                        break;
                    }

                }
                if(player == null)
                {
                    b_Response.Error = ErrorCode.ERR_AccountNotOffline;
                    b_Reply(b_Response);
                    return true;
                }
                if(player.OnlineStatus != EOnlineStatus.Online)
                {
                    b_Response.Error = ErrorCode.ERR_AccountNotOffline;
                    b_Reply(b_Response);
                    return true;
                }
                DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
                DBAccountInfo dbLoginInfo = null;
                if (mDBProxy != null)
                {
                    var list = await mDBProxy.Query<DBAccountInfo>(p => p.Id == player.UserId);
                    if (list.Count > 0)
                    {
                        dbLoginInfo = list[0] as DBAccountInfo;
                    }
                    if (dbLoginInfo.Identity == AccountIdentity.Studio)
                    {
                        player.GetCustomComponent<GamePlayer>().dBCharacterDroplimit.Restrict = 1;
                    }
                    else
                    {
                        player.GetCustomComponent<GamePlayer>().dBCharacterDroplimit.Restrict = 0;
                    }
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}