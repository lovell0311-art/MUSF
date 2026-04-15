using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UIIntroduction)]
    public class UIIntroductionFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIIntroduction.StringToAB(),UIType.UIIntroduction);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIIntroduction,panel);
            ui.AddComponent<UIIntroductionComponent>();
            return ui;
        }
    }
}
