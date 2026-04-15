
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using CustomFrameWork;
using MongoDB.Driver;

namespace ETModel
{
    public partial class C_MongodbProxySave
    {
        public T GetOne<T>(string b_t, long b_Id) where T : DBBase
        {
            if (_ReverseDataDic.TryGetValue(b_t, out Dictionary<long, DBBase> mTypeCaches))
            {
                if (mTypeCaches.TryGetValue(b_Id, out var mResult))
                {
                    return mResult as T;
                }
            }
            return null;
        }
        public T GetOne<T>(long b_Id) where T : DBBase
        {
            string typeName = typeof(T).Name;
            return GetOne<T>(typeName, b_Id);
        }
        public List<T> Get<T>(Expression<Func<T, bool>> b_QueryExpression) where T : DBBase
        {
            string typeName = typeof(T).Name;
            if (_ReverseDataDic.TryGetValue(typeName, out Dictionary<long, DBBase> mTypeCaches))
            {
                List<T> mResult = new List<T>();
                if (mTypeCaches.Count == 0) return mResult;

                var func = b_QueryExpression == null ? (item) => { return true; } : b_QueryExpression.Compile();

                var mDataList = mTypeCaches.Values.ToList();
                for (int i = 0, len = mDataList.Count; i < len; i++)
                {
                    var mTemp = mDataList[i] as T;
                    if (mTemp != null)
                    {
                        if (func(mTemp))
                        {
                            mResult.Add(mTemp);
                        }
                    }
                }

                return mResult;
            }
            return null;
        }
        public async Task Save<T>(T b_Data, DBProxyComponent b_ProxyComponent) where T : DBBase
        {
            using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.DB,b_Data.Id))
            {
                b_Data.IsChange = true;
                b_Data.ProxyComponent = b_ProxyComponent;
                if (b_Data.ChangeTick == 0)
                {
                    b_Data.ChangeTick = CustomFrameWork.TimeHelper.ClientNowSeconds() + _IntervalTimeTick / 10000000;
                }

                if (_ReverseDataDic.TryGetValue(b_Data._t, out var mTypeCaches) == false)
                {
                    mTypeCaches = _ReverseDataDic[b_Data._t] = new Dictionary<long, DBBase>();
                }
                if (mTypeCaches.ContainsKey(b_Data.Id) == false)
                {
                    mTypeCaches[b_Data.Id] = b_Data;
                }

                if (_UpdateDataDic.TryGetValue(b_Data.ChangeTick, out C_DBSaveGroup mDataSaveGroup) == false)
                {
                    mDataSaveGroup = _UpdateDataDic[b_Data.ChangeTick] = Root.CreateBuilder.GetInstance<C_DBSaveGroup>();
                    mDataSaveGroup.ContextAwake();
                }
                if (mDataSaveGroup.UpdateDataDic.ContainsKey(b_Data.Id) == false)
                {
                    mDataSaveGroup.UpdateDataDic[b_Data.Id] = b_Data;
                }
            }
        }
    }
}
