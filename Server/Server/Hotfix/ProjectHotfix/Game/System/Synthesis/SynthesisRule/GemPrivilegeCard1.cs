
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
    [SynthesisRule(typeof(GemPrivilegeCard1))]
    public class GemPrivilegeCard1 : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis, int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            return;
            int curSuccessRate = config.BaseSuccessRate;

            uint SumCnt = 0;
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

                if (item.Type != EItemType.Gemstone)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                    b_Response.Message = "物品种类或数量不正确";
                    return;
                }
                switch (item.data.ConfigID)
                {
                    case (int)EItemStrengthen.CREATE_GEMS:
                        SumCnt += 2;
                        break;
                    case (int)EItemStrengthen.MAYA_GEMS:
                        SumCnt += 1;
                        break;
                    case (int)EItemStrengthen.RECYCLED_GEMS:
                        SumCnt += 2;
                        break;
                    case (int)EItemStrengthen.BLESSING_GEMS:
                    case (int)EItemStrengthen.SOUL_GEMS:
                        SumCnt += 4;
                        break;
                    case (int)EItemStrengthen.ANIMA_GEMS:
                        SumCnt += 8;
                        break;
                }
            }
            SumCnt *= 24 * 60 * 60 * 1000;

            //=================判定逻辑结束，开始消耗逻辑(所有异常放到上面报，下面默认判断为合成逻辑成功)==================
            //消耗金币
            if (!synthesis.UseGold((int)config.NeedGold))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                return;
            }

            foreach (Item item in itemList)
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
            if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
            {
                var ShopVip = synthesis.Parent?.GetCustomComponent<PlayerShopMallComponent>();
                if (ShopVip != null) 
                {
                    ShopVip.HeBingMonthlyCard(true, SumCnt);
                    ShopVip.SendPlayerShopState();
                }
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