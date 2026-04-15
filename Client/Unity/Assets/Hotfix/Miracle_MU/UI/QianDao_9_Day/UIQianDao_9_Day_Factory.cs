using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIQianDao_9_Day)]
    public class UIQianDao_9_Day_Factory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIQianDao_9_Day.StringToAB(), UIType.UIQianDao_9_Day);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIQianDao_9_Day, panel,false);
            ui.AddComponent<UIQianDao_9_DayComponent>();
            return ui;
        }

    }
}