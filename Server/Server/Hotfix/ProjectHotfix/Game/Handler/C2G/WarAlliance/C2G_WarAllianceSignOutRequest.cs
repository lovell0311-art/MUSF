using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_WarAllianceSignOutRequestHandler : AMActorRpcHandler<C2G_WarAllianceSignOutRequest, G2C_WarAllianceSignOutResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_WarAllianceSignOutRequest b_Request, G2C_WarAllianceSignOutResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_WarAllianceSignOutRequest b_Request, G2C_WarAllianceSignOutResponse b_Response, Action<IMessage> b_Reply)
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

            G2M_WarAllianceSignOutRequest g2M_WarAllianceSignOutRequest = new G2M_WarAllianceSignOutRequest();

            g2M_WarAllianceSignOutRequest.AppendData = b_Request.AppendData;
            g2M_WarAllianceSignOutRequest.WarAllianceID = playerWarAllianceComponent.WarAllianceID;
            g2M_WarAllianceSignOutRequest.GameUserID = mPlayer.GameUserId;

            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_WarAllianceSignOutRequest) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("退出失败!");
                b_Reply(b_Response);
                return false;
            }
            else
            {
                GMStruct_WarAllinceInfo gMStruct_WarAllinceInfo = new GMStruct_WarAllinceInfo();
                playerWarAllianceComponent.UpData(gMStruct_WarAllinceInfo,0,Help_TimeHelper.GetNowSecond()+7200);
                mPlayer.GetCustomComponent<GamePlayer>().Data.WarAllianceID = 0;
                b_Reply(b_Response);
                return true;
            }
        }
    }
}