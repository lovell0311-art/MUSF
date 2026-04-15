using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIFirstCharge)]
    public class UIFirstChargeComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIFirstCharge.StringToAB(),UIType.UIFirstCharge);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIFirstCharge,panel,false);
            ui.AddComponent<UIFirstChargeComponent>();
            return ui;
        }
    }
}
