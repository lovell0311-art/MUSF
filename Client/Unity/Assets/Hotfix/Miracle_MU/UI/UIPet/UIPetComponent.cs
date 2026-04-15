using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIPetComponentAwake : AwakeSystem<UIPetComponent>
    {
        public override void Awake(UIPetComponent self)
        {
            self.Awake();

            self.InitPoint();
            self.InitPetRight();
            self.InitAttribute();
            self.InitSkillAwake();
            self.InitUseExpPanel();
            self.InitUseSkill();
            self.InitPetZhuoyue();
            self.InitPetLeft();
            self.BeginnerGuide();
            //self.SetPetRedDot();

        }
    }
    public partial class UIPetComponent : Component, IUGUIStatus
    {
        public ReferenceCollector petCollector;
        public ReferenceCollector expCollector;
        public ReferenceCollector useSkillCollector;
        public ReferenceCollector addPointCollector;
        public ReferenceCollector enhanceCollector;

       
        /// <summary>
        /// 是否进入宠物复活倒计时
        /// </summary>
        public bool resurrectionTiming = false;
        //所有宠物信息
        public List<UIPetInfo> PetList = new List<UIPetInfo>();

        public GameObject UIBeginnerGuide, UIBeginnerGuideSkill, UIBeginnerGuideCanSkill, UIBeginnerGuideClose;
        public void BeginnerGuide()
        {
            UIBeginnerGuide = petCollector.GetImage("UIBeginnerGuide").gameObject;
            UIBeginnerGuideSkill = petCollector.GetImage("UIBeginnerGuideSkill").gameObject;
            UIBeginnerGuideCanSkill = petCollector.GetImage("UIBeginnerGuideCanSkill").gameObject;
            UIBeginnerGuideClose = petCollector.GetImage("UIBeginnerGuideClose").gameObject;

            if (BeginnerGuideData.IsComplete(21))
            {
                BeginnerGuideData.SetBeginnerGuide(21);
                UIBeginnerGuide.SetActive(true);
            }

        }

        public void SetPetRedDot()
        {
            int count = 0;
            for (int i = 0, length = PetList.Count; i < length; i++)
            {
                count += PetList[i].petsLVpoint;
            }
            if (count == 0)
            {
                RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Pet, 0);
                UIMainComponent.Instance.RedDotFriendCheack();
            }
            
        }

        public void Awake()
        {
            petCollector = GetParent<UI>().GameObject.GetReferenceCollector();
            expCollector = petCollector.GetGameObject("PetExpPanel").GetReferenceCollector();
            useSkillCollector = petCollector.GetGameObject("PetSkillPanel").GetReferenceCollector();
            addPointCollector = petCollector.GetGameObject("AddPoint").GetReferenceCollector();
            //关闭按钮
            petCollector.GetButton("CloseBtn").onClick.AddListener(() =>
            {
                if (BeginnerGuideData.IsComplete(25))
                {
                    BeginnerGuideData.SetBeginnerGuide(25);
                    UIMainComponent.Instance.SetBeginnerGuide();
                }
                HindEnhanceCaiLiao();
                UIComponent.Instance.Remove(UIType.UIPet);
            });

        }

        /// <summary>
        /// 从服务器获取宠物信息
        /// </summary>
        public async ETVoid InitPetList()
        {
            if(expCollector != null)
            {
                expCollector.gameObject.SetActive(false);
            }
            if(useSkillCollector != null)
            {
                useSkillCollector.gameObject.SetActive(false);
            }
            HindYaoPingObj();
           
            //请求服务器，获取所有宠物属性
            G2C_OpenPetsInterfaceResponse g2C_OpenPets = (G2C_OpenPetsInterfaceResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenPetsInterfaceRequest() { });
            Log.DebugBrown("获取宠物属性" + JsonHelper.ToJson(g2C_OpenPets.Current));
            if(g2C_OpenPets.Error == 0)
            {
               
                PetList.Clear();
                //有宠物----第一个显示的宠物
                if (g2C_OpenPets.Current?.PetsConfigID != 0)
                {
                    UIPetInfo uIPetInfo = new UIPetInfo();
                    uIPetInfo.restOrWar = g2C_OpenPets.IsToWar == 1 ? PetWarState.War : PetWarState.Rest;
                    uIPetInfo.Click = true;
                    SetPetList(g2C_OpenPets.Current, uIPetInfo);
                    PetIconInit(g2C_OpenPets.Current.PetsConfigID, uIPetInfo);
                    PetList.Add(uIPetInfo);
                    PetRecommend petRecommend = new PetRecommend();
                    UIRoleInfoData.RecommendAddPointPetInit(uIPetInfo.petAttributeSystem, uIPetInfo.petsLVpoint, out petRecommend.recommendkeyValue);
                    PetRecommendData.PetsRecommendList[uIPetInfo.petId] = petRecommend;
                }
                
                for (int i = 0, length = g2C_OpenPets.List.Count; i < length; i++)
                {
                    UIPetInfo uIPetInfo = new UIPetInfo();
                    uIPetInfo.petsConfigID = g2C_OpenPets.List[i].PetsConfigID;
                    uIPetInfo.petId = g2C_OpenPets.List[i].PetsID;
                    uIPetInfo.Elv = g2C_OpenPets.List[i].ELvis;
                    uIPetInfo.petLevel = g2C_OpenPets.List[i].PetsLevel;
                    uIPetInfo.petsTrialTime = g2C_OpenPets.List[i].PetsTrialTime;
                    uIPetInfo.IsDeath = g2C_OpenPets.List[i].IsDeath;
                    uIPetInfo.deathTime = g2C_OpenPets.List[i].DeathTime;
                    uIPetInfo.petsLVpoint = g2C_OpenPets.List[i].Point;
                    PetIconInit(g2C_OpenPets.List[i].PetsConfigID, uIPetInfo);
                    PetList.Add(uIPetInfo);
                    PetRecommend petRecommend = new PetRecommend();
                    UIRoleInfoData.RecommendAddPointPetInit(uIPetInfo.petAttributeSystem, uIPetInfo.petsLVpoint, out petRecommend.recommendkeyValue);
                    PetRecommendData.PetsRecommendList[uIPetInfo.petId] = petRecommend;

                }
                if (PetList.Count > 0)
                {
                    PetList.Sort((UIPetInfo p1, UIPetInfo p2) => p2.petLevel.CompareTo(p1.petLevel));
                    int petCount = GetPetSkillCount(PetList.Count, PetSkillType.InitiativeSkill);
                    for (int i = PetList.Count; i < petCount; i++)
                    {
                        PetList.Add(new UIPetInfo());
                    }
                }
                else
                {
                    //显示无宠物界面
                    SetPetAttribute(new UIPetInfo());
                    PetClickEvent(PetInfo: new UIPetInfo());
                    RefeachItem();
                    SetPetTime(false);
                    lastClickItem.petIcomObj = null;
                    lastClickItem.uIPetInfo = null;
                }

                resurrectionTiming = true;
                uICircular_Pet.Items = PetList;
                SetPetRedDot();
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
                PetClickEvent(PetInfo: new UIPetInfo());
            }
        }
        public void SetPetList(PetsInfo petsInfo, UIPetInfo uIPetInfo)
        {
            uIPetInfo.petsConfigID = petsInfo.PetsConfigID;
            uIPetInfo.petId = petsInfo.PetsID;
            uIPetInfo.petName = petsInfo.PetsName;
            uIPetInfo.petsType = petsInfo.PetsType;
            uIPetInfo.petLevel = petsInfo.PetsLevel;
            uIPetInfo.Elv = petsInfo.Elv;
            uIPetInfo.petsLVpoint = petsInfo.PetsLVpoint;
            uIPetInfo.curHp = petsInfo.PetsHP;
            uIPetInfo.maxHp = petsInfo.PetsHPMax;
            uIPetInfo.curMp = petsInfo.PetsMP;
            uIPetInfo.maxMp = petsInfo.PetsMPMax;
            uIPetInfo.curExp = petsInfo.PetsEXP;
            uIPetInfo.usingPetsSkillID = petsInfo.PetsSkillID;
            uIPetInfo.curHighestExp = GetPetExprience(petsInfo.PetsLevel);
            if(petsInfo.PetsTrialTime > 0)
            uIPetInfo.petsTrialTime = petsInfo.PetsTrialTime;
            if(petsInfo.IsDeath == 1)
            uIPetInfo.IsDeath = petsInfo.IsDeath;
            uIPetInfo.deathTime = petsInfo.DeathTime;
            uIPetInfo.petAttribute.power = petsInfo.PetsSTR;
            uIPetInfo.petAttribute.attackMinForce = petsInfo.PetsMinATK;
            uIPetInfo.petAttribute.attackMaxForce = petsInfo.PetsMaxATK;
            uIPetInfo.petAttribute.attackSuccess = petsInfo.PetsASM;
            uIPetInfo.petAttribute.pvpAttack = petsInfo.PetsPAR;
            uIPetInfo.petAttribute.Agile = petsInfo.PetsDEX;
            uIPetInfo.petAttribute.defense = petsInfo.PetsDEF;
            uIPetInfo.petAttribute.speedSkating = petsInfo.PetsDFR;
            uIPetInfo.petAttribute.attackSpeed = petsInfo.PetsSPD;
            uIPetInfo.petAttribute.pvpSpeedSkating = petsInfo.PetsPDFR;
            uIPetInfo.petAttribute.PhysicalStrength = petsInfo.PetsPSTR;
            uIPetInfo.petAttribute.wit = petsInfo.PetsPINT;
            uIPetInfo.petAttribute.skillAttackPower = petsInfo.PetsSATK;
            uIPetInfo.petAttribute.minMagicValue = petsInfo.PetsMinINT;
            uIPetInfo.petAttribute.maxMagicValue = petsInfo.PetsMaxINT;
            uIPetInfo.Anvance = petsInfo.AdvancedLevel;


        }
        /// <summary>
        /// 获得宠物Icon
        /// </summary>
        /// <param name="index"></param>
        public void PetIconInit(int index, UIPetInfo uIPetInfo)
        {
            Pets_InfoConfig petConfig = ConfigComponent.Instance.GetItem<Pets_InfoConfig>(index);
            if (petConfig != null)
            {
                Sprite initSprite = petICons.GetReferenceCollector().GetSprite(petConfig.PetAsset);
                uIPetInfo.petName = petConfig.Name;
                uIPetInfo.petAsset = petConfig.PetAsset;
                uIPetInfo.sprite = initSprite;
                if (petConfig.AttackType == 1)
                {
                    uIPetInfo.petAttributeSystem = PetAttributeSystem.Magic;
                }
                else
                {
                    uIPetInfo.petAttributeSystem = PetAttributeSystem.Physics;
                }
                uIPetInfo.petsType = petConfig.PetsType;
            }
        }
        public long GetPetExprience(int index)
        {
            Pets_ExpConfig petConfig = ConfigComponent.Instance.GetItem<Pets_ExpConfig>(index);
            if(petConfig != null)
            {
                return petConfig.Exprience; 
            }
            return 0;
        }
        public void OnVisible(object[] data)
        {

        }
        public void OnVisible()
        {
            InitPetList().Coroutine();
        }
        public void OnInVisibility()
        {
           
        }
        public override void Dispose()
        {
            tokenSource.Cancel();
            base.Dispose();
            if (petGameObject != null)
                ResourcesComponent.Instance.DestoryGameObjectImmediate(petGameObject, petGameObject.name.StringToAB());
            petGameObject = null;
            PetRecommendData.PetsRecommendList.Clear();
            RedDotPet.Clear();
            uICircular_Pet.Dispose();
            uICircular_PetPassiveSkill.Dispose();
            uICircular_PetInitiativeSkill.Dispose();
            uICircular_PetCanLearnSkill.Dispose();
        }
    }
}
