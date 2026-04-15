
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ReceiveTaskHandler : AMActorRpcHandler<C2G_ReceiveTask,
        G2C_ReceiveTask>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ReceiveTask b_Request, G2C_ReceiveTask b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ReceiveTask b_Request, G2C_ReceiveTask b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            var tasksComponent = mPlayer.GetCustomComponent<GameTasksComponent>();

            var taskConfigManager = Root.MainFactory.GetCustomComponent<GameTaskConfigManager>();
            if (!taskConfigManager.TryGetConfig(b_Request.TaskId, out var taskConf))
            {
                // 没找到任务配置
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2000);
                b_Reply(b_Response);
                return true;
            }

            switch (taskConf.TaskType)
            {
                case EGameTaskType.Hunting:
                    // 狩猎任务
                    {
                        if (!taskConf.CanReceive(mPlayer.GetCustomComponent<GamePlayer>()))
                        {
                            // 不满足领取条件
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2001);
                            b_Reply(b_Response);
                            return true;
                        }
                        if (tasksComponent.data.HuntingTask != null && tasksComponent.data.HuntingTask.TaskState != EGameTaskState.Received)
                        {
                            // 重复领取任务
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2002);
                            b_Reply(b_Response);
                            return true;
                        }
                        if(tasksComponent.data.OneTimeTaskCompletionList.Contains(taskConf.ConfigId))
                        {
                            // 重复领取任务
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2002);
                            b_Reply(b_Response);
                            return true;
                        }
                        if (tasksComponent.data.HuntingTask != null)
                        {
                            if (tasksComponent.data.HuntingTask.Config.OneTimeTask == false &&
                            taskConf.OneTimeTask == false)
                            {
                                // 非一次性任务
                                if (tasksComponent.data.HuntingTask.Config.ConfigId != taskConf.ConfigId)
                                {
                                    tasksComponent.data.HuntingTaskComleteTime = 0;
                                    tasksComponent.data.HuntingTaskToDayCompleteCount = 0;
                                }
                            }
                        }
                        if (taskConf.OneTimeTask == false)
                        {
                            DateTime comleteDateTime = Help_TimeHelper.GetDateTime(tasksComponent.data.HuntingTaskComleteTime);
                            if (DateTime.Now.Date != comleteDateTime.Date)
                            {
                                tasksComponent.data.HuntingTaskComleteTime = 0;
                                tasksComponent.data.HuntingTaskToDayCompleteCount = 0;
                            }
                            if (tasksComponent.data.HuntingTaskToDayCompleteCount >= 3)
                            {
                                // 任务达到今日领取上限
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2010);
                                b_Reply(b_Response);
                                return true;
                            }
                        }


                        var gameTask = GameTaskFactory.Create(taskConf);
                        gameTask.TaskState = EGameTaskState.Doing;

                        b_Reply(b_Response);

                        GameHuntingTasksHelper.SetTask(tasksComponent, gameTask);


                        // 发布 ReceiveHuntingTask 事件
                        ETModel.EventType.ReceiveHuntingTask.Instance.player = mPlayer;
                        ETModel.EventType.ReceiveHuntingTask.Instance.taskConfigId = gameTask.ConfigId;
                        Root.EventSystem.OnRun("ReceiveHuntingTask", ETModel.EventType.ReceiveHuntingTask.Instance);
                    }
                    return true;
                case EGameTaskType.Activity:
                    // 活动任务
                    {
                        if (!taskConf.CanReceive(mPlayer.GetCustomComponent<GamePlayer>()))
                        {
                            // 不满注释足领取条件
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2001);
                            b_Reply(b_Response);
                            return true;
                        }
                        if (tasksComponent.data.ActivityTasks.ContainsKey(b_Request.TaskId))
                        {
                            // 重复领取任务
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2002);
                            b_Reply(b_Response);
                            return true;
                        }
                        if (!tasksComponent.BeforeTaskComplete(b_Request.TaskId))
                        {
                            // 上一个任务还未完成
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2009);
                            b_Reply(b_Response);
                            return true;
                        }

                        var gameTask = GameTaskFactory.Create(taskConf);
                        gameTask.TaskState = EGameTaskState.Doing;

                        b_Reply(b_Response);

                        foreach (var beforeId in gameTask.Config.TaskBeforeId)
                        {
                            if (tasksComponent.data.ActivityTasks.ContainsKey(beforeId))
                            {
                                tasksComponent.data.ActivityTasks.Remove(beforeId);
                                tasksComponent.SaveDB();
                            }
                        }

                        GameActivityTasksHelper.AddTask(tasksComponent, gameTask);
                    }
                    return true;
                case EGameTaskType.Entrust:
                    // 委托任务
                    {
                        if (!taskConf.CanReceive(mPlayer.GetCustomComponent<GamePlayer>()))
                        {
                            // 不满足领取条件
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2001);
                            b_Reply(b_Response);
                            return true;
                        }
                        if (tasksComponent.data.EntrustTask != null &&
                            (tasksComponent.data.EntrustTask.ConfigId == taskConf.ConfigId ||
                            tasksComponent.data.EntrustTask.TaskState != EGameTaskState.Received))
                        {
                            // 重复领取任务
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2002);
                            b_Reply(b_Response);
                            return true;
                        }
                        if (!tasksComponent.BeforeTaskComplete(b_Request.TaskId))
                        {
                            // 上一个任务还未完成
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2009);
                            b_Reply(b_Response);
                            return true;
                        }

                        var gameTask = GameTaskFactory.Create(taskConf);
                        gameTask.TaskState = EGameTaskState.Doing;

                        b_Reply(b_Response);

                        GameEntrustTasksHelper.SetTask(tasksComponent, gameTask);
                    }
                    return true;
                case EGameTaskType.CareerChange:
                    // 转职任务
                    {
                        if (!taskConf.CanReceive(mPlayer.GetCustomComponent<GamePlayer>()))
                        {
                            // 不满足领取条件
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2001);
                            b_Reply(b_Response);
                            return true;
                        }
                        if (tasksComponent.data.CareerChangeTask != null &&
                            (tasksComponent.data.CareerChangeTask.ConfigId == taskConf.ConfigId ||
                            tasksComponent.data.CareerChangeTask.TaskState != EGameTaskState.Received))
                        {
                            // 重复领取任务
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2002);
                            b_Reply(b_Response);
                            return true;
                        }
                        if (!tasksComponent.BeforeTaskComplete(b_Request.TaskId))
                        {
                            // 上一个任务还未完成
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2009);
                            b_Reply(b_Response);
                            return true;
                        }

                        var gameTask = GameTaskFactory.Create(taskConf);
                        gameTask.TaskState = EGameTaskState.Doing;

                        b_Reply(b_Response);

                        GameCareerChangeTasksHelper.SetTask(tasksComponent, gameTask);
                    }
                    return true;
                default:
                    // 无法领取这个类型的任务
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2003);
                    b_Reply(b_Response);
                    return true;
            }

        }
    }
}