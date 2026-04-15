
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
    [GameMasterCommandLine("获取大师点数")]
    public class GMCommandLine_获取大师点数 : C_GameMasterCommandLine<Player, IResponse>
    {
        public override async Task Run(Player b_Player, List<int> b_Parameter, IResponse b_Response)
        {
            if (b_Parameter.Count <= 0)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Player.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheComponent.Get<DBMasterData>();
            if (mDataCache == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "大师系统不满足开放条件";
                return;
            }
            var mData = mDataCache.OnlyOne();

            mData.PropertyPoint = b_Parameter[0];

            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mData, dBProxy2).Coroutine();
        }
    }
}