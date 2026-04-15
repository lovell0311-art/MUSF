using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UIAnnouncement)]
    public class UIAnnouncementFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIAnnouncement.StringToAB(),UIType.UIAnnouncement);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIAnnouncement,panel,false);
            ui.AddComponent<UIAnnouncementComponent>();
            return ui;
        }

      
    }
}