
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 使用血瓶
    /// </summary>
    [ItemUseRule(typeof(Use310151))]
    public class Use310151 : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
          
            DataCacheManageComponent mDataCacheComponent = b_Player.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheComponent.Get<DBMasterData>();
            if (mDataCache == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "大师系统不满足开放条件";
                return;
            }
            var mData = mDataCache.OnlyOne();

            mData.PropertyPoint +=  b_Item.ConfigData.Value;
            mData.ExtraPoints +=  b_Item.ConfigData.Value;

            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mData, dBProxy).Coroutine();

            return;
        }
    }
}