using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETHotfix
{
    /// <summary>
    /// 判断有没有翅膀
    /// </summary>
    [GameTaskActionMethod(EGameTaskActionType.HaveWing)]
    public class GameTaskAction_HaveWing : IGameTaskActionHandler
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
            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
            EquipmentComponent equipment = player.GetCustomComponent<EquipmentComponent>();
            if(backpack.Where(item => item.Type == EItemType.Wing).Count() != 0 ||
                equipment.GetEquipItemByPosition(EquipPosition.Wing) != null)
            {
                for (int i = 0; i < gameTask.TaskProgress.Count; ++i)
                {
                    gameTask.TaskProgress[i] = 1;
                }
            }
        }

    }


    [EventMethod("BackpackAddItem")]
    public class HaveWing_BackpackAddItem : ITEventMethodOnRun<ETModel.EventType.BackpackAddItem>
    {
        public void OnRun(ETModel.EventType.BackpackAddItem args)
        {
            GameTasksComponent gameTasksCom;

            gameTasksCom = args.player.GetCustomComponent<GameTasksComponent>();
            if (gameTasksCom == null) return;
            if (args.item.Type != EItemType.Wing) return;

            using ListComponent<GameTask> gameTaskList = ListComponent<GameTask>.Create();
            List<GameTask> list = gameTaskList;
            gameTasksCom.GetTasksInProgressByActionType(ref list,EGameTaskActionType.CollectItem);

            foreach (var gameTask in gameTaskList)
            {
                gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
            }
        }
    }


}
