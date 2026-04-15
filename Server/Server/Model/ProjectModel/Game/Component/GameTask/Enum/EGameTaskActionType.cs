using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public enum EGameTaskActionType
    {
        /// <summary>
        /// 击杀怪物  [1,...] [1,...]
        /// </summary>
        KillMonster = 1,
        /// <summary>
        /// 提升等级    [0] [1]
        /// </summary>
        LevelUp = 2,
        /// <summary>
        /// 进入传送点   [1] [1]
        /// </summary>
        EnterTransferPoint = 3,
        /// <summary>
        /// 收集物品    [1,...] [1,...]
        /// </summary>
        CollectItem = 4,
        /// <summary>
        /// 组队击杀怪物   [1,...] [1,...]
        /// </summary>
        TeamKillMonster = 5,
        /// <summary>
        /// 自动完成    [0] [1]
        /// </summary>
        AutoComplete = 6,
        /// <summary>
        /// 击杀指定地图中的怪物    [0] [1]
        /// </summary>
        KillMonsterInMap = 7,
        /// <summary>
        /// 完成1次血色城堡
        /// </summary>
        CompleteRedCopy = 8,
        /// <summary>
        /// 完成1次恶魔广场
        /// </summary>
        CompleteDemonCopy = 9,
        /// <summary>
        /// 破坏血色城堡城门
        /// </summary>
        BreakThroughTheGate = 10,
        /// <summary>
        /// 转职完成   [0] [1-3]
        /// </summary>
        CareerChangeCompleted = 11,
        /// <summary>
        /// 在恶魔广场获取点数
        /// </summary>
        DemonPlazaGainsPoints = 12,
        /// <summary>
        /// 收集并提交物品 [1,...] [1,...]
        /// </summary>
        CollectAndSubmitItem = 14,
        /// <summary>
        /// 有翅膀(判断背包，装备栏有没有翅膀或披风) [0] [1]
        /// </summary>
        HaveWing = 15,
        /// <summary>
        /// 丢弃物品    [0] [1]
        /// </summary>
        DiscardItem = 100,
        /// <summary>
        /// 属性加点
        /// </summary>
        ConfigureAttributePoint = 101,
        /// <summary>
        /// 穿戴装备
        /// </summary>
        WearEquip = 102,
        /// <summary>
        /// 设置技能
        /// </summary>
        SetSkill = 103,
        /// <summary>
        /// 使用传送
        /// </summary>
        UseTransfer = 104,
        /// <summary>
        /// 领取狩猎任务
        /// </summary>
        ReceiveHuntingTask = 105,
        /// <summary>
        /// 领取新手buff
        /// </summary>
        ReceiveXinShouBuff = 106,
        /// <summary>
        /// 装备相关任务
        /// </summary>
        EquipmentRelatedSettings = 300,
    }
}
