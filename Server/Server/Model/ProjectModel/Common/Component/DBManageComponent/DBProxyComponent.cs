using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using CustomFrameWork;

namespace ETModel
{

    /// <summary>
    /// 用来缓存数据
    /// </summary>
    public partial class DBProxyComponent : DBProxyComponentBase
    {
        public const int CommonDBId = 0;

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBType DBType { get; private set; }

        public int AreaId { get; private set; }
        public int DBId { get; private set; }

        public void SetDBInfo(int b_id, DBType b_DbType, int b_DBZone)
        {
            this.DBId = b_id;
            this.DBType = b_DbType;
            this.AreaId = b_DBZone;
        }
    }
    public partial class DBProxyComponent
    {

        public async Task<bool> Save(ComponentWithId component)
        {
            // TODO 数据落地调试日志
            Log.Info($"#{component.GetType().Name}# {component.ToJson()}");
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(this.dbAddress);
            var mResponse = await session.Call(new DBSaveRequest { Component = component });
            return mResponse.Error == 0;
        }

        public async Task<bool> SaveT<T>(T component) where T : ComponentWithId
        {
            return await Save(component);
        }

        public async Task SaveBatch(List<ComponentWithId> components)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(this.dbAddress);
            await session.Call(new DBSaveBatchRequest { Components = components });
        }

