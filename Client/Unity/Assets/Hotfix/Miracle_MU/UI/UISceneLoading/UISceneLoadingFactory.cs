using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UISceneLoading)]
    public class UISceneLoadingFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject gameObject =ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadGameObject(UIType.UISceneLoading.StringToAB(),UIType.UISceneLoading);
            UI uI = ComponentFactory.Create<UI, string, GameObject>(UIType.UISceneLoading,gameObject,false);
            uI.AddComponent<UISceneLoadingComponent>();
            return uI;
        }
    }
}