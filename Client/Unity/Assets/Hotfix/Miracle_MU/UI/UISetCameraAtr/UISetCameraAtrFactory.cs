using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UISetCameraAtr)]
    public class UISetCameraAtrFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UISetCameraAtr.StringToAB(),UIType.UISetCameraAtr);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UISetCameraAtr,panel,false);
            ui.AddComponent<UISetCameraAtrComponent>();
            return ui;
        }

    }
}
