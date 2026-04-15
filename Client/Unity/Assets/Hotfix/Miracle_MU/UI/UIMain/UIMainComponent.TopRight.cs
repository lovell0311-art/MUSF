using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;
using UnityEngine.PlayerLoop;
using System.Reflection;

namespace ETHotfix
{   

    /// <summary>
    /// 宸︿笂瑙掓寜閽ā鍧?
    /// </summary>
    public partial class UIMainComponent
    {
        

        /// <summary>
        /// 灏忔湀鍗″€掕鏃?鏄剧ず鍥炶皟
        /// </summary>
        public Action<TimeSpan> UpdateMCCountDownAction;
        /// <summary>
        /// 澶ф湀鍗″€掕鏃?鏄剧ず鍥炶皟
        /// </summary>
        public Action<TimeSpan> UpdateMaxCCountDownAction;
        //鍘熷湴澶嶆椿CD
        public Action<TimeSpan> UpdateInSituCdAction; 
        //绾㈠悕CD
        public Action UpdateRedNameCdAction;

        public TimeSpan subTimeSpan = new TimeSpan(0, 0, 1);
        private Action minMonthCardTickAction;
        private Action maxMonthCardTickAction;
        private Action inSituCdTickAction;
        private Action pkNumberTickAction;
        private float minMonthCardNextTime = 1f;
        private float maxMonthCardNextTime = 1f;
        private float inSituCdNextTime = 1f;
        private float pkNumberNextTime = 1f;
        private bool minMonthCardTimerActive;
        private bool maxMonthCardTimerActive;
        private bool inSituCdTimerActive;
        private bool pkNumberTimerActive;

