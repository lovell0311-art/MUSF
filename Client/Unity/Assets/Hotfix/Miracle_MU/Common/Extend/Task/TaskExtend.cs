using ILRuntime.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public static class TaskExtend
    {
        public static IConfig GetTaskConfig(this long self) => (self / 100000) switch
        {
          
            1=>ConfigComponent.Instance.GetItem<GameTask_MainConfig>(self.ToInt32()),//主线任务
            2=>ConfigComponent.Instance.GetItem<GameTask_HuntingConfig>(self.ToInt32()),//狩猎任务
            3=>ConfigComponent.Instance.GetItem<GameTask_ActivityConfig>(self.ToInt32()),//活动任务
            4=>ConfigComponent.Instance.GetItem<GameTask_EntrustConfig>(self.ToInt32()),//委托任务
            5=>ConfigComponent.Instance.GetItem<GameTask_CareerChangeConfig>(self.ToInt32()),//转职任务
            6=>ConfigComponent.Instance.GetItem<ActiveMap_ActivityConfig>(self.ToInt32()),//转职任务
            7 => ConfigComponent.Instance.GetItem<PassportTask_PassportConfig>(self.ToInt32()),//通行证
            _ =>null
        };

        public static void GetTaskInfo_Ref(this long self, ref TaskInfo taskInfo)
        {
            
            IConfig config = self.GetTaskConfig();
            if (config is GameTask_MainConfig mainTask)
            {
                taskInfo.Id = mainTask.Id;
                taskInfo.TaskName = mainTask.TaskName;
                taskInfo.TaskActionType = mainTask.TaskActionType;
                taskInfo.TaskDes = mainTask.TaskDes;
                taskInfo.MapId = mainTask.MapId;
                taskInfo.AutoPathPos = mainTask.AutoPathPos;
                taskInfo.Pos_X = mainTask.AutoPathPos.Count != 0 ? mainTask.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = mainTask.AutoPathPos.Count != 0 ? mainTask.AutoPathPos[1] : 0;
                taskInfo.MonsterName = mainTask.MonsterName;
                taskInfo.TaskTargetId = mainTask.TaskTargetId;
                taskInfo.TaskTargetCount = mainTask.TaskTargetCount;
                taskInfo.RewardExp = mainTask.RewardExp;
                taskInfo.RewardCoin = mainTask.RewardCoin;
                taskInfo.CustomReward = mainTask.CustomReward;
                taskInfo.TaskBeforeId = mainTask.TaskBeforeId;
                taskInfo.ReqLevelMin = mainTask.ReqLevelMin;
                taskInfo.ReqLevelMax = mainTask.ReqLevelMax;
                taskInfo.Spell = mainTask.Spell;
                taskInfo.Swordsman = mainTask.Swordsman;
                taskInfo.Archer = mainTask.Archer;
                taskInfo.Spellsword = mainTask.Spellsword;
                taskInfo.Holyteacher = mainTask.Holyteacher;
                taskInfo.SummonWarlock = mainTask.SummonWarlock;
                taskInfo.Combat = mainTask.Combat;
                taskInfo.GrowLancer = mainTask.GrowLancer;
               

            }
            else if (config is GameTask_ActivityConfig activityTask)
            {
                taskInfo.Id = activityTask.Id;
                taskInfo.TaskName = activityTask.TaskName;
                taskInfo.TaskActionType = activityTask.TaskActionType;
                taskInfo.TaskDes = activityTask.TaskDes;
                taskInfo.MapId = activityTask.MapId;
                taskInfo.AutoPathPos = activityTask.AutoPathPos;
                taskInfo.Pos_X = activityTask.AutoPathPos.Count != 0 ? activityTask.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = activityTask.AutoPathPos.Count != 0 ? activityTask.AutoPathPos[1] : 0;
                taskInfo.MonsterName = activityTask.MonsterName;
                taskInfo.TaskTargetId = activityTask.TaskTargetId;
                taskInfo.TaskTargetCount = activityTask.TaskTargetCount;
                taskInfo.RewardExp = activityTask.RewardExp;
                taskInfo.RewardCoin = activityTask.RewardCoin;
                taskInfo.TaskBeforeId = activityTask.TaskBeforeId;
                taskInfo.CustomReward = activityTask.CustomReward;
              
                taskInfo.ReqLevelMin = activityTask.ReqLevelMin;
                taskInfo.ReqLevelMax = activityTask.ReqLevelMax;
                taskInfo.Spell = activityTask.Spell;
                taskInfo.Swordsman = activityTask.Swordsman;
                taskInfo.Archer = activityTask.Archer;
                taskInfo.Spellsword = activityTask.Spellsword;
                taskInfo.Holyteacher = activityTask.Holyteacher;
                taskInfo.SummonWarlock = activityTask.SummonWarlock;
                taskInfo.Combat = activityTask.Combat;
                taskInfo.GrowLancer = activityTask.GrowLancer;
            }
            else if (config is GameTask_HuntingConfig huntTask)
            {
                taskInfo.Id = huntTask.Id;
                taskInfo.TaskName = huntTask.TaskName;
                taskInfo.TaskActionType = huntTask.TaskActionType;
                taskInfo.TaskDes = huntTask.TaskDes;
                taskInfo.MapId = huntTask.MapId;
                taskInfo.AutoPathPos = huntTask.AutoPathPos;
                taskInfo.Pos_X = huntTask.AutoPathPos.Count != 0 ? huntTask.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = huntTask.AutoPathPos.Count != 0 ? huntTask.AutoPathPos[1] : 0;
                taskInfo.MonsterName = huntTask.MonsterName;
                taskInfo.TaskTargetId = huntTask.TaskTargetId;
                taskInfo.TaskTargetCount = huntTask.TaskTargetCount;
                taskInfo.RewardExp = huntTask.RewardExp;
                taskInfo.RewardCoin = huntTask.RewardCoin;
                taskInfo.OneTimeTask = huntTask.OneTimeTask;
                taskInfo.CustomReward = huntTask.CustomReward;
              
                taskInfo.ReqLevelMin = huntTask.ReqLevelMin;
                taskInfo.ReqLevelMax = huntTask.ReqLevelMax;
                taskInfo.Spell = huntTask.Spell;
                taskInfo.Swordsman = huntTask.Swordsman;
                taskInfo.Archer = huntTask.Archer;
                taskInfo.Spellsword = huntTask.Spellsword;
                taskInfo.Holyteacher = huntTask.Holyteacher;
                taskInfo.SummonWarlock = huntTask.SummonWarlock;
                taskInfo.Combat = huntTask.Combat;
                taskInfo.GrowLancer = huntTask.GrowLancer;
               
            }
            else if (config is GameTask_EntrustConfig entrustTask)
            {
                taskInfo.Id = entrustTask.Id;
                taskInfo.TaskName = entrustTask.TaskName;
                taskInfo.TaskActionType = entrustTask.TaskActionType;
                taskInfo.TaskDes = entrustTask.TaskDes;
                taskInfo.MapId = entrustTask.MapId;
                taskInfo.AutoPathPos = entrustTask.AutoPathPos;
                taskInfo.Pos_X = entrustTask.AutoPathPos.Count != 0 ? entrustTask.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = entrustTask.AutoPathPos.Count != 0 ? entrustTask.AutoPathPos[1] : 0;
                taskInfo.MonsterName = entrustTask.MonsterName;
                taskInfo.TaskTargetId = entrustTask.TaskTargetId;
                taskInfo.TaskTargetCount = entrustTask.TaskTargetCount;
                taskInfo.RewardExp = entrustTask.RewardExp;
                taskInfo.RewardCoin = entrustTask.RewardCoin;
                taskInfo.CustomReward = entrustTask.CustomReward;
                taskInfo.TaskBeforeId = entrustTask.TaskBeforeId;
                taskInfo.ReqLevelMin = entrustTask.ReqLevelMin;
                taskInfo.ReqLevelMax = entrustTask.ReqLevelMax;
                taskInfo.Spell = entrustTask.Spell;
                taskInfo.Swordsman = entrustTask.Swordsman;
                taskInfo.Archer = entrustTask.Archer;
                taskInfo.Spellsword = entrustTask.Spellsword;
                taskInfo.Holyteacher = entrustTask.Holyteacher;
                taskInfo.SummonWarlock = entrustTask.SummonWarlock;
                taskInfo.Combat = entrustTask.Combat;
                taskInfo.GrowLancer = entrustTask.GrowLancer;
             
            }
            else if (config is GameTask_CareerChangeConfig careerTask)
            {
                taskInfo.Id = careerTask.Id;
                taskInfo.TaskName = careerTask.TaskName;
                taskInfo.TaskActionType = careerTask.TaskActionType;
                taskInfo.TaskDes = careerTask.TaskDes;
                taskInfo.MapId = careerTask.MapId;
                taskInfo.AutoPathPos = careerTask.AutoPathPos;
                taskInfo.Pos_X = careerTask.AutoPathPos.Count != 0 ? careerTask.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = careerTask.AutoPathPos.Count != 0 ? careerTask.AutoPathPos[1] : 0;
                taskInfo.MonsterName = careerTask.MonsterName;
                taskInfo.TaskTargetId = careerTask.TaskTargetId;
                taskInfo.TaskTargetCount = careerTask.TaskTargetCount;
                taskInfo.RewardExp = careerTask.RewardExp;
                taskInfo.RewardCoin = careerTask.RewardCoin;
                taskInfo.CustomReward = careerTask.CustomReward;
                taskInfo.ReqCoin = careerTask.ReqCoin;
                taskInfo.TaskBeforeId = careerTask.TaskBeforeId;
                taskInfo.ReqLevelMin = careerTask.ReqLevelMin;
                taskInfo.ReqLevelMax = careerTask.ReqLevelMax;
                taskInfo.Spell = careerTask.Spell;
                taskInfo.Swordsman = careerTask.Swordsman;
                taskInfo.Archer = careerTask.Archer;
                taskInfo.Spellsword = careerTask.Spellsword;
                taskInfo.Holyteacher = careerTask.Holyteacher;
                taskInfo.SummonWarlock = careerTask.SummonWarlock;
                taskInfo.Combat = careerTask.Combat;
                taskInfo.GrowLancer = careerTask.GrowLancer;
              

            }
        }
        public static void GetTaskInfo_Out(this long self, out TaskInfo taskInfo)
        {

            IConfig config = self.GetTaskConfig();
           // Log.DebugWhtie($"{self} {self / 100000} 是否是活动任务{config is GameTask_ActivityConfig}");
            taskInfo = new TaskInfo
            {
               // IsComplete = false,
            };
            if (config is GameTask_MainConfig mainTask)
            {
                taskInfo.Id = mainTask.Id;
                taskInfo.TaskName = mainTask.TaskName;
                taskInfo.TaskActionType = mainTask.TaskActionType;
                taskInfo.TaskDes = mainTask.TaskDes;
                taskInfo.MapId = mainTask.MapId;
                taskInfo.AutoPathPos = mainTask.AutoPathPos;
                taskInfo.Pos_X = mainTask.AutoPathPos.Count != 0 ? mainTask.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = mainTask.AutoPathPos.Count != 0 ? mainTask.AutoPathPos[1] : 0;
                taskInfo.MonsterName = mainTask.MonsterName;
                taskInfo.TaskTargetId = mainTask.TaskTargetId;
                taskInfo.TaskTargetCount = mainTask.TaskTargetCount;
                taskInfo.RewardExp = mainTask.RewardExp;
                taskInfo.RewardCoin = mainTask.RewardCoin;
                taskInfo.CustomReward = mainTask.CustomReward;
                taskInfo.TaskBeforeId = mainTask.TaskBeforeId;
                taskInfo.ReqLevelMin = mainTask.ReqLevelMin;
                taskInfo.ReqLevelMax = mainTask.ReqLevelMax;
                taskInfo.Spell = mainTask.Spell;
                taskInfo.Swordsman = mainTask.Swordsman;
                taskInfo.Archer = mainTask.Archer;
                taskInfo.Spellsword = mainTask.Spellsword;
                taskInfo.Holyteacher = mainTask.Holyteacher;
                taskInfo.SummonWarlock = mainTask.SummonWarlock;
                taskInfo.Combat = mainTask.Combat;
                taskInfo.GrowLancer = mainTask.GrowLancer;
                taskInfo.IsAutoHitMonster = mainTask.IsAutoHitMonster;

            }
            else if (config is GameTask_ActivityConfig activityTask)
            {
                taskInfo.Id = activityTask.Id;
                taskInfo.TaskName = activityTask.TaskName;
                taskInfo.TaskActionType = activityTask.TaskActionType;
                taskInfo.TaskDes = activityTask.TaskDes;
                taskInfo.MapId = activityTask.MapId;
                taskInfo.AutoPathPos = activityTask.AutoPathPos;
                taskInfo.Pos_X = activityTask.AutoPathPos.Count != 0 ? activityTask.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = activityTask.AutoPathPos.Count != 0 ? activityTask.AutoPathPos[1] : 0;
                taskInfo.MonsterName = activityTask.MonsterName;
                taskInfo.TaskTargetId = activityTask.TaskTargetId;
                taskInfo.TaskTargetCount = activityTask.TaskTargetCount;
                taskInfo.RewardExp = activityTask.RewardExp;
                taskInfo.RewardCoin = activityTask.RewardCoin;
                taskInfo.CustomReward = activityTask.CustomReward;
                taskInfo.TaskBeforeId = activityTask.TaskBeforeId;
                taskInfo.ReqLevelMin = activityTask.ReqLevelMin;
                taskInfo.ReqLevelMax = activityTask.ReqLevelMax;
                taskInfo.Spell = activityTask.Spell;
                taskInfo.Swordsman = activityTask.Swordsman;
                taskInfo.Archer = activityTask.Archer;
                taskInfo.Spellsword = activityTask.Spellsword;
                taskInfo.Holyteacher = activityTask.Holyteacher;
                taskInfo.SummonWarlock = activityTask.SummonWarlock;
                taskInfo.Combat = activityTask.Combat;
                taskInfo.GrowLancer = activityTask.GrowLancer;
                taskInfo.IsAutoHitMonster = activityTask.IsAutoHitMonster;
            }
            else if (config is GameTask_HuntingConfig huntTask)
            {
                taskInfo.Id = huntTask.Id;
                taskInfo.TaskName = huntTask.TaskName;
                taskInfo.TaskActionType = huntTask.TaskActionType;
                taskInfo.TaskDes = huntTask.TaskDes;
                taskInfo.MapId = huntTask.MapId;
                taskInfo.AutoPathPos = huntTask.AutoPathPos;
                taskInfo.Pos_X = huntTask.AutoPathPos.Count != 0 ? huntTask.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = huntTask.AutoPathPos.Count != 0 ? huntTask.AutoPathPos[1] : 0;
                taskInfo.MonsterName = huntTask.MonsterName;
                taskInfo.TaskTargetId = huntTask.TaskTargetId;
                taskInfo.TaskTargetCount = huntTask.TaskTargetCount;
                taskInfo.RewardExp = huntTask.RewardExp;
                taskInfo.RewardCoin = huntTask.RewardCoin;
                taskInfo.CustomReward = huntTask.CustomReward;
              
                taskInfo.ReqLevelMin = huntTask.ReqLevelMin;
                taskInfo.ReqLevelMax = huntTask.ReqLevelMax;
                taskInfo.Spell = huntTask.Spell;
                taskInfo.Swordsman = huntTask.Swordsman;
                taskInfo.Archer = huntTask.Archer;
                taskInfo.Spellsword = huntTask.Spellsword;
                taskInfo.Holyteacher = huntTask.Holyteacher;
                taskInfo.SummonWarlock = huntTask.SummonWarlock;
                taskInfo.Combat = huntTask.Combat;
                taskInfo.GrowLancer = huntTask.GrowLancer;
                taskInfo.IsAutoHitMonster = huntTask.IsAutoHitMonster;
            }
            else if (config is GameTask_EntrustConfig entrustTask)
            {
                taskInfo.Id = entrustTask.Id;
                taskInfo.TaskName = entrustTask.TaskName;
                taskInfo.TaskActionType = entrustTask.TaskActionType;
                taskInfo.TaskDes = entrustTask.TaskDes;
                taskInfo.MapId = entrustTask.MapId;
                taskInfo.AutoPathPos = entrustTask.AutoPathPos;
                taskInfo.Pos_X = entrustTask.AutoPathPos.Count != 0 ? entrustTask.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = entrustTask.AutoPathPos.Count != 0 ? entrustTask.AutoPathPos[1] : 0;
                taskInfo.MonsterName = entrustTask.MonsterName;
                taskInfo.TaskTargetId = entrustTask.TaskTargetId;
                taskInfo.TaskTargetCount = entrustTask.TaskTargetCount;
                taskInfo.RewardExp = entrustTask.RewardExp;
                taskInfo.RewardCoin = entrustTask.RewardCoin;
                taskInfo.CustomReward = entrustTask.CustomReward;
                taskInfo.TaskBeforeId = entrustTask.TaskBeforeId;
                taskInfo.ReqLevelMin = entrustTask.ReqLevelMin;
                taskInfo.ReqLevelMax = entrustTask.ReqLevelMax;
                taskInfo.Spell = entrustTask.Spell;
                taskInfo.Swordsman = entrustTask.Swordsman;
                taskInfo.Archer = entrustTask.Archer;
                taskInfo.Spellsword = entrustTask.Spellsword;
                taskInfo.Holyteacher = entrustTask.Holyteacher;
                taskInfo.SummonWarlock = entrustTask.SummonWarlock;
                taskInfo.Combat = entrustTask.Combat;
                taskInfo.GrowLancer = entrustTask.GrowLancer;
                taskInfo.IsAutoHitMonster = entrustTask.IsAutoHitMonster;

            }
            else if (config is GameTask_CareerChangeConfig careerTask)
            {
                taskInfo.Id = careerTask.Id;
                taskInfo.TaskName = careerTask.TaskName;
                taskInfo.TaskActionType = careerTask.TaskActionType;
                taskInfo.TaskDes = careerTask.TaskDes;
                taskInfo.MapId = careerTask.MapId;
                taskInfo.AutoPathPos = careerTask.AutoPathPos;
                taskInfo.Pos_X = careerTask.AutoPathPos.Count != 0 ? careerTask.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = careerTask.AutoPathPos.Count != 0 ? careerTask.AutoPathPos[1] : 0;
                taskInfo.MonsterName = careerTask.MonsterName;
                taskInfo.TaskTargetId = careerTask.TaskTargetId;
                taskInfo.TaskTargetCount = careerTask.TaskTargetCount;
                taskInfo.RewardExp = careerTask.RewardExp;
                taskInfo.RewardCoin = careerTask.RewardCoin;
                taskInfo.CustomReward = careerTask.CustomReward;
                taskInfo.TaskBeforeId = careerTask.TaskBeforeId;
                taskInfo.ReqLevelMin = careerTask.ReqLevelMin;
                taskInfo.ReqLevelMax = careerTask.ReqLevelMax;
                taskInfo.Spell = careerTask.Spell;
                taskInfo.Swordsman = careerTask.Swordsman;
                taskInfo.Archer = careerTask.Archer;
                taskInfo.Spellsword = careerTask.Spellsword;
                taskInfo.Holyteacher = careerTask.Holyteacher;
                taskInfo.SummonWarlock = careerTask.SummonWarlock;
                taskInfo.Combat = careerTask.Combat;
                taskInfo.GrowLancer = careerTask.GrowLancer;
                taskInfo.ReqCoin = careerTask.ReqCoin;
                taskInfo.IsAutoHitMonster = careerTask.IsAutoHitMonster;

            }
            else if (config is PassportTask_PassportConfig passportTaskConfig)
            {
                taskInfo.Id = passportTaskConfig.Id;
                taskInfo.TaskName = passportTaskConfig.TaskName;
                taskInfo.TaskActionType = passportTaskConfig.TaskActionType;
                taskInfo.TaskDes = passportTaskConfig.TaskDes;
                taskInfo.MapId = passportTaskConfig.MapId;
                taskInfo.AutoPathPos = passportTaskConfig.AutoPathPos;
                taskInfo.Pos_X = passportTaskConfig.AutoPathPos.Count != 0 ? passportTaskConfig.AutoPathPos[0] : 0;
                taskInfo.Pos_Y = passportTaskConfig.AutoPathPos.Count != 0 ? passportTaskConfig.AutoPathPos[1] : 0;
                taskInfo.MonsterName = passportTaskConfig.MonsterName;
                taskInfo.TaskTargetId = passportTaskConfig.TaskTargetId;
                taskInfo.TaskTargetCount = passportTaskConfig.TaskTargetCount;
                taskInfo.RewardExp = passportTaskConfig.RewardExp;
                taskInfo.RewardCoin = passportTaskConfig.RewardCoin;
                //taskInfo.CustomReward = passportTaskConfig.CustomReward;
                taskInfo.TaskBeforeId = passportTaskConfig.TaskBeforeId;
                taskInfo.ReqLevelMin = passportTaskConfig.ReqLevelMin;
                taskInfo.ReqLevelMax = passportTaskConfig.ReqLevelMax;
                taskInfo.Spell = passportTaskConfig.Spell;
                taskInfo.Swordsman = passportTaskConfig.Swordsman;
                taskInfo.Archer = passportTaskConfig.Archer;
                taskInfo.Spellsword = passportTaskConfig.Spellsword;
                taskInfo.Holyteacher = passportTaskConfig.Holyteacher;
                taskInfo.SummonWarlock = passportTaskConfig.SummonWarlock;
                taskInfo.Combat = passportTaskConfig.Combat;
                taskInfo.GrowLancer = passportTaskConfig.GrowLancer;
                taskInfo.IsAutoHitMonster = passportTaskConfig.IsAutoHitMonster;
            }
        }

        public static void GetTaskInfo_Out(this long self, out ActiveMapInfo activeMapInfo)
        {
            IConfig config = self.GetTaskConfig();
            activeMapInfo = new ActiveMapInfo
            {
                // IsComplete = false,
            };
            if (config is ActiveMapInfo mainTask)
            {
                activeMapInfo.Id = mainTask.Id;
                activeMapInfo.MapName = mainTask.MapName;
                activeMapInfo.IntoCount = mainTask.IntoCount;
            }
        }


    }
}
