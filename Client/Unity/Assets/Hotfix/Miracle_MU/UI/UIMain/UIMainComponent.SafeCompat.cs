using System;
using UnityEngine;
using UnityEngine.UI;
using ETModel;

namespace ETHotfix
{
    public partial class UIMainComponent
    {
        private Button SafeGetTopRightButton(string key)
        {
            return referenceCollector_TopRigh?.GetButton(key);
        }

        private Toggle SafeGetTopRightToggle(string key)
        {
            return referenceCollector_TopRigh?.GetToggle(key);
        }

        private Image SafeGetTopRightImage(string key)
        {
            return referenceCollector_TopRigh?.GetImage(key);
        }

        private GameObject SafeGetTopRightObject(string key)
        {
            return referenceCollector_TopRigh?.GetGameObject(key);
        }

        private Button SafeGetBottomButton(string key)
        {
            return referenceCollector_BottomCenter?.GetButton(key);
        }

        private Toggle SafeGetBottomToggle(string key)
        {
            return referenceCollector_BottomCenter?.GetToggle(key);
        }

        private Image SafeGetBottomImage(string key)
        {
            return referenceCollector_BottomCenter?.GetImage(key);
        }

        private Text SafeGetBottomText(string key)
        {
            return referenceCollector_BottomCenter?.GetText(key);
        }

        private static void SafeSetActive(GameObject target, bool isActive)
        {
            if (target != null)
            {
                target.SetActive(isActive);
            }
        }

        private void BindTopRightButtonSafe(string key, UnityEngine.Events.UnityAction action)
        {
            Button button = SafeGetTopRightButton(key);
            if (button != null)
            {
                button.onClick.AddSingleListener(action);
            }
        }

        private void BindBottomButtonSafe(string key, UnityEngine.Events.UnityAction action)
        {
            Button button = SafeGetBottomButton(key);
            if (button != null)
            {
                button.onClick.AddSingleListener(action);
            }
        }

        public void Init_TopRight_Safe()
        {
            TogRightObj = ReferenceCollector_Main?.GetGameObject("TopRight");
            if (TogRightObj == null)
            {
                return;
            }

            referenceCollector_TopRigh = TogRightObj.GetReferenceCollector();
            if (referenceCollector_TopRigh == null)
            {
                return;
            }

            BindTopRightButtonSafe("camesetBtn", () => UIComponent.Instance.VisibleUI(UIType.UISetCameraAtr));
            BindTopRightButtonSafe("shopBtn", () => UIComponent.Instance.VisibleUI(UIType.UIShop, (int)E_TopUpType.ShopTopUp));
            BindTopRightButtonSafe("qiandao", () => UIComponent.Instance.VisibleUI(UIType.UIQianDao_9_Day));
            BindTopRightButtonSafe("shangjin", () => UIComponent.Instance.VisibleUI(UIType.UINewYearActivity));
            BindTopRightButtonSafe("shouchongBtn", () => UIComponent.Instance.VisibleUI(UIType.UIFirstCharge));
            BindTopRightButtonSafe("shengjiBtn", () => UIComponent.Instance.VisibleUI(UIType.UISprint));
            BindTopRightButtonSafe("cangbaoge", () => UIComponent.Instance.VisibleUI(UIType.UITreasureHouse));
            BindTopRightButtonSafe("huodongBtn", () => UIComponent.Instance.VisibleUI(UIType.UINewYearActivity));
            BindTopRightButtonSafe("levelJiangLi", () => UIComponent.Instance.VisibleUI(UIType.UI_RankReward));
            BindTopRightButtonSafe("ZaiXian", () => UIComponent.Instance.VisibleUI(UIType.UIZaiXian));
            BindTopRightButtonSafe("7dayTopup", () => UIComponent.Instance.VisibleUI(UIType.TopUp_7_Day));
            dayTopupBtn = SafeGetTopRightButton("7dayTopup");
            BindTopRightButtonSafe("LimiTopUp", () => UIComponent.Instance.VisibleUI(UIType.UI_LimitTopUpActivity));
            LimtBtn = SafeGetTopRightButton("LimiTopUp");
            BindTopRightButtonSafe("51", () => UIComponent.Instance.VisibleUI(UIType.UI_51Active));
            Btn51 = SafeGetTopRightButton("51");
            BindTopRightButtonSafe("passportBtn", () => UIComponent.Instance.VisibleUI(UIType.UIPassport));

            BtnOpen = SafeGetTopRightButton("BtnOpen");
            BtnClose = SafeGetTopRightButton("BtnClose");
            Image scrollViewImage = SafeGetTopRightImage("Scroll View");
            GameObject scrollViewObject = scrollViewImage != null ? scrollViewImage.gameObject : null;
            GameObject extraObject = SafeGetTopRightObject("obj");

            if (BtnOpen != null)
            {
                BtnOpen.onClick.AddSingleListener(() =>
                {
                    SafeSetActive(BtnOpen.gameObject, false);
                    SafeSetActive(BtnClose != null ? BtnClose.gameObject : null, true);
                    SafeSetActive(scrollViewObject, true);
                    SafeSetActive(extraObject, true);
                });
            }

            if (BtnClose != null)
            {
                BtnClose.onClick.AddSingleListener(() =>
                {
                    SafeSetActive(BtnOpen != null ? BtnOpen.gameObject : null, true);
                    SafeSetActive(BtnClose.gameObject, false);
                    SafeSetActive(extraObject, false);
                    SafeSetActive(scrollViewObject, false);
                });
            }

            choujiangBtn = SafeGetTopRightButton("choujiang");
            if (choujiangBtn != null)
            {
                choujiangBtn.onClick.AddSingleListener(() =>
                {
                    UIComponent.Instance.VisibleUI(UIType.UIChouJiang);
                });
            }

            BindTopRightButtonSafe("chongzhiBtn", () => UIComponent.Instance.VisibleUI(UIType.UICongZhiPaiHangBang));
            BindTopRightButtonSafe("Welfare", () => UIComponent.Instance.VisibleUI(UIType.UIWelfare));
            BindTopRightButtonSafe("reclamationBtn", () => UIComponent.Instance.VisibleUI(UIType.UIReclamation));
            BindTopRightButtonSafe("Awakening", () => UIComponent.Instance.VisibleUI(UIType.UIAwakening));
            BindTopRightButtonSafe("TangibleLimit", () => UIComponent.Instance.VisibleUI(UIType.UITangibleLimit));
            BindTopRightButtonSafe("MonthCard", () => UIComponent.Instance.VisibleUI(UIType.UIMonthCard));
            BindTopRightButtonSafe("PurchaseLimit", () => UIComponent.Instance.VisibleUI(UIType.UIPurchaseLimit));
            BindTopRightButtonSafe("Challenge", () => UIComponent.Instance.VisibleUI(UIType.UIChallenge));
            BindTopRightButtonSafe("chaxun", () => UIComponent.Instance.VisibleUI(UIType.UIChaXun));
            BindTopRightButtonSafe("TopUpRewards", () => UIComponent.Instance.VisibleUI(UIType.UITopUpRewards));

            Toggle btnTog = SafeGetTopRightToggle("BtnTog");
            if (btnTog != null)
            {
                btnTog.onValueChanged.AddSingleListener((value) =>
                {
                    Transform iconRoot = btnTog.transform.childCount > 0 ? btnTog.transform.GetChild(0) : null;
                    Image iconImage = iconRoot != null ? iconRoot.GetComponent<Image>() : null;
                    if (iconImage != null)
                    {
                        iconImage.enabled = !value;
                    }

                    SafeSetActive(TogRightObj, !value);
                });
            }
        }

