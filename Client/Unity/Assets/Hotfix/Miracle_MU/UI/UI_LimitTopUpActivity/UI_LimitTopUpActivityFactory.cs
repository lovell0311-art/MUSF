using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UI_LimitTopUpActivity)]
    public class UI_LimitTopUpActivityFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UI_LimitTopUpActivity.StringToAB(),UIType.UI_LimitTopUpActivity);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UI_LimitTopUpActivity,panel,false);
            ui.AddComponent<UI_LimitTopUpActivityComponent>();
            return ui;
        }

     
    }
}