using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 好友信息类
    /// </summary>
    public class FriendInfo
    {
        /// <summary>
        /// 状态
        /// </summary>
        public string State;
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName;
        /// <summary>
        /// 所在场景
        /// </summary>
        public string SceneName;
        /// <summary>
        /// 账号ID
        /// </summary>
        public long AccentID;
        /// <summary>
        /// 实体ID
        /// </summary>
        public long UUID;
        /// <summary>
        /// 好友类型
        /// </summary>
        public int Type;
        /// <summary>
        /// 角度
        /// </summary>
        public int Angle;
        /// <summary>
        /// 战盟
        /// </summary>
        public string Zhanmeng;
        /// <summary>
        /// 战盟身份
        /// </summary>
        public string Identity;
        /// <summary>
        /// 队伍
        /// </summary>
        public string Teams;
        /// <summary>
        /// 等级
        /// </summary>
        public int Level;
        /// <summary>
        /// 职业
        /// </summary>
        public string Job;
        /// <summary>
        /// 是否被选择
        /// </summary>
        public bool isChoose = false;
        /// <summary>
        /// 拉黑或屏蔽时间
        /// </summary>
        public long TimeDate;
        /// <summary>
        /// 发送或接受消息时间
        /// </summary>
        public long SendOrReceiveMessageTimeDate;
    }
    public class FriendChatNewInfo
    {
        public long UUID;
        public string NickName;
        public long Time;
        public string Message;
        /// <summary>
        /// x坐标
        /// </summary>
        public int XPos;
        /// <summary>
        /// y坐标
        /// </summary>
        public int YPos;
        public int mapID;
        public ChatType type;
    }
}