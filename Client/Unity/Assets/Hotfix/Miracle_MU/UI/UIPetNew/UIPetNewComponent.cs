using ETModel;
using ILRuntime.Mono.Cecil.Cil;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

namespace ETHotfix
{
    public class UIPetNewInfo
    {
        public bool IsWar = false;
        public bool IsClick = false;
        public string Name;
        public string petAsset;
        public Sprite sprite;
        public string basicAttribute;
        public string excellent;
        public long petsTrialTime;
        public NewPetsInfo newPetsInfo = new NewPetsInfo();
    }


    [ObjectSystem]
    public class UIPetNewComponentAwake : AwakeSystem<UIPetNewComponent>
    {
        public override void Awake(UIPetNewComponent self)
        {
            self.Awake();
            self.InitPetRight();
            self.InitAttribute();
            self.InitLeft();
        }
    }
    public partial class UIPetNewComponent : Component
    {
        public ReferenceCollector petCollector;
        public List<UIPetNewInfo> newPetsInfos = new List<UIPetNewInfo>();
        public ReferenceCollector enhanceCollector;
        public GameObject NoPetHint;
        public GameObject UIBeginnerGuide,UIBeginnerGuideClose;
        public void Awake()
        {
            petCollector = GetParent<UI>().GameObject.GetReferenceCollector();
            UIBeginnerGuide = petCollector.GetImage("UIBeginnerGuide").gameObject;
            UIBeginnerGuideClose = petCollector.GetImage("UIBeginnerGuideClose").gameObject;
            NoPetHint = petCollector.GetText("NoPetHint").gameObject;
            petCollector.GetButton("CloseBtn").onClick.AddListener(() =>
            {
                //if (BeginnerGuideData.IsComplete(25))
                //{
                //    BeginnerGuideData.SetBeginnerGuide(25);
                //    UIMainComponent.Instance.SetBeginnerGuide();
                //}
                //HindEnhanceCaiLiao();
                if (BeginnerGuideData.IsComplete(23))
                {
                    BeginnerGuideData.SetBeginnerGuide(23);
                    UIMainComponent.Instance.SetBeginnerGuide();
                }
                UIComponent.Instance.Remove(UIType.UIPetNew);
            });
            if (BeginnerGuideData.IsComplete(21))
            {
                BeginnerGuideData.SetBeginnerGuide(21);
                UIBeginnerGuide.SetActive(true);
            }
        }

        /// <summary>
        /// 从服务器获取宠物信息
        /// </summary>
        public async ETVoid InitPetNewList()
        {
            newPetsInfos.Clear();
            G2C_OpenNewPetsUI g2C_OpenNewPets = (G2C_OpenNewPetsUI)await SessionComponent.Instance.Session.Call(new C2G_OpenNewPetsUI() { });
            Log.DebugBrown("获取的宠物信息" + JsonHelper.ToJson(g2C_OpenNewPets));
            if (g2C_OpenNewPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenNewPets.Error.GetTipInfo());
            }
            else
            {
                foreach (var item in g2C_OpenNewPets.PetList)
                {
                    UIPetNewInfo uIPetNewInfo = new UIPetNewInfo();
                    uIPetNewInfo.newPetsInfo = item;
                    uIPetNewInfo.IsWar = item.ISBattle == 1;
                    uIPetNewInfo.petsTrialTime = item.DaoQiTime;
                    foreach (var itemskill in item.SkillID)
                    {
                        Pets_SkillConfig petSkillConfig = ConfigComponent.Instance.GetItem<Pets_SkillConfig>(itemskill);
                        if (petSkillConfig != null)
                        {
                            if (petSkillConfig.skillType == 2)
                                uIPetNewInfo.basicAttribute += petSkillConfig.Describe + "\n";
                        }
                    } 
                    foreach (var itemszuoyue in item.Excellent)
                    {
                        ItemAttrEntry_ExcConfig petzuoyueConfig = ConfigComponent.Instance.GetItem<ItemAttrEntry_ExcConfig>(itemszuoyue);
                        uIPetNewInfo.excellent += petzuoyueConfig.Name + "\n";
                    }

                    Pets_InfoConfig petConfig = ConfigComponent.Instance.GetItem<Pets_InfoConfig>(item.PetsConfigID);
                    if (petConfig != null)
                    {
                        Sprite initSprite = petICons.GetReferenceCollector().GetSprite(petConfig.PetAsset);
                        uIPetNewInfo.petAsset = petConfig.PetAsset;
                        uIPetNewInfo.sprite = initSprite;
                        
                    }
                    uIPetNewInfo.Name = petConfig.Name;
                    newPetsInfos.Add(uIPetNewInfo);
                }
            }
            if(newPetsInfos.Count > 0)
            {
                newPetsInfos.First().IsClick = true;
                uICircular_Pet.Items = newPetsInfos;
            }
            else
            {
                uICircular_Pet.Items = null;
                NoPetHint.SetActive(true); 
                last2ClickInfo = null;
                last2ClickItem = null;
                lastClickItem = null;
                lastClickInfo = null;
                SetAttribute(new UIPetNewInfo());
            }
        }

        public override void Dispose()
        {
            tokenSource.Cancel();
            base.Dispose();
            if (petGameObject != null)
                ResourcesComponent.Instance.DestoryGameObjectImmediate(petGameObject, petGameObject.name.StringToAB());
            petGameObject = null;
            HindEnhanceCaiLiao();
            PetRecommendData.PetsRecommendList.Clear();
            uICircular_Pet.Dispose();
        }
    }

}
