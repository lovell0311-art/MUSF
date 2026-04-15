using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public ReferenceCollector attributeCollector;
        //血量
        public GameObject Mp;
        //蓝量
        public GameObject Hp;
        //物理
        public GameObject Wuli;
        //魔法
        public GameObject Mofa;
        //剩余点数
        public Text SurplusCount;

        public Button ReleaseBtn, RestBtn, WarBtn, FuHuoBtn, XiDian, IntoBagBtn;

        public GameObject AttributeContent;
        /// <summary>
        /// 等级限制
        /// </summary>
        public int levelLimit = 1;

        public void InitAttribute()
        {
            attributeCollector = petRightCollector.GetImage("AttributeBG").gameObject.GetReferenceCollector();
            Mp = attributeCollector.GetImage("MP").gameObject;
            Hp = attributeCollector.GetImage("HP").gameObject;
            Wuli = attributeCollector.GetImage("Wuli").gameObject;
            Mofa = attributeCollector.GetImage("Mofa").gameObject;
            XiDian = attributeCollector.GetButton("XiDian");
            IntoBagBtn = attributeCollector.GetButton("IntoBagBtn");
            AttributeContent = attributeCollector.GetImage("AttributeContent").gameObject;
            WarBtn = attributeCollector.GetButton("WarBtn");
            RestBtn = attributeCollector.GetButton("RestBtn");
            ReleaseBtn = attributeCollector.GetButton("ReleaseBtn");
            SurplusCount = attributeCollector.GetText("SurplusCount");
            AttributeBtnInit();
        }

        public void AttributeBtnInit()
        {

            ReleaseBtn.onClick.AddListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($" 是否放生宠物：<color=red>{lastClickItem.uIPetInfo.petName}</color>？");
                    uIConfirmComponent.AddActionEvent(() =>
                    {
                        //放生
                        PetsReleaseRequest(lastClickItem.uIPetInfo).Coroutine();
                    });
                }
                    
            });
            IntoBagBtn.onClick.AddListener(() =>
            {
                if (lastClickItem.uIPetInfo == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"请选择放入背包的宠物！");
                    return;
                }
                PetPackBackRequest(lastClickItem.uIPetInfo.petId).Coroutine();
                //UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                //uIConfirmComponent.SetTipText($"是否消耗1魔晶把宠物：<color=red>{lastClickItem.uIPetInfo.petName}</color>放入背包？");
                //uIConfirmComponent.AddActionEvent(() =>
                //{
                //    //进入背包
                //    PetPackBackRequest(lastClickItem.uIPetInfo.petId).Coroutine();
                //});
                
            });
            RestBtn.onClick.AddSingleListener(delegate ()
            {
                if (BeginnerGuideData.IsComplete(22))
                {
                    BeginnerGuideData.SetBeginnerGuide(22);
                    UIBeginnerGuideSkill.SetActive(true);
                }
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                PetsRestRequest(lastClickItem.uIPetInfo.petId).Coroutine();
                SetPetIconHint(PetWarState.Rest);
                SetBtnActive(PetWarState.Rest, lastClickItem.uIPetInfo.IsDeath); 
                
            });
            WarBtn.onClick.AddSingleListener(delegate ()
            {
                if (BeginnerGuideData.IsComplete(22))
                {
                    BeginnerGuideData.SetBeginnerGuide(22);
                    UIBeginnerGuideSkill.SetActive(true);
                }
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    if (lastClickItem.uIPetInfo.IsDeath != 0)
                    {
                        return;
                    }
                    PetsGoToWarRequest(lastClickItem.uIPetInfo.petId).Coroutine();
                    SetPetIconHint(PetWarState.War);
                    SetBtnActive(PetWarState.War, lastClickItem.uIPetInfo.IsDeath);
                    
                }
                    
            });
            //洗点
            XiDian.onClick.AddSingleListener(() =>
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($" 确定洗点？");
                    uIConfirmComponent.AddActionEvent(() =>
                    {
                        PetsWashASpotRequest(lastClickItem.uIPetInfo.petId).Coroutine();
                    });
                }
            });
            //复活
            FuHuoBtn = attributeCollector.GetButton("FuHuoBtn");
            FuHuoBtn.onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    Log.DebugGreen("宠物复活");
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($" 确定复活？");
                    uIConfirmComponent.AddActionEvent(() =>
                    {
                        PetsResurrectionRequest(lastClickItem.uIPetInfo.petId).Coroutine();
                    });
                } 
            });

            AttributeContent.transform.GetChild(0).Find("AddBtnMore").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    Log.DebugGreen("增加力量");
                    AddPointValue(1);
                }
            });
            AttributeContent.transform.GetChild(0).Find("AddBtn").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    Log.DebugGreen("增加力量");
                    AddAttributePointRequest(lastClickItem.uIPetInfo.petId, 1, 1).Coroutine();
                }
            });
            //-------------------------------------------------------
            AttributeContent.transform.GetChild(4).Find("AddBtnMore").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    Log.DebugGreen("增加敏捷");
                    AddPointValue(3);
                }
                    

            });
            AttributeContent.transform.GetChild(4).Find("AddBtn").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    Log.DebugGreen("增加敏捷");
                    AddAttributePointRequest(lastClickItem.uIPetInfo.petId, 3, 1).Coroutine();
                }

            });

            //-------------------------------------------------------
            AttributeContent.transform.GetChild(9).Find("AddBtnMore").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    AddPointValue(4);
                }
                    
            });

            AttributeContent.transform.GetChild(9).Find("AddBtn").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    AddAttributePointRequest(lastClickItem.uIPetInfo.petId, 4, 1).Coroutine();
                }

            });
            //-------------------------------------------------------
            AttributeContent.transform.GetChild(10).Find("AddBtnMore").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                    Log.DebugGreen("增加智力");
                    AddPointValue(2);
                }
                    
            });
            AttributeContent.transform.GetChild(10).Find("AddBtn").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Exists(e => e.petId != 0))
                {
                   
                    AddAttributePointRequest(lastClickItem.uIPetInfo.petId, 2, 1).Coroutine();
                }

            });
        }

        /// <summary>
        /// 实时更新宠物血量
        /// </summary>
        public void RealTimeUpdatePetHp(long petId,long curHp,long HpMaxValue)
        {
            Log.DebugGreen($"lastClickItem.uIPetInfo:"+curHp+":::"+HpMaxValue);
            if (lastClickItem != null && lastClickItem.uIPetInfo != null && lastClickItem.uIPetInfo.petId == petId)
            {
                Hp.transform.Find("HPValue").GetComponent<Image>().fillAmount = (float)curHp / HpMaxValue/*PetArchiveInfoManager.Instance.HpMaxValue*/;
                Hp.transform.Find("HPValueTxt").GetComponent<Text>().text = $"{(int)curHp} / {HpMaxValue/*(int)PetArchiveInfoManager.Instance.HpMaxValue*/}";
            }
        }
        /// <summary>
        /// 实时更新宠物蓝量
        /// </summary>
        public void RealTimeUpdatePetMp(long petId,long curMp)
        {
            if(lastClickItem != null && lastClickItem.uIPetInfo != null && lastClickItem.uIPetInfo.petId == petId)
            {
                Mp.transform.Find("MPValue").GetComponent<Image>().fillAmount = (float)curMp / PetArchiveInfoManager.Instance.MpMaxValue;
                Mp.transform.Find("MPValueTxt").GetComponent<Text>().text = $"{(int)curMp} / {(int)PetArchiveInfoManager.Instance.MpMaxValue}";
            }
        }
        /// <summary>
        /// 设置宠物属性
        /// </summary>
        /// <param name="uIPetInfo"></param>
        public void SetPetAttribute(UIPetInfo uIPetInfo)
        {
            if (Mp == null) return;
            Mp.transform.Find("MPValue").GetComponent<Image>().fillAmount = uIPetInfo.curMp / uIPetInfo.maxMp;
            Mp.transform.Find("MPValueTxt").GetComponent<Text>().text = $"{(int)uIPetInfo.curMp} / {(int)uIPetInfo.maxMp}";

            Hp.transform.Find("HPValue").GetComponent<Image>().fillAmount = uIPetInfo.curHp / uIPetInfo.maxHp;
            Hp.transform.Find("HPValueTxt").GetComponent<Text>().text = $"{(int)uIPetInfo.curHp} / {(int)uIPetInfo.maxHp}";

            SurplusCount.text = $"剩余点数：{uIPetInfo.petsLVpoint}";

            switch (uIPetInfo.petAttributeSystem)
            {
                case PetAttributeSystem.Magic:
                    Wuli.gameObject.SetActive(false);
                    Mofa.gameObject.SetActive(true);
                    break;
                case PetAttributeSystem.Physics:
                    Mofa.gameObject.SetActive(false);
                    Wuli.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            SetBtnActive(uIPetInfo.restOrWar, uIPetInfo.IsDeath);
            SetAttributeValue(uIPetInfo);

        }
        /// <summary>
        /// 设置休息出战复活按钮
        /// </summary>
        /// <param name="restOrWar"></param>
        /// <param name="daed"></param>
        public void SetBtnActive(PetWarState restOrWar,int daed)
        {

            WarBtn.gameObject.SetActive(restOrWar == PetWarState.Rest && daed == 0);
            RestBtn.gameObject.SetActive(restOrWar == PetWarState.War && daed == 0);
            FuHuoBtn.gameObject.SetActive(daed == 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uIPetInfo"></param>
        public void SetAttributeValue(UIPetInfo uIPetInfo)
        {
            
            AttributeContent.transform.GetChild(0).Find("Value").GetComponent<Text>().text = uIPetInfo.petAttribute.power.ToString();
            if (uIPetInfo.petAttributeSystem == PetAttributeSystem.Physics)
            {
                AttributeContent.transform.GetChild(1).Find("Text").GetComponent<Text>().text = " 攻击力";
                AttributeContent.transform.GetChild(1).Find("Value").GetComponent<Text>().text = $"{uIPetInfo.petAttribute.attackMinForce}~{uIPetInfo.petAttribute.attackMaxForce}";
            }
            else
            {
                AttributeContent.transform.GetChild(1).Find("Text").GetComponent<Text>().text = " 技能攻击力(%)";
                AttributeContent.transform.GetChild(1).Find("Value").GetComponent<Text>().text = $"{uIPetInfo.petAttribute.skillAttackPower}%";
            }
            AttributeContent.transform.GetChild(2).Find("Value").GetComponent<Text>().text = uIPetInfo.petAttribute.attackSuccess.ToString();
            AttributeContent.transform.GetChild(3).Find("Value").GetComponent<Text>().text = uIPetInfo.petAttribute.pvpAttack.ToString();
            AttributeContent.transform.GetChild(4).Find("Value").GetComponent<Text>().text = uIPetInfo.petAttribute.Agile.ToString();
            AttributeContent.transform.GetChild(5).Find("Value").GetComponent<Text>().text = uIPetInfo.petAttribute.defense.ToString();
            AttributeContent.transform.GetChild(6).Find("Value").GetComponent<Text>().text = uIPetInfo.petAttribute.attackSpeed.ToString();//---------------
            AttributeContent.transform.GetChild(7).Find("Value").GetComponent<Text>().text = uIPetInfo.petAttribute.speedSkating.ToString();
            AttributeContent.transform.GetChild(8).Find("Value").GetComponent<Text>().text = uIPetInfo.petAttribute.pvpSpeedSkating.ToString();
            AttributeContent.transform.GetChild(9).Find("Value").GetComponent<Text>().text = uIPetInfo.petAttribute.PhysicalStrength.ToString();
            AttributeContent.transform.GetChild(10).Find("Value").GetComponent<Text>().text = uIPetInfo.petAttribute.wit.ToString();
            if(uIPetInfo.petAttributeSystem == PetAttributeSystem.Physics)
            {
                if (PetRecommendData.PetsRecommendList.TryGetValue(uIPetInfo.petId, out PetRecommend recommend))
                {
                    AttributeContent.transform.GetChild(10)?.Find("AddBtn/RedDot")?.gameObject.SetActive(false);
                    AttributeContent.transform.GetChild(0)?.Find("AddBtn/RedDot")?.gameObject.SetActive(recommend.recommendkeyValue["power"] > 0 && uIPetInfo.petsLVpoint > 0);
                    AttributeContent.transform.GetChild(4)?.Find("AddBtn/RedDot")?.gameObject.SetActive(recommend.recommendkeyValue["agile"] > 0 && uIPetInfo.petsLVpoint > 0);
                    AttributeContent.transform.GetChild(9)?.Find("AddBtn/RedDot")?.gameObject.SetActive(recommend.recommendkeyValue["PhysicalStrength"] > 0 && uIPetInfo.petsLVpoint > 0);
                    if (uIPetInfo.petsLVpoint <= 0)
                    {
                        if (RedDotPet.TryGetValue(uIPetInfo.petId, out GameObject item))
                        {
                            item.transform.Find("RedDot").gameObject.SetActive(false);
                        }
                    }
                }
                AttributeContent.transform.GetChild(11).Find("Text").GetComponent<Text>().text = " 技能攻击力(%)";
                AttributeContent.transform.GetChild(11).Find("Value").GetComponent<Text>().text = $"{uIPetInfo.petAttribute.skillAttackPower}%";
            }
            else
            {
                if (PetRecommendData.PetsRecommendList.TryGetValue(uIPetInfo.petId, out PetRecommend recommend))
                {
                    AttributeContent.transform.GetChild(0)?.Find("AddBtn/RedDot")?.gameObject.SetActive(false);
                    AttributeContent.transform.GetChild(10)?.Find("AddBtn/RedDot")?.gameObject.SetActive(recommend.recommendkeyValue["intell"] > 0 && uIPetInfo.petsLVpoint > 0);
                    AttributeContent.transform.GetChild(4)?.Find("AddBtn/RedDot")?.gameObject.SetActive(recommend.recommendkeyValue["agile"] > 0 && uIPetInfo.petsLVpoint > 0);
                    AttributeContent.transform.GetChild(9)?.Find("AddBtn/RedDot")?.gameObject.SetActive(recommend.recommendkeyValue["PhysicalStrength"] > 0 && uIPetInfo.petsLVpoint > 0);
                    if (uIPetInfo.petsLVpoint <= 0)
                    {
                        if (RedDotPet.TryGetValue(uIPetInfo.petId, out GameObject item))
                        {
                            item.transform.Find("RedDot").gameObject.SetActive(false);
                        }
                    }
                }
                AttributeContent.transform.GetChild(11).Find("Text").GetComponent<Text>().text = " 魔力";
                AttributeContent.transform.GetChild(11).Find("Value").GetComponent<Text>().text = $"{uIPetInfo.petAttribute.minMagicValue}~{uIPetInfo.petAttribute.maxMagicValue}";
            }
             
        }
    }

}
