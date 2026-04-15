using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIActiveInfo)]
    public class UIActiveInfoFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panle = ResourcesComponent.Instance.LoadGameObject(UIType.UIActiveInfo.StringToAB(), UIType.UIActiveInfo);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIActiveInfo, panle);
            ui.AddComponent<UIActiveInfoComponent>();
            return ui;
        }
    }

}
