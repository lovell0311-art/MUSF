using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public static class LotteryComponentSystem
    {
        public static async Task<bool> Init(this LotteryComponent self)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = self.Parent.AddCustomComponent<DataCacheManageComponent>();

            var lotteryDataCache = mDataCacheComponent.Get<DBLotteryData>();
            if (lotteryDataCache == null)
            {
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, self.Parent.GameAreaId);
                lotteryDataCache = await mDataCacheComponent.Add<DBLotteryData>(dBProxy, p => p.Id == self.Parent.GameUserId
                                                                                && p.GameAreaId == self.Parent.GameAreaId);
            }

            var dataList = lotteryDataCache.DataQuery(p => p.Id == self.Parent.GameUserId
                                                                                && p.GameAreaId == self.Parent.GameAreaId);

            if(dataList.Count == 0)
            {
                self.Data = new DBLotteryData()
                {
                    Id = self.Parent.GameUserId,
                    GameAreaId = self.Parent.GameAreaId,
                };
            }
            else
            {
                self.Data = dataList[0];
            }
            return true;
        }

        public static void SaveDB(this LotteryComponent self)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)self.Parent.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(self.Parent.GameAreaId);
            mWriteDataComponent.Save(self.Data, dBProxy).Coroutine();
        }

    }
}
