
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
    /// 梦幻骑士角色卡
    /// </summary>
    [ItemUseRule(typeof(Use310081))]
    public class Use310081 : C_ItemUseRule<Player, Item, IResponse>
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
            var Info = mCurrentTemp.Data;
            if (Info.Agility2 + Info.Strength2 + Info.Willpower2 + Info.BoneGas2 + Info.Command2 > 100)
            {
                b_Response.Error = 3124;
                b_Response.Message = "参数不对";
                return;
            }

            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
            {
                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                mBattleKVData.Key = (int)b_GameProperty;
                mBattleKVData.Value = mCurrentTemp.GetNumerial(b_GameProperty);
                b_ChangeValue_notice.Info.Add(mBattleKVData);
            }

            switch (b_Item.ConfigID)
            {
                case 310081:
                    {
                        mCurrentTemp.Data.Strength2 += 1;
                        mCurrentTemp.ChangeNumerialType(E_GameProperty.Property_Strength);

                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = mCurrentTemp.InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinAtteck);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxAtteck);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.AtteckSuccessRate);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PVPAtteckSuccessRate);

                        var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, mCurrentTemp.Player.GameUserId);
                        mMapComponent.SendNotice(mCurrentTemp, mChangeValueMessage);
                    }
                    break;
                case 310082:
                    {
                        mCurrentTemp.Data.Agility2 += 1;
                        mCurrentTemp.ChangeNumerialType(E_GameProperty.Property_Agility);

                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = mCurrentTemp.InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.Defense);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.AttackSpeed);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.DefenseRate);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PVPDefenseRate);

                        switch ((E_GameOccupation)Info.PlayerTypeId)
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

                        var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, mCurrentTemp.Player.GameUserId);
                        mMapComponent.SendNotice(mCurrentTemp, mChangeValueMessage);
                    }
                    break;
                case 310083:
                    {
                        mCurrentTemp.Data.BoneGas2 += 1;
                        mCurrentTemp.ChangeNumerialType(E_GameProperty.Property_BoneGas);

                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = mCurrentTemp.InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);


                        var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, mCurrentTemp.Player.GameUserId);
                        mMapComponent.SendNotice(mCurrentTemp, mChangeValueMessage);

                    }
                    break;
                case 310084:
                    {
                        mCurrentTemp.Data.Willpower2 += 1;
                        mCurrentTemp.ChangeNumerialType(E_GameProperty.Property_Willpower);

                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = mCurrentTemp.InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP_MAX);

                        switch ((E_GameOccupation)Info.PlayerTypeId)
                        {
                            case E_GameOccupation.Spell:
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinMagicAtteck);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxMagicAtteck);
                                break;
                            case E_GameOccupation.Swordsman:
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.SkillAddition);
                                break;
                            case E_GameOccupation.Archer:
                                break;
                            case E_GameOccupation.Spellsword:
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinAtteck);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxAtteck);

                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinMagicAtteck);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxMagicAtteck);
                                break;
                            case E_GameOccupation.Holyteacher:
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinAtteck);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxAtteck);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.SkillAddition);
                                break;
                            case E_GameOccupation.SummonWarlock:
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinMagicAtteck);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxMagicAtteck);

                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinDamnationAtteck);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxDamnationAtteck);
                                break;
                            case E_GameOccupation.Combat:
                                break;
                            case E_GameOccupation.GrowLancer:
                                break;
                            default:
                                break;
                        }

                        var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, mCurrentTemp.Player.GameUserId);
                        mMapComponent.SendNotice(mCurrentTemp, mChangeValueMessage);

                    }
                    break;
                case 310085:
                    {
                        mCurrentTemp.Data.Command2 += 1;
                        mCurrentTemp.ChangeNumerialType(E_GameProperty.Property_Command);

                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = mCurrentTemp.InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, mCurrentTemp.Player.GameUserId);
                        mMapComponent.SendNotice(mCurrentTemp, mChangeValueMessage);
                    }
                    break;
            }
            mCurrentTemp.DataUpdateProperty();
            //保存数据库
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mCurrentTemp.Data, dBProxy).Coroutine();
            return;
        }
    }
}