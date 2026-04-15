using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIChaXun)]
    public class UIChaXunFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIChaXun.StringToAB(),UIType.UIChaXun);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIChaXun,panel,false);
            ui.AddComponent<UIChaXunComponent>();
            return ui;
        }

       
    }
}
