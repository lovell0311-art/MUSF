
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
    /// 三代翅膀合成
    /// </summary>
    [SynthesisRule(typeof(ThirdWingSynthesis))]
    public class ThirdWingSynthesis : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis, int>
    {
        /// <summary>
        /// 出现幸运属性概率
        /// </summary>
        private const int LuckyAttrRate = 50;
        /// <summary>
        /// 出现特殊词条概率
        /// </summary>
        private const int SpecialEntryRate = 50;
        /// <summary>
        /// 出现特殊词条 条数
        /// </summary>
        private const int SpecialEntryCountMin = 1;
        private const int SpecialEntryCountMax = 1;
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            List<int> ThirdWing = new List<int>()
            {
                220027,
                220029,
                220030,
                220031,
                220032,
                220033,
                220034,
                220035,
                //220016,
                //220018
            };

            Dictionary<int, int> GenerationWingDict = new Dictionary<int, int>
            {
                [320422] = 220035,
                [320423] = 220033,
                [320424] = 220034,
                [320425] = 220032,
                //[320426] = 220018,
                [320427] = 220030,
                //[320428] = 220016,
                [320429] = 220031,
                [320430] = 220029,
                [320431] = 220027,
            };
            List<Item> addRateItemList = new List<Item>();  //附加成功率的物品
            int curSuccessRate = config.BaseSuccessRate;
            int curSynItemConfigID = ThirdWing[RandomHelper.RandomNumber(0, ThirdWing.Count)];
            Item RuneItem = null;   //合成符咒
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

            bool mHasNeedMaterial = false;
            foreach (var item in itemList)
            {
                if (item.ConfigID == 320020)
                {
                    mHasNeedMaterial = true;
                    break;
                }
                if (item.ConfigData.WingLevel == 25)
                {
                    //if (item.GetProp(EItemValue.Level) >= 9
                    // || item.GetProp(EItemValue.OptLevel) >= 1)
                    mHasNeedMaterial = true;
                    break;
                }
            }
            if (mHasNeedMaterial == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类或数量不正确";
                return;
            }

            //判断生成物品ConfigID
            bool isBreak = false;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (GenerationWingDict.TryGetValue(itemList[i].ConfigID, out int value))
                {
                    if (isBreak)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                        b_Response.Message = "决定合成物品的主要材料重复";
                        return;
                    }
                    curSynItemConfigID = value;
                    RuneItem = itemList[i];
                    isBreak = true;
                }
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
                    case (int)EItemStrengthen.HIGH_MAGIC_STONE:
                        curSuccessRate += quantity * 5;
                        addRateItemList.Add(itemList[i]);
                        break;
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
            void DelItem(Item item1, bool Del = true)
            {
                if (item1 != null)
                {
                    synthesis.UseItem(item1.ItemUID, config.Info, item1.GetProp(EItemValue.Quantity));
                    if (!Del)
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
                foreach (var item in itemList)
                {
                    if (item.IsDisposeable == true) continue;
                    if (item.ConfigData == null) continue;
                    if (item.ItemUID == 0) continue;
                    if (item.ConfigID == 320020)
                    {
                        if (synthesis.UseItem(item.ItemUID, config.Info, 1) == false)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1719);
                            return;
                        }
                        break;
                    }
                    if (item.ConfigData.WingLevel == 25)
                    {
                        if (synthesis.UseItem(item.ItemUID, config.Info, 1) == false)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1719);
                            return;
                        }
                        break;
                    }
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
                if (RuneItem != null)
                {
                    synthesis.UseItem(RuneItem.ItemUID, config.Info);
                }
                //消耗完毕，返回所有材料至背包
                synthesis.BackAllItemToBackpack(config.Info);
            }
            if (mClientFinalR != curSuccessRate && config.MaxSuccessRate > curSuccessRate)
            {
                Log.Warning($"方法: ThirdWingSynthesis 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
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
                GenerationWingSynthesis.InitWingsProp_Rand(resultItem, LuckyAttrRate, SpecialEntryRate, SpecialEntryCountMin, SpecialEntryCountMax);

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
            else
            {
                DelItem(ProtectionItem,false);
                b_Response.Result = false;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, false).Coroutine();
            }

            return;
        }
    }
}