
using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenWarAllianceHandler : AMActorRpcHandler<C2G_OpenWarAllianceRequest, G2C_OpenWarAllianceResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenWarAllianceRequest b_Request, G2C_OpenWarAllianceResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenWarAllianceRequest b_Request, G2C_OpenWarAllianceResponse b_Response, Action<IMessage> b_Reply)
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

            PlayerWarAllianceComponent playerWarAllianceComponent=  mPlayer.AddCustomComponent<PlayerWarAllianceComponent>();
            if (playerWarAllianceComponent == null || playerWarAllianceComponent.WarAllianceID == 0)
            {
                if (await playerWarAllianceComponent.OnInit(mAreaId))
                {
                    if (playerWarAllianceComponent.WarAllianceID == 0)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2513);
                        b_Reply(b_Response);
                        return true;
                    }
                }
            }

            G2M_OpenWarAllianceRequest g2M_OpenWarAllianceRequest = new G2M_OpenWarAllianceRequest();
            g2M_OpenWarAllianceRequest.AppendData = b_Request.AppendData;
            g2M_OpenWarAllianceRequest.WarAllianceID = playerWarAllianceComponent.WarAllianceID;
            g2M_OpenWarAllianceRequest.GameUserID = mPlayer.GameUserId;
            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_OpenWarAllianceRequest) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;
                b_Reply(b_Response);
                return false;
            }
            else
            {
                M2G_OpenWarAllianceResponse m2G_OpenWarAllianceResponse = Message as M2G_OpenWarAllianceResponse;
                /*b_Response.Info.WarAllianceID = m2G_OpenWarAllianceResponse.Info.WarAllianceID;
                b_Response.Info.WarAllianceName = m2G_OpenWarAllianceResponse.Info.WarAllianceName;
                b_Response.Info.LeaderName = m2G_OpenWarAllianceResponse.Info.LeaderName;
                b_Response.Info.WarAllianceLevel = m2G_OpenWarAllianceResponse.Info.WarAllianceLevel;
                b_Response.Info.WarAllianceNotice = m2G_OpenWarAllianceResponse.Info.WarAllianceNotice;
                b_Response.Info.MemberPost = m2G_OpenWarAllianceResponse.Info.MemberPost;
                b_Response.Info.WarAllianceBadge = m2G_OpenWarAllianceResponse.Info.WarAllianceBadge;*/
                b_Response.Info = new Struct_WarAllinceInfo();
                playerWarAllianceComponent.UpData(m2G_OpenWarAllianceResponse.Info);
                b_Response.Info = playerWarAllianceComponent.GetInfo();

                b_Response.MemberListCont = m2G_OpenWarAllianceResponse.MemberListCont;
                b_Reply(b_Response);
                return true;
            }
        }
    }
}