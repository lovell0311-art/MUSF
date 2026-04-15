using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIYaoQingMaComponentAwake : AwakeSystem<UIYaoQingMaComponent>
    {
        public override void Awake(UIYaoQingMaComponent self)
        {
            self.referenceCollector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.referenceCollector.GetButton("CloseBtn").onClick.AddSingleListener(() => { self.Close(); });
            self.Get1Btn = self.referenceCollector.GetButton("Get1");
            self.Get2Btn = self.referenceCollector.GetButton("Get2");
            self.Get3Btn = self.referenceCollector.GetButton("Get3");
            self.copy = self.referenceCollector.GetButton("copy");
            self.yaoqingma = self.referenceCollector.GetText("tuiguangma");
            self.YaiQingSlider = self.referenceCollector.GetGameObject("YaiQingSlider").GetComponent<Slider>();
            self.slider120 = self.referenceCollector.GetGameObject("Slider120").GetComponent<Slider>();
            self.slider220 = self.referenceCollector.GetGameObject("Slider220").GetComponent<Slider>();
            self.Awake();
        }
    }
    public class UIYaoQingMaComponent : Component
    {
        public ReferenceCollector referenceCollector;
        public Button Get1Btn, Get2Btn, Get3Btn, copy;
        public Text yaoqingma;
        public Slider slider120, slider220, YaiQingSlider;
        public int AwardStatusI,AwardStatusII,AwardStatusIII;
        public void Awake()
        {
            #region 复制按钮
                #if UNITY_IOS
                                copy.gameObject.SetActive(false);
                #else
                            copy.gameObject.SetActive(true);
                #endif
            #endregion

            Get1Btn.onClick.AddSingleListener(() =>
            {
                if(AwardStatusI == 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"条件不满足！");
                }
                else if (AwardStatusI == 1)
                {
                    PromotionAward(1).Coroutine();
                }
                else if (AwardStatusI == 2)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "不能重复领取奖励！");
                }
            });
            Get2Btn.onClick.AddSingleListener(() =>
            {
                if (AwardStatusII == 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "条件不满足！");
                }
                else if(AwardStatusII == 1)
                {
                    PromotionAward(2).Coroutine();
                }
                else if(AwardStatusII == 2)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "不能重复领取奖励！");
                }
            });
            Get3Btn.onClick.AddSingleListener(() =>
            {
                if (AwardStatusIII == 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "条件不满足！");
                }
                else if (AwardStatusIII == 1)
                {
                    PromotionAward(3).Coroutine();
                }
                else if (AwardStatusIII == 2)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "不能重复领取奖励！");
                }
            });
            copy.onClick.AddSingleListener(() =>
            {
                GUIUtility.systemCopyBuffer = yaoqingma.text;
            });
            OpenInvitationInterface().Coroutine();
        }

        public async ETTask OpenInvitationInterface()
        {
            G2C_OpenInvitationInterface g2C_Open = (G2C_OpenInvitationInterface)await SessionComponent.Instance.Session.Call(new C2G_OpenInvitationInterface(){});
            if (g2C_Open.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Open.Error.GetTipInfo());
            }
            else
            {
                yaoqingma.text = g2C_Open.Info.InvitationCode;
                YaiQingSlider.value = g2C_Open.Info.NumberCntI / 10;
                YaiQingSlider.transform.Find("jinduTxt").GetComponent<Text>().text = $"{g2C_Open.Info.NumberCntI}/10";
                slider120.value = g2C_Open.Info.NumberCntII / 10;
                slider120.transform.Find("jinduTxt").GetComponent<Text>().text = $"{g2C_Open.Info.NumberCntII}/10";
                slider220.value = g2C_Open.Info.NumberCntIII / 10;
                slider220.transform.Find("jinduTxt").GetComponent<Text>().text = $"{g2C_Open.Info.NumberCntIII}/10";

                AwardStatusI = g2C_Open.Info.AwardStatusI;
                AwardStatusII = g2C_Open.Info.AwardStatusII;
                AwardStatusIII = g2C_Open.Info.AwardStatusIII;
            }
        }
        public async ETTask PromotionAward(int type)
        {
            G2C_PromotionAward g2C_Open = (G2C_PromotionAward)await SessionComponent.Instance.Session.Call(new C2G_PromotionAward()
            {
                Type = type
            });
            if (g2C_Open.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Open.Error.GetTipInfo());
            }
            else 
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功！");
            }
        }

        internal void Close()
        {
            UIComponent.Instance.Remove(UIType.UIYaoQingMa);
        }
    }
}
