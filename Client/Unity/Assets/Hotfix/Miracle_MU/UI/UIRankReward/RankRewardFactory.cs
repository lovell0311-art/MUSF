using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UI_RankReward)]
    public class RankRewardFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UI_RankReward.StringToAB(),UIType.UI_RankReward);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UI_RankReward,panel,false);
            ui.AddComponent<RankRewardComponent>();
            return ui;
        }
    }

}