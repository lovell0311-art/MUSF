using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UI_HUD)]
    public class HUDComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UI_HUD.StringToAB(),UIType.UI_HUD);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UI_HUD,panel,false);
            ui.AddComponent<HUDComponent>();
            return ui;
        }
    }
}
