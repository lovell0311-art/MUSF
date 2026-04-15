using ETHotfix;
using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class SendFullServiceMessage_Handler : AMHandler<G2C_SendFullServiceMessage>
    {
        protected override void Run(ETModel.Session session, G2C_SendFullServiceMessage message)
        {
            string noticeStr = message.MessageId.GetTipInfo();
            if (noticeStr.Contains("PlayerName"))
            {
                noticeStr = noticeStr.Replace("PlayerName", message.PlayerName);
            }
            if (noticeStr.Contains("TitleName"))
            {
                TitleConfig_InfoConfig titleConfig = ConfigComponent.Instance.GetItem<TitleConfig_InfoConfig>(message.TitleID);
                if (titleConfig != null && !string.IsNullOrEmpty(titleConfig.Name))
                {
                    noticeStr = noticeStr.Replace("TitleName", titleConfig.Name);
                }
            }
            UIMainComponent.Instance.ShowNotice($"{noticeStr}");
        }
    }
}
