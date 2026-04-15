using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIYaoQingMa)]
    public class UIYaoQingMaFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIYaoQingMa.StringToAB(), UIType.UIYaoQingMa);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIYaoQingMa, panel, false);
            ui.AddComponent<UIYaoQingMaComponent>();
            return ui;
        }
    }
}
