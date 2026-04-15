using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UITitle)]
    public class UITitleComponenetFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UITitle.StringToAB(), UIType.UITitle);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UITitle, panel, false);
            ui.AddComponent<UITitleComponnet>();
            return ui;
        }
    }
}