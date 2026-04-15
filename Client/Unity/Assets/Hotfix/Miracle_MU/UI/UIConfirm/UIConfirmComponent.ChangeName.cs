using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.CLR.Utils;
using System;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 改名
    /// </summary>
    public partial class UIConfirmComponent
    {
        public Func<string> ChangeNameFunc;//获取角色名
        public Action ChangeNameEventAction, ChangeNameCancelAction;//角色卡 回调函数
        public InputField ChangeNameinputField;//角色名输入框
        public Text ChangeNameTitleTxt;//名字
        public void Init_ChangeName()
        {
            ReferenceCollector collector = ChangeName.GetReferenceCollector();
            ChangeNameinputField = collector.GetInputField("GlodInputField");
            ChangeNameTitleTxt = collector.GetText("InfoTitleTxt ");
            ChangeNameinputField.onValueChanged.AddSingleListener(value =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    ChangeNameFunc = null;
                    return;
                };

              
                ChangeNameFunc = GetName;
               

                string GetName() 
                {
                    return value;
                }
            });
            collector.GetButton("YesBtn").onClick.AddSingleListener(() =>
            {
                ChangeNameEventAction?.Invoke();
                ChangeNameinputField.text = String.Empty;
            });
            collector.GetButton("NoBtn").onClick.AddSingleListener(() =>
            {
                ChangeNameCancelAction?.Invoke();
                ChangeNameinputField.text = String.Empty;
                ChangeNameFunc = null;
                HidePanel();
            });
        }
    }
}
