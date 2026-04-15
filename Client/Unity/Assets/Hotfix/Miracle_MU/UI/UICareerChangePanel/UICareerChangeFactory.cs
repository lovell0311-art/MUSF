using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UICareerChangePanel)]
    public class UICareerChangeFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UICareerChangePanel.StringToAB(),UIType.UICareerChangePanel);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UICareerChangePanel,panel,false);
            ui.AddComponent<UICareerChangeComponent>();
            return ui;
        }
    }
}
