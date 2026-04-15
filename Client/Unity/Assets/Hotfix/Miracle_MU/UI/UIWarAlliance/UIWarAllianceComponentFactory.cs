using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIWarAlliance)]
    public class UIWarAllianceComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIWarAlliance.StringToAB(),UIType.UIWarAlliance);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIWarAlliance,panel,false);
            ui.AddComponent<UIWarAllianceComponent>();
            return ui;
        }
    }
}
