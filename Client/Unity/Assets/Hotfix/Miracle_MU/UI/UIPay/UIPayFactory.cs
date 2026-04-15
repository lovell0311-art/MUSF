using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIPay)]
    public class UIPayFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIPay.StringToAB(),UIType.UIPay);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIPay,panel,false);
            ui.AddComponent<UIPayComponent>();
            return ui;

        }
    }
}
