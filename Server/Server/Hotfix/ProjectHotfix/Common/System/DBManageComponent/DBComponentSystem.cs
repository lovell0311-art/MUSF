using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ETModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using CustomFrameWork;

namespace ETHotfix
{
    [EventMethod("DBComponent")]
    public class DBComponentEventOnRun : ITEventMethodOnRun<DBComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnRun(DBComponent b_Component)
        {
            b_Component.OnRun();
        }
    }
    /// <summary>
    /// 用来缓存数据
    /// </summary>
    public static class DBComponentSystem
    {
        public static void OnInit(this DBComponent b_Component, int b_DBType, int b_DBZone, int b_taskCountMax = 32)
        {
            var mConfigs = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Server_DataConfigJson>().JsonDic.Values.ToArray();
   
            for (int i = 0, len = mConfigs.Length; i < len; i++)
            {
                var mStartZoneConfigs = mConfigs[i];

                if (OptionComponent.Options.AppId == mStartZoneConfigs.AppId //是我这个服务器
                    && b_DBZone == mStartZoneConfigs.DBZone   //对应区
                    && (int)b_DBType == mStartZoneConfigs.DBType)  //对应数据库类型
                {
                    if (mStartZoneConfigs.DBConnection == "")
                    {
                        throw new Exception($"type:{b_DBType}   zone: {b_DBZone} not found mongo connect string");
                    }

                    b_Component.SetDBInfo(mStartZoneConfigs.Id, (DBType)mStartZoneConfigs.DBType, mStartZoneConfigs.DBZone);
                    b_Component.Init(mStartZoneConfigs.DBConnection, mStartZoneConfigs.DBName, b_taskCountMax);
                    break;
                }
            }
        }
        public static void OnRun(this DBComponent b_Component, int b_taskCountMax = 32)
        {
            if (OptionComponent.Options.AppType != AppType.DB && OptionComponent.Options.AppType != AppType.UpdateDB)
            {
                throw new Exception($"Appid:{OptionComponent.Options.AppId} dont's db server");
            }

            var mConfigs = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Server_DataConfigJson>().JsonDic.Values.ToArray();
            for (int i = 0, len = mConfigs.Length; i < len; i++)
            {
                var mStartZoneConfigs = mConfigs[i];

                if (OptionComponent.Options.AppId == mStartZoneConfigs.AppId)//是我这个服务器
                {
                    if (mStartZoneConfigs.DBConnection == "")
                    {
                        throw new Exception($"Appid:{OptionComponent.Options.AppId}  not found mongo connect string");
                    }

                    b_Component.SetDBInfo((int)mStartZoneConfigs.Id, (DBType)mStartZoneConfigs.DBType, mStartZoneConfigs.DBZone);
                    b_Component.Init(mStartZoneConfigs.DBConnection, mStartZoneConfigs.DBName, b_taskCountMax);
                    break;
                }
            }
        }

        public static Task<bool> Add(this DBComponent self, ComponentWithId component, string collectionName = "")
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = component.GetType().Name;
            }
            DBSaveTask task = ComponentFactory.CreateWithId<DBSaveTask, ComponentWithId, string, TaskCompletionSource<bool>>(component.Id, component, collectionName, tcs);
            self.tasks[(int)((ulong)task.Id % self.taskCount)].Add(task);

            return tcs.Task;
        }

        public static Task AddBatch(this DBComponent self, List<ComponentWithId> components, string collectionName)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            DBSaveBatchTask task = ComponentFactory.Create<DBSaveBatchTask, List<ComponentWithId>, string, TaskCompletionSource<bool>>(components, collectionName, tcs);
            self.tasks[(int)((ulong)task.Id % self.taskCount)].Add(task);
            return tcs.Task;
        }

        public static Task<ComponentWithId> Get(this DBComponent self, string collectionName, long id)
        {
            TaskCompletionSource<ComponentWithId> tcs = new TaskCompletionSource<ComponentWithId>();
            DBQueryTask dbQueryTask = ComponentFactory.CreateWithId<DBQueryTask, string, TaskCompletionSource<ComponentWithId>>(id, collectionName, tcs);
            self.tasks[(int)((ulong)id % self.taskCount)].Add(dbQueryTask);

            return tcs.Task;
        }

        public static Task<List<ComponentWithId>> GetBatch(this DBComponent self, string collectionName, List<long> idList)
        {
            TaskCompletionSource<List<ComponentWithId>> tcs = new TaskCompletionSource<List<ComponentWithId>>();
            DBQueryBatchTask dbQueryBatchTask = ComponentFactory.Create<DBQueryBatchTask, List<long>, string, TaskCompletionSource<List<ComponentWithId>>>(idList, collectionName, tcs);
            self.tasks[(int)((ulong)dbQueryBatchTask.Id % self.taskCount)].Add(dbQueryBatchTask);

            return tcs.Task;
        }

        public static Task<List<ComponentWithId>> GetJson(this DBComponent self, string collectionName, string json, int b_Count = 0)
        {
            TaskCompletionSource<List<ComponentWithId>> tcs = new TaskCompletionSource<List<ComponentWithId>>();

            DBQueryJsonTask dbQueryJsonTask = ComponentFactory.Create<DBQueryJsonTask, string, string, TaskCompletionSource<List<ComponentWithId>>>(collectionName, json, tcs);
            dbQueryJsonTask.Count = b_Count;
            self.tasks[(int)((ulong)dbQueryJsonTask.Id % self.taskCount)].Add(dbQueryJsonTask);

            return tcs.Task;
        }


        public static Task<long> GetCount(this DBComponent self, string collectionName, string json)
        {
            TaskCompletionSource<long> tcs = new TaskCompletionSource<long>();

            DBGetCountTask dbGetCountTask = ComponentFactory.Create<DBGetCountTask, string, string, TaskCompletionSource<long>>(collectionName, json, tcs, false);
            self.tasks[(int)((ulong)dbGetCountTask.Id % self.taskCount)].Add(dbGetCountTask);

            return tcs.Task;
        }

        public static Task<long> GetCount<T>(this DBComponent self, Expression<Func<T, bool>> exp, string collectionName = "") where T : ComponentWithId
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = typeof(T).Name;
            }
            ExpressionFilterDefinition<T> filter = new ExpressionFilterDefinition<T>(exp);
            IBsonSerializerRegistry serializerRegistry = BsonSerializer.SerializerRegistry;
            IBsonSerializer<T> documentSerializer = serializerRegistry.GetSerializer<T>();
            string json = filter.Render(documentSerializer, serializerRegistry).ToJson();
            return self.GetCount(collectionName, json);
        }


        public static Task<ComponentWithId> UpdateAtomicallyInc<T>(this DBComponent self, Expression<Func<T, bool>> exp, Expression<Func<ComponentWithId, long>> field, long fieldValue, string collectionName = "") where T : ComponentWithId
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = typeof(T).Name;
            }

            ExpressionFilterDefinition<T> filter = new ExpressionFilterDefinition<T>(exp);
            IBsonSerializerRegistry serializerRegistry = BsonSerializer.SerializerRegistry;
            IBsonSerializer<T> documentSerializer = serializerRegistry.GetSerializer<T>();
            string json = filter.Render(documentSerializer, serializerRegistry).ToJson();

            var updateDefinition = Builders<ComponentWithId>.Update.Inc(field, fieldValue);

            TaskCompletionSource<ComponentWithId> tcs = new TaskCompletionSource<ComponentWithId>();

            DBUpdateAtomicallyTask dbUpdateAtomicallyTask = ComponentFactory.Create<DBUpdateAtomicallyTask, string, string, UpdateDefinition<ComponentWithId>, TaskCompletionSource<ComponentWithId>, DBComponent>(collectionName, json, updateDefinition, tcs, self, false);
            self.tasks[(int)((ulong)dbUpdateAtomicallyTask.Id % self.taskCount)].Add(dbUpdateAtomicallyTask);

            return tcs.Task;
        }

        public static Task<ComponentWithId> UpdateAtomically<T>(this DBComponent self, Expression<Func<T, bool>> exp, BsonDocument updates, string collectionName = "") where T : ComponentWithId
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = typeof(T).Name;
            }

            ExpressionFilterDefinition<T> filter = new ExpressionFilterDefinition<T>(exp);
            IBsonSerializerRegistry serializerRegistry = BsonSerializer.SerializerRegistry;
            IBsonSerializer<T> documentSerializer = serializerRegistry.GetSerializer<T>();
            string json = filter.Render(documentSerializer, serializerRegistry).ToJson();

            TaskCompletionSource<ComponentWithId> tcs = new TaskCompletionSource<ComponentWithId>();

            DBUpdateAtomicallyTask dbUpdateAtomicallyTask = ComponentFactory.Create<DBUpdateAtomicallyTask, string, string, UpdateDefinition<ComponentWithId>, TaskCompletionSource<ComponentWithId>, DBComponent>(collectionName, json, updates, tcs, self, false);
            self.tasks[(int)((ulong)dbUpdateAtomicallyTask.Id % self.taskCount)].Add(dbUpdateAtomicallyTask);

            return tcs.Task;
        }

        public static Task<bool> UpdateOne(this DBComponent self, BsonDocument filter, BsonDocument updates, string collectionName)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            DBUpdateOneTask dbUpdateAtomicallyTask = ComponentFactory.Create<DBUpdateOneTask, string, FilterDefinition<ComponentWithId>, UpdateDefinition<ComponentWithId>, TaskCompletionSource<bool>, DBComponent>(collectionName, filter, updates, tcs, self, false);
            self.tasks[(int)((ulong)dbUpdateAtomicallyTask.Id % self.taskCount)].Add(dbUpdateAtomicallyTask);

            return tcs.Task;
        }

        public static Task<List<BsonDocument>> Aggregate(this DBComponent self, List<BsonDocument> pipeline, string collectionName)
        {
            TaskCompletionSource<List<BsonDocument>> tcs = new TaskCompletionSource<List<BsonDocument>>();

            DBAggregateTask dbAggregateTask = ComponentFactory.Create<DBAggregateTask, string, PipelineDefinition<ComponentWithId, BsonDocument>, TaskCompletionSource<List<BsonDocument>>, DBComponent>(collectionName, pipeline, tcs, self, false);
            self.tasks[(int)((ulong)dbAggregateTask.Id % self.taskCount)].Add(dbAggregateTask);
            return tcs.Task;
        }

    }
}
