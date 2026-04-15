using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod(typeof(GameTaskConfigManager), EventSystemType.INIT)]
    public class GameTaskConfigManagerInitSystem : ITEventMethodOnInit<GameTaskConfigManager>
    {
        public void OnInit(GameTaskConfigManager self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(GameTaskConfigManager), EventSystemType.LOAD)]
    public class GameTaskConfigManagerLoadSystem : ITEventMethodOnLoad<GameTaskConfigManager>
    {
        public override void OnLoad(GameTaskConfigManager self)
        {
            self.OnInit();
        }
    }



    public static class GameTaskConfigManagerSystem
    {

        public static void OnInit(this GameTaskConfigManager self)
        {
            self.BeforeTaskConfigDict.Clear();
            self.AllConfigDict.Clear();
            self.RewardItemsDict.Clear();
            self.DropItemConfigDict.Clear();
            // TODO 初始化前置任务关系
            var readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            // 主线任务
            foreach(var conf in readConfig.GetJson<GameTask_MainConfigJson>().JsonDic.Values)
            {
                foreach(var id in conf.TaskBeforeId)
                {
                    self.BeforeTaskConfigDict.Add(id, conf.Id);
                }
            }
            foreach (var conf in readConfig.GetJson<PassportTask_PassportConfigJson>().JsonDic.Values)
            {
                foreach (var id in conf.TaskBeforeId)
                {
                    self.BeforeTaskConfigDict.Add(id, conf.Id);
                }
            }
            // 活动任务
            foreach (var conf in readConfig.GetJson<GameTask_ActivityConfigJson>().JsonDic.Values)
            {
                foreach (var id in conf.TaskBeforeId)
                {
                    // 只存开始的任务
                    self.BeforeTaskConfigDict.Add(id, conf.Id);
                }
            }

            // TODO 初始化奖励物品
            foreach (var conf in readConfig.GetJson<GameTask_RewardItemConfigJson>().JsonDic.Values)
            {
                self.RewardItemsDict.Add(conf.TaskId, conf.Id);
            }

            // TODO 初始化任务物品掉落
            foreach (var conf in readConfig.GetJson<GameTask_DropItemConfigJson>().JsonDic.Values)
            {
                MultiMap<int, GameTask_DropItemConfig> dropItem;
                if(!self.DropItemConfigDict.TryGetValue(conf.MonsterId, out dropItem))
                {
                    dropItem = new MultiMap<int, GameTask_DropItemConfig>();
                    self.DropItemConfigDict.Add(conf.MonsterId, dropItem);
                }
                dropItem.Add(conf.TaskId, conf);
            }
            // 排序 掉率大的放前面
            foreach(var v in self.DropItemConfigDict.Values)
            {
                foreach(var dropList in v.GetDictionary().Values)
                {
                    dropList.Sort((a, b) =>
                    {
                        if (a.DropRate < b.DropRate) return 1;
                        else if (a.DropRate > b.DropRate) return -1;
                        else return 0;
                    });
                }
            }

        }

        /// <summary>
        /// 通过前置任务id,获取后续的任务id
        /// </summary>
        /// <param name="beforeConfigId">前置任务id</param>
        /// <returns></returns>
        public static List<int> GetAfterTaskIdListByBeforeId(this GameTaskConfigManager self,int beforeConfigId)
        {
            return self.BeforeTaskConfigDict[beforeConfigId];
        }


        /// <summary>
        /// 尝试获取任务配置
        /// </summary>
        /// <param name="self"></param>
        /// <param name="configId"></param>
        /// <param name="conf"></param>
        /// <returns></returns>
        public static bool TryGetConfig(this GameTaskConfigManager self,int configId,out GameTaskConfig conf)
        {
            if(self.AllConfigDict.TryGetValue(configId,out conf))
            {
                return true;
            }
            conf = null;
            var readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            switch((EGameTaskType)(configId / 100000))
            {
                case EGameTaskType.Main:
                    {
                        if(readConfig.GetJson<GameTask_MainConfigJson>().JsonDic.TryGetValue(configId, out var conf2))
                        {
                            conf = conf2.ToGameTaskConfig();
                        }
                        break;
                    }
                case EGameTaskType.Hunting:
                    {
                        if(readConfig.GetJson<GameTask_HuntingConfigJson>().JsonDic.TryGetValue(configId, out var conf2))
                        {
                            conf = conf2.ToGameTaskConfig();
                        }
                        break;
                    }
                case EGameTaskType.Activity:
                    {
                        if(readConfig.GetJson<GameTask_ActivityConfigJson>().JsonDic.TryGetValue(configId, out var conf2))
                        {
                            conf = conf2.ToGameTaskConfig();
                        }
                        break;
                    }
                case EGameTaskType.Entrust:
                    {
                        if(readConfig.GetJson<GameTask_EntrustConfigJson>().JsonDic.TryGetValue(configId, out var conf2))
                        {
                            conf = conf2.ToGameTaskConfig();
                        }
                        break;
                    }
                case EGameTaskType.CareerChange:
                    {
                        if(readConfig.GetJson<GameTask_CareerChangeConfigJson>().JsonDic.TryGetValue(configId, out var conf2))
                        {
                            conf = conf2.ToGameTaskConfig();
                        }
                        break;
                    }
                case EGameTaskType.PassMission:
                    {
                        if (readConfig.GetJson<PassportTask_PassportConfigJson>().JsonDic.TryGetValue(configId, out var conf2))
                        {
                            conf = conf2.ToGameTaskConfig();
                        }
                        break;
                    }
                    // 其他任务配置
            }
            if(conf != null)
            {
                self.AllConfigDict.Add(conf.ConfigId, conf);
                return true;
            }
            return false;
        }


        /// <summary>
        /// 通过任务id,获取奖励物品id列表
        /// </summary>
        /// <param name="self"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static List<int> GetRewardItemIdListByTaskId(this GameTaskConfigManager self, int taskId)
        {
            return self.RewardItemsDict[taskId];
        }



    }
}
