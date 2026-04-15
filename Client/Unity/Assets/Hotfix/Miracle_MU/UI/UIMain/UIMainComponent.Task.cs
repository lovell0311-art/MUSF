using ETModel;
using UnityEngine.UI;
using ILRuntime.Runtime;
using System;
using UnityEditor;

namespace ETHotfix
{
    /// <summary>
    /// 任务模块
    /// </summary>
    public partial class UIMainComponent
    {
        ReferenceCollector collector_Task;
        public Button MainTask, ActivityTask, HuntTask, EntrustTask, ChangeTask;
        public bool isShow = false;

        public void Init_Task()
        {
            collector_Task = ReferenceCollector_Main.GetGameObject("Task").GetReferenceCollector();
            MainTask = collector_Task.GetButton("Main");
            ActivityTask = collector_Task.GetButton("Activity");
            HuntTask = collector_Task.GetButton("Hunt");
            EntrustTask = collector_Task.GetButton("Entrust");
            ChangeTask = collector_Task.GetButton("Change");

            HideTask();



            MainTask.onClick.AddSingleListener(() => ClickEvent(TaskDatas.MainTaskInfo).Coroutine());
            ActivityTask.onClick.AddSingleListener(() => ClickEvent(TaskDatas.ActivityTaskInfo).Coroutine());
            HuntTask.onClick.AddSingleListener(() => ClickEvent(TaskDatas.HuntTaskInfo).Coroutine());
            EntrustTask.onClick.AddSingleListener(() => ClickEvent(TaskDatas.EntrustTaskInfo).Coroutine());
            ChangeTask.onClick.AddSingleListener(() => ClickEvent(TaskDatas.ChangeTaskInfo).Coroutine());


            SetTaskInfo(TaskDatas.MainTaskInfo);
            SetTaskInfo(TaskDatas.HuntTaskInfo);
            SetTaskInfo(TaskDatas.ActivityTaskInfo);
            SetTaskInfo(TaskDatas.EntrustTaskInfo);
            if (TaskDatas.ChangeTaskInfo != null && (TaskDatas.ChangeTaskInfo.State == 1 || TaskDatas.ChangeTaskInfo.State == 2))
                SetTaskInfo(TaskDatas.ChangeTaskInfo);
            SetTaskInfo(TaskDatas.PassportTaskInfo328);
            SetTaskInfo(TaskDatas.PassportTaskInfo688);

        }

