using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UINewYearActivity)]
    public class UINewYearActivityComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UINewYearActivity.StringToAB(), UIType.UINewYearActivity);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UINewYearActivity, panel);
            ui.AddComponent<UINewYearActivityComponent>();
            return ui;
        }


    }
}