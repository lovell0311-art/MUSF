using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIWelfare)]
    public class UIWelfareFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIWelfare.StringToAB(), UIType.UIWelfare);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIWelfare, panel, false);
            ui.AddComponent<UIWelfareComponent>();
            return ui;
        }

    }
}

