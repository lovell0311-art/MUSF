
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
    /// 去除在生属性
    /// </summary>
    [SynthesisRule(typeof(RemoveRegeneration))]
    public class RemoveRegeneration : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {

            Item NeedItem1 = null;  //合成主要材料1
            int curSuccessRate = config.BaseSuccessRate;

            //检查金币
            if (!synthesis.CheckGold((int)config.NeedGold))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                return;
            }

            bool haveOtherItem = false;
            int targetCount = 0;
            //检查是否有主要物品
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                if (!itemList[i].CanStrengthen())
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                    b_Response.Message = "只能强化武器、防具和翅膀";
                    return;
                }
                if (itemList[i].GetProp(EItemValue.OrecycledID) <= 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                    b_Response.Message = "只能强化武器、防具和翅膀";
                    return;
                }
                targetCount++;
                NeedItem1 = itemList[i];
                itemList.RemoveAt(i);
                if (targetCount > 1)
                {
                    haveOtherItem = true;
                }
            }
            if (haveOtherItem)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "物品种类不正确";
                return;
            }
            if (NeedItem1 == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                b_Response.Message = "装备要求等级或追加不足";
                return;
            }

            //=================判定逻辑结束，开始消耗逻辑(所有异常放到上面报，下面默认判断为合成逻辑成功)==================
            //消耗金币
            if (!synthesis.UseGold((int)config.NeedGold))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
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

                NeedItem1.SetProp(EItemValue.OrecycledID, 0);
                NeedItem1.SetProp(EItemValue.OrecycledLevel, 0);
                if (!synthesis.AddResultItem(NeedItem1))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                    return;
                }
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(NeedItem1));
                b_Response.Result = true;
            }
            else
            {
                if (!synthesis.AddResultItem(NeedItem1))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                    return;
                }
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(NeedItem1));
                b_Response.Result = false;
            }

            return;

        }
    }
}