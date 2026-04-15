using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEditor.Graphs;

namespace ETHotfix
{
    /// <summary>
    /// 推送实体卸下装备(周边一定范围玩家也广播)
    /// </summary>
    [MessageHandler]
    public class G2C_UnitEquipUnload_notice_Handler : AMHandler<G2C_UnitEquipUnload_notice>
    {
        protected override void Run(ETModel.Session session, G2C_UnitEquipUnload_notice message)
        {
            //Log.DebugYellow($"推送实体卸载装备；本地玩家：{message.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID}  {message.EquipPosition}");
            //移除 玩家的装备
            if (UnitEntityComponent.Instance.RoleEntityDic.TryGetValue(message.GameUserId, out RoleEntity roleEntity))
            {
              
                roleEntity.GetComponent<RoleEquipmentComponent>()?.RemoveCacheEquipment((E_Grid_Type)message.EquipPosition);
                roleEntity.GetComponent<RoleEquipmentComponent>()?.UnLoadEquipment(message.EquipPosition);
                
                //刷新玩家装备
              
                UnitEntityComponent.Instance.SetUnitObjState(GlobalDataManager.IsHideRole, roleEntity);
            }

            //守护销毁
            if ((E_Grid_Type)message.EquipPosition == E_Grid_Type.Guard && UIComponent.Instance.Get(UIType.UIKnapsack) == null&&message.GameUserId==UnitEntityComponent.Instance.LocaRoleUUID)
            {
              //  UIComponent.Instance.VisibleUI(UIType.UIHint,"守护已销毁");
            }
        }
    }
}
