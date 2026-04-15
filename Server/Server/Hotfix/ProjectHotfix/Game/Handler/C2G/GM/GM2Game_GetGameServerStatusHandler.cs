using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class GM2Game_GetGameServerStatusHandler : AMRpcHandler<GM2Game_GetGameServerStatus, Game2GM_GetGameServerStatus>
    {
        protected override async Task<bool> CodeAsync(Session session, GM2Game_GetGameServerStatus b_Request, Game2GM_GetGameServerStatus b_Response, Action<IMessage> b_Reply)
        {
            GameUserComponent gameUserComponent = Root.MainFactory.GetCustomComponent<GameUserComponent>();
            PlayerManageComponent playerManage = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();

            b_Response.OnlineCount = gameUserComponent.GetUserCount();
            int playerCount = 0;
            foreach(var v in playerManage.AllUserDic.Values)
            {
                playerCount += v.Count;
            }
            b_Response.EnterGameCount = playerCount;

            b_Reply(b_Response);
            return true;
        }
    }
}