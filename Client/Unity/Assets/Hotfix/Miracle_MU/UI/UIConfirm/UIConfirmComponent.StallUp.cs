using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System;
using UnityEngine.UI;
using System.Runtime.Remoting.Messaging;

namespace ETHotfix
{   
    /// <summary>
    /// 摆摊金币设置
    /// </summary>
    public partial class UIConfirmComponent
    {
        public Func<int> GetStallUpGoldFunc;//获取摆摊输入的金币
        public Func<int> GetStallUpYuanBaoFunc;//获取摆摊输入的元宝
        public Action StallUpEventAction, StallUpCancelAction;//摆摊 回调函数
        public InputField StallUpinputField;//摆摊金币输入框
        public InputField StallUpYuanBaoinputField;//摆摊元宝输入框
        public int coinprice, yuanbaoprice;
        public void Init_StallUp()
        {
            coinprice = 0;
            yuanbaoprice = 0;
            GetStallUpGoldFunc = () => coinprice;
            GetStallUpYuanBaoFunc = () => yuanbaoprice;
            ReferenceCollector collector = StallUpPanel.GetReferenceCollector();
            //金币价格
           StallUpinputField = collector.GetInputField("GlodInputField");
           StallUpinputField.onEndEdit.AddSingleListener(value =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    coinprice = 0;
                    return;
                };
                coinprice=int.Parse(value);
                if (coinprice < 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"价格不能为负数，请您设置价格");
                    StallUpinputField.text = string.Empty;
                    return;
                }
            });
            //元宝价格
            StallUpYuanBaoinputField = collector.GetInputField("YuanInputField");
            StallUpYuanBaoinputField.onEndEdit.AddSingleListener(value =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    yuanbaoprice = 0;
                    return;
                };
                yuanbaoprice = int.Parse(value);
                if (yuanbaoprice < 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "价格不能为负数，请您设置价格");
                    StallUpYuanBaoinputField.text = string.Empty;
                    return;
                }
            });
            collector.GetButton("YesBtn").onClick.AddSingleListener(() =>
            {
                StallUpEventAction?.Invoke();
                StallUpinputField.text = String.Empty;
                StallUpYuanBaoinputField.text = String.Empty;
            });
            collector.GetButton("NoBtn").onClick.AddSingleListener(() =>
            {
                StallUpCancelAction?.Invoke();
                StallUpinputField.text = String.Empty;
                StallUpYuanBaoinputField.text = String.Empty;
                HidePanel();
            });
        }
    }
}
