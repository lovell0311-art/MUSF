using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Org.BouncyCastle.Crypto.Tls;

namespace ETHotfix
{
    [FriendOf(typeof(CumulativeRechargeComponent))]
    public static class CumulativeRechargeComponentSystem
    {
        public const long statisticsStartTime = 1691424000; // 2023-08-08 00:00:00
        public static async Task<bool> Load(this CumulativeRechargeComponent self)
        {
            Player player = self.Parent;
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DBProxyComponent dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, player.GameAreaId);
            List<BsonDocument> results = await dBProxy.Aggregate<DBPlayerPayOrderInfo>(
                p => p.GUid == player.UserId &&
                p.Time > statisticsStartTime &&
                p.SuccessTime != 0 &&
                p.StatisticalAmount == true,
                BsonDocument.Parse(@"
{
    $group: {
      _id: null,
      totalAmount: { $sum: ""$Money"" }
    }
  }
"));
            if(results.Count == 0)
            {
                // 没有充值记录
                self.totalAmount = 0;
                return true;
            }

            self.totalAmount = results[0]["totalAmount"].AsInt32;
            return true;
        }
    }
}
