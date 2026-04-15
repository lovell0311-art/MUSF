using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using SharpCompress.Common;

namespace ETHotfix.ItemUpdateProp
{
    /// <summary>
    /// 物品默认属性更新方法
    /// </summary>
    [ItemUpdateProp]
    public class Default : IItemUpdatePropHandler
    {
        /// <summary>
        /// 更新全部属性
        /// </summary>
        /// <param name="item">更新的物品</param>
        public void UpdateProp(Item item)
        {
            StartUpdate(item);

            UpdateDurability(item);
            UpdateBattleProp(item);
            UpdateRequire(item);
            UpdateBuyMoney(item);
            UpdateSellMoney(item);
            UpdateRepairMoney(item);

            EndUpdate(item);
        }

        /// <summary>
        /// 应用装备属性到单位
        /// </summary>
        /// <param name="item"></param>
        /// <param name="equipCmt"></param>
        /// <param name="inPos"></param>
        public virtual void ApplyEquipProp(Item item, EquipmentComponent equipCmt, EquipPosition pos)
        {
            var gamePlayer = equipCmt.mPlayer.GetCustomComponent<GamePlayer>();
            #region 耐久应用计算
            float currDurabilityState = 1f;
            if (item.GetProp(EItemValue.DurabilityMax) > 0)
            {
                currDurabilityState = ((float)item.GetProp(EItemValue.Durability) / (float)item.GetProp(EItemValue.DurabilityMax));
            }

            if (currDurabilityState > 0.5f)
            {
                currDurabilityState = 1f;
            }
            else if (currDurabilityState > 0.3f)
            {
                currDurabilityState = 0.8f;
            }
            else if (currDurabilityState > 0.2f)
            {
                currDurabilityState = 0.7f;
            }
            else if (currDurabilityState > 0.0001f)
            {
                currDurabilityState = 0.5f;
            }
            else
            {
                currDurabilityState = 0f;
            }
            #endregion

            // 普通属性

            if (pos == EquipPosition.Weapon || pos == EquipPosition.Shield)
            {
                // 双持武器，属性应用
                float applyPct = 1f;
                if (pos == EquipPosition.Shield && item.ConfigData.Slot == (int)EquipPosition.Weapon)
                {
                    // 副手武器，应用 20% 的属性
                    applyPct = 0.2f;
                }
                if (item.Type == EItemType.MagicBook) applyPct = 1f;

                gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, (int)(item.GetProp(EItemValue.DamageMin) * applyPct * currDurabilityState));
                gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, (int)(item.GetProp(EItemValue.DamageMax) * applyPct * currDurabilityState));
                gamePlayer.AddEquipProperty(E_GameProperty.MagicRate_Increase, (int)(item.GetProp(EItemValue.MagicDamage) * applyPct * currDurabilityState));
            }

            gamePlayer.AddEquipProperty(E_GameProperty.DefenseRate, (int)(item.GetProp(EItemValue.DefenseRate) * currDurabilityState));
            gamePlayer.AddEquipProperty(E_GameProperty.Defense, (int)(item.GetProp(EItemValue.Defense) * currDurabilityState));
            gamePlayer.AddEquipProperty(E_GameProperty.AttackBonus2, (int)(item.GetProp(EItemValue.UpPetAttackPct) * currDurabilityState));
            if (pos == EquipPosition.Weapon)
            {
                gamePlayer.AddEquipProperty(E_GameProperty.AttackSpeed, (int)(item.GetProp(EItemValue.AttackSpeed) * currDurabilityState));
            }
            else if(pos == EquipPosition.Shield)
            {
                gamePlayer.AddEquipProperty(E_GameProperty.DamnationRate_Increase, (int)(item.GetProp(EItemValue.Curse) * currDurabilityState));
                if (item.Type == EItemType.MagicBook)
                {
                    gamePlayer.AddEquipProperty(E_GameProperty.AttackSpeed, (int)(item.GetProp(EItemValue.AttackSpeed) * currDurabilityState));
                }
            }
            else if(pos == EquipPosition.HandGuard)
            {
                gamePlayer.AddEquipProperty(E_GameProperty.AttackSpeed, (int)(item.GetProp(EItemValue.AttackSpeed) * currDurabilityState));
            }
            if (currDurabilityState < 0.001)
            {
                // 耐久为 0
                return;
            }
            var itemAttrEntryManager = Root.MainFactory.GetCustomComponent<ItemAttrEntryManager>();