        async ETVoid ClickEvent(TaskInfo taskInfo)
        {
            if (taskInfo == null) return;
            //NoviceGuidePanel._instance.NextStep(MenuGuideConst.QianWangYongZheDaLu);

            if (taskInfo.TaskType == E_TaskType.CareerChangeTask)
            {
                UIComponent.Instance.VisibleUI(UIType.UICareerChangePanel);
            }
            else
            {
                //UIComponent.Instance.VisibleUI(UIType.UILHTask, new object[] { taskInfo.TaskType, taskInfo.State });
                UIComponent.Instance.VisibleUI(UIType.UITask, new object[] { taskInfo.TaskType, taskInfo.State });
                await TimerComponent.Instance.WaitAsync(100);
                UIComponent.Instance.Get(UIType.UITask).GetComponent<UITaskComponent>().IsNpc = false;
                UIComponent.Instance.Get(UIType.UITask).GetComponent<UITaskComponent>().ShowCurTaskInfo(taskInfo);
            }

            return;
            if (taskInfo.State == 2)
            {
                //直接提交任务
                G2C_ReceiveTaskReward g2C_ReceiveTask = (G2C_ReceiveTaskReward)await SessionComponent.Instance.Session.Call(new C2G_ReceiveTaskReward { TaskId = taskInfo.Id.ToInt32() });
                if (g2C_ReceiveTask.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ReceiveTask.Error.GetTipInfo());
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "奖励 领取成功");
                    //已经领取奖励
                    taskInfo.IsReceiveRewards = true;
                    taskInfo.State = 3;
                    Log.DebugGreen($"TaskDatas.ChangeTaskInfo.IsReceiveRewards:{taskInfo.IsReceiveRewards}");
                    if (taskInfo.TaskType == E_TaskType.CareerChangeTask || taskInfo.TaskType == E_TaskType.EntrustTask || taskInfo.TaskType == E_TaskType.HuntingTask)
                    {
                        UIMainComponent.Instance.GiveUpTask(taskInfo);
                    }

                    if (taskInfo.TaskType == E_TaskType.EntrustTask)
                    {
                        TaskDatas.ChangeTaskStatus(E_TaskType.EntrustTask, taskInfo);
                    }

                }

            }
            else if (taskInfo.State == 3)
            {
                //已领取
            }
            else
            {
                Log.DebugBrown($"移动到目标点");
                //寻路到目标点
                taskInfo.Nav2Target();

            }
        }

        public void HideTask()
        {
            MainTask.gameObject.SetActive(false);
            ActivityTask.gameObject.SetActive(false);
            HuntTask.gameObject.SetActive(false);
            EntrustTask.gameObject.SetActive(false);
            ChangeTask.gameObject.SetActive(false);
        }
        /// <summary>
        /// 显示任务 进度
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="info"></param>
        public void SetTaskInfo(TaskInfo info)
        {
            if (info == null) return;

            Log.DebugBrown("显示任务进度" + info.Id);
            E_TaskType taskType = (E_TaskType)(info.Id / 100000);
            if (info.State == 3)
            {
                switch (taskType)
                {
                    case E_TaskType.MainTask:
                        TaskDatas.MainTaskInfo = info;
                        break;
                    case E_TaskType.HuntingTask:
                        TaskDatas.HuntTaskInfo = info;
                        break;
                    case E_TaskType.ActivityTask:
                        TaskDatas.ActivityTaskInfo = info;
                        break;
                    case E_TaskType.EntrustTask:
                        TaskDatas.EntrustTaskInfo = info;
                        break;
                    case E_TaskType.CareerChangeTask:
                        TaskDatas.ChangeTaskInfo = info;
                        break;
                    case E_TaskType.ActivityMap:
                        break;
                    case E_TaskType.Passport:
                        Log.Debug("打" + info.Id / 1000);
                        if (info.Id / 1000 == 700 || info.Id / 1000 == 701 || info.Id / 1000 == 702 || info.Id / 1000 == 703 || info.Id / 1000 == 704)
                        {
                            TaskDatas.PassportTaskInfo328 = info;
                        }
                        else
                        {
                            TaskDatas.PassportTaskInfo688 = info;
                        }
                        break;
                    default:
                        break;
                }
                return;
            }

            switch (taskType)
            {
                case E_TaskType.MainTask:
                    {
                        ShowKillTips();
                        MainTask.transform.Find("info").GetComponent<Text>().text = info.TaskName + $"\t{info.KillMonsterCount}/{info.TaskTargetCounts}";
                        if (info.State == 0)
                        {
                            MainTask.transform.Find("Text").GetComponent<Text>().text = "<color=red>未达标</color>";
                        }
                        else
                        {
                            MainTask.transform.Find("Text").GetComponent<Text>().text = info.States();
                        }

                        TaskDatas.MainTaskInfo = info;
                        MainTask.gameObject.SetActive(true);

                        //查询当前任务界面是否打开
                        UI uitask = UIComponent.Instance.Get(UIType.UITask);
                        if (uitask != null)
                        {
                            uitask.GetComponent<UITaskComponent>()?.UpdateTask();
                        }
                        break;
                    }

                case E_TaskType.ActivityTask:
                    ShowKillTips();
                    ActivityTask.transform.Find("info").GetComponent<Text>().text = info.TaskName + $"\t{info.KillMonsterCount}/{info.TaskTargetCounts}";
                    ActivityTask.transform.Find("Text").GetComponent<Text>().text = info.States();
                    TaskDatas.ActivityTaskInfo = info;
                    ActivityTask.gameObject.SetActive(true);
                    break;
                case E_TaskType.HuntingTask:
                    ShowKillTips();
                    HuntTask.transform.Find("info").GetComponent<Text>().text = info.TaskName + $"{info.KillMonsterCount}/{info.TaskTargetCounts}"; ;
                    HuntTask.transform.Find("Text").GetComponent<Text>().text = info.States();
                    HuntTask.gameObject.SetActive(true);
                    TaskDatas.HuntTaskInfo = info;
                    break;
                case E_TaskType.EntrustTask:
                    ShowKillTips();
                    EntrustTask.transform.Find("info").GetComponent<Text>().text = info.TaskName + $"{info.KillMonsterCount}/{info.TaskTargetCounts}"; ;
                    EntrustTask.transform.Find("Text").GetComponent<Text>().text = info.States();
                    TaskDatas.EntrustTaskInfo = info;
                    EntrustTask.gameObject.SetActive(true);
                    break;
                case E_TaskType.CareerChangeTask:
                    var count = 1;
                    var killcount = 0;
                    string[] items = info.MonsterName.Split(new char[] { '、' }, StringSplitOptions.RemoveEmptyEntries);
                    string tips = string.Empty;
                    if (items.Length > 1)
                    {
                        //关闭提示
                        for (int i = 0, length = items.Length; i < length; i++)
                        {
                            count += info.TaskTargetCount[i];
                            killcount += info.TaskProgress[i];
                            if (killcount > count) continue;
                            if (count > 1)
                            {
                                tips += count != killcount ? $"击杀 {items[i]} <color=green>{info.TaskProgress[i]}</color>/{info.TaskTargetCount[i]} 只\n" :
                             $"{items[i]} {killcount}只 已完成\n";
                            }
                            else
                            {
                                tips += count != killcount ? $"收集 {items[i]} <color=green>{info.TaskProgress[i]}</color>/{info.TaskTargetCount[i]} \n" :
                            $"{items[i]} {killcount}只 已完成\n";
                            }
                        }
                        UIComponent.Instance.VisibleUI(UIType.UIHint, tips);

                    }
                    else
                    {

                        if (info.State == 2)
                        {
                            count = info.TaskTargetCount[0];
                            killcount = info.TaskProgress[0];
                            UIComponent.Instance.VisibleUI(UIType.UIHint, $"{info.TaskName} 已完成");
                        }
                    }
                    //  ChangeTask.transform.Find("info").GetComponent<Text>().text = info.TaskName + $"{killcount}/{count}";
                    // ChangeTask.transform.Find("Text").GetComponent<Text>().text = info.States();
                    TaskDatas.ChangeTaskInfo = info;
                    // ChangeTask.gameObject.SetActive(true);
                    break;
                case E_TaskType.Passport:
                    {
                        Log.DebugBrown("数据" + info.Id / 1000);
                        if (info.Id / 1000 == 700|| info.Id / 1000 == 701 || info.Id / 1000 == 702 || info.Id / 1000 == 703 || info.Id / 1000 == 704)
                        {
                            TaskDatas.PassportTaskInfo328 = info;
                        }
                        else
                        {
                            TaskDatas.PassportTaskInfo688 = info;
                        }
                        UI uitask = UIComponent.Instance.Get(UIType.UIPassport);
                        if (uitask != null)
                        {
                            uitask.GetComponent<UIPassportComponent>()?.UpdateView();
                        }
                        break;
                    }
            }

            ///显示任务进度提示
            void ShowKillTips()
            {
                return;

                if (!isShow)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(info.MonsterName))
                {
                    string tips = string.Empty;
                    if (info.TaskActionType == 1)
                    {
                        tips = info.KillMonsterCount != info.TaskTargetCounts ? $"击杀 {info.MonsterName} <color=green>{info.KillMonsterCount}</color>/{info.TaskTargetCounts} 只" :
                            $"{info.TaskName} {info.TaskTargetCounts}只 已完成";
                    }
                    else if (info.TaskActionType == 4)
                    {
                        tips = info.KillMonsterCount != info.TaskTargetCounts ? $"{info.MonsterName} <color=green>{info.KillMonsterCount}</color>/{info.TaskTargetCounts} 个" :
                                $"{info.TaskName} {info.TaskTargetCounts}个 已完成";
                    }
                    UIComponent.Instance.VisibleUI(UIType.UIHint, tips);
                }
            }
        }
        /// <summary>
        /// 放弃任务
        /// </summary>
        /// <param name="info"></param>

        public void GiveUpTask(TaskInfo info)
        {
            if (info == null) return;

            E_TaskType taskType = (E_TaskType)(info.Id / 100000);
            switch (taskType)
            {
                case E_TaskType.MainTask:

                    //TaskDatas.MainTaskInfo = null;
                    MainTask.gameObject.SetActive(false);
                    break;
                case E_TaskType.ActivityTask:

                    TaskDatas.ActivityTaskInfo = null;
                    ActivityTask.gameObject.SetActive(false);
                    break;
                case E_TaskType.HuntingTask:

                    HuntTask.gameObject.SetActive(false);
                    TaskDatas.HuntTaskInfo = null;
                    break;
                case E_TaskType.EntrustTask:

                    //TaskDatas.EntrustTaskInfo = null;
                    EntrustTask.gameObject.SetActive(false);
                    break;
                case E_TaskType.CareerChangeTask:
                    // TaskDatas.ChangeTaskInfo = null;
                    ChangeTask.gameObject.SetActive(false);
                    break;

            }
        }

        /// <summary>
        /// 清理当前角色的任务
        /// </summary>
        public void ClearTask()
        {
            HideTask();
            TaskDatas.MainTaskInfo = null;
            TaskDatas.ActivityTaskInfo = null;
            TaskDatas.HuntTaskInfo = null;
            TaskDatas.EntrustTaskInfo = null;
        }
    }
}
