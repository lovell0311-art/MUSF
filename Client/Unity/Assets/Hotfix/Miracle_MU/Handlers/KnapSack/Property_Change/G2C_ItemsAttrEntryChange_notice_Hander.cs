using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 广播物品属性词条变动 (卓越属性词条)
    /// </summary>
    [MessageHandler]
    public class G2C_ItemsAttrEntryChange_notice_Hander : AMHandler<G2C_ItemsAttrEntryChange_notice>
    {
        protected override void Run(ETModel.Session session, G2C_ItemsAttrEntryChange_notice message)
        {
           
            //更新 背包 、仓库、装备栏物品的属性
            ///背包中的物品
            if (KnapsackItemsManager.KnapsackItems.TryGetValue(message.ItemUUID, out KnapsackDataItem dataItem))
            {
                UpdateAtr();
            }
            ///仓库中的物品
            else if (KnapsackItemsManager.WareHouseItems.TryGetValue(message.ItemUUID, out dataItem))
            {
                UpdateAtr();
            }
            //更新 交易面板的属性
            else if (UIKnapsackComponent.Instance != null && UIKnapsackComponent.Instance.OtherTradeItemDic.TryGetValue(message.ItemUUID, out dataItem))
            {
                UpdateAtr();
            }
            else
            {
                ///装备栏的物品
                List<KnapsackDataItem> CurWareEquip = UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic.Values.ToList();
                if (CurWareEquip.Exists(r => r.UUID == message.ItemUUID))
                {
                    dataItem = CurWareEquip.Find(r => r.UUID == message.ItemUUID);
                    UpdateAtr();
                }

                UI ui = UIComponent.Instance.Get(UIType.UIKnapsackNew);
                if (ui != null)
                {
                    UIKnapsackNewComponent uIKnapsackNew = ui.GetComponent<UIKnapsackNewComponent>();
                    //更新 交易面板的属性
                    if (uIKnapsackNew != null && uIKnapsackNew.OtherTradeItemDic.TryGetValue(message.ItemUUID, out dataItem))
                    {
                        UpdateAtr();
                    }
                }

            }

            //更新摊位上的物品属性
           
            if (message.Scene == E_ItemPropertyNotice.StallUp.ToInt32() && UnitEntityComponent.Instance.Get<RoleEntity>(message.GameUserId) is RoleEntity roleEntity)
            {
                var stallUpComponent = roleEntity.GetComponent<RoleStallUpComponent>();
                dataItem = stallUpComponent.GetKnapsackDataItem(message.ItemUUID);
                if (dataItem == null)
                {
                    dataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(message.ItemUUID);
                    stallUpComponent.StallUpItemDic[message.ItemUUID] = dataItem;
                }
                UpdateAtr();


            }
            //更新属性
            void UpdateAtr()
            {
                UpdateExecllentEntry();
                UpdateExtraEntry();
                UpdateSpecialEntry();
            }
            ///更新卓越属性
            void UpdateExecllentEntry()
            {
                dataItem.ExecllentEntryDic.Clear();
                for (int i = 0, length = message.ExecllentEntry.Count; i < length; i++)
                {
                    var item = message.ExecllentEntry[i];
                    dataItem.ExecllentEntryDic[item.PropId] = item.Level;
                }
            }

            //更新套装附带的额外属性
            void UpdateExtraEntry() 
            {
                dataItem.ExtraEntryDic.Clear();
                for (int i = 0, length = message.ExtraEntry.Count; i < length; i++)
                {
                 
                    var item = message.ExtraEntry[i];
                    dataItem.ExtraEntryDic[item.PropId] = item.Level;
                }
            }

            ///更新特殊属性
            void UpdateSpecialEntry()
            {
                dataItem.SpecialEntryDic.Clear();
                for (int i = 0, length = message.SpecialEntry.Count; i < length; i++)
                {
                    var item = message.SpecialEntry[i];
                    dataItem.SpecialEntryDic[item.PropId] = item.Level;
                }
            }
        }
    }
}