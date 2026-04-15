using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System;

namespace ETHotfix
{
    /// <summary>
    /// 世界聊天消息
    /// </summary>
    [MessageHandler]
    public class G2C_MessageInChatRoom_Handler : AMHandler<G2C_MessageInChatRoom_notice>
    {
        protected override void Run(ETModel.Session session, G2C_MessageInChatRoom_notice message)
        {
           

            int curRoomID = -1;
            if (ChatMessageDataManager.valuePairs.ContainsValue(message.ChatRoomID))
            {
                foreach (var item in ChatMessageDataManager.valuePairs)
                {
                    if(item.Value == message.ChatRoomID)
                    {
                        switch (item.Key)
                        {
                            case E_ChatType.World:
                                curRoomID = 1;
                                break;
                            case E_ChatType.Family:
                                curRoomID = 5;
                                break;
                            case E_ChatType.Team:
                                curRoomID = 6;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            ChatMessage chat = new ChatMessage
            {
                sendUserName = message.SendUserName,
                chatMessage = message.ChatMessage, 
                sendTime = message.SendTime,
                chatRoomID = message.ChatRoomID,
                curRoonID = curRoomID,
                chatType = ChatMessageType.NormalChat
            };
            ChatMessageDataManager.AddChatMessage(chat);
            UIMainComponent.Instance?.ShowChatInfo(chat);
            UIComponent.Instance.Get(UIType.UIChatPanel)?.GetComponent<UIChatPanelComponent>().RefrenshMessage();
        }
    }
}