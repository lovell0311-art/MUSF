using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIKnapsackNew)]
    public class UIKnapsackNewFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIKnapsackNew.StringToAB(), UIType.UIKnapsackNew);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIKnapsackNew, panel, false);
            ui.AddComponent<UIKnapsackNewComponent>();
            return ui;
        }
    }
}
