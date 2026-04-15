
using System;
using System.Collections;
using System.Collections.Generic;
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
    /// 卓越换奇迹币
    /// </summary>
    [SynthesisRule(typeof(ExcellentExchangeCoin))]
    public class ExcellentExchangeCoin : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            int curSuccessRate = config.BaseSuccessRate;
            int curSynItemConfigID = 320316;
            List<int> CntList = new List<int>();
            int SumCnt = 0;
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
                if (item.data.ConfigID == 340001)
                {
                    synthesis.AddResultItem(item);
                    continue;
                }
                if (item.data.ConfigID == 320316)
                {
                    CntList.Add(item.GetProp(EItemValue.Quantity));
                    continue;
                }
                //if (item.data.ExcellentEntry.Count >= 1 && item.GetProp(EItemValue.Level) >= 1)
                //{
                //    int Cnt = item.GetProp(EItemValue.Level) * 10;
                //    CntList.Add(Cnt);
                //}
                if (item.HaveSetOption())
                {
                    CntList.Add(100);
                }
                switch (item.data.ExcellentEntry.Count)
                {
                    case 1: CntList.Add(20); break;
                    case 2: CntList.Add(30); break;
                    case 3: CntList.Add(40); break;
                    case 4: CntList.Add(60); break;
                    case 5: CntList.Add(80); break;
                    case 6: CntList.Add(100); break;
                }

                switch (item.data.ConfigID)
                {
                    case (int)EItemStrengthen.MAYA_GEMS:
                        int Cnt = item.GetProp(EItemValue.Quantity) * 20;
                        CntList.Add(Cnt);
                        break;
                    //case (int)EItemStrengthen.RECYCLED_GEMS:
                    case (int)EItemStrengthen.CREATE_GEMS:
                    case (int)EItemStrengthen.BLESSING_GEMS:
                        int Cnt3 = item.GetProp(EItemValue.Quantity) * 50;
                        CntList.Add(Cnt3);
                        break;
                    case (int)EItemStrengthen.SOUL_GEMS:
                        int Cnt1 = item.GetProp(EItemValue.Quantity) * 30;
                        CntList.Add(Cnt1);
                        break;
                    case (int)EItemStrengthen.ANIMA_GEMS:
                        int Cnt2 = item.GetProp(EItemValue.Quantity) * 80;
                        CntList.Add(Cnt2);
                        break;
                }
            }
            SumCnt = CntList.Sum();

            //=================判定逻辑结束，开始消耗逻辑(所有异常放到上面报，下面默认判断为合成逻辑成功)==================
            //消耗金币
            if (!synthesis.UseGold((int)config.NeedGold))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                return;
            }

            foreach(Item item in itemList)
                synthesis.UseItem(item.ItemUID, config.Info, item.GetProp(EItemValue.Quantity));

            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                if (itemList[i] == null)
                {
                    itemList.RemoveAt(i);   //消耗物品以后清理所有空对象
                }
            }
            if (mClientFinalR != curSuccessRate && config.MaxSuccessRate > curSuccessRate)
            {
                Log.Warning($"方法: ExcellentExchangeCoin 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //限制最大成功率
            if (curSuccessRate > config.MaxSuccessRate)
            {
                curSuccessRate = config.MaxSuccessRate;
            }
            //消耗完毕，返回所有材料至背包
            synthesis.BackAllItemToBackpack(config.Info);
            //生成合成物品
            Item resultItem = ItemFactory.Create(curSynItemConfigID, synthesis.mPlayer.GameAreaId, SumCnt);
            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
            {
                //if (!synthesis.AddResultItem(resultItem))
                //{
                //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                //    b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                //    return;
                //}

                // b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(resultItem));
                b_Response.Result = true;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, true).Coroutine();
                BackpackComponent backpack = synthesis.Parent?.GetCustomComponent<BackpackComponent>();
                backpack?.AddItem(resultItem, "合成奇迹币");
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