            // TODO 基础属性
            if (item.ConfigData.BaseAttrId.Count != 0)
            {
                bool IsE = item.HaveExcellentOption();
                int itemLevel = item.GetProp(EItemValue.Level);
                foreach (var entryId in item.ConfigData.BaseAttrId)
                {
                    var baseEntry = itemAttrEntryManager.GetOrCreate(entryId, itemLevel);
                    if (baseEntry == null)
                    {
                        Log.Error($"没找到物品属性词条。entryId={entryId}");
                    }
                    else
                    {
                        baseEntry.ApplyPropTo(gamePlayer, currDurabilityState, IsE);
                    }
                }
            }
            // TODO 卓越属性
            if (item.data.ExcellentEntry.Count != 0)
            {
                foreach (var entryId in item.data.ExcellentEntry)
                {
                    var excEntry = itemAttrEntryManager.GetOrCreate(entryId, 0);
                    if (excEntry == null)
                    {
                        Log.Error($"没找到物品属性词条。entryId={entryId}");
                    }
                    else
                    {
                        excEntry.ApplyPropTo(gamePlayer);
                    }
                }
            }
            // TODO 额外属性 套装附加属性
            if (item.data.ExtraEntry.Count != 0)
            {
                foreach (var kv in item.data.ExtraEntry)
                {
                    var excEntry = itemAttrEntryManager.GetOrCreate(kv.Key, kv.Value);
                    if (excEntry == null)
                    {
                        Log.Error($"没找到物品属性词条。entryId={kv.Key}，level={kv.Value}");
                    }
                    else
                    {
                        excEntry.ApplyPropTo(gamePlayer);
                    }
                }
            }
            // TODO 特殊属性-翅膀
            if (item.data.SpecialEntry.Count != 0)
            {
                foreach (var kv in item.data.SpecialEntry)
                {
                    switch (kv.Key)
                    {
                        case 600005:
                            {
                                // 魔法值增加 50+(强化等级)*5
                                int itemLevel = item.GetProp(EItemValue.Level);
                                gamePlayer.AddEquipProperty(E_GameProperty.PROP_MP_MAX, 50 + (itemLevel * 5));
                            }
                            break;
                        case 600006:
                            {
                                // 生命值增加 50+(强化等级)*5
                                int itemLevel = item.GetProp(EItemValue.Level);
                                gamePlayer.AddEquipProperty(E_GameProperty.PROP_HP_MAX, 50 + (itemLevel * 5));
                            }
                            break;
                        case 600022:
                            {
                                // 统率增加 10+(强化等级)*5
                                int itemLevel = item.GetProp(EItemValue.Level);
                                gamePlayer.AddEquipProperty(E_GameProperty.Property_Command, 10 + (itemLevel * 5));
                            }
                            break;
                        default:
                            {
                                var excEntry = itemAttrEntryManager.GetOrCreate(kv.Key, kv.Value);
                                if (excEntry == null)
                                {
                                    Log.Error($"没找到物品属性词条。entryId={kv.Key}，level={kv.Value}");
                                }
                                else
                                {
                                    excEntry.ApplyPropTo(gamePlayer);
                                }
                            }
                            break;
                    }

                   
                }
            }



            // TODO 追加
            int appendId = item.GetProp(EItemValue.OptValue);
            int appendLevel = item.GetProp(EItemValue.OptLevel);
            if (appendId > 0 && appendLevel > 0)
            {
                appendId += 800000;
                var appendEntry = itemAttrEntryManager.GetOrCreate(appendId, appendLevel);
                if (appendEntry == null)
                {
                    Log.Error($"没找到物品属性词条。appendEntry={appendId}");
                }
                else
                {
                    appendEntry.ApplyPropTo(gamePlayer);
                }
            }

            // TODO 再生
            int orecycledId = item.GetProp(EItemValue.OrecycledID);
            int orecycledLevel = item.GetProp(EItemValue.OrecycledLevel);
            if (orecycledId > 0 && orecycledLevel > 0)
            {
                var orecycledEntry = itemAttrEntryManager.GetOrCreate(orecycledId, orecycledLevel);
                if (orecycledEntry == null)
                {
                    Log.Error($"没找到物品属性词条。orecycledEntry={orecycledId}");
                }
                else
                {
                    orecycledEntry.ApplyPropTo(gamePlayer);
                }
            }

