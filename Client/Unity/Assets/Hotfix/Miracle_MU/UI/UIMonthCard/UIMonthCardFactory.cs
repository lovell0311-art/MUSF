using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIMonthCard)]
    public class UIMonthCardFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIMonthCard.StringToAB(), UIType.UIMonthCard);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIMonthCard, panel, false);
            ui.AddComponent<UIMonthCardComponent>();
            return ui;
        }

    }
}

