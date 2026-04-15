using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public GameObject canLearnSkillContent;
        public ScrollRect canLearnSkillScrollView;
        public UICircularScrollView<PetSkillInfo> uICircular_PetCanLearnSkill;

        public void InitPetCanLearnSkill()
        {
            canLearnSkillContent = skillCollCctor.GetGameObject("CanLearnSkillContent");
            canLearnSkillScrollView = skillCollCctor.GetImage("CanLearnSkillScrollView").GetComponent<ScrollRect>();
            InitUiCircular_PetCanLearnSkill();
        }
        public void InitUiCircular_PetCanLearnSkill()
        {
            uICircular_PetCanLearnSkill = ComponentFactory.Create<UICircularScrollView<PetSkillInfo>>();
            uICircular_PetCanLearnSkill.InitInfo(E_Direction.Horizontal, 1, 50, 0);
            uICircular_PetCanLearnSkill.ItemInfoCallBack = InitCanLearPetSkillItem;
            uICircular_PetCanLearnSkill.IninContent(canLearnSkillContent, canLearnSkillScrollView);
        }
    }

}
