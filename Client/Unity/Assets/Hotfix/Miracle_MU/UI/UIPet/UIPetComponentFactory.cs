using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIPet)]
    public class UIPetComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIPet.StringToAB(), UIType.UIPet);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIPet, panel);
            ui.AddComponent<UIPetComponent>();
            return ui;
        }
    }

}
