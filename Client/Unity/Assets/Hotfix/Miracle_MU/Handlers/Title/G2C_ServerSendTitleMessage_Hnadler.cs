using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_ServerSendTitleMessage_Hnadler : AMHandler<G2C_ServerSendTitleMessage>
    {
        protected override void Run(ETModel.Session session, G2C_ServerSendTitleMessage message)
        {
            Log.DebugBrown($"通知称号 {message.TitleList.Count}"+":::"+ message.UseTitle);
            TitleManager.allTitles.Clear();
            TitleManager.useID = message.UseTitle;
            for (int i = 0,length = message.TitleList.Count; i < length; i++)
            {
                UITitleInfo uITitleInfo = new UITitleInfo();
                uITitleInfo.TitleId = message.TitleList[i].TitleID;
                uITitleInfo.BingTime = message.TitleList[i].BingTime;
                uITitleInfo.EndTime = message.TitleList[i].EndTime;
                if(message.UseTitle == message.TitleList[i].TitleID)
                    uITitleInfo.UseInfo = (message.UseTitle == message.TitleList[i].TitleID) ? 2 : 1;//获取了，未使用
                TitleManager.allTitles.Add(uITitleInfo);
            }
        }
    }
}