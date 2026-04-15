using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIMainComponent
    {
        /// <summary>
        /// 引导面板
        /// </summary>
        public GameObject UIBeginnerGuide, UIBeginnerGuideknapsack, UIBeginnerGuidePet, UIBeginnerGuideHook, 
            UIBeginnerGuideAttribute, UIBeginnerGuideSkill, UIBeginnerGuideMap, UIBeginnerGuideJianShi,UIBeginnerGuideGongJianShou;
        /// <summary>
        /// 全屏遮罩
        /// </summary>
        public Image MaskImage;
        public void InitBeginnerGuide()
        {
            UIBeginnerGuide = ReferenceCollector_Main.GetImage("UIBeginnerGuide").gameObject;
            UIBeginnerGuideknapsack = ReferenceCollector_Main.GetImage("UIBeginnerGuideknapsack").gameObject;
            UIBeginnerGuidePet = ReferenceCollector_Main.GetImage("UIBeginnerGuidePet").gameObject;
            UIBeginnerGuideHook = ReferenceCollector_Main.GetImage("UIBeginnerGuideHook").gameObject;
            UIBeginnerGuideAttribute = ReferenceCollector_Main.GetImage("UIBeginnerGuideAttribute").gameObject;
            UIBeginnerGuideSkill = ReferenceCollector_Main.GetImage("UIBeginnerGuideSkill").gameObject;
            UIBeginnerGuideMap = ReferenceCollector_Main.GetImage("UIBeginnerGuideMap").gameObject;
            UIBeginnerGuideJianShi = ReferenceCollector_Main.GetImage("UIBeginnerGuideJianShi").gameObject;
            UIBeginnerGuideGongJianShou = ReferenceCollector_Main.GetImage("UIBeginnerGuideGongJianShou").gameObject;
            MaskImage = ReferenceCollector_Main.GetImage("MaskImage");
            MaskImage.raycastTarget = false;
            //SetBeginnerGuide();
          //  BtnList.horizontal = true;
            Game.EventCenter.EventListenner(EventTypeId.LOCALROLE_DEAD, SetMask);
        }

        public void SetMask() 
        {
            HookTog.isOn = false; SetMask(false);
        }

        public void SetMask(bool show)
        {
            if (!Guidance_Define.IsBeginnerGuide) return;
            if (MaskImage == null) return;
            MaskImage.raycastTarget = show;
            MaskImage.transform.Find("Text").gameObject.SetActive(show);
        }
        public void SetBeginnerGuide(bool isTrigger = false)
        {
          //  BtnList.horizontal = true;
            if (isTrigger)
            {
                if (BeginnerGuideData.IsCompleteTrigger(46, 45))
                {
                    UIComponent.Instance.RemoveAll();
                    BeginnerGuideData.SetBeginnerGuide(46);
                    UIBeginnerGuideAttribute.SetActive(true);
                   // BtnList.horizontal = false;
                }
                else if (BeginnerGuideData.IsCompleteTrigger(49, 45))
                {
                    BeginnerGuideData.SetBeginnerGuide(49);
                    UIBeginnerGuide.SetActive(true);
                }
                else if (BeginnerGuideData.IsCompleteTrigger(54,53))
                {
                    UIComponent.Instance.RemoveAll();
                    BeginnerGuideData.SetBeginnerGuide(54);
                    UIBeginnerGuideSkill.SetActive(true);
                  //  BtnList.horizontal = false;
                }
                else if (BeginnerGuideData.IsCompleteTrigger(59, 58))
                {
                    UIComponent.Instance.RemoveAll();
                    BeginnerGuideData.SetBeginnerGuide(59);
                    UIBeginnerGuideMap.SetActive(true);
                }
                return;
            }


            if (BeginnerGuideData.IsComplete(1))
            {
                BeginnerGuideData.SetBeginnerGuide(1);
                UIBeginnerGuide.SetActive(true);
            }
            else if(BeginnerGuideData.IsComplete(4))
            {
                SetMask(false);
                BeginnerGuideData.SetBeginnerGuide(4);
                UIBeginnerGuide.SetActive(true);
            }
            else if (BeginnerGuideData.IsComplete(7))
            {
                UIBeginnerGuide.SetActive(true);
            }
            else if (BeginnerGuideData.IsComplete(8))
            {
                BtnList.horizontal = false;
                BeginnerGuideData.SetBeginnerGuide(8);
                UIBeginnerGuideknapsack.SetActive(true);
            }
            else if (BeginnerGuideData.IsComplete(11))
            {
                UIComponent.Instance.RemoveAll();
                BeginnerGuideData.SetBeginnerGuide(11);
                UIBeginnerGuide.SetActive(true);
            }
            else if (BeginnerGuideData.IsComplete(14))
            {
                BeginnerGuideData.SetBeginnerGuide(14);
                UIBeginnerGuideknapsack.SetActive(true);

                BtnList.horizontal = false;
            }
            else if (BeginnerGuideData.IsComplete(17))
            {
                UIComponent.Instance.RemoveAll();
                BeginnerGuideData.SetBeginnerGuide(17);
                UIBeginnerGuide.SetActive(true);
            }
            else if (BeginnerGuideData.IsComplete(20))
            {
                BeginnerGuideData.SetBeginnerGuide(20);
                UIBeginnerGuidePet.SetActive(true);

                BtnList.horizontal = false;
            }
            else
            {
                 SetMask(false);
            }
            //else if (BeginnerGuideData.IsComplete(26))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(26);
            //    UIBeginnerGuide.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(32))
            //{
            //    SetMask(false);
            //    if (UIBeginnerGuideHook.activeSelf)
            //    {
            //        UIBeginnerGuideHook.GetComponent<UserGuidance>().OnMaskRectHide();
            //    }
            //    BeginnerGuideData.SetBeginnerGuide(32);
            //    UIBeginnerGuide.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(35))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(35);
            //    UIBeginnerGuide.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(39))
            //{
            //    SetMask(false); 
            //    BeginnerGuideData.SetBeginnerGuide(39);
            //    if (roleEntity.RoleType == E_RoleType.Swordsman)
            //    {
            //        UIBeginnerGuideJianShi.SetActive(true);
            //    }
            //    else if (roleEntity.RoleType == E_RoleType.Archer)
            //    {
            //        UIBeginnerGuideGongJianShou.SetActive(true);
            //    }
            //    else if (roleEntity.RoleType == E_RoleType.Magician)
            //    {
            //        UIBeginnerGuideJianShi.SetActive(true);
            //    }
            //}
        }
    }

}