        public GameObject TogRightObj;
        ReferenceCollector referenceCollector_TopRigh;
        public Button dayTopupBtn,LimtBtn,Btn51, choujiangBtn,BtnOpen,BtnClose;
        public void Init_TopRight()
        {
            TogRightObj = ReferenceCollector_Main.GetGameObject("TopRight");
            referenceCollector_TopRigh = ReferenceCollector_Main.GetGameObject("TopRight").GetReferenceCollector();
            //璁剧疆鐩告満浣嶇疆
            referenceCollector_TopRigh.GetButton("camesetBtn").onClick.AddSingleListener(() => { UIComponent.Instance.VisibleUI(UIType.UISetCameraAtr); });
           
            //鍟嗗煄
            referenceCollector_TopRigh.GetButton("shopBtn").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UIShop, (int)E_TopUpType.ShopTopUp));
            //绛惧埌
            referenceCollector_TopRigh.GetButton("qiandao").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UIQianDao_9_Day));
            //绛惧埌
            referenceCollector_TopRigh.GetButton("shangjin").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UINewYearActivity));
            //棣栧厖
            referenceCollector_TopRigh.GetButton("shouchongBtn").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UIFirstCharge));
            //鍐插埡
            referenceCollector_TopRigh.GetButton("shengjiBtn").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UISprint)); 
            //钘忓疂闃?
            referenceCollector_TopRigh.GetButton("cangbaoge").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UITreasureHouse));
            //娲诲姩
            referenceCollector_TopRigh.GetButton("huodongBtn").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UINewYearActivity));
            //绛夌骇濂栧姳
            referenceCollector_TopRigh.GetButton("levelJiangLi").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UI_RankReward));
            //鍦ㄧ嚎濂栧姳
            referenceCollector_TopRigh.GetButton("ZaiXian").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UIZaiXian));
            //涓冩棩鍏呭€?
            referenceCollector_TopRigh.GetButton("7dayTopup").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.TopUp_7_Day));
            dayTopupBtn = referenceCollector_TopRigh.GetButton("7dayTopup");
            //闄愭椂鍏呭€?
            referenceCollector_TopRigh.GetButton("LimiTopUp").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UI_LimitTopUpActivity));
            LimtBtn = referenceCollector_TopRigh.GetButton("LimiTopUp");
            //浜斾竴娲诲姩路
            referenceCollector_TopRigh.GetButton("51").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UI_51Active));
            Btn51 = referenceCollector_TopRigh.GetButton("51");
            //閫氳璇?
            referenceCollector_TopRigh.GetButton("passportBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.VisibleUI(UIType.UIPassport);
            });
            //------------鏂板鎺у埗鎸夐挳
            BtnOpen = referenceCollector_TopRigh.GetButton("BtnOpen");
            BtnClose = referenceCollector_TopRigh.GetButton("BtnClose");
            BtnOpen.onClick.AddSingleListener(() =>
            {
                BtnOpen.gameObject.SetActive(false);
                BtnClose.gameObject.SetActive(true);
                referenceCollector_TopRigh.GetImage("Scroll View").gameObject.SetActive(true);
                referenceCollector_TopRigh.GetGameObject("obj").gameObject.SetActive(true);

            });
            BtnClose.onClick.AddSingleListener(() =>
            {
                BtnOpen.gameObject.SetActive(true);
                BtnClose.gameObject.SetActive(false);
                referenceCollector_TopRigh.GetGameObject("obj").gameObject.SetActive(false);
                referenceCollector_TopRigh.GetImage("Scroll View").gameObject.SetActive(false);

            });
            //鎶藉
            choujiangBtn =referenceCollector_TopRigh.GetButton("choujiang");
            choujiangBtn.onClick.AddSingleListener(() => 
            {
                UIComponent.Instance.VisibleUI(UIType.UIChouJiang);
            });

            //鍏呭€兼帓琛屾
            referenceCollector_TopRigh.GetButton("chongzhiBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.VisibleUI(UIType.UICongZhiPaiHangBang);
            });
            //骞歌繍鎶藉
            referenceCollector_TopRigh.GetButton("Welfare").onClick.AddSingleListener(() =>
            {
                Log.DebugBrown("璇锋眰鎵撳紑绂忓埄娲诲姩");
                UIComponent.Instance.VisibleUI(UIType.UIWelfare);
            });
            //寮€鎸傜ぜ鍖?
            referenceCollector_TopRigh.GetButton("reclamationBtn").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UIReclamation));
            //寮€鎸傜ぜ鍖?
            referenceCollector_TopRigh.GetButton("Awakening").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UIAwakening));
            //瓒呭€肩ぜ鍖?
            referenceCollector_TopRigh.GetButton("TangibleLimit").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UITangibleLimit));
            //瓒呭€肩ぜ鍖?
            referenceCollector_TopRigh.GetButton("MonthCard").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UIMonthCard));
            //闄愯喘绀煎寘
            referenceCollector_TopRigh.GetButton("PurchaseLimit").onClick.AddSingleListener(() => UIComponent.Instance.VisibleUI(UIType.UIPurchaseLimit));
            //鍏呭€兼寜閽姌鍙?
            referenceCollector_TopRigh.GetToggle("BtnTog").onValueChanged.AddSingleListener((value) => 
            {
                referenceCollector_TopRigh.GetToggle("BtnTog").transform.GetChild(0).GetComponent<Image>().enabled = !value;
                
                TogRightObj.SetActive(!value);
            });
            //鎸戞垬Boss
            referenceCollector_TopRigh.GetButton("Challenge").onClick.AddSingleListener(() => 
            {
                UIComponent.Instance.VisibleUI(UIType.UIChallenge);
            });

            //鏌ヨ
            referenceCollector_TopRigh.GetButton("chaxun").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.VisibleUI(UIType.UIChaXun);
            });

            //绱鍏呭€?
            referenceCollector_TopRigh.GetButton("TopUpRewards").onClick.AddSingleListener(() => 
            {
                UIComponent.Instance.VisibleUI(UIType.UITopUpRewards);

            });
            //referenceCollector_TopRigh.GetToggle("BtnTog").isOn = true;

            // Game.EventManager.AddEventHandler(EventTypeId.LOCALROLE_DEAD, delegate { HookTog.isOn = false; });
            

            //鍏抽棴娲诲姩
            //Btn51.gameObject.SetActive(true);
          //  LimtBtn.gameObject.SetActive(false);
            /*dayTopup_7.gameObject.SetActive(roleEntity.Level >= 10);
            LimiTopUp.gameObject.SetActive(roleEntity.Level >= 50);
            Btn_51.gameObject.SetActive(roleEntity.Level >= 100);*/
            //CloseActivity();
        }
        //鏃堕棿鍒颁簡鑷姩鍏抽棴娲诲姩
       /* public void CloseActivity() 
        {
            *//* DateTime starttime =System.DateTime.Now;
             DateTime endtime = Convert.ToDateTime(info.EndTime);*//*
            //  DateTime starttime = Convert.ToDateTime("2023-10-31 23:00:00");

            var info = ConfigComponent.Instance.GetItem<Activity_InfoConfig>(3);
            DateTime endtime = Convert.ToDateTime(info.EndTime);
           // DateTime endtime = Convert.ToDateTime("2023-05-05 23:00:00");
            closeSpane = endtime.Subtract(DateTime.Now);

            if ((long)closeSpane.TotalMilliseconds > 0)
            {
                TimerComponent.Instance.RegisterTimeCallBack((long)closeSpane.TotalMilliseconds, () =>
                {
                   
                    LimiTopUp.gameObject.SetActive(false);
                    Btn_51.gameObject.SetActive(false);
                });

            }
            else
            {
                LimiTopUp.gameObject.SetActive(false);
                Btn_51.gameObject.SetActive(false);
            }
            SetTopUpHint(roleEntity.Level);

        }*/
       public  async ETVoid GetCurTopupInfo()
        {
            G2C_PlayerShopInfoResponse g2C_PlayerShopInfo = (G2C_PlayerShopInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_PlayerShopInfoRequest { });
            if (g2C_PlayerShopInfo.Error != 0)
            {
             //   Log.DebugRed($"{g2C_PlayerShopInfo.Error.GetTipInfo()}");
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_PlayerShopInfo.Error.GetTipInfo());
            }
            else
            {
                roleEntity.Property.ChangeProperValue(E_GameProperty.MiracleCoin, g2C_PlayerShopInfo.Info.MiracleCoin);//濂囪抗甯?
                roleEntity.Property.ChangeProperValue(E_GameProperty.MoJing, g2C_PlayerShopInfo.Info.CurrentYuanbao);//鍏冨疂鏁伴噺
                roleEntity.Property.ChangeProperValue(E_GameProperty.AccumulatedRecharge, g2C_PlayerShopInfo.Info.AccumulatedRecharge);//绱鍏呭€奸搴?
                roleEntity.RechargeStates = g2C_PlayerShopInfo.Info.RechargeStatus;//棣栧厖杩涘害鐘舵€?
               

                if (g2C_PlayerShopInfo.Info.MinMCEndTime != 0)
                    roleEntity.MinMonthluCardTimeSpan = TimeHelper.GetSpacingTime_Milliseconds(g2C_PlayerShopInfo.Info.MinMCEndTime);//灏忔湀鍗″埌鏈熸椂闂?
                if (g2C_PlayerShopInfo.Info.MaxMCEndTime != 0)
                    roleEntity.MaxMonthluCardTimeSpan = TimeHelper.GetSpacingTime_Milliseconds(g2C_PlayerShopInfo.Info.MaxMCEndTime);//澶ф湀鍗″埌鏈熸椂闂?
                if (g2C_PlayerShopInfo.Info.InSituCd != 0)
                    roleEntity.InsiteTimeSpan = TimeHelper.GetSpacingTime_Milliseconds(g2C_PlayerShopInfo.Info.InSituCd);//鍘熷湴澶嶆椿

                roleEntity.Property.ChangeProperValue(E_GameProperty.InSituCd, g2C_PlayerShopInfo.Info.InSituCd);//鍘熷湴澶嶆椿CD

                ///璐拱浜嗗皬鏈堝崱
                if (g2C_PlayerShopInfo.Info.MinMCEndTime != 0)
                {
                    //MinMothCard();

                }
                ///璐拱浜嗗ぇ鏈堝崱 
                if (g2C_PlayerShopInfo.Info.MaxMCEndTime != 0)
                {
                   // MaxMothCard();
                }
                //鍘熷湴澶嶆椿鐗规潈
                if (roleEntity.InSitu && g2C_PlayerShopInfo.Info.InSituCd != 0)
                {
                   // InSituCd();
                }
            }
        }
        /// <summary>
        /// 璁剧疆鍏呭€奸殣钘?
        /// </summary>
        public void SetTopUpHint(int level)
        {
            
           // dayTopupBtn.gameObject.SetActive(level >= 1);
           // referenceCollector_TopRigh.GetButton("LimiTopUp").gameObject.SetActive(level >= 50);
           // referenceCollector_TopRigh.GetButton("51").gameObject.SetActive(level >= 100);
        }
        /// <summary>
        /// 灏忔湀鍗?鍊掕鏃?
        /// </summary>
        public void MinMothCard() 
        {
            return;
#if false

            var nextTime = 1f;
            CountDownAction += UpdateMacMin;
            void UpdateMacMin()
            {
                if (roleEntity.MinMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    //鏈堝崱宸插埌鏈?
                    CountDownAction -= UpdateMacMin;
                   // Log.DebugRed($"灏忔湀鍗″埌鏈?绉婚櫎鐩戝惉");
                    return;
                }
                if (Time.time >= nextTime)
                {
                    roleEntity.MinMonthluCardTimeSpan = roleEntity.MinMonthluCardTimeSpan.Subtract(subTimeSpan);
                    //Log.DebugGreen($"灏忔湀鍗℃椂闂?-銆媨roleEntity.MinMonthluCardTimeSpan}");
                    UpdateMCCountDownAction?.Invoke(roleEntity.MinMonthluCardTimeSpan);
                    nextTime = Time.time + 1;
                }
            }
#endif
        }
        /// <summary>
        /// 澶ф湀鍗?鍊掕鏃?
        /// </summary>
        public void MaxMothCard()
        {
            return;
#if false

            var nextTime = 1f;
            CountDownAction += UpdateMacMax;
            void UpdateMacMax()
            {
                if (roleEntity.MaxMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    //鏈堝崱宸插埌鏈?
                    CountDownAction -= UpdateMacMax;
                   // Log.DebugRed($"澶ф湀鍗″埌鏈?绉婚櫎鐩戝惉");
                    return;
                }
                if (Time.time >= nextTime)
                {
                    roleEntity.MaxMonthluCardTimeSpan = roleEntity.MaxMonthluCardTimeSpan.Subtract(subTimeSpan);
                    UpdateMaxCCountDownAction?.Invoke(roleEntity.MaxMonthluCardTimeSpan);
                    nextTime = Time.time + 1;
                }
            }
#endif
        }
        /// <summary>
        /// 鍘熷湴澶嶆椿CD 鍊掕鏃?
        /// </summary>
        public void InSituCd() 
        {
            return;
#if false

            float InSitunextTime = 1f;
            CountDownAction += InSituCallBack;
            void InSituCallBack()
            {
                if (roleEntity.InsiteTimeSpan.TotalSeconds <= 0)
                {
                    //鏈堝崱宸插埌鏈?
                    CountDownAction -= InSituCallBack;
                   
                    return;
                }
                if (Time.time >= InSitunextTime)
                {
                    roleEntity.InsiteTimeSpan = roleEntity.InsiteTimeSpan.Subtract(subTimeSpan);
                    UpdateInSituCdAction?.Invoke(roleEntity.InsiteTimeSpan);
                    InSitunextTime = Time.time + 1;
                }
            }
#endif
        }
        /// <summary>
        /// 鍒锋柊Pk鎯╃綒鐐规暟
        /// </summary>

        public void PkNumber()
        {
            return;
#if false

            var nextTime = 1f;
            foreach (var item in CountDownAction.GetInvocationList())
            {
                //Log.DebugGreen($"CountDownAction.GetInvocationList():{item.GetMethodInfo().Name}");
                if (item.GetMethodInfo().Name == "UpdatePk")
                {
                    return;
                }
            }
            CountDownAction += UpdatePk;
            void UpdatePk()
            {
                if (roleEntity.Property.GetProperValue(E_GameProperty.PkNumber)<= 0)
                {
                    //Pk鎯╃綒鐐规暟
                    this.roleEntity.GetComponent<UIUnitEntityHpBarComponent>().ChangeNameColor(ColorTools.GetColorHtmlString(Color.yellow));
                    CountDownAction -= UpdatePk;
                    return;
                }
                if (Time.time >= nextTime)
                {
                    var point = roleEntity.Property.GetProperValue(E_GameProperty.PkNumber);
                    point -= 2;
                    roleEntity.Property.ChangeProperValue(E_GameProperty.PkNumber, point);
                    UpdateRedNameCdAction?.Invoke();
                    nextTime = Time.time + 1;
                }
            }
#endif
        }

        private void EnsureCountDownRegistered(Action action)
        {
            if (action == null)
            {
                return;
            }
            CountDownAction -= action;
            CountDownAction += action;
        }

        private void ScheduleMinMonthCardTick()
        {
            TimerComponent.Instance?.RegisterTimeCallBack(1000, () =>
            {
                if (this.IsDisposed || !minMonthCardTimerActive || roleEntity == null)
                {
                    minMonthCardTimerActive = false;
                    return;
                }

                if (roleEntity.MinMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    minMonthCardTimerActive = false;
                    return;
                }

                roleEntity.MinMonthluCardTimeSpan = roleEntity.MinMonthluCardTimeSpan.Subtract(subTimeSpan);
                UpdateMCCountDownAction?.Invoke(roleEntity.MinMonthluCardTimeSpan);

                if (roleEntity.MinMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    minMonthCardTimerActive = false;
                    return;
                }

                ScheduleMinMonthCardTick();
            });
        }

        private void ScheduleMaxMonthCardTick()
        {
            TimerComponent.Instance?.RegisterTimeCallBack(1000, () =>
            {
                if (this.IsDisposed || !maxMonthCardTimerActive || roleEntity == null)
                {
                    maxMonthCardTimerActive = false;
                    return;
                }

                if (roleEntity.MaxMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    maxMonthCardTimerActive = false;
                    return;
                }

                roleEntity.MaxMonthluCardTimeSpan = roleEntity.MaxMonthluCardTimeSpan.Subtract(subTimeSpan);
                UpdateMaxCCountDownAction?.Invoke(roleEntity.MaxMonthluCardTimeSpan);

                if (roleEntity.MaxMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    maxMonthCardTimerActive = false;
                    return;
                }

                ScheduleMaxMonthCardTick();
            });
        }

        private void ScheduleInSituCdTick()
        {
            TimerComponent.Instance?.RegisterTimeCallBack(1000, () =>
            {
                if (this.IsDisposed || !inSituCdTimerActive || roleEntity == null)
                {
                    inSituCdTimerActive = false;
                    return;
                }

                if (roleEntity.InsiteTimeSpan.TotalSeconds <= 0)
                {
                    inSituCdTimerActive = false;
                    return;
                }

                roleEntity.InsiteTimeSpan = roleEntity.InsiteTimeSpan.Subtract(subTimeSpan);
                UpdateInSituCdAction?.Invoke(roleEntity.InsiteTimeSpan);

                if (roleEntity.InsiteTimeSpan.TotalSeconds <= 0)
                {
                    inSituCdTimerActive = false;
                    return;
                }

                ScheduleInSituCdTick();
            });
        }

        private void SchedulePkTick()
        {
            TimerComponent.Instance?.RegisterTimeCallBack(1000, () =>
            {
                if (this.IsDisposed || !pkNumberTimerActive || roleEntity == null || roleEntity.Property == null)
                {
                    pkNumberTimerActive = false;
                    return;
                }

                long point = roleEntity.Property.GetProperValue(E_GameProperty.PkNumber);
                if (point <= 0)
                {
                    this.roleEntity.GetComponent<UIUnitEntityHpBarComponent>()?.ChangeNameColor(ColorTools.GetColorHtmlString(Color.yellow));
                    pkNumberTimerActive = false;
                    return;
                }

                point -= 2;
                roleEntity.Property.ChangeProperValue(E_GameProperty.PkNumber, point);
                UpdateRedNameCdAction?.Invoke();

                if (point <= 0)
                {
                    this.roleEntity.GetComponent<UIUnitEntityHpBarComponent>()?.ChangeNameColor(ColorTools.GetColorHtmlString(Color.yellow));
                    pkNumberTimerActive = false;
                    return;
                }

                SchedulePkTick();
            });
        }

        private void UpdateMinMonthCardTick()
        {
            if (roleEntity == null || roleEntity.MinMonthluCardTimeSpan.TotalSeconds <= 0)
            {
                CountDownAction -= minMonthCardTickAction;
                return;
            }

            if (Time.time < minMonthCardNextTime)
            {
                return;
            }

            roleEntity.MinMonthluCardTimeSpan = roleEntity.MinMonthluCardTimeSpan.Subtract(subTimeSpan);
            UpdateMCCountDownAction?.Invoke(roleEntity.MinMonthluCardTimeSpan);
            minMonthCardNextTime = Time.time + 1f;
        }

        private void UpdateMaxMonthCardTick()
        {
            if (roleEntity == null || roleEntity.MaxMonthluCardTimeSpan.TotalSeconds <= 0)
            {
                CountDownAction -= maxMonthCardTickAction;
                return;
            }

            if (Time.time < maxMonthCardNextTime)
            {
                return;
            }

            roleEntity.MaxMonthluCardTimeSpan = roleEntity.MaxMonthluCardTimeSpan.Subtract(subTimeSpan);
            UpdateMaxCCountDownAction?.Invoke(roleEntity.MaxMonthluCardTimeSpan);
            maxMonthCardNextTime = Time.time + 1f;
        }

        private void UpdateInSituCdTick()
        {
            if (roleEntity == null || roleEntity.InsiteTimeSpan.TotalSeconds <= 0)
            {
                CountDownAction -= inSituCdTickAction;
                return;
            }

            if (Time.time < inSituCdNextTime)
            {
                return;
            }

            roleEntity.InsiteTimeSpan = roleEntity.InsiteTimeSpan.Subtract(subTimeSpan);
            UpdateInSituCdAction?.Invoke(roleEntity.InsiteTimeSpan);
            inSituCdNextTime = Time.time + 1f;
        }

        private void UpdatePkTick()
        {
            if (roleEntity == null || roleEntity.Property == null || roleEntity.Property.GetProperValue(E_GameProperty.PkNumber) <= 0)
            {
                if (roleEntity != null)
                {
                    this.roleEntity.GetComponent<UIUnitEntityHpBarComponent>()?.ChangeNameColor(ColorTools.GetColorHtmlString(Color.yellow));
                }

                CountDownAction -= pkNumberTickAction;
                return;
            }

            if (Time.time < pkNumberNextTime)
            {
                return;
            }

            long point = roleEntity.Property.GetProperValue(E_GameProperty.PkNumber);
            point -= 2;
            roleEntity.Property.ChangeProperValue(E_GameProperty.PkNumber, point);
            UpdateRedNameCdAction?.Invoke();
            pkNumberNextTime = Time.time + 1f;
        }


    }

}
