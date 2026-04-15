using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using Microsoft.VisualBasic;
using TencentCloud.Bri.V20190328.Models;
using TencentCloud.Mgobe.V20201014.Models;

namespace ETModel
{
    public class StructActivit
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityID { get; set; } = 0;
        /// <summary>
        /// 开启时间
        /// </summary>
        public long OpenTime { get; set; } = 0;
        /// <summary>
        /// 结束时间
        /// </summary>
        public long EndTime { get; set; } = 0;
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool IsOpen { get; set; } = false;
    }
    public class StructItem
    {
        public int ItemID { get; set; } = 0;
        public ItemCreateAttr ItemInfo { get; set; } = new ItemCreateAttr();
    }
    public class RushGradeRewardItem
    {
        public int RewardID { get; set; } = 0;
        public int RewardLimit { get; set; } = 0;
        public List<StructItem> ItemList = new List<StructItem>();
    }
    public class ActivitiesComponent : TCustomComponent<C_ServerArea>
    {
        /// <summary>
        /// <活动ID，StructActivit>
        /// </summary>
        public long UpDataTime { get; set; }
        public Dictionary<int, StructActivit> Activities;
        public override void Awake()
        {
            Activities = new Dictionary<int, StructActivit>();
            UpDataTime = Help_TimeHelper.GetNowSecond();
        }
        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }
            Activities.Clear();
        }
    }

    public class RushGradeActivity : TCustomComponent<C_ServerArea>
    {
        public int ActivitID { get; set; } = 0;
        public Dictionary<int, RushGradeRewardItem> ItemList = new Dictionary<int, RushGradeRewardItem> ();

        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }
            ItemList.Clear();
        }
    }
    //public class MobInfo
    //{ 
    //    public long MobID { get; set; }
    //    public bool IsDet { get; set; }
    //    public int X { get; set; }
    //    public int Y { get; set; }
    //    public bool IsProp { get; set; }
    //}
    public class NewYearAnimalActivity : TCustomComponent<C_ServerArea>
    {
        public int ActivitID { get; set; } = 0;
        public bool IsBrushMonster { get; set; } = true;
        public (int,int) RecycleTime { get; set; } = (0,0);
        public Dictionary<int,int> MobCnt = new Dictionary<int, int> ();
        //public Dictionary<int, Dictionary<long,MobInfo>> MobList = new Dictionary<int, Dictionary<long, MobInfo>>();
        public Dictionary<int, List<Enemy>> Mob_LIst = new Dictionary<int, List<Enemy>>();
        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }
            //MobList.Clear();
            Mob_LIst.Clear ();
            ActivitID = 0;
            RecycleTime = (0, 0);
            MobCnt.Clear();
            IsBrushMonster = false;
        }
    }
    public class PayActivities : TCustomComponent<C_ServerArea>
    {
        public int ActivitID { get; set; } = 0;
        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }
            ActivitID = 0;
        }
    }
    public class CitySiegeActivities : TCustomComponent<C_ServerArea>
    {
        /// <summary>
        /// 王座的角色ID
        /// </summary>
        public long SupremeThrone { get; set; } = 0;
        /// <summary>
        /// 王座的角色名
        /// </summary>
        public string SupremeThroneName { get; set; } = "";
        public string WarAlliance { get; set; } = "";
        /// <summary>
        /// 座上王座计时
        /// </summary>
        public long SupremeThroneTiem { get; set; } = 0;
        public long LeaveTiem { get; set; } = 0;
        public bool HaveInHand { get; set; } = false;
        /*测试数据*/
        //public Dictionary<int,int> OpenWkList = new Dictionary<int, int>() { {1,1},{ 2,2},{ 3,3}, { 4, 4 }, { 5, 5 }, { 6, 6 }, { 7, 7 } };
        //public Dictionary<int,int> OpenHList = new Dictionary<int,int>() { { 1, 10 }, { 2, 11 }, { 3, 12 }, { 4, 15 }, { 5, 16 }, { 6, 17 }, { 7, 18 } ,{ 8, 19 } };
        //public Dictionary<int, int> OpenMList = new Dictionary<int, int>();
        //public int Cnt = 1;
        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }
            SupremeThrone = 0;
            SupremeThroneTiem = 0;
            HaveInHand =false;
            SupremeThroneName = "";
            LeaveTiem=0;
            WarAlliance = "";
        }
    }
}