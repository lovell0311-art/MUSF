using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIChatPanel)]
    public class UIChatPanelFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIChatPanel.StringToAB(),UIType.UIChatPanel);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIChatPanel,panel,false);
            ui.AddComponent<UIChatPanelComponent>();
            return ui;
        }

       
    }
}