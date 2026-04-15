using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIBuyMedicine)]
    public class UIBuyMedicineFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panle = ResourcesComponent.Instance.LoadGameObject(UIType.UIBuyMedicine.StringToAB(),UIType.UIBuyMedicine);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIBuyMedicine,panle,false);
            ui.AddComponent<UIBuyMedicineComponent>();
            return ui;
        }
    }
}
