using ETModel;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public GameObject skillIcon;
        public GameObject initiativeBtn;
        public GameObject AttributeBtn;
        public GameObject InitiativeDesContent;
        public GameObject AttributeDesContent;
        public GameObject SkillInfoOBJ;
        public Text SkillTypeTxt;
        public Text SkillNameTxt;

        public Button CloseBtn;
        public Button PetSkillCloseBtn;

        public Button InitiativeCloseBtn;
        public Button InitiativeLearnBtn;
        public Button InitiativeLearnedBtn;
        public Button InitiativeUsingBtn;
        public Button InitiativeNotBtn;
        public Button InitiativeUseBtn; 


        public Button AttributeLearnBtn;
        public Button AttributeLearnedBtn;
        public Button AttributeNotBtn;
        public Button CloseAttributeDesBtn;

        public PetSkillInfo clickPetSkill;

       
        public void InitUseSkill()
        {
            skillIcon = useSkillCollector.GetGameObject("SkillIcon");
            initiativeBtn = useSkillCollector.GetImage("InitiativeBtn").gameObject;
            AttributeBtn = useSkillCollector.GetImage("AttributeBtn").gameObject;
            InitiativeDesContent = useSkillCollector.GetImage("InitiativeDesContent").gameObject; 
            AttributeDesContent = useSkillCollector.GetImage("AttributeDesContent").gameObject;
            SkillTypeTxt = useSkillCollector.GetText("SkillTypeTxt");
            SkillNameTxt = useSkillCollector.GetText("SkillNameTxt");

            CloseBtn = useSkillCollector.GetButton("CloseBtn");
            InitiativeCloseBtn = InitiativeDesContent.GetReferenceCollector().GetButton("CloseInitiativeDesBtn");
            InitiativeCloseBtn.gameObject.SetActive(true);
            SkillInfoOBJ = InitiativeDesContent.GetReferenceCollector().GetImage("SkillInfo").gameObject;
            InitBtn();
        }
        public void InitBtn()
        {
            //关闭技能面板
            useSkillCollector.GetButton("PetSkillCloseBtn").onClick.AddSingleListener(delegate ()
            {
                useSkillCollector.gameObject.SetActive(false);
            });


            //主动关闭技能面板
            InitiativeCloseBtn.onClick.AddSingleListener(delegate ()
            {
                useSkillCollector.gameObject.SetActive(false);
            });
            //主动学习按钮
            InitiativeLearnBtn = InitiativeDesContent.GetReferenceCollector().GetImage("InitiativeBtn").gameObject.transform.Find("InitiativeLearnBtn").GetComponent<Button>();
            InitiativeLearnBtn.onClick.AddSingleListener(delegate ()
            {
                PetsLearnSkillRequest(lastClickItem.uIPetInfo.petId, clickPetSkill.petsItem).Coroutine();
                useSkillCollector.gameObject.SetActive(false);

                OpenPetsSkillRequest(lastClickItem.uIPetInfo).Coroutine();
            });
            //主动已学习按钮
            InitiativeLearnedBtn = InitiativeDesContent.GetReferenceCollector().GetImage("InitiativeBtn").gameObject.transform.Find("InitiativeLearnedBtn").GetComponent<Button>();
            InitiativeLearnedBtn.onClick.AddSingleListener(delegate ()
            {
                
            });
            //主动使用按钮
            InitiativeUseBtn = InitiativeDesContent.GetReferenceCollector().GetImage("InitiativeBtn").gameObject.transform.Find("InitiativeUseBtn").GetComponent<Button>();
            InitiativeUseBtn.onClick.AddSingleListener(delegate ()
            {
                UsePetsSkillRequest(lastClickItem, (int)clickPetSkill.petSkillID).Coroutine();
                useSkillCollector.gameObject.SetActive(false); 
                //OpenPetsSkillRequest(lastClickItem.uIPetInfo.petId).Coroutine();
            });
            //主动已使用按钮
            InitiativeUsingBtn = InitiativeDesContent.GetReferenceCollector().GetImage("InitiativeBtn").gameObject.transform.Find("InitiativeUsingBtn").GetComponent<Button>();
            InitiativeUsingBtn.onClick.AddSingleListener(delegate ()
            {
                PetsSkillCancelUseRequest(lastClickItem, (int)clickPetSkill.petSkillID).Coroutine();
                useSkillCollector.gameObject.SetActive(false);
                //OpenPetsSkillRequest(lastClickItem.uIPetInfo.petId).Coroutine();
            });
            //主动学习等级不够
            InitiativeNotBtn = InitiativeDesContent.GetReferenceCollector().GetImage("InitiativeBtn").gameObject.transform.Find("InitiativeNotBtn").GetComponent<Button>();
            InitiativeNotBtn.onClick.AddSingleListener(delegate ()
            {
                //useSkillCollector.gameObject.SetActive(false); 
            });




            //被动关闭按钮
            CloseAttributeDesBtn = AttributeDesContent.GetReferenceCollector().GetImage("AttributeBtn").gameObject.transform.Find("AttributeCloseBtn").GetComponent<Button>();
            CloseAttributeDesBtn.onClick.AddSingleListener(delegate ()
            {
                useSkillCollector.gameObject.SetActive(false);
            });
            //被动学习按钮
            AttributeLearnBtn = AttributeDesContent.GetReferenceCollector().GetImage("AttributeBtn").gameObject.transform.Find("AttributeLearnBtn").GetComponent<Button>();
            AttributeLearnBtn.onClick.AddSingleListener(delegate ()
            {
                PetsLearnSkillRequest(lastClickItem.uIPetInfo.petId, clickPetSkill.petsItem).Coroutine();
                useSkillCollector.gameObject.SetActive(false);
                OpenPetsSkillRequest(lastClickItem.uIPetInfo).Coroutine();
            });
            //被动已学习按钮
            AttributeLearnedBtn = AttributeDesContent.GetReferenceCollector().GetImage("AttributeBtn").gameObject.transform.Find("AttributeLearnedBtn").GetComponent<Button>();
            AttributeLearnedBtn.onClick.AddSingleListener(delegate ()
            {

            });
            //被动等级不够按钮
            AttributeNotBtn = AttributeDesContent.GetReferenceCollector().GetImage("AttributeBtn").gameObject.transform.Find("AttributeNotBtn").GetComponent<Button>();
            AttributeNotBtn.onClick.AddSingleListener(delegate ()
            {
                //useSkillCollector.gameObject.SetActive(false);
            });
            
        }

        public void SetUsePassiveSkillInfo(PetSkillInfo petSkillInfo)
        {
            if (petSkillInfo.petSkillID == 0)
                return;

            useSkillCollector.gameObject.SetActive(true);


            AttributeDesContent.SetActive(true);
            InitiativeDesContent.SetActive(false);
            SkillTypeTxt.text = "被动技能";
            AttributeDesContent.GetReferenceCollector().GetText("DesTxt").text = petSkillInfo.petSkillDes;

            AttributeLearnBtn.gameObject.SetActive(false);
            AttributeLearnedBtn.gameObject.SetActive(false);
            AttributeNotBtn.gameObject.SetActive(false);
            CloseAttributeDesBtn.gameObject.SetActive(true); ;

            skillIcon.transform.Find("skillIcon").GetComponent<Image>().sprite = petSkillInfo.sprite;
            SkillNameTxt.text = petSkillInfo.petSkillName;
        }

        public void SetUseInitiativetInfo(PetSkillInfo petSkillInfo)
        {
            if (petSkillInfo.petSkillID == 0)
                return;

            useSkillCollector.gameObject.SetActive(true);


            AttributeDesContent.SetActive(false);
            InitiativeDesContent.SetActive(true);
            SkillTypeTxt.text = "主动技能";
            skillIcon.transform.Find("skillIcon").GetComponent<Image>().sprite = petSkillInfo.sprite;
            InitiativeDesContent.GetReferenceCollector().GetText("DesTxt").text = petSkillInfo.petSkillDes;

            Pets_SkillConfig petSkillConfig = ConfigComponent.Instance.GetItem<Pets_SkillConfig>((int)petSkillInfo.petSkillID);

            var consume = JsonConvert.DeserializeObject<Dictionary<int, int>>(petSkillConfig.Consume);//附加数值
            var otherData = JsonConvert.DeserializeObject<Dictionary<int, int>>(petSkillConfig.OtherData);//附加数值
            if (consume.TryGetValue(1,out int Consume))
            {
                SkillInfoOBJ.transform.Find("MagicValue").GetComponent<Text>().text = $"魔法值    {Consume}";
            }
            
            SkillInfoOBJ.transform.Find("Distance").GetComponent<Text>().text = $"距离     {petSkillConfig.Distance}";

            if (otherData.TryGetValue(1, out int OtherData))
            {
                SkillInfoOBJ.transform.Find("AttackValue").GetComponent<Text>().text = $"基本技能攻击力    {OtherData}";
            }
           


            if (lastClickItem.uIPetInfo.usingPetsSkillID == petSkillInfo.petSkillID)
            {
                InitiativeCloseBtn.gameObject.SetActive(true);
                InitiativeLearnBtn.gameObject.SetActive(false);
                InitiativeLearnedBtn.gameObject.SetActive(false);
                InitiativeUsingBtn.gameObject.SetActive(true);
                InitiativeNotBtn.gameObject.SetActive(false);
                InitiativeUseBtn.gameObject.SetActive(false);
            }
            else
            {
                InitiativeCloseBtn.gameObject.SetActive(true);
                InitiativeLearnBtn.gameObject.SetActive(false);
                InitiativeLearnedBtn.gameObject.SetActive(false);
                InitiativeUsingBtn.gameObject.SetActive(false);
                InitiativeNotBtn.gameObject.SetActive(false);
                InitiativeUseBtn.gameObject.SetActive(true);
            }
            SkillNameTxt.text = petSkillInfo.petSkillName;
        }
        public void SetUseCanLearSkillInfo(PetSkillInfo petSkillInfo)
        {
            if (petSkillInfo.petSkillID == 0)
                return;

            useSkillCollector.gameObject.SetActive(true);

            skillIcon.transform.Find("skillIcon").GetComponent<Image>().sprite = petSkillInfo.sprite;

            if (petSkillInfo.skillType == 1)
            {
                SkillTypeTxt.text = "主动技能";
                Pets_SkillConfig petSkillConfig = ConfigComponent.Instance.GetItem<Pets_SkillConfig>((int)petSkillInfo.petSkillID);

                var consume = JsonConvert.DeserializeObject<Dictionary<int, int>>(petSkillConfig.Consume);//附加数值
                var otherData = JsonConvert.DeserializeObject<Dictionary<int, int>>(petSkillConfig.OtherData);//附加数值
                if (consume.TryGetValue(1, out int Consume))
                {
                    SkillInfoOBJ.transform.Find("MagicValue").GetComponent<Text>().text = $"魔法值    {Consume}";
                }

                SkillInfoOBJ.transform.Find("Distance").GetComponent<Text>().text = $"距离     {petSkillConfig.Distance}";

                if (otherData.TryGetValue(1, out int OtherData))
                {
                    SkillInfoOBJ.transform.Find("AttackValue").GetComponent<Text>().text = $"基本技能攻击力    {OtherData}";
                }


                AttributeDesContent.SetActive(false);
                InitiativeDesContent.SetActive(true);
                if(petInitiativeSkillList.Exists(e => e.petSkillID == petSkillInfo.petSkillID))
                {
                    InitiativeCloseBtn.gameObject.SetActive(true);
                    InitiativeLearnBtn.gameObject.SetActive(false);
                    InitiativeLearnedBtn.gameObject.SetActive(true);
                    InitiativeUsingBtn.gameObject.SetActive(false);
                    InitiativeNotBtn.gameObject.SetActive(false);
                    InitiativeUseBtn.gameObject.SetActive(false);
                }
                else
                {
                    InitiativeCloseBtn.gameObject.SetActive(true);
                    InitiativeLearnBtn.gameObject.SetActive(true);
                    InitiativeLearnedBtn.gameObject.SetActive(false);
                    InitiativeUsingBtn.gameObject.SetActive(false);
                    InitiativeNotBtn.gameObject.SetActive(false);
                    InitiativeUseBtn.gameObject.SetActive(false);
                }
            }
            else if(petSkillInfo.skillType == 2)
            {
                SkillTypeTxt.text = "被动技能";
                AttributeDesContent.SetActive(true);
                InitiativeDesContent.SetActive(false);
                
                if(petPassiveSkillList.Exists(e => e.petSkillID == petSkillInfo.petSkillID))
                {
                    AttributeLearnBtn.gameObject.SetActive(false);
                    AttributeLearnedBtn.gameObject.SetActive(true);
                    AttributeNotBtn.gameObject.SetActive(false);
                    CloseAttributeDesBtn.gameObject.SetActive(false);
                }
                else
                {
                    AttributeLearnBtn.gameObject.SetActive(true);
                    AttributeLearnedBtn.gameObject.SetActive(false);
                    AttributeNotBtn.gameObject.SetActive(false);
                    CloseAttributeDesBtn.gameObject.SetActive(false);
                }
                
            }
            InitiativeDesContent.GetReferenceCollector().GetText("DesTxt").text = petSkillInfo.petSkillDes;
            SkillNameTxt.text = petSkillInfo.petSkillName;
        }
    }

}
