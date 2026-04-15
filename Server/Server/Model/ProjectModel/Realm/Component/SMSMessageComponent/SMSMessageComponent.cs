using System;
using System.Collections.Generic;
using System.Diagnostics;
using CustomFrameWork.Baseic;
using CustomFrameWork;

namespace ETModel
{
    /// <summary>
    /// 短信验证码类型
    /// </summary>
    public enum ESMSCodeType
    {
        Register,   // 注册
        ResetPasswd,// 修改密码
        End,
    }

    public class SMSCodeLimit
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 当天发送次数
        /// </summary>
        public int SendCountToDay { get; set; }
    }

    public class SMSMessageComponent : TCustomComponent<MainFactory>
    {
        public class SFZYZ2classResult
        {
            public int result { get; set; }
            public string desc { get; set; }
            public string sex { get; set; }
            public string birthday { get; set; }
            public string address { get; set; }
        }

        public class SFZYZ2class
        {
            public int code { get; set; }
            public string msg { get; set; }
            public string taskNo { get; set; }
            public SFZYZ2classResult data { get; set; }
            public bool success { get; set; }
        }


        #region Proto
        public class SendStatusSet
        {
            public string SerialNo { get; set; }
            public string PhoneNumber { get; set; }
            public int Fee { get; set; }

            public string SessionContext { get; set; }

            public string Code { get; set; }

            public string Message { get; set; }

            public string IsoCode { get; set; }
        }
        public class SendResult
        {
            public List<SendStatusSet> SendStatusSet { get; set; }

            public string RequestId { get; set; }

        }
        #endregion
        /// <summary>
        /// 短信渠道 
        /// </summary>
        public int SmsChannel { get; set; }
        /// <summary>
        /// 是否验证 1 为验证 0 为不验证
        /// </summary>
        public int IsVerify { get; set; }

        public Dictionary<string, (string,long)>[] SmsCodeDict = new Dictionary<string, (string, long)>[(int)ESMSCodeType.End];

        public override void Awake() {
            base.Awake();
            for (int i = 0; i < SmsCodeDict.Length; i++)
            {
                SmsCodeDict[i] = new Dictionary<string, (string, long)>();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="smsChannel">短信渠道</param>
        /// <param name="isVerify">是否验证 1 为验证 0 为不验证</param>
        public void Set(int smsChannel, int isVerify)
        {
            this.SmsChannel = smsChannel;
            this.IsVerify = isVerify;
        }
    }
}