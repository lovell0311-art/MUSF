using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UISynthesis)]
    public class UISynthesisFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panle = ResourcesComponent.Instance.LoadGameObject(UIType.UISynthesis.StringToAB(), UIType.UISynthesis);
            UI uI = ComponentFactory.Create<UI, string, GameObject>(UIType.UISynthesis, panle, false);
            uI.AddComponent<UISynthesisComponent>();
            return uI;
        }


    }

}
