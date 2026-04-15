using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIFuBen)]
    public class UIFuBenComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIFuBen.StringToAB(),UIType.UIFuBen);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIFuBen,panel,false);
            ui.AddComponent<UIFuBenComponent>();
            return ui;
        }
    }
}
