using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UITreasureMap)]
    public class UITreasureMapFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UITreasureMap.StringToAB(),UIType.UITreasureMap);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UITreasureMap,panel,false);
            ui.AddComponent<UITreasureMapComponent>();
            return ui;
        }
    }
}
