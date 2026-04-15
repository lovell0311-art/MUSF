
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using static ETModel.C_MongodbProxySave;

namespace ETHotfix
{
    public static class MongodbProxySaveComponentSystem
    {
        public static void Update(this C_MongodbProxySave b_Component, long b_ChangeTick)
        {
            var mKeys = b_Component._UpdateDataDic.Keys.ToArray();
            for (int i = 0, len = mKeys.Length; i < len; i++)
            {
                var mKey = mKeys[i];

                if (mKey < b_ChangeTick)
                {
                    C_DBSaveGroup mDataGroup = b_Component._UpdateDataDic[mKey];
                    b_Component._UpdateDataDic.Remove(mKey);

                    b_Component.Save(mDataGroup).Coroutine();
                }
            }
        }
        private static async Task Save(this C_MongodbProxySave b_Component, C_DBSaveGroup b_DataGroup)
        {
            var mDatalist = b_DataGroup.UpdateDataDic.Values.ToArray();
            using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();
            for (int i = 0, len = mDatalist.Length; i < len; i++)
            {
                var mData = mDatalist[i];

                async Task<bool> SaveDB()
                {
                    using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.DB, mData.Id))
                    {
                        if (mData.IsChange == false)
                        {// 哪些功能修改了别人的数据
                            if (b_Component._ReverseDataDic.TryGetValue(mData._t, out Dictionary<long, DBBase> mTypeCaches))
                            {
                                if (mTypeCaches.ContainsKey(mData.Id))
                                {
                                    mTypeCaches.Remove(mData.Id);
                                }
                            }
                            mData.ChangeTick = 0;
                            return true;
                        }

                        bool mSaveResult = await mData.ProxyComponent.Save(mData);
                        if (mSaveResult)
                        {
                            if (b_Component._ReverseDataDic.TryGetValue(mData._t, out Dictionary<long, DBBase> mTypeCaches))
                            {
                                if (mTypeCaches.ContainsKey(mData.Id))
                                {
                                    mTypeCaches.Remove(mData.Id);
                                }
                            }
                            mData.IsChange = false;
                            mData.ChangeTick = 0;
                        }
                        else
                        {
                            b_Component.Save(mData, mData.ProxyComponent).Coroutine();
                            return false;
                        }
                    }
                    return true;
                }
                tasks.Add(SaveDB());
            }
            await TaskHelper.WaitAll(tasks);

            b_DataGroup.Dispose();
        }
        
        /// <summary>
        /// 保存所有
        /// </summary>
        /// <param name="b_Component"></param>
        /// <returns></returns>
        public static async Task SaveAllAsync(this C_MongodbProxySave b_Component)
        {
            while(b_Component._UpdateDataDic.Count > 0)
            {
                var first = b_Component._UpdateDataDic.First();
                C_DBSaveGroup mDataGroup = first.Value;
                long mKey = first.Key;
                b_Component._UpdateDataDic.Remove(mKey);

                await b_Component.Save(mDataGroup);
            }
        }

    }
}
