using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;

using CustomFrameWork;

namespace ETModel
{
    public class C_DataCache : ADataContext
    {
        public virtual async Task ForceSave() { }

    }
    public class C_DataCache<T> : C_DataCache where T : DBBase
    {
        public static readonly List<T> _NullList = new List<T>();
        public Dictionary<int, Dictionary<long,T>> _DataCacheDic;
        public void Init()
        {
            if (_DataCacheDic == null)
            {
                _DataCacheDic = new Dictionary<int, Dictionary<long, T>>();
            }
            else
            {
                _DataCacheDic.Clear();
            }
        }

        public override async Task ForceSave()
        {
            if (_DataCacheDic != null && _DataCacheDic.Count > 0)
            {
                var mValuelistByType = _DataCacheDic.Values.ToArray();
                using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();
                for (int i = 0, len = mValuelistByType.Length; i < len; i++)
                {
                    var mValuelist = mValuelistByType[i];

                    if (mValuelist == null || mValuelist.Count == 0) continue;

                    foreach (T mSingleValue in mValuelist.Values)
                    {
                        async Task<bool> SaveDB()
                        {
                            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.DB, mSingleValue.Id))
                            {
                                if (mSingleValue.IsChange)
                                {
                                    bool mSaveResult = await mSingleValue.ProxyComponent.Save(mSingleValue);
                                    mSingleValue.IsChange = false;
                                    if (mSaveResult == false)
                                    {
                                        Log.Error($"强制保存时失败! {mSingleValue._t}  Id:{mSingleValue.Id} {Help_JsonSerializeHelper.Serialize(mSingleValue)}");
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                        tasks.Add(SaveDB());
                    }
                }
                await TaskHelper.WaitAll(tasks);

            }
        }

        public override void Dispose()
        {
            if (IsDisposeable) return;

            if (_DataCacheDic != null)
            {
                if (_DataCacheDic.Count > 0)
                {
                    _DataCacheDic.Clear();
                }
                _DataCacheDic = null;
            }
            base.Dispose();
        }
        public bool ContainsKey(int b_TagId = 0)
        {
            return _DataCacheDic.ContainsKey(b_TagId);
        }
      

        public async Task<bool> DataQueryInit(DBProxyComponent b_DBProxy, Expression<Func<T, bool>> b_QueryExpression, int b_TagId = 0, int b_Count = 0)
        {
            if (IsDisposeable == false && b_DBProxy != null && b_DBProxy.IsDisposeable == false)
            {
                // 如果有缓存 则全部清理掉 换新的数据
                if (_DataCacheDic.TryGetValue(b_TagId, out Dictionary<long, T> mDataCacheList))
                {
                    if (mDataCacheList.Count != 0)
                    {
                        mDataCacheList.Clear();
                    }
                }
                else
                {
                    mDataCacheList = _DataCacheDic[b_TagId] = new Dictionary<long, T>();
                }

                var mDataQuerys = await b_DBProxy.Query<T>(b_QueryExpression, b_Count);
                if (mDataQuerys != null && mDataQuerys.Count > 0)
                {
                    DBMongodbProxySaveManageComponent mManageComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>();
                    C_MongodbProxySave mDBSaveComponent = mManageComponent.Get(b_DBProxy.AreaId);
                    for (int i = 0, len = mDataQuerys.Count; i < len; i++)
                    {
                        var mTemp = mDataQuerys[i] as T;

                        var mTempNew = mDBSaveComponent.GetOne<T>(mTemp._t, mTemp.Id);

                        if (mTempNew != null)
                        {
                            mDataCacheList.Add(mTempNew.Id, mTempNew);
                        }
                        else
                        {
                            mDataCacheList.Add(mTemp.Id, mTemp);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public async Task<bool> DataQueryAppend(DBProxyComponent b_DBProxy, Expression<Func<T, bool>> b_QueryExpression, int b_TagId = 0)
        {
            if (IsDisposeable == false && b_DBProxy != null && b_DBProxy.IsDisposeable == false)
            {
                // 如果有缓存 则在缓存后面添加
                if (!_DataCacheDic.TryGetValue(b_TagId, out Dictionary<long, T> mDataCacheList))
                {
                    mDataCacheList = _DataCacheDic[b_TagId] = new Dictionary<long, T>();
                }

                var mDataQuerys = await b_DBProxy.Query<T>(b_QueryExpression);
                if (mDataQuerys != null && mDataQuerys.Count > 0)
                {
                    DBMongodbProxySaveManageComponent mManageComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>();
                    C_MongodbProxySave mDBSaveComponent = mManageComponent.Get(b_DBProxy.AreaId);
                    for (int i = 0, len = mDataQuerys.Count; i < len; i++)
                    {
                        var mTemp = mDataQuerys[i] as T;

                        var mTempNew = mDBSaveComponent.GetOne<T>(mTemp._t, mTemp.Id);

                        if (mTempNew != null)
                        {
                            if(mDataCacheList.TryGetValue(mTempNew.Id,out T oldData))
                            {
                                Log.Error($"追加数据重复 Type:{typeof(T).Name} oldData:{oldData.ToJson()} newData:{mTempNew.ToJson()}");
                            }
                            else
                            {
                                mDataCacheList.Add(mTempNew.Id, mTempNew);
                            }
                        }
                        else
                        {
                            if (mDataCacheList.TryGetValue(mTemp.Id, out T oldData))
                            {
                                Log.Error($"追加数据重复 Type:{typeof(T).Name} oldData:{oldData.ToJson()} newData:{mTemp.ToJson()}");
                            }
                            else
                            {
                                mDataCacheList.Add(mTemp.Id, mTemp);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public List<T> DataQuery(Expression<Func<T, bool>> b_QueryExpression, int b_TagId = 0)
        {
            if (_DataCacheDic.TryGetValue(b_TagId, out Dictionary<long, T> mDataCacheList))
            {
                if (mDataCacheList.Count == 0 || b_QueryExpression == null) return _NullList;

                var mResult = mDataCacheList.Values.Where(b_QueryExpression.Compile());
                return mResult.ToList();
            }
            else
            {
                return null;
            }
        }

        public T OnlyOne(int b_TagId = 0)
        {
            if (_DataCacheDic.TryGetValue(b_TagId, out var mDataCacheList))
            {
                if (mDataCacheList != null && mDataCacheList.Count > 0)
                {
                    if (mDataCacheList.Count > 1)
                        throw new Exception($"=>{typeof(T).Name}:\tTime:{DateTime.Now}\n{Environment.StackTrace}\n\n");

                    return mDataCacheList.Values.First();
                }
            }
            return null;
        }

        /// <summary>
        /// 通过Id进行查询，效率高
        /// </summary>
        /// <param name="b_QueryExpression"></param>
        /// <param name="b_TagId"></param>
        /// <returns></returns>
        public T DataQueryById(long b_Id, int b_TagId = 0)
        {
            if (_DataCacheDic.TryGetValue(b_TagId, out Dictionary<long, T> mDataCacheList))
            {
                if (mDataCacheList.TryGetValue(b_Id,out T value))
                {
                    return value;
                }
            }
            return null;
        }

        public void DataAdd(T b_Object, int b_TagId = 0)
        {
            if (_DataCacheDic.TryGetValue(b_TagId, out Dictionary<long, T> mDataCacheList) == false)
            {
                mDataCacheList = _DataCacheDic[b_TagId] = new Dictionary<long, T>();
            }
            if (mDataCacheList.TryGetValue(b_Object.Id, out T oldData))
            {
                Log.Error($"添加数据重复 Type:{typeof(T).Name} oldData:{oldData.ToJson()} newData:{b_Object.ToJson()}");
            }
            else
            {
                mDataCacheList.Add(b_Object.Id, b_Object);
            }
        }

        /// <summary>
        /// 当数据进行转移时，进行移除操作
        /// </summary>
        /// <param name="b_Id"></param>
        /// <param name="b_TagId"></param>
        public void DataRemove(long b_Id, int b_TagId = 0)
        {
            if (_DataCacheDic.TryGetValue(b_TagId, out Dictionary<long, T> mDataCacheList) == false)
            {
                mDataCacheList = _DataCacheDic[b_TagId] = new Dictionary<long, T>();
            }
            mDataCacheList.Remove(b_Id);
        }
    }
}