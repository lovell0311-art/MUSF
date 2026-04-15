using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UITask)]
    public class UITaskComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UITask.StringToAB(), UIType.UITask);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UITask, panel, false);
            ui.AddComponent<UITaskComponent>();
            return ui;
        }
    }
}