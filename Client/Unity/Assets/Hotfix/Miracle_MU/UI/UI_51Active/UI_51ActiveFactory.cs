using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UI_51Active)]
    public class UI_51ActiveFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UI_51Active.StringToAB(),UIType.UI_51Active);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UI_51Active,panel,false);
            ui.AddComponent<UI_51ActiveComponent>();
            return ui;
        }

      
    }
}
