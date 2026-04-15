using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
namespace ETHotfix
{
    public static class StrengthenItemSystem
    {
        /// <summary>强化+6~9成功几率</summary>
        public const int STRENGTHEN_SUCCESS_RATE = 50;
        /// <summary>追加最高等级</summary>
        public const int MAX_OPT_LEVEL = 4;
        /// <summary>追加成功几率</summary>
        public const int OPT_SUCCESS_RATE = 50;
        /// <summary>再生附加成功几率</summary>
        public const int REGEN_SUCCESS_RATE = 50;
        /// <summary>再生进化成功几率</summary>
        public const int REGEN_EVO_SUCCESS_RATE = 50;
        public const int REGEN_EVO_SUCCESS_RATE2 = 70;
        /// <summary>
        /// 宝石强化  +1~+9
        /// 传进来的Item不判断是否在玩家背包里，只判断物品是否能强化
        /// </summary>
        /// <param name="targetItem"></param>
        /// <param name="gemItem"></param>
        /// <returns></returns>
        public static int Strengthen(Item targetItem,Item gemItem,Player player, out string message)
        {
            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
            if (backpack == null )
            {
                message = "未找到角色背包";
                return ErrorCodeHotfix.ERR_EquipmentStrengthenFail;
            }

            //判断物品是否在背包
            if (!backpack.HaveItem(targetItem.ItemUID) || !backpack.HaveItem(gemItem.ItemUID))
            {
                message = "所选物品不在玩家背包中";
                return ErrorCodeHotfix.ERR_EquipmentStrengthenFail;
            }

            //进入+1~+9强化逻辑
            if (gemItem.ConfigID == (int)EItemStrengthen.SOUL_GEMS || gemItem.ConfigID == (int)EItemStrengthen.BLESSING_GEMS)
            {
                //判断是否符合强化条件
                if (!targetItem.CanStrengthen())
                {
                    message = "此物品不能强化";
                    return 3000;
                }
                return EquipLevelStrengthen(targetItem, gemItem, player, out message);
            }
            //进入追加逻辑
            else if (gemItem.ConfigID == (int)EItemStrengthen.ANIMA_GEMS)
            {
                return EquipmentAddition(targetItem, gemItem, player, out message);
            }
            //进入添加再生属性逻辑
            else if (gemItem.ConfigID == (int)EItemStrengthen.RECYCLED_GEMS)
            {
                return EquipmentOrecycled(targetItem, gemItem, player, out message);
            }
            //进入再生属性进化逻辑
            else if (gemItem.ConfigID == (int)EItemStrengthen.ELEMENTARY_EVOLUTION_GEMS)
            {
                return EquipmentOrecycledEvolution(targetItem, gemItem, player, REGEN_EVO_SUCCESS_RATE, out message);
            }
            else if (gemItem.ConfigID == (int)EItemStrengthen.ADVANCED_EVOLUTION_GEMS)
            {
                return EquipmentOrecycledEvolution(targetItem, gemItem, player, REGEN_EVO_SUCCESS_RATE2, out message);
            }
            message = "强化失败，未找到强化方法";
            return 3009;
        }

        /// <summary>
        /// 强化结果，返回true即为成功
        /// </summary>
        /// <param name="successRate">成功率 value:0-100</param>
        /// <returns></returns>
        public static bool StrengthenResult(int successRate)
        {
            int curNum = Help_RandomHelper.Range(0, 100);
            return curNum < successRate;
        }

        /// <summary>
        /// 用于多概率的计算
        /// </summary>
        /// <param name="rateList">例：[50,30,50,10] 会根据概率大小返回下标</param>
        /// <returns>进来的rateList的下标</returns>
        public static int StrengthenResult(List<int> rateList)
        {
            int totalRate = 0;
            for (int i = 0; i < rateList.Count; i++)
            {
                totalRate += rateList[i];
            }
            int curNum = Help_RandomHelper.Range(0, totalRate);
            for (int i = 0; i < rateList.Count; i++)
            {
                curNum -= rateList[i];
                if (curNum < 0)
                {
                    return i;
                }
            }
            return -1;
        }

