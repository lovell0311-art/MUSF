using ETModel;
using NPOI.SS.Formula.Functions;


namespace ETHotfix
{
    /// <summary>
    /// 推送实体穿戴装备(周边一定范围玩家也广播)
    /// </summary>
    [MessageHandler]

    public class G2C_UnitEquipLoad_notice_Handler : AMHandler<G2C_UnitEquipLoad_notice>
    {
        protected override void Run(ETModel.Session session, G2C_UnitEquipLoad_notice message)
        {
            
            //遍历所有装备
            for (int i = 0, length=message.AllEquipStatus.Count; i < length; i++)
            {
             
               var item=message.AllEquipStatus[i];
                //线路切换时
                if (item.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID && UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic.ContainsKey((E_Grid_Type)item.SlotID)
                    &&(E_Grid_Type)item.SlotID != E_Grid_Type.Pet)
                {
                    continue;
                }


                KnapsackDataItem dataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(item.ItemUID);
                dataItem.GameUserId = item.GameUserId;//装备所属玩家
                dataItem.UUID = item.ItemUID;//物品UUID
                dataItem.ConfigId = item.ConfigID;//配置表ID
                dataItem.SetProperValue(E_ItemValue.Level, item.ItemLevel) ;//物品等级
                dataItem.Slot = item.SlotID;//物品卡槽信息
                //TODO 需新加一个追加等级

                //更新本地玩家的装备
                if (item.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    
                    var equip = UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>();
                   
                  
                    //缓存玩家 当前穿戴的装备
                    equip.CacheEquipment((E_Grid_Type)item.SlotID, dataItem);
                    //更新玩家装备
                
                    equip.UpdateRoleEquipment(item);

                    if (UIKnapsackComponent.Instance != null)
                    {
                        if ((E_Grid_Type)item.SlotID != E_Grid_Type.Mounts)
                            UIKnapsackComponent.Instance.AddItem(dataItem, true, type: E_Grid_Type.Knapsack);
                    }
                    UI ui = UIComponent.Instance.Get(UIType.UIKnapsackNew);
                    if (ui != null)
                    {
                        ui.GetComponent<UIKnapsackNewComponent>().AddItem(dataItem, true, type: E_Grid_Type.Knapsack);
                    }
                    //显示守护
                    if (UIMainComponent.Instance != null && (E_Grid_Type)item.SlotID == E_Grid_Type.Guard)
                    {
                        UIMainComponent.Instance.ShowGuard(item.ConfigID);
                    }
                }
                else
                {
                    if (UnitEntityComponent.Instance.RoleEntityDic.TryGetValue(item.GameUserId,out RoleEntity roleEntity))
                    {
                        //缓存玩家 当前穿戴的装备
                        roleEntity.GetComponent<RoleEquipmentComponent>().CacheEquipment((E_Grid_Type)item.SlotID, dataItem);
                        //更新玩家装备
                        roleEntity.GetComponent<RoleEquipmentComponent>().UpdateRoleEquipment(item);

                        //刷新玩家装备
                        UnitEntityComponent.Instance.SetUnitObjState(GlobalDataManager.IsHideRole, roleEntity);
                    }
                }
            }
          
        }
    }
}