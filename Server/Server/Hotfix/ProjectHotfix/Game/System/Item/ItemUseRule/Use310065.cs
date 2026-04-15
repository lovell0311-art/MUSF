
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
    /// 敏捷洗点果实
    /// </summary>
    [ItemUseRule(typeof(Use310065))]
    public class Use310065 : C_ItemUseRule<Player, Item, IResponse>
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

            var mCurrentTemp = b_Player.GetCustomComponent<GamePlayer>();

            if (mCurrentTemp.Data.Agility < 0)
            {
                b_Response.Error = 725;
                return;
            }

            var mEquipmentComponent = b_Player.GetCustomComponent<EquipmentComponent>();
            if (mEquipmentComponent.EquipIsEmpty() == false)
            {
                b_Response.Error = 753;
                return;
            }

            mCurrentTemp.Data.FreePoint += mCurrentTemp.Data.Agility;
            mCurrentTemp.Data.Agility = 0;
            b_Player.GetCustomComponent<EquipmentComponent>()?.ApplyEquipProp();
            mCurrentTemp.DataUpdateProperty();
            //保存数据库
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mCurrentTemp.Data, dBProxy).Coroutine();

            //发送推送
            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
            {
                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                mBattleKVData.Key = (int)b_GameProperty;
                mBattleKVData.Value = mCurrentTemp.GetNumerial(b_GameProperty);
                b_ChangeValue_notice.Info.Add(mBattleKVData);
            }

            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            mChangeValueMessage.GameUserId = mCurrentTemp.InstanceId;

            AddPropertyNotice(mChangeValueMessage, E_GameProperty.Property_Agility);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.FreePoint);

            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

            AddPropertyNotice(mChangeValueMessage, E_GameProperty.Defense);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.AttackSpeed);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.DefenseRate);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PVPDefenseRate);

            switch ((E_GameOccupation)mCurrentTemp.Data.PlayerTypeId)
            {
                case E_GameOccupation.Archer:
                    {
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinAtteck);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxAtteck);

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.AtteckSuccessRate);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PVPAtteckSuccessRate);
                    }
                    break;
                default:
                    break;
            }

            var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, b_Player.GameUserId);
            if (mMapComponent == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            mMapComponent.SendNotice(mCurrentTemp, mChangeValueMessage);

            return;
        }
    }
}