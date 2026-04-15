using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIAddFirendConfirm)]
    public class UIAddFirendConfirmFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIAddFirendConfirm.StringToAB(), UIType.UIAddFirendConfirm);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIAddFirendConfirm, panel);
            ui.AddComponent<UIAddFirendConfirmComponent>();
            return ui;
        }
    }
}

