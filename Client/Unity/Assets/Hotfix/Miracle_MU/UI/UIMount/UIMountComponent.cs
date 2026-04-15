using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using ILRuntime.Runtime;
namespace ETHotfix
{
    [ObjectSystem]
    public class UIMountComponentAwake : AwakeSystem<UIMountComponent>
    {
        public override void Awake(UIMountComponent self)
        {
            self.Awake();

            //self.InitPoint();
            self.InitPetRight();
            //self.InitAttribute();
            //self.InitSkillAwake();
            //self.InitUseExpPanel();
            //self.InitUseSkill();
            //self.InitPetZhuoyue();
            self.InitPetLeft();
            self.BeginnerGuide();
            //self.SetPetRedDot();

        }
    }
    public class MountData
    {
        /// <summary>
        /// 坐骑uid
        /// </summary>
        public long UId=0;
        /// <summary>
        /// 坐骑配置id
        /// </summary>
        public int MountId=0;
        /// <summary>
        /// 坐骑的等级
        /// </summary>
        public int Fortifiedlevel=0;
        /// <summary>
        /// 坐骑的进阶等级
        /// </summary>
        public int AdvancedLevel=0;
        /// <summary>
        /// 是否在使用 0未使用1使用
        /// </summary>
        public int IsUsing=-1;

    }
    public partial class UIMountComponent : Component, IUGUIStatus
    {
        public ReferenceCollector petCollector;
        public ReferenceCollector expCollector;
        public ReferenceCollector useSkillCollector;
        public ReferenceCollector addPointCollector;
        public ReferenceCollector enhanceCollector;
        public MountData UseMountData = new MountData();
        /// <summary>
        /// 是否进入宠物复活倒计时
        /// </summary>
        public bool resurrectionTiming = false;
        //所有宠物信息
        public List<UIPetInfo> PetList = new List<UIPetInfo>();
        //-----------
        public Dictionary<string, int> dicmount=new Dictionary<string, int>();
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


