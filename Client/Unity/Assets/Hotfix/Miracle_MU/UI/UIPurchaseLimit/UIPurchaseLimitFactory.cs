using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIPurchaseLimit)]
    public class UIPurchaseLimitFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIPurchaseLimit.StringToAB(), UIType.UIPurchaseLimit);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIPurchaseLimit, panel, false);
            ui.AddComponent<UIPurchaseLimitComponent>();
            return ui;
        }

    }
}

