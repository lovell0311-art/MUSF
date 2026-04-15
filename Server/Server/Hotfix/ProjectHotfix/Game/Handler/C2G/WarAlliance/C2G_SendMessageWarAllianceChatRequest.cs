using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SendMessageWarAllianceChatRequestHandler : AMActorRpcHandler<C2G_SendMessageWarAllianceChatRequest, G2C_SendMessageWarAllianceChatResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SendMessageWarAllianceChatRequest b_Request, G2C_SendMessageWarAllianceChatResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SendMessageWarAllianceChatRequest b_Request, G2C_SendMessageWarAllianceChatResponse b_Response, Action<IMessage> b_Reply)
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

            G2M_SendMessageWarAllianceChatRequest g2M_SendMessageWarAllianceChatRequest = new G2M_SendMessageWarAllianceChatRequest();
            g2M_SendMessageWarAllianceChatRequest.AppendData = b_Request.AppendData;
            g2M_SendMessageWarAllianceChatRequest.WarAllianceID = playerWarAllianceComponent.WarAllianceID;
            g2M_SendMessageWarAllianceChatRequest.SendGameUserId = mPlayer.GameUserId;
            g2M_SendMessageWarAllianceChatRequest.SendUserName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
            g2M_SendMessageWarAllianceChatRequest.ChatMessage = b_Request.ChatMessage;
            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_SendMessageWarAllianceChatRequest) as IResponse;
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
                b_Reply(b_Response);
                return true;
            }
        }
    }
}