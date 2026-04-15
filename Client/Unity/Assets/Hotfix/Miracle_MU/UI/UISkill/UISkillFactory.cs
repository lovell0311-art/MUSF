using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UISkill)]
    public class UISkillFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panle = ResourcesComponent.Instance.LoadGameObject(UIType.UISkill.StringToAB(),UIType.UISkill);
            UI uI = ComponentFactory.Create<UI, string, GameObject>(UIType.UISkill,panle,false);
            uI.AddComponent<UISkillComponent>();
            return uI;
        }

     
    }

}
