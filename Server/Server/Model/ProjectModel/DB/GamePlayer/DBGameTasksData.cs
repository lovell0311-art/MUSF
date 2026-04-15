using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBGameTasksData : DBBase
    {
        [DBMongodb(1,true)]
        public long GameUserId { get; set; }
        /// <summary>
        /// 主线任务
        /// </summary>
        public GameTask MainTask { get; set; }

        /// <summary>
        /// 狩猎任务
        /// </summary>
        public GameTask HuntingTask { get; set; }

        /// <summary>
        /// 活动任务
        /// </summary>
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int,GameTask> ActivityTasks { get; set; } = new Dictionary<int, GameTask>();

        /// <summary>
        /// 委托任务
        /// </summary>
        public GameTask EntrustTask { get; set; }

        /// <summary>
        /// 转职任务
        /// </summary>
        public GameTask CareerChangeTask { get; set; }

        /// <summary>
        /// 一次性任务完成列表
        /// </summary>
        public HashSet<int> OneTimeTaskCompletionList { get; set; } = new HashSet<int>();
        /// <summary>
        /// 狩猎任务今日完成次数
        /// </summary>
        public int HuntingTaskToDayCompleteCount { get; set; } = 0;
        /// <summary>
        /// 狩猎任务完成时间 毫秒
        /// </summary>
        public long HuntingTaskComleteTime { get; set; } = 0;

        /// <summary>
        /// 区服id
        /// </summary>
        [DBMongodb(2,true)]
        public int GameAraeId { get; set; }
        /// <summary>
        /// 通行证
        /// </summary>
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, GameTask> PassTasks { get; set; } = new Dictionary<int, GameTask>();
        /// <summary>
        /// 1 代表删除了
        /// </summary>
        public int IsDispose { get; set; } = 0;

    }
}
