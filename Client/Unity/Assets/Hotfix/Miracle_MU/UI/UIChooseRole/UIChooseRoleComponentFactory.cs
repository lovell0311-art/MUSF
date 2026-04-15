using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UIChooseRole)]
    public class UIChooseRoleComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject gameObject = ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadGameObject(UIType.UIChooseRole.StringToAB(),UIType.UIChooseRole);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIChooseRole,gameObject,false);
            ui.AddComponent<UIChooseRoleComponent>();
            ui.AddComponent<RenderTextureComponent>();
            return ui;
        }

      
    }
}