            // TODO 幸运
            if (item.IsLuckyEquip())
            {
                // 幸运(灵魂宝石之成功机率 + 25 %)
                // 幸运(会心一击率 + 5 %)
                gamePlayer.AddEquipProperty(E_GameProperty.LucklyAttackRate, 5 * 100);
            }


            // TODO 镶嵌
            if (item.GetProp(EItemValue.FluoreSlotCount) >= 1)
            {
                var Embedjson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<FluoreSet_AttrConfigJson>().JsonDic;
                for (EItemValue i = EItemValue.FluoreSlot1; i <= EItemValue.FluoreSlot5; i++)
                {
                    int Id1 = item.GetProp(i);
                    if (Id1 > 0)
                    {
                        int ConfigId = Id1 / 100;
                        int ConfigLevel = Id1 % 100;
                        if (Embedjson.TryGetValue(ConfigId, out var fluoreSet_AttrConfig))
                        {
                            int setEntry = fluoreSet_AttrConfig.ToItemConfig(ConfigId, ConfigLevel);

                            gamePlayer.EmbedApplyPropTo(fluoreSet_AttrConfig.Attribute, setEntry);
                            
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新开始
        /// </summary>
        /// <param name="item">更新的物品</param>
        public virtual void StartUpdate(Item item)
        {
            
        }

        /// <summary>
        /// 更新耐久
        /// </summary>
        /// <param name="item">更新的物品</param>
        public virtual void UpdateDurability(Item item)
        {
            int level = item.GetProp(EItemValue.Level);
            int dur = item.ConfigData.Durable;
            int diff = item.GetProp(EItemValue.DurabilityMax);
            if(dur > 0)
            {
                if (item.HaveSetOption())
                {
                    // 套装
                    if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemCustom_SetAttrConfigJson>().JsonDic.TryGetValue(item.ConfigID, out var setConf))
                    {
                        dur = setConf.Durable;
                    }
                }
                else if (item.HaveExcellentOption())
                {
                    // 卓越
                    if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemCustom_ExcAttrConfigJson>().JsonDic.TryGetValue(item.ConfigID, out var excConf))
                    {
                        dur = excConf.Durable;
                    }
                }

                if (level < 5)
                {
                    dur = dur + level;
                }
                else
                {
                    if (level == 10)
                    {
                        dur = dur + level * 2 - 3;
                    }
                    else if (level == 11)
                    {
                        dur = dur + level * 2 - 1;
                    }
                    else if (level == 12)
                    {
                        dur = dur + level * 2 + 2;
                    }
                    else if (level == 13)
                    {
                        dur = dur + level * 2 + 6;
                    }
                    else if (level == 14)
                    {
                        dur = dur + 39;
                    }
                    else if (level == 15)
                    {
                        dur = dur + 47;
                    }
                    else if (level == 16)
                    {
                        dur = dur + 55;
                    }
                    else
                    {
                        dur = dur + level * 2 - 4;
                    }
                }
                if (dur > 255)
                {
                    dur = 255;
                }
            }
            else
            {
                dur = 0;
            }
            item.SetProp(EItemValue.DurabilityMax,dur);
        }

        /// <summary>
        /// 根据等级，获取增加的值
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private int Level2Value(int level)
        {
            switch (level - 9)
            {
                case 6:return 9 + 8 + 7 + 6 + 5 + 4;
                case 5:return 8 + 7 + 6 + 5 + 4;
                case 4:return 7 + 6 + 5 + 4;
                case 3:return 6 + 5 + 4;
                case 2:return 5 + 4;
                case 1:return 4;
            }
            return 0;
        }

        /// <summary>
        /// 更新战斗属性
        /// </summary>
        /// <param name="item">更新的物品</param>
        public virtual void UpdateBattleProp(Item item)
        {
            ItemConfig conf = item.ConfigData;
            int level = item.GetProp(EItemValue.Level);
            int damageMin = conf.DamageMin;
            int damageMax = conf.DamageMax;
            int magicPct = conf.MagicPct;
            int curse = conf.Curse;
            int upPet = conf.UpPet;
            int itemLevel = conf.Level;
            int confLevel = conf.Level;
            int def = conf.Defense;
            int defRate = conf.DefenseRate;
            int magicDef = 0;
            int attackSpeed = conf.AttackSpeed;
            int walkSpeed = conf.WalkSpeed;
            // 自定义配置
            ItemCustom_ExcAttrConfig excConf = null;
            ItemCustom_SetAttrConfig setConf = null;

            if (item.HaveSetOption())
            {
                // 有套装属性
                itemLevel += 30;
                if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemCustom_SetAttrConfigJson>().JsonDic.TryGetValue(item.ConfigID, out setConf))
                {
                    damageMin = setConf.DamageMin;
                    damageMax = setConf.DamageMax;
                    magicPct = setConf.MagicPct;
                    curse = setConf.Curse;
                    def = setConf.Defense;
                    defRate = setConf.DefenseRate;
                    attackSpeed = setConf.AttackSpeed;
                }
            }
            else if(item.HaveExcellentOption())
            {
                // 卓越
                if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemCustom_ExcAttrConfigJson>().JsonDic.TryGetValue(item.ConfigID, out excConf))
                {
                    damageMin = excConf.DamageMin;
                    damageMax = excConf.DamageMax;
                    magicPct = excConf.MagicPct;
                    curse = excConf.Curse;
                    def = excConf.Defense;
                    defRate = excConf.DefenseRate;
                    attackSpeed = excConf.AttackSpeed;
                }
            }
            
            // 最大攻击
            if(damageMax > 0)
            {
                if(item.HaveSetOption() && itemLevel != 0 && confLevel != 0)
                {
                    if(setConf == null)
                    {
                        // 使用自定义属性
                        damageMax += (damageMin * 25) / confLevel + 5;
                        damageMax += itemLevel / 40 + 5;
                        if (item.HaveExcellentOption() && confLevel != 0)
                        {
                            // 有套装和卓越 额外加10点
                            damageMax += 10;
                        }
                    }
                }
                else if(item.HaveExcellentOption() && confLevel != 0)
                {
                    if(excConf == null)
                    {
                        damageMax += (damageMin * 25) / confLevel + 5;
                    }
                }

                int tmpLevel = (level > 9) ? 9 : level;
                damageMax += tmpLevel * 3;
                damageMax += Level2Value(level);
            }
            // 最小攻击
            if(damageMin > 0)
            {
                if(item.HaveSetOption() && itemLevel != 0 && confLevel != 0)
                {
                    if(setConf == null)
                    {
                        damageMin += (damageMin * 25) / confLevel + 5;
                        damageMin += itemLevel / 40 + 5;
                        if (item.HaveExcellentOption() && confLevel != 0)
                        {
                            // 有套装和卓越 额外加10点
                            damageMin += 10;
                        }
                    }
                }
                else if(item.HaveExcellentOption() && confLevel != 0)
                {
                    if (excConf == null)
                    {
                        damageMin += (damageMin * 25) / confLevel + 5;
                    }
                }

                int tmpLevel = (level > 9) ? 9 : level;
                damageMin += tmpLevel * 3;
                damageMin += Level2Value(level);
            }
            // 魔法伤害百分比
            if (magicPct > 0)
            {
                if(item.HaveSetOption() && itemLevel != 0 && confLevel != 0)
                {
                    if(setConf == null)
                    {
                        magicPct += (magicPct * 25) / confLevel + 5;
                        magicPct += itemLevel / 60 + 2;
                        if (item.HaveExcellentOption() && confLevel != 0)
                        {
                            // 有套装和卓越 额外加10点
                            magicPct += 10;
                        }
                    }
                }
                else if (item.HaveExcellentOption() && confLevel != 0)
                {
                    if(excConf == null)
                    {
                        magicPct += (magicPct * 25) / confLevel + 5;
                    }
                }
                int tmpLevel = (level > 9) ? 9 : level;
                magicPct += tmpLevel * 3;
                magicPct += Level2Value(level);
            }
            // 诅咒
            if(curse > 0)
            {
                if(item.HaveSetOption() && itemLevel != 0 && confLevel != 0)
                {
                    if(setConf == null)
                    {
                        curse += (curse * 25) / confLevel + 5;
                        curse += itemLevel / 60 + 2;
                        if (item.HaveExcellentOption() && confLevel != 0)
                        {
                            // 有套装和卓越 额外加10点
                            curse += 10;
                        }
                    }
                }
                else if (item.HaveExcellentOption() && confLevel != 0)
                {
                    if(excConf == null)
                    {
                        curse += (curse * 25) / confLevel + 5;
                    }
                }
                int tmpLevel = (level > 9) ? 9 : level;
                curse += tmpLevel * 3;
                curse += Level2Value(level);
            }
            // 宠物攻击提升
            if (upPet > 0)
            {
                if (item.HaveSetOption() && itemLevel != 0 && confLevel != 0)
                {
                    if (setConf == null)
                    {
                        upPet += (upPet * 25) / confLevel + 5;
                        upPet += itemLevel / 60 + 2;
                    }
                }
                else if (item.HaveExcellentOption() && confLevel != 0)
                {
                    if (excConf == null)
                    {
                        upPet += (upPet * 25) / confLevel + 5;
                    }
                }
                int tmpLevel = (level > 9) ? 9 : level;
                upPet += tmpLevel * 3;
                upPet += Level2Value(level);
            }
            // 防御率
            if (defRate > 0)
            {
                if (item.HaveSetOption() && itemLevel != 0 && confLevel != 0)
                {
                    if(setConf == null)
                    {
                        defRate += (defRate * 25) / confLevel + 5;
                        defRate += itemLevel / 40 + 2;
                    }
                }
                else if (item.HaveExcellentOption() && confLevel != 0)
                {
                    if(excConf == null)
                    {
                        defRate += (defRate * 25) / confLevel + 5;
                    }
                }
                int tmpLevel = (level > 9) ? 9 : level;
                defRate += tmpLevel * 3;
                defRate += Level2Value(level);
            }
            // 防御
            if (def > 0)
            {
                if (item.Type == EItemType.Shields)
                {
                    // 是盾牌
                    //def += level;
                    //if(item.HaveSetOption() && itemLevel > 0)
                    //{
                    // 套装加成
                    //    def += (def * 20) / itemLevel + 2;
                    //}
                }
                else
                {
                    if (item.HaveSetOption() && itemLevel != 0 && confLevel != 0)
                    {
                        // 套装
                        if(setConf == null)
                        {
                            def += (def * 12) / confLevel + (confLevel / 5) + 4;
                            def += (def * 3) / itemLevel + (itemLevel / 30) + 2;
                            if(item.HaveExcellentOption() && confLevel != 0)
                            {
                                // 有套装和卓越 额外加10点
                                def += 10;
                            }
                        }
                    }
                    else if (item.HaveExcellentOption() && confLevel != 0)
                    {
                        // 卓越
                        if (excConf == null)
                        {
                            def += (def * 12) / confLevel + (confLevel / 5) + 4;
                        }
                    }
                }

                int tmpLevel = (level > 9) ? 9 : level;
                def += tmpLevel * 3;
                def += Level2Value(level);
            }
            // 魔法防御
            if(magicDef > 0)
            {
                int tmpLevel = (level > 9) ? 9 : level;
                magicDef += tmpLevel * 3;
                magicDef += Level2Value(level);
            }

            item.SetProp(EItemValue.DamageMin, damageMin);
            item.SetProp(EItemValue.DamageMax, damageMax);
            item.SetProp(EItemValue.MagicDamage, magicPct);
            item.SetProp(EItemValue.Curse, curse);
            item.SetProp(EItemValue.UpPetAttackPct, upPet);
            item.SetProp(EItemValue.Defense, def);
            item.SetProp(EItemValue.DefenseRate, defRate);
            item.SetProp(EItemValue.MagicDefense, magicDef);
            item.SetProp(EItemValue.AttackSpeed, attackSpeed);
            item.SetProp(EItemValue.WalkSpeed, walkSpeed);
        }

