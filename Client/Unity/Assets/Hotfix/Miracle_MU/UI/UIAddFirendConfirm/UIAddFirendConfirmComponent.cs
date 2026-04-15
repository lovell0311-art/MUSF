using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIAddFirendConfirmComponentAwake : AwakeSystem<UIAddFirendConfirmComponent>
    {
        public override void Awake(UIAddFirendConfirmComponent self)
        {
            self.Awake();
        }
    }
    public class UIAddFirendConfirmComponent : Component, IUGUIStatus
    {
        public InputField searchInput;
        public GameObject TipWindow;
        public Text State;
        public Text Nickname;
        private Action addBtnAction, rejectBtnAction;
        public void Awake()
        {
            ReferenceCollector collector = GetParent<UI>().GameObject.GetReferenceCollector();
            State = collector.GetText("State");
            Nickname = collector.GetText("NickName");
            collector.GetButton("AddFirendBtn").onClick.AddSingleListener(OnAddBtnClick);
            collector.GetButton("RejectBtn").onClick.AddSingleListener(OnRejectBtnClick);
        }
        private void OnAddBtnClick()
        {
            addBtnAction?.Invoke();
            HidePanel();
        }
        /// <summary>
        /// 添加确认添加好友回调函数
        /// </summary>
        /// <param name="action"></param>
        public void AddFirendActionEvent(Action action)
        {
            if (addBtnAction != null) addBtnAction = null;
            addBtnAction = action;
        }

        private void OnRejectBtnClick()
        {
            rejectBtnAction?.Invoke();
            HidePanel();
        }
        /// <summary>
        /// 添加拒绝添加好友回调函数
        /// </summary>
        /// <param name="action"></param>
        public void AddRejectActionEvent(Action action)
        {
            if (rejectBtnAction != null) rejectBtnAction = null;
            rejectBtnAction = action;
        }

        public void SetTipText(string state,string name)
        {
            State.text = state;
            Nickname.text = name;
        }
        private void HidePanel()
        {
            UIComponent.Instance.InVisibilityUI(UIType.UIAddFirendConfirm);
        }

        public void OnVisible(object[] data)
        {
            
        }

        public void OnVisible()
        {
            
        }

        public void OnInVisibility()
        {
            addBtnAction = null;
            rejectBtnAction = null;
        }
    }

}
