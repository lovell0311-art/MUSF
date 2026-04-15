using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using TencentCloud.Mrs.V20200910.Models;

namespace ETHotfix
{


    public static class EquipmentSetComponentSystem
    {
        /// <summary>
        /// 添加物品，不更新属性词条
        /// </summary>
        /// <param name="self"></param>
        /// <param name="item">需要添加的物品</param>
        /// <returns></returns>
        public static ItemSet AddItemNotUpdate(this EquipmentSetComponent self, Item item)
        {
            if (!item.HaveSetOption())
            {
                return null;
            }
            int setId = item.GetProp(EItemValue.SetId);
            ItemSet itemSet;
            self.AllItemSet.TryGetValue(setId, out itemSet);
            if (itemSet == null)
            {
                itemSet = ItemSetFactory.Create(item.GetProp(EItemValue.SetId));
                if (itemSet == null)
                {
                    Log.Warning($"未知的套装Id,Id = {setId}");
                    return null;
                }
                self.AllItemSet.Add(setId, itemSet);
            }


            itemSet.AddItemNotUpdate(item);
            return itemSet;
        }

        /// <summary>
        /// 添加物品 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="item">需要添加的物品</param>
        public static void AddItem(this EquipmentSetComponent self, Item item)
        {
            var itemSet = self.AddItemNotUpdate(item);
            if (itemSet != null && itemSet.NeedUpdate)
            {
                itemSet.UpdateAttrEntry();
            }
        }

