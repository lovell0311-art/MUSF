using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using TencentCloud.Ame.V20190916.Models;
using TencentCloud.Bri.V20190328.Models;
using TencentCloud.Solar.V20181011.Models;


namespace ETHotfix
{
    [EventMethod(typeof(PayActivities), EventSystemType.INIT)]
    public class PayActivitiesEventOnInit : ITEventMethodOnInit<PayActivities>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(PayActivities b_Component)
        {
            b_Component.OnInit();
        }
    }
    public static partial class PayActivitiesComponentSystem
    {
        public static void OnInit(this PayActivities b_Component)
        {
            
            var Json2 = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Activity_InfoConfigJson>().JsonDic;
            if (Json2 != null)
            {
                foreach (var Info in Json2)
                {
                    int ActivityID = Info.Key;
                    if(!b_Component.Parent.GetCustomComponent<ActivitiesComponent>().Activities.ContainsKey(ActivityID)) 
                    {
                        b_Component.Parent.GetCustomComponent<ActivitiesComponent>().CheckActivitTime(Json2[ActivityID].OpenTime, Json2[ActivityID].EndTime, ActivityID);
                    }
                }
            }
        }
    }
}