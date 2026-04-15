using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;
using System;

namespace ETHotfix
{
    /// <summary>
    /// 任务领取成功、任务奖励领取成功后 收到的通知
    /// </summary>
    [MessageHandler]
    public class G2C_UpdateGameTaskNotice_Handler : AMHandler<G2C_UpdateGameTaskNotice>
    {
        protected override void Run(ETModel.Session session, G2C_UpdateGameTaskNotice message)
        {
            Log.DebugBrown("任务通知" + message.TaskInfo.Id);
            TaskInfo taskInfo = TaskDatas.GetCurTaskInfo(message.TaskInfo.Id);
            taskInfo.TaskProgress = message.TaskInfo.Progress.ToList();
            taskInfo.State = message.TaskInfo.State;

            Log.Info("taskInfo:" + JsonHelper.ToJson(taskInfo));

            GuideComponent.Instance.ChangeTaskResetGuide(taskInfo);
            UIMainComponent.Instance?.SetTaskInfo(taskInfo);

            if (message.TaskInfo.State == 2)
            {
                /*  if (BeginnerGuideData.IsComplete(16))
                  {
                      BeginnerGuideData.SetBeginnerGuide(16);
                      UIMainComponent.Instance.SetBeginnerGuide();
                  }
                  //else if (BeginnerGuideData.IsComplete(27))
                  //{
                  //    BeginnerGuideData.SetBeginnerGuide(27);
                  //    UIMainComponent.Instance.SetBeginnerGuide();
                  //}
                  else if (BeginnerGuideData.IsComplete(31) || BeginnerGuideData.IsComplete(30))
                  {
                      if (BeginnerGuideData.IsComplete(30))
                      {
                          BeginnerGuideData.SetBeginnerGuide(30);
                      }
                      BeginnerGuideData.SetBeginnerGuide(31);
                      UIMainComponent.Instance.SetBeginnerGuide();
                  }*/
            }
            if (message.TaskInfo.State == 1 && taskInfo.TaskName == "地图传送")
            {
                /* if (BeginnerGuideData.IsCompleteTrigger(58, 58))
                 {
                     BeginnerGuideData.SetBeginnerGuide(58);
                     UIMainComponent.Instance.SetBeginnerGuide(true);
                 }*/
            }

            //如果刚领取的任务正在进行中
            if (message.TaskInfo.State == 1 && message.TaskInfo.StartTask == 1)
            {

                //UIMainComponent.Instance.SetBeginnerGuide(message.TaskInfo.Id);
                //GameTask_MainConfig taskConfig = ConfigComponent.Instance.GetItem<GameTask_MainConfig>(message.TaskInfo.Id);
                //if (taskConfig != null)
                //{
                //    UIComponent.Instance.Get(UIType.UIBeginnerGuide)?.GetComponent<UIBegiennerGuideComponent>().SetOnMaskRectShow();
                //    /* if (taskConfig.Guidance == 1 && !string.IsNullOrEmpty(taskConfig.GuidanceGif))
                //     {
                //         UIComponent.Instance.Get(UIType.UIBeginnerGuide)?.GetComponent<UIBegiennerGuideComponent>().PlayGif(taskConfig.GuidanceGif).Coroutine();
                //     }*/

                //}

            }
            UIMainComponent.Instance?.SetTaskInfo(taskInfo);


        }
    }
}
