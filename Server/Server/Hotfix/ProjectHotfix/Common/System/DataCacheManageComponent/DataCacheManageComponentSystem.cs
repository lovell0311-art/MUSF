using ETModel;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using CustomFrameWork;
using System.Linq;

namespace ETHotfix
{
    public static class DataCacheManageComponentSystem
    {
        public static async Task<C_DataCache<T>> Add<T>(this DataCacheManageComponent self, DBProxyComponent b_DBProxy, Expression<Func<T, bool>> b_QueryExpression, int b_TagId = 0, int b_Count = 0) where T : DBBase
        {
            string typeName = typeof(T).Name;
            if (self.DataCacheDic.TryGetValue(typeName, out C_DataCache mCustomComponent))
            {
                C_DataCache<T> mTemp = mCustomComponent as C_DataCache<T>;

                if (mTemp.ContainsKey(b_TagId) == false)
                {
                    await mTemp.DataQueryInit(b_DBProxy, b_QueryExpression, b_TagId, b_Count);
                }
                return mTemp;
            }
            else
            {
                var mResult = Root.CreateBuilder.GetInstance<C_DataCache<T>>();
                mResult.Init();
                self.DataCacheDic[typeName] = mResult;

                await mResult.DataQueryInit(b_DBProxy, b_QueryExpression, b_TagId, b_Count);

                return mResult;
            }
        }

        public static async Task<C_DataCache<T>> Append<T>(this DataCacheManageComponent self, DBProxyComponent b_DBProxy, Expression<Func<T, bool>> b_QueryExpression, int b_TagId = 0) where T : DBBase
        {
            string typeName = typeof(T).Name;
            if (self.DataCacheDic.TryGetValue(typeName, out C_DataCache mCustomComponent))
            {
                C_DataCache<T> mTemp = mCustomComponent as C_DataCache<T>;

                await mTemp.DataQueryAppend(b_DBProxy, b_QueryExpression, b_TagId);
                return mTemp;
            }
            else
            {
                var mResult = Root.CreateBuilder.GetInstance<C_DataCache<T>>();
                mResult.Init();
                self.DataCacheDic[typeName] = mResult;

                await mResult.DataQueryAppend(b_DBProxy, b_QueryExpression, b_TagId);

                return mResult;
            }
        }

        public static C_DataCache<T> Get<T>(this DataCacheManageComponent self) where T : DBBase
        {
            string name = typeof(T).Name;
            if (self.DataCacheDic != null && self.DataCacheDic.ContainsKey(name))
            {
                return self.DataCacheDic[name] as C_DataCache<T>;
            }
            return default;
        }

        public static C_DataCache<T> GetOrCreate<T>(this DataCacheManageComponent self) where T : DBBase
        {
            string name = typeof(T).Name;
            string typeName = typeof(T).Name;
            if (!self.DataCacheDic.TryGetValue(typeName, out C_DataCache mCustomComponent))
            {
                var mResult = Root.CreateBuilder.GetInstance<C_DataCache<T>>();
                mResult.Init();
                mCustomComponent = mResult;
                self.DataCacheDic[typeName] = mCustomComponent;
            }
            return mCustomComponent as C_DataCache<T>;
        }
        public static async Task ForceSave(this DataCacheManageComponent self)
        {
            if (self.DataCacheDic != null && self.DataCacheDic.Count > 0)
            {
                var mValueByType = self.DataCacheDic.Values.ToArray();
                using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();
                for (int i = 0, len = mValueByType.Length; i < len; i++)
                {
                    var mValue = mValueByType[i];

                    async Task<bool> ForceSaveCoroutine()
                    {
                        await mValue.ForceSave();
                        return true;
                    }
                    tasks.Add(ForceSaveCoroutine());
                }
                await TaskHelper.WaitAll(tasks);
            }
        }
    }
}