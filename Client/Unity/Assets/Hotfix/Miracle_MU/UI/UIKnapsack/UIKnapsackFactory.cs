using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UIKnapsack)]
    public class UIKnapsackFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIKnapsack.StringToAB(),UIType.UIKnapsack);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIKnapsack,panel,false);
            ui.AddComponent<UIKnapsackComponent>();
            return ui;
        }

    
    }
}
