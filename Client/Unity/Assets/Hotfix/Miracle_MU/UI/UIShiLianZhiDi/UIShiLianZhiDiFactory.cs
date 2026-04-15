using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIShiLianZhiDi)]
    public class UIShiLianZhiDiFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIShiLianZhiDi.StringToAB(),UIType.UIShiLianZhiDi);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIShiLianZhiDi,panel,false);
            ui.AddComponent<UIShiLianZhiDiComponent>();
            return ui;
        }
    }

}