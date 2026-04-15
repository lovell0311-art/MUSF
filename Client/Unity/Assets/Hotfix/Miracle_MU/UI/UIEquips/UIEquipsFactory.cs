using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIEquips)]
    public class UIEquipsFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIEquips.StringToAB(),UIType.UIEquips);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIEquips,panel,false);
            ui.AddComponent<UIEquipsComponent>();
            return ui;
        }

       
    }
}