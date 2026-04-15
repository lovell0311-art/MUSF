using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace ETHotfix
{

    public enum E_TipPanelType
    {
    NorMal,
    WareHouse,
    StallUp,
    Split,
    FuBenReward,
    ChangeName,
    }
    [ObjectSystem]
    public class UIConfirmComponentAwake : AwakeSystem<UIConfirmComponent>
    {
        public override void Awake(UIConfirmComponent self)
        {
            self.Awake();
        }
    }
    public class BanType
    {
        // 登录界面提示
        public const string LOGIN = "login";
        // 场景界面提示
        public const string SCENE = "scene";
        // 顶号提示
        public const string OFFLINE = "offline";
        // 退出游戏提示
        public const string QUIT_GAME = "quitgame";
        //登陆限制
        public const string Kicked = "Kicked";
        public const string Normal = "Normal";
        public const string WareHouse = "WareHouse";
        public const string StallUp = "StallUp";
    }
    /// <summary>
    /// 提示/确认组件
    /// </summary>
    public partial class UIConfirmComponent : Component,IUGUIStatus
    {
        private Button yesBtn, noBtn;
        private Text infoTxt, InfoTitleTxt;
        //为了避免写转换器，尽量使用回调Action和Func。
        private Action eventAction, cancelAction;

        public GameObject NormaPanel, WareHousePanel, StallUpPanel,SplitPanel,FuBenRewardPanel, ChangeName;
        public Image Panel;
        public void Awake()
        {
            ReferenceCollector collector = GetParent<UI>().GameObject.GetReferenceCollector();
            collector.gameObject.GetComponent<Canvas>().planeDistance = 8;
            yesBtn = collector.GetButton("YesBtn");
            noBtn = collector.GetButton("NoBtn");
            infoTxt = collector.GetText("InfoTxt");
            InfoTitleTxt = collector.GetText("InfoTitleTxt ");
            NormaPanel = collector.GetImage("Normal").gameObject;//普通确认面板
            WareHousePanel = collector.GetImage("WareHouse").gameObject;//仓库 金币输入提示面板
            StallUpPanel = collector.GetImage("StallUp").gameObject;//摆摊价格设定面板
            SplitPanel = collector.GetImage("Split").gameObject;//物品分堆面板
            FuBenRewardPanel = collector.GetImage("FuBenReward").gameObject;//副本奖励面板
            ChangeName = collector.GetImage("ChangeName").gameObject;//改名面板
            Panel = collector.GetImage("Panel");
            yesBtn.onClick.AddSingleListener(OnYesbtnClick);
            noBtn.onClick.AddSingleListener(OnNobtnClick);
            Init_WareHouse();
            Init_StallUp();
            Init_Split();
            Init_ChangeName();
            InitFuBenReward();
            ChangePanel();
        }
        /// <summary>
        /// 更新 提示面板的类型
        /// </summary>
        /// <param name="panelType"></param>
        public void ChangePanel(E_TipPanelType panelType=E_TipPanelType.NorMal)
        {
            NormaPanel.SetActive(panelType== E_TipPanelType.NorMal);
            WareHousePanel.SetActive(panelType== E_TipPanelType.WareHouse);
            StallUpPanel.SetActive(panelType== E_TipPanelType.StallUp);
            SplitPanel.SetActive(panelType== E_TipPanelType.Split);
            FuBenRewardPanel.SetActive(panelType== E_TipPanelType.FuBenReward);
            ChangeName.SetActive(panelType== E_TipPanelType.ChangeName);
            Panel.raycastTarget = panelType!= E_TipPanelType.Split;
        }


        /// <summary>
        /// 添加确认回调函数
        /// </summary>
        /// <param name="action"></param>
        public void AddActionEvent(Action action)
        {
            if (eventAction != null) eventAction = null;
            eventAction = action;
        }
        /// <summary>
        /// 添加取消回调函数
        /// </summary>
        /// <param name="action"></param>
        public void AddCancelEventAction(Action action)
        {
            if (cancelAction != null) cancelAction = null;
            cancelAction = action;
        }
        private void OnYesbtnClick() 
        {
            eventAction?.Invoke();
            HidePanel();
        }

        private void OnNobtnClick() 
        {
            cancelAction?.Invoke();
            HidePanel();
        }
        /// <summary>
        /// 设置提示文字
        /// </summary>
        /// <param name="info">提示文本信息</param>
        /// <param name="isHidecancleBtn">是否隐藏 取消按钮(默认不隐藏)</param>
        /// <param name="title">标题</param>
        public void SetTipText(string info,bool isHidecancleBtn=false, string title = null)
        {
            InfoTitleTxt.text = title;//设置标题
            infoTxt.text = info;
            HideCancelBtn(!isHidecancleBtn);
        }
        private void HidePanel()
        {
            UIComponent.Instance.InVisibilityUI(UIType.UIConfirm);
        }
        /// <summary>
        /// 登录封禁
        /// </summary>
        /// <param name="ban_conent"></param>
        /// <param name="ban_to_time"></param>
        public void OnLoginIsRestricted(string ban_conent, long ban_to_time)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime targetDt = dtStart.AddSeconds(ban_to_time);
            SetTipText($"在游戏中检测到违规行为\n{ban_conent}\n\n将在 {targetDt.ToString("yyyy年MM月dd日 HH:mm")} 被解封");
            AddActionEvent(QuitGame);

        }
        public void OnSceneIsRestricted(string ban_conent)
        {
            SetTipText($"在游戏中检测到违规行为\n{ban_conent}");
            AddActionEvent(QuitGame);

        }
        // 离线，被踢出游戏
        public void OnOffline()
        {
            SetTipText($"该账号已在其他设备上登陆 如非本人操作，请您及时修改密码",title:"安全提醒");
            AddActionEvent(QuitGame);
            AddCancelEventAction(QuitGame);
        }
        /// <summary>
        /// 退出游戏
        /// </summary>
        public void OnQuitGame()
        {
            SetTipText($"是否退出游戏？");
            AddActionEvent(QuitGame);
        }
        /// <summary>
        /// 隐藏取消 按钮
        /// </summary>
        /// <param name="isshow"></param>
        public void HideCancelBtn(bool isshow = true)
        {
           // noBtn.gameObject.SetActive(isshow);
        }
        public void QuitGame()
        {
            LogCollectionComponent.Instance.Info("退出游戏");
            Application.Quit();

        }
        public void OnVisible(object[] data)
        {
            if (data.Length > 0)
            {
                switch (data[0].ToString())
                {
                    case BanType.LOGIN:
                        // 1.string 封禁原因
                        // 2.long   解封时间
                        HideCancelBtn(false);
                        OnLoginIsRestricted(data[1].ToString(), Convert.ToInt64(data[2]));
                        break;
                    case BanType.SCENE:
                        // 1.string 封禁原因
                        HideCancelBtn(false);
                        OnSceneIsRestricted(data[1].ToString());

                        break;
                    case BanType.OFFLINE:
                        // 被顶号
                        HideCancelBtn(false);
                        OnOffline();
                        break;
                    case BanType.QUIT_GAME:
                        // 退出游戏
                        HideCancelBtn(false);
                        OnQuitGame();
                        break;
                    default:
                        break;
                }
            }
        }

        public void OnVisible()
        {
        }

        public void OnInVisibility()
        {
            eventAction = null;
            cancelAction = null;
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
        }
    }
}