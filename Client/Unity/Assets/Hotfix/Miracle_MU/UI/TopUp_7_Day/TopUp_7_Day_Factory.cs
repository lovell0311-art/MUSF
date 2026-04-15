using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.TopUp_7_Day)]
    public class TopUp_7_Day_Factory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.TopUp_7_Day.StringToAB(), UIType.TopUp_7_Day);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.TopUp_7_Day,panel,false);
            ui.AddComponent<TopUp_7_DayComponent>();
            return ui;
        }

    }
}