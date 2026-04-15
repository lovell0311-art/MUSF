using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetNearbyPlayerListHandler : AMActorRpcHandler<C2G_GetNearbyPlayerList, G2C_GetNearbyPlayerList>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetNearbyPlayerList b_Request, G2C_GetNearbyPlayerList b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetNearbyPlayerList b_Request, G2C_GetNearbyPlayerList b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            TeamManageComponent teamManager = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
            if (teamManager == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1206);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("找不到组队组件!");
                b_Reply(b_Response);
                return false;
            }

            using ListComponent<GamePlayer> gamePlayersList = ListComponent<GamePlayer>.Create();
            List<GamePlayer> gamePlayers = gamePlayersList;
            if (!mPlayer.TryGetAroundPlayer(ref gamePlayers))
            {
                b_Reply(b_Response);
                return true;
            }
            foreach (GamePlayer gamePlayer in gamePlayers)
            {
                TeamComponent curTeamComponent = gamePlayer.Player.GetCustomComponent<TeamComponent>();
                //检查是否已经组队
                long curTeamID = 0;
                if (curTeamComponent != null && curTeamComponent.TeamID > 0)
                {
                    curTeamID = curTeamComponent.TeamID;
                }
                string curWarAllianceName = gamePlayer.Player.GetCustomComponent<PlayerWarAllianceComponent>()?.WarAllianceName;

                b_Response.PlayerList.Add(new Team_PlayerData()
                {
                    GameUserId = gamePlayer.InstanceId,
                    Name = gamePlayer.Data.NickName,
                    Level = gamePlayer.Data.Level,
                    PlayerTypeId = gamePlayer.Data.PlayerTypeId,
                    OccupationLevel = gamePlayer.Data.OccupationLevel,
                    WarAllianceName = (curWarAllianceName == null) ? "" : curWarAllianceName,
                    TeamId = curTeamID,
                });
            }

            b_Reply(b_Response);
            return true;
        }
    }
}