        public void Init_BottomCenter_Safe()
        {
            enityProperty = roleEntity != null ? roleEntity.Property : null;
            Image bottomCenterRoot = ReferenceCollector_Main?.GetImage("BottonCenter");
            if (enityProperty == null || bottomCenterRoot == null)
            {
                return;
            }

            referenceCollector_BottomCenter = bottomCenterRoot.gameObject.GetReferenceCollector();
            if (referenceCollector_BottomCenter == null)
            {
                return;
            }

            if (!GlobalDataManager.IsEnteringGame)
            {
                WarAlliance().Coroutine();
            }

            hpvalue = SafeGetBottomText("hpvalue");
            mpvalue = SafeGetBottomText("mpValue");
            sdvalue = SafeGetBottomText("sdTxt");
            agvalue = SafeGetBottomText("agTxt");

            Image btnListImage = SafeGetBottomImage("BtnList");
            BtnList = btnListImage != null ? btnListImage.GetComponent<ScrollRect>() : null;
            EnsureBottomButtonListCanScroll();
            Gongneng = SafeGetBottomToggle("Gongneng");

            Hp = SafeGetBottomImage("HP");
            Mp = SafeGetBottomImage("Mp");
            ag = SafeGetBottomImage("AG");
            sd = SafeGetBottomImage("SD");
            HPValue = SafeGetBottomImage("HPValue");
            MPValue = SafeGetBottomImage("MPValue");
            FriendMain = SafeGetBottomImage("FriendMain");
            WarAllianceMain = SafeGetBottomImage("WarAllianceRedDot");
            EmailRed = SafeGetBottomImage("EmailRed");
            AttributeRed = SafeGetBottomImage("AttributeRed");
            PetRed = SafeGetBottomImage("PetRed");

            BindBottomButtonSafe("knapsackBtn", () => UIComponent.Instance.VisibleUI(UIType.UIKnapsackNew));
            BindBottomButtonSafe("skillBtn", () => UIComponent.Instance.VisibleUI(UIType.UISkill));
            BindBottomButtonSafe("Awakening", () => UIComponent.Instance.VisibleUI(UIType.UIAwakening));
            BindBottomButtonSafe("FriendBtn", () => UIComponent.Instance.VisibleUI(UIType.UIFirendList));
            BindBottomButtonSafe("AttributeBtn", () => UIComponent.Instance.VisibleUI(UIType.UIRoleInfo));
            BindBottomButtonSafe("MasterBtn", TryOpenMasterPanel);
            BindBottomButtonSafe("ZhanMengBtn", () =>
            {
                if (WarAllianceDatas.IsJoinWar)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIWarAlliance);
                    return;
                }

                UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText("Please go to the Ice Wind Valley War Alliance envoy at 324,329.");
            });
            BindBottomButtonSafe("E_MailBtn", () => UIComponent.Instance.VisibleUI(UIType.UIE_Mail));
            BindBottomButtonSafe("SetBtn", () => UIComponent.Instance.VisibleUI(UIType.UIGameSet));
            BindBottomButtonSafe("PetBtn", () => UIComponent.Instance.VisibleUI(UIType.UIPet));
            BindBottomButtonSafe("MountBtn", () => UIComponent.Instance.VisibleUI(UIType.UIMount));

