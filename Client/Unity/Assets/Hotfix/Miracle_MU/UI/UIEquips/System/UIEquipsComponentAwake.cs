using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIEquipsComponentAwake : AwakeSystem<UIEquipsComponent>
    {
        public override void Awake(UIEquipsComponent self)
        {
            ReferenceCollector reference = self.GetParent<UI>().GameObject.GetReferenceCollector();
            reference.GetButton("CloseBtn_1").onClick.AddSingleListener(self.ClearViewRoleEquips);
            Transform EquipGrid = reference.GetGameObject("EquipGrid").transform;
            for (int i = 0, length = EquipGrid.childCount; i < length; i++)
            {
                GameObject ob = EquipGrid.GetChild(i).gameObject;
                self.ViewEquipDic[(E_Grid_Type)int.Parse(ob.name)] = ob.transform;
            }
        }
    }
}
