using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using ETModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    /// <summary>
    /// 用来与数据库操作代理
    /// </summary>
    public static class DBProxyComponentEx
    {
        public static void OnInit(this DBProxyComponent self, int b_DBType, int b_DBZone)
        {
            if (OptionComponent.Options.AppType == AppType.DB)
            {
                throw new Exception($"Appid:{OptionComponent.Options.AppId} is db server");
            }

            var mConfigs = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Server_DataConfigJson>().JsonDic.Values.ToArray();
            for (int i = 0, len = mConfigs.Length; i < len; i++)
            {
                var mStartZoneConfigs = mConfigs[i];

                if (b_DBZone == mStartZoneConfigs.DBZone   //对应区
                    && (int)b_DBType == mStartZoneConfigs.DBType)  //对应数据库类型
                {
                    if (mStartZoneConfigs.DBConnection == "")
                    {
                        throw new Exception($"type:{b_DBType}   zone: {b_DBZone} not found mongo connect string");
                    }

                    var startConfigTemp = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(mStartZoneConfigs.AppId);
                    //StartConfig startConfigTemp = StartConfigComponent.Instance.Get(mStartZoneConfigs.AppId);
                    if (startConfigTemp.AppType == AppType.DB)
                    {
                        self.dbAddress = startConfigTemp.ServerInnerIP;
                        self.SetDBInfo(mStartZoneConfigs.AppId, (DBType)mStartZoneConfigs.DBType, mStartZoneConfigs.DBZone);
                        break;
                    }
                }
            }
        }

        //public static async Task<bool> Save(this DBProxyComponent self, ComponentWithId component)
        //{
        //    Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
        //    var mResponse = await session.Call(new DBSaveRequest { Component = component });
        //    return mResponse.Error == 0;
        //}
        //public static async Task<bool> SaveT<T>(this DBProxyComponent self, T component) where T : ComponentWithId
        //{
        //    return await self.Save(component);
        //}

        //public static async Task SaveBatch(this DBProxyComponent self, List<ComponentWithId> components)
        //{
        //    Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
        //    await session.Call(new DBSaveBatchRequest { Components = components });
        //}

        //public static async Task Save(this DBProxyComponent self, ComponentWithId component, CancellationToken cancellationToken)
        //{
        //    Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
        //    await session.Call(new DBSaveRequest { Component = component }, cancellationToken);
        //}

        //public static async void SaveLog(this DBProxyComponent self, ComponentWithId component)
        //{
        //    Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
        //    await session.Call(new DBSaveRequest { Component = component, CollectionName = "Log" });
        //}

        //public static Task<T> Query<T>(this DBProxyComponent self, long id) where T : ComponentWithId
        //{
        //    string key = typeof(T).Name + id;
        //    TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
        //    if (self.TcsQueue.ContainsKey(key))
        //    {
        //        self.TcsQueue.Add(key, tcs);
        //        return tcs.Task;
        //    }

        //    self.TcsQueue.Add(key, tcs);
        //    self.QueryInner<T>(id, key);
        //    return tcs.Task;
        //}

        //private static async void QueryInner<T>(this DBProxyComponent self, long id, string key) where T : ComponentWithId
        //{
        //    try
        //    {
        //        Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
        //        DBQueryResponse dbQueryResponse = (DBQueryResponse)await session.Call(new DBQueryRequest { CollectionName = typeof(T).Name, Id = id });
        //        T result = (T)dbQueryResponse.Component;

        //        object[] tcss = self.TcsQueue.GetAll(key);
        //        self.TcsQueue.Remove(key);

        //        foreach (TaskCompletionSource<T> tcs in tcss)
        //        {
        //            tcs.SetResult(result);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        object[] tcss = self.TcsQueue.GetAll(key);
        //        self.TcsQueue.Remove(key);

        //        foreach (TaskCompletionSource<T> tcs in tcss)
        //        {
        //            tcs.SetResult(null);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 根据查询表达式查询
        ///// </summary>
        ///// <param name="self"></param>
        ///// <param name="exp"></param>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static async Task<List<ComponentWithId>> Query<T>(this DBProxyComponent self, Expression<Func<T, bool>> exp) where T : ComponentWithId
        //{
        //    ExpressionFilterDefinition<T> filter = new ExpressionFilterDefinition<T>(exp);
        //    IBsonSerializerRegistry serializerRegistry = BsonSerializer.SerializerRegistry;
        //    IBsonSerializer<T> documentSerializer = serializerRegistry.GetSerializer<T>();
        //    string json = filter.Render(documentSerializer, serializerRegistry).ToJson();
        //    return await self.Query<T>(json);
        //}

        //public static async Task<List<ComponentWithId>> Query<T>(this DBProxyComponent self, List<long> ids) where T : ComponentWithId
        //{
        //    Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
        //    DBQueryBatchResponse dbQueryBatchResponse = (DBQueryBatchResponse)await session.Call(new DBQueryBatchRequest { CollectionName = typeof(T).Name, IdList = ids });
        //    return dbQueryBatchResponse.Components;
        //}

        ///// <summary>
        ///// 根据json查询条件查询
        ///// </summary>
        ///// <param name="self"></param>
        ///// <param name="json"></param>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static Task<List<ComponentWithId>> Query<T>(this DBProxyComponent self, string json) where T : ComponentWithId
        //{
        //    string key = typeof(T).Name + json;
        //    TaskCompletionSource<List<ComponentWithId>> tcs = new TaskCompletionSource<List<ComponentWithId>>();
        //    if (self.TcsQueue.ContainsKey(key))
        //    {
        //        self.TcsQueue.Add(key, tcs);
        //        return tcs.Task;
        //    }

        //    self.TcsQueue.Add(key, tcs);
        //    self.QueryInner<T>(json, key);
        //    return tcs.Task;
        //}

        //private static async void QueryInner<T>(this DBProxyComponent self, string json, string key) where T : ComponentWithId
        //{
        //    try
        //    {
        //        Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
        //        DBQueryJsonResponse dbQueryJsonResponse = (DBQueryJsonResponse)await session.Call(new DBQueryJsonRequest { CollectionName = typeof(T).Name, Json = json });

        //        var result = dbQueryJsonResponse.Components;

        //        object[] tcss = self.TcsQueue.GetAll(key);
        //        self.TcsQueue.Remove(key);

        //        foreach (TaskCompletionSource<List<ComponentWithId>> tcs in tcss)
        //        {
        //            tcs.SetResult(result);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        object[] tcss = self.TcsQueue.GetAll(key);
        //        self.TcsQueue.Remove(key);

        //        foreach (TaskCompletionSource<List<ComponentWithId>> tcs in tcss)
        //        {
        //            tcs.SetResult(null);
        //        }
        //    }
        //}



    }
}