        #region +1~+9宝石强化
        private static int EquipLevelStrengthen(Item targetItem, Item gemItem, Player player, out string message)
        {
            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
            int itemLevel = targetItem.GetProp(EItemValue.Level);
            if (itemLevel < 0)
            {
                itemLevel = 0;
            }
            if (itemLevel >= 9)
            {
                message = "只能强化9级以下物品";
                return 3001;
            }
            if(gemItem.ConfigID == (int)EItemStrengthen.BLESSING_GEMS)
            {
                //0~6 祝福宝石强化
                if (itemLevel >= 6)
                {
                    message = "物品不正确，需要灵魂宝石强化";
                    return 3002;
                }
                ETModel.EventType.EquipmentRelatedSettings.Instance.player = player;
                ETModel.EventType.EquipmentRelatedSettings.Instance.item = targetItem;
                ETModel.EventType.EquipmentRelatedSettings.Instance.TitleCount = 0;
                ETModel.EventType.EquipmentRelatedSettings.Instance.ItemCount = 0;
                Root.EventSystem.OnRun("EquipmentRelatedSettings", ETModel.EventType.EquipmentRelatedSettings.Instance);
                //100%成功，不会失败
                targetItem.SetProp(EItemValue.Level, itemLevel + 1);
                targetItem.UpdateProp();
                targetItem.SendAllPropertyData(player);
                targetItem.OnlySaveDB();
                Log.PLog("Item", $"a:{player.UserId} r:{player.GameUserId} 使用祝福宝石强化 targetItem=({targetItem.ToLogString()}),gemItem=({gemItem.ToLogString()})");
            }
            else if(gemItem.ConfigID == (int)EItemStrengthen.SOUL_GEMS)
            {
                // 0~9 灵魂宝石强化

                //50%成功(TODO:幸运装备75%) 失败则降级
                if (StrengthenResult(STRENGTHEN_SUCCESS_RATE + (targetItem.IsLuckyEquip() ? 25 : 0)))
                {
                    ETModel.EventType.EquipmentRelatedSettings.Instance.player = player;
                    ETModel.EventType.EquipmentRelatedSettings.Instance.item = targetItem;
                    ETModel.EventType.EquipmentRelatedSettings.Instance.TitleCount = 0;
                    ETModel.EventType.EquipmentRelatedSettings.Instance.ItemCount = 0;
                    Root.EventSystem.OnRun("EquipmentRelatedSettings", ETModel.EventType.EquipmentRelatedSettings.Instance);
                    targetItem.SetProp(EItemValue.Level, itemLevel + 1);
                    targetItem.UpdateProp();
                    targetItem.SendAllPropertyData(player);
                    targetItem.OnlySaveDB();
                    Log.PLog("Item", $"a:{player.UserId} r:{player.GameUserId} 使用灵魂宝石强化(成功) targetItem=({targetItem.ToLogString()}),gemItem=({gemItem.ToLogString()})");
                }
                else
                {
                    // +8、+9 失败，强化等级清零
                    //if(itemLevel >= 7)
                    //{
                    //    targetItem.SetProp(EItemValue.Level, 0);
                    //}
                    //else
                    {
                        if (itemLevel > 0)
                        {
                            targetItem.SetProp(EItemValue.Level, itemLevel - 1);
                        }
                    }
                    targetItem.UpdateProp();
                    targetItem.SendAllPropertyData(player);
                    targetItem.OnlySaveDB();
                    Log.PLog("Item", $"a:{player.UserId} r:{player.GameUserId} 使用灵魂宝石强化(失败) targetItem=({targetItem.ToLogString()}),gemItem=({gemItem.ToLogString()})");
                }
            }
            //消耗掉宝石
            backpack.UseItem(gemItem.ItemUID, $"强化装备 targetItem.UID={targetItem.ItemUID}");

            message = "success";
            return ErrorCodeHotfix.ERR_Success;
        }
        #endregion

