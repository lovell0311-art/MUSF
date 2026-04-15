
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
    /// 一般合成-果实合成
    /// </summary>
    [SynthesisRule(typeof(Attributefruit))]
    public class Attributefruit : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {

        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            List<int> GuoShiDict = new List<int>()
            {
                310081,310082,310083,310084,310085
            };
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
            //消耗物品
            if (!synthesis.UseAllItem(config.NeedItemsDic, itemList, config.Info))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1719);
                return;
            }
            synthesis.BackAllItemToBackpack(config.Info);
            if (mClientFinalR != config.BaseSuccessRate)
            {
                Log.Warning($"方法: Attributefruit 前后概率不一致C:{mClientFinalR}S:{config.BaseSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //生成合成物品
            Item resultItem = ItemFactory.Create(GuoShiDict[RandomHelper.RandomNumber(0, GuoShiDict.Count)], synthesis.mPlayer.GameAreaId);

            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(config.BaseSuccessRate))
            {
                ////生成合成物品
                //Item resultItem = ItemFactory.Create(GuoShiDict[RandomHelper.RandomNumber(0, GuoShiDict.Count)], synthesis.mPlayer.GameAreaId);

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
            else
            {
                b_Response.Result = false;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, false).Coroutine();
            }

            return;
        }
    }
}