        /// <summary>
        /// 更新 佩戴/使用 需求
        /// </summary>
        /// <param name="item">更新的物品</param>
        public virtual void UpdateRequire(Item item)
        {
            int level = item.GetProp(EItemValue.Level);
            int itemLevel = item.ConfigData.Level;
            int confLevel = item.ConfigData.Level;
            int reqStr = item.ConfigData.ReqStr;
            int reqAgi = item.ConfigData.ReqAgi;
            int reqEne = item.ConfigData.ReqEne;
            int reqVit = item.ConfigData.ReqVit;
            int reqCom = item.ConfigData.ReqCom;
            int reqLevel = item.ConfigData.ReqLvl;
            /*{ 屏蔽2024-3-13
            // 自定义配置
            ItemCustom_ExcAttrConfig excConf = null;
            ItemCustom_SetAttrConfig setConf = null;

            if (item.HaveSetOption())
            {
                // 套装
                itemLevel += 25;
                if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemCustom_SetAttrConfigJson>().JsonDic.TryGetValue(item.ConfigID, out setConf))
                {
                    reqStr = setConf.ReqStr;
                    reqAgi = setConf.ReqAgi;
                    reqEne = setConf.ReqEne;
                    reqVit = setConf.ReqVit;
                    reqCom = setConf.ReqCom;
                    reqLevel = setConf.ReqLvl;
                }
            }
            else if (item.HaveExcellentOption())
            {
                // 是卓越物品
                itemLevel += 25;
                if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemCustom_ExcAttrConfigJson>().JsonDic.TryGetValue(item.ConfigID, out excConf))
                {
                    reqStr = excConf.ReqStr;
                    reqAgi = excConf.ReqAgi;
                    reqEne = excConf.ReqEne;
                    reqVit = excConf.ReqVit;
                    reqCom = excConf.ReqCom;
                    reqLevel = excConf.ReqLvl;
                }
            }
            if (itemLevel <= 0) itemLevel = 1;
            if (confLevel <= 0) confLevel = 1;
            if (level > 0 || confLevel != itemLevel)
            {
                // TODO 先还原成原始的属性，再进行计算
                // 属性小于或等于20，已无法反推出原来的数值，停止计算
                // 力量
                if (reqStr > 20)
                {
                    reqStr = (int)Math.Ceiling((double)(((reqStr - 20) * 100) / 3f) / confLevel);
                    reqStr = ((reqStr * (itemLevel + level * 3)) * 3) / 100 + 20;
                }
                // 敏捷
                if (reqAgi > 20)
                {
                    reqAgi = (int)Math.Ceiling((double)(((reqAgi - 20) * 100) / 3f) / confLevel);
                    reqAgi = ((reqAgi * (itemLevel + level * 3)) * 3) / 100 + 20;
                }
                // 智力
                if (reqEne > 20)
                {
                    reqEne = (int)Math.Ceiling((double)(((reqEne - 20) * 100) / 3f) / confLevel);
                    reqEne = ((reqEne * (itemLevel + level * 3)) * 3) / 100 + 20;
                }
                // 体力
                if (reqVit > 20)
                {
                    reqVit = (int)Math.Ceiling((double)(((reqVit - 20) * 100) / 4f) / confLevel);
                    reqVit = ((reqVit * (itemLevel + level * 3)) * 4) / 100 + 20;
                }
                // 统帅
                if (reqCom > 20)
                {
                    reqCom = (int)Math.Ceiling((double)(((reqCom - 20) * 100) / 3f) / confLevel);
                    reqCom = ((reqCom * (itemLevel + level * 3)) * 3) / 100 + 20;
                }
                // 佩戴等级
                if (reqLevel > 0)
                {
                    reqLevel += level * 4;
                }
            }

            // 再生属性 减少佩戴所需力量、敏捷
            int orecycledID = item.GetProp(EItemValue.OrecycledID);
            int orecycledLevel = item.GetProp(EItemValue.OrecycledLevel);
            switch (orecycledID)
            {
                case 300002:
                case 300003:
                case 300101:
                case 300102:
                    {
                        ItemAttrEntryManager itemAttrEntryManager = Root.MainFactory.GetCustomComponent<ItemAttrEntryManager>();
                        ItemAttrEntry baseEntry = itemAttrEntryManager.GetOrCreate(orecycledID, orecycledLevel);
                        if (baseEntry == null)
                        {
                            Log.Error($"没找到物品属性词条。entryId={orecycledID}，level={orecycledLevel}");
                        }
                        else
                        {
                            switch (orecycledID)
                            {
                                case 300002:
                                case 300101:
                                    // 力量减少
                                    reqStr -= baseEntry.Value;
                                    if (reqStr < 0) reqStr = 0;
                                    break;
                                case 300003:
                                case 300102:
                                    // 敏捷减少
                                    reqAgi -= baseEntry.Value;
                                    if (reqAgi < 0) reqAgi = 0;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    }
                default:
                    break;
            }

            }*/

            item.SetProp(EItemValue.RequireStrength, reqStr);
            item.SetProp(EItemValue.RequireAgile, reqAgi);
            item.SetProp(EItemValue.RequireEnergy, reqEne);
            item.SetProp(EItemValue.RequireVitality, reqVit);
            item.SetProp(EItemValue.RequireCommand, reqCom);
            item.SetProp(EItemValue.RequireLevel, reqLevel);
        }

