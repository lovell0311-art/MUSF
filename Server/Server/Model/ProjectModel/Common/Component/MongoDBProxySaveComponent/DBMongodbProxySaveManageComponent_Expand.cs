
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using MongoDB.Driver;

namespace ETModel
{
    public partial class DBMongodbProxySaveManageComponent
    {
        public void Add(int b_GameAreaId)
        {
            if (_Instance.ContainsKey(b_GameAreaId) == false)
            {
                var mResult = Root.CreateBuilder.GetInstance<C_MongodbProxySave, int>(b_GameAreaId);

                _Instance[mResult.GameAreaId] = mResult;
            }
        }

        public C_MongodbProxySave Get(int b_GameAreaId)
        {
            if (_Instance.TryGetValue(b_GameAreaId, out var mResult))
            {
                return mResult;
            }
            return null;
        }

        public void Remove(int b_GameAreaId)
        {
            if (_Instance.TryGetValue(b_GameAreaId, out var mResult))
            {
                _Instance.Remove(b_GameAreaId);

                mResult.Dispose();
            }
        }
    }
}
