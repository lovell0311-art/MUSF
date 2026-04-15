
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
using Aop.Api.Domain;
using TencentCloud.Tdmq.V20200217.Models;
using System.Net;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ReincarnateRequestHandler : AMActorRpcHandler<C2G_ReincarnateRequest,G2C_ReincarnateResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ReincarnateRequest b_Request, G2C_ReincarnateResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ReincarnateRequest b_Request, G2C_ReincarnateResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
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
            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (gameplayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            BackpackComponent mBackpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            if (mBackpackComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            var equipComponent = mPlayer.GetCustomComponent<EquipmentComponent>();
            for (EquipPosition Index = EquipPosition.Weapon; Index <= EquipPosition.Mounts; Index++)
            {
                if (equipComponent.GetEquipItemByPosition(Index) != null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2012);
                    b_Reply(b_Response);
                    return false;
                }
            }
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDic = readConfig.GetJson<Reincarnate_InfoConfigJson>().JsonDic;
            int Current = gameplayer.Data.ReincarnateCnt;
            if (jsonDic.TryGetValue(Current+1,out var reincarnate_Infoconfig))
            {
                if (reincarnate_Infoconfig.RestrictionLevel <= gameplayer.Data.Level)
                {
                    if (reincarnate_Infoconfig.DemandGold <= gameplayer.Data.GoldCoin)
                    {
                        if (reincarnate_Infoconfig.DemandCrystal <= mPlayer.Data.YuanbaoCoin)
                        {
                            Dictionary<int,int> Material = new Dictionary<int,int>();
                            if (!string.IsNullOrEmpty(reincarnate_Infoconfig.ReincarnationMaterial))
                                Material = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(reincarnate_Infoconfig.ReincarnationMaterial);
                            Dictionary<int,List<Item>> expend = new Dictionary<int, List<Item>>();
                            foreach (var info in Material)
                            {
                                int Quantity = info.Value;
                                var Itemlist = mBackpackComponent.GetAllItemByConfigID(info.Key);
                                if (Itemlist != null && Itemlist.Count > 0)
                                {
                                    foreach (var item in Itemlist)
                                    {
                                        if (Quantity > 0)
                                        {
                                            if (item.Value.GetProp(EItemValue.Quantity) >= Quantity)
                                            {
                                                if (expend.TryGetValue(info.Key, out var items))
                                                    items.Add(item.Value);
                                                else
                                                    expend.Add(info.Key,new List<Item>(){ item.Value }); 

                                                Quantity = 0;
                                                break;
                                            }
                                            else
                                            {
                                                Quantity -= item.Value.GetProp(EItemValue.Quantity);
                                                if (expend.TryGetValue(info.Key, out var items))
                                                    items.Add(item.Value);
                                                else
                                                    expend.Add(info.Key, new List<Item>() { item.Value });

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1609);
                                    b_Reply(b_Response);
                                    return true;
                                }
                            }
                            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                            DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                            //消耗
                            if (reincarnate_Infoconfig.DemandGold != 0)
                            {
                                gameplayer.UpdateCoin(E_GameProperty.GoldCoin,-reincarnate_Infoconfig.DemandGold,"转生消耗");
                                await dBProxy.Save(gameplayer.Data);
                                G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                mBattleKVData.Key = (int)E_GameProperty.GoldCoin;
                                mBattleKVData.Value = gameplayer.Data.GoldCoin;
                                mChangeValue_notice.Info.Add(mBattleKVData);
                                mPlayer.Send(mChangeValue_notice);
                            }
                            if (reincarnate_Infoconfig.DemandCrystal != 0)
                            {
                                gameplayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -reincarnate_Infoconfig.DemandCrystal, "转生消耗");
                                await dBProxy.Save(mPlayer.Data);
                                G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                mBattleKVData.Key = (int)E_GameProperty.YuanbaoCoin;
                                mBattleKVData.Value = mPlayer.Data.YuanbaoCoin;
                                mChangeValue_notice.Info.Add(mBattleKVData);
                                mPlayer.Send(mChangeValue_notice);
                            }
                            foreach (var info in Material)
                            {
                                int Cnt = info.Value;
                                if (expend.TryGetValue(info.Key, out var Items))
                                {
                                    foreach (var item in Items)
                                    {
                                        if (Cnt == 0) break;
                                        if (item.GetProp(EItemValue.Quantity) <= Cnt)
                                        {
                                            mBackpackComponent.UseItem(item.ItemUID, "转生消耗", item.GetProp(EItemValue.Quantity));
                                            Cnt -= item.GetProp(EItemValue.Quantity);
                                        }
                                        else
                                        {
                                            mBackpackComponent.UseItem(item.ItemUID, "转生消耗", Cnt);
                                            Cnt = 0;
                                        }
                                    }
                                }
                            }

                            var gamePlayerData = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<CreateRole_InfoConfigJson>().JsonDic;
                            if (gamePlayerData.TryGetValue(gameplayer.Data.PlayerTypeId, out var Info))
                            {
                                gameplayer.Data.Agility = 0;
                                gameplayer.Data.BoneGas = 0;
                                gameplayer.Data.Command = 0;
                                gameplayer.Data.Strength = 0;
                                gameplayer.Data.Willpower = 0;
                            }
                            
                            gameplayer.Data.FreePoint = reincarnate_Infoconfig.ReincarnatePoints;
                            gameplayer.Data.Level = 1;
                            gameplayer.Data.Exp = 0;

                            gameplayer.Data.ReincarnateCnt++;

                            G2C_ChangeValue_notice mExprienceNotice = new G2C_ChangeValue_notice();
                            mExprienceNotice.GameUserId = gameplayer.InstanceId;
                            G2C_BattleKVData mExpMessage = new G2C_BattleKVData();
                            mExpMessage.Key = (int)E_GameProperty.ExprienceDrop;
                            mExpMessage.Value = 0;
                            mExprienceNotice.Info.Add(mExpMessage);
                            mExpMessage = new G2C_BattleKVData();
                            mExpMessage.Key = (int)E_GameProperty.Exprience;
                            mExpMessage.Value = gameplayer.Data.Exp;
                            mExprienceNotice.Info.Add(mExpMessage);

                            gameplayer.DataUpdateProperty();

                            var mHP_MAX = gameplayer.GetNumerial(E_GameProperty.PROP_HP_MAX);
                            if (gameplayer.UnitData.Hp != mHP_MAX) gameplayer.UnitData.Hp = mHP_MAX;

                            var mMP_MAX = gameplayer.GetNumerial(E_GameProperty.PROP_MP_MAX);
                            if (gameplayer.UnitData.Mp != mMP_MAX) gameplayer.UnitData.Mp = mMP_MAX;

                            var mSd_MAX = gameplayer.GetNumerial(E_GameProperty.PROP_SD_MAX);
                            if (gameplayer.UnitData.SD != mSd_MAX) gameplayer.UnitData.SD = mSd_MAX;

                            var mAG_MAX = gameplayer.GetNumerial(E_GameProperty.PROP_AG_MAX);
                            if (gameplayer.UnitData.AG != mAG_MAX) gameplayer.UnitData.AG = mAG_MAX;

                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                            {
                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                mBattleKVData.Key = (int)b_GameProperty;
                                mBattleKVData.Value = gameplayer.GetNumerial(b_GameProperty);
                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                            }

                            AddPropertyNotice(mExprienceNotice, E_GameProperty.Level);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_HP);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_HP_MAX);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_MP);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_MP_MAX);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_SD);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_SD_MAX);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_AG);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_AG_MAX);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.FreePoint);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.Property_Strength);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.Property_Willpower);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.Property_Agility);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.Property_BoneGas);
                            AddPropertyNotice(mExprienceNotice, E_GameProperty.Property_Command);
                            mPlayer.Send(mExprienceNotice);
                            await dBProxy.Save(gameplayer.Data);
                            DataCacheManageComponent mDataCacheComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();

                            var mDataCache_Skill = mDataCacheComponent.Get<DBMasterData>();
                            if (mDataCache_Skill != null)
                            {
                                var mData = mDataCache_Skill.OnlyOne();
                                if (mData != null)
                                {
                                    mData.PropertyPoint += reincarnate_Infoconfig.MasterPoints;
                                    mWriteDataComponent.Save(mData, dBProxy).Coroutine();
                                    await dBProxy.Save(mData);
                                }
                            }
                            
                            b_Response.ReincarnateCnt = gameplayer.Data.ReincarnateCnt;
                            b_Reply(b_Response);
                            return true;
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3312);
                            b_Reply(b_Response);
                            return true;
                        }
                    }
                    else
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                        b_Reply(b_Response);
                        return true;
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3601);
                    b_Reply(b_Response);
                    return true;
                }
            }
            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3600);
            b_Reply(b_Response);
            return true;

        }
    }
}