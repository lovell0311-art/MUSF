using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Threading.Tasks;

namespace ETHotfix
{
    public static class GameTasksComponentSystem
    {
        public async static Task<bool> Init(this GameTasksComponent self)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = self.Parent.AddCustomComponent<DataCacheManageComponent>();

            var taskDataCache = mDataCacheComponent.Get<DBGameTasksData>();
            if (taskDataCache == null)
            {
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, self.Parent.GameAreaId);
                taskDataCache = await mDataCacheComponent.Add<DBGameTasksData>(dBProxy, p => p.GameUserId == self.Parent.GameUserId
                                                                                && p.GameAraeId == self.Parent.GameAreaId
                                                                                && p.IsDispose == 0);
            }

            var dataList = taskDataCache.DataQuery(p => p.GameUserId == self.Parent.GameUserId
                                    && p.GameAraeId == self.Parent.GameAreaId
                                    && p.IsDispose == 0);

            DBGameTasksData data;

            if (dataList.Count == 0)
            {
                data = new DBGameTasksData();
                data.Id = IdGeneraterNew.Instance.GenerateUnitId(self.Parent.GameAreaId);
                data.GameUserId = self.Parent.GameUserId;
                data.GameAraeId = self.Parent.GameAreaId;
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, self.Parent.GameAreaId);
                bool saveResult = await dBProxy.Save(data);
                if (saveResult == false)
                {
                    Log.Error($"任务组件保存失败。GameUserId={self.Parent.GameUserId}，GameAreaId={self.Parent.GameAreaId}, GameUserId = {self.Parent.GameUserId}");
                    return false;
                }
                taskDataCache.DataAdd(data);
            }
            else if (dataList.Count == 1)
            {
                data = dataList[0];
            }
            else
            {
                Log.Error($"任务组件数据初始化异常！存在2个以上的数据。GameUserId={self.Parent.GameUserId}，GameAreaId={self.Parent.GameAreaId}, GameUserId = {self.Parent.GameUserId}");
                return false;
            }

            if (data == null)
            {
                data = new DBGameTasksData();
                data.Id = self.Parent.GameUserId;
                data.GameAraeId = self.Parent.GameAreaId;
            }
            // 初始化DB中保存的任务
            var configManager = Root.MainFactory.GetCustomComponent<GameTaskConfigManager>();

            #region 各种任务初始化
            // 主线任务
            if (data.MainTask != null)
            {
                if (configManager.TryGetConfig(data.MainTask.ConfigId, out var conf))
                {
                    data.MainTask.Config = conf;
                }
                else
                {
                    self.Parent.PLog($"存档损坏 'DBGameTasksData'，无法找到主线任务id。gameTask.ConfigId = {data.MainTask.ConfigId}, GameUserId = {self.Parent.GameUserId}");
                    data.MainTask = null;
                }
            }

            // 狩猎任务
            if (data.HuntingTask != null)
            {
                if (configManager.TryGetConfig(data.HuntingTask.ConfigId, out var conf))
                {
                    data.HuntingTask.Config = conf;
                }
                else
                {
                    self.Parent.PLog($"存档损坏 'DBGameTasksData'，无法找到狩猎任务id。gameTask.ConfigId = {data.HuntingTask.ConfigId}, GameUserId = {self.Parent.GameUserId}");
                    data.HuntingTask = null;
                }
            }

            // 活动任务
            if (data.ActivityTasks.Count != 0)
            {
                using ListComponent<int> delIds = ListComponent<int>.Create();
                foreach (var gameTask in data.ActivityTasks.Values)
                {
                    if (configManager.TryGetConfig(gameTask.ConfigId, out var conf))
                    {
                        gameTask.Config = conf;
                    }
                    else
                    {
                        delIds.Add(gameTask.ConfigId);
                        self.Parent.PLog($"存档损坏 'DBGameTasksData'，无法找到活动任务id。gameTask.ConfigId = {gameTask.ConfigId}, GameUserId = {self.Parent.GameUserId}");
                    }
                }
                foreach (int delId in delIds) data.ActivityTasks.Remove(delId);
            }

            // 委托任务
            if (data.EntrustTask != null)
            {
                if (configManager.TryGetConfig(data.EntrustTask.ConfigId, out var conf))
                {
                    data.EntrustTask.Config = conf;
                }
                else
                {
                    self.Parent.PLog($"存档损坏 'DBGameTasksData'，无法找到委托任务id。gameTask.ConfigId = {data.EntrustTask.ConfigId}, GameUserId = {self.Parent.GameUserId}");
                    data.EntrustTask = null;
                }
            }

            // 转职任务
            if (data.CareerChangeTask != null)
            {
                if (configManager.TryGetConfig(data.CareerChangeTask.ConfigId, out var conf))
                {
                    data.CareerChangeTask.Config = conf;
                }
                else
                {
                    self.Parent.PLog($"存档损坏 'DBGameTasksData'，无法找到转职任务id。gameTask.ConfigId = {data.CareerChangeTask.ConfigId}, GameUserId = {self.Parent.GameUserId}");
                    data.CareerChangeTask = null;
                }
            }
            // 通行证任务
            if (data.PassTasks == null) data.PassTasks = new Dictionary<int, GameTask>();
            if (data.PassTasks.Count != 0)
            {
                using ListComponent<int> delIds = ListComponent<int>.Create();
                foreach (var gameTask in data.PassTasks.Values)
                {
                    if (configManager.TryGetConfig(gameTask.ConfigId, out var conf))
                    {
                        gameTask.Config = conf;
                    }
                    else
                    {
                        delIds.Add(gameTask.ConfigId);
                        self.Parent.PLog($"存档损坏 'DBGameTasksData'，无法找到活动任务id。gameTask.ConfigId = {gameTask.ConfigId}, GameUserId = {self.Parent.GameUserId}");
                    }
                }
                foreach (int delId in delIds) data.PassTasks.Remove(delId);
            }


            self.data = data;


            // 主线任务初始化
            GameMainTasksHelper.Init(self);
            GameHuntingTasksHelper.Init(self);
            GameActivityTasksHelper.Init(self);
            GameEntrustTasksHelper.Init(self);
            GameCareerChangeTasksHelper.Init(self);
            GamePassTasksHelper.Init(self);
            #endregion

            return true;
        }


        public static void SaveDB(this GameTasksComponent self)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)self.Parent.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(self.Parent.GameAreaId);
            mWriteDataComponent.Save(self.data, dBProxy).Coroutine();
        }

        public static void NotifyAllTask(this GameTasksComponent self)
        {
            var message = new G2C_AllUpdateGameTaskNotice();
            if (self.data.MainTask != null)
            {
                message.MainTasks.Add(self.data.MainTask.ToMessage());
            }
            if (self.data.HuntingTask != null)
            {
                message.HuntingTasks.Add(self.data.HuntingTask.ToMessage());
            }
            foreach (var gameTask in self.data.ActivityTasks.Values)
            {
                message.ActivityTasks.Add(gameTask.ToMessage());
            }
            if (self.data.EntrustTask != null)
            {
                message.EntrustTasks.Add(self.data.EntrustTask.ToMessage());
            }
            if (self.data.CareerChangeTask != null)
            {
                message.CareerChangeTasks.Add(self.data.CareerChangeTask.ToMessage());
            }
            foreach (var gameTask in self.data.PassTasks.Values)
            {
                message.PassTasks.Add(gameTask.ToMessage());
            }
            self.Parent.Send(message);
        }


        public static GameTask GetTask(this GameTasksComponent self, int taskConfigId)
        {
            switch ((EGameTaskType)(taskConfigId / 100000))
            {
                case EGameTaskType.Main:
                    {
                        if (self.data.MainTask != null && self.data.MainTask.ConfigId == taskConfigId) return self.data.MainTask;
                        break;
                    }
                case EGameTaskType.Hunting:
                    {
                        if (self.data.HuntingTask != null && self.data.HuntingTask.ConfigId == taskConfigId) return self.data.HuntingTask;
                        break;
                    }
                case EGameTaskType.Activity:
                    {
                        if (self.data.ActivityTasks.TryGetValue(taskConfigId, out var gameTask)) return gameTask;
                        break;
                    }
                case EGameTaskType.Entrust:
                    {
                        if (self.data.EntrustTask != null && self.data.EntrustTask.ConfigId == taskConfigId) return self.data.EntrustTask;
                        break;
                    }
                case EGameTaskType.CareerChange:
                    {
                        if (self.data.CareerChangeTask != null && self.data.CareerChangeTask.ConfigId == taskConfigId) return self.data.CareerChangeTask;
                        break;
                    }
                case EGameTaskType.PassMission:
                    {
                        if (self.data.PassTasks.TryGetValue(taskConfigId, out var gameTask)) return gameTask;
                        break;
                    }
            }
            return null;
        }

        /// <summary>
        /// 用过滤器获取指定的任务
        /// </summary>
        /// <param name="self"></param>
        /// <param name="tasks"></param>
        /// <param name="filter"></param>
        public static void GetTasksWith(this GameTasksComponent self, ref List<GameTask> tasks, Func<GameTask, bool> filter)
        {
            var gameTask = self.data.MainTask;
            if (gameTask != null && filter(gameTask) == true) tasks.Add(gameTask);
            gameTask = self.data.HuntingTask;
            if (gameTask != null && filter(gameTask) == true) tasks.Add(gameTask);
            foreach (var v in self.data.ActivityTasks.Values)
            {
                if (filter(v) == true) tasks.Add(v);
            }
            gameTask = self.data.EntrustTask;
            if (gameTask != null && filter(gameTask) == true) tasks.Add(gameTask);
            gameTask = self.data.CareerChangeTask;
            if (gameTask != null && filter(gameTask) == true) tasks.Add(gameTask);
            foreach (var v in self.data.PassTasks.Values)
            {
                if (filter(v) == true) tasks.Add(v);
            }
            // 其他任务
            // ...
        }


        /// <summary>
        /// 通过行为类型获取正在进行的任务
        /// </summary>
        /// <param name="self"></param>
        /// <param name="actionType"></param>
        /// <returns>返回原数组</returns>
        public static void GetTasksInProgressByActionType(this GameTasksComponent self, ref List<GameTask> tasks, EGameTaskActionType actionType)
        {
            if (self.data == null)
            {
                // 组件还未初始化完成
                return;
            }
            self.GetTasksWith(ref tasks, p => { return p.Config.TaskActionType == actionType && p.TaskState == EGameTaskState.Doing; });
        }




        /// <summary>
        /// 前置任务已完成
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gamePlayer"></param>
        /// <returns></returns>
        public static bool BeforeTaskComplete(this GameTasksComponent self, int taskId)
        {
            if (taskId == 0) return false;
            var taskConfigManager = Root.MainFactory.GetCustomComponent<GameTaskConfigManager>();
            if (!taskConfigManager.TryGetConfig(taskId, out var afterTaskConfig)) return false;

            var gamePlayer = self.Parent.GetCustomComponent<GamePlayer>();

            foreach (var id in afterTaskConfig.TaskBeforeId)
            {
                if (!taskConfigManager.TryGetConfig(id, out var taskConfig)) continue;
                if (!taskConfig.AllowedOccupation((E_GameOccupation)gamePlayer.Data.PlayerTypeId)) continue;    /* 不是这个职业的任务 */

                var gameTask = self.GetTask(id);
                if (gameTask == null || gameTask.TaskState != EGameTaskState.Received) return false;    /* 还有任务没有完成 */
            }
            return true;
        }

        public static void TriggerTaskAction(this GameTasksComponent self, int taskConfigId, int progressId, int value)
        {
            switch ((EGameTaskType)(taskConfigId / 100000))
            {
                case EGameTaskType.Main:
                    GameMainTasksHelper.TriggerTaskAction(self, taskConfigId, progressId, value);
                    break;
                case EGameTaskType.Hunting:
                    GameHuntingTasksHelper.TriggerTaskAction(self, taskConfigId, progressId, value);
                    break;
                case EGameTaskType.Activity:
                    GameActivityTasksHelper.TriggerTaskAction(self, taskConfigId, progressId, value);
                    break;
                case EGameTaskType.Entrust:
                    GameEntrustTasksHelper.TriggerTaskAction(self, taskConfigId, progressId, value);
                    break;
                case EGameTaskType.CareerChange:
                    GameCareerChangeTasksHelper.TriggerTaskAction(self, taskConfigId, progressId, value);
                    break;
                case EGameTaskType.PassMission:
                    GamePassTasksHelper.TriggerTaskAction(self, taskConfigId, progressId, value);
                    break;
                    // 其他任务
            }
            self.SaveDB();
        }


        /// <summary>
        /// 领取任务奖励
        /// </summary>
        /// <param name="tasksCom"></param>
        /// <param name="taskConfigId"></param>
        public static bool ReceiveTaskReward(this GameTasksComponent self, int taskConfigId, out int err)
        {
            GameTask gameTask = self.GetTask(taskConfigId);
            if (gameTask == null)
            {
                // 任务不存在
                err = 2004;
                return false;
            }
            if (gameTask.TaskState != EGameTaskState.Complete)
            {
                // 无法领取未完成的任务
                err = 2005;
                return false;
            }
            var gamePlayer = self.Parent.GetCustomComponent<GamePlayer>();

            if (gameTask.Config.ReqCoin > gamePlayer.Data.GoldCoin)
            {
                // 金币不足
                err = 1704;
                return false;
            }

            // TODO 先检查任务能不能领取
            using var backpackLockList = ItemsBoxStatus.LockList.Create();
            // 自定义奖励
            // 先检查能不能领取

            IGameTaskRewardHandler customRewardMethod = null;
            if (string.IsNullOrEmpty(gameTask.Config.CustomReward) == false)
            {
                customRewardMethod = Root.MainFactory.GetCustomComponent<GameTaskRewardCreateBuilder>().GetHandlerByMethodName(gameTask.Config.CustomReward);
                if (customRewardMethod.RewardsCanBeGiven(gameTask, self.Parent, backpackLockList, out err) == false)
                {
                    return false;
                }
            }
            // 检查背包能不能放下物品

            var gameTaskConfigManager = Root.MainFactory.GetCustomComponent<GameTaskConfigManager>();
            var readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var itemConfigManager = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>();
            var rewardItemIdList = gameTaskConfigManager.GetRewardItemIdListByTaskId(gameTask.ConfigId);
            var backpack = self.Parent.GetCustomComponent<BackpackComponent>();
            List<(GameTask_RewardItemConfig, ItemConfig, (int, int))> rewardItemConfigList = new List<(GameTask_RewardItemConfig, ItemConfig, (int, int))>();
            if (rewardItemIdList != null && rewardItemIdList.Count != 0)
            {
                var rewardItemConfigJson = readConfig.GetJson<GameTask_RewardItemConfigJson>();

                foreach (var rewardItemId in rewardItemIdList)
                {
                    if (!rewardItemConfigJson.JsonDic.TryGetValue(rewardItemId, out var rewardItemConfig))
                    {
                        Log.Error($"[任务] 没找到 GameTask_RewardItemConfig! configId = {rewardItemId}");
                        // 服务器配置错误
                        err = 2007;
                        return false;
                    }
                    var itemConfig = itemConfigManager.Get(rewardItemConfig.ItemId);
                    if (itemConfig == null)
                    {
                        Log.Warning($"[任务] 没找到 ItemConfig! rewardItemConfig.Id = {rewardItemConfig.Id}, itemId = {rewardItemConfig.ItemId}");
                        // 服务器配置错误
                        err = 2007;
                        return false;
                    }
                    // 找到可以放下物品的格子，并锁住
                    int posX = 0;
                    int posY = 0;
                    if (!backpack.mItemBox.CheckStatus(itemConfig.X, itemConfig.Y, ref posX, ref posY))
                    {
                        // 背包空间无法放下奖励的物品
                        err = 2006;
                        return false;
                    }
                    backpackLockList.Add(backpack.mItemBox.LockGrid(itemConfig.X, itemConfig.Y, posX, posY));

                    rewardItemConfigList.Add((rewardItemConfig, itemConfig, (posX, posY)));
                }
            }
            // 检查完成，可以放下

            if (gameTask.Config.TaskType == EGameTaskType.Hunting)
            {
                if (gameTask.Config.OneTimeTask == true)
                {
                    self.data.OneTimeTaskCompletionList.Add(gameTask.Config.ConfigId);
                }
                else
                {
                    // 狩猎任务 非 一次性任务
                    // 狩猎任务限制次数
                    self.data.HuntingTaskToDayCompleteCount += 1;
                    self.data.HuntingTaskComleteTime = Help_TimeHelper.GetNow();
                }
            }



            gameTask.TaskState = EGameTaskState.Received;
            gameTask.NotifyChanged(self.Parent);
            self.SaveDB();

            if (gameTask.Config.ReqCoin > 0)
            {
                // 消耗金币
                gamePlayer.UpdateCoin(E_GameProperty.GoldCoin, -gameTask.Config.ReqCoin, "任务-消耗金币");

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)self.Parent.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(self.Parent.GameAreaId);
                mWriteDataComponent.Save(gamePlayer.Data, dBProxy2).Coroutine();

                // 金币变动
                G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                mBattleKVData.Key = (int)E_GameProperty.GoldCoin;
                mBattleKVData.Value = gamePlayer.Data.GoldCoin;
                mChangeValue_notice.Info.Add(mBattleKVData);
                self.Parent.Send(mChangeValue_notice);
            }
            if (gameTask.Config.RewardCoin > 0)
            {
                // 奖励金币
                //gamePlayer.Data.GoldCoin += gameTask.Config.RewardCoin;
                gamePlayer.UpdateCoin(E_GameProperty.GoldCoin, gameTask.Config.RewardCoin, "任务-奖励金币");

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)self.Parent.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(self.Parent.GameAreaId);
                mWriteDataComponent.Save(gamePlayer.Data, dBProxy2).Coroutine();

                // 金币变动
                G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                mBattleKVData.Key = (int)E_GameProperty.GoldCoin;
                mBattleKVData.Value = gamePlayer.Data.GoldCoin;
                mChangeValue_notice.Info.Add(mBattleKVData);
                self.Parent.Send(mChangeValue_notice);
            }
            if (gameTask.Config.RewardExp > 0)
            {
                // 奖励经验
                gamePlayer.AddExprience(gameTask.Config.RewardExp);
            }

            // 背包解锁
            backpackLockList.Dispose();
            // 奖励物品
            if (rewardItemConfigList.Count != 0)
            {


                foreach (var rewardItemConfig in rewardItemConfigList)
                {
                    var item = ItemFactory.Create(rewardItemConfig.Item2, self.Parent.GameAreaId, rewardItemConfig.Item1.ToItemCreateAttr());
                    backpack.AddItem(item, rewardItemConfig.Item3.Item1, rewardItemConfig.Item3.Item2, $"任务奖励 taskId={gameTask.ConfigId}");
                }
            }

            // 自定义奖励
            if (customRewardMethod != null)
            {
                customRewardMethod.StartGivingRewards(gameTask, self.Parent);
            }


            // 领取奖励后，触发行为
            // 可以在此方法中扣除物品等
            gameTask.AfterReceiveReward(self.Parent);
            // 更新之后的任务
            switch (gameTask.Config.TaskType)
            {
                case EGameTaskType.Main:
                    GameMainTasksHelper.UpdateAfterTaskInfo(self, gameTask.ConfigId);
                    break;
                case EGameTaskType.Hunting:
                    GameHuntingTasksHelper.UpdateAfterTaskInfo(self, gameTask.ConfigId);
                    break;
                case EGameTaskType.Activity:
                    GameActivityTasksHelper.UpdateAfterTaskInfo(self, gameTask.ConfigId);
                    break;
                case EGameTaskType.Entrust:
                    GameEntrustTasksHelper.UpdateAfterTaskInfo(self, gameTask.ConfigId);
                    break;
                case EGameTaskType.CareerChange:
                    GameCareerChangeTasksHelper.UpdateAfterTaskInfo(self, gameTask.ConfigId);
                    break;
                case EGameTaskType.PassMission:
                    GamePassTasksHelper.UpdateAfterTaskInfo(self, gameTask.ConfigId);
                    break;
                default:
                    break;
            }

            err = 0;
            return true;
        }
    }
}
