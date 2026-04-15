
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
    /// 装备强化+9~+10
    /// </summary>
    [SynthesisRule(typeof(EquipmentStrengthenSynthesis10))]
    public class EquipmentStrengthenSynthesis10 : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            await EquipmentStrengthenSynthesis.Run(synthesis, itemList, config, b_Response, 9,mClientFinalR);
        }
    }
    public static class EquipmentStrengthenSynthesis
    {
        public static async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int needLevel, int mClientFinalR)
        {
            Item NeedItem1 = null;  //合成主要材料1
            List<Item> addRateItemList = new List<Item>();  //附加成功率的物品
            int curSuccessRate = config.BaseSuccessRate;
            int haveProtect = 0;

            //检查金币
            if (!synthesis.CheckGold((int)config.NeedGold))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                return;
            }
            //检查物品
            if (!synthesis.CheckItem(config.NeedItemsDic, itemList))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类或数量不正确";
                return;
            }
            bool haveOtherItem = false;
            int targetCount = 0;
            //检查是否有主要物品
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                int optLevel = itemList[i].GetProp(EItemValue.OptLevel);
                int level = itemList[i].GetProp(EItemValue.Level);
                //排除配置表中已判断的物品
                if (config.NeedItemsDic.ContainsKey(itemList[i].ConfigID) || itemList[i].ConfigID == (int)EItemStrengthen.PROTECT_RULE || itemList[i].ConfigID == (int)EItemStrengthen.LUKCY_RULE || itemList[i].ConfigID == (int)EItemStrengthen.LUKCY_RULE_10 || itemList[i].ConfigID == (int)EItemStrengthen.Protection_Charm)
                {
                    continue;
                }
                if (!itemList[i].CanStrengthen())
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                    b_Response.Message = "只能强化武器、防具和翅膀";
                    return;
                }
                if (level != needLevel)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                    b_Response.Message = "只能强化武器、防具和翅膀";
                    return;
                }
                targetCount++;
                NeedItem1 = itemList[i];
                itemList.RemoveAt(i);
                if (targetCount > 1)
                {
                    haveOtherItem = true;
                }
            }
            if (haveOtherItem)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类不正确";
                return;
            }
            if (NeedItem1 == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "装备要求等级或追加不足";
                return;
            }
            //增加成功率加成判定
            int ruleMaxCount = 10;
            int curRuleCount = 0;
            Item ProtectionItem = null;
            for (int i = 0; i < itemList.Count; i++)
            {
                int quantity = itemList[i].GetProp(EItemValue.Quantity);
                switch (itemList[i].ConfigID)
                {
                    case (int)EItemStrengthen.LUKCY_RULE:
                        if (curRuleCount >= ruleMaxCount)
                        {
                            break;
                        }
                        curSuccessRate += 1;    //符咒不可叠加，默认数量为1
                        addRateItemList.Add(itemList[i]);
                        curRuleCount++;
                        break;
                    case (int)EItemStrengthen.LUKCY_RULE_10:
                        if (curRuleCount >= ruleMaxCount)
                        {
                            break;
                        }
                        curSuccessRate += 10;    //符咒不可叠加，默认数量为1
                        addRateItemList.Add(itemList[i]);
                        curRuleCount+=10;
                        break;
                    case (int)EItemStrengthen.PROTECT_RULE:
                        haveProtect = 1;
                        ProtectionItem = itemList[i];
                        break;
                    case (int)EItemStrengthen.Protection_Charm:
                        ProtectionItem = itemList[i];
                        haveProtect = 2;
                        break;
                    default:
                        break;
                }
            }
            //镶嵌装备减成功率
            if (NeedItem1.IsSocketEquip())
            {
                curSuccessRate -= 20;
            }
            //幸运装备加成功率
            if (NeedItem1.IsLuckyEquip())
            {
                curSuccessRate += 5;
            }
            if (!synthesis.mItemDict.Remove(NeedItem1.ItemUID))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                return;
            }
            //=================判定逻辑结束，开始消耗逻辑(所有异常放到上面报，下面默认判断为合成逻辑成功)==================
            //消耗金币
            if (!synthesis.UseGold((int)config.NeedGold))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                return;
            }
            void DelItem(Item item, int Del = 0)
            {
                if (item != null)
                {
                    synthesis.UseItem(item.ItemUID, config.Info, item.GetProp(EItemValue.Quantity));
                    if (Del == 2)
                    {
                        //消耗完毕，返回所有材料至背包
                        synthesis.BackAllItemToBackpack(config.Info);
                        return;
                    }
                }
                //消耗物品(配置表所需物品)
                if (!synthesis.UseAllItem(config.NeedItemsDic, itemList, config.Info))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1719);
                    return;
                }
                for (int i = itemList.Count - 1; i >= 0; i--)
                {
                    if (itemList[i] == null)
                    {
                        itemList.RemoveAt(i);   //消耗物品以后清理所有空对象
                    }
                }
                for (int i = 0; i < addRateItemList.Count; i++)
                {
                    synthesis.UseItem(addRateItemList[i].ItemUID, config.Info, addRateItemList[i].GetProp(EItemValue.Quantity));
                }
                //消耗完毕，返回所有材料至背包
                //synthesis.BackAllItemToBackpack(config.Info);
            }
            //synthesis.UseItem(NeedItem1.ItemUID, config.Info, NeedItem1.GetProp(EItemValue.Quantity));
            if (mClientFinalR != curSuccessRate && config.MaxSuccessRate > curSuccessRate)
            {
                Log.Warning($"方法: EquipmentStrengthenSynthesis10 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //限制最大成功率
            if (curSuccessRate > config.MaxSuccessRate)
            {
                curSuccessRate = config.MaxSuccessRate;
            }
            //消耗完毕,清空缓存空间
            //synthesis.BackAllItemToBackpack(config.Info);
            //synthesis.mItemDict.Clear();
            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
            {
                DelItem(ProtectionItem, 0);

                ETModel.EventType.EquipmentRelatedSettings.Instance.player = synthesis.Parent;
                ETModel.EventType.EquipmentRelatedSettings.Instance.item = NeedItem1;
                ETModel.EventType.EquipmentRelatedSettings.Instance.TitleCount = 0;
                ETModel.EventType.EquipmentRelatedSettings.Instance.ItemCount = 0;
                Root.EventSystem.OnRun("EquipmentRelatedSettings", ETModel.EventType.EquipmentRelatedSettings.Instance);
                //强化成功
                NeedItem1.SetProp(EItemValue.Level, needLevel + 1);
                if (!synthesis.AddResultItem(NeedItem1))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                    return;
                }
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(NeedItem1));
                b_Response.Result = true;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(2, NeedItem1, true).Coroutine();
            }
            else
            {
                DelItem(ProtectionItem, haveProtect);
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(2, NeedItem1, false).Coroutine();
                if (haveProtect == 1)
                {
                    int OldLevel = NeedItem1.GetProp(EItemValue.Level);
                    if (9 <= OldLevel && OldLevel < 12)
                        NeedItem1.SetProp(EItemValue.Level, 9);
                    else if (12 <= OldLevel && OldLevel < 15)
                        NeedItem1.SetProp(EItemValue.Level, 12);

                    if (!synthesis.AddResultItem(NeedItem1))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                        b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                        return;
                    }
                    b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(NeedItem1));
                    b_Response.Result = false;
                    b_Response.Message = "合成失败，装备保护启动，保留装备但等级清零";

                }
                else if (haveProtect == 2)
                {
                    if (!synthesis.AddResultItem(NeedItem1))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                        b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                        return;
                    }
                    b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(NeedItem1));
                    b_Response.Result = false;
                    b_Response.Message = "合成失败，装备保护启动，保留装备但等级清零";
                }
                else
                    synthesis.UseItem(NeedItem1.ItemUID, config.Info, NeedItem1.GetProp(EItemValue.Quantity));

                b_Response.Result = false;
                
            }

            return;
        }
    }


}