using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIMount)]
    public class UIMountComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIMount.StringToAB(), UIType.UIMount);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIMount, panel);
            ui.AddComponent<UIMountComponent>();
            return ui;
        }
    }

}
