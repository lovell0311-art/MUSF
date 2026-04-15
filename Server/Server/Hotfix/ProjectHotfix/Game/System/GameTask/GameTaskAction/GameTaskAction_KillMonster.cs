using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETHotfix
{
    [GameTaskActionMethod(EGameTaskActionType.KillMonster)]
    public class GameTaskAction_KillMonster : IGameTaskActionHandler
    {
        /// <summary>
        /// 初始化任务进度
        /// </summary>
        public void InitTaskProgress(GameTask gameTask, Player ownPlayer)
        {
            for(int i=0;i<gameTask.TaskProgress.Count;++i)
            {
                gameTask.TaskProgress[i] = 0;
            }
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


    [EventMethod("EnemyDeath")]
    public class EnemyDeath_KillMonster : ITEventMethodOnRun<ETModel.EventType.EnemyDeath>
    {
        public void OnRun(ETModel.EventType.EnemyDeath args)
        {
            GameTasksComponent gameTasksCom;
            if(args.attacker is GamePlayer gamePlayer)
            {
                gameTasksCom = gamePlayer.Player.GetCustomComponent<GameTasksComponent>();
            }
            else if(args.attacker is Pets pet)
            {
                gameTasksCom = pet.GamePlayer.Player.GetCustomComponent<GameTasksComponent>();
            }
            else if(args.attacker is Summoned summoned)
            {
                gameTasksCom = summoned.GamePlayer.Player.GetCustomComponent<GameTasksComponent>();
            }
            else
            {
                return;
            }


            if (gameTasksCom == null) return;
            using ListComponent<GameTask> gameTaskList = ListComponent<GameTask>.Create();
            List<GameTask> list = gameTaskList;
            gameTasksCom.GetTasksInProgressByActionType(ref list,EGameTaskActionType.KillMonster);

            foreach (var gameTask in gameTaskList)
            {
                #region 通行证 击杀任意怪物
                //普通怪
                if (gameTask.Config.TaskTargetId.Contains(-1))
                {
                    if (gameTask.Config.TaskTargetId[0] == -1 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
                    }
                }
                //黄金怪
                if (gameTask.Config.TaskTargetId.Contains(-2))
                {
                    if (gameTask.Config.TaskTargetId[0] == -2 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
                    }
                }
                #endregion
                if (!gameTask.Config.TaskTargetId.Contains(args.enemy.Config.Id))
                {
                    continue;
                }
                for (int i = 0; i < gameTask.Config.TaskTargetId.Count; ++i)
                {
                    if (gameTask.Config.TaskTargetId[i] == args.enemy.Config.Id &&
                        gameTask.CanUpdateProgress(i))
                    {
                        // TODO 更新任务
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, i, 1);
                    }
                }
            }
        }

    }



}
