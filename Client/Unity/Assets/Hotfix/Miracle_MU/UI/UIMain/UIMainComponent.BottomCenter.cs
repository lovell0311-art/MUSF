using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using NPOI.Util;


namespace ETHotfix
{

    /// <summary>
    /// 搴曢儴涓棿鎸夐挳妯″潡
    /// </summary>
    public partial class UIMainComponent
    {
        ReferenceCollector referenceCollector_BottomCenter;
        public Text hpvalue, mpvalue, sdvalue, agvalue;
        public Image Hp, Mp,ag, sd,HPValue,MPValue;
        UnitEnityProperty enityProperty;
       Image FriendMain, WarAllianceMain, AttributeRed, PetRed;
        public ScrollRect BtnList;
        public Toggle Gongneng;
        public static Image EmailRed;
        public void Init_BottomCenter()
        {
            enityProperty = this.roleEntity.Property;

            referenceCollector_BottomCenter = ReferenceCollector_Main.GetImage("BottonCenter").gameObject.GetReferenceCollector();
         //   BtnList = referenceCollector_BottomCenter.GetImage("Scroll View").gameObject.GetComponent<ScrollRect>();
           
            if (!GlobalDataManager.IsEnteringGame)
            {
                WarAlliance().Coroutine();
            }

           
            hpvalue = referenceCollector_BottomCenter.GetText("hpvalue");
            mpvalue = referenceCollector_BottomCenter.GetText("mpValue");
            sdvalue = referenceCollector_BottomCenter.GetText("sdTxt");
            agvalue = referenceCollector_BottomCenter.GetText("agTxt");
            BtnList = referenceCollector_BottomCenter.GetImage("BtnList").GetComponent<ScrollRect>();
            Gongneng = referenceCollector_BottomCenter.GetToggle("Gongneng");
            EnsureBottomButtonListCanScroll();
            hpvalue.text = $"{enityProperty.GetProperValue(E_GameProperty.PROP_HP)}/{enityProperty.GetProperValue(E_GameProperty.PROP_HP_MAX)}";
            mpvalue.text = $"{enityProperty.GetProperValue(E_GameProperty.PROP_MP)}/{enityProperty.GetProperValue(E_GameProperty.PROP_MP_MAX)}";

            Hp = referenceCollector_BottomCenter.GetImage("HP");
            Mp = referenceCollector_BottomCenter.GetImage("Mp");
            ag = referenceCollector_BottomCenter.GetImage("AG");
            sd = referenceCollector_BottomCenter.GetImage("SD");
            HPValue = referenceCollector_BottomCenter.GetImage("HPValue");
            MPValue = referenceCollector_BottomCenter.GetImage("MPValue");
            FriendMain = referenceCollector_BottomCenter.GetImage("FriendMain");
            WarAllianceMain = referenceCollector_BottomCenter.GetImage("WarAllianceRedDot");
            EmailRed = referenceCollector_BottomCenter.GetImage("EmailRed");
            AttributeRed = referenceCollector_BottomCenter.GetImage("AttributeRed");
            PetRed = referenceCollector_BottomCenter.GetImage("PetRed");

            //referenceCollector_BottomCenter.GetButton("knapsackBtn").onClick.AddSingleListener(()=> { UIComponent.Instance.VisibleUI(UIType.UIKnapsack,$"{E_KnapsackState.KS_Knapsack}"); });
            referenceCollector_BottomCenter.GetButton("knapsackBtn").onClick.AddSingleListener(() => { UIComponent.Instance.VisibleUI(UIType.UIKnapsackNew); });


            referenceCollector_BottomCenter.GetButton("skillBtn").onClick.AddSingleListener(()=> { UIComponent.Instance.VisibleUI(UIType.UISkill); });
            referenceCollector_BottomCenter.GetButton("Awakening").onClick.AddSingleListener(() => { UIComponent.Instance.VisibleUI(UIType.UIAwakening); });
            referenceCollector_BottomCenter.GetButton("FriendBtn").onClick.AddSingleListener(()=> { UIComponent.Instance.VisibleUI(UIType.UIFirendList);});
            referenceCollector_BottomCenter.GetButton("AttributeBtn").onClick.AddSingleListener(()=> { UIComponent.Instance.VisibleUI(UIType.UIRoleInfo); });
            referenceCollector_BottomCenter.GetButton("MasterBtn").onClick.AddSingleListener(()=> {
                //if (enityProperty.GetProperValue(E_GameProperty.Level) < 399)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "绛夌骇鏈揪鍒?00绾э紝鏃犳硶浣跨敤澶у笀");
                //    return;
                //}

                //if (enityProperty.GetProperValue(E_GameProperty.OccupationLevel) < 3)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "璇峰厛瀹屾垚涓夎浆");
                //    return;
                //}
                //UIComponent.Instance.VisibleUI(UIType.DaShiCanvas); 
                //   UIComponent.Instance.VisibleUI(UIType.UIDaShiNew); 
                UIComponent.Instance.VisibleUI("Function unavailable.");

            });
            referenceCollector_BottomCenter.GetButton("ZhanMengBtn").onClick.AddSingleListener(() =>
            {
               
                if (WarAllianceDatas.IsJoinWar)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIWarAlliance);
                    return;
                }
                UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText("Please go to the Ice Wind Valley War Alliance envoy at 324,329.");
                //UIComponent.Instance.VisibleUI(UIType.UIWarAlliance); 
            });
            referenceCollector_BottomCenter.GetButton("E_MailBtn").onClick.AddSingleListener(()=> { UIComponent.Instance.VisibleUI(UIType.UIE_Mail); });
            referenceCollector_BottomCenter.GetButton("SetBtn").onClick.AddSingleListener(()=> { UIComponent.Instance.VisibleUI(UIType.UIGameSet); });
            referenceCollector_BottomCenter.GetButton("PetBtn").onClick.AddSingleListener(()=> { UIComponent.Instance.VisibleUI(UIType.UIPet); });
            referenceCollector_BottomCenter.GetButton("MountBtn").onClick.AddSingleListener(() => { UIComponent.Instance.VisibleUI(UIType.UIMount); });
            // referenceCollector_BottomCenter.GetButton("PetBtn").onClick.AddSingleListener(()=> { UIComponent.Instance.VisibleUI(UIType.UIPetNew); });

