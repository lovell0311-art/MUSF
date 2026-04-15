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


namespace ETModel.t20230228
{
    [BsonIgnoreExtraElements]
    public class Old_DBBackpackItem : DBBase
    {
        /// <summary>
        /// 所属玩家，对应DBGamePlayer的ID，Player的GameUserId
        /// </summary>
        public long GameUserId { get; set; }
        public int PageHeight;
        public int Width;
        public int Capacity;
        public int GameAreaId;
        //[itemUID,itemUID,...]
        public string ItemList;
        public int IsDispose { get; set; }   //0+代表已删除
    }

    [BsonIgnoreExtraElements]
    public class Old_DBWarehouseItem : DBBase
    {
        /// <summary>
        /// 所属玩家，对应DBGamePlayer的UserId，Player的UserId
        /// </summary>
        public long GameUserId { get; set; }
        public int PageHeight;
        public int Width;
        public int Capacity;
        /// <summary>
        /// 所在区服
        /// </summary>
        public int GameAreaId { get; set; } = 0;
        //[itemUID,itemUID,...]
        public string ItemList;
        public int IsDispose { get; set; }   //0+代表已删除
    }

    [BsonIgnoreExtraElements]
    public class Old_DBEquipmentItem : DBBase
    {
        /// <summary>
        /// 所属玩家，对应DBGamePlayer的ID，Player的GameUserId
        /// </summary>
        public long GameUserId { get; set; }
        public string ItemList;
    }


    [BsonIgnoreExtraElements]
    public class Old_DBGamePlayerData : DBBase
    {
        public long UserId { get; set; }
        public int GameAreaId { get; set; }

        public int GoldCoin { get; set; }

        public int WarehouseCoin { get; set; } = 0;
    }


