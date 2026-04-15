using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETHotfix
{
    [GameTaskActionMethod(EGameTaskActionType.EnterTransferPoint)]
    public class GameTaskAction_EnterTransferPoint : IGameTaskActionHandler
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
            if(gameTask.TaskProgress[0] < gameTask.Config.TaskTargetCount[0])
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


    [EventMethod("GamePlayerEnterTransferPoint")]
    public class GamePlayerEnterTransferPoint_EnterTransferPoint : ITEventMethodOnRun<ETModel.EventType.GamePlayerEnterTransferPoint>
    {
        public void OnRun(ETModel.EventType.GamePlayerEnterTransferPoint args)
        {
            GameTasksComponent gameTasksCom;

            gameTasksCom = args.triggerGamePlayer.Player.GetCustomComponent<GameTasksComponent>();
            if (gameTasksCom == null) return;

            using ListComponent<GameTask> gameTaskList = ListComponent<GameTask>.Create();
            List<GameTask> list = gameTaskList;

            gameTasksCom.GetTasksInProgressByActionType(ref list,EGameTaskActionType.EnterTransferPoint);

            foreach (var gameTask in gameTaskList)
            {
                if (gameTask.Config.TaskTargetId.Contains(args.transferPoint))
                {
                    gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
                }
            }
        }

    }



}
