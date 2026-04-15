using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        UIPetInfo uIPetInfo = new UIPetInfo();
        /// <summary>
        /// 增加属性点
        /// </summary>
        /// <param name="petsId"></param>
        /// <param name="petsAttributeType"></param>
        /// <param name="petsAddPoint"></param>
        /// <returns></returns>
        public async ETTask AddAttributePointRequest(long petsId,int petsAttributeType,int petsAddPoint)
        {

            G2C_AddAttributePointResponse g2C_OpenPets = (G2C_AddAttributePointResponse)await SessionComponent.Instance.Session.Call(new C2G_AddAttributePointRequest()
            {
                PetsID = petsId,
                PetsAttributeType = petsAttributeType,
                PetsAddPoint = petsAddPoint
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                SurplusCount.text = $"剩余点数：{g2C_OpenPets.Info.PetsLVpoint}";

                if (PetRecommendData.PetsRecommendList.TryGetValue(petsId, out PetRecommend recommend))
                {
                    if(recommend.recommendkeyValue.ContainsKey(GetPropertyId()))
                        recommend.recommendkeyValue[GetPropertyId()] -= petsAddPoint;
                }

                uIPetInfo = GetPetInfoToId(petsId);
                SetPetList(g2C_OpenPets.Info, uIPetInfo);
                SetPetAttribute(uIPetInfo);
                SetPetRedDot();
                Log.DebugGreen($"增加属性点成功");
            }

            /// <summary>
            /// 根据属性名字 获取对应的Id
            /// </summary>
            /// <param name="ts"></param>
            /// <returns></returns>
            string GetPropertyId() => petsAttributeType switch
            {
                1 => "power",
                2 => "intell",
                3 => "agile",
                4 => "PhysicalStrength",
                _ => string.Empty
            };

        }
        /// <summary>
        /// 放生宠物
        /// </summary>
        /// <param name="petsId"></param>
        /// <returns></returns>
        public async ETTask PetsReleaseRequest(UIPetInfo uIPetInfo)
        {
            G2C_PetsReleaseResponse g2C_OpenPets = (G2C_PetsReleaseResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsReleaseRequest()
            {
                PetsID = uIPetInfo.petId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                if (UnitEntityComponent.Instance.Get<PetEntity>(uIPetInfo.petId) != null)
                {
                    UnitEntityComponent.Instance.Get<PetEntity>(uIPetInfo.petId).DelayTime = 0;
                    UnitEntityComponent.Instance.Get<PetEntity>(uIPetInfo.petId).Dispose();
                    usePetId = 0;
                }
                if(lastWarClickItem.uIPetInfo != null && lastWarClickItem.uIPetInfo.petId == uIPetInfo.petId)
                {
                    UIMainComponent.Instance?.HidePetPanle();
                }

                InitPetList().Coroutine();
                SetPetRedDot();
                if (petGameObject != null && petGameObject.activeSelf) ResourcesComponent.Instance.DestoryGameObjectImmediate(petGameObject, petGameObject.name.StringToAB());//回收上一个 角色模型
                
                UIComponent.Instance.VisibleUI(UIType.UIHint, "放生成功!");
            }
        }
        /// <summary>
        /// 洗点
        /// </summary>
        /// <param name="petsId"></param>
        /// <returns></returns>
        public async ETTask PetsWashASpotRequest(long petsId)
        {

            C2G_PetsWashASpotResponse g2C_OpenPets = (C2G_PetsWashASpotResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsWashASpotRequest()
            {
                PetsID = petsId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                lastClickItem.uIPetInfo.petsLVpoint = g2C_OpenPets.Info.PetsLVpoint;
                lastClickItem.uIPetInfo.petAttribute.power = g2C_OpenPets.Info.PetsSTR;
                lastClickItem.uIPetInfo.petAttribute.attackMinForce = g2C_OpenPets.Info.PetsMinATK;
                lastClickItem.uIPetInfo.petAttribute.attackMaxForce = g2C_OpenPets.Info.PetsMaxATK;
                lastClickItem.uIPetInfo.petAttribute.attackSuccess = g2C_OpenPets.Info.PetsASM;
                lastClickItem.uIPetInfo.petAttribute.pvpAttack = g2C_OpenPets.Info.PetsPAR;
                lastClickItem.uIPetInfo.petAttribute.Agile = g2C_OpenPets.Info.PetsDEX;
                lastClickItem.uIPetInfo.petAttribute.defense = g2C_OpenPets.Info.PetsDEF;
                lastClickItem.uIPetInfo.petAttribute.attackSpeed = g2C_OpenPets.Info.PetsSPD;
                lastClickItem.uIPetInfo.petAttribute.speedSkating = g2C_OpenPets.Info.PetsDFR;
                lastClickItem.uIPetInfo.petAttribute.pvpSpeedSkating = g2C_OpenPets.Info.PetsPDFR;
                lastClickItem.uIPetInfo.petAttribute.PhysicalStrength = g2C_OpenPets.Info.PetsPSTR;
                lastClickItem.uIPetInfo.petAttribute.wit = g2C_OpenPets.Info.PetsPINT;
                lastClickItem.uIPetInfo.petAttribute.skillAttackPower = g2C_OpenPets.Info.PetsSATK;
                lastClickItem.uIPetInfo.petAttribute.minMagicValue = g2C_OpenPets.Info.PetsMP;
                lastClickItem.uIPetInfo.petAttribute.maxMagicValue = g2C_OpenPets.Info.PetsMPMax;
                SetPetAttribute(lastClickItem.uIPetInfo);

                UIComponent.Instance.VisibleUI(UIType.UIHint, "洗点成功!");
            }
        }
        /// <summary>
        /// 复活
        /// </summary>
        /// <param name="petsId"></param>
        /// <returns></returns>
        public async ETTask PetsResurrectionRequest(long petsId)
        {

            G2C_PetsResurrectionResponse g2C_OpenPets = (G2C_PetsResurrectionResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsResurrectionRequest()
            {
                PetsID = petsId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());

            }
            else
            {
                InitPetList().Coroutine();
                UIMainComponent.Instance?.GetWarPetData().Coroutine();
            }
        }
        /// <summary>
        /// 出战宠物
        /// </summary>
        /// <param name="petsId"></param>
        /// <returns></returns>
        public async ETTask PetsGoToWarRequest(long petsId)
        {

            G2C_PetsGoToWarResponse g2C_OpenPets = (G2C_PetsGoToWarResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsGoToWarRequest()
            {
                PetsID = petsId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                if(UnitEntityComponent.Instance.Get<PetEntity>(usePetId) != null)
                {
                    PetEntity petEntity = UnitEntityComponent.Instance.Get<PetEntity>(usePetId);
                    petEntity.DelayTime = 0;
                    petEntity.Dispose();
                }
                usePetId = lastWarClickItem.uIPetInfo.petId;

                UIMainComponent.Instance?.GetWarPetData().Coroutine();
              
            }
        }
        /// <summary>
        /// 宠物休息
        /// </summary>
        /// <param name="petsId"></param>
        /// <returns></returns>
        public async ETTask PetsRestRequest(long petsId)
        {

            G2C_PetsRestResponse g2C_OpenPets = (G2C_PetsRestResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsRestRequest()
            {
                PetsID = petsId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                UIMainComponent.Instance?.HidePetPanle();
                usePetId = 0;
            }
        }
        /// <summary>
        /// 宠物放进背包
        /// </summary>
        /// <param name="petsId"></param>
        /// <returns></returns>
        public async ETTask PetPackBackRequest(long petsId)
        {

            G2C_PetsPackBackResponse g2C_OpenPets = (G2C_PetsPackBackResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsPackBackRequest()
            {
                PetsID = petsId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                if (UnitEntityComponent.Instance.Get<PetEntity>(petsId) != null)
                {
                    UnitEntityComponent.Instance.Get<PetEntity>(petsId).DelayTime = 0;
                    UnitEntityComponent.Instance.Get<PetEntity>(petsId).Dispose();
                    usePetId = 0;
                }
                if (lastWarClickItem.uIPetInfo != null && lastWarClickItem.uIPetInfo.petId == petsId)
                {
                    UIMainComponent.Instance?.HidePetPanle();
                }
                InitPetList().Coroutine();

                if (petGameObject != null && petGameObject.activeSelf) ResourcesComponent.Instance.DestoryGameObjectImmediate(petGameObject, petGameObject.name.StringToAB());//回收上一个 角色模型

                UIComponent.Instance.VisibleUI(UIType.UIHint, "放入背包成功!");
            }
        }
        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="petsId"></param>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public async ETTask UsePetsSkillRequest(PetInfo_Obj petInfo_Obj, int skillID)
        {
            
            G2C_UsePetsSkillResponse g2C_OpenPets = (G2C_UsePetsSkillResponse)await SessionComponent.Instance.Session.Call(new C2G_UsePetsSkillRequest()
            {
                PetsID = petInfo_Obj.uIPetInfo.petId,
                SkillID = skillID
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                for (int i = 0; i < petInitiativeSkillList.Count; i++)
                {
                    if (petInitiativeSkillList[i].isUse == true)
                    {
                        petInitiativeSkillList[i].isUse = false;
                    }
                }
                clickPetSkill.isUse = true;
                petInfo_Obj.uIPetInfo.usingPetsSkillID = skillID;
                RefeachItem();
            }
        }
        /// <summary>
        /// 技能学习
        /// </summary>
        /// <param name="petsID"></param>
        /// <returns></returns>
        public async ETTask PetsLearnSkillRequest(long petsID, PetsItem skillId)
        {
            G2C_PetsLearnSkillResponse g2C_OpenPets = (G2C_PetsLearnSkillResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsLearnSkillRequest()
            {
                PetsID = petsID,
                Skill = skillId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                clickPetSkill.learned = true;
                RefeachItem();
            }
        }
        /// <summary>
        /// 取消技能
        /// </summary>
        /// <param name="petsId"></param>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public async ETTask PetsSkillCancelUseRequest(PetInfo_Obj petInfo_Obj, int skillID)
        {

            G2C_PetsSkillCancelUseResponse g2C_OpenPets = (G2C_PetsSkillCancelUseResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsSkillCancelUseRequest()
            {
                PetsID = petInfo_Obj.uIPetInfo.petId,
                SkillID = skillID
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                clickPetSkill.isUse = false;
                petInfo_Obj.uIPetInfo.usingPetsSkillID = 0;
                RefeachItem();
            }
        }
        /// <summary>
        /// 获得新宠物
        /// </summary>
        /// <param name="petsConfigID"></param>
        /// <returns></returns>
        public async ETTask InsertPetsRequest(int petsConfigID)
        {
            G2C_InsertPetsResponse g2C_OpenPets = (G2C_InsertPetsResponse)await SessionComponent.Instance.Session.Call(new C2G_InsertPetsRequest()
            {
                PetsConfigID = petsConfigID
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            //else
            //{
            //    InitPetList().Coroutine();
            //    Log.DebugGreen("获得新宠物成功");
            //}
        }
        /// <summary>
        /// 获取单个宠物信息
        /// </summary>
        /// <param name="petsID"></param>
        /// <returns></returns>
        public async ETTask GetPetsInfoRequest(long petsID, GameObject item = null, UIPetInfo PetInfo = null)
        {
           
            G2C_GetPetsInfoResponse g2C_OpenPets = (G2C_GetPetsInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_GetPetsInfoRequest()
            {
                PetsID = petsID
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                UIPetInfo petsInfo = PetList.Find(e => e.petId == petsID);
                if (petsInfo != null)
                {
                    
                    SetPetList(g2C_OpenPets.Info, PetList[PetList.IndexOf(petsInfo)]);
                    //uICircular_Pet.Items = PetList;
                    //设置宠物等级
                    PetLevelTxt.text = "LV:" + PetList[PetList.IndexOf(petsInfo)].petLevel.ToString();
                    //设置宠物经验条
                    ExpSlider.transform.Find("ExpSliderValue").GetComponent<Image>().fillAmount =
                        (float)(PetList[PetList.IndexOf(petsInfo)].curExp / PetList[PetList.IndexOf(petsInfo)].curHighestExp);
                    //设置宠物经验文本提示
                    ExpSlider.transform.Find("ExpTxt").GetComponent<Text>().text = $"{PetList[PetList.IndexOf(petsInfo)].curExp} / {PetList[PetList.IndexOf(petsInfo)].curHighestExp}";
                }
               
                if (item != null)
                {
                    SetPetClickEvent(item, PetInfo);
                }

            }
        }
    }
}
