using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIZhuanSheng)]
    public class UIZhuanShengFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIZhuanSheng.StringToAB(), UIType.UIZhuanSheng);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIZhuanSheng, panel, false);
            ui.AddComponent<UIZhuanShengComponent>();
            return ui;
        }
    }
}
