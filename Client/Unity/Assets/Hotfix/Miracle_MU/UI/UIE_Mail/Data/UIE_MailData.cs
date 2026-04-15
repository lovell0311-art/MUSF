using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETHotfix
{
    public static class UIE_MailData
    {
        /// <summary>
        /// 所有邮件信息
        /// </summary>
        public static List<UIE_MailInfo> uIE_MailInfos = new List<UIE_MailInfo>();
        /// <summary>
        /// 上一个点击的邮件
        /// </summary>
        public static UIE_MailItemInfo lastClickEmail = new UIE_MailItemInfo();
        /// <summary>
        /// 是否有未领取附件
        /// </summary>
        /// <returns></returns>
        public static bool GetNoReceive()
        {
            for (int i = 0, length = uIE_MailInfos.Count; i < length; i++)
            {
                //有未领取的附件
                if (!uIE_MailInfos[i].AttachmentIsGet)
                {
                    return false;
                }
            }
            return true;
        }

        public static void AddEmail(Mailinfo mailinfo)
        {
            UIE_MailInfo uIE_MailInfo = new UIE_MailInfo();
            uIE_MailInfo.id = mailinfo.MailId;
            uIE_MailInfo.Title = mailinfo.MailName;
            uIE_MailInfo.email_Time = mailinfo.MailAcceptanceTime;
            uIE_MailInfo.email_EndTime = mailinfo.MailValidTime;
            uIE_MailInfo.emailContent = mailinfo.MailContent;
            uIE_MailInfo.emailIsRend = mailinfo.MailState == 0 ? false : true;
            uIE_MailInfo.AttachmentIsGet = mailinfo.ReceiveOrNot == 0 ? false : true;
           
            if(mailinfo.MailEnclosure.Count == 0) { uIE_MailInfo.AttachmentIsGet = true; }//附件数量等于0说明没有附件
            for (int j = 0, length_j = mailinfo.MailEnclosure.Count; j < length_j; j++)
            {
                UIE_MailAttachment mailAttachment = new UIE_MailAttachment();
                mailAttachment.Id = mailinfo.MailEnclosure[j].ItemID;
                mailAttachment.ConfigId = mailinfo.MailEnclosure[j].ItemConfigID;
                mailAttachment.AttachmentCount = mailinfo.MailEnclosure[j].ItemCnt;
                uIE_MailInfo.mailAttachment.Add(mailAttachment);
            }
            uIE_MailInfos.Add(uIE_MailInfo);
        }
    }
}
