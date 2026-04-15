using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIRealName)]
    public class UIRealNameComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIRealName.StringToAB(),UIType.UIRealName);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIRealName,panel,false);
            ui.AddComponent<UIRealNameComponent>();
            return ui;
        }
    }

}