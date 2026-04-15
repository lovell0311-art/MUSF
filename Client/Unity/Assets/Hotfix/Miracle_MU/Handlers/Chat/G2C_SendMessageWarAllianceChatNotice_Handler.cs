using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 战盟聊天消息
    /// </summary>
    [MessageHandler]
    public class G2C_SendMessageWarAllianceChatNotice_Handler : AMHandler<G2C_SendMessageWarAllianceChatNotice>
    {
        protected override void Run(ETModel.Session session, G2C_SendMessageWarAllianceChatNotice message)
        {
          
            ChatMessage chat = new ChatMessage
            {
                sendUserName = message.SendUserName,
                chatMessage = message.ChatMessage,
                sendTime = message.SendDataTime,
                SendGameUserId = message.SendGameUserId,
                curRoonID = 5,
                chatType = ChatMessageType.NormalChat
            };
            ChatMessageDataManager.AddChatMessage(chat);
            UIMainComponent.Instance?.ShowChatInfo(chat);
            UIComponent.Instance.Get(UIType.UIChatPanel)?.GetComponent<UIChatPanelComponent>().RefrenshMessage();
        }
    }
}
