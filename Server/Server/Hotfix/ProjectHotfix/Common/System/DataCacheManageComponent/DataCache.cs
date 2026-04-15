//using ETModel;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;

//using CustomFrameWork;
//using CustomFrameWork.Baseic;

//namespace ETHotfix
//{

//    public class C_DataCache<T> : C_DataCache where T : DBBase
//    {

//        public Dictionary<int, List<T>> _DataCacheDic;
//        public void Init()
//        {
//            if (_DataCacheDic == null)
//            {
//                _DataCacheDic = new Dictionary<int, List<T>>();
//            }
//            else
//            {
//                _DataCacheDic.Clear();
//            }
//        }

//        public override async Task ForceSave()
//        {
//            if (_DataCacheDic != null && _DataCacheDic.Count > 0)
//            {
//                var mValuelistByType = _DataCacheDic.Values.ToArray();
//                for (int i = 0, len = mValuelistByType.Length; i < len; i++)
//                {
//                    var mValuelist = mValuelistByType[i];

//                    if (mValuelist == null || mValuelist.Count == 0) continue;

//                    for (int j = 0, jlen = mValuelist.Count; j < jlen; j++)
//                    {
//                        var mSingleValue = mValuelist[j];
//                        if (mSingleValue.IsChange)
//                        {
//                            bool mSaveResult = await mSingleValue.ProxyComponent.Save(mSingleValue);
//                            if (mSaveResult == false)
//                            {
//                                CustomFrameWork.Component.LogToolComponent.Error($"强制保存时失败! {mSingleValue._t}  Id:{mSingleValue.Id} {Help_JsonSerializeHelper.Serialize(mSingleValue)}", false);
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        public override void Dispose()
//        {
//            if (IsDisposeable) return;

//            if (_DataCacheDic != null)
//            {
//                if (_DataCacheDic.Count > 0)
//                {
//                    _DataCacheDic.Clear();
//                }
//                _DataCacheDic = null;
//            }
//            base.Dispose();
//        }
//        public bool ContainsKey(int b_TagId = 0)
//        {
//            return _DataCacheDic.ContainsKey(b_TagId);
//        }

//        public async Task<bool> DataQueryInit(DBProxyComponent b_DBProxy, Expression<Func<T, bool>> b_QueryExpression, int b_TagId = 0)
//        {
//            if (IsDisposeable == false && b_DBProxy != null && b_DBProxy.IsDisposeable == false)
//            {
//                // 如果有缓存 则全部清理掉 换新的数据
//                if (_DataCacheDic.TryGetValue(b_TagId, out List<T> mDataCacheList))
//                {
//                    if (mDataCacheList.Count != 0)
//                    {
//                        mDataCacheList.Clear();
//                    }
//                }
//                else
//                {
//                    mDataCacheList = _DataCacheDic[b_TagId] = new List<T>();
//                }

//                var mDataQuerys = await b_DBProxy.Query<T>(b_QueryExpression);
//                if (mDataQuerys != null && mDataQuerys.Count > 0)
//                {
//                    DBMongodbProxySaveManageComponent mManageComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>();
//                    C_MongodbProxySave mDBSaveComponent = mManageComponent.Get(b_DBProxy.AreaId);
//                    for (int i = 0, len = mDataQuerys.Count; i < len; i++)
//                    {
//                        var mTemp = mDataQuerys[i] as T;

//                        var mTempNew = mDBSaveComponent.GetOne<T>(mTemp._t, mTemp.Id);

//                        if (mTempNew != null)
//                        {
//                            mDataCacheList.Add(mTempNew);
//                        }
//                        else
//                        {
//                            mDataCacheList.Add(mTemp);
//                        }
//                    }
//                }
//                return true;
//            }
//            return false;
//        }
//        public List<T> DataQuery(Expression<Func<T, bool>> b_QueryExpression, int b_TagId = 0)
//        {
//            if (_DataCacheDic.TryGetValue(b_TagId, out List<T> mDataCacheList))
//            {
//                if (mDataCacheList.Count == 0 || b_QueryExpression == null) return mDataCacheList;

//                var mResult = mDataCacheList.Where(b_QueryExpression.Compile());
//                return mResult.ToList();
//            }
//            else
//            {
//                return null;
//            }
//        }
//        public void DataAdd(T b_Object, int b_TagId = 0)
//        {
//            if (_DataCacheDic.TryGetValue(b_TagId, out List<T> mDataCacheList) == false)
//            {
//                mDataCacheList = _DataCacheDic[b_TagId] = new List<T>();
//            }
//            mDataCacheList.Add(b_Object);
//        }
//    }
//}