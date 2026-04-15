using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using ILRuntime.Runtime;
using System;

namespace ETHotfix
{

    /// <summary>
    /// 副本
    /// </summary>
    public partial class UIMainComponent
    {
        private GameObject fubenobj;
        public Text fubentime;
        public Button EMoGuangChangEnterBtn, XueSeChengBaoEnterBtn, ExitFuBenBtn;
        public GameObject fubentimeBg;
        public GameObject fubeninfo;
        public Transform Fuebnitem;
        public Text skillCount;
        /// <summary>
        /// 倒计时委托
        /// </summary>
        public Action CountDownAction;
        /// <summary>
        /// 副本倒计时时间
        /// </summary>
        public long FuBenCountDownTime = 900;
        public long FuBenXueSeTime = 900;
        public long FuBenEMoTime = 900;
        public bool IsStartFubenCountDown = false;//是否开始副本倒计时
        public bool IsMonsterShow = false;//是否显示怪物数量提示
        public float fubennexttime = 0;
        public float XueSefubennexttime = 0;
        public float EMofubennexttime = 0;
        public string FuBenName = string.Empty;

        public string FuBenStartOrEnd = string.Empty;
        /// <summary>
        /// 1:大门打破，2:水晶棺打破
        /// </summary>
        public int xueSeStata;//血色城堡状态
        /// <summary>
        /// 大天使武器是否归还
        /// </summary>
        public bool weaponIsReturn = false;
        public int FuBenLevel = 1;
        public int FuBenType = 1;

        public void Init_Fuben()
        {
            weaponIsReturn = false;
            fubenobj = ReferenceCollector_Main.GetGameObject("FuBen");
            ReferenceCollector collector = fubenobj.GetReferenceCollector();
            fubentime = collector.GetText("fubentime");
            skillCount = collector.GetText("skillCount");
            fubeninfo = collector.GetGameObject("fubeninfo");
            Fuebnitem = fubeninfo.transform.GetChild(0);
            fubeninfo.gameObject.SetActive(false);
            fubentimeBg = collector.GetImage("fubentimeBg").gameObject;
            fubentimeBg.SetActive(false);
            ExitFuBenBtn = collector.GetButton("ExitFuBenBtn");
            ExitFuBenBtn.onClick.AddSingleListener(async () =>
            {
                //退出副本
                G2C_QuitBattleCopyResponse g2C_QuitBattle = (G2C_QuitBattleCopyResponse)await SessionComponent.Instance.Session.Call(new C2G_QuitBattleCopyRequest
                {
                    Level = FuBenLevel,
                    Type = FuBenType
                });
                if (g2C_QuitBattle.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_QuitBattle.Error.GetTipInfo());
                }
                else
                {
                    fubentimeBg.gameObject.SetActive(false);
                    ExitFuBenBtn.gameObject.SetActive(false);
                }
            });
            EMoGuangChangEnterBtn = collector.GetButton("EMoGuangChangeBtn");
            XueSeChengBaoEnterBtn = collector.GetButton("XueSeChengBaoBtn");
            EMoGuangChangEnterBtn.onClick.AddSingleListener(() =>
            {
                Transfer(408);//传送到冰风谷 副本入口
            });
            XueSeChengBaoEnterBtn.onClick.AddSingleListener(() =>
            {
                Transfer(408);//传送到冰风谷 副本入口
            });

