using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIFirendList)]
    public class UIFriendListnewFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIFirendList.StringToAB(), UIType.UIFirendList);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIFirendList, panel,false);
            //给实体增加组件
            ui.AddComponent<UIFriendListComponent>();
            return ui;
        }
    }
}


