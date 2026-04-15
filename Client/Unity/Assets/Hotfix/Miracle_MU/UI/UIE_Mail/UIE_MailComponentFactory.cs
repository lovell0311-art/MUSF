using ETHotfix;
using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIE_Mail)]
    public class UIE_MailComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIE_Mail.StringToAB(), UIType.UIE_Mail);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIE_Mail, panel, false);
            ui.AddComponent<UIE_MailComponent>();

            return ui;
        }
    }
}