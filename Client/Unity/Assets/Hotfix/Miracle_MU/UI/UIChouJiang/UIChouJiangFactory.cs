using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIChouJiang)]
    public class UIChouJiangFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIChouJiang.StringToAB(),UIType.UIChouJiang);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIChouJiang,panel,false);
            ui.AddComponent<UIChouJiangComponent>();
            return ui;
        }

    }
}
