using CustomFrameWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// 角色
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DBGamePlayerData : DBBase
    {
        [DBMongodb(1)]
        [DBMongodb(11)]
        public long UserId { get; set; }
        [DBMongodb(2)]
        [DBMongodb(11)]
        public int GameAreaId { get; set; }

        public int PlayerTypeId { get; set; }
        public string NickName { get; set; } = "";
        public int Level { get; set; } = 1;
        public long Exp { get; set; }
        /// <summary>
        /// 金币
        /// </summary>
        public long GoldCoin { get; set; }
        /// <summary>
        /// 奇迹币
        /// </summary>
        public int MiracleCoin { get; set; }
        ///// <summary>
        ///// 当前元宝数量
        ///// </summary>
        //public int YuanbaoCoin { get; set; }
        /// <summary>
        /// 系统邮件时间节点，用于记录离线后的系统邮寄
        /// </summary>
        public long NewServerMailTime { get; set; } = 0;
        /// <summary>
        /// 转职次数
        /// </summary>
        public int OccupationLevel { get; set; }

        /// <summary>
        /// 力量
        /// </summary>
        public int Strength { get; set; }
        /// <summary>
        /// 意志 智力
        /// </summary>
        public int Willpower { get; set; }
        /// <summary>
        /// 敏捷
        /// </summary>
        public int Agility { get; set; }
        /// <summary>
        /// 体力
        /// </summary>
        public int BoneGas { get; set; }
        /// <summary>
        /// 统帅
        /// </summary>
        public int Command { get; set; }


        /// <summary>
        /// 力量
        /// </summary>
        public int Strength2 { get; set; }
        /// <summary>
        /// 意志 智力
        /// </summary>
        public int Willpower2 { get; set; }
        /// <summary>
        /// 敏捷
        /// </summary>
        public int Agility2 { get; set; }
        /// <summary>
        /// 体力
        /// </summary>
        public int BoneGas2 { get; set; }
        /// <summary>
        /// 统帅
        /// </summary>
        public int Command2 { get; set; }

        /// <summary>
        /// 自由属性点
        /// </summary>
        public int FreePoint { get; set; }

        /// <summary>
        /// 新存档 1.新存档 0.不是第一次进入游戏了
        /// </summary>
        public int NewArchive { get; set; } = 1;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        public long CreateTimeTick { get; set; }
        /// <summary>
        /// 1 代表删除了
        /// </summary>
        [DBMongodb(11)]
        public int IsDisposePlayer { get; set; } = 0;

        public string DBBuffId { get; set; } = "{}";
        /// <summary>
        /// 转生次数
        /// </summary>
        public int ReincarnateCnt { get; set; } = 0;
        /// <summary>
        /// 新手引导状态
        /// </summary>
        public long BeginnerGuideStatus { get; set; } = 0;
        [BsonIgnore]
        [JsonIgnore]
        public long WarAllianceID { get; set; } = 0;

        [BsonIgnore]
        [JsonIgnore]
        public Dictionary<int, long> DBBufflist { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public string WallTile { get; set; } = "";
        [BsonIgnore]
        [JsonIgnore]
        public int Title { get; set; } = 0;
        /// <summary>
        /// 是否拥有主宰无双称号
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public bool SpecialTitle { get; set; } = false;
        [BsonIgnore]
        [JsonIgnore]
        public long GuZhanChangExittime { get; set; } = 0;
        public void DeSerialize()
        {
            DBBufflist = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, long>>(DBBuffId);
        }
        public void Serialize()
        {
            DBBuffId = Help_JsonSerializeHelper.Serialize(DBBufflist);
        }
    }
}