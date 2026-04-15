using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIDaShiNew)]
    public class UIDaShiNewFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIDaShiNew.StringToAB(), UIType.UIDaShiNew);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIDaShiNew, panel, false);
            ui.AddComponent<UIDaShiNewComponent>();
            return ui;
            
        }

    }
}

