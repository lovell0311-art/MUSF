using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static NPOI.HSSF.Util.HSSFColor;

namespace ETHotfix
{
    /// <summary>
    /// 背包装备管理类
    /// </summary>
    public static class KnapsackItemsManager
    {

        public static List<long> MedicineHpIdList = new List<long>() { 310002, 310003, 310004, 310045 };///治疗药水
        public static List<long> MedicineMpIdList = new List<long>() { 310005, 310006, 310007 };///魔力药水
        public static List<long> MountUUIDList = new List<long>();//背包中的坐骑
        ///坐骑材料
        public static Dictionary<long, string> MountMatDic = new Dictionary<long, string>()
        {
            { 320006,"破烂的铠甲片"},
            { 320007,"女神的智慧"},
            { 320008,"猛兽的脚甲"},
            { 320009,"碎角片"},
            { 320010,"折断的角"},
            { 260008,"炎狼兽之角"},
        };
        ///羽毛材料
        public static Dictionary<long, string> YuMaoDic = new Dictionary<long, string>()
        {
            { 320307,"天魔菲尼斯的羽毛"},
            { 320300,"加鲁达之羽"},
            { 320297,"洛克之羽"},
            { 320020,"神鹰之羽"},
            { 320129,"天魔菲尼斯的羽毛"},
        };
        /// <summary>
        /// 是否使用坐骑
        /// </summary>
        public static int UseMountId = 0;
        /// <summary>
        /// 背包中的物品 字典
        /// </summary>
        public static Dictionary<long, KnapsackDataItem> KnapsackItems = new Dictionary<long, KnapsackDataItem>();
        //等待回收列表
        public static List<long> WaitingForAutomaticRecyclings = new List<long>();

        /// <summary>
        /// 整理背包的间隔时间
        /// </summary>
        public static float PackBackpackTime = 0;
        //每隔30秒 可以整理一次
        public static float PackBackpackSpaceTime = 30;
        //是否正在整理背包
        public static bool IsPackBackpack = false;
        //掉落的物品集合
        public static List<long> DisDropItemList = new List<long>();
        //上一个物品是否拾取完成
        public static bool LastItemIsComplete = true;
        /// <summary>
        /// 仓库中的装备
        /// </summary>
        public static Dictionary<long, KnapsackDataItem> WareHouseItems = new Dictionary<long, KnapsackDataItem>();

        /// <summary>
        /// 仓库中的金币
        /// </summary>
        public static long WareGlodCoin;
        /// <summary>
        /// 仓库 扩容（默认11行）
        /// </summary>
        public static int WareHouseRows = 11;

        /// <summary>
        /// 根据配置表ID判断字典中是否存在这个值
        /// </summary>
        /// <param name="ConfigId"></param>
        /// <returns></returns>
        public static bool GetIncludeValue(long ConfigId)
        {
            var list = KnapsackItems.Values.ToList();
            foreach (var item in list)
            {
                if (KnapsackItems.ContainsKey(item.Id) == false) continue;
                if (item.ConfigId == ConfigId)
                {
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// 清理背包数据
        /// </summary>
        public static void ClearKnapsackItems()
        {
            KnapsackItems.Clear();

            MountUUIDList.Clear();

            GlobalDataManager.SevenDaysToRechargeDic.Clear();
            KnapsackTools.Clean();
        }


    }
}
