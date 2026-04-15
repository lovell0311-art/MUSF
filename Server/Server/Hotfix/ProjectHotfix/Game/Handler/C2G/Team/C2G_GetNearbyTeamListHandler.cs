using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetNearbyTeamListHandler : AMActorRpcHandler<C2G_GetNearbyTeamList, G2C_GetNearbyTeamList>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetNearbyTeamList b_Request, G2C_GetNearbyTeamList b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetNearbyTeamList b_Request, G2C_GetNearbyTeamList b_Response, Action<IMessage> b_Reply)
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
            HashSet<long> teamIDList = new HashSet<long>();
            foreach (GamePlayer gamePlayer in gamePlayers)
            {
                TeamComponent curTeamComponent = gamePlayer.Player.GetCustomComponent<TeamComponent>();
                //检查是否已经组队
                if (curTeamComponent != null && curTeamComponent.TeamID > 0)
                {
                    long curTeamID = curTeamComponent.TeamID;
                    if (!teamIDList.Contains(curTeamID))    //检查TeamID是否重复，重复不添加到列表
                    {
                        teamIDList.Add(curTeamID);
                        var memberDict = teamManager.GetAllByTeamID(curTeamID);
                        if (memberDict != null)
                        {
                            //找到队长
                            foreach (var member in memberDict)
                            {
                                TeamComponent memberComponent = member.Value.GetCustomComponent<TeamComponent>();
                                if (memberComponent != null && memberComponent.IsCaptain)
                                {
                                    b_Response.TeamList.Add(new TeamData()
                                    {
                                        LeaderGameUserId = member.Value.GameUserId,
                                        LeaderUserName = member.Value.GetCustomComponent<GamePlayer>().Data.NickName,
                                        TeamMemberCount = memberDict.Count
                                    });
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("周围共有" + gamePlayers.Count + "个玩家");
            b_Reply(b_Response);
            return true;
        }
    }
}