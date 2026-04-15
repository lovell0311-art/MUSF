
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
    /// 一代翅膀合成
    /// </summary>
    [SynthesisRule(typeof(GenerationWingSynthesis))]
    public class GenerationWingSynthesis : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
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
            List<int> GenerationWing = new List<int>()
            {
                220058,
                220059,
                220060,
                220057
            };

            Dictionary<int, int> GenerationWingDict = new Dictionary<int, int>
            {
                [320037] = 220058,
                [320038] = 220059,
                [320039] = 220060,
                [320040] = 220057
            };

            List<int> NeedItem = new List<int>() { 30005, 40006, 80007 };

            Item NeedItem1 = null;  //合成主要材料1
            Item RuneItem = null;   //合成符咒
            List<Item> addRateItemList = new List<Item>();  //附加成功率的物品
            int curSuccessRate = config.BaseSuccessRate;
            int curSynItemConfigID = GenerationWing[RandomHelper.RandomNumber(0, GenerationWing.Count)];

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
            //检查是否有玛雅物品
            for (int i = 0; i < itemList.Count; i++)
            {
                int optLevel = itemList[i].GetProp(EItemValue.OptLevel);
                int level = itemList[i].GetProp(EItemValue.Level);
                if (NeedItem.Contains(itemList[i].ConfigID) && itemList[i].GetProp(EItemValue.OptLevel) >= 1 && itemList[i].GetProp(EItemValue.Level) >= 4)
                {
                    NeedItem1 = itemList[i];
                    itemList.RemoveAt(i);
                    curSuccessRate += optLevel * 3;
                    curSuccessRate += level * 2;
                    break;
                }
            }
            if (NeedItem1 == null)
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
            Item ProtectionItem = null;
            //增加成功率加成判定
            for (int i = 0; i < itemList.Count; i++)
            {
                int quantity = itemList[i].GetProp(EItemValue.Quantity);
                switch (itemList[i].ConfigID)
                {
                    case (int)EItemStrengthen.BLESSING_GEMS:
                        curSuccessRate += quantity * 5;
                        addRateItemList.Add(itemList[i]);
                        break;
                    case (int)EItemStrengthen.SOUL_GEMS:
                        curSuccessRate += quantity * 3;
                        addRateItemList.Add(itemList[i]);
                        break;
                    case (int)EItemStrengthen.MAYA_GEMS:
                        curSuccessRate += quantity * 2;
                        addRateItemList.Add(itemList[i]);
                        break;
                    case (int)EItemStrengthen.LOW_MAGIC_STONE:
                        curSuccessRate += quantity * 5;
                        addRateItemList.Add(itemList[i]);
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
            void DelItem(Item item,bool Del = true)
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
                if (RuneItem != null)
                {
                    synthesis.UseItem(RuneItem.ItemUID, config.Info);
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
                Log.Warning($"方法: GenerationWingSynthesis 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
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
                InitWingsProp_Rand(resultItem, LuckyAttrRate, SpecialEntryRate, SpecialEntryCountMin, SpecialEntryCountMax);

                if (!synthesis.AddResultItem(resultItem))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                    return;
                }
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(resultItem));
                b_Response.Result = true;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, true).Coroutine();
                //生成翅膀额外属性

            }
            else
            {
                DelItem(ProtectionItem,false);

                b_Response.Result = false;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, false).Coroutine();
            }

            return;
        }

        public static void InitWingsProp_Rand(Item item, int luckyRate, int specialEntryRate, int entryCountMin, int entryCountMax)
        {
            if (!item.IsWing())
            {
                Log.Error($"无法为非翅膀物品添加特殊属性，item.ConfigID={item.ConfigID}");
                return;
            }
            // 随机追加
            if (item.ConfigData.AppendAttrId.Count != 0)
            {
                item.SetProp(EItemValue.OptValue, item.ConfigData.AppendAttrId[RandomHelper.RandomNumber(0, item.ConfigData.AppendAttrId.Count)]);
            }

            if(StrengthenItemSystem.StrengthenResult(luckyRate))
            {
                // 设置幸运属性
                item.SetProp(EItemValue.LuckyEquip, 1);
            }

            if (StrengthenItemSystem.StrengthenResult(specialEntryRate))
            {
                // 设置特殊属性
                int entryCount = RandomHelper.RandomNumber(entryCountMin, entryCountMax);
                item.RandSpecialEntry(entryCount);
            }
        }
    }
}