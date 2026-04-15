using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIShop)]
    public class UIShopComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIShop.StringToAB(),UIType.UIShop);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIShop,panel,false);
            ui.AddComponent<UIShopComponent>();
            return ui;
        }

       
    }
}
