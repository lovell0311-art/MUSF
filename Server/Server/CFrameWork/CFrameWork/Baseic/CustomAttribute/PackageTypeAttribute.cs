using System;

namespace CustomFrameWork
{
    /// <summary>
    /// 网络协议特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ResultPackageAttribute : BaseAttribute
    {
        /// <summary>
        /// id1
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// id2
        /// </summary>
        public int ID2 { get; set; }
        /// <summary>
        /// 网络协议
        /// </summary>
        /// <param name="b_MessageId"></param>
        /// <param name="b_MessageId2"></param>
        public ResultPackageAttribute(int b_MessageId, int b_MessageId2)
        {
            this.ID = b_MessageId;
            this.ID2 = b_MessageId2;
        }
    }
    /// <summary>
    /// 网络消息特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class MessagePackageAttribute : BaseAttribute
    {
        /// <summary>
        /// 网络消息中的网络协议类型
        /// </summary>
        public Type Type { get; private set; }
        /// <summary>
        /// 网络消息特性
        /// </summary>
        /// <param name="b_MessageType"></param>
        public MessagePackageAttribute(Type b_MessageType)
        {
            this.Type = b_MessageType;
        }
    }
}
