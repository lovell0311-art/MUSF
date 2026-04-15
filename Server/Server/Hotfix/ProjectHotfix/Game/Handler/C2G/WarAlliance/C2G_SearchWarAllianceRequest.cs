
using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SearchWarAllianceRequestHandler : AMActorRpcHandler<C2G_SearchWarAllianceRequest, G2C_SearchWarAllianceResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SearchWarAllianceRequest b_Request, G2C_SearchWarAllianceResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SearchWarAllianceRequest b_Request, G2C_SearchWarAllianceResponse b_Response, Action<IMessage> b_Reply)
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
            if (playerWarAllianceComponent == null || playerWarAllianceComponent.WarAllianceID != 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2513);
                b_Reply(b_Response);
                return true;
            }
            if (b_Request.WarAllianceName.Length >= 17)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2530);
                b_Reply(b_Response);
                return false;
            }

            M2G_SearchWarAllianceRequest m2G_SearchWarAllianceRequest = new M2G_SearchWarAllianceRequest();
            m2G_SearchWarAllianceRequest.AppendData = b_Request.AppendData;
            m2G_SearchWarAllianceRequest.WarAllianceName = b_Request.WarAllianceName;
            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(m2G_SearchWarAllianceRequest) as IResponse;
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
                M2G_SearchWarAllianceResponse m2G_SearchWarAllianceResponse = Message as M2G_SearchWarAllianceResponse;
                b_Response.WarAllianceID = m2G_SearchWarAllianceResponse.WarAllianceID;
                b_Response.WarAllianceName = m2G_SearchWarAllianceResponse.WarAllianceName;
                b_Response.WarAllianceLevel = m2G_SearchWarAllianceResponse.WarAllianceLevel;
                b_Response.AllianceLeaderName = m2G_SearchWarAllianceResponse.AllianceLeaderName;
                b_Reply(b_Response);
                return true;
            }
        }
    }
}