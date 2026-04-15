using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIMountComponent
    {
        public GameObject EnhanceLevelValue, XiaoHaoValue;
        public Text AddAttributeOne, AddAttributeTwo, AddAttributeThree, SuccessRateValue, AddAttributeFour;
        public Button QiangHuaBtn;
        public GameObject oneObj, TwoObj, ThreeObj;
        public void EnhanceInit()
        {
            EnhanceLevelValue = enhanceCollector.GetGameObject("EnhanceLevelValue");
            XiaoHaoValue = enhanceCollector.GetGameObject("XiaoHaoValue");
            AddAttributeOne = enhanceCollector.GetText("AddAttributeOne");
            AddAttributeTwo = enhanceCollector.GetText("AddAttributeTwo");
            AddAttributeThree = enhanceCollector.GetText("AddAttributeThree");
            AddAttributeFour = enhanceCollector.GetText("AddAttributeFour");
            SuccessRateValue = enhanceCollector.GetText("SuccessRateValue");
            enhanceCollector.GetButton("QiangHuaBtn").onClick.AddSingleListener(() =>
            {
                PetsEnhanceRequest(UseMountData.UId).Coroutine();
            });
        }
        /// <summary>
        /// 强化信息
        /// </summary>
        public void SetEnhanceAtrribe()
        {
            if (UseMountData.UId == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "未获取坐骑数据");
                return;
            }
            EnhanceInit();
            Petproperty(UseMountData.Fortifiedlevel, UseMountData.MountId);
            if (CurLevel < 15)
            {
                EnhanceLevelValue.transform.Find("NoMax/NowValue").GetComponent<Text>().text =UseMountData.Fortifiedlevel.ToString();
                EnhanceLevelValue.transform.Find("NoMax/AfterValue").GetComponent<Text>().text = (UseMountData.Fortifiedlevel + 1).ToString();
            }
            else
            {
                EnhanceLevelValue.transform.Find("MaxValue").GetComponent<Text>().text = UseMountData.Fortifiedlevel.ToString();
            }
            SuccessRateValue.gameObject.SetActive(UseMountData.Fortifiedlevel < 15);
            if (UseMountData.Fortifiedlevel<15)
            {
                SuccessRateValue.text = (100 - UseMountData.Fortifiedlevel * 5)+"%（等级大于7失败降级2）";
            }
            //G2C_FortifiedMountResponse g2C_OpenPetsEnhance = (G2C_FortifiedMountResponse)await SessionComponent.Instance.Session.Call(new C2G_FortifiedMountRequest()
            //{
            //    MountID = UseMountData.UId
            //});
            //if (g2C_OpenPetsEnhance.Error != 0)
            //{
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPetsEnhance.Error.GetTipInfo());
            //}
            //else
            //{
            //    UseMountData.Fortifiedlevel = g2C_OpenPetsEnhance.AdvancedLevel;
            //    //HindEnhanceCaiLiao();
            //    //Log.DebugGreen($"坐骑强化等级{g2C_OpenPetsEnhance.Lv}");
            //    //CurLevel = g2C_OpenPetsEnhance.Lv;
            //    Petproperty(UseMountData.Fortifiedlevel, UseMountData.MountId);
            //    //enhanceCollector.GetButton("QiangHuaBtn").interactable = (CurLevel < 15 ? true : false);
            //    //EnhanceLevelValue.transform.Find("NoMax").gameObject.SetActive(CurLevel < 15);
            //    //EnhanceLevelValue.transform.Find("MaxValue").gameObject.SetActive(CurLevel == 15);
            //    //if (CurLevel < 15)
            //    //{
            //    //    EnhanceLevelValue.transform.Find("NoMax/NowValue").GetComponent<Text>().text = g2C_OpenPetsEnhance.Lv.ToString();
            //    //    EnhanceLevelValue.transform.Find("NoMax/AfterValue").GetComponent<Text>().text = (g2C_OpenPetsEnhance.Lv + 1).ToString();
            //    //}
            //    //else
            //    //{
            //    //    EnhanceLevelValue.transform.Find("MaxValue").GetComponent<Text>().text = (g2C_OpenPetsEnhance.Lv).ToString();
            //    //}
            //    ////AddAttributeOne.text = CurLevel < 15 ? $"生命值+{g2C_OpenPetsEnhance.Lv + 1}%" : $"生命值+{g2C_OpenPetsEnhance.Lv}%";
            //    ////AddAttributeTwo.text = uIPetInfo.petAttributeSystem == PetAttributeSystem.Physics ? (CurLevel < 15 ? $"攻击力+{g2C_OpenPetsEnhance.Lv + 1}%" : $"攻击力+{g2C_OpenPetsEnhance.Lv}%") : (CurLevel < 15 ? $"魔力+{g2C_OpenPetsEnhance.Lv + 1}%" : $"魔力+{g2C_OpenPetsEnhance.Lv}%");
            //    ////AddAttributeThree.text = CurLevel < 15 ? $"防御力+{g2C_OpenPetsEnhance.Lv + 1}%" : $"防御力+{g2C_OpenPetsEnhance.Lv}%";
            //    ////SuccessRateValue.gameObject.SetActive(CurLevel < 15);
            //    ////enhanceCollector.GetText("SuccessRate").gameObject.SetActive(CurLevel < 15);
            //    ////enhanceCollector.GetText("XiaoHao").gameObject.SetActive(CurLevel < 15);
            //    //enhanceCollector.GetText("SuccessRate").text = CurLevel < 15 ? "成功率" : null;
            //    //enhanceCollector.GetText("XiaoHao").text = CurLevel < 15 ? "消耗材料" : null;
            //    //if (CurLevel < 15)
            //    //    SuccessRateValue.text = g2C_OpenPetsEnhance.Lv != 0 ? (g2C_OpenPetsEnhance.Lv <= 5 ? $"{100 - g2C_OpenPetsEnhance.Lv * 10}%（失败强化等级减1）" : "50%（失败强化等级归0）") : "100%";
            //    //else
            //    //    SuccessRateValue.text = null;
            XiaoHaoValue.transform.Find("One").GetComponent<Text>().text = null;
            XiaoHaoValue.transform.Find("Two").GetComponent<Text>().text = null;
            foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)//背包拥有的材料
            {

                if (item1.ConfigId ==280003)
                {
                    //  Log.DebugBrown("打印数据" + item1.GetProperValue(E_ItemValue.Quantity));
                    ((long)280003).GetItemInfo_Out(out Item_infoConfig item_Info);
                    if (item1.GetProperValue(E_ItemValue.Quantity) >= 1)
                    {
                        XiaoHaoValue.transform.Find("One").GetComponent<Text>().text = $"<color=green>{1}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                    }
                    break;
                }
            }
            Item_GemstoneConfig fGemstoneConfig = ConfigComponent.Instance.GetItem<Item_GemstoneConfig>(280003);
            oneObj = ResourcesComponent.Instance.LoadGameObject(fGemstoneConfig.ResName.StringToAB(), fGemstoneConfig.ResName);//显示当前角色的模型
            oneObj.transform.SetParent(XiaoHaoValue.transform.Find("One").transform);
            oneObj.transform.position = XiaoHaoValue.transform.Find("One").position - new Vector3(0.75f, 0, 0);
            foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)//背包拥有的材料
            {

                if (item1.ConfigId == 280004)
                {
                    //  Log.DebugBrown("打印数据" + item1.GetProperValue(E_ItemValue.Quantity));
                    ((long)280004).GetItemInfo_Out(out Item_infoConfig item_Info);
                    if (item1.GetProperValue(E_ItemValue.Quantity) >= 1)
                    {
                        XiaoHaoValue.transform.Find("Two").GetComponent<Text>().text = $"<color=green>{1}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                    }
                    break;
                }
            }
            Item_GemstoneConfig fGemstoneConfig1 = ConfigComponent.Instance.GetItem<Item_GemstoneConfig>(280004);
            TwoObj = ResourcesComponent.Instance.LoadGameObject(fGemstoneConfig1.ResName.StringToAB(), fGemstoneConfig1.ResName);//显示当前角色的模型
            TwoObj.transform.SetParent(XiaoHaoValue.transform.Find("Two").transform);
            TwoObj.transform.position = XiaoHaoValue.transform.Find("Two").position - new Vector3(0.75f, 0, -10);

            //if (g2C_OpenPetsEnhance.EnhanceMaterials.Count > 0)
            //{
            //    XiaoHaoValue.transform.Find("One").GetComponent<Text>().text = $"{g2C_OpenPetsEnhance.EnhanceMaterials[0].ItemID}/{g2C_OpenPetsEnhance.EnhanceMaterials[0].ItemCnt}";
            //    Item_GemstoneConfig fGemstoneConfig = ConfigComponent.Instance.GetItem<Item_GemstoneConfig>(g2C_OpenPetsEnhance.EnhanceMaterials[0].ItemConfingID);

            //}

            //if (g2C_OpenPetsEnhance.EnhanceMaterials.Count > 1)
            //{
            //    XiaoHaoValue.transform.Find("Two").GetComponent<Text>().text = $"{g2C_OpenPetsEnhance.EnhanceMaterials[1].ItemID}/{g2C_OpenPetsEnhance.EnhanceMaterials[1].ItemCnt}";
            //    Item_GemstoneConfig fGemstoneConfig = ConfigComponent.Instance.GetItem<Item_GemstoneConfig>(g2C_OpenPetsEnhance.EnhanceMaterials[1].ItemConfingID);
            //    TwoObj = ResourcesComponent.Instance.LoadGameObject(fGemstoneConfig.ResName.StringToAB(), fGemstoneConfig.ResName);//显示当前角色的模型
            //    TwoObj.transform.position = XiaoHaoValue.transform.Find("Two").position - new Vector3(0.75f, 0, 0);
            //}
            //XiaoHaoValue.transform.Find("Three").GetComponent<Text>().text = null;
            //if (g2C_OpenPetsEnhance.EnhanceMaterials.Count > 2)
            //{
            //    XiaoHaoValue.transform.Find("Three").GetComponent<Text>().text = $"{g2C_OpenPetsEnhance.EnhanceMaterials[2].ItemID}/{g2C_OpenPetsEnhance.EnhanceMaterials[2].ItemCnt}";
            //    Item_GemstoneConfig fGemstoneConfig = ConfigComponent.Instance.GetItem<Item_GemstoneConfig>(g2C_OpenPetsEnhance.EnhanceMaterials[2].ItemConfingID);
            //    ThreeObj = ResourcesComponent.Instance.LoadGameObject(fGemstoneConfig.ResName.StringToAB(), fGemstoneConfig.ResName);//显示当前角色的模型
            //    ThreeObj.transform.position = XiaoHaoValue.transform.Find("Three").position - new Vector3(0.75f, 0, 0);
            //}
            //}
        }
        /// <summary>
        ///展示坐骑的属性
        /// </summary>
        public void Petproperty(int curLevel, long petId)
        {
           // curLevel += 1;
            // Item_MountsConfig mounts_Info = ConfigComponent.Instance.GetItem<Item_MountsConfig>((int)petId);
            Item_MountsConfig mounts_Info = ConfigComponent.Instance.GetItem<Item_MountsConfig>((int)petId);
            Log.DebugBrown("点击的坐骑id" + petId+"::level"+curLevel+":"+ mounts_Info.BaseAttrId.Count);
            //  var  itemAttrEntry_Base = ConfigComponent.Instance.GetItem<ItemAttrEntry_BaseConfig>();
            int index = 0;
            for (int i = 0; i < mounts_Info.BaseAttrId.Count; i++)
            {
                ItemAttrEntry_BaseConfig itemAttrEntry_Base = ConfigComponent.Instance.GetItem<ItemAttrEntry_BaseConfig>(mounts_Info.BaseAttrId[i]);
                if (itemAttrEntry_Base != null)
                {
                    index++;
                   // int value = 0;
                    AddAttributeOne.gameObject.SetActive(1 <= index);
                    AddAttributeTwo.gameObject.SetActive(2 <= index);
                    AddAttributeThree.gameObject.SetActive(3 <= index);
                    AddAttributeFour.gameObject.SetActive(4 <= index);
                    if (index == 1)
                    {
                        // list.Add($"<color={ColorTools.NormalItemNameColor}>{string.Format(itemAttrEntry_Base.Name, GetValue() + GetOutsattrib())}</color>");
                        AddAttributeOne.text = string.Format(itemAttrEntry_Base.Name, GetValue());
                    }
                    else if (index == 2)


                    {
                        AddAttributeTwo.text = string.Format(itemAttrEntry_Base.Name, GetValue());
                    }
                    else if (index == 3)
                    {
                        AddAttributeThree.text = string.Format(itemAttrEntry_Base.Name, GetValue());
                    }
                    else
                    {
                        AddAttributeFour.text = string.Format(itemAttrEntry_Base.Name, GetValue());
                    }
                    ///获取 属性值
                    int GetValue() => curLevel switch
                    {
                        0 => itemAttrEntry_Base.Value0,
                        1 => itemAttrEntry_Base.Value1,
                        2 => itemAttrEntry_Base.Value2,
                        3 => itemAttrEntry_Base.Value3,
                        4 => itemAttrEntry_Base.Value4,
                        5 => itemAttrEntry_Base.Value5,
                        6 => itemAttrEntry_Base.Value6,
                        7 => itemAttrEntry_Base.Value7,
                        8 => itemAttrEntry_Base.Value8,
                        9 => itemAttrEntry_Base.Value9,
                        10 => itemAttrEntry_Base.Value10,
                        11 => itemAttrEntry_Base.Value11,
                        12 => itemAttrEntry_Base.Value12,
                        13 => itemAttrEntry_Base.Value13,
                        14 => itemAttrEntry_Base.Value14,
                        15 => itemAttrEntry_Base.Value15,
                        _ => 0
                    };
                }
                else
                {
                    Log.DebugBrown("未查询到ItemAttrEntry_BaseConfig索引：" + mounts_Info.BaseAttrId[i] + "id");
                    return;
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

            if (UseMountData.UId == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "未获取坐骑数据");
                return;
            }

            G2C_FortifiedMountResponse c_PetsEnhanceResponse = (G2C_FortifiedMountResponse)await SessionComponent.Instance.Session.Call(new C2G_FortifiedMountRequest()
            {
                MountID = UseMountData.UId
            });
            Log.DebugBrown("请求宠物强化" + id + ":数据" + JsonHelper.ToJson(c_PetsEnhanceResponse));
            UseMountData.Fortifiedlevel = c_PetsEnhanceResponse.AdvancedLevel;
            if (c_PetsEnhanceResponse.Error == 1612)
            {
              //  UseMountData.Fortifiedlevel = c_PetsEnhanceResponse.AdvancedLevel;
                UIComponent.Instance.VisibleUI(UIType.UIHint, "强化成功！");
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, c_PetsEnhanceResponse.Error.GetTipInfo());
            }
            SetEnhanceAtrribe();
        }

        public UIPetInfo GetPetInfoToId(long petId)
        {
            foreach (var item in PetList)
            {
                if (item.petId == petId)
                {
                    return item;
                }
            }
            return null;
        }
        public void SetEnhanceMax()
        {

        }

    }

}
