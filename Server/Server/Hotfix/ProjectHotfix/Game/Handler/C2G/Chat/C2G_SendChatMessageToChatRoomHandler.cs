using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SendChatMessageToChatRoomHandler : AMActorRpcHandler<C2G_SendChatMessageToChatRoom, G2C_SendChatMessageToChatRoom>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SendChatMessageToChatRoom b_Request, G2C_SendChatMessageToChatRoom b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SendChatMessageToChatRoom b_Request, G2C_SendChatMessageToChatRoom b_Response, Action<IMessage> b_Reply)
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


            ChatManageComponent chatManager = Root.MainFactory.GetCustomComponent<ChatManageComponent>();
            if (chatManager == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1300);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("找不到组件!");
                b_Reply(b_Response);
                return false;
            }

            if (string.IsNullOrWhiteSpace(b_Request.ChatMessage))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1307);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不允许发送空内容");
                b_Reply(b_Response);
                return false;
            }

            if (b_Request.ChatMessage.Length > 15)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1307);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不允许发送空内容");
                b_Reply(b_Response);
                return false;
            }

            if (mPlayer.GetCustomComponent<GamePlayer>().Data.Level < 100)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1307);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不允许发送空内容");
                b_Reply(b_Response);
                return false;
            }
            long Time = Help_TimeHelper.GetNowSecond();
            if (mPlayer.GetCustomComponent<GamePlayer>().SendMassgeTime > Time)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1307);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不允许发送空内容");
                b_Reply(b_Response);
                return false;
            }
            mPlayer.GetCustomComponent<GamePlayer>().SendMassgeTime = Time + 10;
            var roomPlayerDict = chatManager.GetAllByRoomID(b_Request.ChatRoomID);
            if (roomPlayerDict != null)
            {
                var gamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
                foreach (var item in roomPlayerDict)
                {
                    G2C_MessageInChatRoom_notice notice = new G2C_MessageInChatRoom_notice()
                    {
                        ChatRoomID = b_Request.ChatRoomID,
                        ChatMessage = b_Request.ChatMessage,
                        SendGameUserId = mPlayer.GameUserId,
                        SendUserName = gamePlayer.Data.NickName,
                        SendTime = CustomFrameWork.TimeHelper.ClientNowSeconds()
                    };
                    item.Value.Send(notice);
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}