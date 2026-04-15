using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Baseic;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{







    /// <summary>
    /// 任务状态
    /// </summary>
    public enum EGameTaskState
    {
        /// <summary>
        /// 无状态/没满足领取条件的任务
        /// </summary>
        None = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        Doing = 1,
        /// <summary>
        /// 已完成
        /// </summary>
        Complete = 2,
        /// <summary>
        /// 已领取
        /// </summary>
        Received = 3,
    }



    // 游戏任务
    public class GameTask
    {
        public static ETHotfix.G2C_UpdateGameTaskNotice G2C_UpdateGameTaskProto = new ETHotfix.G2C_UpdateGameTaskNotice();
        [BsonIgnore]
        public ETHotfix.Struct_TaskInfo Struct_TaskInfoProto = new ETHotfix.Struct_TaskInfo();

        /// <summary>
        /// 任务信息
        /// </summary>
        [BsonIgnore]
        public GameTaskConfig Config;

        /// <summary>
        /// 配置id
        /// </summary>
        public int ConfigId { get; set; } = 0;
        /// <summary>
        /// 任务状态
        /// </summary>
        public EGameTaskState TaskState { get; set; } = EGameTaskState.None;

        /// <summary>
        ///  任务进度
        /// </summary>
        public List<int> TaskProgress { get; set; } = new List<int>();

        /// <summary>
        /// 1.任务刚开始，第一次推送给玩家
        /// </summary>
        public int StartTask = 1;
    }
}
