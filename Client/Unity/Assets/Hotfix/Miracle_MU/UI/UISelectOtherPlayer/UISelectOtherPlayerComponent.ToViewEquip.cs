using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

namespace ETHotfix
{
    /// <summary>
    /// 查看玩家 装备模块
    /// </summary>
    public partial class UISelectOtherPlayerComponent
    {
        public RoleEquipmentComponent equipmentComponent;
        private readonly Dictionary<E_Grid_Type, Transform> ViewEquipDic = new Dictionary<E_Grid_Type, Transform>();
        private readonly Dictionary<E_Grid_Type, GameObject> curShowEquipDic = new Dictionary<E_Grid_Type, GameObject>();//当前显示的装备

        public GameObject EquipMentPanel = null;//装备面板
        public void Init_() 
        {
            EquipMentPanel = collector.GetImage("EquipPanel").gameObject;
            ReferenceCollector reference= EquipMentPanel.GetReferenceCollector();
            reference.GetButton("CloseBtn_1").onClick.AddSingleListener(ClearViewRoleEquips);
            Transform EquipGrid = reference.GetGameObject("EquipGrid").transform;
            for (int i = 0, length=EquipGrid.childCount; i < length; i++)
            {
                GameObject ob = EquipGrid.GetChild(i).gameObject;
                ViewEquipDic[(E_Grid_Type)int.Parse(ob.name)] =ob.transform;
            }
            EquipMentPanel.SetActive(false);

        }
        //显示该玩家当前穿戴的装备 
        public void ToViewRoleEquips() 
        {
            equipmentComponent ??=SelectroleEntity?.GetComponent<RoleEquipmentComponent>();
            for (int i = 0,length=equipmentComponent.curWareEquipsData_Dic.Count; i < length; i++)
            {
               
                KnapsackDataItem item = equipmentComponent.curWareEquipsData_Dic.ElementAt(i).Value;
                E_Grid_Type grid_Typt = equipmentComponent.curWareEquipsData_Dic.ElementAt(i).Key;
                if (grid_Typt==E_Grid_Type.Mounts) 
                {
                    continue;
                }
                item.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                Log.DebugBrown("资源名" + item_Info.ResName+":::部位"+ item.ConfigId);
                if (item.ConfigId==0)
                {
                    break;
                }
                GameObject obj = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
                obj.transform.position = new Vector3(ViewEquipDic[grid_Typt].position.x, ViewEquipDic[(E_Grid_Type)item_Info.Slot].position.y, 85);
                obj.SetUI(item.GetProperValue(E_ItemValue.Level));
                curShowEquipDic[grid_Typt] = obj;
                ViewEquipDic[grid_Typt].GetComponent<Image>().color = Color.green;
                //点击事件
                UGUITriggerProxy proxy = ViewEquipDic[grid_Typt].gameObject.GetComponent<UGUITriggerProxy>() ?? ViewEquipDic[grid_Typt].gameObject.AddComponent<UGUITriggerProxy>();
                proxy.OnPointerClickEvent += () => { OnPointerClickEvent((int)grid_Typt, equipmentComponent.roleEntity.Id); };
            }
            EquipMentPanel.SetActive(true);
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
            equipmentComponent = null;
            EquipMentPanel.SetActive(false);
            UIComponent.Instance.Remove(UIType.UIIntroduction);
        }
        //点击事件
        public void OnPointerClickEvent(int equipPos,long userId)
        {
            GetEquipDataAsync().Coroutine();
            async ETVoid GetEquipDataAsync()
            {
                C2G_GetEquipItemAllPropResponse g2C_GetEquipItemAllPropResponse = (C2G_GetEquipItemAllPropResponse)await SessionComponent.Instance.Session.Call(new C2G_GetEquipItemAllPropRequest
                {
                    GameUserId = userId,
                    EquipPosition = equipPos
                });
                if (g2C_GetEquipItemAllPropResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetEquipItemAllPropResponse.Error.GetTipInfo());
                    //  Log.DebugGreen($"g2C_ItemsSynthesis.Error：{g2C_ItemsSynthesis.Error}");
                }
                else {
                    //g2C_GetEquipItemAllPropResponse.AllProperty;
                    if (equipmentComponent.curWareEquipsData_Dic.TryGetValue((E_Grid_Type)equipPos, out KnapsackDataItem knapsackDataItem))
                    {
                        foreach (var item in g2C_GetEquipItemAllPropResponse.AllProperty.PropList)
                        {
                            knapsackDataItem.Set(item);
                        } 
                        foreach (var exitem in g2C_GetEquipItemAllPropResponse.AllProperty.ExecllentEntry)
                        {
                            knapsackDataItem.SetExecllentEntry(exitem);
                        }
                        foreach (var spitem in g2C_GetEquipItemAllPropResponse.AllProperty.SpecialEntry)
                        {
                            knapsackDataItem.SetSpecialEntry(spitem);
                        }
                        knapsackDataItem.ExtraEntryDic.Clear();
                        foreach (var Extra in g2C_GetEquipItemAllPropResponse.AllProperty.ExtraEntry)
                        {
                            knapsackDataItem.ExtraEntryDic[Extra.PropId] = Extra.Level; 
                        }
                    }
                   
                    UIIntroductionComponent uIIntroduction = UIComponent.Instance.VisibleUI(UIType.UIIntroduction).GetComponent<UIIntroductionComponent>();
                    uIIntroduction.GetAllAtrs(knapsackDataItem);
                    uIIntroduction.ShowAtrs();
                    Transform img = ViewEquipDic[(E_Grid_Type)equipPos];
                    Vector3 pos = new Vector3(img.position.x, img.position.y, 0);
                    pos -= Vector3.left / 2;
                    var screenPos = CameraComponent.Instance.UICamera.WorldToScreenPoint(pos);
                    var pivot_x = 0;
                    uIIntroduction.ChangeStartCorner(Corner.UpperLeft);
                    if (screenPos.x > Screen.width - 700)
                    {

                        pivot_x = 1;
                        pos += Vector3.left;
                        uIIntroduction.ChangeStartCorner();
                    }
                    uIIntroduction.SetPos(pos, pivot_x);
                }
            }
        }      
    }
}
