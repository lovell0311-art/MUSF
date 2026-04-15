using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CustomFrameWork;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;


namespace ETModel.t20230306
{
    public static class UpdateDB_GamePlayerData
    {
        public static long CountMax = 0;
        public static long Count = 0;
        public static DBComponent DB = null;
        public static HashSet<long> AccountIdList = new HashSet<long>();

        public static async Task StartAsync()
        {
            try
            {
                await ET.TimerComponent.Instance.WaitAsync(1000);
                Log.Console("开始刷库...");
                await Start();
                Log.Console("刷库完成");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static async Task Start()
        {
            DB = Root.MainFactory.GetCustomComponent<DBComponent>();
            //await DisposeAllItem();
            // 任务
            //await RefreshDBGamePlayerData();
        }

        //private static async Task RefreshDBGamePlayerData()
        //{
        //    CountMax = await DB.database.GetCollection<DBGamePlayerData>(typeof(DBGamePlayerData).Name).CountDocumentsAsync(p => p.IsDisposePlayer == 0);
        //    Count = 0;
        //    IAsyncCursor<DBGamePlayerData> cursor = await DB.database.GetCollection<DBGamePlayerData>(typeof(DBGamePlayerData).Name).FindAsync(p => p.IsDisposePlayer == 0);
        //    await cursor.ForEachAsync(async (number) =>
        //    {
        //        await RefreshDBGamePlayerData_DBGamePlayerData(number);
        //    });
        //}

        //private static async Task RefreshDBGamePlayerData_DBGamePlayerData(DBGamePlayerData gamePlayerData)
        //{
        //    ++Count;
        //    Log.Console($"DBGamePlayerData 更新进度 [{Count}/{CountMax}]");

        //    if (AccountIdList.Contains(gamePlayerData.UserId)) return;  // 每个账号只送一次
        //    AccountIdList.Add(gamePlayerData.UserId);
        //    gamePlayerData.GoldCoin += 10000 * 1000;
        //    gamePlayerData.YuanbaoCoin += 1000;
        //    await DB.database.GetCollection<ComponentWithId>(typeof(DBGamePlayerData).Name).ReplaceOneAsync(p => p.Id == gamePlayerData.Id, gamePlayerData, new ReplaceOptions { IsUpsert = true });
        //}

    }
}
