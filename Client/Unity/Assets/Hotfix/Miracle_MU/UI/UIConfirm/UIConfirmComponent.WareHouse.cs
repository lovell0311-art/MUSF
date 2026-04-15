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
    /// 仓库金币 输入提示窗口
    /// </summary>
    public partial class UIConfirmComponent
    {
        public Func<int> GetGoldFunc;//获取输入的金币
        public Action WareHouseEventAction, WareHouseCancelAction;//仓库 回调函数
        public InputField WareHouseinputField;//仓库金币输入框
        public Text WareInfoTitleTxt;//取出、存取 提示
        public void Init_WareHouse()
        {
            ReferenceCollector collector = WareHousePanel.GetReferenceCollector();
            WareHouseinputField = collector.GetInputField("GlodInputField");
            WareInfoTitleTxt = collector.GetText("InfoTitleTxt ");
            WareHouseinputField.onValueChanged.AddSingleListener(value =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    GetGoldFunc = null;
                    return;
                };

                if (int.TryParse(value, out int price))
                {
                    GetGoldFunc = GetPrice;
                }

                int GetPrice() 
                {
                    return price;
                }
            });
            collector.GetButton("YesBtn").onClick.AddSingleListener(() =>
            {
                WareHouseEventAction?.Invoke();
                WareHouseinputField.text = String.Empty;
            });
            collector.GetButton("NoBtn").onClick.AddSingleListener(() =>
            {
                WareHouseCancelAction?.Invoke();
                WareHouseinputField.text = String.Empty;
                HidePanel();
            });
        }
    }
}
