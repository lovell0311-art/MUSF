using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UITreasureHouse)]
    public class UITreasureHouseFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UITreasureHouse.StringToAB(), UIType.UITreasureHouse);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UITreasureHouse, panel, false);
            ui.AddComponent<UITreasureHouseComponent>();
            return ui;
        }
    }
}