        /// <summary>
        /// 更新 购买价格
        /// </summary>
        /// <param name="item">更新的物品</param>
        public virtual void UpdateBuyMoney(Item item)
        {
            var conf = item.ConfigData;
            int level = item.GetProp(EItemValue.Level);
            int level2 = conf.Level + level * 3;
            int excellent = 0;
            long gold = 0;
            if(item.HaveExcellentOption())
            {
                excellent = 1;
            }


            if(excellent != 0)
            {
                level2 += 25;
            }

            switch (level)
            {
                case 5: level2 += 4; break;
                case 6: level2 += 10; break;
                case 7: level2 += 25; break;
                case 8: level2 += 45; break;
                case 9: level2 += 65; break;
                case 10: level2 += 95; break;
                case 11: level2 += 135; break;
                case 12: level2 += 185; break;
                case 13: level2 += 245; break;
                case 14: level2 += 305; break;
                case 15: level2 += 365; break;
                default: break;
            }
            gold = (level2 + 40) * level2 / 8 + 100;
            if (item.IsWing())
            {
                gold = (level2 + 40) * level2 * level2 * 11 + 40000000;
            }
            else
            {
                gold = (level2 + 40) * level2 * level2 / 8 + 100;
            }

            if (item.Type >= EItemType.Swords && item.Type <= EItemType.Shields)
            {
                // 武器和盾牌
                if (conf.TwoHand == 0)
                {
                    gold = gold * 80 / 100;
                }
            }

            // TODO 卓越属性、追加属性、翅膀属性 对价值进行增加
            // ...
            // 卓越测试代码
            if (item.HaveExcellentOption())
            {
                gold = (long)(gold * Math.Pow(1.2, item.data.ExcellentEntry.Count));
            }

            if (item.Have380Option())
            {
                gold += (long)(gold * 16f / 100f);
                //gold = gold * 2;
            }

            if(item.HaveEnableSocket())
            {
                // 镶嵌物品价值
                // ...
            }



            if(gold > 1200000000)
            {
                gold = 1200000000;
            }

            var itemPrice = Root.MainFactory.GetCustomComponent<ItemPriceComponent>().Get(item.ConfigID);
            if (itemPrice != null)
            {
                // 有自定义的价格
                if (itemPrice.UseFormula)
                {
                    itemPrice.Formula.SetLocalVar("强化等级", level);
                    itemPrice.Formula.SetLocalVar("配置等级", conf.Level);
                    itemPrice.Formula.SetLocalVar("耐久", item.GetProp(EItemValue.Durability));
                    itemPrice.Formula.SetLocalVar("最大耐久", item.GetProp(EItemValue.DurabilityMax));
                    itemPrice.Formula.SetLocalVar("数量", item.GetProp(EItemValue.Quantity));
                    itemPrice.Formula.SetLocalVar("单组数量", conf.StackSize);
                    itemPrice.Formula.SetLocalVar("默认价格", gold);
                    itemPrice.Formula.SetLocalVar("Value", itemPrice.Value[level]);
                    itemPrice.Formula.SetLocalVar("有卓越属性", excellent);
                    itemPrice.Formula.SetLocalVar("有套装属性", item.HaveSetOption() ? 1 : 0);
                    itemPrice.Formula.SetLocalVar("有镶嵌孔洞", item.HaveEnableSocket() ? 1 : 0);

                    gold = (long)itemPrice.Formula.GetValue();
                }
                else
                {
                    if (level >= 0 && level < itemPrice.Value.Count)
                    {
                        gold = itemPrice.Value[level];
                    }
                }

                if (gold > 1200000000)
                {
                    gold = 1200000000;
                }
            }

            item.SetProp(EItemValue.BuyMoney, (int)gold);
        }

