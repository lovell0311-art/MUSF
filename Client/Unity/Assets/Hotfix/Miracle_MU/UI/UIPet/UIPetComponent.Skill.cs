using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public List<PetSkillInfo> petPassiveSkillList = new List<PetSkillInfo>();
        public List<PetSkillInfo> petInitiativeSkillList = new List<PetSkillInfo>();
        public List<PetSkillInfo> petCanLearnSkillList = new List<PetSkillInfo>();
        public GameObject petSkillIcon;
        public void InitSkillAwake()
        {
            petSkillIcon = ResourcesComponent.Instance.LoadGameObject("PetSkill_Icon".StringToAB(), "PetSkill_Icon");
            skillCollCctor = SkillBG.GetComponent<ReferenceCollector>();
            InitPetInitiative();
            InitPetPassiveSkill();
            InitPetCanLearnSkill();
            //TestGetPetSkillInfo();

        }
        public async ETTask OpenPetsSkillRequest(UIPetInfo uIPetInfo)
        {
          
            G2C_OpenPetsSkillResponse g2C_OpenPets = (G2C_OpenPetsSkillResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenPetsSkillRequest()
            {
                PetsID = uIPetInfo.petId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
             
                SkillListClear();
                foreach (var item in g2C_OpenPets.SkillList)
                {
                    PetSkillInfo petSkillInfo = new PetSkillInfo();
                    petSkillInfo.petSkillID = item;
                    PetSkillIconInit((item), petSkillInfo);//(i+1)->技能ID，服务器返回，根据ID获取模型，精灵
                    petSkillInfo.learned = true;
                    switch (petSkillInfo.skillType)
                    {
                        case 1:
                            petSkillInfo.isUse = (uIPetInfo.usingPetsSkillID == item);
                            petInitiativeSkillList.Add(petSkillInfo);
                            break;
                        case 2:
                            petPassiveSkillList.Add(petSkillInfo);
                            break;
                        default:
                            break;
                    }
                }
                foreach (var item in g2C_OpenPets.LesrnSkillList)
                {
                    PetSkillInfo petSkillInfo = new PetSkillInfo();
                    petSkillInfo.petSkillID = item.ItemID;
                    petSkillInfo.petsItem = item;
                    PetSkillIconInit((item.ItemConfingID), petSkillInfo);//(i+1)->技能ID，服务器返回，根据ID获取模型，精灵
                    petSkillInfo.learned = false;
                    petCanLearnSkillList.Add(petSkillInfo);
                }
                InitPetSkillContent();
            }
        }
        /// <summary>
        /// 宠物技能储存完调用
        /// </summary>
        public void InitPetSkillContent()
        {
            int count = GetPetSkillCount(petPassiveSkillList.Count);
            int realCount = petPassiveSkillList.Count;
           
            for (int i = 0; i < count - realCount; i++)
            {
                PetSkillInfo petSkillInfo = new PetSkillInfo();
                petSkillInfo.skillType = 2;
                petPassiveSkillList.Add(petSkillInfo);
            }
            count = GetPetSkillCount(petInitiativeSkillList.Count);
            realCount = petInitiativeSkillList.Count;
            
            for (int i = 0; i < count - realCount; i++)
            {
                PetSkillInfo petSkillInfo = new PetSkillInfo();
                petSkillInfo.skillType = 1;
                petSkillInfo.isUse = false;
                petInitiativeSkillList.Add(petSkillInfo);
            }
            count = GetPetSkillCount(petCanLearnSkillList.Count, PetSkillType.CanLearnPassiveSkill);
            realCount = petCanLearnSkillList.Count;
        
            for (int i = 0; i < count - realCount; i++)
            {
                petCanLearnSkillList.Add(new PetSkillInfo());
            }
          
            RefeachItem();
        }
        public void PetSkillIconInit(int index, PetSkillInfo petSkillInfo)
        {
        
            Pets_SkillConfig petSkillConfig = ConfigComponent.Instance.GetItem<Pets_SkillConfig>(index);
            if (petSkillConfig != null)
            {
                Sprite initSprite = petSkillIcon.GetReferenceCollector().GetSprite(petSkillConfig.Icon);

                petSkillInfo.petSkillID = (int)petSkillConfig.Id;
                petSkillInfo.petSkillName = petSkillConfig.Name;
                petSkillInfo.petAsset = petSkillConfig.Icon;
                petSkillInfo.sprite = initSprite;
                petSkillInfo.petSkillDes = petSkillConfig.Describe;
                petSkillInfo.skillType = petSkillConfig.skillType;
            }
        }
        private int GetPetSkillCount(int count,PetSkillType petSkillType = PetSkillType.PassiveSkill)
        {
            switch (petSkillType)
            {
                case PetSkillType.InitiativeSkill:
                    if (count <= 10) return 10;
                    if (count <= 15) return 15;
                    if (count <= 20) return 20;
                    break;
                case PetSkillType.PassiveSkill:
                    if (count <= 8) return 8;
                    if (count <= 12) return 12;
                    if (count <= 16) return 16;
                    break;
                case PetSkillType.CanLearnPassiveSkill:
                    if (count <= 4) return 4;
                    if (count <= 8) return 8;
                    if (count <= 12) return 12;
                    if (count <= 16) return 16;
                    break;
                default:
                    break;
            }
            return 0;
        }

        public void RefeachItem()
        {
            uICircular_PetPassiveSkill.Items = petPassiveSkillList;
            uICircular_PetInitiativeSkill.Items = petInitiativeSkillList;
            uICircular_PetCanLearnSkill.Items = petCanLearnSkillList;
        }
        public void SkillListClear()
        {
            petPassiveSkillList.Clear();
            petInitiativeSkillList.Clear();
            petCanLearnSkillList.Clear();
        }

    }
}

