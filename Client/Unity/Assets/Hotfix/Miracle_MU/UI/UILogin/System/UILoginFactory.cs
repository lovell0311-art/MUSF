using System;
using ETModel;
using UnityEngine;


namespace ETHotfix
{
    [UIFactory(UIType.UILogin)]
    public class UILoginFactory : IUIFactory
    {
        public UI Create()
        {
            LoginStageTrace.Append("UILoginFactory.Create start");
            GameObject gameObject = ResourcesComponent.Instance.LoadGameObject(UIType.UILogin.StringToAB(),UIType.UILogin);
            LoginStageTrace.Append($"UILoginFactory.Create prefabNull={gameObject == null}");
            //创建实体,并且内部调用了Awake方法
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UILogin, gameObject,false);
            //给实体增加组件
            //添加 登录组件
            ui.AddComponent<UILoginComponent>();
            GuideComponent.Instance.InitConfig();
            LoginStageTrace.Append("UILoginFactory.Create finish");

            return ui;
        }
    }
}
