
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public class UIE_MailItemInfo
    {
        public UIE_MailInfo uIE_MailInfo = new UIE_MailInfo();
        public GameObject item;
    }
    public class UIE_MailInfo
    {
        /// <summary>
        /// 邮件ID
        /// </summary>
        public long id;
        /// <summary>
        /// 邮件是否已读
        /// </summary>
        public bool emailIsRend = false;
        /// <summary>
        /// 当前邮件是否选中
        /// </summary>
        public bool isClick = false;
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Title = string.Empty;
        /// <summary>
        /// 邮件收到时间
        /// </summary>
        public long email_Time;
        /// <summary>
        /// 邮件到期时间
        /// </summary>
        public long email_EndTime;
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string emailContent;
        /// <summary>
        /// 附件是否领取了
        /// </summary>
        public bool AttachmentIsGet = false;
        /// <summary>
        /// 附件信息
        /// </summary>
        public List<UIE_MailAttachment> mailAttachment = new List<UIE_MailAttachment>();
    }
    public class UIE_MailAttachment
    {
        /// <summary>
        /// 附件配置表ID
        /// </summary>
        public int ConfigId = 0;
        /// <summary>
        /// 附件ID
        /// </summary>
        public long Id = 0;
        /// <summary>
        /// 道具品质
        /// </summary>
        public int ItemQuality;
        /// <summary>
        /// 附件数量
        /// </summary>
        public int AttachmentCount;
    }
}