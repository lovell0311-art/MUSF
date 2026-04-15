
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
    /// 坐骑护卫-折断的角合成
    /// </summary>
    [SynthesisRule(typeof(YanLangShouJiaoSynthesis))]
    public class YanLangShouJiaoSynthesis : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        

        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response,int mClientFinalR)
        {
            List<Item> addRateItemList = new List<Item>();  //附加成功率的物品
            int curSuccessRate = config.BaseSuccessRate;
            //检查物品
            if (!synthesis.CheckItem(config.NeedItemsDic,itemList))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类或数量不正确";
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
                        curRuleCount += 10;
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
                        //消耗完毕，返回所有材料至背包
                        synthesis.BackAllItemToBackpack(config.Info);
                        return;
                    }
                }
                //消耗物品
                if (!synthesis.UseAllItem(config.NeedItemsDic, itemList, config.Info))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1719);
                    return;
                }
                for (int i = 0; i < addRateItemList.Count; i++)
                {
                    synthesis.UseItem(addRateItemList[i].ItemUID, config.Info, addRateItemList[i].GetProp(EItemValue.Quantity));
                }
                //消耗完毕，返回所有材料至背包
                synthesis.BackAllItemToBackpack(config.Info);
            }
            if (mClientFinalR != curSuccessRate && config.MaxSuccessRate > curSuccessRate)
            {
                Log.Warning($"方法: YanLangShouJiaoSynthesis 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //限制最大成功率
            if (curSuccessRate > config.MaxSuccessRate)
            {
                curSuccessRate = config.MaxSuccessRate;
            }
            //synthesis.BackAllItemToBackpack(config.Info);
            //生成合成物品
            Item resultItem = ItemFactory.Create(260008, synthesis.mPlayer.GameAreaId);
            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
            {
                DelItem(ProtectionItem);
                if (!synthesis.AddResultItem(resultItem))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";
                    return;
                }
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(resultItem));
                b_Response.Result = true;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, true).Coroutine();
            }
            else {
                DelItem(ProtectionItem,false);
                b_Response.Result = false;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, false).Coroutine();
            }

            return;
        }
    }
}