            Gongneng.onValueChanged.AddSingleListener((isOn) =>
            {
                referenceCollector_BottomCenter.GetImage("BtnList").gameObject.SetActive(isOn);
                if (isOn)
                {
                    EnsureBottomButtonListCanScroll();
                }
            });

            ChangeRoleHp();
            ChangeRoleMp();
            ChangeAG();
            ChangeSD();
            RedDotFriendCheack();
           // referenceCollector_BottomCenter.GetImage("BtnList").gameObject.SetActive(false);
        }
        public async ETVoid WarAlliance()
        {
            G2C_OpenWarAllianceResponse g2C_Open = (G2C_OpenWarAllianceResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenWarAllianceRequest { });
            if (g2C_Open.Error != 0)
            {
                //褰撳墠娌℃湁鎴樼洘
                WarAllianceDatas.IsJoinWar = false;
            }
            else
            {
                WarAllianceDatas.IsJoinWar = true;
            }
        }
        /// <summary>
        /// 绾㈢偣妫€娴?
        /// </summary>
        public void RedDotFriendCheack()
        {
            if (RedDotManagerComponent.redDotData.Count == 0) { return; }
            FriendMain.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Friend) > 0);
            WarAllianceMain.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_WarAlliance) > 0);
            EmailRed.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Email) > 0);
            AttributeRed.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Attribute) > 0);
            //PetRed.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Pet) > 0);
            Log.DebugBrown("绯荤粺绾㈢偣" + RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Email));
            


        }


        public async ETTask OpenMailRequest()
        {
            Log.DebugBrown("缁忚繃123");
            G2C_OpenMailResponse g2C_OpenMail = (G2C_OpenMailResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMailRequest() { });
            Log.DebugBrown("OpenMail error=" + g2C_OpenMail.Error + " count=" + g2C_OpenMail.MailList.Count);
            bool state = false;
            for (int i = 0, length = g2C_OpenMail.MailList.Count; i < length; i++)
            {
                if (g2C_OpenMail.MailList[i].MailState == 0)
                {
                    state = true;
                    break;
                }
                Debug.Log("閭欢鏁版嵁" + g2C_OpenMail.MailList[i].MailState + ":::" + g2C_OpenMail.MailList[i].ReceiveOrNot + ":::鍚嶅瓧" + g2C_OpenMail.MailList[i].MailName);
            }
            //if (g2C_OpenMail.Error == 0)
            //{
            //    bool state = false;
            //    for (int i = 0, length = g2C_OpenMail.MailList.Count; i < length; i++)
            //    {
            //        if (g2C_OpenMail.MailList[i].MailState==0)
            //        {
            //            state = true;
            //            break;
            //        }
            //        Debug.Log("閭欢鏁版嵁" + g2C_OpenMail.MailList[i].MailState + ":::" + g2C_OpenMail.MailList[i].ReceiveOrNot+":::鍚嶅瓧"+ g2C_OpenMail.MailList[i].MailName);
            //    }
            //}
            //else
            //{
            //   // UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMail.Error.GetTipInfo());
            //}
        }




        /// <summary>
        /// 鏀瑰彉鐜╁Hp
        /// </summary>
        /// <param name="value">褰撳墠琛€閲?/param>
        /// <param name="Maxvalue">鏈€澶ц閲?/param>
        public void ChangeRoleHp(float Maxvalue, float value) 
        {
            if (Maxvalue <= 0f)
            {
                return;
            }

            float Ratio = value / Maxvalue;
            if ((object)hpvalue != null)
            {
                hpvalue.text = string.Format("{0} / {1}", value, Maxvalue);
            }

            if ((object)Hp != null)
            {
                Hp.fillAmount = Ratio;
            }

            if ((object)HPValue != null)
            {
                HPValue.fillAmount = Ratio;
            }

            RoleOnHookComponent onHook = RoleOnHookComponent.Instance;
            if (onHook != null)
            {
                onHook.UseMedicnieHp(Ratio);
            }
        }
        public void ChangeRoleHp() 
        {
            if (enityProperty == null)
            {
                return;
            }

            float maxHp = enityProperty.GetProperValue(E_GameProperty.PROP_HP_MAX);
            if (maxHp <= 0f)
            {
                return;
            }

            float currentHp = enityProperty.GetProperValue(E_GameProperty.PROP_HP);
            float Ratio = currentHp / maxHp;
            if ((object)hpvalue != null)
            {
                hpvalue.text = string.Format("{0} / {1}", currentHp, maxHp);
            }

            if ((object)Hp != null)
            {
                Hp.fillAmount = Ratio;
            }

            if ((object)HPValue != null)
            {
                HPValue.fillAmount = Ratio;
            }

            RoleOnHookComponent onHook = RoleOnHookComponent.Instance;
            if (onHook != null)
            {
                onHook.UseMedicnieHp(Ratio);
            }
        }
        public void ChangeRoleMp(float curMp)
        {
            if (enityProperty == null)
            {
                return;
            }

            float maxMp = enityProperty.GetProperValue(E_GameProperty.PROP_MP_MAX);
            if (maxMp <= 0f)
            {
                return;
            }

            float Ratio = curMp / maxMp;
            if ((object)mpvalue != null)
            {
                mpvalue.text = string.Format("{0} / {1}", curMp, maxMp);
            }

            if ((object)Mp != null)
            {
                Mp.fillAmount = Ratio;
            }

            if ((object)MPValue != null)
            {
                MPValue.fillAmount = Ratio;
            }

            RoleOnHookComponent onHook = RoleOnHookComponent.Instance;
            if (onHook != null)
            {
                onHook.UseMedicineMp(Ratio);
            }
        }

        public void ChangeRoleMp() 
        {
            if (enityProperty == null)
            {
                return;
            }

            float maxMp = enityProperty.GetProperValue(E_GameProperty.PROP_MP_MAX);
            if (maxMp <= 0f)
            {
                return;
            }

            float currentMp = enityProperty.GetProperValue(E_GameProperty.PROP_MP);
            float Ratio = currentMp / maxMp;
            if ((object)mpvalue != null)
            {
                mpvalue.text = string.Format("{0} / {1}", currentMp, maxMp);
            }

            if ((object)Mp != null)
            {
                Mp.fillAmount = Ratio;
            }

            if ((object)MPValue != null)
            {
                MPValue.fillAmount = Ratio;
            }

            RoleOnHookComponent onHook = RoleOnHookComponent.Instance;
            if (onHook != null)
            {
                onHook.UseMedicineMp(Ratio);
            }

        }

        public void ChangeAG() 
        {
            if (enityProperty == null)
            {
                return;
            }

            float maxAg = enityProperty.GetProperValue(E_GameProperty.PROP_AG_MAX);
            if (maxAg <= 0f)
            {
                return;
            }

            float currentAg = enityProperty.GetProperValue(E_GameProperty.PROP_AG);
            float Ratio = currentAg / maxAg;
            if ((object)agvalue != null)
            {
                agvalue.text = string.Format("{0} / {1}", currentAg, maxAg);
            }

            if ((object)ag != null)
            {
                ag.fillAmount = Ratio;
            }
        }
        public void ChangeSD()
        {
            if (enityProperty == null)
            {
                return;
            }

            float maxSd = enityProperty.GetProperValue(E_GameProperty.PROP_SD_MAX);
            if (maxSd <= 0f)
            {
                return;
            }

            float currentSd = enityProperty.GetProperValue(E_GameProperty.PROP_SD);
            float Ratio = currentSd / maxSd;
            if ((object)sdvalue != null)
            {
                sdvalue.text = string.Format("{0} / {1}", currentSd, maxSd);
            }

            if ((object)sd != null)
            {
                sd.fillAmount = Ratio;
            }
        } 
       
    }
}
