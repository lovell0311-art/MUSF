using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public ReferenceCollector skillCollCctor;
        public GameObject passiveSkillContent;
        public ScrollRect passiveSkillScrollView;
        public UICircularScrollView<PetSkillInfo> uICircular_PetPassiveSkill;
        public void InitPetPassiveSkill()
        {
            passiveSkillContent = skillCollCctor.GetGameObject("PassiveSkillContent");
            passiveSkillScrollView = skillCollCctor.GetImage("PassiveSkillScrollView").GetComponent<ScrollRect>();
            InitUiCircular_PetPassiveSkill();
        }
        public void InitUiCircular_PetPassiveSkill()
        {
            uICircular_PetPassiveSkill = ComponentFactory.Create<UICircularScrollView<PetSkillInfo>>();
            uICircular_PetPassiveSkill.InitInfo(E_Direction.Vertical, 4, 50, 10);
            uICircular_PetPassiveSkill.ItemInfoCallBack = InitPetPassiveSkillItem;
            uICircular_PetPassiveSkill.IninContent(passiveSkillContent, passiveSkillScrollView);
        }

        /// <summary>
        /// 被动技能
        /// </summary>
        /// <param name="item"></param>
        /// <param name="petSkillInfo"></param>
        public void InitPetPassiveSkillItem(GameObject item, PetSkillInfo petSkillInfo)
        {
          
            PetSkillInfo petConfig = null;
            item.transform.Find("Button").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                clickPetSkill = petSkillInfo;
                SetUsePassiveSkillInfo(petSkillInfo);
               
            });
            petConfig = petPassiveSkillList.Find(f => f.petSkillID == petSkillInfo.petSkillID);
            if (petSkillInfo.petSkillID == 0)
            {
                item.transform.Find("skillIcon").gameObject.SetActive(false);
                item.transform.Find("BG").gameObject.SetActive(true);
                return;
            }
            item.transform.Find("skillIcon").gameObject.SetActive(true);
            item.transform.Find("skillIcon").gameObject.GetComponent<Image>().sprite = petConfig.sprite;
        }
        /// <summary>
        /// 主动技能
        /// </summary>
        /// <param name="item"></param>
        /// <param name="petSkillInfo"></param>
        public void InitInitiativetSkillItem(GameObject item, PetSkillInfo petSkillInfo)
        {
            PetSkillInfo petConfig = petInitiativeSkillList.Find(f => f.petSkillID == petSkillInfo.petSkillID);
            item.transform.Find("Button").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                clickPetSkill = petSkillInfo;
                SetUseInitiativetInfo(petSkillInfo);
                
            });
        
            item.transform.Find("UsingImage").gameObject.SetActive(petSkillInfo.isUse);

            if (petSkillInfo.petSkillID == 0)
            {
                item.transform.Find("skillIcon").gameObject.SetActive(false);
                item.transform.Find("BG").gameObject.SetActive(true);
                return;
            }
            item.transform.Find("skillIcon").gameObject.SetActive(true);
            item.transform.Find("skillIcon").gameObject.GetComponent<Image>().sprite = petConfig.sprite;
        }
        /// <summary>
        /// 可学习技能
        /// </summary>
        /// <param name="item"></param>
        /// <param name="petSkillInfo"></param>
        public void InitCanLearPetSkillItem(GameObject item, PetSkillInfo petSkillInfo)
        {
            PetSkillInfo petConfig = petCanLearnSkillList.Find(f => f.petSkillID == petSkillInfo.petSkillID);
            item.transform.Find("Button").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                clickPetSkill = petSkillInfo;
                SetUseCanLearSkillInfo(petSkillInfo);
              
            });
            if (petSkillInfo.petSkillID == 0)
            {
                item.transform.Find("skillIcon").gameObject.SetActive(false);
                item.transform.Find("BG").gameObject.SetActive(true);
                return;
            }
            item.transform.Find("skillIcon").gameObject.SetActive(true);
            item.transform.Find("skillIcon").gameObject.GetComponent<Image>().sprite = petConfig.sprite;
        }
    }
}

