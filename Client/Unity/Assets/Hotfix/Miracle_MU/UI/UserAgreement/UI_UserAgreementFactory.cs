using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UI_UserAgreement)]
    public class UI_UserAgreementFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UI_UserAgreement.StringToAB(),UIType.UI_UserAgreement);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UI_UserAgreement,panel,false);
            ui.AddComponent<UI_UserAgreementComponent>();
            return ui;
        }

       
    }
}