        #region 追加 生命宝石
        private static int EquipmentAddition(Item targetItem, Item gemItem, Player player, out string message)
        {
            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
            int optLevel = 0;
            var targetConfig = targetItem.ConfigData;
            if (targetItem.data.PropertyData.TryGetValue((int)EItemValue.OptLevel, out optLevel))
            {
                if (optLevel >= MAX_OPT_LEVEL)
                {
                    message = "追加等级已达到最高";
                    return 3003;
                }
            }
            //开始追加
            if (StrengthenResult(OPT_SUCCESS_RATE))
            {
                int equipPosition = targetConfig.Slot;
                //给没有追加属性的装备赋值
                if (!targetItem.data.PropertyData.ContainsKey((int)EItemValue.OptValue))
                {
                    if (equipPosition == (int)EquipPosition.Weapon) //武器
                    {
                        if (targetItem.Type == EItemType.Staffs)
                        {
                            targetItem.SetProp(EItemValue.OptValue, (int)EItemOpt.MAGIC_ATTACK, player);
                        }
                        else
                        {
                            targetItem.SetProp(EItemValue.OptValue, (int)EItemOpt.ATTACK, player);
                        }
                    }
                    else if (equipPosition == (int)EquipPosition.Shield)  //盾牌
                    {
                        targetItem.SetProp(EItemValue.OptValue, (int)EItemOpt.DEFENCE_RATE, player);
                    }
                    else if (equipPosition == (int)EquipPosition.Wing)  //翅膀
                    {
                        //TODO;翅膀一代二代三代貌似有不同
                    }
                    else 
                    {
                        targetItem.SetProp(EItemValue.OptValue, (int)EItemOpt.DEFENCE, player);
                    }
                }
                targetItem.SetProp(EItemValue.OptLevel, optLevel + 1, player);
            }
            else {
                targetItem.SetProp(EItemValue.OptLevel, optLevel <= 0 ? 0 : optLevel - 1, player);
            }
            //消耗掉宝石
            backpack.UseItem(gemItem.ItemUID,$"装备追加属性 targetItem.UID={targetItem.ItemUID}");
            //backpack.DeleteItem(gemItem).data.IsDispose = 1;
            //gemItem.SaveDB(player);
            //gemItem.Dispose();

            message = "success";
            return ErrorCodeHotfix.ERR_Success;
        }
        #endregion

        #region 添加再生属性
        private static int EquipmentOrecycled(Item targetItem, Item gemItem, Player player, out string message)
        {
            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
            var targetConfig = targetItem.ConfigData;
            if (targetItem.data.PropertyData.TryGetValue((int)EItemValue.OrecycledID, out int orecycledID))
            {
                if (orecycledID > 0)
                {
                    message = "装备已有再生属性，请还原再生属性后再尝试";
                    return 3004;
                }
            }

            EItemRegen curRegenEnum = EItemRegen.None;
            //添加再生属性
            if (targetItem.Type == EItemType.Staffs) //魔杖
            {
                curRegenEnum = EItemRegen.Staffs;
            }
            else if (targetConfig.Slot == (int)EquipPosition.Weapon) //武器
            {
                curRegenEnum = EItemRegen.Weapon;
            }
            else //其余的是防具,包括翅膀
            {
                curRegenEnum = EItemRegen.Body;
            }

            var regenConfigList = GetRegenList(curRegenEnum, targetItem.GetProp(EItemValue.Level));
            if (regenConfigList.Count == 0)
            {
                message = "装备不满足再生条件，请强化后再尝试";
                return 3005;
            }

            //随机添加再生属性
            if (StrengthenResult(REGEN_SUCCESS_RATE))
            {
                List<int> rateList = new List<int>();
                for (int i = 0; i < regenConfigList.Count; i++)
                {
                    rateList.Add(regenConfigList[i].Rate);
                }
                int curIndex = StrengthenResult(rateList);
                if (curIndex >= 0)
                {
                    targetItem.SetProp(EItemValue.OrecycledID, regenConfigList[curIndex].Id);
                    targetItem.SetProp(EItemValue.OrecycledLevel, regenConfigList[curIndex].ReqLevel);
                    targetItem.SendAllPropertyData(player);
                    targetItem.SaveDB(player);
                }
                else
                {
                    message = "返回再生属性结果失败";
                    return 3006;
                }
            }
            //消耗掉宝石
            //backpack.DeleteItem(gemItem).data.IsDispose = 1;
            //gemItem.SaveDB(player);
            //gemItem.Dispose();
            backpack.UseItem(gemItem.ItemUID,$"添加再生属性 targetItem.UID={targetItem.ItemUID}");

            message = "success";
            return ErrorCodeHotfix.ERR_Success;
        }

