
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
    /// 一般合成-透明披风
    /// </summary>
    [SynthesisRule(typeof(TouMingPiFengSynthesis))]
    public class TouMingPiFengSynthesis : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public const int TouMingPiFeng = 320303;   //透明披风
        public const int XueLingZhiShu = 320004;   //血灵之书
        public const int XueLingZhiKu = 320005;    //血灵之骷
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            Log.Debug("进入果实合成逻辑");
            //检查物品
            if (!synthesis.CheckItem(config.NeedItemsDic,itemList))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                return;
            }
            Item xuelingShu = null;
            Item xuelingKu = null;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].ConfigID == XueLingZhiShu)
                {
                    xuelingShu = itemList[i];
                }
                if (itemList[i].ConfigID == XueLingZhiKu)
                {
                    xuelingKu = itemList[i];
                }
            }
            if (xuelingShu == null || xuelingKu == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                return;
            }
            if (xuelingShu.GetProp(EItemValue.Level) !=  xuelingKu.GetProp(EItemValue.Level))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                return;
            }
            //=================判定逻辑结束，开始消耗逻辑(所有异常放到上面报，下面默认判断为合成逻辑成功)==================
            int itemLevel = xuelingShu.GetProp(EItemValue.Level);

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
                Log.Warning($"方法: TouMingPiFengSynthesis 前后概率不一致C:{mClientFinalR}S:{config.BaseSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //生成合成物品
            Item resultItem = ItemFactory.Create(TouMingPiFeng, synthesis.mPlayer.GameAreaId);
            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(config.BaseSuccessRate))
            {

                
               
                if (!synthesis.AddResultItem(resultItem))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";
                    return;
                }
                resultItem.SetProp(EItemValue.Level, itemLevel);
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(resultItem));
                b_Response.Result = true;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, true).Coroutine();
            }
            else {
                //b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1718);
                //b_Response.Message = "合成失败";
                b_Response.Result = false;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, false).Coroutine();
            }

            return;
        }
    }
}