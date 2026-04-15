using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETHotfix
{
    [GameTaskActionMethod(EGameTaskActionType.LevelUp)]
    public class GameTaskAction_LevelUp : IGameTaskActionHandler
    {
        /// <summary>
        /// 初始化任务进度
        /// </summary>
        public void InitTaskProgress(GameTask gameTask, Player ownPlayer)
        {
            gameTask.TaskProgress[0] = ownPlayer.GetCustomComponent<GamePlayer>().Data.Level;
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


    [EventMethod("GamePlayerLevelUp")]
    public class GamePlayerLevelUp_LevelUp : ITEventMethodOnRun<ETModel.EventType.GamePlayerLevelUp>
    {
        public void OnRun(ETModel.EventType.GamePlayerLevelUp args)
        {
            GameTasksComponent gameTasksCom;

            gameTasksCom = args.gamePlayer.Player.GetCustomComponent<GameTasksComponent>();
            if (gameTasksCom == null) return;

            using ListComponent<GameTask> gameTaskList = ListComponent<GameTask>.Create();
            List<GameTask> list = gameTaskList;
            gameTasksCom.GetTasksInProgressByActionType(ref list,EGameTaskActionType.LevelUp);

            if(gameTaskList != null)
            {
                foreach (var gameTask in gameTaskList)
                {
                    gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, args.gamePlayer.Data.Level);
                }
            }

        }

    }



}
