using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 摆摊结果通知
    /// 摊位 开启时 有人购买了摊位的物品。就移除掉摊位上的物品
    /// </summary>
    [MessageHandler]
    public class G2C_BaiTanResult_notice_Handler : AMHandler<G2C_BaiTanResult_notice>
    {
        protected override void Run(ETModel.Session session, G2C_BaiTanResult_notice message)
        {
          
            if (UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>() is UIKnapsackComponent knapsackComponent)
            { 
            // knapsackComponent.RemoveWareHouse
            }
        }

       
    }
}
