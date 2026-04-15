using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 全服聊天消息
    /// </summary>
    [MessageHandler]
    public class G2C_FullServiceHornnotice_Handler : AMHandler<G2C_FullServiceHornnotice>
    {
        protected override void Run(ETModel.Session session, G2C_FullServiceHornnotice message)
        {
            int roodId = message.SendGameUserId == 0 ? 1 :3;
            ChatMessage chat = new ChatMessage
            {
                sendUserName = message.SendUserName,
                chatMessage = message.MessageInfo,
                sendTime = message.SendTime,
                SendGameUserId = message.SendGameUserId,
                curRoonID = roodId,
                curLine = message.LineId,
                chatType = ChatMessageType.NormalChat
            };
            ChatMessageDataManager.AddChatMessage(chat);
            UIMainComponent.Instance?.ShowChatInfo(chat);
            UIComponent.Instance.Get(UIType.UIChatPanel)?.GetComponent<UIChatPanelComponent>().RefrenshMessage();

            //UIMainComponent.Instance.ShowHornNotice($"{message.SendUserName}:{message.MessageInfo}");
        }
    }

}
