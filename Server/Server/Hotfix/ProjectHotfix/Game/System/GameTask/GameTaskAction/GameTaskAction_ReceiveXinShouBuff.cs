using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETHotfix
{
    /// <summary>
    /// 领取狩猎任务
    /// </summary>
    [GameTaskActionMethod(EGameTaskActionType.ReceiveXinShouBuff)]
    public class GameTaskAction_ReceiveXinShouBuff : IGameTaskActionHandler
    {
        /// <summary>
        /// 初始化任务进度
        /// </summary>
        public void InitTaskProgress(GameTask gameTask, Player ownPlayer)
        {
            gameTask.TaskProgress[0] = 0;
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

    }

    [EventMethod("ReceiveXinShouBuff")]
    public class ReceiveXinShouBuff_ReceiveXinShouBuff : ITEventMethodOnRun<ETModel.EventType.ReceiveXinShouBuff>
    {
        public void OnRun(ETModel.EventType.ReceiveXinShouBuff args)
        {
            GameTasksComponent gameTasksCom;

            gameTasksCom = args.player.GetCustomComponent<GameTasksComponent>();
            if (gameTasksCom == null) return;


            using ListComponent<GameTask> gameTaskList = ListComponent<GameTask>.Create();
            List<GameTask> list = gameTaskList;

            gameTasksCom.GetTasksInProgressByActionType(ref list, EGameTaskActionType.ReceiveXinShouBuff);

            foreach (var gameTask in gameTaskList)
            {
                gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
            }
        }
    }




}
