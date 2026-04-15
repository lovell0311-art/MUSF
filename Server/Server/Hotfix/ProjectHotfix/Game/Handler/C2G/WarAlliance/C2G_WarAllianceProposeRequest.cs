using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_WarAllianceProposeRequestHandler : AMActorRpcHandler<C2G_WarAllianceProposeRequest, G2C_WarAllianceProposeResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_WarAllianceProposeRequest b_Request, G2C_WarAllianceProposeResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_WarAllianceProposeRequest b_Request, G2C_WarAllianceProposeResponse b_Response, Action<IMessage> b_Reply)
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

            if (playerWarAllianceComponent.MemberPost != (int)PostType.AllianceLeader && playerWarAllianceComponent.MemberPost != (int)PostType.ViceAllianceLeader)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2507);
                b_Reply(b_Response);
                return false;
            }

            G2M_WarAllianceProposeRequest g2M_WarAllianceProposeRequest = new G2M_WarAllianceProposeRequest();
            g2M_WarAllianceProposeRequest.AppendData = b_Request.AppendData;
            g2M_WarAllianceProposeRequest.WarAllianceID = playerWarAllianceComponent.WarAllianceID;
            g2M_WarAllianceProposeRequest.GameUserID = b_Request.GameUserID;
            
            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_WarAllianceProposeRequest) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;
               //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("踢出失败!");
                b_Reply(b_Response);
                return false;
            }
            else
            {
                //M2G_WarAllianceProposeResponse m2G_WarAllianceProposeResponse = Message as M2G_WarAllianceProposeResponse;

                Player mMemberPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.GameUserID);
                if (mMemberPlayer != null)
                {
                    GMStruct_WarAllinceInfo gMStruct_WarAllinceInfo = new GMStruct_WarAllinceInfo();
                    mMemberPlayer.GetCustomComponent<PlayerWarAllianceComponent>().UpData(gMStruct_WarAllinceInfo);
                    G2C_WarAllianceProposeNotice g2C_WarAllianceProposeNotice = new G2C_WarAllianceProposeNotice();
                    g2C_WarAllianceProposeNotice.Message = "已被战盟踢出";
                    mMemberPlayer.Send(g2C_WarAllianceProposeNotice);
                    mMemberPlayer.GetCustomComponent<GamePlayer>().Data.WarAllianceID = 0;
                }
                b_Reply(b_Response);
                return true;
            }

        }
    }
}
