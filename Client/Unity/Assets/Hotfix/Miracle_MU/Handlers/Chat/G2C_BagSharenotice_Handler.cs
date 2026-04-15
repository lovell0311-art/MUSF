using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 装备信息聊天消息
    /// </summary>
    [MessageHandler]
    public class G2C_BagSharenotice_Handler : AMHandler<G2C_BagSharenotice>
    {
        protected override void Run(ETModel.Session session, G2C_BagSharenotice message)
        {
            string colortype = (message.Color & 0xff) == -1 ? "white" : GetColor();
            int roomId = message.Color >> 8 == 0 ? 2 : 3;//message.Color >> 8 == 0是线路，1是全服
            ChatMessage chat = new ChatMessage
            {
                sendUserName = message.SendUserName,
                chatMessage = $"分享<color={colortype}>[{message.MessageInfo}]</color>",
                sendTime = message.SendTime,
                SendGameUserId = message.SendGameUserId,
                curRoonID = roomId,
                curLine = message.LineId,
                chatType = ChatMessageType.ItemChat,
                ccolor = message.Color & 0xff,
                itemId = message.ShareItemId
            };

            ChatMessageDataManager.AddChatMessage(chat);
            UIMainComponent.Instance?.ShowChatInfo(chat);
            UIComponent.Instance.Get(UIType.UIChatPanel)?.GetComponent<UIChatPanelComponent>().RefrenshMessage();

            string GetColor() => (message.Color & 0xff) switch
            {
                3 => "#f5f5f5",
                4 => "#38b641",
                5 => "#56abda",
                _ => string.Empty
            };
            
            //UIMainComponent.Instance.ShowHornNotice($"{message.SendUserName}:{message.MessageInfo}");
    }
}

}
