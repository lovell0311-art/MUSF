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


namespace ETModel.t20231214
{

    public static class UpdateDB_ItemData_SetId
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
            // 背包
            await RefreshDBItemData_SetId();
        }

        private static async Task RefreshDBItemData_SetId()
        {
            List<int> itemConfigIds = new List<int>()
            {
                30001, 170005, 180005, 190005, 200005, 210005, 240010,
                160000,
                160009, 170006, 180006, 190006, 230011,
                200006, 210006,
                180008,190008,200008,210008,230012,
                170008,
                180009,200009,210009,240011,240006,
                170009,190009
            };

            Dictionary<int, int> setId2SetId = new Dictionary<int, int>()
            {
                { 6000,1},
                { 6001,2},
                { 6002,5},
                { 6003,6},
                { 6004,7},
                { 6005,8},
                { 6006,9},
                { 6007,10},
            };
            CountMax = await DB.database.GetCollection<DBItemData>(typeof(DBItemData).Name).CountDocumentsAsync(p => p.IsDispose == 0 && itemConfigIds.Contains(p.ConfigID));

            Log.Console($"DBItemData 更新进度 [{Count}/{CountMax}]");
            IAsyncCursor<DBItemData> cursor = await DB.database.GetCollection<DBItemData>(typeof(DBItemData).Name).FindAsync(p => p.IsDispose == 0 && itemConfigIds.Contains(p.ConfigID));
            List<DBItemData> itemList = await cursor.ToListAsync();
            int updateCount = 0;
            itemList.ForEach(async (item) =>
            {
                Count++;
                Log.Console($"DBItemData 更新进度 [{Count}/{CountMax}]");
                if (item.PropertyData.TryGetValue((int)EItemValue.SetId,out int val))
                {
                    if(setId2SetId.TryGetValue(val,out int newVal))
                    {
                        updateCount++;
                        item.PropertyData[(int)EItemValue.SetId] = newVal;
                        await DB.database.GetCollection<ComponentWithId>(typeof(DBItemData).Name).ReplaceOneAsync(p => p.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
                    }
                }
            });
            Log.Console($"DBItemData 修复 {updateCount}");
        }
    }
}
