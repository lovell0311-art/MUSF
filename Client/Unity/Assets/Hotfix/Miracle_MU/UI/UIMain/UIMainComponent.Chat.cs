using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 聊天模块
    /// </summary>
    public partial class UIMainComponent
    {
        public Text chatInfo;
        public string lastChatContentl;
        public Queue<string> messagesQueue = new Queue<string>();
        public int indexs = 0;
        public void Init_Chat()
        {
            GameObject chatRoot = ReferenceCollector_Main.GetGameObject("Chat");
            if (chatRoot == null)
            {
                return;
            }

            ReferenceCollector referenceCollector_Chat = chatRoot.GetReferenceCollector();
            if (referenceCollector_Chat == null)
            {
                return;
            }

            Button chatBtn = referenceCollector_Chat.GetButton("chatBtn");
            if (chatBtn != null)
            {
                chatBtn.onClick.AddSingleListener(() => { UIComponent.Instance.VisibleUI(UIType.UIChatPanel); });
            }

            Button cutBtn = referenceCollector_Chat.GetButton("CutBtn");
            if (cutBtn != null)
            {
                cutBtn.onClick.AddSingleListener(() =>
                {
                    indexs++;

                    if (UIMainComponent.Instance?.Skills != null)
                    {
                        UIMainComponent.Instance.Skills.gameObject.SetActive(indexs % 2 == 1);
                    }

                    Transform btnList = UIMainComponent.Instance?.referenceCollector_BottomCenter?.transform?.Find("BtnList");
                    if (btnList != null)
                    {
                        btnList.gameObject.SetActive(indexs % 2 == 0);
                    }
                });
            }

            chatInfo = referenceCollector_Chat.GetText("chatInfo");
            if (chatInfo != null)
            {
                chatInfo.text = string.Empty;
            }


        }
        /// <summary>
        /// 显示聊天信息
        /// </summary>
        /// <param name="chat"></param>
        public void ShowChatInfo(ChatMessage chat)
        {
            if (chatInfo == null)
            {
                return;
            }

            //chatInfo.text = lastChatContentl + $"{ChatMessageDataManager.GetChatType((E_ChatType)chat.curRoonID)} {chat.sendUserName}：{chat.chatMessage}";
            //lastChatContentl = $"{ChatMessageDataManager.GetChatType((E_ChatType)chat.curRoonID)} {chat.sendUserName}：{chat.chatMessage}\n";
            //messages.Add($"{ChatMessageDataManager.GetChatType((E_ChatType)chat.curRoonID)} {chat.sendUserName}：{chat.chatMessage}\n");
            //for (int i = 0; i < messages.Count; i++)
            //{
            //    chatInfo.text += messages[i];
            //}
            chatInfo.text = null;
            if (messagesQueue.Count >= 3)
            {
                messagesQueue.Dequeue();
            }
            if (chat.curRoonID != 3)
                messagesQueue.Enqueue($"{ChatMessageDataManager.GetChatType((E_ChatType)chat.curRoonID)} {chat.sendUserName}：{chat.chatMessage}\n");
            else
                messagesQueue.Enqueue($"{ChatMessageDataManager.GetChatType((E_ChatType)chat.curRoonID)} {chat.curLine}线 {chat.sendUserName}：{chat.chatMessage}\n");
            for (int i = 0; i < messagesQueue.Count; i++)
            {
                chatInfo.text += messagesQueue.ToArray()[i];
            }

        }


    }

}
