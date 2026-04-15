using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 任务信息
    /// </summary>
    public class GameTaskConfig
    {
        /// <summary>
        /// 任务配置id
        /// </summary>
        public int ConfigId;

        /// <summary>
        /// 任务类型
        /// </summary>
        public EGameTaskType TaskType { get { return (EGameTaskType)(ConfigId / 100000); } }

        /// <summary>
        /// 任务行为类型
        /// </summary>
        public EGameTaskActionType TaskActionType;

        /// <summary>
        /// 任务目标id
        /// </summary>
        public List<int> TaskTargetId = new List<int>();
        /// <summary>
        /// 任务目标数量
        /// </summary>
        public List<int> TaskTargetCount = new List<int>();

        /// <summary>
        /// 奖励经验
        /// </summary>
        public int RewardExp;
        /// <summary>
        /// 奖励奇迹币
        /// </summary>
        public int RewardCoin;
        /// <summary>
        /// 奖励奇迹币
        /// </summary>
        public int RewardUCoin;
        /// <summary>
        /// 自定义奖励
        /// </summary>
        public string CustomReward;
        /// <summary>
        /// 自动领取奖励
        /// </summary>
        public bool AutoReceiveReward = false;


        /// <summary>
        /// 前置任务ID
        /// </summary>
        public List<int> TaskBeforeId { get; set; } = new List<int>();

        /// <summary>
        /// 一次性任务，不可重复做
        /// </summary>
        public bool OneTimeTask { get; set; }

        /// <summary>
        /// 需要金币
        /// </summary>
        public int ReqCoin { get; set; }
        /// <summary>
        /// 需要最小等级
        /// </summary>
        public int ReqLevelMin { get; set; }
        /// <summary>
        /// 最大等级
        /// </summary>
        public int ReqLevelMax { get; set; }
        /// <summary>
        /// 法师
        /// </summary>
        public int Spell { get; set; }
        /// <summary>
        /// 剑士
        /// </summary>
        public int Swordsman { get; set; }
        /// <summary>
        /// 弓箭手
        /// </summary>
        public int Archer { get; set; }
        /// <summary>
        /// 魔剑士
        /// </summary>
        public int Spellsword { get; set; }
        /// <summary>
        /// 圣导师
        /// </summary>
        public int Holyteacher { get; set; }
        /// <summary>
        /// 召唤术士
        /// </summary>
        public int SummonWarlock { get; set; }
        /// <summary>
        /// 格斗
        /// </summary>
        public int Combat { get; set; }
        /// <summary>
        /// 梦幻骑士
        /// </summary>
        public int GrowLancer { get; set; }

    }
}
