using System.Collections.Generic;
using ETModel;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 更新全部任务
    /// 进入游戏时通知
    /// </summary>
    [MessageHandler]
    public class G2C_AllUpdateGameTaskNotice_Handler : AMHandler<G2C_AllUpdateGameTaskNotice>
    {
        protected override void Run(ETModel.Session session, G2C_AllUpdateGameTaskNotice message)
        {
            Log.Info("初始化任务");
           // UpdateTask(message.MainTasks.ToList());
            //UpdateTask(message.HuntingTasks.ToList());
            //UpdateTask(message.ActivityTasks.ToList());
            //UpdateTask(message.EntrustTasks.ToList());
            UpdateTask(message.CareerChangeTasks.ToList());
            UpdateTask(message.PassTasks.ToList());

            static void UpdateTask(List<Struct_TaskInfo> struct_TaskInfos)
            {
                for (int i = 0, length = struct_TaskInfos.Count; i < length; i++)
                {

                    var task = struct_TaskInfos[i];
                    // Log.DebugBrown($"更新任务：{(E_TaskType)(task.Id/100000)} : {task.Id} : {task.State}");
                    TaskDatas.InitCurTask(task.Id, task.State, task.Progress.ToList());

                }
            }

            ////UIComponent.Instance.VisibleUI(UIType.UIBeginnerGuide);
            ////UIComponent.Instance.Get(UIType.UIBeginnerGuide)?.GetComponent<UIBegiennerGuideComponent>().SetOnMaskRectShow();
            //for (int i = 0; i < message.MainTasks.count; i++)
            //{
            //    //如果刚领取的任务正在进行中
            //    if (message.MainTasks[i].State == 1 || message.MainTasks[i].State == 2/* && message.MainTasks[i].StartTask == 1*/)
            //    {
            //      //  Log.DebugBrown($"通知任务引导：{message.MainTasks[i].Id - 100000 + 1}");
            //        if (Guidance_Define.IsBeginnerGuide)
            //        {
            //            //UIMainComponent.Instance.SetBeginnerGuide(message.MainTasks[i].Id - 100000 + 1);
            //        }
            //        GameTask_MainConfig taskConfig = ConfigComponent.Instance.GetItem<GameTask_MainConfig>(message.MainTasks[i].Id);
            //        if (taskConfig != null)
            //        {

            //            //if (taskConfig.Guidance == 1 && !string.IsNullOrEmpty(taskConfig.GuidanceGif))
            //            //{
            //            //    Log.DebugGreen("----播GIF勒");
            //            //    UIComponent.Instance.Get(UIType.UIBeginnerGuide)?.GetComponent<UIBegiennerGuideComponent>().PlayGif(taskConfig.GuidanceGif).Coroutine();
            //            //}

            //        }

            //    }
            //}
        }
    }
}