        private static List<ItemAttrEntry_RegenConfig> GetRegenList(EItemRegen regenEnum, int regenLevel)
        {
            List<ItemAttrEntry_RegenConfig> result = new List<ItemAttrEntry_RegenConfig>();
            ReadConfigComponent mReadConfigComponent = CustomFrameWork.Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var RegenConfig = mReadConfigComponent.GetJson<ItemAttrEntry_RegenConfigJson>().JsonDic;
            foreach (var item in RegenConfig)
            {
                if (item.Value.EntryType == (int)regenEnum && item.Value.ReqLevel <= regenLevel)
                {
                    result.Add(item.Value);
                }
            }
            return result;
        }

        private static ItemAttrEntry_RegenConfig GetRegen(int regenID)
        {
            ReadConfigComponent mReadConfigComponent = CustomFrameWork.Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var RegenConfig = mReadConfigComponent.GetJson<ItemAttrEntry_RegenConfigJson>().JsonDic;
            if (RegenConfig.TryGetValue(regenID,out var result))
            {
                return result;
            }
            return null ;
        }
        #endregion

        #region 再生属性进化
        private static int EquipmentOrecycledEvolution(Item targetItem, Item gemItem, Player player,int successRate, out string message)
        {
            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
            int orecycledID = targetItem.GetProp(EItemValue.OrecycledID);
            if (orecycledID > 0)
            {
                int orecycledLevel = targetItem.GetProp(EItemValue.OrecycledLevel);
                var curRegenConfig = GetRegen(orecycledID);
                if (orecycledLevel >= curRegenConfig.MaxLevel)
                {
                    message = "已达到最大等级，无法进化";
                    return 3007;
                }
                if (orecycledLevel >= targetItem.GetProp(EItemValue.Level))
                {
                    message = "进化等级不能超过装备强化等级";
                    return 3007;
                }
                if (StrengthenResult(successRate))
                {
                    orecycledLevel++;
                    targetItem.SetProp(EItemValue.OrecycledLevel, orecycledLevel, player);
                }
                else
                {
                    orecycledLevel = orecycledLevel <= curRegenConfig.ReqLevel ? curRegenConfig.ReqLevel : orecycledLevel - 1;
                    targetItem.SetProp(EItemValue.OrecycledLevel, orecycledLevel, player);
                }

            }
            else
            {
                message = "装备没有再生属性";
                return 3008;
            }


            //消耗掉宝石
            //backpack.DeleteItem(gemItem).data.IsDispose = 1;
            //gemItem.SaveDB(player);
            //gemItem.Dispose();
            backpack.UseItem(gemItem.ItemUID,$"再生属性进化 targetItem.UID={targetItem.ItemUID}");

            message = "success";
            return ErrorCodeHotfix.ERR_Success;
        }
        #endregion

        #region 再生属性还原
        public static int EquipmentOrecycledRestore(Item targetItem, Player player, out string message)
        {
            //TODO:等待策划配好消耗品
            if (true)
            {
                targetItem.SetProp(EItemValue.OrecycledID, 0);
                targetItem.SetProp(EItemValue.OrecycledLevel, 0);
                targetItem.SendAllPropertyData(player);
                targetItem.SaveDB(player);
            }
            else {
                message = "金币不足，还原再生属性失败";
                return ErrorCodeHotfix.ERR_EquipmentOrecycledRestoreFail;
            }
            message = "success";
            return ErrorCodeHotfix.ERR_Success;
        }
        #endregion
    }
}
