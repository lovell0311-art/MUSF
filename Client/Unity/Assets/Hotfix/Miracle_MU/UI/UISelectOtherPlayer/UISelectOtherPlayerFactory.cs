using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UISelectOtherPlayer)]
    public class UISelectOtherPlayerFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UISelectOtherPlayer.StringToAB(),UIType.UISelectOtherPlayer);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UISelectOtherPlayer,panel,false);
            ui.AddComponent<UISelectOtherPlayerComponent>();
            return ui;
        }

      
    }
}