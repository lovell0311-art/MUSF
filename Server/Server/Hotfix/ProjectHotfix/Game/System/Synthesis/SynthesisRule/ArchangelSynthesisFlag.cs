
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
    /// 坐骑护卫-龙魂(冰晶)
    /// </summary>
    [SynthesisRule(typeof(ArchangelSynthesisFlag))]
    public class ArchangelSynthesisFlag : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis, int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            int curSuccessRate = config.BaseSuccessRate;
            int curSynItemConfigID = 340004;
            int SetId = 0;
            List<Item> addRateItemList = new List<Item>();  //附加成功率的物品
            Item Protection = null;
            Dictionary<int, int> SetList = new Dictionary<int, int>()
            {
                { 320462,27},
                { 320463,1015},
                { 320464,2024},
                { 320465,4010},
                { 320466,5010},
                { 320467,6016},
                { 320468,7011}
            };
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
            int ruleMaxCount = 10;
            int curRuleCount = 0;
            foreach (var item in itemList)
            {
                if (SetList.TryGetValue(item.ConfigID, out var value))
                {
                    SetId = value;
                    addRateItemList.Add(item);
                }

                int quantity = item.GetProp(EItemValue.Quantity);
                switch (item.ConfigID)
                {
                    case (int)EItemStrengthen.LUKCY_RULE:
                        if (curRuleCount >= ruleMaxCount)
                        {
                            break;
                        }
                        curSuccessRate += 1;    //符咒不可叠加，默认数量为1
                        addRateItemList.Add(item);
                        curRuleCount++;
                        break;
                    case (int)EItemStrengthen.LUKCY_RULE_10:
                        if (curRuleCount >= ruleMaxCount)
                        {
                            break;
                        }
                        curSuccessRate += 10;    //符咒不可叠加，默认数量为1
                        addRateItemList.Add(item);
                        curRuleCount += 10;
                        break;
                    case (int)EItemStrengthen.ANIMA_GEMS:
                        curSuccessRate += 6 * quantity;
                        addRateItemList.Add(item);
                        break;
                    case (int)EItemStrengthen.MID_MAGIC_STONE:
                        curSuccessRate += 5 * quantity;
                        addRateItemList.Add(item);
                        break;
                    case (int)EItemStrengthen.Protection_Charm:
                        Protection = item;
                        break;
                    default:
                        break;
                }
            }
            //=================判定逻辑结束，开始消耗逻辑(所有异常放到上面报，下面默认判断为合成逻辑成功)==================
            //消耗金币
            void DelItem(bool Del = true)
            {
                if (Del)
                {
                    if (!synthesis.UseGold((int)config.NeedGold))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                        return;
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
                }
                //消耗完毕，返回所有材料至背包
                synthesis.BackAllItemToBackpack(config.Info);
            }
            if (Protection != null)
                synthesis.UseItem(Protection.ItemUID, config.Info, Protection.GetProp(EItemValue.Quantity));
            if (mClientFinalR != curSuccessRate && config.MaxSuccessRate > curSuccessRate)
            {
                Log.Warning($"方法: FlamingHorseSynthesis 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //限制最大成功率
            if (curSuccessRate > config.MaxSuccessRate)
            {
                curSuccessRate = config.MaxSuccessRate;
            }
            //消耗完毕，返回所有材料至背包
            //synthesis.BackAllItemToBackpack(config.Info);
            //ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
            //itemCreateAttr.Quantity = 1;
            //itemCreateAttr.CustomAttrMethod.Add("ItemRandAddExcAttr_3");
            //生成合成物品
            Item resultItem = ItemFactory.Create(curSynItemConfigID, synthesis.mPlayer.GameAreaId/*, itemCreateAttr*/);
            if (SetId != 0)
                resultItem.SetProp(EItemValue.SetId, SetId);
            resultItem.SetProp(EItemValue.LuckyEquip, 1);
            if (resultItem.CanHaveExcellentOption())
            {
                var excAttrEntryManager = Root.MainFactory.GetCustomComponent<ExcAttrEntryManagerComponent>();
                if (excAttrEntryManager.TryGetSelectorByItem(resultItem, out var selector))
                {
                    var newSelector = new RandomSelector<int>(selector);
                    excAttrEntryManager.FlagExcAttrEntryCount.TryGetValue(out int count1);
                    if (count1 < 3) count1 = 3;

                    while (count1 > 0)
                    {
                        if (newSelector.TryGetValueAndRemove(out var entryId))
                        {
                            if (resultItem.data.ExcellentEntry.Add(entryId))
                            {
                                --count1;
                            }
                        }
                        else
                        {
                            // 词条取空了
                            break;
                        }
                    }
                }
            }

            if (resultItem.CanHaveEnableSocket())
            {
                ItemSocketManager itemSocketManager = Root.MainFactory.GetCustomComponent<ItemSocketManager>();
                if (itemSocketManager.DropHoleCountSelector.TryGetValue(out int count))
                {
                    count = 5;
                    resultItem.SetProp(EItemValue.FluoreSlotCount, count);
                    resultItem.SetProp(EItemValue.FluoreSlot1, 0);
                    resultItem.SetProp(EItemValue.FluoreSlot2, 0);
                    resultItem.SetProp(EItemValue.FluoreSlot3, 0);
                    resultItem.SetProp(EItemValue.FluoreSlot4, 0);
                    resultItem.SetProp(EItemValue.FluoreSlot5, 0);
                }
            }

            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
            {
                DelItem();

                if (!synthesis.AddResultItem(resultItem))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                    return;
                }
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(resultItem));
                b_Response.Result = true;
                synthesis.Parent.GetCustomComponent<GamePlayer>().SendItem(3, resultItem, true).Coroutine();
            }
            else
            {
                if (Protection == null)
                    DelItem(true);
                else
                    DelItem(false);
                b_Response.Result = false;
                synthesis.Parent.GetCustomComponent<GamePlayer>().SendItem(3, resultItem, false).Coroutine();
            }

            return;
        }
    }
}