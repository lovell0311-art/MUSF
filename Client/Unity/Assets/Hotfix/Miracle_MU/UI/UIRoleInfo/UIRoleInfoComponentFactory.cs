using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIRoleInfo)]
    public class UIRoleInfoComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIRoleInfo.StringToAB(), UIType.UIRoleInfo);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIRoleInfo, panel, false);
            ui.AddComponent<UIRoleInfoComponent>();

            return ui;
        }


    }
}