            void Transfer(int mapId)
            {
                //主动暂停挂机
                if (RoleOnHookComponent.Instance.IsOnHooking)
                {
                    UIMainComponent.Instance.HookTog.isOn = false;
                }
                Map_TransferPointConfig map_Transfer = ConfigComponent.Instance.GetItem<Map_TransferPointConfig>(mapId);

                UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                confirmComponent.SetTipText($"确定花费<color=red>{map_Transfer.MapCostGold}</color>金币 传送到冰风谷副本入口？");
                confirmComponent.AddActionEvent(async () =>
                {
                    if (roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin) < map_Transfer.MapCostGold)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足 无法切换地图");
                        return;
                    }

                    G2C_MapDeliveryResponse response = (G2C_MapDeliveryResponse)await SessionComponent.Instance.Session.Call(new C2G_MapDeliveryRequest
                    {
                        MapId = map_Transfer.Id.ToInt32()
                    });
                    if (response.Error != 0)
                    {
                        Log.DebugGreen($"传送异常：{response.Error}");
                        UIComponent.Instance.VisibleUI(UIType.UIHint, response.Error.GetTipInfo());
                    }
                });
            }
            
            fubentime.gameObject.SetActive(false);

            ChangEMoGuangChangState(0);
            ChangEMoGuangChangState(false);
            ChangXueSeChengBaoState(0);
            ChangXueSeChengBaoState(false);
            CountDownAction += UpdateEMoFuCountdown;
            CountDownAction += UpdateXueSeFuCountdown;
            CountDownAction += StartFuben;
            CountDownAction += ShowNoticeInfo;
            CountDownAction += HideText;


        }
        public bool IsOpenEMoFuben = false;
        public bool IsOpenXueSeFuben = false;
        
        public void StartFubenCountDown(bool isShowExit,string name, long time,bool isMonsterShow)
        {
            IsMonsterShow = isMonsterShow;
            FuBenCountDownTime = time;
            IsStartFubenCountDown = true;
            FuBenName = name;
            fubentimeBg.gameObject.SetActive(true);
            fubentime.gameObject.SetActive(true);
            ExitFuBenBtn.gameObject.SetActive(isShowExit);
        }

        /// <summary>
        /// 副本开始 倒计时
        /// </summary>
        public void StartFuben()
        {
            if (!IsStartFubenCountDown)
            {
                return;
            }
            if (Time.time > fubennexttime)
            {
                skillCount.gameObject.SetActive(IsMonsterShow);
                --FuBenCountDownTime;
                NormTime(FuBenEMoTime, out string Mtime, out string Stime);
                fubentime.text = $"{FuBenName}还剩{FuBenCountDownTime / 60}分:{FuBenCountDownTime % 60}秒后{FuBenStartOrEnd}";
                fubennexttime = Time.time + 1;
                if (FuBenCountDownTime <= 0)
                {
                    fubentimeBg.gameObject.SetActive(false);
                    fubentime.gameObject.SetActive(false);
                    IsStartFubenCountDown = false;
                }
            }
        }
        /// <summary>
        /// 恶魔广场准备倒计时
        /// </summary>
        public void UpdateEMoFuCountdown()
        {
            if (!IsOpenEMoFuben) return;
            if (Time.time > EMofubennexttime)
            {
                EMoGuangChangEnterBtn.transform.Find("TimeTxt").gameObject.SetActive(true);
                --FuBenEMoTime;
                NormTime(FuBenEMoTime,out string Mtime,out string Stime);
                EMoGuangChangEnterBtn.transform.Find("TimeTxt").GetComponent<Text>().text = $"{Mtime}{FuBenEMoTime / 60}:{Stime}{FuBenEMoTime % 60}";
                EMofubennexttime = Time.time + 1;
                if (FuBenEMoTime <= 0)
                {
                    EMoGuangChangEnterBtn.transform.Find("TimeTxt").gameObject.SetActive(false);
                    IsOpenEMoFuben = false;
                }
            }
        }
        /// <summary>
        /// 血色城堡准备倒计时
        /// </summary>
        public void UpdateXueSeFuCountdown()
        {
            if (!IsOpenXueSeFuben) return;
            if (Time.time > XueSefubennexttime)
            {
                XueSeChengBaoEnterBtn.transform.Find("TimeTxt").gameObject.SetActive(true);
                --FuBenXueSeTime;
                NormTime(FuBenXueSeTime, out string Mtime, out string Stime);
                XueSeChengBaoEnterBtn.transform.Find("TimeTxt").GetComponent<Text>().text = $"{Mtime}{FuBenXueSeTime / 60}:{Stime}{FuBenXueSeTime % 60}";
                XueSefubennexttime = Time.time + 1;
                if (FuBenXueSeTime <= 0)
                {
                    XueSeChengBaoEnterBtn.transform.Find("TimeTxt").gameObject.SetActive(false);
                    IsOpenXueSeFuben = false;
                }
            }
        }
        public void NormTime(long time,out string Mtime,out string Stime)
        {
            if ((time / 60) / 10 < 1)
            {
                Mtime = "0";
            }
            else
            {
                Mtime = "";
            }
            if ((time % 60) / 10 < 1)
            {
                Stime = "0";
            }
            else
            {
                Stime = "";
            }
        }
        /// <summary>
        /// 改变恶魔广场的状态
        /// </summary>
        /// <param name="state">true 开启 false 关闭</param>
        public void ChangEMoGuangChangState(bool state)
        {
            //50级才可以开启副本
            EMoGuangChangEnterBtn.gameObject.SetActive(state);
        }
        public void SetEmoTips()
        {
            StartFubenCountDown(true,"恶魔广场", FuBenEMoTime, false);
        }
        /// <summary>
        /// 改变恶魔广场的状态时间
        /// </summary>
        /// <param name="state">true 开启 false 关闭</param>
        public void ChangEMoGuangChangState(long time)
        {
            FuBenEMoTime = time;
            IsOpenEMoFuben = (time > 0);
        }
        /// <summary>
        /// 改变血色城堡的状态
        /// </summary>
        /// <param name="state"></param>
        public void ChangXueSeChengBaoState(bool state)
        {
            XueSeChengBaoEnterBtn.gameObject.SetActive(state);
        }
        public void SeXueseTips()
        {
            StartFubenCountDown(true,"血色城堡", FuBenXueSeTime, false);
        }
        // <summary>
        /// 改变血色城堡的状态时间
        /// </summary>
        /// <param name="state"></param>
        public void ChangXueSeChengBaoState(long time)
        {
            FuBenXueSeTime = time;
            IsOpenXueSeFuben = (time > 0);
        }
        public void SetSkillCount(string count)
        {
            skillCount.text = count;
        }

        /// <summary>
        /// 血色城堡桥门,是否可行走
        /// </summary>
        /// <param name="stata"></param>
        public void ChangeXueSeAstar(bool stataQiao,bool stateMen)
        {
            Log.DebugGreen($"设置目标点是否可行走桥->{stataQiao},门->{stateMen}");
            AstarComponent.Instance.ChangeAstarNodeState(169, 114, stataQiao);
            AstarComponent.Instance.ChangeAstarNodeState(169, 113, stataQiao);
            AstarComponent.Instance.ChangeAstarNodeState(169, 112, stataQiao);
            AstarComponent.Instance.ChangeAstarNodeState(169, 111, stataQiao);
            AstarComponent.Instance.ChangeAstarNodeState(169, 110, stataQiao);
            AstarComponent.Instance.ChangeAstarNodeState(169, 109, stataQiao);

            AstarComponent.Instance.ChangeAstarNodeState(87, 110, stateMen);
            AstarComponent.Instance.ChangeAstarNodeState(87, 111, stateMen);
            AstarComponent.Instance.ChangeAstarNodeState(87, 112, stateMen);
            AstarComponent.Instance.ChangeAstarNodeState(87, 113, stateMen);
            AstarComponent.Instance.ChangeAstarNodeState(87, 114, stateMen);
            AstarComponent.Instance.ChangeAstarNodeState(87, 115, stateMen);
        }

        public void ShowFuBenInfo(string str)
        {
            Fuebnitem.GetComponent<Text>().text = str;
            Fuebnitem.SetAsLastSibling();
            fubeninfo.SetActive(true);
        }

        public void HideFuBenInfo() 
        {
            fubeninfo.SetActive(false);
        }
        
    }
}