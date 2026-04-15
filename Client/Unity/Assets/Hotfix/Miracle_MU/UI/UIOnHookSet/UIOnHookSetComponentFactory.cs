using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UIOnHookSet)]
    public class UIOnHookSetComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIOnHookSet.StringToAB(),UIType.UIOnHookSet);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIOnHookSet,panel);
            ui.AddComponent<UIOnHookSetComponent>();
            return ui;
        }

       
    }
}