using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIAwakening)]
    public class UIAwakeningFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIAwakening.StringToAB(), UIType.UIAwakening);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIAwakening, panel, false);
            ui.AddComponent<UIAwakeningComponent>();
            return ui;
        }

    }
}