        public async Task Save(ComponentWithId component, CancellationToken cancellationToken)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(this.dbAddress);
            await session.Call(new DBSaveRequest { Component = component }, cancellationToken);
        }

        public async Task SaveLog(ComponentWithId component)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(this.dbAddress);
            await session.Call(new DBSaveRequest { Component = component, CollectionName = "Log" });
        }

        public Task<T> Query<T>(long id) where T : ComponentWithId
        {
            string key = typeof(T).Name + id;
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            if (this.TcsQueue.ContainsKey(key))
            {
                this.TcsQueue.Add(key, tcs);
                return tcs.Task;
            }

            this.TcsQueue.Add(key, tcs);
            this.QueryInner<T>(id, key).Coroutine();
            return tcs.Task;
        }

        private async Task QueryInner<T>(long id, string key) where T : ComponentWithId
        {
            try
            {
                Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(this.dbAddress);
                DBQueryResponse dbQueryResponse = (DBQueryResponse)await session.Call(new DBQueryRequest { CollectionName = typeof(T).Name, Id = id });
                T result = (T)dbQueryResponse.Component;

                object[] tcss = this.TcsQueue.GetAll(key);
                this.TcsQueue.Remove(key);

                foreach (TaskCompletionSource<T> tcs in tcss)
                {
                    tcs.SetResult(result);
                }
            }
            catch (Exception e)
            {
                object[] tcss = this.TcsQueue.GetAll(key);
                this.TcsQueue.Remove(key);

                foreach (TaskCompletionSource<T> tcs in tcss)
                {
                    tcs.SetException(e);
                }
            }
        }

        /// <summary>
        /// 根据查询表达式查询
        /// </summary>
        /// <param name="self"></param>
        /// <param name="exp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<List<ComponentWithId>> Query<T>(Expression<Func<T, bool>> exp, int b_Count = 0) where T : ComponentWithId
        {
            ExpressionFilterDefinition<T> filter = new ExpressionFilterDefinition<T>(exp);
            IBsonSerializerRegistry serializerRegistry = BsonSerializer.SerializerRegistry;
            IBsonSerializer<T> documentSerializer = serializerRegistry.GetSerializer<T>();
            string json = filter.Render(documentSerializer, serializerRegistry).ToJson();
            return await this.Query<T>(json, b_Count);
        }

        public async Task<List<ComponentWithId>> Query<T>(List<long> ids) where T : ComponentWithId
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(this.dbAddress);
            DBQueryBatchResponse dbQueryBatchResponse = (DBQueryBatchResponse)await session.Call(new DBQueryBatchRequest { CollectionName = typeof(T).Name, IdList = ids });
            return dbQueryBatchResponse.Components;
        }

        /// <summary>
        /// 根据json查询条件查询
        /// </summary>
        /// <param name="self"></param>
        /// <param name="json"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<List<ComponentWithId>> Query<T>(string json, int b_Count = 0) where T : ComponentWithId
        {
            string key = typeof(T).Name + json;
            TaskCompletionSource<List<ComponentWithId>> tcs = new TaskCompletionSource<List<ComponentWithId>>();
            if (this.TcsQueue.ContainsKey(key))
            {
                this.TcsQueue.Add(key, tcs);
                return tcs.Task;
            }

            this.TcsQueue.Add(key, tcs);
            this.QueryInner<T>(json, key, b_Count).Coroutine();
            return tcs.Task;
        }

        private async Task QueryInner<T>(string json, string key, int b_Count = 0) where T : ComponentWithId
        {
            try
            {
                Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(this.dbAddress);
                Log.Info($"#DBProxy# Query start collection={typeof(T).Name} json={json} db={this.dbAddress}");
                DBQueryJsonResponse dbQueryJsonResponse = (DBQueryJsonResponse)await session.Call(new DBQueryJsonRequest { CollectionName = typeof(T).Name, Json = json, Count = b_Count });
                Log.Info($"#DBProxy# Query finish collection={typeof(T).Name} count={dbQueryJsonResponse.Components?.Count ?? 0} db={this.dbAddress}");

                var result = dbQueryJsonResponse.Components;

                object[] tcss = this.TcsQueue.GetAll(key);
                this.TcsQueue.Remove(key);

                foreach (TaskCompletionSource<List<ComponentWithId>> tcs in tcss)
                {
                    tcs.SetResult(result);
                }
            }
            catch (Exception e)
            {
                Log.Error($"#DBProxy# Query error collection={typeof(T).Name} json={json} db={this.dbAddress}\n{e}");
                object[] tcss = this.TcsQueue.GetAll(key);
                this.TcsQueue.Remove(key);

                foreach (TaskCompletionSource<List<ComponentWithId>> tcs in tcss)
                {
                    tcs.SetException(e);
                }
            }
        }


        public async Task<bool> UpdateOneSet<T>(Expression<Func<T, bool>> exp, Dictionary<string, object> updates) where T : ComponentWithId
        {
            ExpressionFilterDefinition<T> filter = new ExpressionFilterDefinition<T>(exp);
            IBsonSerializerRegistry serializerRegistry = BsonSerializer.SerializerRegistry;
            IBsonSerializer<T> documentSerializer = serializerRegistry.GetSerializer<T>();
            BsonDocument bsonDocFilter = filter.Render(documentSerializer, serializerRegistry);
            BsonDocument bsonDocUpdates = new BsonDocument("$set", new BsonDocument(updates));
            return await this.UpdateOneInner<T>(
                bsonDocFilter,
                bsonDocUpdates);
        }

        private async Task<bool> UpdateOneInner<T>(BsonDocument filter, BsonDocument updates) where T : ComponentWithId
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(this.dbAddress);
            DBUpdateOneResponse dbUpdateOneResponse = (DBUpdateOneResponse)await session.Call(new DBUpdateOneRequest { CollectionName = typeof(T).Name, Filter = filter, Updates = updates });
            return dbUpdateOneResponse.Error == ErrorCode.ERR_Success;
        }

        public async Task<List<BsonDocument>> Aggregate<T>(Expression<Func<T, bool>> match, BsonDocument group) where T : ComponentWithId
        {
            ExpressionFilterDefinition<T> filter = new ExpressionFilterDefinition<T>(match);
            IBsonSerializerRegistry serializerRegistry = BsonSerializer.SerializerRegistry;
            IBsonSerializer<T> documentSerializer = serializerRegistry.GetSerializer<T>();
            BsonDocument matchDoc = filter.Render(documentSerializer, serializerRegistry);
            using ListComponent<BsonDocument> pipeline = ListComponent<BsonDocument>.Create();
            pipeline.Add(new BsonDocument("$match", matchDoc));
            pipeline.Add(group);
            return await AggregateInner<T>(pipeline);
        }
        public async Task<List<BsonDocument>> Aggregate<T>(List<BsonDocument> pipeline) where T : ComponentWithId
        {
            return await AggregateInner<T>(pipeline);
        }
        private async Task<List<BsonDocument>> AggregateInner<T>(List<BsonDocument> pipeline) where T : ComponentWithId
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(this.dbAddress);
            DBAggregateResponse dbAggregateResponse = (DBAggregateResponse)await session.Call(new DBAggregateRequest { CollectionName = typeof(T).Name, PipeLine = pipeline });
            return dbAggregateResponse.Result;
        }
    }
}
