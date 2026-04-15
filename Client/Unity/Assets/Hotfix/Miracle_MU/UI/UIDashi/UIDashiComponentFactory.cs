using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.DaShiCanvas)]
    public class UIDashiComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.DaShiCanvas.StringToAB(), UIType.DaShiCanvas);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.DaShiCanvas, panel, false);
            ui.AddComponent<UIDashiComponent>();

            return ui;
        }
    }

}
