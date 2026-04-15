using ETModel;
using NPOI.Util;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIPetNewComponentUpdate : UpdateSystem<UIPetNewComponent>
    {
        public override void Update(UIPetNewComponent self)
        {
            if (self.petGameObject != null && self.isDrag && self.lastRatePos.x != Input.mousePosition.x && self.petGameObject.activeSelf)
            {

                float offPos_x = self.lastRatePos.x - Input.mousePosition.x;
                self.petGameObject.transform.Rotate(0, offPos_x, 0);
                self.lastRatePos = Input.mousePosition;
            }
        }
    }
    public partial class UIPetNewComponent
    {
        CancellationTokenSource tokenSource;
        CancellationToken cancelLogin;
        public GameObject petGameObject = null;
        public bool isDrag = false;//是否开始旋转角色
        public Vector3 lastRatePos;//上一次旋转的为位置
        public ReferenceCollector petLeftCollector;
        public UICircularScrollView<UIPetNewInfo> uICircular_Pet;
        GameObject petICons;
        GameObject RenderTextConfig;
        public Text PetNameTxt;
        public GameObject PetContent;
        public GameObject PetShowModel;
        public GameObject PetModelBase;
        public ScrollRect PetView;
        public Transform renderTexture;
        public GameObject petsTrialTime;

        public GameObject lastClickItem;
        public UIPetNewInfo lastClickInfo;
        public GameObject last2ClickItem;
        public UIPetNewInfo last2ClickInfo;
        public void InitLeft()
        {
            tokenSource = new CancellationTokenSource();
            cancelLogin = tokenSource.Token;
            petICons = ResourcesComponent.Instance.LoadGameObject("Pet_Icon".StringToAB(), "Pet_Icon");
            RenderTextConfig = ResourcesComponent.Instance.LoadGameObject("RenderTxtureConfig".StringToAB(), "RenderTxtureConfig");
            petLeftCollector = petCollector.GetImage("PetLeft").gameObject.GetReferenceCollector();
            PetNameTxt = petLeftCollector.GetText("PetNameTxt");
            PetContent = petLeftCollector.GetGameObject("Content");
            PetShowModel = petLeftCollector.GetGameObject("PetShowModel");
            PetModelBase = petLeftCollector.GetGameObject("PetModelBase");
            PetView = petLeftCollector.GetImage("Scroll View").gameObject.GetComponent<ScrollRect>();
            renderTexture = petLeftCollector.GetGameObject("PetShowModel").transform;
            petsTrialTime = petLeftCollector.GetImage("SetPetTime").transform.Find("petsTrialTime").gameObject;
            RegisterDragEvent();
            InitUICircular_Pet();
            InitPetNewList().Coroutine();
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
            uICircular_Pet = ComponentFactory.Create<UICircularScrollView<UIPetNewInfo>>();
            uICircular_Pet.InitInfo(E_Direction.Vertical, 5, 8, 30);
            uICircular_Pet.ItemInfoCallBack = InitPetItem;
            //uICircular_Pet.ItemClickCallBack = PetClickEvent;
            uICircular_Pet.IninContent(PetContent, PetView);
        }
        /// <summary>
        /// 初始化宠物信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="PetInfo"></param>
        private void InitPetItem(GameObject item, UIPetNewInfo PetInfo)
        {
            item.transform.Find("Hint").gameObject.SetActive(false);
            item.transform.Find("restOrWar").gameObject.SetActive(PetInfo.IsWar);
            if (PetInfo.IsClick == true)
            {
                last2ClickInfo = PetInfo;
                last2ClickItem = item;
                lastClickItem = item;
                lastClickInfo = PetInfo;
                item.transform.Find("Hint").gameObject.SetActive(true);
                ShowPetModel(item, PetInfo);
                PetClickEvent(item, PetInfo);
                SetAttribute(PetInfo);
                SetBtnActive((PetInfo.IsWar ? PetWarState.War: PetWarState.Rest));
            }
            item.transform.Find("PetColumnBtn").gameObject.GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                //Log.DebugGreen($"{PetInfo.Name}");
                lastClickItem = item;
                lastClickInfo = PetInfo;
                if (lastClickInfo.newPetsInfo.PetsID != last2ClickInfo.newPetsInfo.PetsID)
                {
                    last2ClickItem.transform.Find("Hint").gameObject.SetActive(false);
                    item.transform.Find("Hint").gameObject.SetActive(true);
                    last2ClickInfo = PetInfo;
                    last2ClickItem = item;
                    //切换宠物
                    PetClickEvent(item, PetInfo);
                    ShowPetModel(item, PetInfo);
                    SetAttribute(PetInfo);
                    SetBtnActive((PetInfo.IsWar ? PetWarState.War : PetWarState.Rest));
                }
            });

            //宠物图标
            item.transform.Find("PetIcon").GetComponent<Image>().sprite = PetInfo.sprite;
        }
        public void ShowPetModel(GameObject item, UIPetNewInfo PetInfo)
        {
            PetSkillAttributeTogs.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            ToggleEvent(AttributeSkill.Attribute, true);

            Pets_InfoConfig infoConfig;
            //宠物ID不为null
            if (PetInfo.newPetsInfo.PetsID != 0)
            {
                if (petGameObject != null && petGameObject.activeSelf) ResourcesComponent.Instance.DestoryGameObjectImmediate(petGameObject, petGameObject.name.StringToAB());//回收上一个 角色模型
                //显示宠物模型
                //PetEntity petEntity = UnitEntityFactory.CreatPet(10086 + PetInfo.petId, PetInfo.petId, AstarComponent.Instance.GetVectory3(10, 10), 0);
                infoConfig = ConfigComponent.Instance.GetItem<Pets_InfoConfig>((int)PetInfo.newPetsInfo.PetsConfigID);
                if (!string.IsNullOrEmpty(infoConfig.PetModleAsset))
                {
                    string path = infoConfig.PetModleAsset + "_UI";
                    petGameObject = ResourcesComponent.Instance.LoadGameObject(path.StringToAB(), path);//显示当前角色的模型
                    if (petGameObject != null)
                    {
                        petGameObject.transform.SetParent(UnitEntityComponent.Instance.LocalRole.roleTrs.parent);
                        petGameObject.transform.eulerAngles = new Vector3(0, 140, 0);//Vector3.up * 180;
                        Vector3 pos = PetModelBase.transform.position;
                        pos.z = 94;
                        pos.y -= GetPetPosy();
                        //Vector3 pos = GetPetPos();
                        petGameObject.transform.localPosition = pos;
                        petGameObject.SetLayer(LayerNames.UI);
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
                106 => -2f,
                _ => 0
            };
        }
        public void PetClickEvent(GameObject item = null, UIPetNewInfo PetInfo = null)
        {
            //设置提示
            if (item != null)
                item.transform.Find("Hint").gameObject.SetActive(true);
            //设置宠物名字
            if (PetInfo == null) return;
            PetNameTxt.text = PetInfo.Name;
            if (TimerComponent.Instance.SecondTimeToDayBool(PetInfo.petsTrialTime))
            {
                petsTrialTime.transform.Find("Text").GetComponent<Text>().text = "体验剩余时间：" + TimerComponent.Instance.SecondTimeToDay(PetInfo.petsTrialTime);
            }
            else
            {
                petsTrialTime.transform.Find("Text").GetComponent<Text>().text = null;
            }
        }

    }

}
