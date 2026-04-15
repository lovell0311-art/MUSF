using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;
using UnityEngine.UI;

namespace ETHotfix
{

    /// <summary>
    /// 玩家装备面板
    /// </summary>
    public class UIEquipsComponent : Component
    {

        public readonly Dictionary<E_Grid_Type, Transform> ViewEquipDic = new Dictionary<E_Grid_Type, Transform>();
        public readonly Dictionary<E_Grid_Type, GameObject> curShowEquipDic = new Dictionary<E_Grid_Type, GameObject>();//当前显示的装备

        ///显示玩家的装备 
        public void ToViewRoleEquips(RoleEntity roleEntity)
        {
            RoleEquipmentComponent equipmentComponent=roleEntity?.GetComponent<RoleEquipmentComponent>();
            for (int i = 0, length = equipmentComponent.curWareEquipsData_Dic.Count; i < length; i++)
            {
                KnapsackDataItem item = equipmentComponent.curWareEquipsData_Dic.ElementAt(i).Value;
                item.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                GameObject obj = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
                obj.transform.position = new Vector3(ViewEquipDic[(E_Grid_Type)item.Slot].position.x, ViewEquipDic[(E_Grid_Type)item.Slot].position.y, 10);
                obj.SetUI(item.GetProperValue(E_ItemValue.Level));
                curShowEquipDic[(E_Grid_Type)item.Slot] = obj;
                ViewEquipDic[(E_Grid_Type)item.Slot].GetComponent<Image>().color = Color.green;
            }
        }
        public void ToViewRoleEquips(long equipId,int lev)
        {
            equipId.GetItemInfo_Out(out Item_infoConfig item_Info);
            GameObject obj = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
            obj.transform.position = new Vector3(ViewEquipDic[(E_Grid_Type)item_Info.Slot].position.x, ViewEquipDic[(E_Grid_Type)item_Info.Slot].position.y, 10);
            obj.SetUI(lev);
            curShowEquipDic[(E_Grid_Type)item_Info.Slot] = obj;
            ViewEquipDic[(E_Grid_Type)item_Info.Slot].GetComponent<Image>().color = Color.green;
        }


        //清理 当前显示的装备
        public void ClearViewRoleEquips()
        {
            for (int i = 0, length = curShowEquipDic.Count; i < length; i++)
            {
               // ResourcesComponent.Instance.RecycleGameObject(curShowEquipDic.ElementAt(i).Value);
                ResourcesComponent.Instance.DestoryGameObjectImmediate(curShowEquipDic.ElementAt(i).Value, curShowEquipDic.ElementAt(i).Value.name.StringToAB());
            }
            curShowEquipDic.Clear();
            UIComponent.Instance.Remove(UIType.UIEquips);
        }
    }
}
