
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
    /// 宠物使用血瓶
    /// </summary>
    [ItemUseRule(typeof(UseItemPetBloodBottle))]
    public class UseItemPetBloodBottle : C_ItemUseRule<Player, Item, IResponse>
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
            int Value_A = b_Item.ConfigData.Value;//治疗效果
            int Value_B = b_Item.ConfigData.Value2;//值
            int Value_C = mGamePlayer.Pets.dBPetsData.PetsLevel;//等级
            int Value_D = 0;//计算值
            int maxHP = mGamePlayer.Pets.GetNumerial(E_GameProperty.PROP_HP_MAX);

            Value_D = (Value_B * 10 - Value_C * 2);
            if (Value_D < 0) Value_D = 0;

            mGamePlayer.Pets.dBPetsData.PetsHP += (int)MathF.Ceiling(Value_D + maxHP * Value_A * 0.01f);
            if (mGamePlayer.Pets.dBPetsData.PetsHP > maxHP)
            {
                mGamePlayer.Pets.dBPetsData.PetsHP = maxHP;
            }
            //保存数据库
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mGamePlayer.Pets.dBPetsData, dBProxy).Coroutine();
            //发送推送
            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            mChangeValueMessage.GameUserId = mGamePlayer.Pets.InstanceId;

            G2C_BattleKVData mData = new G2C_BattleKVData();
            mData.Key = (int)E_GameProperty.PROP_HP;
            mData.Value = mGamePlayer.Pets.GetNumerialFunc(E_GameProperty.PROP_HP);
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