using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIZaiXian)]
    public class UIZaiXianFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIZaiXian.StringToAB(), UIType.UIZaiXian);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIZaiXian, panel, false);
            ui.AddComponent<UIZaiXianComponent>();
            return ui;
        }
    }
}
