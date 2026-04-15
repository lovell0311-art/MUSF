using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIFuBenPaiHangBang)]
    public class UIFuBenPaiHangBangComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIFuBenPaiHangBang.StringToAB(), UIType.UIFuBenPaiHangBang);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIFuBenPaiHangBang, panel, false);
            ui.AddComponent<UIFuBenPaiHangBangComponent>();
            return ui;
        }
    }
}