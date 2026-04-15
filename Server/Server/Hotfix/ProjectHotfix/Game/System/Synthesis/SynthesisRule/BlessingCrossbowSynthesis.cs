
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
    /// 其他装备-祝福大天使之弩合成
    /// </summary>
    [SynthesisRule(typeof(BlessingCrossbowSynthesis))]
    public class BlessingCrossbowSynthesis : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            Item NeedItem1 = null;  //合成主要材料1
            List<Item> addRateItemList = new List<Item>();  //附加成功率的物品
            int curSuccessRate = config.BaseSuccessRate;
            int curSynItemConfigID = 50010;
            int NeedItemID = 50008; //所需的装备物品ID

            //检查金币
            if (!synthesis.CheckGold((int)config.NeedGold))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                return;
            }
            //检查物品
            if (!synthesis.CheckItem(config.NeedItemsDic,itemList))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类或数量不正确";
                return;
            }
            //检查是否有主要物品
            for (int i = 0; i < itemList.Count; i++)
            {
                int optLevel = itemList[i].GetProp(EItemValue.OptLevel);
                int level = itemList[i].GetProp(EItemValue.Level);
                if (itemList[i].ConfigID == NeedItemID && optLevel >= 4 && level >= 15) 
                {
                    NeedItem1 = itemList[i];
                    itemList.RemoveAt(i);
                    break;
                }
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
                    case (int)EItemStrengthen.ANIMA_GEMS:
                        curSuccessRate += 10 * quantity;
                        addRateItemList.Add(itemList[i]);
                        break;
                    case (int)EItemStrengthen.Protection_Charm:
                        ProtectionItem = itemList[i];
                        break;
                    default:
                        break;
                }
            }

            //=================判定逻辑结束，开始消耗逻辑(所有异常放到上面报，下面默认判断为合成逻辑成功)==================
            //消耗金币
            if (!synthesis.UseGold((int)config.NeedGold))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                return;
            }
            void DelItem(Item item, bool Del = true)
            {
                if (item != null)
                {
                    synthesis.UseItem(item.ItemUID, config.Info, item.GetProp(EItemValue.Quantity));
                    if (!Del)
                    {
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
                synthesis.UseItem(NeedItem1.ItemUID, config.Info, NeedItem1.GetProp(EItemValue.Quantity));
                //消耗完毕，返回所有材料至背包
                synthesis.BackAllItemToBackpack(config.Info);
            }
            if (mClientFinalR != curSuccessRate && config.MaxSuccessRate > curSuccessRate)
            {
                Log.Warning($"方法: BlessingCrossbowSynthesis 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //限制最大成功率
            if (curSuccessRate > config.MaxSuccessRate)
            {
                curSuccessRate = config.MaxSuccessRate;
            }
            //消耗完毕，返回所有材料至背包
            //synthesis.BackAllItemToBackpack(config.Info);
            //生成合成物品
            Item resultItem = ItemFactory.Create(curSynItemConfigID, synthesis.mPlayer.GameAreaId);
            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
            {
                DelItem(ProtectionItem);

                if (!synthesis.AddResultItem(resultItem))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                    return;
                }
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(resultItem));
                b_Response.Result = true;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, true).Coroutine();
            }
            else {
                DelItem(ProtectionItem, false);
                b_Response.Result = false;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, false).Coroutine();
            }

            return;
        }
    }
}