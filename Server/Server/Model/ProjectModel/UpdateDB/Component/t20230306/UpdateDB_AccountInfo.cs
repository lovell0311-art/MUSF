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
    public static class UpdateDB_AccountInfo
    {
        public static long CountMax = 0;
        public static long Count = 0;
        public static DBComponent DB = null;
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
            await RefreshDBAccountInfo();
        }

        private static async Task RefreshDBAccountInfo()
        {
            CountMax = await DB.database.GetCollection<DBAccountInfo>(typeof(DBAccountInfo).Name).CountDocumentsAsync(p => p.Disabled == 0);
            Count = 0;
            IAsyncCursor<DBAccountInfo> cursor = await DB.database.GetCollection<DBAccountInfo>(typeof(DBAccountInfo).Name).FindAsync(p => p.Disabled == 0);
            await cursor.ForEachAsync(async (number) =>
            {
                await RefreshDBAccountInfo_DBAccountInfo(number);
            });
        }

        private static async Task RefreshDBAccountInfo_DBAccountInfo(DBAccountInfo accountInfo)
        {
            ++Count;
            Log.Console($"DBAccountInfo 更新进度 [{Count}/{CountMax}]");

            accountInfo.Password = MD5Helper.GetMD5Hash(accountInfo.Password);
            await DB.database.GetCollection<ComponentWithId>(typeof(DBAccountInfo).Name).ReplaceOneAsync(p => p.Id == accountInfo.Id, accountInfo, new ReplaceOptions { IsUpsert = true });
        }

    }
}
