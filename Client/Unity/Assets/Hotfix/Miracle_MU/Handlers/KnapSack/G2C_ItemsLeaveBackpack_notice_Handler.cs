using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 推送客户端背包中有物品离开
    /// </summary>
    [MessageHandler]
    public class G2C_ItemsLeaveBackpack_notice_Handler : AMHandler<G2C_ItemsLeaveBackpack_notice>
    {
        protected override void Run(ETModel.Session session, G2C_ItemsLeaveBackpack_notice message)
        {

           ///背包 中有物品离开
           // if (KnapsackItemsManager.KnapsackItems.ContainsKey(message.LeaveItemUUID))
            if (KnapsackItemsManager.KnapsackItems.TryGetValue(message.LeaveItemUUID,out KnapsackDataItem knapsackDataItem))
            {

                if (knapsackDataItem.ItemType == (int)E_ItemType.Consumables)
                {
                    //Log.DebugGreen($"数量：{KnapsackItemsManager.KnapsackItems[message.LeaveItemUUID].GetProperValue(E_ItemValue.Quantity)}");
                    //更新主界面的数量
                    UIMainComponent.Instance.ChangeNum(KnapsackItemsManager.KnapsackItems[message.LeaveItemUUID]);
                }

              //  Log.DebugRed($"背包中移除：{message.LeaveItemUUID}  {KnapsackItemsManager.KnapsackItems[message.LeaveItemUUID].ConfigId}");
                Game.EventCenter.EventTrigger<long>(EventTypeId.RemoveKnapsack, message.LeaveItemUUID);
               /* if (UIKnapsackComponent.Instance!=null)
                {
               //镶嵌、仓库、背包、商城、摆摊、交易
                    if (knapsackDataItem.ItemType == (int)E_ItemType.Consumables||UIKnapsackComponent.Instance.curKnapsackState==E_KnapsackState.KS_Inlay
                        || UIKnapsackComponent.Instance.curKnapsackState == E_KnapsackState.KS_Ware_House || UIKnapsackComponent.Instance.curKnapsackState == E_KnapsackState.KS_Knapsack || UIKnapsackComponent.Instance.curKnapsackState == E_KnapsackState.KS_Shop || UIKnapsackComponent.Instance.curKnapsackState == E_KnapsackState.KS_Stallup|| UIKnapsackComponent.Instance.curKnapsackState == E_KnapsackState.KS_Trade
                       )
                    {
                        
                        UIKnapsackComponent.Instance.RemoveKnapsack(message.LeaveItemUUID);
                        KnapsackItemsManager.KnapsackItems[message.LeaveItemUUID].Dispose();
                    }
                   
                }*/
               
                KnapsackItemsManager.KnapsackItems.Remove(message.LeaveItemUUID);
              
                //关闭藏宝图提示
                if (knapsackDataItem.ConfigId == 310073)
                {
                    UIMainComponent.Instance?.HideFuBenInfo();
                }
                //移除缓存的坐骑
                if (KnapsackItemsManager.MountUUIDList.Contains(message.LeaveItemUUID))
                {
                    KnapsackItemsManager.MountUUIDList.Remove(message.LeaveItemUUID);
                    if (UIMainComponent.Instance.curMountUUID == message.LeaveItemUUID)
                    {
                        UIMainComponent.Instance.curMountUUID = -1;
                    }
                }

            }
          

        }
    }
}
