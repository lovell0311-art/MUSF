using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 附近聊天消息
    /// </summary>
    [MessageHandler]
    public class G2C_SendChatMessageToNearby_Handler : AMHandler<ChatMessageFromNearby_notice>
    {
        protected override void Run(ETModel.Session session, ChatMessageFromNearby_notice message)
        {
          
            ChatMessage chat = new ChatMessage
            {
                sendUserName = message.SendUserName,
                chatMessage = message.ChatMessage,
                sendTime = message.SendTime,
                SendGameUserId = message.SendGameUserId,
                curRoonID = 4,
                chatType = ChatMessageType.NormalChat
            };
            ChatMessageDataManager.AddChatMessage(chat);
            UIMainComponent.Instance?.ShowChatInfo(chat);
            UIComponent.Instance.Get(UIType.UIChatPanel)?.GetComponent<UIChatPanelComponent>().RefrenshMessage();
        }
    }
}
