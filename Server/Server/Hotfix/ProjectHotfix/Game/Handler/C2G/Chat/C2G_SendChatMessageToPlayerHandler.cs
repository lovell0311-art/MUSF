using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SendChatMessageToPlayerHandler : AMActorRpcHandler<C2G_SendChatMessageToPlayer, G2C_SendChatMessageToPlayer>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SendChatMessageToPlayer b_Request, G2C_SendChatMessageToPlayer b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SendChatMessageToPlayer b_Request, G2C_SendChatMessageToPlayer b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
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

            Player targetPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.PlayerGameUserId);
            if (targetPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1308);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标玩家不存在或已下线!");
                b_Reply(b_Response);
                return false;
            }

            if (string.IsNullOrWhiteSpace(b_Request.ChatMessage))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1307);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不允许发送空内容!");
                b_Reply(b_Response);
                return false;
            }

            var gamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            targetPlayer.Send(new ReceivePlayerChatMessage_notice()
            {
                ChatMessage = b_Request.ChatMessage,
                PlayerGameUserId = mPlayer.GameUserId,
                PlayerName = gamePlayer.Data.NickName,
                SendTime = CustomFrameWork.TimeHelper.ClientNowSeconds()
            });

            b_Reply(b_Response);
            return true;
        }
    }
}