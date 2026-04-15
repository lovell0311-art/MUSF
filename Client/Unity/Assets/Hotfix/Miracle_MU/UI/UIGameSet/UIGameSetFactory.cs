using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UIGameSet)]
    public class UIGameSetFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIGameSet.StringToAB(),UIType.UIGameSet);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIGameSet,panel,false);
            ui.AddComponent<UIGameSetComponent>();
            return ui;
        }

    }
}
