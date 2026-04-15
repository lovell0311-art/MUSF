
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
    /// 使用蓝瓶
    /// </summary>
    [ItemUseRule(typeof(Use310005_06_07))]
    public class Use310005_06_07 : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();
            int addPer = b_Item.ConfigData.Value;
            int maxMP = mGamePlayer.GetNumerial(E_GameProperty.PROP_MP_MAX);

            int addValue = (int)MathF.Ceiling(maxMP * addPer * 0.01f);
            var mCount = b_Item.ConfigData.Value2 * 10 - mGamePlayer.Data.Level;
            if (mCount > 0)
            {
                addValue += mCount;
            }

            mGamePlayer.UnitData.Mp += addValue;
            if (mGamePlayer.UnitData.Mp > maxMP)
            {
                mGamePlayer.UnitData.Mp = maxMP;
            }
            //保存数据库
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mGamePlayer.UnitData, dBProxy).Coroutine();
            //发送推送
            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
            {
                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                mBattleKVData.Key = (int)b_GameProperty;
                mBattleKVData.Value = mGamePlayer.GetNumerial(b_GameProperty);
                b_ChangeValue_notice.Info.Add(mBattleKVData);
            }

            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            mChangeValueMessage.GameUserId = mGamePlayer.InstanceId;

            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP);

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