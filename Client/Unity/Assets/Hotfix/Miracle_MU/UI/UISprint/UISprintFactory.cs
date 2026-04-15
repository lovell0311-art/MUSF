using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UISprint)]
    public class UISprintFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panle = ResourcesComponent.Instance.LoadGameObject(UIType.UISprint.StringToAB(), UIType.UISprint);
            UI uI = ComponentFactory.Create<UI, string, GameObject>(UIType.UISprint, panle, false);
            uI.AddComponent<UISprintComponent>();
            return uI;
        }


    }

}
