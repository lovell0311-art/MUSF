using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    public static class HelpDb_DBGameSkillData
    {

        public static async Task<C_DataCache<DBGameSkillData>> Init(Player b_Player, DataCacheManageComponent b_Component, DBProxyComponent b_DbProxy)
        {
            var mDataCache = b_Component.Get<DBGameSkillData>();
            if (mDataCache == null)
            {
                mDataCache = await b_Component.Add<DBGameSkillData>(b_DbProxy, p => p.GameUserId == b_Player.GameUserId, b_Count: 1);
            }

            return mDataCache;
        }

    }
}
