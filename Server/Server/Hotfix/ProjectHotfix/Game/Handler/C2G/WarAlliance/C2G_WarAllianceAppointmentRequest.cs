using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_WarAllianceAppointmentRequestHandler : AMActorRpcHandler<C2G_WarAllianceAppointmentRequest, G2C_WarAllianceAppointmentResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_WarAllianceAppointmentRequest b_Request, G2C_WarAllianceAppointmentResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_WarAllianceAppointmentRequest b_Request, G2C_WarAllianceAppointmentResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_AccountNotExists;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }

            PlayerWarAllianceComponent playerWarAllianceComponent = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (playerWarAllianceComponent == null || playerWarAllianceComponent.WarAllianceID == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2500);
                b_Reply(b_Response);
                return false;
            }

            if (playerWarAllianceComponent.MemberPost != (int)PostType.AllianceLeader)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2507);
                b_Reply(b_Response);
                return false;
            }
            if (b_Request.GameUserID == mPlayer.GameUserId)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2515);
                b_Reply(b_Response);
                return false;
            }
            if (b_Request.ClassType != 3)
            {
                var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);
                if (mDBProxy == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2500);
                    b_Reply(b_Response);
                    return false;
                }
                var WInfo = await mDBProxy.Query<DBMemberInfo>(p =>p.DBWarAllianceID == playerWarAllianceComponent.WarAllianceID&& p.MemberID == b_Request.GameUserID && p.MemberPost == b_Request.ClassType && p.IsDisabled == 1);
                if (WInfo==null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2500);
                    b_Reply(b_Response);
                    return false;
                }
                if (WInfo.Count >= 1)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2527);
                    b_Reply(b_Response);
                    return false;
                }
            }
            G2M_WarAllianceAppointmentRequest g2M_WarAllianceAppointmentRequest = new G2M_WarAllianceAppointmentRequest();
            g2M_WarAllianceAppointmentRequest.AppendData = b_Request.AppendData;
            g2M_WarAllianceAppointmentRequest.WarAllianceID = playerWarAllianceComponent.WarAllianceID;
            g2M_WarAllianceAppointmentRequest.GameUserID = b_Request.GameUserID;
            g2M_WarAllianceAppointmentRequest.ClassType = b_Request.ClassType;
            g2M_WarAllianceAppointmentRequest.LeaderUserID = mPlayer.GameUserId;
            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_WarAllianceAppointmentRequest) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("失败!");
                b_Reply(b_Response);
                return false;
            }
            else
            {
                if (b_Request.ClassType == (int)PostType.AllianceLeader)
                {
                    playerWarAllianceComponent.MemberPost = 0;
                }
                Player b_Player = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.GameUserID);
                if (b_Player != null)
                {
                    b_Player.GetCustomComponent<PlayerWarAllianceComponent>().MemberPost = b_Request.ClassType;
                    switch (b_Request.ClassType)
                    {
                        case 3:
                            b_Player.GetCustomComponent<GamePlayer>().Data.WallTile = $"<{playerWarAllianceComponent.WarAllianceName}>%盟主";
                            break;
                        case 2:
                            b_Player.GetCustomComponent<GamePlayer>().Data.WallTile = $"<{playerWarAllianceComponent.WarAllianceName}>%副盟主";
                            break;
                        case 1:
                            b_Player.GetCustomComponent<GamePlayer>().Data.WallTile = $"<{playerWarAllianceComponent.WarAllianceName}>%大队长";
                            break;
                    }
                }
                b_Reply(b_Response);
                return true;
            }

        }
    }
}
