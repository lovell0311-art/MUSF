using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;


namespace ETHotfix
{
    public static class GameMainTasksHelper
    {
        public static void Init(GameTasksComponent tasksCom)
        {
            if(tasksCom.data.MainTask == null)
            {
                // 初始化任务
                UpdateAfterTaskInfo(tasksCom, 0, false);
                return;
            }

            UpdateStatelessTasks(tasksCom,false);


            if (tasksCom.data.MainTask != null && tasksCom.data.MainTask.TaskState == EGameTaskState.Received)
            {
                UpdateAfterTaskInfo(tasksCom, tasksCom.data.MainTask.ConfigId, false);
            }


        }

        public static void UpdateAfterTaskInfo(GameTasksComponent tasksCom, int beforeTaskConfigId, bool isNoticeClient = true)
        {
            var taskConfigIdList = Root.MainFactory.GetCustomComponent<GameTaskConfigManager>().GetAfterTaskIdListByBeforeId(beforeTaskConfigId);
            if (taskConfigIdList == null)
            {
                // 没有可以做的任务了
                return;
            }

            var configManager = Root.MainFactory.GetCustomComponent<GameTaskConfigManager>();
            var gamePlayer = tasksCom.Parent.GetCustomComponent<GamePlayer>();
            foreach (var taskConfigId in taskConfigIdList)
            {
                if (!configManager.TryGetConfig(taskConfigId, out var conf))
                {
                    continue;
                }
                if(conf.TaskType != EGameTaskType.Main)
                {
                    if(beforeTaskConfigId != 0) Log.Error($"主线任务，不能切换到其他任务。taskId={conf.ConfigId},taskType={conf.TaskType}");
                    continue;
                }
                if (!conf.AllowedOccupation((E_GameOccupation)gamePlayer.Data.PlayerTypeId))
                {
                    // 不是这个职业的任务
                    continue;
                }
                if(!tasksCom.BeforeTaskComplete(taskConfigId))
                {
                    // 还有未完成的前置任务
                    continue;
                }

                var gameTask = GameTaskFactory.Create(conf);

                // 设置任务是否领取
                gameTask.TaskState = conf.CanReceive(gamePlayer) ? EGameTaskState.Doing : EGameTaskState.None;

                // 自动领取任务
                SetTask(tasksCom, gameTask, isNoticeClient);
                tasksCom.SaveDB();
                break;
            }
        }

        /// <summary>
        /// 更新无状态任务
        /// </summary>
        /// <param name="taskCom"></param>
        public static void UpdateStatelessTasks(GameTasksComponent tasksCom, bool isNoticeClient = true)
        {
            if (tasksCom.data.MainTask == null) return;
            var gameTask = tasksCom.data.MainTask;
            if (gameTask.TaskState != EGameTaskState.None)
            {
                return;
            }
            if(gameTask.Config.CanReceive(tasksCom.Parent.GetCustomComponent<GamePlayer>()))
            {
                // 满足领取条件
                // 自动领取任务
                gameTask.TaskState = EGameTaskState.Doing;
                gameTask.InitProgress(tasksCom.Parent);
                gameTask.TryCompleteTask();
                tasksCom.SaveDB();

                if(isNoticeClient)
                {
                    // 通知玩家
                    gameTask.NotifyChanged(tasksCom.Parent);
                    gameTask.StartTask = 0;
                }
                Log.Info($"[主线任务] 自动领取任务 taskId = {gameTask.ConfigId} state = {gameTask.TaskState}");
                AutoReceiveReward(tasksCom, gameTask.ConfigId);
            }
        }

        public static void SetTask(GameTasksComponent tasksCom, GameTask gameTask, bool isNoticeClient = true)
        {
            tasksCom.data.MainTask = gameTask;

            if (gameTask.TaskState == EGameTaskState.Doing)
            {
                gameTask.InitProgress(tasksCom.Parent);
                gameTask.TryCompleteTask();
            }
            tasksCom.SaveDB();
            if (isNoticeClient == true)
            {
                // 通知玩家，任务变动
                gameTask.NotifyChanged(tasksCom.Parent);
                gameTask.StartTask = 0;
            }
            Log.Info($"[主线任务] 添加任务 taskId = {gameTask.ConfigId} state = {gameTask.TaskState}");
            AutoReceiveReward(tasksCom, gameTask.ConfigId);
        }

        public static void TriggerTaskAction(GameTasksComponent tasksCom, int taskConfigId, int progressId, int value)
        {
            if (tasksCom.data.MainTask == null) return;
            if (tasksCom.data.MainTask.ConfigId == taskConfigId)
            {
                tasksCom.data.MainTask.UpdateProgress(progressId, value);
                tasksCom.data.MainTask.TryCompleteTask();

                tasksCom.SaveDB();
                // 通知玩家，任务变动
                tasksCom.data.MainTask.NotifyChanged(tasksCom.Parent);
                Log.Info($"[主线任务] 触发行为 taskId = {tasksCom.data.MainTask.ConfigId} progressId = {progressId} value = {value} state = {tasksCom.data.MainTask.TaskState}");
                AutoReceiveReward(tasksCom, taskConfigId);
            }
        }

        public static void AutoReceiveReward(GameTasksComponent tasksCom, int taskConfigId)
        {
            var gameTask = tasksCom.data.MainTask;
            if (gameTask == null) return;
            if (gameTask.ConfigId != taskConfigId) return;
            if (gameTask.Config.AutoReceiveReward != true) return;
            if(gameTask.TaskState == EGameTaskState.Complete)
            {
                // 任务完成
                // TODO 开始领取奖励
                int err = 0;
                tasksCom.ReceiveTaskReward(taskConfigId, out err);
            }
            
        }

    }

    [EventMethod("GamePlayerLevelUp")]
    public class GamePlayerLevelUp_MainTasksHelper : ITEventMethodOnRun<ETModel.EventType.GamePlayerLevelUp>
    {
        public void OnRun(ETModel.EventType.GamePlayerLevelUp args)
        {
            GameTasksComponent gameTasksCom;

            gameTasksCom = args.gamePlayer.Player.GetCustomComponent<GameTasksComponent>();

            GameMainTasksHelper.UpdateStatelessTasks(gameTasksCom);


        }

    }

    [EventMethod("GamePlayerCareerChangeComplete")]
    public class GamePlayerCareerChangeComplete_MainTasksHelper : ITEventMethodOnRun<ETModel.EventType.GamePlayerCareerChangeComplete>
    {
        public void OnRun(ETModel.EventType.GamePlayerCareerChangeComplete args)
        {
            GameTasksComponent gameTasksCom;

            gameTasksCom = args.gamePlayer.Player.GetCustomComponent<GameTasksComponent>();

            GameMainTasksHelper.UpdateStatelessTasks(gameTasksCom);


        }

    }
}


