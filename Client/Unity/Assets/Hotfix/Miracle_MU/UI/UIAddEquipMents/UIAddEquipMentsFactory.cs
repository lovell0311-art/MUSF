using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UIAddEquipMents)]
    public class UIAddEquipMentsFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panle = ResourcesComponent.Instance.LoadGameObject(UIType.UIAddEquipMents.StringToAB(),UIType.UIAddEquipMents);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIAddEquipMents,panle);
            ui.AddComponent<UIAddEquipMentsComponet>();
            return ui;
        }

    }
}
