using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public GameObject EnhanceLevelValue, XiaoHaoValue;
        public Text AddAttributeOne, AddAttributeTwo, AddAttributeThree, SuccessRateValue;
        public Button QiangHuaBtn;
        public GameObject oneObj, TwoObj, ThreeObj;
        public void EnhanceInit()
        {
            EnhanceLevelValue = enhanceCollector.GetGameObject("EnhanceLevelValue");
            XiaoHaoValue = enhanceCollector.GetGameObject("XiaoHaoValue");
            AddAttributeOne = enhanceCollector.GetText("AddAttributeOne");
            AddAttributeTwo = enhanceCollector.GetText("AddAttributeTwo");
            AddAttributeThree = enhanceCollector.GetText("AddAttributeThree");
            SuccessRateValue = enhanceCollector.GetText("SuccessRateValue");
            enhanceCollector.GetButton("QiangHuaBtn").onClick.AddSingleListener(() =>
            {
                PetsEnhanceRequest(lastClickItem.uIPetInfo.petId).Coroutine();
            });
        }
        public async ETVoid SetEnhanceAtrribe(UIPetInfo uIPetInfo)
        {
            G2C_OpenPetsEnhanceResponse g2C_OpenPetsEnhance = (G2C_OpenPetsEnhanceResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenPetsEnhanceRequest()
            {
                PetsID = uIPetInfo.petId
            }); 
            if(g2C_OpenPetsEnhance.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPetsEnhance.Error.GetTipInfo());
            }
            else
            {
                HindEnhanceCaiLiao();
                //Log.DebugGreen($"宠物强化等级{g2C_OpenPetsEnhance.Lv}");
                CurLevel = g2C_OpenPetsEnhance.Lv;
                enhanceCollector.GetButton("QiangHuaBtn").interactable = (CurLevel < 15 ? true : false);
                EnhanceLevelValue.transform.Find("NoMax").gameObject.SetActive(CurLevel < 15);
                EnhanceLevelValue.transform.Find("MaxValue").gameObject.SetActive(CurLevel == 15);
                if (CurLevel < 15)
                {
                    EnhanceLevelValue.transform.Find("NoMax/NowValue").GetComponent<Text>().text = g2C_OpenPetsEnhance.Lv.ToString();
                    EnhanceLevelValue.transform.Find("NoMax/AfterValue").GetComponent<Text>().text = (g2C_OpenPetsEnhance.Lv + 1).ToString();
                }
                else
                {
                    EnhanceLevelValue.transform.Find("MaxValue").GetComponent<Text>().text = (g2C_OpenPetsEnhance.Lv).ToString();
                }
                AddAttributeOne.text = CurLevel < 15 ? $"生命值+{g2C_OpenPetsEnhance.Lv + 1}%" : $"生命值+{g2C_OpenPetsEnhance.Lv}%";
                AddAttributeTwo.text = uIPetInfo.petAttributeSystem == PetAttributeSystem.Physics ? (CurLevel < 15 ? $"攻击力+{g2C_OpenPetsEnhance.Lv + 1}%" : $"攻击力+{g2C_OpenPetsEnhance.Lv}%") : (CurLevel < 15 ? $"魔力+{g2C_OpenPetsEnhance.Lv + 1}%" : $"魔力+{g2C_OpenPetsEnhance.Lv}%");
                AddAttributeThree.text = CurLevel < 15 ? $"防御力+{g2C_OpenPetsEnhance.Lv + 1}%" : $"防御力+{g2C_OpenPetsEnhance.Lv}%";
                //SuccessRateValue.gameObject.SetActive(CurLevel < 15);
                //enhanceCollector.GetText("SuccessRate").gameObject.SetActive(CurLevel < 15);
                //enhanceCollector.GetText("XiaoHao").gameObject.SetActive(CurLevel < 15);
                enhanceCollector.GetText("SuccessRate").text = CurLevel < 15 ? "成功率" : null;
                enhanceCollector.GetText("XiaoHao").text = CurLevel < 15 ? "消耗材料" : null;
                if (CurLevel < 15)
                    SuccessRateValue.text = g2C_OpenPetsEnhance.Lv != 0 ? (g2C_OpenPetsEnhance.Lv <= 5 ? $"{100 - g2C_OpenPetsEnhance.Lv * 10}%（失败强化等级减1）" : "50%（失败强化等级归0）") : "100%";
                else
                    SuccessRateValue.text = null;
                XiaoHaoValue.transform.Find("One").GetComponent<Text>().text = null;
                if (g2C_OpenPetsEnhance.EnhanceMaterials.Count > 0)
                {
                    XiaoHaoValue.transform.Find("One").GetComponent<Text>().text = $"{g2C_OpenPetsEnhance.EnhanceMaterials[0].ItemID}/{g2C_OpenPetsEnhance.EnhanceMaterials[0].ItemCnt}";
                    Item_GemstoneConfig fGemstoneConfig = ConfigComponent.Instance.GetItem<Item_GemstoneConfig>(g2C_OpenPetsEnhance.EnhanceMaterials[0].ItemConfingID);
                    oneObj = ResourcesComponent.Instance.LoadGameObject(fGemstoneConfig.ResName.StringToAB(), fGemstoneConfig.ResName);//显示当前角色的模型
                    oneObj.transform.position = XiaoHaoValue.transform.Find("One").position - new Vector3(0.75f, 0, 0);
                }
                XiaoHaoValue.transform.Find("Two").GetComponent<Text>().text = null;
                if (g2C_OpenPetsEnhance.EnhanceMaterials.Count > 1)
                {
                    XiaoHaoValue.transform.Find("Two").GetComponent<Text>().text = $"{g2C_OpenPetsEnhance.EnhanceMaterials[1].ItemID}/{g2C_OpenPetsEnhance.EnhanceMaterials[1].ItemCnt}";
                    Item_GemstoneConfig fGemstoneConfig = ConfigComponent.Instance.GetItem<Item_GemstoneConfig>(g2C_OpenPetsEnhance.EnhanceMaterials[1].ItemConfingID);
                    TwoObj = ResourcesComponent.Instance.LoadGameObject(fGemstoneConfig.ResName.StringToAB(), fGemstoneConfig.ResName);//显示当前角色的模型
                    TwoObj.transform.position = XiaoHaoValue.transform.Find("Two").position - new Vector3(0.75f, 0, 0);
                }
                XiaoHaoValue.transform.Find("Three").GetComponent<Text>().text = null;
                if (g2C_OpenPetsEnhance.EnhanceMaterials.Count > 2)
                {
                    XiaoHaoValue.transform.Find("Three").GetComponent<Text>().text = $"{g2C_OpenPetsEnhance.EnhanceMaterials[2].ItemID}/{g2C_OpenPetsEnhance.EnhanceMaterials[2].ItemCnt}";
                    Item_GemstoneConfig fGemstoneConfig = ConfigComponent.Instance.GetItem<Item_GemstoneConfig>(g2C_OpenPetsEnhance.EnhanceMaterials[2].ItemConfingID);
                    ThreeObj = ResourcesComponent.Instance.LoadGameObject(fGemstoneConfig.ResName.StringToAB(), fGemstoneConfig.ResName);//显示当前角色的模型
                    ThreeObj.transform.position = XiaoHaoValue.transform.Find("Three").position - new Vector3(0.75f, 0, 0);
                }
            }
        }
        public void HindEnhanceCaiLiao()
        {
            if (oneObj != null) ResourcesComponent.Instance.DestoryGameObjectImmediate(oneObj, oneObj.name.StringToAB());
            if (TwoObj != null) ResourcesComponent.Instance.DestoryGameObjectImmediate(TwoObj, TwoObj.name.StringToAB());
            if (ThreeObj != null) ResourcesComponent.Instance.DestoryGameObjectImmediate(ThreeObj, ThreeObj.name.StringToAB());
        }
        public int CurLevel = 0;
        public async ETVoid PetsEnhanceRequest(long id)
        {
            
            G2C_PetsEnhanceResponse c_PetsEnhanceResponse = (G2C_PetsEnhanceResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsEnhanceRequest()
            {
                PetsID = id
            });
            Log.DebugBrown("请求宠物强化" + id+":数据"+JsonHelper.ToJson(c_PetsEnhanceResponse));
            if (c_PetsEnhanceResponse.Error == 0)
            {
                CurLevel += 1;
                UIComponent.Instance.VisibleUI(UIType.UIHint, "强化成功！");
                SetEnhanceLevel(lastClickItem.petIcomObj.transform.Find("EnhanceLevel").GetComponent<Text>(), CurLevel);
                SetEnhanceAtrribe(lastClickItem.uIPetInfo).Coroutine();
                uIPetInfo = GetPetInfoToId(uIPetInfo.petId);
                SetPetList(c_PetsEnhanceResponse.Info, uIPetInfo);
                SetPetAttribute(uIPetInfo);
            }
            else if (c_PetsEnhanceResponse.Error == 1611)
            {
                if(CurLevel <= 5)
                {
                    CurLevel -= 1;
                }
                else
                {
                    CurLevel =0;
                }
                UIComponent.Instance.VisibleUI(UIType.UIHint, "强化失败！");
                SetEnhanceLevel(lastClickItem.petIcomObj.transform.Find("EnhanceLevel").GetComponent<Text>(), CurLevel);
                SetEnhanceAtrribe(lastClickItem.uIPetInfo).Coroutine();
                uIPetInfo = GetPetInfoToId(uIPetInfo.petId);
                SetPetList(c_PetsEnhanceResponse.Info, uIPetInfo);
                SetPetAttribute(uIPetInfo);
            }
            else if (c_PetsEnhanceResponse.Error == 1613)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "材料不足！");
            }
        }

        public void SetEnhanceMax()
        {

        }

    }

}
