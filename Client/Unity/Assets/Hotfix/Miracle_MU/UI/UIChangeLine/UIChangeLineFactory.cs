using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIChangeLine)]
    public class UIChangeLineFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIChangeLine.StringToAB(), UIType.UIChangeLine);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIChangeLine, panel,false);
            ui.AddComponent<UIChangeLineComponent>();
            return ui;
        }

    }
}
