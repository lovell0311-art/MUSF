using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// Ë½ÁÄÁÄÌìÏûÏ¢
    /// </summary>
    [MessageHandler]
    public class G2C_ReceivePlayerChatMessage_notice_Handler : AMHandler<ReceivePlayerChatMessage_notice>
    {
        protected override void Run(ETModel.Session session, ReceivePlayerChatMessage_notice message)
        {
            Log.DebugBrown($"Ë½ÁÄ:{message.ChatMessage}");
            ChatMessage chat = new ChatMessage
            {
                sendUserName = message.PlayerName,
                chatMessage = message.ChatMessage,
                sendTime = message.SendTime,
                SendGameUserId = message.PlayerGameUserId,
                curRoonID = 7,
                chatType = ChatMessageType.NormalChat
            };
            ChatMessageDataManager.AddChatMessage(chat);
            UIMainComponent.Instance?.ShowChatInfo(chat);
            UIComponent.Instance.Get(UIType.UIChatPanel)?.GetComponent<UIChatPanelComponent>().RefrenshMessage();
        }
    }
}

