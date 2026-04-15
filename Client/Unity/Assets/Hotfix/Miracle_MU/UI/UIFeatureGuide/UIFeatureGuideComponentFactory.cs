using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIFeatureGuide)]
    public class UIFeatureGuideComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIFeatureGuide.StringToAB(), UIType.UIFeatureGuide);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIFeatureGuide, panel, false);
            ui.AddComponent<UIFeatureGuideComponent>();
            return ui;
        }
    }
}
