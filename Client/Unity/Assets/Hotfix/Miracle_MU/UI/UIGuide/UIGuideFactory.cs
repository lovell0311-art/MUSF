using ETHotfix;
using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETHotfix
{
    [UIFactory(UIType.UIGuide)]
    public class UIGuideFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject gameObject = ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadGameObject(UIType.UIGuide.StringToAB(), UIType.UIGuide);
            UI uI = ComponentFactory.Create<UI, string, GameObject>(UIType.UIGuide, gameObject, false);
            uI.AddComponent<UIGuideComponent>();
            return uI;
        }
    }
}