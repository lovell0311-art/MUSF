using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 任务信息类
    /// </summary>
    public class TaskInfo
    {

        public long Id { get; set; }
        ///<summary>名称 </summary>
        public string TaskName;
       
        ///<summary>任务行为类型 </summary>
        public int TaskActionType;
        ///<summary>任务介绍 </summary>
        public string TaskDes;
        ///<summary>地图 </summary>
        public int MapId;
        ///<summary>自动寻路坐标 </summary>
        public List<int> AutoPathPos;
        ///<summary>自动寻路坐标_X </summary>
        public int Pos_X;
        ///<summary>自动寻路坐标_Y </summary>
        public int Pos_Y;
        /// <summary>
        /// 是否自动攻击
        /// </summary>
        public int IsAutoHitMonster;
        ///<summary>怪物名称 </summary>
        public string MonsterName;
        ///<summary>任务目标id </summary>
        public List<int> TaskTargetId;
        ///<summary>已经击杀的怪物数量 </summary>
        public int KillMonsterCount => TaskProgress.Count != 0 ? TaskProgress[0]:0;
        ///<summary>任务目标数量 </summary>
        public List<int> TaskTargetCount;
        public int TaskTargetCounts => TaskTargetCount.Count != 0 ? TaskTargetCount[0] : 0;

        ///<summary>下一个任务ID </summary>
        public long NextTaskId; 

        /// <summary>
        /// 任务状态
        /// </summary>
        public string States() => State switch
        {
            0 => string.Empty,
            1 => $"进行中",
            2 => "<color=green>已完成</color>",
            3 => "<color=green>已领取</color>",
            _ => string.Empty,
        };
        /// <summary>
        /// 是否已经领取奖励
        /// </summary>
        public bool IsReceiveRewards = false;
        /// <summary>
        /// 任务类型
        /// </summary>
        public E_TaskType TaskType => (E_TaskType)(Id / 100000);
        /// <summary>
        /// 是否是一次性任务 1只能领取一次
        /// </summary>
        public int OneTimeTask=-1;
        /// <summary>
        /// 任务状态 
        /// 0 无状态 还没领取的任务，当玩家满足条件时，自动领取
        /// 1 进行中
        /// 2 已完成
        /// 3 已经领取
        /// </summary>
        public int State=0;

        /// <summary>
        /// 任务进度（已经杀了多少只怪物）
        /// </summary>
        public List<int> TaskProgress;

        ///<summary>需要奇迹币</summary>
        public int ReqCoin;

        ///<summary>奖励经验 </summary>
        public int RewardExp;
        ///<summary>奖励奇迹币 </summary>
        public int RewardCoin;
        ///<summary>奖励物品Id </summary>
        public List<int> RewardItems;
        ///<summary>自定义奖励 </summary>
        public string CustomReward;
        ///<summary>前置任务ID </summary>
        public List<int> TaskBeforeId;

        ///<summary>需要最小等级 </summary>
        public int ReqLevelMin;
        ///<summary>最大等级 </summary>
        public int ReqLevelMax;
        ///<summary>法师 </summary>
        public int Spell;
        ///<summary>剑士 </summary>
        public int Swordsman;
        ///<summary>弓箭手 </summary>
        public int Archer;
        ///<summary>魔剑士 </summary>
        public int Spellsword;
        ///<summary>圣导师 </summary>
        public int Holyteacher;
        ///<summary>召唤术士 </summary>
        public int SummonWarlock;
        ///<summary>格斗 </summary>
        public int Combat;
        ///<summary>梦幻骑士 </summary>
        public int GrowLancer;

        public override bool Equals(object obj)
        {
            return obj is TaskInfo info && info.Id == this.Id;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ActiveMapInfo
    {
        public int Id;//id
        public string MapName;//副本名字
        public int IntoCount;//每天可进入次数
    }
}
