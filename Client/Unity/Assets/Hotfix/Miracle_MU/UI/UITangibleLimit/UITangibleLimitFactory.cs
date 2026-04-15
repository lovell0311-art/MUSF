using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UITangibleLimit)]
    public class UITangibleLimitFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UITangibleLimit.StringToAB(), UIType.UITangibleLimit);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UITangibleLimit, panel, false);
            ui.AddComponent<UITangibleLimitComponent>();
            return ui;
        }

    }
}

