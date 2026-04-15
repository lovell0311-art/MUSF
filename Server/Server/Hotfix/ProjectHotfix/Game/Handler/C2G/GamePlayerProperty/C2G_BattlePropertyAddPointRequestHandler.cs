using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BattlePropertyAddPointRequestHandler : AMActorRpcHandler<C2G_BattlePropertyAddPointRequest, G2C_BattlePropertyAddPointResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BattlePropertyAddPointRequest b_Request, G2C_BattlePropertyAddPointResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_BattlePropertyAddPointRequest b_Request, G2C_BattlePropertyAddPointResponse b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mServerArea.GameAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            if (b_Request.AddPointNumber <= 0) b_Request.AddPointNumber = 1;

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            var mData = mGamePlayer.Data;

            if (mData.FreePoint == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1100);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色没有自由点数!");
                b_Reply(b_Response);
                return false;
            }
            if (mData.FreePoint < b_Request.AddPointNumber)
            {
                b_Request.AddPointNumber = mData.FreePoint;
            }
            if (b_Request.AddPointNumber <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1100);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色没有自由点数!");
                b_Reply(b_Response);
                return false;
            }

            bool mResult = false;

            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
            {
                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                mBattleKVData.Key = (int)b_GameProperty;
                mBattleKVData.Value = mGamePlayer.GetNumerial(b_GameProperty);
                b_ChangeValue_notice.Info.Add(mBattleKVData);
            }
            switch ((E_GameProperty)b_Request.BattlePropertyId)
            {
                case E_GameProperty.Property_Strength:
                    {
                        mData.Strength += b_Request.AddPointNumber;
                        mData.FreePoint -= b_Request.AddPointNumber;
                        mResult = true;

                        // 更新玩家属性
                        mGamePlayer.DataUpdateProperty();
                      
                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = mGamePlayer.InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinAtteck);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxAtteck);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.AtteckSuccessRate);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PVPAtteckSuccessRate);
                        switch ((E_GameOccupation)mData.PlayerTypeId)
                        {
                            case E_GameOccupation.GrowLancer:
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.DreamRiderPenalize);
                                break;
                        }
                        var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, mPlayer.GameUserId);
                        mMapComponent.SendNotice(mGamePlayer, mChangeValueMessage);
                    }
                    break;
                case E_GameProperty.Property_Willpower:
                    {
                        mData.Willpower += b_Request.AddPointNumber;
                        mData.FreePoint -= b_Request.AddPointNumber;
                        mResult = true;

                        // 更新玩家属性
                        mGamePlayer.DataUpdateProperty();

                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = mGamePlayer.InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP_MAX);

                        switch ((E_GameOccupation)mData.PlayerTypeId)
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
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.AdvanceAttackPower);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.RangeAttack);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.SacredBeast);
                                break;
                            case E_GameOccupation.GrowLancer:
                                break;
                            default:
                                break;
                        }

                        var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, mPlayer.GameUserId);
                        mMapComponent.SendNotice(mGamePlayer, mChangeValueMessage);
                    }
                    break;
                case E_GameProperty.Property_Agility:
                    {
                        mData.Agility += b_Request.AddPointNumber;
                        mData.FreePoint -= b_Request.AddPointNumber;
                        mResult = true;

                        // 更新玩家属性
                        mGamePlayer.DataUpdateProperty();

                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = mGamePlayer.InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.Defense);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.AttackSpeed);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.DefenseRate);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PVPDefenseRate);

                        switch ((E_GameOccupation)mData.PlayerTypeId)
                        {
                            case E_GameOccupation.Archer:
                                {
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinAtteck);
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxAtteck);

                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.AtteckSuccessRate);
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PVPAtteckSuccessRate);
                                }
                                break;
                            case E_GameOccupation.Combat:
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.AdvanceAttackPower);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.RangeAttack);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.SacredBeast);
                                break;
                            case E_GameOccupation.GrowLancer:
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.DreamRiderIrritate);
                                break;
                                
                            default:
                                break;
                        }

                        var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, mPlayer.GameUserId);
                        mMapComponent.SendNotice(mGamePlayer, mChangeValueMessage);
                    }
                    break;
                case E_GameProperty.Property_BoneGas:
                    {
                        mData.BoneGas += b_Request.AddPointNumber;
                        mData.FreePoint -= b_Request.AddPointNumber;
                        mResult = true;

                        // 更新玩家属性
                        mGamePlayer.DataUpdateProperty();

                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = mGamePlayer.InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);
                        switch ((E_GameOccupation)mData.PlayerTypeId)
                        {
                            case E_GameOccupation.Combat:
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.AdvanceAttackPower);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.RangeAttack);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.SacredBeast);
                                break;
                            default:
                                break;
                        }

                        var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, mPlayer.GameUserId);
                        mMapComponent.SendNotice(mGamePlayer, mChangeValueMessage);
                    }
                    break;
                case E_GameProperty.Property_Command:
                    {
                        mData.Command += b_Request.AddPointNumber;
                        mData.FreePoint -= b_Request.AddPointNumber;
                        mResult = true;

                        // 更新玩家属性
                        mGamePlayer.DataUpdateProperty();

                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = mGamePlayer.InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, mPlayer.GameUserId);
                        mMapComponent.SendNotice(mGamePlayer, mChangeValueMessage);
                    }
                    break;
                default:
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1101);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("加点对象异常,不是可加点属性!");
                    }
                    break;
            }

            // 加点成功
            if (mResult)
            {
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mWriteDataComponent.Save(mData, dBProxy2).Coroutine();

                b_Response.PropertyPoint = mData.FreePoint;

                G2C_BattleKVData mChildData = new G2C_BattleKVData();
                mChildData.Key = b_Request.BattlePropertyId;
                mChildData.Value = mGamePlayer.GetNumerialFunc((E_GameProperty)b_Request.BattlePropertyId);
                b_Response.Info.Add(mChildData);

                // 发布 GamePlayerEnterTransferPoint 事件
                ETModel.EventType.GamePlayerConfigureAttributePoint.Instance.gamePlayer = mGamePlayer;
                ETModel.EventType.GamePlayerConfigureAttributePoint.Instance.gameProperty = (E_GameProperty)b_Request.BattlePropertyId;
                ETModel.EventType.GamePlayerConfigureAttributePoint.Instance.addPointNumber = b_Request.AddPointNumber;
                Root.EventSystem.OnRun("GamePlayerConfigureAttributePoint", ETModel.EventType.GamePlayerConfigureAttributePoint.Instance);
            }

            b_Reply(b_Response);
            return true;
        }
    }
}