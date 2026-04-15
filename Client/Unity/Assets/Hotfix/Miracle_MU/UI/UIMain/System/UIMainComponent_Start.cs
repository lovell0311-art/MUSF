using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIMainComponent_Start : StartSystem<UIMainComponent>
    {
        public override void Start(UIMainComponent self)
        {
          //  self.ChangeRolePosTxt(AstarComponent.Instance.GetNodeVector(self.roleEntity.Position.x, self.roleEntity.Position.z));
           
            self.isShow = true;

            if (self.roleEntity.Property.GetProperValue(E_GameProperty.PkNumber) > 0)
            {
                self.PkNumber();
            }
            self.Init_SceneName();

            self.InitBossHp();

            //获取宠物信息
            self.GetWarPetData().Coroutine();
            ///获取玩家的充值信息
            TimerComponent.Instance.RegisterTimeCallBack(5000, () => { self.GetCurTopupInfo().Coroutine(); });
             
            //显示玩家的ID 和区服信息
            self.roleId=self.ReferenceCollector_Main.GetText("roleID");
            self.roleId.text = $"{self.roleEntity.Id}_{GlobalDataManager.EnterZoneID}_{GlobalDataManager.EnterLineID}";
            
        }
    }
}