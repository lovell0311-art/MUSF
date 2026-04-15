using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UITeam)]
    public class UITeamComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UITeam.StringToAB(),UIType.UITeam);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UITeam,panel);
            ui.AddComponent<UITeamComponent>();
            return ui;
        }

       
    }
}