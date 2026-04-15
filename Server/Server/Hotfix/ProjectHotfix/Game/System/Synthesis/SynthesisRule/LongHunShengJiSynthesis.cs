
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
    /// 坐骑护卫-龙魂（冰晶）/圣迹龙魂（赤耀）合成
    /// </summary>
    [SynthesisRule(typeof(LongHunShengJiSynthesis))]
    public class LongHunShengJiSynthesis : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {

        const int YouLingZhanMa = 260007;
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            List<int> SynResultList = new List<int>()
            {
                260005,260006
            };
            int curSuccessRate = config.BaseSuccessRate;
            Item ZhanMaItem = null;
            //检查物品
            if (!synthesis.CheckItem(config.NeedItemsDic, itemList))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类或数量不正确";
                return;
            }
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].ConfigID == YouLingZhanMa)
                {
                    if (ZhanMaItem != null)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                        b_Response.Message = "幽灵战马只能放一个";
                        return;
                    }
                    ZhanMaItem = itemList[i];
                    int level = ZhanMaItem.GetProp(EItemValue.Level);
                    if (level < 11)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                        b_Response.Message = "幽灵战马少于+11";
                        return;
                    }
                    curSuccessRate += (level - 11) * 10;
                }
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
                Log.Warning($"方法: LongHunShengJiSynthesis 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //生成合成物品
            Item resultItem = ItemFactory.Create(SynResultList[RandomHelper.RandomNumber(0, SynResultList.Count)], synthesis.mPlayer.GameAreaId);

            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
            {
                
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