using System;
using System.Collections.Generic;
using System.Drawing;
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
    public static class PlayerActivityInfoSystemComponent
    {
        /// <summary>
        /// 角色加载活动信息
        /// </summary>
        /// <param name="b_Component"></param>
        public static async Task<bool> PlayerLoadActivityInfo(this PlayerActivitComponent b_Component)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Component.mPlayer.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheComponent.Get<DBMiracleActivities>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, b_Component.mPlayer.GameAreaId);
            if (mDataCache == null)
            {
                mDataCache = await mDataCacheComponent.Add<DBMiracleActivities>(dBProxy2, p => p.GameUesrID == b_Component.mPlayer.GameUserId && p.IsDisabled != 1);
            }

            var ShopInfo = mDataCache.DataQuery(p => p.GameUesrID == b_Component.mPlayer.GameUserId);
            if (ShopInfo.Count >= 1)
            {
                foreach (var Info in ShopInfo)
                {
                    //DBMiracleActivities dBMiracleActivities = new DBMiracleActivities();
                    //dBMiracleActivities = Info;
                    b_Component.MiracleActivitInfo.Add(Info.ID, Info);
                }
            }
            return true;
        }
        public static bool AddPlayerActivit(this PlayerActivitComponent b_Component, int ActivitId)
        {
            DBMiracleActivities dBMiracleActivities = new DBMiracleActivities();
            dBMiracleActivities.Id = IdGeneraterNew.Instance.GenerateUnitId(b_Component.mPlayer.GameAreaId);
            dBMiracleActivities.ID = ActivitId;
            dBMiracleActivities.GameUesrID = b_Component.mPlayer.GameUserId;
            dBMiracleActivities.Status = 0;
            dBMiracleActivities.Value32_A = 0;
            dBMiracleActivities.Value32_B = 0;
            dBMiracleActivities.Value64_A = 0;
            dBMiracleActivities.Value64_B = 0;

            b_Component.MiracleActivitInfo.Add(ActivitId, dBMiracleActivities);

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Component.mPlayer.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheComponent.Get<DBMiracleActivities>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, b_Component.mPlayer.GameAreaId);
            if (mDataCache == null) return false;

            var ActivitInfo = mDataCache.DataQuery(p => p.GameUesrID == b_Component.mPlayer.GameUserId && p.ID == ActivitId);
            if (ActivitInfo.Count <= 0)
            {
                mDataCache.DataAdd(dBMiracleActivities);
                dBProxy2.Save(dBMiracleActivities).Coroutine();
                return true;
            }
            return false;
        }
        /// <summary>
        ///对应活动获取对应的领奖状态
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="ActivitId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool GetPlayerActivitRewardState(this PlayerActivitComponent b_Component, int ActivitId, long type)
        {
            if (b_Component.MiracleActivitInfo.TryGetValue(ActivitId, out DBMiracleActivities Info) != false)
            {
                return (Info.Value64_A & type) == type;
            }
            else
            {
                Log.PLog("Activit", $"活动ID不存在 {ActivitId} 获取领奖状态失败");
                return true;
            }
        }
        /// <summary>
        /// 对应活动修改领奖状态
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="ActivitId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool SetPlayerActivitRewardState(this PlayerActivitComponent b_Component, int ActivitId, long type)
        {
            if (b_Component.MiracleActivitInfo.TryGetValue(ActivitId, out DBMiracleActivities Info) != false)
            {
                Info.Value64_A |= type;
                return true;
            }
            else
            {
                Log.PLog("Activit", $"活动ID不存在 {ActivitId} 设置状态失败");
                return false;
            }
        }
        public static void SetPlayerActivit(this PlayerActivitComponent b_Component, int ActivitId, struct_MiracleActivities struct_MiracleActivities)
        {
            if (b_Component.MiracleActivitInfo.ContainsKey(ActivitId))
            {
                b_Component.MiracleActivitInfo[ActivitId].ID = struct_MiracleActivities.ID;
                b_Component.MiracleActivitInfo[ActivitId].Status = struct_MiracleActivities.Status;
                // 32位数值A，根据活动内容代表数据
                b_Component.MiracleActivitInfo[ActivitId].Value32_A = struct_MiracleActivities.Value32A;
                // 32位数值B，根据活动内容代表数据
                b_Component.MiracleActivitInfo[ActivitId].Value32_B = struct_MiracleActivities.Value32B;
                // 64位数值A，根据活动内容代表数据
                b_Component.MiracleActivitInfo[ActivitId].Value64_A = struct_MiracleActivities.Value64A;
                // 64位数值B，根据活动内容代表数据
                b_Component.MiracleActivitInfo[ActivitId].Value64_B = struct_MiracleActivities.Value64B;

            }
        }
        public static void DBPlayerActivit(this PlayerActivitComponent b_Component, int ActivitId)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, b_Component.mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Component.mPlayer.GameAreaId);
            mWriteDataComponent.Save(b_Component.MiracleActivitInfo[ActivitId], dBProxy).Coroutine();
        }

        public static struct_MiracleActivities GetMiracleActivities(this PlayerActivitComponent b_Component, int ActivitId)
        {
            struct_MiracleActivities struct_MiracleActivities = new struct_MiracleActivities();
            if (b_Component.MiracleActivitInfo.TryGetValue((ActivitId), out DBMiracleActivities Info))
            {
                struct_MiracleActivities.ID = Info.ID;
                struct_MiracleActivities.Status = Info.Status;
                // 32位数值A，根据活动内容代表数据
                struct_MiracleActivities.Value32A = Info.Value32_A;
                // 32位数值B，根据活动内容代表数据
                struct_MiracleActivities.Value32B = Info.Value32_B;
                // 64位数值A，根据活动内容代表数据
                struct_MiracleActivities.Value64A = Info.Value64_A;
                // 64位数值B，根据活动内容代表数据
                struct_MiracleActivities.Value64B = Info.Value64_B;

                return struct_MiracleActivities;
            }
            else
            {
                b_Component.AddPlayerActivit(ActivitId);
                struct_MiracleActivities.ID = ActivitId;
                return struct_MiracleActivities;
            }
        }
    }
}
