using ETModel;
using static UnityEditor.Progress;

namespace ETHotfix
{
    /// <summary>
    /// 推送客户端背包中有物品进入 选择角色进入游戏场景时会推送玩家背包里现有物品
    /// </summary>
    [MessageHandler]
    public class G2C_ItemsIntoBackpack_notice_Handler : AMHandler<G2C_ItemsIntoBackpack_notice>
    {
        protected override void Run(ETModel.Session session, G2C_ItemsIntoBackpack_notice message)
        {

            for (int i = 0, length = message.AllItems.Count; i < length; i++)
            {

                Struct_ItemInBackpack_Status item = message.AllItems[i];

                if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.ItemUID))
                {
                    continue;
                }

                KnapsackDataItem knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(item.ItemUID);
                knapsackDataItem.GameUserId = item.GameUserId;//玩家的UID
                knapsackDataItem.UUID = item.ItemUID;//装备的UID
                knapsackDataItem.ConfigId = item.ConfigID;//装备配置表id
                knapsackDataItem.ItemType = item.Type;//装备类型
                knapsackDataItem.PosInBackpackX = item.PosInBackpackX;//装备在背包中的起始格子 坐标
                knapsackDataItem.PosInBackpackY = item.PosInBackpackY;
                knapsackDataItem.X = item.Width;//装备所占的格子
                knapsackDataItem.Y = item.Height;
                knapsackDataItem.SetProperValue(E_ItemValue.Quantity, item.Quantity);//装备的数量
                knapsackDataItem.SetProperValue(E_ItemValue.Level, item.ItemLevel);//装备的等级
                KnapsackItemsManager.KnapsackItems[item.ItemUID] = knapsackDataItem;
                KnapsackTools.AddEquip(knapsackDataItem);
                Remove().Coroutine();
                async ETTask Remove()
                {
                    await TimerComponent.Instance.WaitAsync(1000);
                    if (KnapsackItemsManager.WaitingForAutomaticRecyclings.FindIndex(p => p == knapsackDataItem.UUID) != -1)
                    {
                        Log.Info("进入背包的物品 属于等待回收的物品  执行回收 " + knapsackDataItem.UUID);
                        RecycleEquipTools.AutoSell(knapsackDataItem);
                        KnapsackItemsManager.WaitingForAutomaticRecyclings.Remove(knapsackDataItem.UUID);
                    }
                }
                //缓存坐骑
                if (knapsackDataItem.ItemType == (int)E_ItemType.Mounts)
                {
                    KnapsackItemsManager.MountUUIDList.Add(item.ItemUID);
                }

                //背包面板打开
                if (UIComponent.Instance.Get(UIType.UIKnapsack)?.GetComponent<UIKnapsackComponent>() is UIKnapsackComponent knapsackComponent)
                {
                    knapsackComponent.AddItem(knapsackDataItem, type: E_Grid_Type.Knapsack);

                }
                {
                    UI uI = UIComponent.Instance.Get(UIType.UIKnapsackNew);
                    if (uI != null)
                    {
                        uI.GetComponent<UIKnapsackNewComponent>().AddItem(knapsackDataItem, type: E_Grid_Type.Knapsack);
                    }
                }
                ///生命药水
                if (KnapsackItemsManager.MedicineHpIdList.Contains(item.ConfigID))
                {

                    //if (UIComponent.Instance.Get(UIType.UIMainCanvas)?.GetComponent<UIMainComponent>() is UIMainComponent mainComponent)
                    {
                        UIMainComponent.Instance?.medicineEntity_Hp.Add(item.Quantity, item.ItemUID);

                    }
                }
                ///魔力药水
                if (KnapsackItemsManager.MedicineMpIdList.Contains(item.ConfigID))
                {

                    //  if (UIComponent.Instance.Get(UIType.UIMainCanvas)?.GetComponent<UIMainComponent>() is UIMainComponent mainComponent)
                    {
                        UIMainComponent.Instance?.medicineEntity_Mp.Add(item.Quantity, item.ItemUID);
                    }
                }

            

            }

        }

    }
}