        public Sprite GetSprite(int id)
        {
            if (id==260005)//火龙
            {
              return  petCollector.GetSprite("7");
            }
            else if (id==260006)//冰龙
            {
                return petCollector.GetSprite("6");
            }
            else if (id== 260008||id== 260009)
            {
                return petCollector.GetSprite("4");
            }
            else if(id==260007||id==260012||id==260015||id==260021)
            {
                return petCollector.GetSprite("5");
            }
            else if(id==260024||id==260023||id==260022||id==260017||id==260018)
            {
                return petCollector.GetSprite("2");
            }
            else if(id==260019)
            {
                return petCollector.GetSprite("1");
            }
            else if (id==11)
            {
                return petCollector.GetSprite("8");
            }
            else if (id == 10)
            {
                return petCollector.GetSprite("9");
            }
            return petCollector.GetSprite("2"); 
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
              //  HindEnhanceCaiLiao();
                UIComponent.Instance.Remove(UIType.UIMount);
            });

        }

        /// <summary>
        /// 从服务器获取宠物信息
        /// </summary>
        public async ETVoid InitPetList()
        {
            if (expCollector != null)
            {
                expCollector.gameObject.SetActive(false);
            }
            if (useSkillCollector != null)
            {
                useSkillCollector.gameObject.SetActive(false);
            }
            // HindYaoPingObj();
            //请求服务器，获取所有宠物属性
            G2C_OpenTheMountPanelResponse g2C_OpenPets = (G2C_OpenTheMountPanelResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenTheMountPanelRequest() { });
            Log.DebugBrown("获取坐骑属性" +g2C_OpenPets.MountInfo);
            if (g2C_OpenPets.Error == 0)
            {
                dicmount.Clear();
                PetList.Clear();


                if (string.IsNullOrEmpty(g2C_OpenPets.MountInfo))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"为获取到坐骑数据");
                    return;
                }
                dicmount = JsonConvert.DeserializeObject<Dictionary<string, int>>(g2C_OpenPets.MountInfo);
                foreach (var item in dicmount)
                {
                    UIPetInfo uIPetInfo = new UIPetInfo();
                    uIPetInfo.petId =long.Parse(item.Key);
                    uIPetInfo.petsConfigID = (int)item.Value;
                   // uIPetInfo.Click = true;
                    PetList.Add(uIPetInfo);
                }
                //有宠物----第一个显示的宠物
                //if (g2C_OpenPets.MountInfo != null)
                //{

                //}
                //if (g2C_OpenPets.Current?.PetsConfigID != 0)
                //{
                //    UIPetInfo uIPetInfo = new UIPetInfo();
                //    uIPetInfo.restOrWar = g2C_OpenPets.IsToWar == 1 ? PetWarState.War : PetWarState.Rest;
                //    uIPetInfo.Click = true;
                //    SetPetList(g2C_OpenPets.Current, uIPetInfo);
                //    PetIconInit(g2C_OpenPets.Current.PetsConfigID, uIPetInfo);
                //    PetList.Add(uIPetInfo);
                //    PetRecommend petRecommend = new PetRecommend();
                //    UIRoleInfoData.RecommendAddPointPetInit(uIPetInfo.petAttributeSystem, uIPetInfo.petsLVpoint, out petRecommend.recommendkeyValue);
                //    PetRecommendData.PetsRecommendList[uIPetInfo.petId] = petRecommend;
                //}

                //for (int i = 0, length = g2C_OpenPets.List.Count; i < length; i++)
                //{
                //    UIPetInfo uIPetInfo = new UIPetInfo();
                //    uIPetInfo.petsConfigID = g2C_OpenPets.List[i].PetsConfigID;
                //    uIPetInfo.petId = g2C_OpenPets.List[i].PetsID;
                //    uIPetInfo.Elv = g2C_OpenPets.List[i].ELvis;
                //    uIPetInfo.petLevel = g2C_OpenPets.List[i].PetsLevel;
                //    uIPetInfo.petsTrialTime = g2C_OpenPets.List[i].PetsTrialTime;
                //    uIPetInfo.IsDeath = g2C_OpenPets.List[i].IsDeath;
                //    uIPetInfo.deathTime = g2C_OpenPets.List[i].DeathTime;
                //    uIPetInfo.petsLVpoint = g2C_OpenPets.List[i].Point;
                //    PetIconInit(g2C_OpenPets.List[i].PetsConfigID, uIPetInfo);
                //    PetList.Add(uIPetInfo);
                //    PetRecommend petRecommend = new PetRecommend();
                //    UIRoleInfoData.RecommendAddPointPetInit(uIPetInfo.petAttributeSystem, uIPetInfo.petsLVpoint, out petRecommend.recommendkeyValue);
                //    PetRecommendData.PetsRecommendList[uIPetInfo.petId] = petRecommend;

                //}
                if (PetList.Count > 0)
                {
                   // PetList.Sort((UIPetInfo p1, UIPetInfo p2) => p2.petLevel.CompareTo(p1.petLevel));
                    //int petCount = GetPetSkillCount(PetList.Count, PetSkillType.InitiativeSkill);
                    //for (int i = PetList.Count; i < petCount; i++)
                    //{
                    //    PetList.Add(new UIPetInfo());
                    //}
                }
                else
                {
                    //显示无宠物界面
                    //  SetPetAttribute(new UIPetInfo());
                    PetClickEvent(PetInfo: new UIPetInfo());
                    //  RefeachItem();
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
            if (petsInfo.PetsTrialTime > 0)
                uIPetInfo.petsTrialTime = petsInfo.PetsTrialTime;
            if (petsInfo.IsDeath == 1)
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
            if (petConfig != null)
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
            UseMountData.UId = 0;
            //uICircular_PetPassiveSkill.Dispose();
            //uICircular_PetInitiativeSkill.Dispose();
            //uICircular_PetCanLearnSkill.Dispose();
        }
    }
}
