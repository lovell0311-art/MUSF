using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIConfirm)]
    public class UIConfirmFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIConfirm.StringToAB(), UIType.UIConfirm);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIConfirm, panel);
            ui.AddComponent<UIConfirmComponent>();
            return ui;
        }
    }
}