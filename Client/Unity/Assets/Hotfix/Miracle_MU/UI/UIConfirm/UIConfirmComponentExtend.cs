using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 提示 确认面板拓展类
    /// </summary>
    public static class UIConfirmComponentExtend 
    {
        public static UIConfirmComponent GetUIConfirmComponent(E_TipPanelType tipPanelType=E_TipPanelType.NorMal,string tiptype=null) 
        {
            UIComponent.Instance.VisibleUI(UIType.UIConfirm,$"{tiptype}");
            UIConfirmComponent uIConfirmComponent = UIComponent.Instance.Get(UIType.UIConfirm).GetComponent<UIConfirmComponent>();
            uIConfirmComponent.ChangePanel(tipPanelType);
            return uIConfirmComponent;
        }
    }
}
