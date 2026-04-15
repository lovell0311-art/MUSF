
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
    /// 合成宝箱
    /// </summary>
    [SynthesisRule(typeof(GoldChestSynthesis))]
    public class GoldChestSynthesis : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            await TreasureBoxSynthesis.Run(synthesis, itemList, config, b_Response, 320408, mClientFinalR);
        }

        public static class TreasureBoxSynthesis
        {
            public static async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int curSynItemConfigID,int mClientFinalR)
            {
                int curSuccessRate = config.BaseSuccessRate;

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
                for (int i = itemList.Count - 1; i >= 0; i--)
                {
                    if (itemList[i] == null)
                    {
                        itemList.RemoveAt(i);   //消耗物品以后清理所有空对象
                    }
                }
                //synthesis.UseItem(NeedItem1.ItemUID, config.Info, NeedItem1.GetProp(EItemValue.Quantity));
                if (mClientFinalR != curSuccessRate && config.MaxSuccessRate > curSuccessRate)
                {
                    Log.Warning($"方法: GoldChestSynthesis 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
                }
                //限制最大成功率
                if (curSuccessRate > config.MaxSuccessRate)
                {
                    curSuccessRate = config.MaxSuccessRate;
                }
                //消耗完毕,清空缓存空间
                //synthesis.BackAllItemToBackpack(config.Info);
                synthesis.mItemDict.Clear();
                //判断是否合成成功
                if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
                {
                    Item resultItem = ItemFactory.Create(curSynItemConfigID, synthesis.mPlayer.GameAreaId, 1);
                    if (!synthesis.AddResultItem(resultItem))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                        b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                        return;
                    }
                    b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(resultItem));
                    b_Response.Result = true;
                }
                else
                {
                    b_Response.Result = false;
                }

                return;
            }
        }

    }
}