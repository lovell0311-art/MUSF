using ETModel;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIMountComponentUpdate : UpdateSystem<UIMountComponent>
    {
        public override void Update(UIMountComponent self)
        {
            //if (self.petGameObject != null && self.isDrag && self.lastRatePos.x != Input.mousePosition.x && self.petGameObject.activeSelf)
            //{

            //    float offPos_x = self.lastRatePos.x - Input.mousePosition.x;
            //    self.petGameObject.transform.Rotate(0, offPos_x, 0);
            //    self.lastRatePos = Input.mousePosition;
            //}
            //self.ExpUpdate();
        }
    }
    public partial class UIMountComponent
    {
        public bool isDaoJishi = false;
        public bool isDrag = false;//是否开始旋转角色
        public Vector3 lastRatePos;//上一次旋转的为位置
        public ReferenceCollector petLeftCollector;
        public GameObject ExpSlider;
        public Text PetNameTxt;
        public Text PetLevelTxt;
        public Button ExpAddBtn;
        public UICircularScrollView<UIPetInfo> uICircular_Pet;
        public GameObject PetContent;
        public GameObject PetShowModel;
        public GameObject PetModelBase;
        public ScrollRect PetView;
        //=======================================================================================
        //宠物图标内容
        GameObject petICons;
        GameObject RenderTextConfig;
        //上一个点击的宠物
        public PetInfo_Obj lastClickItem = new PetInfo_Obj();
        //上一个出战的宠物
        public PetInfo_Obj lastWarClickItem = new PetInfo_Obj();
        public List<PetInfo_Obj> tiYanPet = new List<PetInfo_Obj>();
        public long usePetId = 0;

        public GameObject petGameObject = null;

        public Transform renderTexture;
        public GameObject petsTrialTime;
        public GameObject DidTime;

        private bool IsHaveDead = false;
        private int DeadTime = 0;

        public Dictionary<long, GameObject> RedDotPet = new Dictionary<long, GameObject>();

        CancellationTokenSource tokenSource;
        CancellationToken cancelLogin;
        public void InitPetLeft()
        {
            tokenSource = new CancellationTokenSource();
            cancelLogin = tokenSource.Token;
            petICons = ResourcesComponent.Instance.LoadGameObject("Pet_Icon".StringToAB(), "Pet_Icon");
            RenderTextConfig = ResourcesComponent.Instance.LoadGameObject("RenderTxtureConfig".StringToAB(), "RenderTxtureConfig");
            petLeftCollector = petCollector.GetImage("PetLeft").gameObject.GetReferenceCollector();
            ExpSlider = petLeftCollector.GetImage("ExpSlider").gameObject;
            PetNameTxt = petLeftCollector.GetText("PetNameTxt");
            PetLevelTxt = petLeftCollector.GetText("PetLevelTxt");
            ExpAddBtn = petLeftCollector.GetButton("ExpAddBtn");
            ExpAddBtn.onClick.AddListener(delegate ()
            {
                if (PetList.Count != 0)
                {
                    if (lastClickItem.uIPetInfo == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "请先选择宠物");
                        return;
                    }
                   // SetExpPanelInfo(lastClickItem.uIPetInfo).Coroutine();
                }

            });
            PetContent = petLeftCollector.GetGameObject("Content");
            PetShowModel = petLeftCollector.GetGameObject("PetShowModel");
            PetModelBase = petLeftCollector.GetGameObject("PetModelBase");
            PetView = petLeftCollector.GetImage("Scroll View").gameObject.GetComponent<ScrollRect>();
            renderTexture = petLeftCollector.GetGameObject("PetShowModel").transform;
            petsTrialTime = petLeftCollector.GetImage("SetPetTime").transform.Find("petsTrialTime").gameObject;
            DidTime = petLeftCollector.GetImage("SetPetTime").transform.Find("DidTime").gameObject;
            Log.DebugBrown("mount_left");
          //  ReviveTime().Coroutine();
            InitUICircular_Pet();
            //this.GetParent<UI>().GetComponent<RenderTextureComponent>().Render(RenderTextConfig.GetReferenceCollector().GetRenderTexture("PetRenderTexture"), renderTexture);
            RegisterDragEvent();

        }
        /// <summary>
        /// 旋转宠物
        /// </summary>
        public void RegisterDragEvent()
        {
            GameObject drag = petLeftCollector.GetImage("DragPetEvent").gameObject;
            UGUITriggerProxy proxy = drag.GetComponent<UGUITriggerProxy>();
            proxy.OnBeginDragEvent = () =>
            {
                isDrag = true;
                lastRatePos = Input.mousePosition;
            };
            proxy.OnEndDragEvent += () =>
            {
                isDrag = false;
                lastRatePos = Vector3.zero;
            };
        }
        /// <summary>
        /// 初始化宠物栏滑动框
        /// </summary>
        public void InitUICircular_Pet()
        {
            uICircular_Pet = ComponentFactory.Create<UICircularScrollView<UIPetInfo>>();
            uICircular_Pet.InitInfo(E_Direction.Vertical, 5, 19, 30);
            uICircular_Pet.ItemInfoCallBack = InitPetItem;
            uICircular_Pet.IninContent(PetContent, PetView);
        }




        /// <summary>
        /// 获取坐骑的数据
        /// </summary>
        /// <param name="petInfo"></param>
        /// <returns></returns>
        public async ETVoid GetMountDataList(UIPetInfo petInfo)
        {
            Log.DebugBrown("打印数据" + petInfo.petId + ":::" + petInfo.petsConfigID);
            G2C_GetMountInfoResponse g2C_OpenPets = (G2C_GetMountInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_GetMountInfoRequest() {MountID=petInfo.petId });
            if (g2C_OpenPets.Error!=0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
                return;
            }
            else
            {
                UseMountData.UId = g2C_OpenPets.MountID;
                UseMountData.MountId = g2C_OpenPets.ConfigId;
                UseMountData.Fortifiedlevel = g2C_OpenPets.Fortifiedlevel;
                UseMountData.AdvancedLevel = g2C_OpenPets.AdvancedLevel;
                UseMountData.IsUsing = g2C_OpenPets.IsUsing;
                RefreshMount();
            }
            ToggleEvent(AttributeSkill.Attribute, true);
            PetSkillAttributeTogs.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            MountClickEvent();
            Log.DebugBrown("坐骑数据返回" + JsonHelper.ToJson(g2C_OpenPets));
        }



        /// <summary>
        /// 初始化宠物信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="PetInfo"></param>
        private void InitPetItem(GameObject item, UIPetInfo PetInfo)
        {
            if (!RedDotPet.ContainsKey(PetInfo.petId))
            {
                RedDotPet.Add(PetInfo.petId, item);
            }

            //宠物图标按钮
            item.transform.Find("PetColumnBtn").gameObject.GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {

                SetPetTime(PetInfo);
                GetPetsInfoRequest(PetInfo.petId, item, PetInfo).Coroutine();

                ShowPetModel(item, PetInfo);
                GetMountDataList(PetInfo).Coroutine();
                
                //  OpenPetsSkillRequest(PetInfo).Coroutine();

            });

            item.transform.Find("RedDot").gameObject.SetActive(PetInfo.petsLVpoint > 0);
            //没有宠物，只显示背景，rturn
            if (PetInfo.petId == 0)
            {
                item.transform.Find("BG").gameObject.SetActive(true);
                item.transform.Find("PetColumnBtn").gameObject.SetActive(false);
                item.transform.Find("Hint").gameObject.SetActive(false);
                item.transform.Find("PetIcon").gameObject.SetActive(false);
                item.transform.Find("restOrWar").gameObject.SetActive(false);
                item.transform.Find("Physics").gameObject.SetActive(false);
                item.transform.Find("Magic").gameObject.SetActive(false);
                item.transform.Find("Level").gameObject.SetActive(false);
                item.transform.Find("Hint").gameObject.SetActive(false);
                item.transform.Find("EnhanceLevel").gameObject.SetActive(false);
                return;
            }
            else
            {
                item.transform.Find("PetColumnBtn").gameObject.SetActive(true);
                item.transform.Find("Level").gameObject.SetActive(true);
                item.transform.Find("PetIcon").gameObject.SetActive(true);
            }


            SetEnhanceLevel(item.transform.Find("EnhanceLevel").GetComponent<Text>(), PetInfo.Elv);

            //如果当前item被点击
            if (PetInfo.Click == true)
            {
                SetPetTime(PetInfo);
                if (PetInfo.restOrWar == PetWarState.Rest)
                    GetPetsInfoRequest(PetInfo.petId).Coroutine();
                ShowPetModel(item, PetInfo);
                PetClickEvent(item, PetInfo);
              //  OpenPetsSkillRequest(PetInfo).Coroutine();
            }
            else
            {
                item.transform.Find("Hint").gameObject.SetActive(false);
            }

            //有宠物关闭BG
            //item.transform.Find("BG").gameObject.SetActive(false);
            //出战，显示选中提示框，显示出战图标
            if (PetInfo.restOrWar == PetWarState.War)
            {
                item.transform.Find("restOrWar").gameObject.SetActive(true);
                lastWarClickItem.petIcomObj = item;
                lastWarClickItem.uIPetInfo = PetInfo;
                usePetId = PetInfo.petId;
            }
            else
            {
                item.transform.Find("restOrWar").gameObject.SetActive(false);
            }
            //系别
            switch (PetInfo.petAttributeSystem)
            {
                case PetAttributeSystem.Magic:
                    item.transform.Find("Magic").gameObject.SetActive(true);
                    break;
                case PetAttributeSystem.Physics:
                    item.transform.Find("Physics").gameObject.SetActive(true);
                    break;
                default:
                    break;
            };
            //宠物等级
            item.transform.Find("Level").GetComponent<Text>().text = "LV:" + PetInfo.petLevel.ToString();
            //宠物图标
            item.transform.Find("PetIcon").GetComponent<Image>().sprite =GetSprite(PetInfo.petsConfigID);
        }
        public void SetPetTime(UIPetInfo PetInfo)
        {
            if (TimerComponent.Instance.SecondTimeToDayBool(PetInfo.petsTrialTime))
            {
                petsTrialTime.transform.Find("Text").GetComponent<Text>().text = "体验剩余时间：" + TimerComponent.Instance.SecondTimeToDay(PetInfo.petsTrialTime);
            }
            petsTrialTime.SetActive(TimerComponent.Instance.SecondTimeToDayBool(PetInfo.petsTrialTime));

            if (PetInfo.IsDeath == 1)
            {
                DidTime.transform.Find("Text").GetComponent<Text>().text = "复活时间：" + TimerComponent.Instance.MillisecondToformat(DeadTime);
                DeadTime = (int)PetInfo.deathTime;
            }
            DidTime.SetActive(PetInfo.IsDeath == 1);
            IsHaveDead = PetInfo.IsDeath == 1;
        }
        public void SetPetTime(bool IsShow)
        {
            DidTime.SetActive(IsShow);
            petsTrialTime.SetActive(IsShow);
        }
        /// <summary>
        /// 设置强化等级
        /// </summary>
        public void SetEnhanceLevel(Text leveltxt, int level)
        {
            leveltxt.gameObject.SetActive(level != 0);
            if (level == 0) return;
            leveltxt.text = "+" + level.ToString();
        }
        public async ETVoid ReviveTime()
        {
            while (true)
            {
                if (IsHaveDead)
                {
                    DeadTime -= 1000;
                    DidTime.transform.Find("Text").GetComponent<Text>().text = "复活时间：" + TimerComponent.Instance.MillisecondToformat(DeadTime);
                }
                await TimerComponent.Instance.WaitAsync(1000, cancelLogin);
            }
        }

        public void SetPetClickEvent(GameObject item, UIPetInfo PetInfo)
        {
            //宠物ID不为null
            if (PetInfo.petId != 0)
            {
                //点击的是同一个
                if (PetInfo.petId == lastClickItem.uIPetInfo.petId) return;
                //上一个点击的宠物图标不为null
                if (lastClickItem.petIcomObj != null)
                {
                    //上一个点击的宠物点击信息为false
                    lastClickItem.uIPetInfo.Click = false;
                    //隐藏上一个点击的宠物Content提示
                    lastClickItem.petIcomObj.transform.Find("Hint").gameObject.SetActive(false);
                }
                //当前点击为true
                PetInfo.Click = true;
                PetClickEvent(item, PetInfo);
            }
        }
        public void ShowPetModel(GameObject item, UIPetInfo PetInfo)
        {
            //   PetSkillAttributeTogs.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            //   ToggleEvent(AttributeSkill.Attribute, true);
            Log.DebugBrown("显示坐骑");
            // Item_MountsConfig
            Item_MountsConfig infoConfig;
            //宠物ID不为null
            if (PetInfo.petId != 0)
            {
                if (petGameObject != null && petGameObject.activeSelf) ResourcesComponent.Instance.DestoryGameObjectImmediate(petGameObject, petGameObject.name.StringToAB());//回收上一个 角色模型
                //显示宠物模型
                //PetEntity petEntity = UnitEntityFactory.CreatPet(10086 + PetInfo.petId, PetInfo.petId, AstarComponent.Instance.GetVectory3(10, 10), 0);
                infoConfig = ConfigComponent.Instance.GetItem<Item_MountsConfig>((int)PetInfo.petsConfigID);
                if (!string.IsNullOrEmpty(infoConfig.MountResName))
                {
                    string path = infoConfig.MountResName;
                    petGameObject = ResourcesComponent.Instance.LoadGameObject(path.StringToAB(), path);//显示当前角色的模型
                    if (petGameObject != null)
                    {
                        petGameObject.transform.SetParent(UnitEntityComponent.Instance.LocalRole.roleTrs.parent);
                        petGameObject.transform.eulerAngles = new Vector3(0, 140, 0);//Vector3.up * 180;
                        Vector3 pos = PetModelBase.transform.position;
                        pos.z = 94;
                        pos.y -= GetPetPosy();
                        Log.DebugBrown("宠物的y值" + infoConfig.Id);
                        //Vector3 pos = GetPetPos();
                        petGameObject.transform.localPosition = pos;
                        petGameObject.SetLayer(LayerNames.UI);
                        if (infoConfig.Id== 260012)
                        {
                            petGameObject.transform.localScale = Vector3.one * 0.1f;
                        }
                        else
                        {

                            petGameObject.transform.localScale = Vector3.one * 0.5f;
                        }
                    }
                }
            }
            float GetPetPosy() => infoConfig.Id switch
            {
                100 => 0,
                101 => 0.2f,
                102 => 0.85f,
                103 => 0.4f,
                104 => 0f,
                105 => 0.5f,
                _ => 0
            };

            //Vector3 GetPetPos() => infoConfig.Id switch
            //{
            //    100 => new Vector3(0f, 0f, 0.5f),
            //    101 => new Vector3(0f, -1.2f, -4f),
            //    102 => new Vector3(0f, -0.75f, -2.5f),
            //    103 => new Vector3(0f, -0.7f, -3.5f),
            //    104 => new Vector3(0f, 0f, 0.5f),
            //    _ => Vector3.one
            //};
        }

        public void MountClickEvent()
        {
            if (UseMountData.UId == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "未获取坐骑数据");
                return;
            }
            Item_MountsConfig mounts_Info = ConfigComponent.Instance.GetItem<Item_MountsConfig>((int)UseMountData.MountId);
            PetNameTxt.text = mounts_Info.Name+"   阶级:"+UseMountData.AdvancedLevel;
            //设置宠物等级
            PetLevelTxt.text = "LV:" + UseMountData.Fortifiedlevel;
        }
        public void PetClickEvent(GameObject item = null, UIPetInfo PetInfo = null)
        {


          //  Item_MountsConfig mounts_Info = ConfigComponent.Instance.GetItem<Item_MountsConfig>((int)UseMountData.MountId);
          //  //  SkillListClear();
          //  //设置提示
          //  if (item != null)
          //      item.transform.Find("Hint").gameObject.SetActive(true);
          //  //设置宠物名字
          //  PetNameTxt.text = mounts_Info.Name;
          //  //设置宠物等级
          //  PetLevelTxt.text = "LV:" + UseMountData.Fortifiedlevel;
          //  //设置宠物经验条
          //  ExpSlider.transform.Find("ExpSliderValue").GetComponent<Image>().fillAmount =
          //      (float)(PetInfo.curExp / PetInfo.curHighestExp);
          //  //设置宠物经验文本提示
          //  ExpSlider.transform.Find("ExpTxt").GetComponent<Text>().text = $"{PetInfo.curExp} / {PetInfo.curHighestExp}";
          //  //设置上一个点击的信息为当前信息
          //  if (item != null)
          //  {
          //      lastClickItem.petIcomObj = item;
          //      lastClickItem.uIPetInfo = PetInfo;
          //  }
          //  //设置宠物属性
          ////  SetPetAttribute(PetInfo);
          //  petLeftCollector.GetText("NoPetHint").gameObject.SetActive(PetInfo.petId == 0);
        }

        /// <summary>
        /// 设置宠物提示
        /// </summary>
        /// <param name="petWarState"></param>
        public void SetPetIconHint(PetWarState petWarState)
        {

            switch (petWarState)
            {
                case PetWarState.War:

                    for (int i = 0; i < PetList.Count; i++)
                    {
                        if (lastWarClickItem.uIPetInfo != null)
                        {
                            if (PetList[i].petId == lastWarClickItem.uIPetInfo.petId)
                            {
                                PetList[i].restOrWar = PetWarState.Rest;
                            }
                        }

                        if (PetList[i].petId == lastClickItem.uIPetInfo.petId)
                        {
                            PetList[i].restOrWar = PetWarState.War;
                        }
                    }
                    //
                    if (lastWarClickItem.petIcomObj != null)
                    {
                        lastWarClickItem.petIcomObj.transform.Find("restOrWar").gameObject.SetActive(false);

                    }
                    lastWarClickItem.uIPetInfo = lastClickItem.uIPetInfo;
                    lastWarClickItem.petIcomObj = lastClickItem.petIcomObj;
                    lastClickItem.petIcomObj.transform.Find("restOrWar").gameObject.SetActive(true);

                    //uICircular_Pet.Items = PetList;
                    break;
                case PetWarState.Rest:
                    for (int i = 0; i < PetList.Count; i++)
                    {
                        if (lastWarClickItem.uIPetInfo != null)
                        {
                            if (PetList[i].petId == lastWarClickItem.uIPetInfo.petId)
                            {
                                PetList[i].restOrWar = PetWarState.Rest;
                            }
                        }
                    }
                    if (lastWarClickItem.petIcomObj != null)
                    {
                        lastWarClickItem.petIcomObj.transform.Find("restOrWar").gameObject.SetActive(false);
                        lastWarClickItem.uIPetInfo = null;
                        lastWarClickItem.petIcomObj = null;
                    }
                    break;
                default:
                    break;
            }

        }

    }
}

