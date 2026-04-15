using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ETModel
{

    [BsonIgnoreExtraElements]
    public class Friend : CustomComponent
    {
        /// <summary>ID</summary>
        public long GameUserId { get; set; }
        /// <summary>类型</summary>
        public int ListType { get; set; }
        /// <summary>角色名</summary>
        public string CharName { get; set; }
        /// <summary>是否删除</summary>
        public int IsDisabled { get; set; }
        /// <summary>是否在线</summary>
        public int iState { get; set; }
        /// <summary>玩家等级/// </summary>
        public int iLv { get; set; }
        /// <summary>
        /// 拉黑或者被击杀时间
        /// </summary>
        public long TimeDate { get; set; }
        /// <summary>
        /// /职业类型
        /// </summary>
        public int ClassType { get; set; }
        /// <summary>
        /// /战盟职务
        /// </summary>
        public int WarAlliancePost { get; set; }
        public string WarAllianceName { get; set; }
        /// <summary>
        /// 玩家区服ID
        /// </summary>
        public int AreaId { get; set; }
        public override void Dispose()
        {
            GameUserId = 0;
            ListType = 0;
            CharName = null;
            iState = 0;
            iLv = 0;
            TimeDate = 0;
            ClassType = 0;
            WarAlliancePost = 0;
            WarAllianceName = "";
        }
    }

    public enum FirendType
    {
        //黑名单
        BLACKLIST = 1,
        //仇人
        FOELIST = 2,
        //申请列表
        APPLICATION = 3,
        //好友
        FIRENDSLIST = 4,
    }
}