    public static class UpdateDB_ItemData
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
            await RefreshDBBackpackItem();
            // 装备
            await RefreshDBEquipmentItem();
            // 仓库
            await RefreshDBWarehouseItem();
        }

        /// <summary>
        /// 销毁全部物品
        /// </summary>
        /// <returns></returns>
        private static async Task DisposeAllItem()
        {
            var builder = Builders<DBItemData>.Update;
            await DB.database.GetCollection<DBItemData>(typeof(DBItemData).Name).UpdateManyAsync(
                
                p => p.IsDispose != 0,
                builder.Set(p => p.IsDispose,1));
        }



        private static async Task RefreshDBBackpackItem()
        {
            CountMax = await DB.database.GetCollection<Old_DBBackpackItem>(typeof(DBBackpackItem).Name).CountDocumentsAsync(p=>true);
            Count = 0;
            IAsyncCursor<Old_DBBackpackItem> cursor = await DB.database.GetCollection<Old_DBBackpackItem>(typeof(DBBackpackItem).Name).FindAsync(p=>true);
            await cursor.ForEachAsync(async (number) =>
            {
                await RefreshDBBackpackItem_DBItemData(number);
            });
        }

        private static async Task RefreshDBBackpackItem_DBItemData(Old_DBBackpackItem dbBackpack)
        {
            ++Count;
            Log.Console($"DBBackpackItem 更新进度 [{Count}/{CountMax}]");
            List<long> itemListUid = BsonSerializer.Deserialize<List<long>>(dbBackpack.ItemList);
            IAsyncCursor<DBItemData> cursor = await DB.database.GetCollection<DBItemData>(typeof(DBItemData).Name).FindAsync(p => itemListUid.Contains(p.Id));
            List<DBItemData> itemList = await cursor.ToListAsync();

            itemList.ForEach(async (item) =>
            {
                item.InComponent = EItemInComponent.Backpack;
                item.GameUserId = dbBackpack.GameUserId;
                item.IsDispose = 0;
                await DB.database.GetCollection<ComponentWithId>(typeof(DBItemData).Name).ReplaceOneAsync(p => p.Id == item.Id,item,new ReplaceOptions { IsUpsert = true });
            });
        }


        private static async Task RefreshDBEquipmentItem()
        {
            DB = Root.MainFactory.GetCustomComponent<DBComponent>();
            CountMax = await DB.database.GetCollection<Old_DBEquipmentItem>(typeof(DBEquipmentItem).Name).CountDocumentsAsync(p => true);
            Count = 0;
            IAsyncCursor<Old_DBEquipmentItem> cursor = await DB.database.GetCollection<Old_DBEquipmentItem>(typeof(DBEquipmentItem).Name).FindAsync(p => true);
            await cursor.ForEachAsync(async (number) =>
            {
                await RefreshDBEquipmentItem_DBItemData(number);
            });
        }

        private static async Task RefreshDBEquipmentItem_DBItemData(Old_DBEquipmentItem equipment)
        {
            ++Count;
            Log.Console($"DBEquipmentItem 更新进度 [{Count}/{CountMax}]");
            Dictionary<int, long> temp = BsonSerializer.Deserialize<Dictionary<int, long>>(equipment.ItemList);
            Dictionary<long, int> dbItemDict = new Dictionary<long, int>();
            foreach (var kv in temp)
            {
                dbItemDict.Add(kv.Value, kv.Key);
            }
            List<long> Keys = dbItemDict.Keys.ToList();
            IAsyncCursor<DBItemData> cursor = await DB.database.GetCollection<DBItemData>(typeof(DBItemData).Name).FindAsync(p => Keys.Contains(p.Id));
            List<DBItemData> itemList = await cursor.ToListAsync();

            itemList.ForEach(async (item) =>
            {
                item.InComponent = EItemInComponent.Equipment;
                item.GameUserId = equipment.GameUserId;
                dbItemDict.TryGetValue(item.Id, out int posId);
                item.posId = posId;
                item.IsDispose = 0;
                await DB.database.GetCollection<ComponentWithId>(typeof(DBItemData).Name).ReplaceOneAsync(p => p.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
            });
        }

        private static async Task RefreshDBWarehouseItem()
        {
            DB = Root.MainFactory.GetCustomComponent<DBComponent>();
            CountMax = await DB.database.GetCollection<Old_DBWarehouseItem>(typeof(DBWarehouseItem).Name).CountDocumentsAsync(p => true);
            Count = 0;
            IAsyncCursor<Old_DBWarehouseItem> cursor = await DB.database.GetCollection<Old_DBWarehouseItem>(typeof(DBWarehouseItem).Name).FindAsync(p => true);
            await cursor.ForEachAsync(async (number) =>
            {
                await RefreshDBWarehouseItem_DBItemData(number);
            });
        }

        private static async Task RefreshDBWarehouseItem_DBItemData(Old_DBWarehouseItem warehouse)
        {
            ++Count;
            Log.Console($"DBWarehouseItem 更新进度 [{Count}/{CountMax}]");
            if (string.IsNullOrEmpty(warehouse.ItemList))
            {
                Log.Warning($"warehouse.GameUserId = {warehouse.GameUserId}, warehouse.ItemList == null");
                return;
            }
            List<long> itemListUid = BsonSerializer.Deserialize<List<long>>(warehouse.ItemList);
            IAsyncCursor<DBItemData> cursor = await DB.database.GetCollection<DBItemData>(typeof(DBItemData).Name).FindAsync(p => itemListUid.Contains(p.Id));
            List<DBItemData> itemList = await cursor.ToListAsync();

            IAsyncCursor<ETModel.DBGamePlayerData> cursorGamePlayerData = await DB.database.GetCollection<ETModel.DBGamePlayerData>(typeof(ETModel.DBGamePlayerData).Name).FindAsync(p => p.Id == warehouse.GameUserId);
            List<ETModel.DBGamePlayerData> gamePlayerDataList = await cursorGamePlayerData.ToListAsync();
            if(gamePlayerDataList.Count == 0)
            {
                Log.Warning($"warehouse.GameUserId = {warehouse.GameUserId}, DBGamePlayerData 集合不存在");
                return;
            }
            itemList.ForEach(async (item) =>
            {
                item.InComponent = EItemInComponent.Warehouse;
                item.UserId = gamePlayerDataList[0].UserId;
                item.GameUserId = gamePlayerDataList[0].Id;
                item.IsDispose = 0;
                await DB.database.GetCollection<ComponentWithId>(typeof(DBItemData).Name).ReplaceOneAsync(p => p.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
            });

            // 更新WarehouseItem
            IAsyncCursor<ETModel.DBWarehouseItem> cursorNewWarehouse = await DB.database.GetCollection<ETModel.DBWarehouseItem>(typeof(ETModel.DBWarehouseItem).Name).FindAsync(p => p.UserId == gamePlayerDataList[0].UserId);
            List<ETModel.DBWarehouseItem> newWarehouseList = await cursorNewWarehouse.ToListAsync();
            if(newWarehouseList.Count == 0)
            {
                // 没有存档，创建一个新的
                IAsyncCursor<Old_DBGamePlayerData> cursorGamePlayerData2 = await DB.database.GetCollection<Old_DBGamePlayerData>(typeof(DBGamePlayerData).Name).FindAsync(p => p.UserId == gamePlayerDataList[0].UserId);
                List<Old_DBGamePlayerData> gamePlayerDataList2 = await cursorGamePlayerData2.ToListAsync();
                int warehouseCoin = 0;
                gamePlayerDataList2.ForEach(item =>
                {
                    warehouseCoin += item.WarehouseCoin;
                });

                ETModel.DBWarehouseItem newDBWarehouseItem = ComponentFactory.Create<ETModel.DBWarehouseItem>();
                newDBWarehouseItem.UserId = gamePlayerDataList[0].UserId;
                newDBWarehouseItem.PageHeight = 0;
                newDBWarehouseItem.Width = 8;
                newDBWarehouseItem.Capacity = 88;
                newDBWarehouseItem.Coin = warehouseCoin;
                newDBWarehouseItem.GameAreaId = gamePlayerDataList[0].GameAreaId;
                newDBWarehouseItem.IsDispose = 0;
                await DB.database.GetCollection<ComponentWithId>(typeof(ETModel.DBWarehouseItem).Name).ReplaceOneAsync(p => p.Id == newDBWarehouseItem.Id, newDBWarehouseItem, new ReplaceOptions { IsUpsert = true });
            }
        }


    }
}
