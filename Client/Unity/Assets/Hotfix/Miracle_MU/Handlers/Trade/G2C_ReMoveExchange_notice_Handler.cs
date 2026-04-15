using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    /// <summary>
    /// 物品从交易面板 移动 到背包
    /// </summary>
    [MessageHandler]
    public class G2C_ReMoveExchange_notice_Handler : AMHandler<G2C_ReMoveExchange_notice>
    {
        protected override void Run(ETModel.Session session, G2C_ReMoveExchange_notice message)
        {
            if (UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>() is UIKnapsackComponent uIKnapsack)
            {
              
                if (uIKnapsack.OtherTradeItemDic.TryGetValue(message.ItemUUID, out KnapsackDataItem dataItem))
                {
                    dataItem.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                    UIComponent.Instance.VisibleUI(UIType.UIHint,$"对方移除了 {item_Info.Name}");
                    KnapsackGridData data = new KnapsackGridData
                    {
                        UUID = dataItem.UUID,
                        ItemData = dataItem,
                        Point1 = new Vector2Int(dataItem.PosInBackpackX, dataItem.PosInBackpackY),
                        Point2 = new Vector2Int(dataItem.PosInBackpackX+item_Info.X-1,dataItem.PosInBackpackY+item_Info.Y-1),
                        Grid_Type=E_Grid_Type.Trade_Other
                        
                    };
                    uIKnapsack.RemoveItem(data,true,true);
                    dataItem.Dispose();
                    uIKnapsack.OtherTradeItemDic.Remove(message.ItemUUID);

                }
            }
        }
    }
}