        /// <summary>
        /// 更新 出售价格
        /// </summary>
        /// <param name="item">更新的物品</param>
        public virtual void UpdateSellMoney(Item item)
        {
            int dur = item.GetProp(EItemValue.Durability);
            int durMax = item.GetProp(EItemValue.DurabilityMax);
            int buyMoney = item.GetProp(EItemValue.BuyMoney);
            int sellMoney = buyMoney / 3;
            float pct = 1.0f;
            if (durMax > 0)
            {
                pct = 1.0f - (float)(dur / (float)durMax);
                pct = (pct > 1f) ? 1f : pct;
            }

            sellMoney -= (int)(sellMoney * 0.6f * pct);

            if (buyMoney > 1000)
            {
                buyMoney = buyMoney / 100 * 100;
            }
            else if(buyMoney > 100) 
            {
                buyMoney = buyMoney / 10 * 10;
            }

            if (sellMoney > 1000)
            {
                sellMoney = sellMoney / 100 * 100;
            }
            else if (sellMoney > 100)
            {
                sellMoney = sellMoney / 10 * 10;
            }
            //降低出售价格
            //原本56813的5681/2然后2840
            sellMoney = sellMoney / 10 / 2;
            if(sellMoney >= 3000)
                sellMoney /= 2;

            item.SetProp(EItemValue.BuyMoney, buyMoney);
            item.SetProp(EItemValue.SellMoney, sellMoney);

        }

