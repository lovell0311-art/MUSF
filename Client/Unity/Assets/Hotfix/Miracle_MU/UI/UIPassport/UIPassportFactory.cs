using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIPassport)]
    public class UIPassportFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIPassport.StringToAB(), UIType.UIPassport);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIPassport, panel, false);
            ui.AddComponent<UIPassportComponent>();
            return ui;
        }
    }
}
