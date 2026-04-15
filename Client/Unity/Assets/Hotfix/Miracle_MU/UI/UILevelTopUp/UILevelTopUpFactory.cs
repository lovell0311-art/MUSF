using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UILevelTopUp)]
    public class UILevelTopUpFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject gameObject = ResourcesComponent.Instance.LoadGameObject(UIType.UILevelTopUp.StringToAB(), UIType.UILevelTopUp);
            //创建实体,并且内部调用了Awake方法
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UILevelTopUp, gameObject, false);
            //给实体增加组件
            //添加 登录组件
            ui.AddComponent<UILevelTopUpComponent>();
            return ui;
        }
    }
}