            if (Gongneng != null)
            {
                Gongneng.onValueChanged.AddSingleListener((isOn) =>
                {
                    SafeSetActive(btnListImage != null ? btnListImage.gameObject : null, isOn);
                    if (isOn)
                    {
                        EnsureBottomButtonListCanScroll();
                    }
                });
            }

            RefreshBottomCenterVitalsSafe();
            RedDotFriendCheackSafe();
        }

        private void RefreshBottomCenterVitalsSafe()
        {
            if (enityProperty == null)
            {
                return;
            }

            SetBottomValue(hpvalue, E_GameProperty.PROP_HP, E_GameProperty.PROP_HP_MAX);
            SetBottomValue(mpvalue, E_GameProperty.PROP_MP, E_GameProperty.PROP_MP_MAX);
            SetBottomValue(agvalue, E_GameProperty.PROP_AG, E_GameProperty.PROP_AG_MAX);
            SetBottomValue(sdvalue, E_GameProperty.PROP_SD, E_GameProperty.PROP_SD_MAX);

            SetFillAmount(Hp, HPValue, E_GameProperty.PROP_HP, E_GameProperty.PROP_HP_MAX);
            SetFillAmount(Mp, MPValue, E_GameProperty.PROP_MP, E_GameProperty.PROP_MP_MAX);
            SetFillAmount(ag, null, E_GameProperty.PROP_AG, E_GameProperty.PROP_AG_MAX);
            SetFillAmount(sd, null, E_GameProperty.PROP_SD, E_GameProperty.PROP_SD_MAX);

            float hpRatio = GetPropertyRatio(E_GameProperty.PROP_HP, E_GameProperty.PROP_HP_MAX);
            float mpRatio = GetPropertyRatio(E_GameProperty.PROP_MP, E_GameProperty.PROP_MP_MAX);
            RoleOnHookComponent.Instance?.UseMedicnieHp(hpRatio);
            RoleOnHookComponent.Instance?.UseMedicineMp(mpRatio);
        }

        private void SetBottomValue(Text target, E_GameProperty current, E_GameProperty max)
        {
            if (target == null || enityProperty == null)
            {
                return;
            }

            long currentValue = enityProperty.GetProperValue(current);
            long maxValue = enityProperty.GetProperValue(max);
            target.text = $"{currentValue} / {maxValue}";
        }

        private void SetFillAmount(Image primary, Image secondary, E_GameProperty current, E_GameProperty max)
        {
            float ratio = GetPropertyRatio(current, max);
            if (primary != null)
            {
                primary.fillAmount = ratio;
            }

            if (secondary != null)
            {
                secondary.fillAmount = ratio;
            }
        }

        private float GetPropertyRatio(E_GameProperty current, E_GameProperty max)
        {
            if (enityProperty == null)
            {
                return 0f;
            }

            long maxValue = enityProperty.GetProperValue(max);
            if (maxValue <= 0)
            {
                return 0f;
            }

            long currentValue = enityProperty.GetProperValue(current);
            return Mathf.Clamp01((float)currentValue / maxValue);
        }

        private void RedDotFriendCheackSafe()
        {
            if (RedDotManagerComponent.redDotData == null || RedDotManagerComponent.redDotData.Count == 0)
            {
                return;
            }

            if (FriendMain != null)
            {
                FriendMain.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Friend) > 0);
            }

            if (WarAllianceMain != null)
            {
                WarAllianceMain.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_WarAlliance) > 0);
            }

            if (EmailRed != null)
            {
                EmailRed.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Email) > 0);
            }

            if (AttributeRed != null)
            {
                AttributeRed.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Attribute) > 0);
            }
        }
    }
}
