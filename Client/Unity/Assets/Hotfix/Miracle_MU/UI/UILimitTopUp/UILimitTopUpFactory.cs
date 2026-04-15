using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UILimitTopUp)]
    public class UILimitTopUpFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UILimitTopUp.StringToAB(),UIType.UILimitTopUp);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UILimitTopUp,panel,false);
            ui.AddComponent<UILimitTopUpComponent>();
            return ui;
        }
    }
}