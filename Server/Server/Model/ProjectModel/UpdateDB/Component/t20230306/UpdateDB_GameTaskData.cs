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
    public static class UpdateDB_GameTaskData
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
            await RefreshDBGameTaskData();
        }

        private static async Task RefreshDBGameTaskData()
        {
            List<int> PlayerTypeList = new List<int>();
            PlayerTypeList.Add((int)E_GameOccupation.Spellsword);
            PlayerTypeList.Add((int)E_GameOccupation.Holyteacher);
            PlayerTypeList.Add((int)E_GameOccupation.SummonWarlock);

            CountMax = await DB.database.GetCollection<DBGamePlayerData>(typeof(DBGamePlayerData).Name).CountDocumentsAsync(p => p.IsDisposePlayer == 0 && PlayerTypeList.Contains(p.PlayerTypeId));
            Count = 0;
            IAsyncCursor<DBGamePlayerData> cursor = await DB.database.GetCollection<DBGamePlayerData>(typeof(DBGamePlayerData).Name).FindAsync(p => p.IsDisposePlayer == 0 && PlayerTypeList.Contains(p.PlayerTypeId));
            await cursor.ForEachAsync(async (number) =>
            {
                await RefreshDBGameTaskData_DBGameTaskData(number);
            });
        }

        private static async Task RefreshDBGameTaskData_DBGameTaskData(DBGamePlayerData gamePlayerData)
        {
            ++Count;
            Log.Console($"DBGameTaskData 更新进度 [{Count}/{CountMax}]");
            IAsyncCursor<DBGameTasksData> cursor = await DB.database.GetCollection<DBGameTasksData>(typeof(DBGameTasksData).Name).FindAsync(p => p.GameUserId == gamePlayerData.Id);
            List<DBGameTasksData> itemList = await cursor.ToListAsync();

            itemList.ForEach(async (item) =>
            {
                if (item.MainTask == null) return;
                if(item.MainTask.ConfigId == 102033)
                {
                    if(gamePlayerData.PlayerTypeId == (int)E_GameOccupation.Spellsword || gamePlayerData.PlayerTypeId == (int)E_GameOccupation.Holyteacher)
                    {
                        item.MainTask.ConfigId = 102034;
                    }
                    else
                    {
                        return;
                    }
                }
                else if (item.MainTask.ConfigId == 102034)
                {
                    if (gamePlayerData.PlayerTypeId == (int)E_GameOccupation.SummonWarlock)
                    {
                        item.MainTask.ConfigId = 102033;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
                await DB.database.GetCollection<ComponentWithId>(typeof(DBGameTasksData).Name).ReplaceOneAsync(p => p.Id == item.Id,item,new ReplaceOptions { IsUpsert = true });
            });
        }

    }
}
