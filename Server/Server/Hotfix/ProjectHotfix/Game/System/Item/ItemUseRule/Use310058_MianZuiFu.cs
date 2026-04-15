
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
    /// 使用免罪符
    /// </summary>
    [ItemUseRule(typeof(Use310058))]
    public class Use310058 : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();
            int addPer = b_Item.ConfigData.Value;

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            if (mGamePlayer.UnitData.PkPoint <= 0)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            mGamePlayer.UnitData.PkPoint -= addPer;
            if (mGamePlayer.UnitData.PkPoint < 0)
            {
                mGamePlayer.UnitData.PkPoint = 0;
            }
            //保存数据库
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mGamePlayer.UnitData, dBProxy).Coroutine();

            //发送推送
            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            mChangeValueMessage.GameUserId = mGamePlayer.InstanceId;

            G2C_BattleKVData mData = new G2C_BattleKVData();
            mData.Key = (int)E_GameProperty.PkNumber;
            mData.Value = mGamePlayer.GetNumerialFunc(E_GameProperty.PkNumber);
            mChangeValueMessage.Info.Add(mData);

            var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, b_Player.GameUserId);
            if (mMapComponent == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            mMapComponent.SendNotice(mGamePlayer, mChangeValueMessage);

            return;
        }
    }
}