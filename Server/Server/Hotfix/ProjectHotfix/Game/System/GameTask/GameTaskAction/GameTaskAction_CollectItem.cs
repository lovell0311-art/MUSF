using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETHotfix
{
    /// <summary>
    /// 收集物品
    /// </summary>
    [GameTaskActionMethod(EGameTaskActionType.CollectItem)]
    public class GameTaskAction_CollectItem : IGameTaskActionHandler
    {
        /// <summary>
        /// 初始化任务进度
        /// </summary>
        public void InitTaskProgress(GameTask gameTask, Player ownPlayer)
        {
            RefreshTaskProgress(gameTask, ownPlayer);
        }

        /// <summary>
        /// 尝试完成任务
        /// </summary>
        /// <param name="gameTask"></param>
        public void TryCompleteTask(GameTask gameTask)
        {
            // 检测任务是否完成
            bool isComplete = true;
            for (int i = 0; i < gameTask.Config.TaskTargetCount.Count; ++i)
            {
                if (gameTask.TaskProgress[i] < gameTask.Config.TaskTargetCount[i])
                {
                    isComplete = false;
                    break;
                }
            }
            if (!isComplete)
            {
                return;
            }

            // 设置任务状态为完成的
            gameTask.TaskState = EGameTaskState.Complete;
        }

        /// <summary>
        /// 领取奖励后
        /// </summary>
        /// <param name="gameTask"></param>
        public void AfterReceiveReward(GameTask gameTask, Player player)
        {

        }

        /// <summary>
        /// 刷新任务进度
        /// </summary>
        /// <param name="gameTask"></param>
        /// <param name="player"></param>
        public static void RefreshTaskProgress(GameTask gameTask,Player player)
        {
            for (int i = 0; i < gameTask.TaskProgress.Count; ++i)
            {
                gameTask.TaskProgress[i] = 0;
            }
            var backpack = player.GetCustomComponent<BackpackComponent>();
            for (int i = 0; i < gameTask.Config.TaskTargetId.Count; ++i)
            {
                var allItem = backpack.GetAllItemByConfigID(gameTask.Config.TaskTargetId[i]);
                if(allItem != null && allItem.Count != 0)
                {
                    foreach(Item item in allItem.Values)
                    {
                        gameTask.TaskProgress[i] += item.GetProp(EItemValue.Quantity);
                    }
                }
            }
        }

    }


    [EventMethod("BackpackAddItem")]
    public class CollectItem_BackpackAddItem : ITEventMethodOnRun<ETModel.EventType.BackpackAddItem>
    {
        public void OnRun(ETModel.EventType.BackpackAddItem args)
        {
            GameTasksComponent gameTasksCom;

            gameTasksCom = args.player.GetCustomComponent<GameTasksComponent>();
            if (gameTasksCom == null) return;

            using ListComponent<GameTask> gameTaskList = ListComponent<GameTask>.Create();
            List<GameTask> list = gameTaskList;
            gameTasksCom.GetTasksInProgressByActionType(ref list,EGameTaskActionType.CollectItem);
            var backpack = args.player.GetCustomComponent<BackpackComponent>();

            foreach (var gameTask in gameTaskList)
            {
                for (int i = 0; i < gameTask.Config.TaskTargetId.Count; ++i)
                {
                    if (args.item.ConfigID == gameTask.Config.TaskTargetId[i] &&
                        gameTask.CanUpdateProgress(i))
                    {
                        var allItem = backpack.GetAllItemByConfigID(args.item.ConfigID);
                        if(allItem != null && allItem.Count != 0)
                        {
                            int itemCount = 0;
                            foreach(var item in allItem.Values)
                            {
                                itemCount += item.GetProp(EItemValue.Quantity);
                            }
                            gameTasksCom.TriggerTaskAction(gameTask.ConfigId, i, itemCount);
                        }
                    }
                }
            }
        }
    }


    [EventMethod("BackpackRemoveItem")]
    public class CollectItem_BackpackRemoveItem : ITEventMethodOnRun<ETModel.EventType.BackpackRemoveItem>
    {
        public void OnRun(ETModel.EventType.BackpackRemoveItem args)
        {
            GameTasksComponent gameTasksCom;

            gameTasksCom = args.player.GetCustomComponent<GameTasksComponent>();
            if (gameTasksCom == null) return;

            using ListComponent<GameTask> gameTaskList = ListComponent<GameTask>.Create();
            List<GameTask> list = gameTaskList;

            gameTasksCom.GetTasksInProgressByActionType(ref list, EGameTaskActionType.CollectItem);
            var backpack = args.player.GetCustomComponent<BackpackComponent>();

            foreach (var gameTask in gameTaskList)
            {
                for (int i = 0; i < gameTask.Config.TaskTargetId.Count; ++i)
                {
                    if (args.item.ConfigID == gameTask.Config.TaskTargetId[i] &&
                        gameTask.CanUpdateProgress(i))
                    {
                        var allItem = backpack.GetAllItemByConfigID(args.item.ConfigID);
                        if (allItem != null && allItem.Count != 0)
                        {
                            int itemCount = 0;
                            foreach (var item in allItem.Values)
                            {
                                itemCount += item.GetProp(EItemValue.Quantity);
                            }
                            gameTasksCom.TriggerTaskAction(gameTask.ConfigId, i, itemCount);
                        }
                    }
                }
            }
        }
    }

    [EventMethod("ItemCountChangeInBackpack")]
    public class CollectItem_ItemCountChangeInBackpack : ITEventMethodOnRun<ETModel.EventType.ItemCountChangeInBackpack>
    {
        public void OnRun(ETModel.EventType.ItemCountChangeInBackpack args)
        {
            GameTasksComponent gameTasksCom;

            gameTasksCom = args.player.GetCustomComponent<GameTasksComponent>();
            if (gameTasksCom == null) return;

            using ListComponent<GameTask> gameTaskList = ListComponent<GameTask>.Create();
            List<GameTask> list = gameTaskList;

            gameTasksCom.GetTasksInProgressByActionType(ref list, EGameTaskActionType.CollectItem);
            var backpack = args.player.GetCustomComponent<BackpackComponent>();

            foreach (var gameTask in gameTaskList)
            {
                for (int i = 0; i < gameTask.Config.TaskTargetId.Count; ++i)
                {
                    if (args.item.ConfigID == gameTask.Config.TaskTargetId[i] &&
                        gameTask.CanUpdateProgress(i))
                    {
                        var allItem = backpack.GetAllItemByConfigID(args.item.ConfigID);
                        if (allItem != null && allItem.Count != 0)
                        {
                            int itemCount = 0;
                            foreach (var item in allItem.Values)
                            {
                                itemCount += item.GetProp(EItemValue.Quantity);
                            }
                            gameTasksCom.TriggerTaskAction(gameTask.ConfigId, i, itemCount);
                        }
                    }
                }
            }
        }
    }

    [EventMethod("ChecJinBiChangeInBackpack")]
    public class CollectAndSubmitItem_ChecJinBiChangeInBackpack : ITEventMethodOnRun<ETModel.EventType.ChecJinBiChangeInBackpack>
    {
        public void OnRun(ETModel.EventType.ChecJinBiChangeInBackpack args)
        {
            GameTasksComponent gameTasksCom;

            gameTasksCom = args.player.GetCustomComponent<GameTasksComponent>();
            var gameplayer = args.player.GetCustomComponent<GamePlayer>();
            if (gameTasksCom == null || gameplayer == null) return;

            using ListComponent<GameTask> gameTaskList = ListComponent<GameTask>.Create();
            List<GameTask> list = gameTaskList;

            gameTasksCom.GetTasksInProgressByActionType(ref list, EGameTaskActionType.CollectItem);
            var backpack = args.player.GetCustomComponent<BackpackComponent>();

            foreach (var gameTask in gameTaskList)
            {
                for (int i = 0; i < gameTask.Config.TaskTargetId.Count; ++i)
                {
                    if (320294 == gameTask.Config.TaskTargetId[i] &&
                        gameTask.CanUpdateProgress(i))
                    {
                        long JinBiCnt = gameplayer.Data.GoldCoin;
                        if (JinBiCnt >= int.MaxValue)
                            gameTasksCom.TriggerTaskAction(gameTask.ConfigId, i, int.MaxValue);
                        else
                            gameTasksCom.TriggerTaskAction(gameTask.ConfigId, i, (int)JinBiCnt);

                    }
                }
            }
        }
    }
}
