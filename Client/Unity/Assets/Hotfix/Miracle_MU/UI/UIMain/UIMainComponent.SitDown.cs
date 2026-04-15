using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIMainComponent
    {
        public Button sitDownBtn;
        public void SitDownInit()
        {
            sitDownBtn = ReferenceCollector_Main.GetButton("SitDownBtn");
            sitDownBtn.onClick.AddSingleListener( async() =>
            {
                if (roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Mounts))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请先卸下坐骑");
                    return;
                }
                G2C_TakeAThroneResponse g2C_TakeAThrone = (G2C_TakeAThroneResponse)await SessionComponent.Instance.Session.Call(new C2G_TakeAThroneRequest() { });
                if (g2C_TakeAThrone.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_TakeAThrone.Error.GetTipInfo());
                }
                else
                {
                    SetSitDownBtnShow(false);
                }
            });
            SetSitDownBtnShow(false);
        }
        /// <summary>
        /// 设置坐下按钮是否显示
        /// </summary>
        /// <param name="show"></param>
        public void SetSitDownBtnShow(bool show)
        {
            sitDownBtn.gameObject.SetActive(show);
        }

    }
}
