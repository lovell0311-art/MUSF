using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UIHint)]
    public class UIHintComponentFactory : IUIFactory
    {
        public UI Create()
        {

            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIHint.StringToAB(),UIType.UIHint);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIHint,panel);
            //给实体增加组件
            ui.AddComponent<UIHintComponent>();
            return ui;
        }

      
    }
}