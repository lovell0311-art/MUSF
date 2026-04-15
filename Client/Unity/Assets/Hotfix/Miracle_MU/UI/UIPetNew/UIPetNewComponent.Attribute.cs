using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetNewComponent
    {
        public ReferenceCollector attributeCollector;
       
        public Button ReleaseBtn, RestBtn, WarBtn, IntoBagBtn;

        public GameObject AttributeContent;

        public void InitAttribute()
        {
            attributeCollector = AttributeBG.GetReferenceCollector();
            
            AttributeContent = attributeCollector.GetImage("AttributeContent").gameObject;
            WarBtn = attributeCollector.GetButton("WarBtn");
            RestBtn = attributeCollector.GetButton("RestBtn");
            ReleaseBtn = attributeCollector.GetButton("ReleaseBtn"); 
            IntoBagBtn = attributeCollector.GetButton("IntoBagBtn");
            AttributeBtnInit();
        }

        public void AttributeBtnInit()
        {

            ReleaseBtn.onClick.AddListener(delegate ()
            {
                if (newPetsInfos.Exists(e => e.newPetsInfo.PetsID != 0))
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($" 是否放生宠物：<color=red>{lastClickInfo.Name}</color>？");
                    uIConfirmComponent.AddActionEvent(() =>
                    {
                        //放生
                        PetsReleaseRequest(lastClickInfo.newPetsInfo.PetsID).Coroutine();
                    });
                }

            });
           
            RestBtn.onClick.AddSingleListener(delegate ()
            {
               
                PetsRestRequest(lastClickInfo.newPetsInfo.PetsID).Coroutine();
            });
            WarBtn.onClick.AddSingleListener(delegate ()
            {
                if (newPetsInfos.Exists(e => e.newPetsInfo.PetsID != 0))
                {
                    PetsGoToWarRequest(lastClickInfo.newPetsInfo.PetsID).Coroutine();
                    if (BeginnerGuideData.IsComplete(22))
                    {
                        BeginnerGuideData.SetBeginnerGuide(22);
                        UIBeginnerGuideClose.SetActive(true);
                    }
                }

            });
            IntoBagBtn.onClick.AddListener(() =>
            {
                if (lastClickInfo == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"请选择放入背包的宠物！");
                    return;
                }
                PetPackBackRequest(lastClickInfo.newPetsInfo.PetsID).Coroutine();
                //UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                //uIConfirmComponent.SetTipText($"是否消耗<color=red>1</color>魔晶把宠物：<color=red>{lastClickInfo.Name}</color>放入背包？");
                //uIConfirmComponent.AddActionEvent(() =>
                //{
                //    //进入背包
                //    PetPackBackRequest(lastClickInfo.newPetsInfo.PetsID).Coroutine();
                //});

            });
        }
        
        /// <summary>
        /// 设置休息出战复活按钮
        /// </summary>
        /// <param name="restOrWar"></param>
        /// <param name="daed"></param>
        public void SetBtnActive(PetWarState restOrWar)
        {
            WarBtn.gameObject.SetActive(restOrWar == PetWarState.Rest);
            RestBtn.gameObject.SetActive(restOrWar == PetWarState.War);
        }

        public void SetAttribute(UIPetNewInfo uIPetNewInfo)
        {
            AttributeContent.transform.Find("JIChuShuXingTxt").gameObject.GetComponent<Text>().text = uIPetNewInfo.basicAttribute;
            AttributeContent.transform.Find("ZhuoYueShuXingTxt").gameObject.GetComponent<Text>().text = uIPetNewInfo.excellent;
        }

    }

}
