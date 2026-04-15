using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UILogin_XYSDK)]
    public class UILogin_XySdkFactory : IUIFactory
    {
        public UI Create()
        {
            LoginStageTrace.Append("UILogin_XySdkFactory.Create redirect start");
            // 当前安卓包统一走普通账号密码登录，避免旧 XYSDK 协议页挡住登录流程。
            GameObject gameObject = ResourcesComponent.Instance.LoadGameObject(UIType.UILogin.StringToAB(), UIType.UILogin);
            LoginStageTrace.Append($"UILogin_XySdkFactory.Create prefabNull={gameObject == null}");
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UILogin_XYSDK, gameObject, false);
            ui.AddComponent<UILoginComponent>();
            LoginStageTrace.Append("UILogin_XySdkFactory.Create redirect finish");
            return ui;
        }
    }
}
