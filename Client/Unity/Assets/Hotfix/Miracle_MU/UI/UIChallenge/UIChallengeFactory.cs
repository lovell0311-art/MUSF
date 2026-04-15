
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIChallenge)]
    public class UIChallengeFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIChallenge.StringToAB(), UIType.UIChallenge);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIChallenge, panel, false);
            ui.AddComponent<RenderTextureComponent>();
            ui.AddComponent<UIChallengeComponent>();
            return ui;
        }

    }
}
