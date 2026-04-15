using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public enum PetFoodType
    {
        Chuji,
        Zhongji,
        Gaoji
    }
    public partial class UIPetComponent
    {
        public GameObject PetExpColumn;
        public GameObject ExpPanelSlider;
        public GameObject GaoJiExp;
        public GameObject ZhongJiExp;
        public GameObject ChujiExp;
        public Text PetExpLevelTxt;
        public Text PetExpNameTxt;

        public GameObject chujiObj, zhongjiObj, gaojiObj;
        public Dictionary<PetFoodType, UIPetExpInfo> uIPetExpInfos = new Dictionary<PetFoodType, UIPetExpInfo>();
        public RoleEntity roleEntity => UnitEntityComponent.Instance.LocalRole;//本地玩家
        public void InitUseExpPanel()
        {
            PetExpColumn = expCollector.GetGameObject("PetColumn");
            ExpPanelSlider = expCollector.GetImage("ExpSlider").gameObject;
            GaoJiExp = expCollector.GetImage("GaoJiExp").gameObject;
            ZhongJiExp = expCollector.GetImage("ZhongJiExp").gameObject;
            ChujiExp = expCollector.GetImage("ChujiExp").gameObject;
            PetExpLevelTxt = expCollector.GetText("PetLevelTxt");
            PetExpNameTxt = expCollector.GetText("PetNameTxt");
            ExpRegisterDragEvents();
            InitButton();
            LoadYaoPingObj();

        }
        public void InitButton()
        {
            expCollector.GetButton("CloseBtn").onClick.AddSingleListener(delegate ()
            {
                HindYaoPingObj();
                //GetPetsInfoRequest(lastClickItem.uIPetInfo.petId).Coroutine();
                InitPetList().Coroutine();
                expCollector.gameObject.SetActive(false);
            });
            ChujiExp.transform.Find("ChuJiUseBtn").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (uIPetExpInfos.ContainsKey(PetFoodType.Chuji) && uIPetExpInfos[PetFoodType.Chuji].petsItem.ItemCnt > 0)
                {
                    
                    UIPetExpInfo uIPetExpInfo = uIPetExpInfos[PetFoodType.Chuji];
                    UseItemAddExpRequest(lastClickItem.uIPetInfo.petId, uIPetExpInfo.petsItem.ItemID).Coroutine();
                }
                else
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"初级宠物食材不足");
                }

            });
            ZhongJiExp.transform.Find("ZhongJiUseBtn").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (uIPetExpInfos.ContainsKey(PetFoodType.Zhongji) && uIPetExpInfos[PetFoodType.Zhongji].petsItem.ItemCnt > 0)
                {
                  
                    UIPetExpInfo uIPetExpInfo1 = uIPetExpInfos[PetFoodType.Zhongji];
                    UseItemAddExpRequest(lastClickItem.uIPetInfo.petId, uIPetExpInfo1.petsItem.ItemID).Coroutine();
                }
                else
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"中级宠物食材不足");
                }
            });
            GaoJiExp.transform.Find("GaoJiUseBtn").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (uIPetExpInfos.ContainsKey(PetFoodType.Gaoji) && uIPetExpInfos[PetFoodType.Gaoji].petsItem.ItemCnt > 0)
                {
                  
                    UIPetExpInfo uIPetExpInfo2 = uIPetExpInfos[PetFoodType.Gaoji];
                    UseItemAddExpRequest(lastClickItem.uIPetInfo.petId, uIPetExpInfo2.petsItem.ItemID).Coroutine();
                }
                else
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"高级宠物食材不足");
                }
            });
        }
        /// <summary>
        /// 只有3个，直接加载
        /// </summary>
        public void LoadYaoPingObj()
        {
            if (chujiObj == null)
            {
                chujiObj = ResourcesComponent.Instance.LoadGameObject("chujichongwushicai".StringToAB(), "chujichongwushicai");//显示当前角色的模型
                Vector3 chujiPos = new Vector3(-2.39f, -1.1f, 85f);//ChujiExp.transform.Find("Frame").transform.Find("ChujiFood").transform.position;
                chujiObj.transform.position = chujiPos;
                chujiObj.SetLayer(LayerNames.UI);
                chujiObj.SetActive(false);

            }
            if (zhongjiObj == null)
            {

                zhongjiObj = ResourcesComponent.Instance.LoadGameObject("zhongjichongwushicai".StringToAB(), "zhongjichongwushicai");//显示当前角色的模型
                Vector3 zhongjiPos = new Vector3(0.22f, -1.1f, 85f);//ZhongJiExp.transform.Find("Frame").transform.Find("ZhongJiFood").transform.position;
                zhongjiObj.transform.position = zhongjiPos;
                zhongjiObj.SetLayer(LayerNames.UI);
                zhongjiObj.SetActive(false);
            }
            if (gaojiObj == null)
            {
                gaojiObj = ResourcesComponent.Instance.LoadGameObject("gaojichongwushicai".StringToAB(), "gaojichongwushicai");//显示当前角色的模型
                Vector3 gaojiPos = new Vector3(2.84f, -1.1f, 85f);//GaoJiExp.transform.Find("Frame").transform.Find("GaoJiFood").transform.position;
                gaojiObj.transform.position = gaojiPos;
                gaojiObj.SetLayer(LayerNames.UI);
                gaojiObj.SetActive(false);

            }
        }
        public void HindYaoPingObj()
        {
            if (chujiObj != null) chujiObj.SetActive(false);
            if (zhongjiObj != null) zhongjiObj.SetActive(false);
            if (gaojiObj != null) gaojiObj.SetActive(false);
        }
        public bool isDragChuji, isDragZhongji, isDragGaoji;
        public Vector3 lastRatePosChuji, lastRatePosZhongji, lastRatePosGaoji;
        public void ExpRegisterDragEvents()
        {
            GameObject drag = expCollector.GetImage("ChujiDragPetEvent").gameObject;
            UGUITriggerProxy proxy1 = drag.GetComponent<UGUITriggerProxy>();
            proxy1.OnBeginDragEvent = () =>
            {
                isDragChuji = true;
                lastRatePosChuji = Input.mousePosition;
            };
            proxy1.OnEndDragEvent += () =>
            {
                isDragChuji = false;
                lastRatePosChuji = Vector3.zero;
            };

            
            drag = expCollector.GetImage("ZhongjiDragPetEvent").gameObject;
            UGUITriggerProxy proxy2 = drag.GetComponent<UGUITriggerProxy>();
            proxy2.OnBeginDragEvent = () =>
            {
                isDragZhongji = true;
                lastRatePosZhongji = Input.mousePosition;
            };
            proxy2.OnEndDragEvent += () =>
            {
                isDragZhongji = false;
                lastRatePosZhongji = Vector3.zero;
            };

            
            drag = expCollector.GetImage("GaojiDragPetEvent").gameObject;
            UGUITriggerProxy proxy3 = drag.GetComponent<UGUITriggerProxy>();
            proxy3.OnBeginDragEvent = () =>
            {
                isDragGaoji = true;
                lastRatePosGaoji = Input.mousePosition;
            };
            proxy3.OnEndDragEvent += () =>
            {
                isDragGaoji = false;
                lastRatePosGaoji = Vector3.zero;
            };
        }

        public async ETTask UseItemAddExpRequest(long petsID,long itemID)
        {
            G2C_UseItemAddExpResponse g2C_OpenPets = (G2C_UseItemAddExpResponse)await SessionComponent.Instance.Session.Call(new C2G_UseItemAddExpRequest()
            {
                PetsID = petsID,
                ItemID = itemID
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                SetPetList(g2C_OpenPets.Info, lastClickItem.uIPetInfo);
                SetExpPanelInfo(lastClickItem.uIPetInfo).Coroutine();
                RefeachItem();
            }
        }
        public async ETTask SetExpPanelInfo(UIPetInfo uIPetInfo)
        {
          
            G2C_OpenItemInterfaceResponse g2C_OpenPets = (G2C_OpenItemInterfaceResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenItemInterfaceRequest(){ });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {

                uIPetExpInfos.Clear();

                expCollector.gameObject.SetActive(true);
                chujiObj.transform.rotation = Quaternion.Euler(-120, 0, 0);
                zhongjiObj.transform.rotation = Quaternion.Euler(-120, 0, 0);
                gaojiObj.transform.rotation = Quaternion.Euler(-120, 0, 0);

                chujiObj.SetActive(true);
                zhongjiObj.SetActive(true);
                gaojiObj.SetActive(true);

                PetExpColumn.transform.Find("PetIcon").GetComponent<Image>().sprite = uIPetInfo.sprite;
                PetExpColumn.transform.Find("restOrWar").gameObject.SetActive(uIPetInfo.restOrWar == PetWarState.War);

                PetExpColumn.transform.Find("Magic").gameObject.SetActive(uIPetInfo.petAttributeSystem == PetAttributeSystem.Magic);
                PetExpColumn.transform.Find("Physics").gameObject.SetActive(uIPetInfo.petAttributeSystem == PetAttributeSystem.Physics);

                PetExpColumn.transform.Find("Level").GetComponent<Text>().text = uIPetInfo.petLevel.ToString();

                PetExpNameTxt.text = uIPetInfo.petName;
                PetExpLevelTxt.text = $"等级:   {uIPetInfo.petLevel}/{roleEntity.Level}";
                ExpPanelSlider.transform.Find("Image").GetComponent<Image>().fillAmount = (float)uIPetInfo.curExp / uIPetInfo.curHighestExp;
                ExpPanelSlider.transform.Find("ExpTxt").GetComponent<Text>().text = $"{uIPetInfo.curExp} / {uIPetInfo.curHighestExp}";

                

                foreach (var item in g2C_OpenPets.List)
                {
                    UIPetExpInfo uIPetExpInfo = new UIPetExpInfo();
                    uIPetExpInfo.petsItem.ItemConfingID = item.ItemConfingID;
                    uIPetExpInfo.petsItem.ItemCnt = item.ItemCnt;
                    uIPetExpInfo.petsItem.ItemID = item.ItemID;
                    PetFoodIconInit(uIPetExpInfo);
                    switch (uIPetExpInfo.Name)
                    {
                        case "初级宠物食材":
                            ChujiExp.transform.Find("Frame").transform.Find("ChujiCountTxt").GetComponent<Text>().text = $"{uIPetExpInfo.petsItem.ItemCnt}";
                           
                            uIPetExpInfos.Add(PetFoodType.Chuji,uIPetExpInfo);
                            break;
                        case "中级宠物食材":
                            ZhongJiExp.transform.Find("Frame").transform.Find("ZhongjiCountTxt").GetComponent<Text>().text = $"{uIPetExpInfo.petsItem.ItemCnt}";
                           
                            uIPetExpInfos.Add(PetFoodType.Zhongji, uIPetExpInfo);
                            break;
                        case "高级宠物食材":
                            GaoJiExp.transform.Find("Frame").transform.Find("GaojiCountTxt").GetComponent<Text>().text = $"{uIPetExpInfo.petsItem.ItemCnt}";
                            
                            uIPetExpInfos.Add(PetFoodType.Gaoji, uIPetExpInfo);
                            break;
                        default:
                            break;
                    }
                }

            }


        }
        //从物品表获取宠物食材信息
        public void PetFoodIconInit(UIPetExpInfo uIPetExpInfo)
        {
            Item_ConsumablesConfig petSkillConfig = ConfigComponent.Instance.GetItem<Item_ConsumablesConfig>(uIPetExpInfo.petsItem.ItemConfingID);
            if (petSkillConfig != null)
            {
                uIPetExpInfo.petfoodAsset = petSkillConfig.ResName;
                uIPetExpInfo.Name = petSkillConfig.Name;
                uIPetExpInfo.addExpValue = petSkillConfig.Value;
            }
        }

        public void ExpUpdate()
        {
            if (chujiObj != null && isDragChuji && lastRatePosChuji.x != Input.mousePosition.x && chujiObj.activeSelf)
            {
                float offPos_x = lastRatePosChuji.x - Input.mousePosition.x;
                chujiObj.transform.Rotate( 0, 0, offPos_x, Space.Self);
                lastRatePosChuji = Input.mousePosition;
            }
            if (zhongjiObj != null && isDragZhongji && lastRatePosZhongji.x != Input.mousePosition.x && zhongjiObj.activeSelf)
            {
                float offPos_x = lastRatePosZhongji.x - Input.mousePosition.x;
                zhongjiObj.transform.Rotate(0, 0, offPos_x, Space.Self);
                lastRatePosZhongji = Input.mousePosition;
            }
            if (gaojiObj != null && isDragGaoji && lastRatePosGaoji.x != Input.mousePosition.x && gaojiObj.activeSelf)
            {
                float offPos_x = lastRatePosGaoji.x - Input.mousePosition.x;
                gaojiObj.transform.Rotate(0, 0, offPos_x, Space.Self);
                lastRatePosGaoji = Input.mousePosition;
            }
        }

    }

}
