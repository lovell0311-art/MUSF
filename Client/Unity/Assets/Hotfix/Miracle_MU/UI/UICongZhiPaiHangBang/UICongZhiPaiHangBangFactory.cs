using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UICongZhiPaiHangBang)]
    public class UICongZhiPaiHangBangFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject gameObject = ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadGameObject(UIType.UICongZhiPaiHangBang.StringToAB(), UIType.UICongZhiPaiHangBang);
            UI uI = ComponentFactory.Create<UI, string, GameObject>(UIType.UICongZhiPaiHangBang, gameObject, false);
            uI.AddComponent<UICongZhiPaiHangBangComponent>();
            return uI;
        }
    }
}
