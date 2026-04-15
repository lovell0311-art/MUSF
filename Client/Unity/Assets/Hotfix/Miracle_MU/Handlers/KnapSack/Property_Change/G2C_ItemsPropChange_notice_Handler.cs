using System.Collections.Generic;
using ETModel;
using System.Linq;
using UnityEngine.Profiling;

namespace ETHotfix
{
    /// <summary>
    /// 广播物品属性变动  
    /// </summary>
    [MessageHandler]
    public class G2C_ItemsPropChange_notice_Handler : AMHandler<G2C_ItemsPropChange_notice>
    {
        private static bool autoRepairPending = false;

        protected override void Run(ETModel.Session session, G2C_ItemsPropChange_notice message)
        {
        
            RoleEquipmentComponent roleEquipment = UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>();

           
            //message.Scene 0 默认 2仓库 3交易 4摆摊
          
            for (int i = 0, length = message.PropList.Count; i < length; i++)
            {
                var property = message.PropList[i];
               
                //更新装备属性
                for (int j = 0, count = roleEquipment.curWareEquipsData_Dic.Count; j < count; j++)
                {

                    var item = roleEquipment.curWareEquipsData_Dic.ElementAt(j).Value;
                    if (item.UUID == message.ItemUUID)
                    {
                        item.Set(property);
                    }
                    ///更新守护耐久
                    if ((E_ItemValue)property.PropID == E_ItemValue.Durability)
                    {
                        if (roleEquipment.curWareEquipsData_Dic.ElementAt(j).Key == E_Grid_Type.Guard)
                        {
                           // if (UIComponent.Instance.Get(UIType.UIMainCanvas)?.GetComponent<UIMainComponent>() is UIMainComponent mainComponent)
                            {
                                UIMainComponent.Instance?.ChangeGuardDurability();
                            }
                        }
                    }

                }
                UI uI = UIComponent.Instance.Get(UIType.UIKnapsackNew);
                UIKnapsackNewComponent uIKnapsackNew = null;
                if (uI != null)
                {
                    uIKnapsackNew = uI.GetComponent<UIKnapsackNewComponent>();
                }
                //更新背包中的装备 属性
                if (KnapsackItemsManager.KnapsackItems.TryGetValue(message.ItemUUID, out KnapsackDataItem KnapsackItem))
                {
                    var count = KnapsackItem.GetProperValue(E_ItemValue.Quantity);

                    KnapsackItem.Set(property);
                    KnapsackItem.ConfigId.GetItemInfo_Out(out KnapsackItem.item_Info);
                    if (property.PropID == (int)E_ItemValue.Quantity && KnapsackItem.item_Info.StackSize > 1)
                    {
                        //更新药瓶数量
                        if (count < property.Value)
                        {

                            UIMainComponent.Instance?.AddNum(KnapsackItem, property.Value - count);
                        }
                        else
                        {
                            UIMainComponent.Instance?.ChangeNum(KnapsackItem, false);
                        }

                        if (UIKnapsackComponent.Instance != null)
                        {

                            UIKnapsackComponent.Instance.ChangeKnapsackItemCount(KnapsackItem.Id, property.Value);
                        }

                        if (uIKnapsackNew != null)
                        {
                            if (KnapsackItem.Id != 0)
                            {

                                uIKnapsackNew.ChangeKnapsackItemCount(KnapsackItem.Id, property.Value);
                            }
                        }


                        //if (UIComponent.Instance.Get(UIType.UIKnapsack)?.GetComponent<UIKnapsackComponent>() is UIKnapsackComponent knapsackComponent)
                        //{
                           
                        //    knapsackComponent.ChangeKnapsackItemCount(KnapsackItem.Id, property.Value);

                        //}

                    }
                }
                //更新仓库 属性
                else if (KnapsackItemsManager.WareHouseItems.TryGetValue(message.ItemUUID, out KnapsackDataItem waredataItem))
                {

                    //Log.DebugBrown($"更新仓库中的属性：{waredataItem.ConfigId} {property.PropID}->{property.Value}");
                    waredataItem.Set(property);

                }
                //更新 交易面板的属性
                else if (UIKnapsackComponent.Instance != null && UIKnapsackComponent.Instance.OtherTradeItemDic.TryGetValue(message.ItemUUID, out KnapsackDataItem knapsack))
                {
                    knapsack.Set(property);
                }
                ////更新合成装备属性
                else if (UIKnapsackComponent.Instance != null && UIKnapsackComponent.Instance.mergerItemList.TryGetValue(message.ItemUUID, out KnapsackDataItem knapsack1)) 
                {
                    knapsack1.Set(property);
                }
                else
                {
                    //更新摊位物品的属性
                    if (message.Scene == (int)E_ItemPropertyNotice.StallUp && UIKnapsackComponent.Instance != null)
                    {
                        //本地玩家摊位属性
                        if (message.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID)
                        {
                            var stallUpComponent = UnitEntityComponent.Instance.LocalRole.GetComponent<RoleStallUpComponent>();
                            if (stallUpComponent.StallUpItemDic.TryGetValue(message.ItemUUID, out KnapsackDataItem knapsackDataItem))
                            {
                                knapsackDataItem.Set(property);
                            }
                            else
                            {
                                knapsackDataItem = UIKnapsackComponent.Instance.AddStallUpItem();
                                knapsackDataItem.Set(property);
                            }
                        }
                        else
                        {
                            if (UnitEntityComponent.Instance.Get<RoleEntity>(message.GameUserId) is RoleEntity roleEntity)
                            {
                                var stallUpComponent = roleEntity.GetComponent<RoleStallUpComponent>();
                                if (stallUpComponent.StallUpItemDic.TryGetValue(message.ItemUUID, out KnapsackDataItem knapsackDataItem))
                                {
                                    knapsackDataItem.Set(property);
                                }
                                else
                                {
                                    knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(message.ItemUUID);
                                    knapsackDataItem.Set(property);
                                    stallUpComponent.StallUpItemDic[message.ItemUUID] = knapsackDataItem;
                                }
                            }
                        }
                    }


                    /* if (message.Scene == (int)E_ItemPropertyNotice.StallUp && UnitEntityComponent.Instance.GetAllRoleEntityDic().TryGetValue(message.GameUserId, out RoleEntity roleEntity))
                     {
                         // Log.DebugWhtie($"UIKnapsackComponent.Instance == null:{UIKnapsackComponent.Instance == null}");
                         if (UIKnapsackComponent.Instance == null) continue;
                         var stallUpComponent = roleEntity.GetComponent<RoleStallUpComponent>();

                         if (stallUpComponent.StallUpItemDic.TryGetValue(message.ItemUUID, out KnapsackDataItem knapsackDataItem))
                         {
                             Log.DebugGreen($"更新摊位属性 {stallUpComponent.StallUpItemDic.Count} 是否是本地玩家：{message.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID}");
                             knapsackDataItem.Set(property);

                         }
                         else
                         {
                             Log.DebugGreen($"creat KnapsackDataItem:{message.ItemUUID} :{property.PropID}:{property.Value}");
                             knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(message.ItemUUID);
                             knapsackDataItem.Set(property);
                             stallUpComponent.StallUpItemDic[message.ItemUUID] = knapsackDataItem;
                         }
                     }*/
                }
            }

            //大月卡自动维修特权

          //  if (UnitEntityComponent.Instance.LocalRole?.MaxMonthluCardTimeSpan.TotalSeconds > 0|| UnitEntityComponent.Instance.LocalRole?.MinMonthluCardTimeSpan.TotalSeconds>0)
            if (GlobalDataManager.IsEnteringGame
                || GlobalDataManager.ChangeSceneIsChooseRole
                || GlobalDataManager.IsStartReConnect
                || autoRepairPending
                || UIComponent.Instance?.Get(UIType.UISceneLoading) != null
                || SceneComponent.Instance?.CurrentSceneName == SceneName.ChooseRole.ToString())
            {
                return;
            }

            if (roleEquipment.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Guard))//守护自动维修
            {
                var RepairEquipList = new List<int>();//需要维修的装备
                var RepairNeedMoney = 0;//维修需要的价格
                for (E_Grid_Type i = E_Grid_Type.Weapon, length = E_Grid_Type.Pet; i < length; i++)
                {

                    if (roleEquipment.curWareEquipsData_Dic.TryGetValue(i, out KnapsackDataItem item))
                    {
                        if (i == E_Grid_Type.Guard)//守护不能维修
                            continue;

                        //维修价格==0 
                        if (item.GetProperValue(E_ItemValue.RepairMoney) <= 0) continue;

                        if ((float)item.GetProperValue(E_ItemValue.Durability) / item.GetProperValue(E_ItemValue.DurabilityMax) <= 0.8)
                        {
                            RepairEquipList.Add((int)i);
                            if (item.GetProperValue(E_ItemValue.RepairMoney) is int money)
                            {
                                RepairNeedMoney += money;

                            }
                        }

                    }
                }
                if (RepairNeedMoney == 0)
                {
                    //没有装备需要维修
                    return;
                }
                //金币是否 足够
                if (RepairNeedMoney > UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.GoldCoin))
                {
                    return;
                }
                autoRepairPending = true;
                RepairAsync().Coroutine();

                //请求维修
                async ETVoid RepairAsync()
                {
                    try
                    {
                        G2C_RepairEquipItemResponse g2C_RepairEquip = (G2C_RepairEquipItemResponse)await SessionComponent.Instance.Session.Call(new C2G_RepairEquipItemRequest
                        {
                            EquipPosition = new Google.Protobuf.Collections.RepeatedField<int> { RepairEquipList },//指定装备栏id (维修价格大于0 就可以维修)
                            NpcUID = 0,
                            NpcPosX = 0,
                            NpcPosY = 0,
                        });
                        if (g2C_RepairEquip.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RepairEquip.Error.GetTipInfo());
                        }
                        else
                        {
                            RepairEquipList = null;
                            // UIComponent.Instance.VisibleUI(UIType.UIHint, "维修成功");
                        }
                    }
                    finally
                    {
                        autoRepairPending = false;
                    }
                }
            }

        }
    }

}
