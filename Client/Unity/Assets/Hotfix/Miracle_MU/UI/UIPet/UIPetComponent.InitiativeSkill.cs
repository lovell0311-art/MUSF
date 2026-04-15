using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public GameObject initiativeSkillContent;
        public ScrollRect initiativeSkillScrollView;
        public UICircularScrollView<PetSkillInfo> uICircular_PetInitiativeSkill;


        public void InitPetInitiative()
        {
            initiativeSkillContent = skillCollCctor.GetGameObject("InitiativeSkillContent");
            initiativeSkillScrollView = skillCollCctor.GetImage("InitiativeSkillScrollView").GetComponent<ScrollRect>();
            InitUiCircular_PetInitiativeSkill();
        }

        public void InitUiCircular_PetInitiativeSkill()
        {
            uICircular_PetInitiativeSkill = ComponentFactory.Create<UICircularScrollView<PetSkillInfo>>();
            uICircular_PetInitiativeSkill.InitInfo(E_Direction.Vertical, 4, 50, 10);
            uICircular_PetInitiativeSkill.ItemInfoCallBack = InitInitiativetSkillItem;
            uICircular_PetInitiativeSkill.IninContent(initiativeSkillContent, initiativeSkillScrollView);
        }
    }

}
