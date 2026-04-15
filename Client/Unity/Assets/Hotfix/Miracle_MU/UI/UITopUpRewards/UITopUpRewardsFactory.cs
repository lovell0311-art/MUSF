using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UITopUpRewards)]
    public class UITopUpRewardsFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UITopUpRewards.StringToAB(), UIType.UITopUpRewards);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UITopUpRewards, panel, false);
            ui.AddComponent<UITopUpRewardsComponent>();
            return ui;
        }
    }
}