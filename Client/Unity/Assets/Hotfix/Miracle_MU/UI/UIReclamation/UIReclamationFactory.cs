using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIReclamation)]
    public class UIReclamationFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIReclamation.StringToAB(), UIType.UIReclamation);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIReclamation, panel, false);
            ui.AddComponent<UIReclamationComponent>();
            return ui;
        }
    }
}
