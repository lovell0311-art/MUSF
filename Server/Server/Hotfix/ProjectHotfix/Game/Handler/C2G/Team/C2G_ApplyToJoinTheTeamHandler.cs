using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ApplyToJoinTheTeamHandler : AMActorRpcHandler<C2G_ApplyToJoinTheTeam, G2C_ApplyToJoinTheTeam>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ApplyToJoinTheTeam b_Request, G2C_ApplyToJoinTheTeam b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ApplyToJoinTheTeam b_Request, G2C_ApplyToJoinTheTeam b_Response, Action<IMessage> b_Reply)
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1202);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("找不到组队组件!");
                b_Reply(b_Response);
                return false;
            }

            TeamComponent component = mPlayer.GetCustomComponent<TeamComponent>();
            if (component != null && component.TeamID != 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1220);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("已经在队伍中，无法申请加入队伍!");
                b_Reply(b_Response);
                return false;
            }

            Player targetPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.PlayerGameUserId);
            if (targetPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1221);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标不在线或不存在，无法申请加入队伍!");
                b_Reply(b_Response);
                return false;
            }
            var targetComponent = targetPlayer.GetCustomComponent<TeamComponent>();
            if (targetComponent == null || targetComponent.TeamID == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1222);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标不在队伍中，无法申请加入队伍!");
                b_Reply(b_Response);
                return false;
            }

            if (!teamManager.isCanAddMember(targetComponent.TeamID))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1223);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标队伍已满人，无法申请");
                b_Reply(b_Response);
                return false;
            }
            //发送申请推送给队长
            Player captainPlayer = null;
            if (targetComponent.IsCaptain)
            {
                captainPlayer = targetPlayer;
            }
            else
            {
                var members = teamManager.GetAllByTeamID(targetComponent.TeamID);
                foreach (var item in members)
                {
                    if (item.Value.GetCustomComponent<TeamComponent>().IsCaptain)
                    {
                        captainPlayer = item.Value;
                    }
                }
            }
            GamePlayer gamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            string curWarAllianceName = gamePlayer.Player.GetCustomComponent<PlayerWarAllianceComponent>()?.WarAllianceName;
            Team_PlayerData playerData = new Team_PlayerData()
            {
                GameUserId = gamePlayer.InstanceId,
                Name = gamePlayer.Data.NickName,
                Level = gamePlayer.Data.Level,
                PlayerTypeId = gamePlayer.Data.PlayerTypeId,
                OccupationLevel = gamePlayer.Data.OccupationLevel,
                WarAllianceName = (curWarAllianceName == null) ? "" : curWarAllianceName,
                TeamId = 0,
            };

            captainPlayer.Send(new G2C_PlayerApplyJoinTheTeam_notice()
            {
                PlayerData = playerData,
            });

            b_Reply(b_Response);
            return true;
        }
    }
}