using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using System.Linq;

namespace ETHotfix
{
    public static class GamePassTasksHelper
    {
        public static void Init(GameTasksComponent tasksCom)
        {
            if(tasksCom.data.PassTasks.Count == 0)
            {
                // 初始化任务
                UpdateAfterTaskInfo(tasksCom, 0, false);
                return;
            }

            UpdateStatelessTasks(tasksCom,false);


            //if (tasksCom.data.PassTasks != null && tasksCom.data.PassTasks.Count >= 1)
            //{
            //    var Info = tasksCom.data.PassTasks.LastOrDefault();
            //    if (Info.Value.TaskState == EGameTaskState.Doing)
            //    {
            //        Info.Value.TryCompleteTask();
            //        tasksCom.SaveDB();
            //        Info.Value.NotifyChanged(tasksCom.Parent);
            //        Info.Value.TaskState = EGameTaskState.Complete;
            //    }
            //}
        }

        public static void UpdateAfterTaskInfo(GameTasksComponent tasksCom, int beforeTaskConfigId, bool isNoticeClient = true)
        {
            if (beforeTaskConfigId == 0) return;
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
                if(conf.TaskType != EGameTaskType.PassMission)
                {
                    if(beforeTaskConfigId != 0) Log.Error($"通行证任务，不能切换到其他任务。taskId={conf.ConfigId},taskType={conf.TaskType}");
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
                AddTask(tasksCom, gameTask, isNoticeClient);
                break;
            }
        }

        /// <summary>
        /// 更新无状态任务
        /// </summary>
        /// <param name="taskCom"></param>
        public static void UpdateStatelessTasks(GameTasksComponent tasksCom, bool isNoticeClient = true)
        {
            foreach (var gameTask in tasksCom.data.PassTasks.Values)
            {
                if (gameTask.TaskState != EGameTaskState.None)
                {
                    return;
                }
                if (gameTask.Config.CanReceive(tasksCom.Parent.GetCustomComponent<GamePlayer>()))
                {
                    // 满足领取条件
                    // 自动领取任务
                    gameTask.TaskState = EGameTaskState.Doing;
                    gameTask.InitProgress(tasksCom.Parent);
                    gameTask.TryCompleteTask();
                    tasksCom.SaveDB();

                    if (isNoticeClient)
                    {
                        // 通知玩家
                        gameTask.NotifyChanged(tasksCom.Parent);
                        gameTask.StartTask = 0;
                    }
                    Log.Info($"[通行证任务] 自动领取任务 taskId = {gameTask.ConfigId} state = {gameTask.TaskState}");
                }
            }
        }

        public static bool AddTask(GameTasksComponent tasksCom, GameTask gameTask, bool isNoticeClient = true)
        {
            if (tasksCom.data.PassTasks.ContainsKey(gameTask.ConfigId))
            {
                // 已经领取了这个任务
                return false;
            }
            tasksCom.data.PassTasks.Add(gameTask.ConfigId, gameTask);

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
            Log.Info($"[通行证任务] 添加任务 taskId = {gameTask.ConfigId} state = {gameTask.TaskState}");
            return true;
        }

        public static void TriggerTaskAction(GameTasksComponent tasksCom, int taskConfigId, int progressId, int value)
        {
            if (!tasksCom.data.PassTasks.TryGetValue(taskConfigId, out var gameTask))
            {
                return;
            }
            gameTask.UpdateProgress(progressId, value);
            gameTask.TryCompleteTask();

            tasksCom.SaveDB();
            // 通知玩家，任务变动
            gameTask.NotifyChanged(tasksCom.Parent);
            Log.Info($"[通行证任务] 触发行为 taskId = {gameTask.ConfigId} progressId = {progressId} value = {value} state = {gameTask.TaskState}");
        }
    }

}