        /// <summary>
        /// 移除物品，不更新属性词条
        /// </summary>
        /// <param name="self"></param>
        /// <param name="item">需要移除的物品</param>
        /// <returns></returns>
        public static ItemSet RemoveItemNotUpdate(this EquipmentSetComponent self, Item item)
        {
            if (!item.HaveSetOption())
            {
                return null;
            }
            int setId = item.GetProp(EItemValue.SetId);
            if (self.AllItemSet.TryGetValue(setId, out var itemSet))
            {
                itemSet.RemoveItemNotUpdate(item);
                return itemSet;
            }
            else
            {
                Log.Warning($"未知的套装Id,Id = {setId}");
            }
            return null;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="self"></param>
        /// <param name="item">需要移除的物品</param>
        public static void RemoveItem(this EquipmentSetComponent self, Item item)
        {
            var itemSet = self.RemoveItemNotUpdate(item);
            if (itemSet != null && itemSet.NeedUpdate)
            {
                if (itemSet.ItemId2Count.Count != 0)
                {
                    itemSet.UpdateAttrEntry();
                }
                else
                {
                    // TODO 背包中，不存在拥有当前套装的物品
                    // 将其移除
                    self.AllItemSet.Remove(itemSet.ConfigId);
                    itemSet.Dispose();
                }
            }
        }

        /// <summary>
        /// 应用套装属性
        /// </summary>
        /// <param name="self"></param>
        public static void ApplyEquipSetProp(this EquipmentSetComponent self)
        {
            if (self.AllItemSet.Count == 0)
            {
                return;
            }
            var gamePlayer = self.Parent.GetCustomComponent<GamePlayer>();
            var itemAttrEntryManager = Root.MainFactory.GetCustomComponent<ItemAttrEntryManager>();
            foreach (ItemSet itemSet in self.AllItemSet.Values)
            {
                for (int i = 0, count = itemSet.ValidAttrEntryId.Count;
                    i < count; ++i)
                {
                    var setEntry = itemAttrEntryManager.GetOrCreate(itemSet.ValidAttrEntryId[i], 0);
                    if (setEntry == null)
                    {
                        Log.Warning($"没找到物品属性词条。entryId={itemSet.ValidAttrEntryId[i]}");
                    }
                    else
                    {
                        setEntry.ApplyPropTo(gamePlayer);
                    }
                }
            }
        }
        /// <summary>
        /// 应用宠物卓越到角色身上
        /// </summary>
        /// <param name="self"></param>
        public static void ApplyEquipSetPets(this EquipmentSetComponent self)
        {
            var gamePlayer = self.Parent.GetCustomComponent<GamePlayer>();

            //宠物幸运
            gamePlayer.AddEquipProperty(E_GameProperty.LucklyAttackRate, 500);

            var itemAttrEntryManager = Root.MainFactory.GetCustomComponent<ItemAttrEntryManager>();
            if (gamePlayer.Pets != null && !gamePlayer.Pets.IsDeath)
            {
                //卓越属性
                foreach (var ExcellentId in gamePlayer.Pets.dBPetsData.ExcellentId)
                {
                    var setEntry = itemAttrEntryManager.GetOrCreate(ExcellentId, 0);
                    if (setEntry == null)
                    {
                        Log.Warning($"没找到物品属性词条。entryId={ExcellentId}");
                    }
                    else
                    {
                        setEntry.ApplyPropTo(gamePlayer);
                    }
                }
                int Value = gamePlayer.Pets.dBPetsData.EnhanceLv * 3 + gamePlayer.Pets.dBPetsData.AdvancedLevel * 10;
                int MinMagicAtteck = gamePlayer.Pets.GetNumerial(E_GameProperty.MinMagicAtteck) + Value;
                int MaxMagicAtteck = gamePlayer.Pets.GetNumerial(E_GameProperty.MaxMagicAtteck)+ Value;
                int MinAtteck = gamePlayer.Pets.GetNumerial(E_GameProperty.MinAtteck) + Value;
                int MaxAtteck = gamePlayer.Pets.GetNumerial(E_GameProperty.MaxAtteck) + Value;
                int Defense = gamePlayer.Pets.GetNumerial(E_GameProperty.Defense);
                float value = 0.0f;
                switch (gamePlayer.Pets.dBPetsData.ConfigID)
                {
                    case 102: value = 0.15f;break;
                    case 101: value = 0.25f; break;
                    case 105: value = 0.50f; break;
                    case 106: value = 0.50f; break;
                    case 107: value = 0.50f; break;
                    default:  value = 0.10f; break;
                }
                if (gamePlayer.Pets.Config.AttackType == 0)
                {
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, (int)(MaxAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, (int)(MinAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, (int)(MinAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, (int)(MaxAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, (int)(MinAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, (int)(MaxAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.Defense, (int)(Defense * value));
                }
                else
                {
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, (int)(MaxMagicAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, (int)(MinMagicAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, (int)(MinMagicAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, (int)(MaxMagicAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, (int)(MinMagicAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, (int)(MaxMagicAtteck * value));
                    gamePlayer.AddEquipProperty(E_GameProperty.Defense, (int)(Defense * value));
                }
                #region 复古版修改基础属性通过宠物ID设置
                //基础属性
                //foreach (int SkillID in gamePlayer.Pets.dBPetsData.SkillId)
                //{
                //    var SkillJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_SkillConfigJson>().JsonDic;
                //    Pets_SkillConfig SkillInfo = null;
                //    if (!SkillJsonDic.TryGetValue(SkillID, out SkillInfo)) continue;
                //    if (SkillInfo.skillType == 1) continue;
                //    switch (SkillID)
                //    {
                //        case 101001:
                //            gamePlayer.AddEquipProperty(E_GameProperty.PetsDamageAbsorption, SkillInfo.OtherDataDic[2]);
                //            break;
                //        case 101003:
                //            gamePlayer.AddEquipProperty(E_GameProperty.MinAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteckPct, SkillInfo.OtherDataDic[2]); 
                //            break;
                //        case 101005:
                //            gamePlayer.AddEquipProperty(E_GameProperty.Defense, SkillInfo.OtherDataDic[2]);
                //            break;
                //        case 101007:
                //            gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteckPct, SkillInfo.OtherDataDic[2]);
                //            break;
                //        case 101013:
                //            gamePlayer.AddEquipProperty(E_GameProperty.MinAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteckPct, SkillInfo.OtherDataDic[2]);
                //            gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteckPct, SkillInfo.OtherDataDic[2]);
                //            break;
                //        case 101011:
                //            {
                //                if (gamePlayer.Pets != null && gamePlayer.Pets.dBPetsData.ConfigID == 105 && gamePlayer.Player.OnlineStatus == EOnlineStatus.Online)
                //                {
                //                    var b_BattleComponent = gamePlayer.CurrentMap?.GetCustomComponent<BattleComponent>();
                //                    gamePlayer.RemoveHealthState(E_BattleSkillStats.FangYuHuZhao, b_BattleComponent, true);
                //                    gamePlayer.UpdateHealthState();
                //                    // 拉宠物buff
                //                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                //                    {
                //                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                //                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.DeathRemoveTask;
                //                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                //                        mBattleSyncTimer.SyncWaitTime = 2 * 60 * 1000;
                //                        mBattleSyncTimer.NextWaitTime = CustomFrameWork.Help_TimeHelper.GetNow() + mBattleSyncTimer.SyncWaitTime;

                //                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                //                        {
                //                            if (b_CombatSource.IsDisposeable) return;

                //                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.FangYuHuZhao, b_BattleComponent);
                //                            b_CombatSource.UpdateHealthState();
                //                        };
                //                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                //                        {
                //                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                //                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var hp_Curse) == false)
                //                            {
                //                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                //                            }
                //                            Pets pets = (b_CombatSource as GamePlayer).Pets;
                //                            if (pets == null || (pets != null && (b_CombatSource as GamePlayer).Pets.dBPetsData.ConfigID != 105))
                //                            {
                //                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                //                            }
                //                            if (hp_Curse.CacheDatas.TryGetValue(0, out var hpCacheDatas) == false)
                //                            {
                //                                hp_Curse.CacheDatas[0] = new CombatSource.C_CombatUnitStatsSource();
                //                            }
                //                            if (hpCacheDatas.CacheData == null) hpCacheDatas.CacheData = new Dictionary<int, int>();

                //                            var mMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                //                            mMax = (int)(mMax * 0.1f);

                //                            hpCacheDatas.CacheData.TryGetValue(0, out var mTempValue);
                //                            if (mTempValue != mMax)
                //                            {
                //                                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                //                                mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;
                //                                G2C_BattleKVData mData = new G2C_BattleKVData();
                //                                mData.Key = (int)E_GameProperty.FangYuHuZhao;
                //                                mData.Value = mMax;
                //                                mChangeValueMessage.Info.Add(mData);

                //                                b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                //                            }
                //                            hpCacheDatas.CacheData[0] = mMax;

                //                            b_TimerTask.NextWaitTime = b_TimerTask.NextWaitTime + b_TimerTask.SyncWaitTime;
                //                            b_CombatSource.AddTask(b_TimerTask);

                //                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                //                        };
                //                        return mBattleSyncTimer;
                //                    };
                //                    gamePlayer.AddHealthState(E_BattleSkillStats.FangYuHuZhao, 0, 0, 0, mCreateFunc, b_BattleComponent, b_SendNotice: false);
                //                    gamePlayer.UpdateHealthState();
                //                    if (gamePlayer.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var hp_Curse))
                //                    {
                //                        if (hp_Curse.CacheDatas.TryGetValue(0, out var hpCacheDatas))
                //                        {
                //                            if (hpCacheDatas.CacheData == null) hpCacheDatas.CacheData = new Dictionary<int, int>();

                //                            hpCacheDatas.CacheData[0] = 0;
                //                        }
                //                    }
                //                }
                //            }
                //            break;
                //        case 101014:
                //            {
                //                gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteckPct, SkillInfo.OtherDataDic[2]);
                //                gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteckPct, SkillInfo.OtherDataDic[2]);
                //                gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteckPct, SkillInfo.OtherDataDic[2]);
                //                gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteckPct, SkillInfo.OtherDataDic[2]);
                //                gamePlayer.AddEquipProperty(E_GameProperty.MinAtteckPct, SkillInfo.OtherDataDic[2]);
                //                gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteckPct, SkillInfo.OtherDataDic[2]);
                //                gamePlayer.AddEquipProperty(E_GameProperty.DefensePct, SkillInfo.OtherDataDic[2]);
                //            }
                //            break;
                //        case 101015:
                //            {
                //                gamePlayer.AddEquipProperty(E_GameProperty.DefensePct, SkillInfo.OtherDataDic[2]);
                //            }
                //            break;

                //    }
                //}
                #endregion

                //switch (gamePlayer.Pets.dBPetsData.ConfigID)
                //{
                //    case 100:
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 10);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 10);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 10);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 10);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 10);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 10);
                //        break;
                //    case 101:
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 30);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 30);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 30);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 30);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 30);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 30);
                //        break;
                //    case 102:
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 20);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 20);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 20);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 20);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 20);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 20);
                //        break;
                //    case 105:
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 70);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 70);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 70);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 70);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 70);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 70);
                //        gamePlayer.AddEquipProperty(E_GameProperty.Defense, 70);
                //        gamePlayer.AddEquipProperty(E_GameProperty.InjuryValueRate_Increase, 3000);
                //        break;
                //    case 106:
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 50);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 50);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 50);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 50);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 50);
                //        gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 50);
                //        gamePlayer.AddEquipProperty(E_GameProperty.Defense, 50); 
                //        gamePlayer.AddEquipProperty(E_GameProperty.HurtValueAbsorbRate, 1000);
                //        break;
                //}
                //强化属性
                //if (gamePlayer.Pets.dBPetsData.EnhanceLv >= 1)
                //{
                //    var PetsJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
                //    if (PetsJsonDic.TryGetValue(gamePlayer.Pets.dBPetsData.ConfigID, out var Info))
                //    {
                //        var PetsEA = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_EnhanceAttributeConfigJson>().JsonDic;
                //        foreach (var E in Info.EA)
                //        {
                //            if (PetsEA.TryGetValue(E, out var EAInfo))
                //            {
                //                int value = gamePlayer.Pets.dBPetsData.EnhanceLv * EAInfo.Enhance;
                //                switch (EAInfo.Id)
                //                { 
                //                    case 1:
                //                    case 4:
                //                    case 5:
                //                        gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, value);
                //                        gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, value);
                //                        gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, value);
                //                        gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, value);
                //                        gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, value);
                //                        gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, value);
                //                        break;
                //                    case 2:
                //                    case 3:
                //                        gamePlayer.AddEquipProperty(E_GameProperty.Defense, value);
                //                        break;
                //                }
                //            }
                //        }
                //    }
                //}

            }
        }
        /// <summary>
        /// 更新称号
        /// </summary>
        /// <param name="self"></param>
        public static void ApplyEquipSetTitle(this EquipmentSetComponent self)
        {
            var Shop = self.Parent.GetCustomComponent<PlayerTitle>();
            var gamePlayer = self.Parent.GetCustomComponent<GamePlayer>();
            if (Shop != null)
            {
                if (Shop.ListString.Count >= 1)
                {
                    foreach ( var item in Shop.ListString) 
                    {
                        switch (item.TitleID)
                        {
                            case 60001:
                                {
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 50);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 50);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 50);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 50);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 50);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 50);
                                }
                                break;
                            case 60002:
                                {
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 30);
                                }
                                break;
                            case 60003:
                                {
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 20);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 20);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 20);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 20);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 20);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 20);
                                }
                                break;
                            case 60004:
                                {
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.Defense, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.InjuryValueRate_Increase, 200);
                                }
                                break;
                            case 60005:
                                {
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 30);
                                }
                                break;
                            case 60006:
                                {
                                    gamePlayer.AddEquipProperty(E_GameProperty.Defense, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 30);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 30);
                                }
                                break;
                            case 60007:
                                {
                                    gamePlayer.AddEquipProperty(E_GameProperty.Defense, 40);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 40);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 40);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 40);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 40);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 40);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 40);
                                }
                                break;
                            case 60008:
                                {
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, 10);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, 10);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, 10);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, 10);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, 10);
                                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, 10);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static void ApplyEquipSetVip(this EquipmentSetComponent self)
        {
            var Shop = self.Parent.GetCustomComponent<PlayerShopMallComponent>();
            var gamePlayer = self.Parent.GetCustomComponent<GamePlayer>();
            if (Shop != null && gamePlayer != null)
            {
                if (Shop.GetPlayerShopState(DeviationType.MaxMonthlyCard))
                {
                    gamePlayer.AddEquipProperty(E_GameProperty.ExperienceBonus, 200);
                    gamePlayer.AddEquipProperty(E_GameProperty.ExplosionRate, 10);
                    //gamePlayer.AddEquipProperty(E_GameProperty.GoldCoinMarkup, 20);

                    //if (gamePlayer.Data.Level >= 220)
                    //{
                    //    gamePlayer.AddEquipProperty(E_GameProperty.DefensePct, 10);
                    //    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteckPct, 10);
                    //    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteckPct, 10);
                    //    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteckPct, 10);
                    //    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteckPct, 10);
                    //    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteckPct, 10);
                    //    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteckPct, 10);
                    //}
                }
            }
        }
        /// <summary>
        /// 血脉属性应用
        /// </summary>
        /// <param name="self"></param>
        public static void ApplyEquipBloodAwakening(this EquipmentSetComponent self)
        {
            var mBloodAwakening = self.Parent.GetCustomComponent<PlayerBloodAwakeningComponent>();
            var gamePlayer = self.Parent.GetCustomComponent<GamePlayer>();
            if (mBloodAwakening.UseBloodAwakeningId == 0) return;
            if (mBloodAwakening.BloodAwakeningInfo != null && 
                mBloodAwakening.BloodAwakeningInfo.TryGetValue(mBloodAwakening.UseBloodAwakeningId,out var Info))
            {
                var itemAttrEntryManager = Root.MainFactory.GetCustomComponent<ItemAttrEntryManager>();
                foreach (var AttrEntryId in Info.Attribute)
                {
                    var setEntry = itemAttrEntryManager.GetOrCreate(AttrEntryId, 0);
                    if (setEntry == null)
                    {
                        Log.Warning($"没找到属性词条。entryId={AttrEntryId}");
                    }
                    else
                    {
                        setEntry.ApplyPropTo(gamePlayer);
                    }
                }
            }
        }
    }
}
