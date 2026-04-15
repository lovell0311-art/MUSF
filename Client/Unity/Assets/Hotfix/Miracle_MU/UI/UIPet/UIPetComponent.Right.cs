using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public ReferenceCollector petRightCollector;
        public GameObject PetSkillAttributeTogs;
        public GameObject AttributeBG, AdvanceBG;
        public GameObject SkillBG;
        public GameObject EnhanceBG;
        public Button CloseBeinnerButton;
        public void InitPetRight()
        {
            CloseBeinnerButton = petCollector.GetButton("CloseBeinnerButton");
            petRightCollector = petCollector.GetImage("PetRight").gameObject.GetReferenceCollector();

            PetSkillAttributeTogs = petRightCollector.GetGameObject("PetSkillAttributeTogs");
            AttributeBG = petRightCollector.GetImage("AttributeBG").gameObject;
            AdvanceBG = petRightCollector.GetImage("AdvanceBG").gameObject;
            SkillBG = petRightCollector.GetImage("SkillBG").gameObject;
            EnhanceBG = petRightCollector.GetImage("EnhanceBG").gameObject;
            enhanceCollector = petRightCollector.GetImage("EnhanceBG").gameObject.GetReferenceCollector();
            //属性技能按钮事件
            PetSkillAttributeTogs.transform.GetChild(0).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => ToggleEvent(GetAttributeSkill(0), value));
            PetSkillAttributeTogs.transform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => ToggleEvent(GetAttributeSkill(1), value));
            PetSkillAttributeTogs.transform.GetChild(2).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => ToggleEvent(GetAttributeSkill(2), value));
            PetSkillAttributeTogs.transform.GetChild(3).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => ToggleEvent(GetAttributeSkill(3), value));
            PetSkillAttributeTogs.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            CloseBeinnerButton.image.raycastTarget = false;
            CloseBeinnerButton.onClick.AddSingleListener(() =>
            {
                if (BeginnerGuideData.IsComplete(24))
                {
                    BeginnerGuideData.SetBeginnerGuide(24);
                    UIBeginnerGuideClose.SetActive(true);
                }
            });

            EnhanceInit();//强化
            ToggleEvent(AttributeSkill.Attribute,true);
        }

        /// <summary>
        /// 请求宠物进阶
        /// </summary>
        /// <returns></returns>
        public async ETVoid AdvancedPet()
        {
            if (lastClickItem.uIPetInfo.petId== 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "未获取宠物数据");
                return;
            }
            G2C_PetProgressionResponse g2C_OpenPets = (G2C_PetProgressionResponse)await SessionComponent.Instance.Session.Call(new C2G_PetProgressionRequest() { PetsID = lastClickItem.uIPetInfo.petId });
            Log.DebugBrown("进阶返回" + JsonHelper.ToJson(g2C_OpenPets)+":::"+ lastClickItem.uIPetInfo.Anvance);
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "进阶成功");

                lastClickItem.uIPetInfo.Anvance += 1;
            }
            ShowAdvanceBG();
        }


        /// <summary>
        /// 宠物进阶
        /// </summary>
        public void ShowAdvanceBG()
        {
            AdvanceBG.transform.Find("des1").GetComponent<Text>().text = "攻击力"+ lastClickItem.uIPetInfo.Anvance*10;
            AdvanceBG.transform.Find("des2").GetComponent<Text>().text = "攻击力" + (lastClickItem.uIPetInfo.Anvance+1)*10;
            //请求进阶
            AdvanceBG.transform.Find("_bg/Btn_advance").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                AdvancedPet().Coroutine();
            });

            foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)//背包拥有的材料
            {

                if (item1.ConfigId == 280003)
                {
                    //  Log.DebugBrown("打印数据" + item1.GetProperValue(E_ItemValue.Quantity));
                    ((long)280003).GetItemInfo_Out(out Item_infoConfig item_Info);
                    if (item1.GetProperValue(E_ItemValue.Quantity) >= 1)
                    {
                        AdvanceBG.transform.Find("_bg/bg/Text").GetComponent<Text>().text = $"{item_Info.Name}<color=green>{1}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
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
                        AdvanceBG.transform.Find("_bg/bg1/Text").GetComponent<Text>().text = $"{item_Info.Name}<color=green>{1}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                    }
                    break;
                }
            }
            foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)//背包拥有的材料
            {

                if (item1.ConfigId == 320469)
                {
                    //  Log.DebugBrown("打印数据" + item1.GetProperValue(E_ItemValue.Quantity));
                    ((long)320469).GetItemInfo_Out(out Item_infoConfig item_Info);
                    if (item1.GetProperValue(E_ItemValue.Quantity) >= 1)
                    {
                        AdvanceBG.transform.Find("_bg/bg2/Text").GetComponent<Text>().text = $"{item_Info.Name}<color=green>{1}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                    }
                    break;
                }
            }
        }



        public void ToggleEvent(AttributeSkill attributeSkill, bool isOn)
        {
            if (!isOn || lastClickItem == null) return;
            if (attributeSkill != AttributeSkill.Enhance) { HindEnhanceCaiLiao(); }
            SkillBG.gameObject.SetActive(attributeSkill == AttributeSkill.Skill);
            AttributeBG.gameObject.SetActive(attributeSkill == AttributeSkill.Attribute);
            EnhanceBG.gameObject.SetActive(attributeSkill == AttributeSkill.Enhance && lastClickItem.uIPetInfo != null);
            AdvanceBG.gameObject.SetActive(attributeSkill == AttributeSkill.Advance);
            if (AdvanceBG.gameObject.activeSelf==true)
            {
                ShowAdvanceBG();
            }
            if (attributeSkill == AttributeSkill.Skill)
            {
                if (BeginnerGuideData.IsComplete(23))
                {
                    CloseBeinnerButton.image.raycastTarget = true;
                    BeginnerGuideData.SetBeginnerGuide(23);
                    UIBeginnerGuideCanSkill.SetActive(true);
                }
            }
            else if(attributeSkill == AttributeSkill.Enhance && lastClickItem.uIPetInfo != null) { SetEnhanceAtrribe(lastClickItem.uIPetInfo).Coroutine(); }
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
                case 3:
                    return AttributeSkill.Advance;
            }
            return AttributeSkill.Null;
        }
    }

}
