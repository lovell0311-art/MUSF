using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIMountComponent
    {
        public ReferenceCollector petRightCollector;
        public GameObject PetSkillAttributeTogs;
        public GameObject AttributeBG;
        public GameObject SkillBG;
        public GameObject EnhanceBG;
        public Button CloseBeinnerButton;

        public GameObject Move;//进阶界面
        public void InitPetRight()
        {
            //CloseBeinnerButton = petCollector.GetButton("CloseBeinnerButton");
            petRightCollector = petCollector.GetImage("PetRight").gameObject.GetReferenceCollector();

            PetSkillAttributeTogs = petRightCollector.GetGameObject("PetSkillAttributeTogs");
            AttributeBG = petRightCollector.GetImage("AttributeBG").gameObject;
            Move = petRightCollector.GetGameObject("Move").gameObject;
            EnhanceBG = petRightCollector.GetImage("EnhanceBG").gameObject;
            enhanceCollector = petRightCollector.GetImage("EnhanceBG").gameObject.GetReferenceCollector();
            //属性技能按钮事件
            PetSkillAttributeTogs.transform.GetChild(0).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => ToggleEvent(GetAttributeSkill(0), value));
            PetSkillAttributeTogs.transform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => ToggleEvent(GetAttributeSkill(1), value));
            PetSkillAttributeTogs.transform.GetChild(2).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => ToggleEvent(GetAttributeSkill(2), value));
            PetSkillAttributeTogs.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            //CloseBeinnerButton.image.raycastTarget = false;
            //CloseBeinnerButton.onClick.AddSingleListener(() =>
            //{
            //    if (BeginnerGuideData.IsComplete(24))
            //    {
            //        BeginnerGuideData.SetBeginnerGuide(24);
            //        UIBeginnerGuideClose.SetActive(true);
            //    }
            //});

            //强化Item_MountsConfig mounts_Info = ConfigComponent.Instance.GetItem<Item_MountsConfig>(mountConfigId);
            ToggleEvent(AttributeSkill.Attribute, true);
            AttributeBG.GetReferenceCollector().GetButton("WarBtn").gameObject.SetActive(UseMountData.IsUsing == 0);
            AttributeBG.GetReferenceCollector().GetButton("RestBtn").gameObject.SetActive(UseMountData.IsUsing == 1);
            AttributeBG.GetReferenceCollector().GetButton("WarBtn").onClick.AddSingleListener(() =>
            {
                UseMount().Coroutine();
            });
            AttributeBG.GetReferenceCollector().GetButton("IntoBagBtn").onClick.AddSingleListener(() =>
            {
                IntoBagMount().Coroutine();
            });
            AttributeBG.GetReferenceCollector().GetButton("RestBtn").onClick.AddSingleListener(() =>
            {
                RestMount().Coroutine();
            });
        }
        /// <summary>
        /// 刷新ui
        /// </summary>
        public void RefreshMount()
        {
            AttributeBG.GetReferenceCollector().GetButton("WarBtn").gameObject.SetActive(UseMountData.IsUsing == 0);
            AttributeBG.GetReferenceCollector().GetButton("RestBtn").gameObject.SetActive(UseMountData.IsUsing == 1);
        }
        /// <summary>
        /// 使用坐骑
        /// </summary>
        /// <returns></returns>
        public async ETVoid UseMount()
        {
            if (UseMountData.UId == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "未获取坐骑数据");
                return;
            }
            Log.DebugBrown("请求使用坐骑" + UseMountData.UId);
            long equippedMountId = UIMainComponent.Instance?.GetEquippedMountId() ?? 0;
            if (equippedMountId > 0 && equippedMountId != UseMountData.UId)
            {
                G2C_RecallMountResponse recallMountResponse =
                    (G2C_RecallMountResponse)await SessionComponent.Instance.Session.Call(
                        new C2G_RecallMountRequest() { MountID = equippedMountId });
                if (recallMountResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, recallMountResponse.Error.GetTipInfo());
                    return;
                }
            }

            G2C_UseMountResponse g2C_OpenPets = (G2C_UseMountResponse)await SessionComponent.Instance.Session.Call(new C2G_UseMountRequest() { MountID = UseMountData.UId });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
                return;
            }
            UseMountData.IsUsing = 1;
            RefreshMount();
            Log.DebugBrown("获取坐骑属性" + JsonHelper.ToJson(g2C_OpenPets));
        }
        /// <summary>
        /// 坐骑收回背包
        /// </summary>
        /// <returns></returns>
        public async ETVoid IntoBagMount()
        {
            Log.DebugBrown("请求回收坐骑到背包" + UseMountData.UId);
            if (UseMountData.UId == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "未获取坐骑数据");
                return;
            }
            G2C_RecallMountBackpackResponse g2C_OpenPets = (G2C_RecallMountBackpackResponse)await SessionComponent.Instance.Session.Call(new C2G_RecallMountBackpackRequest() { MountID = UseMountData.UId });
            Log.DebugBrown("获取坐骑属性" + JsonHelper.ToJson(g2C_OpenPets));
        }


        /// <summary>
        /// 坐骑召回
        /// </summary>
        /// <returns></returns>
        public async ETVoid RestMount()
        {
            if (UseMountData.UId == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "未获取坐骑数据");
                return;
            }
            Log.DebugBrown("请求回收坐骑" + UseMountData.UId);
            G2C_RecallMountResponse g2C_OpenPets = (G2C_RecallMountResponse)await SessionComponent.Instance.Session.Call(new C2G_RecallMountRequest() { MountID = UseMountData.UId });

            Log.DebugBrown("获取坐骑属性" + JsonHelper.ToJson(g2C_OpenPets));
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
                return;
            }
            UseMountData.IsUsing = 0;
            RefreshMount();
        }


        public void MountProperty()
        {
            AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("power/Text").GetComponent<Text>().text = "";
            AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("attackForce/Text").GetComponent<Text>().text = "";
            AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("attackSuccess/Text").GetComponent<Text>().text = "";
            AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("pvpAttack/Text").GetComponent<Text>().text = "";
            if (UseMountData.UId == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请先选中一个坐骑");
                return;
            }

            Item_MountsConfig mounts_Info = ConfigComponent.Instance.GetItem<Item_MountsConfig>((int)UseMountData.MountId);
            int index = 0;
            for (int i = 0; i < mounts_Info.BaseAttrId.Count; i++)
            {
                ItemAttrEntry_BaseConfig itemAttrEntry_Base = ConfigComponent.Instance.GetItem<ItemAttrEntry_BaseConfig>(mounts_Info.BaseAttrId[i]);
                if (itemAttrEntry_Base != null)
                {
                    index++;
                    // int value = 0;
                    AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("power").gameObject.SetActive(1 <= index);
                    AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("attackForce").gameObject.SetActive(1 <= index);
                    AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("attackSuccess").gameObject.SetActive(1 <= index);
                    AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("pvpAttack").gameObject.SetActive(1 <= index);
                    if (index == 1)
                    {
                        // list.Add($"<color={ColorTools.NormalItemNameColor}>{string.Format(itemAttrEntry_Base.Name, GetValue() + GetOutsattrib())}</color>");
                        AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("power/Text").GetComponent<Text>().text = string.Format(itemAttrEntry_Base.Name, GetValue());
                    }
                    else if (index == 2)
                    {
                        AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("attackForce/Text").GetComponent<Text>().text = string.Format(itemAttrEntry_Base.Name, GetValue());
                    }
                    else if (index == 3)
                    {
                        AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("attackSuccess/Text").GetComponent<Text>().text = string.Format(itemAttrEntry_Base.Name, GetValue());
                    }
                    else
                    {
                        AttributeBG.GetReferenceCollector().GetImage("AttributeContent").transform.Find("pvpAttack/Text").GetComponent<Text>().text = string.Format(itemAttrEntry_Base.Name, GetValue());
                    }
                    ///获取 属性值
                    int GetValue() => UseMountData.Fortifiedlevel switch
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
        /// <summary>
        /// 刷新进阶
        /// </summary>
        public void MoveUIMount()
        {
            if (UseMountData.UId == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "未获取坐骑数据");
                return;
            }
            Log.DebugBrown("打印当前坐骑的阶数" + UseMountData.AdvancedLevel);
            //  Move.transform.Find("Mount_name").GetComponent<Text>().text=""
            Move.transform.Find("Data1/bg/des").GetComponent<Text>().text = "防御力:"+ UseMountData.AdvancedLevel*10;
            Move.transform.Find("Data1/bg/value").GetComponent<Text>().text = "防御力:"+(UseMountData.AdvancedLevel+1)*10;
            foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)//背包拥有的材料
            {

                if (item1.ConfigId == 280003)
                {
                    //  Log.DebugBrown("打印数据" + item1.GetProperValue(E_ItemValue.Quantity));
                    ((long)280003).GetItemInfo_Out(out Item_infoConfig item_Info);
                    if (item1.GetProperValue(E_ItemValue.Quantity) >= 1)
                    {
                        Move.transform.Find("_bg/obj1").GetChild(0).GetChild(0).GetComponent<Text>().text = $"{item_Info.Name}<color=green>{1}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                    }
                    break;
                }
            }

            foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)//背包拥有的材料
            {

                if (item1.ConfigId == 280004)
                {
                    //  Log.DebugBrown("打印数据" + item1.GetProperValue(E_ItemValue.Quantity));
                    ((long)280004).GetItemInfo_Out(out Item_infoConfig item_Info);
                    if (item1.GetProperValue(E_ItemValue.Quantity) >= 1)
                    {
                        Move.transform.Find("_bg/obj1").GetChild(1).GetChild(0).GetComponent<Text>().text = $"{item_Info.Name}<color=green>{1}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                    }
                    break;
                }
            }

            foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)//背包拥有的材料
            {

                if (item1.ConfigId == 280001)
                {
                    //  Log.DebugBrown("打印数据" + item1.GetProperValue(E_ItemValue.Quantity));
                    ((long)280001).GetItemInfo_Out(out Item_infoConfig item_Info);
                    if (item1.GetProperValue(E_ItemValue.Quantity) >= 1)
                    {
                        Move.transform.Find("_bg/obj1").GetChild(2).GetChild(0).GetComponent<Text>().text = $"{item_Info.Name}<color=green>{1}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                    }
                    break;
                }
            }

            foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)//背包拥有的材料
            {

                if (item1.ConfigId == 280006)
                {
                    //  Log.DebugBrown("打印数据" + item1.GetProperValue(E_ItemValue.Quantity));
                    ((long)280006).GetItemInfo_Out(out Item_infoConfig item_Info);
                    if (item1.GetProperValue(E_ItemValue.Quantity) >= 1)
                    {
                        Move.transform.Find("_bg/obj1").GetChild(3).GetChild(0).GetComponent<Text>().text = $"{item_Info.Name}<color=green>{1}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                    }
                    break;
                }
            }




            Move.transform.Find("_bg/Btn_move").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                AdvancedMount().Coroutine();
            });

            //   Item_MountsConfig mounts_Info = ConfigComponent.Instance.GetItem<Item_MountsConfig>((int)UseMountData.UId);
            //int PhylactiCpower = 0;
            //if (mounts_Info.Name== "圣迹火龙"|| mounts_Info.Name == "龙魂冰龙")
            //{
            //    PhylactiCpower = 120;
            //}
            //else if (mounts_Info.Name == "炎狼兽之角 +破坏")
            //{
            //    PhylactiCpower = 70;
            //}
            //else if (mounts_Info.Name == "炎狼兽之角 +守护" || mounts_Info.Name == "黑王马之角")
            //{
            //    PhylactiCpower = 50;
            //}
            //else if (mounts_Info.Name == "炎狼兽之角 +欢迎" || mounts_Info.Name == "烈火战马")
            //{
            //    PhylactiCpower = 100;
            //}
            //else if (mounts_Info.Name == "玉面狐狸"|| mounts_Info.Name == "飞行火麒麟")
            //{
            //    PhylactiCpower = 150;
            //}
            //else if (mounts_Info.Name == "龙之心")
            //{
            //    PhylactiCpower = 200;
            //}

        }
        /// <summary>
        /// 坐骑进阶
        /// </summary>
        /// <returns></returns>
        public async ETVoid AdvancedMount()
        {
            if (UseMountData.UId == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "未获取坐骑数据");
                return;
            }
            G2C_AdvancedMountResponse g2C_OpenPets = (G2C_AdvancedMountResponse)await SessionComponent.Instance.Session.Call(new C2G_AdvancedMountRequest() {MountID=UseMountData.UId });
            Log.DebugBrown("进阶返回" + JsonHelper.ToJson(g2C_OpenPets));
            if (g2C_OpenPets.Error!=0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "进阶成功");
               
            }
            UseMountData.AdvancedLevel = UseMountData.AdvancedLevel + 1;
            MoveUIMount();
        }
        public void ToggleEvent(AttributeSkill attributeSkill, bool isOn)
        {
            if (!isOn || lastClickItem == null) return;
            if (attributeSkill != AttributeSkill.Enhance) { HindEnhanceCaiLiao(); }
            Move.gameObject.SetActive(attributeSkill == AttributeSkill.Skill);
            if (Move.gameObject.activeSelf==true)
            {
                MoveUIMount();
            }
            if (attributeSkill == AttributeSkill.Attribute)
            {
                MountProperty();
            }
            AttributeBG.gameObject.SetActive(attributeSkill == AttributeSkill.Attribute);
            EnhanceBG.gameObject.SetActive(attributeSkill == AttributeSkill.Enhance);
            //if (attributeSkill == AttributeSkill.Skill)
            //{
            //    if (BeginnerGuideData.IsComplete(23))
            //    {
            //        CloseBeinnerButton.image.raycastTarget = true;
            //        BeginnerGuideData.SetBeginnerGuide(23);
            //        UIBeginnerGuideCanSkill.SetActive(true);
            //    }
            //}
            //else
            if (attributeSkill == AttributeSkill.Enhance ) { SetEnhanceAtrribe(); }
        }
        public void SetAttribute()
        {

        }
        public void SetSkill()
        {

        }
        public AttributeSkill GetAttributeSkill(int index)
        {
            switch (index)
            {
                case 0:
                    return AttributeSkill.Attribute;
                case 1:
                    return AttributeSkill.Skill;
                case 2:
                    return AttributeSkill.Enhance;
            }
            return AttributeSkill.Null;
        }
    }

}
