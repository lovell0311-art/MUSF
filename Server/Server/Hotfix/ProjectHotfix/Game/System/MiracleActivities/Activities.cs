using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Ame.V20190916.Models;
using TencentCloud.Bri.V20190328.Models;


namespace ETHotfix
{
    public static partial class ActivitiesComponentSystem
    {
        [EventMethod(typeof(ActivitiesComponent), EventSystemType.INIT)]
        public class ActivitiesComponentOnInit : ITEventMethodOnInit<ActivitiesComponent>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="b_Component"></param>
            public void OnInit(ActivitiesComponent b_Component)
            {
                
                b_Component.OnInit().Coroutine();
            }
        }

        public static async Task OnInit(this ActivitiesComponent b_Component)
        {
 
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            await mTimerComponent.WaitAsync(10000);

            b_Component.Parent.AddCustomComponent<RushGradeActivity>();//冲级活动
            b_Component.Parent.AddCustomComponent<NewYearAnimalActivity>();//兔年打兔兔，年兽活动
            b_Component.Parent.AddCustomComponent<PayActivities>();
            b_Component.OnlyRunUpdate();
                
            while (b_Component.IsDisposeable == false && b_Component.IsRunUpdate)
            {
                long Time = Help_TimeHelper.GetNowSecond();
                await mTimerComponent.WaitAsync(1000);
                if (Time - b_Component.UpDataTime >= 1)
                {
                    foreach (var Activit in b_Component.Activities)
                    {
                        b_Component.ActivitTime(Activit.Key, Time);
                    }
                    var NewYear = b_Component.Parent.GetCustomComponent<NewYearAnimalActivity>();
                    NewYear.UpdateNewYearAnimalActivity(b_Component.Parent);
                    NewYear.ActiveFall(b_Component.Parent);
                    NewYear.RecycleMonster(b_Component.Parent);

                    b_Component.UpDataTime = Time;
                }
            }
            
        }
        public static void ActivitTime(this ActivitiesComponent b_Component, int ActivityID, long CurrentTime)
        {
            b_Component.Activities.TryGetValue(ActivityID, out StructActivit Info);
            if (Info != null)
            {
                if (!Info.IsOpen)
                {
                    if (Info.OpenTime <= CurrentTime)
                    {
                        if (Info.EndTime != 0 && Info.EndTime > CurrentTime)
                        {
                            Info.IsOpen = true;
                            return;
                        }
                    }
                }
                else
                {
                    if (Info.EndTime != 0 && Info.EndTime <= CurrentTime)
                    {
                        Info.IsOpen = false;
                        return;
                    }
                }
            }

        }
        /// <summary>
        /// 添加活动
        /// </summary>
        /// <param name="b_Component"></param>
        public static void AddActivit(this ActivitiesComponent b_Component, StructActivit structActivit)
        {
            if (b_Component.Activities.ContainsKey(structActivit.ActivityID) == false)
                b_Component.Activities.Add(structActivit.ActivityID, structActivit);
            else
                Log.PLog("Activit", $"添加活动失败 ID:{structActivit.ActivityID} 已存在相同ID的活动");
        }
        /// <summary>
        /// 获取活动是否正在进行
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="ActivityID"></param>
        /// <returns></returns>
        public static bool GetActivitState(this ActivitiesComponent b_Component, int ActivityID)
        {
            if (b_Component.Activities.TryGetValue(ActivityID, out StructActivit Info) != false)
                return Info.IsOpen;
            else
            {
                Log.PLog("Activit", $"获取活动失败 ID:{ActivityID} 不存在ID的活动");
                return false;
            }
        }
        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="ActivityID"></param>
        /// <returns></returns>
        public static StructActivit GetActivit(this ActivitiesComponent b_Component, int ActivityID)
        {
            if (b_Component.Activities.TryGetValue(ActivityID, out StructActivit Info) != false)
                return Info;
            else
            {
                Log.PLog("Activit", $"获取活动失败 ID:{ActivityID} 不存在ID的活动");
                return null;
            }
        }
        /// <summary>
        /// 根据时间检查活动是否开启
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="OpenTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="ActivityID"></param>
        public static void CheckActivitTime(this ActivitiesComponent b_Component, string OpenTime, string EndTime, int ActivityID)
        {
            if (b_Component.Activities.ContainsKey(ActivityID) == false)
            {
                StructActivit structActivit = new StructActivit();
                if (OpenTime != "")
                {
                    structActivit.OpenTime = Help_TimeHelper.DateConversionTime(Convert.ToDateTime(OpenTime));

                    if (EndTime != "")
                        structActivit.EndTime = Help_TimeHelper.DateConversionTime(Convert.ToDateTime(EndTime));
                }
                structActivit.ActivityID = ActivityID;
                structActivit.IsOpen = false;

                long CurrentTime = Help_TimeHelper.GetNowSecond();
                if (structActivit.EndTime == 0 || structActivit.EndTime > CurrentTime)
                {
                    if (structActivit.OpenTime <= CurrentTime)
                    {
                        structActivit.IsOpen = true;
                    }
                    b_Component.AddActivit(structActivit);
                }

            }
            else
                Log.PLog("Activit", $"添加活动失败 ID:{ActivityID} 已存在相同ID的活动");
        }

    }
}