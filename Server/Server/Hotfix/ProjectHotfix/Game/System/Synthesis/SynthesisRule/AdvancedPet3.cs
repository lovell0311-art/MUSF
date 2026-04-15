
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    /// <summary>
    /// 宠物进阶3
    /// </summary>
    [SynthesisRule(typeof(AdvancedPet3))]
    public class AdvancedPet3 : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis, int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            int curSuccessRate = config.BaseSuccessRate;
            Item NeedItem1 = null;  //合成主要材料
            Item NeedItem2 = null;  //合成副材料
            //检查金币
            if (!synthesis.CheckGold((int)config.NeedGold))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                return;
            }
            if (itemList.Count <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类或数量不正确";
                return;
            }

            //检查物品
            foreach (Item item in itemList)
            {
                if (item.IsPets())
                {
                    if (NeedItem1 == null)
                        NeedItem1 = item;  //合成主要材料
                    else if (NeedItem1 != null)
                    {
                        if (NeedItem1.GetProp(EItemValue.Level) > item.GetProp(EItemValue.Level))
                        {
                            NeedItem2 = item;
                        }
                        else if (NeedItem1.GetProp(EItemValue.Level) < item.GetProp(EItemValue.Level))
                        {
                            NeedItem2 = NeedItem1;
                            NeedItem1 = item;
                        }
                        else
                        {
                            if (NeedItem1.data.ExcellentEntry.Count >= item.data.ExcellentEntry.Count)
                            {
                                NeedItem2 = item;
                            }
                            else
                            {
                                NeedItem2 = NeedItem1;
                                NeedItem1 = item;
                            }
                        }
                    }
                }
            }
            if (NeedItem2 == null || NeedItem1 == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类或数量不正确";
                return;
            }
            if (NeedItem1.ConfigID != NeedItem2.ConfigID)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类或数量不正确";
                return;
            }
            if (!synthesis.CheckItem(config.NeedItemsDic, itemList))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类或数量不正确";
                return;
            }
            int curSynItemConfigID = 0;
            switch (NeedItem1.ConfigID)
            {
                case 350015: curSynItemConfigID = 350022; break;
                case 350016: curSynItemConfigID = 350023; break;
                case 350017: curSynItemConfigID = 350024; break;
                case 350019: curSynItemConfigID = 350026; break;
                case 350020: curSynItemConfigID = 350027; break;
                case 350021: curSynItemConfigID = 350028; break;
                case 350031: curSynItemConfigID = 350032; break;
            }
            //=================判定逻辑结束，开始消耗逻辑(所有异常放到上面报，下面默认判断为合成逻辑成功)==================
            //消耗金币
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
            synthesis.UseItem(NeedItem2.ItemUID, config.Info, NeedItem2.GetProp(EItemValue.Quantity));
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                if (itemList[i] == null)
                {
                    itemList.RemoveAt(i);   //消耗物品以后清理所有空对象
                }
            }
            if (mClientFinalR != curSuccessRate && config.MaxSuccessRate > curSuccessRate)
            {
                Log.Warning($"方法: PEvolutionGem 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //限制最大成功率
            if (curSuccessRate > config.MaxSuccessRate)
            {
                curSuccessRate = config.MaxSuccessRate;
            }
            //消耗完毕，返回所有材料至背包
            //synthesis.BackAllItemToBackpack(config.Info);

            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
            {
                //生成合成物品
                Item resultItem = ItemFactory.Create(curSynItemConfigID, synthesis.mPlayer.GameAreaId);
                resultItem.SetProp(EItemValue.Level, NeedItem1.GetProp(EItemValue.Level));
                resultItem.SetProp(EItemValue.LuckyEquip, 1);
                resultItem.SetProp(EItemValue.OptLevel, NeedItem1.GetProp(EItemValue.OptLevel));
                resultItem.data.ExcellentEntry = new HashSet<int>(NeedItem1.data.ExcellentEntry);
                if (resultItem.data.ExcellentEntry.Count <= 5)
                {
                    var excAttrEntryManager = Root.MainFactory.GetCustomComponent<ExcAttrEntryManagerComponent>();
                    if (excAttrEntryManager.TryGetPetsAttrEntry(out var selector))
                    {

                        var newSelector = new RandomSelector<int>(selector);
                        do
                        {
                            if (newSelector.TryGetValueAndRemove(out var entryId))
                            {
                                if (!resultItem.data.ExcellentEntry.Contains(entryId))
                                {
                                    resultItem.data.ExcellentEntry.Add(entryId);
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        while (true);
                    }
                }
                synthesis.UseItem(NeedItem1.ItemUID, config.Info, NeedItem1.GetProp(EItemValue.Quantity));
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
                b_Response.Result = false;
                //synthesis.Parent?.GetCustomComponent<GamePlayer>().SendItem(3, resultItem, false).Coroutine();
            }

            return;
        }
    }
}