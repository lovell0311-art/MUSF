using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;


namespace ETHotfix
{
    /// <summary>
    /// 转职任务
    /// </summary>
    public static class GameCareerChangeTasksHelper
    {
        public static void Init(GameTasksComponent tasksCom)
        {

        }

        /// <summary>
        /// 更新后续任务
        /// </summary>
        /// <param name="tasksCom"></param>
        /// <param name="beforeTaskConfigId"></param>
        /// <param name="isNoticeClient"></param>
        public static void UpdateAfterTaskInfo(GameTasksComponent tasksCom, int beforeTaskConfigId, bool isNoticeClient = true)
        {

        }


        public static bool SetTask(GameTasksComponent tasksCom, GameTask gameTask, bool isNoticeClient = true)
        {
            tasksCom.data.CareerChangeTask = gameTask;

            gameTask.InitProgress(tasksCom.Parent);
            gameTask.TryCompleteTask();

            tasksCom.SaveDB();
            if (isNoticeClient == true)
            {
                // 通知玩家，任务变动
                gameTask.NotifyChanged(tasksCom.Parent);
                gameTask.StartTask = 0;
            }
            Log.Info($"[转职任务] 添加任务 taskId = {gameTask.ConfigId} state = {gameTask.TaskState}");
            return true;
        }

        public static void TriggerTaskAction(GameTasksComponent tasksCom, int taskConfigId, int progressId, int value)
        {
            if (tasksCom.data.CareerChangeTask == null) return;
            tasksCom.data.CareerChangeTask.UpdateProgress(progressId, value);
            tasksCom.data.CareerChangeTask.TryCompleteTask();

            tasksCom.SaveDB();
            // 通知玩家，任务变动
            tasksCom.data.CareerChangeTask.NotifyChanged(tasksCom.Parent);
            Log.Info($"[转职任务] 触发行为 taskId = {tasksCom.data.CareerChangeTask.ConfigId} progressId = {progressId} value = {value} state = {tasksCom.data.CareerChangeTask.TaskState}");
        }


    }
}