        /// <summary>
        /// 更新 维修价格
        /// </summary>
        /// <param name="item">更新的物品</param>
        public virtual void UpdateRepairMoney(Item item)
        {
            int repairMoney = 0;
            if (item.HaveDurability() == true)
            {
                float durMax = item.GetProp(EItemValue.DurabilityMax);
                float dur = item.GetProp(EItemValue.Durability);
                int buyMoney = item.GetProp(EItemValue.BuyMoney);

                float lc4;
                float lc5 = 0;
                lc4 = durMax;

                if (dur == lc4)
                {
                    item.SetProp(EItemValue.RepairMoney, 0);
                    return;
                }

                float lc6 = 1.0f - dur / lc4;
                int lc7;

                lc7 = buyMoney / 3;
                

                if (lc7 > 400000000) lc7 = 400000000;

                if (lc7 >= 1000)
                {
                    lc7 = lc7 / 100 * 100;
                }
                else if (lc7 >= 100)
                {
                    lc7 = lc7 / 10 * 10;
                }
                float lc8 = (float)Math.Sqrt(lc7);
                float lc9 = (float)Math.Sqrt(Math.Sqrt(lc7));
                lc5 = 3.0f * lc8 * lc9;
                lc5 *= lc6;
                lc5 += 1.0f;

                if (dur <= 0.0f)
                {
                    // 物品耐久修正
                    /* 源代码
                    if (type == ITEMGET(13, 4) || type == ITEMGET(13, 5))
                    {
                        // 黑王马、天鹰
                        lc5 *= 2;
                    }
                    else
                    {
                        // 其他物品 lc5 *= 0.4f;
                        lc5 *= ::GetAllRepairItemRate(type);
                    }
                    */
                    lc5 *= 1.4f;
                }

                /* 源代码
                if (RequestPos == TRUE)
                {
                    // 在背包中修复，没在npc修复
                    lc5 += lc5 * 2; //season4 changed
                    
                    // 1.03 版本
                    lc5 += lc5
                }
                */
                repairMoney = (int)lc5;
                if(repairMoney < 1)
                {
                    repairMoney = 1;
                }
                //item.SetProp(EItemValue.RepairMoney, (int)lc5);
            }
            else
            {
                repairMoney = 0;
            }

            if(repairMoney > 1000)
            {
                repairMoney = repairMoney / 100 * 100;
            }
            else if (repairMoney > 100)
            {
                repairMoney = repairMoney / 10 * 10;
            }
            item.SetProp(EItemValue.RepairMoney, repairMoney);
        }

        /// <summary>
        /// 更新结束
        /// </summary>
        /// <param name="item">更新的物品</param>
        public virtual void EndUpdate(Item item)
        {

        }

    }
}
