using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    /// <summary>
    /// 
    /// </summary
    [UIFactory(UIType.UI51GoldCard)]
    public class UI51GoldCardFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UI51GoldCard.StringToAB(),UIType.UI51GoldCard);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UI51GoldCard,panel,false);
            ui.AddComponent<UI51GoldCardComponent>();
            return ui;
        }
    }
}
