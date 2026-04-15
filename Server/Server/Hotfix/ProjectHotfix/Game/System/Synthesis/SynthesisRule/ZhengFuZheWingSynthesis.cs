
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
    /// 翅膀披风-征服者的翅膀
    /// </summary>
    [SynthesisRule(typeof(ZhengFuZheWingSynthesis))]
    public class ZhengFuZheWingSynthesis : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        /// <summary>
        /// 出现幸运属性概率
        /// </summary>
        private const int LuckyAttrRate = 100;
        /// <summary>
        /// 出现特殊词条概率
        /// </summary>
        private const int SpecialEntryRate = 100;
        /// <summary>
        /// 出现特殊词条 条数
        /// </summary>
        private const int SpecialEntryCountMin = 1;
        private const int SpecialEntryCountMax = 1;
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            int curSuccessRate = config.BaseSuccessRate;
            //检查物品
            if (!synthesis.CheckItem(config.NeedItemsDic,itemList))
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
            //消耗物品
            if (!synthesis.UseAllItem(config.NeedItemsDic, itemList, config.Info))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1719);
                return;
            }
            synthesis.BackAllItemToBackpack(config.Info);
            if (mClientFinalR != curSuccessRate && config.MaxSuccessRate > curSuccessRate)
            {
                Log.Warning($"方法: ZhengFuZheWingSynthesis 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
            {
                //生成合成物品
                Item resultItem = ItemFactory.Create(220028, synthesis.mPlayer.GameAreaId);
                GenerationWingSynthesis.InitWingsProp_Rand(resultItem, LuckyAttrRate, SpecialEntryRate, SpecialEntryCountMin, SpecialEntryCountMax);

                if (!synthesis.AddResultItem(resultItem))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";
                    return;
                }
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(resultItem));
                b_Response.Result = true;
            }
            else {
                b_Response.Result = false;
            }

            return;
        }
    }
}