
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public ReferenceCollector zhuoyueCollector;
        public Button ZhuoyueBtn, RefineBtn;
        public Button ZhuoyueCloseBtn,Btn_Close;
        public GameObject ZhuoyueContent, _bg;
        public string Outstanding;
        public void InitPetZhuoyue()
        {
            zhuoyueCollector = petRightCollector.GetGameObject("Zhuoyue").GetReferenceCollector();
            ZhuoyueBtn = zhuoyueCollector.GetButton("ZhuoyueBtn");
            RefineBtn = zhuoyueCollector.GetButton("RefineBtn");
            Btn_Close = zhuoyueCollector.GetButton("Btn_Close");
            ZhuoyueCloseBtn = zhuoyueCollector.GetButton("ZhuoyuePanel");
            ZhuoyueContent = zhuoyueCollector.GetImage("Content").gameObject;
            _bg = zhuoyueCollector.GetImage("_bg").gameObject;
            _bg.transform.Find("Btn_Refine").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                RefinePetsInfoRequest(lastClickItem.uIPetInfo.petId).Coroutine();
            });
            ZhuoyueContent.SetActive(false);
            Btn_Close.onClick.AddSingleListener(() =>
            {

                
                _bg.gameObject.SetActive(false);
            });
            RefineBtn.onClick.AddSingleListener(() =>
            {
                InitPetsExcellentInfoRequest(lastClickItem.uIPetInfo.petId).Coroutine();
                _bg.gameObject.SetActive(true);

            });
            ZhuoyueBtn.onClick.AddSingleListener(() =>
            {
                if (roleEntity.Level < levelLimit)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    return;
                }
                if (PetList.Count > 0)
                {
                    ZhuoyueCloseBtn.gameObject.SetActive(true);
                    ZhuoyueContent.SetActive(true);
                    GetPetsExcellentInfoRequest(lastClickItem.uIPetInfo.petId).Coroutine();
                }
                
            });
            ZhuoyueCloseBtn.GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                ZhuoyueContent.SetActive(false);
                ZhuoyueCloseBtn.gameObject.SetActive(false);
                for (int i = 0; i < ZhuoyueContent.transform.childCount; i++)
                {
                    ZhuoyueContent.transform.GetChild(i).gameObject.SetActive(false);
                }
            });
            ZhuoyueCloseBtn.gameObject.SetActive(false);
        }


        /// <summary>
        /// 请求卓越
        /// </summary>
        /// <param name="petId"></param>
        /// <returns></returns>
        private async ETTask RefinePetsInfoRequest(long petId)
        {
            G2C_PetRefiningResponse g2C_GetPetsExcellent = (G2C_PetRefiningResponse)await SessionComponent.Instance.Session.Call(new C2G_PetRefiningRequest()
            {
                PetsID = petId
            });

            Log.DebugBrown("返回卓越属性" + JsonHelper.ToJson(g2C_GetPetsExcellent));
            if (g2C_GetPetsExcellent.Error == 0)
            {
                Outstanding = "";
                for (int i = 0; i < g2C_GetPetsExcellent.ExcellentList.Count; i++)
                {
                    ItemAttrEntry_ExcConfig petConfig = ConfigComponent.Instance.GetItem<ItemAttrEntry_ExcConfig>(g2C_GetPetsExcellent.ExcellentList[i]);
                    if (petConfig != null)
                    {
                        Outstanding += $"{petConfig.Name + '\n'}"; ;
                    }
                }
            }
            else
            {

               UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetPetsExcellent.Error.GetTipInfo());
            }
            RefreshRefine();
        }

        /// <summary>
        /// 初始化获取卓越属性
        /// </summary>
        /// <param name="petId"></param>
        /// <returns></returns>
        private async ETTask InitPetsExcellentInfoRequest(long petId)
        {
            G2C_GetPetsExcellentInfoResponse g2C_GetPetsExcellent = (G2C_GetPetsExcellentInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_GetPetsExcellentInfoRequest()
            {
                PetsID = petId
            });
            Log.DebugBrown("初始化数据" + JsonHelper.ToJson(g2C_GetPetsExcellent));
            if (g2C_GetPetsExcellent.Error == 0)
            {
                Outstanding = "";
                for (int i = 0; i < g2C_GetPetsExcellent.ExcellentList.Count; i++)
                {
                    ItemAttrEntry_ExcConfig petConfig = ConfigComponent.Instance.GetItem<ItemAttrEntry_ExcConfig>(g2C_GetPetsExcellent.ExcellentList[i]);
                    if (petConfig != null)
                    {
                        // Text text = ZhuoyueContent.transform.Find($"{i + 1}").GetComponent<Text>();
                        Outstanding += $"{petConfig.Name + '\n'}"; ;
                    }

                }
            }
            else
            {
                //UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetPetsExcellent.Error.GetTipInfo());
            }
            Log.DebugBrown("打印" + Outstanding);
           // _bg.transform.Find("des1").GetComponent<Text>().text = Outstanding;
             RefreshRefine();
        }

        /// <summary>
        /// 精炼界面
        /// </summary>
        private void RefreshRefine()
        {
      
            _bg.transform.Find("des1").GetComponent<Text>().text = Outstanding;
        }

        public async ETTask GetPetsExcellentInfoRequest(long petId)
        {
            G2C_GetPetsExcellentInfoResponse g2C_GetPetsExcellent = (G2C_GetPetsExcellentInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_GetPetsExcellentInfoRequest()
            {
                PetsID = petId
            });
            if (g2C_GetPetsExcellent.Error == 0)
            {
                ZhuoyueContent.SetActive(true);
                ZhuoyueContent.transform.Find($"{0}").GetComponent<Text>().gameObject.SetActive(true);
                for (int i = 0; i < g2C_GetPetsExcellent.ExcellentList.Count; i++)
                {
                    ItemAttrEntry_ExcConfig petConfig = ConfigComponent.Instance.GetItem<ItemAttrEntry_ExcConfig>(g2C_GetPetsExcellent.ExcellentList[i]);
                    if(petConfig != null)
                    {
                        Text text = ZhuoyueContent.transform.Find($"{i + 1}").GetComponent<Text>();
                        text.gameObject.SetActive(true);
                        text.text = petConfig.Name;
                    }
                   
                }
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetPetsExcellent.Error.GetTipInfo());
            }
        }
    }

}
