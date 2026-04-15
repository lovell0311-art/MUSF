using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetNewComponent
    {
        public ReferenceCollector petRightCollector;
        public GameObject PetSkillAttributeTogs;
        public GameObject AttributeBG;
        public GameObject SkillBG;
        public GameObject EnhanceBG;
        public void InitPetRight()
        {
            petRightCollector = petCollector.GetImage("PetRight").gameObject.GetReferenceCollector();

            PetSkillAttributeTogs = petRightCollector.GetGameObject("PetSkillAttributeTogs");
            AttributeBG = petRightCollector.GetImage("AttributeBGNew").gameObject;
            EnhanceBG = petRightCollector.GetImage("EnhanceBG").gameObject;
            enhanceCollector = petRightCollector.GetImage("EnhanceBG").gameObject.GetReferenceCollector();
            //属性技能按钮事件
            PetSkillAttributeTogs.transform.GetChild(0).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => ToggleEvent(GetAttributeSkill(0), value));
            PetSkillAttributeTogs.transform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => ToggleEvent(GetAttributeSkill(1), value));
            PetSkillAttributeTogs.transform.GetChild(2).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => ToggleEvent(GetAttributeSkill(2), value));
            PetSkillAttributeTogs.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            

            EnhanceInit();//强化
            ToggleEvent(AttributeSkill.Attribute, true);
        }
        public void ToggleEvent(AttributeSkill attributeSkill, bool isOn)
        {
            if (!isOn) return;
            if (attributeSkill != AttributeSkill.Enhance) { HindEnhanceCaiLiao(); }
            AttributeBG.gameObject.SetActive(attributeSkill == AttributeSkill.Attribute);
            EnhanceBG.gameObject.SetActive(attributeSkill == AttributeSkill.Enhance);
           
            if (attributeSkill == AttributeSkill.Enhance) { SetEnhanceAtrribe(lastClickInfo.newPetsInfo.PetsID).Coroutine(); }
        }
        public AttributeSkill GetAttributeSkill(int index)
        {
            switch (index)
            {
                case 0:
                    return AttributeSkill.Attribute;
                //case 1:
                //    return AttributeSkill.Skill;
                case 1:
                    return AttributeSkill.Enhance;
            }
            return AttributeSkill.Null;
        }




    }

}
