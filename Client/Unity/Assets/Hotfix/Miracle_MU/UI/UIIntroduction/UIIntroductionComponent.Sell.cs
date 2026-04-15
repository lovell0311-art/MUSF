using ETModel;
using ILRuntime.Runtime;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIIntroductionComponent
    {
        public ReferenceCollector SellCollector;
        public int sellgold = 0;
        public InputField sellInputField;
        public void Sell()
        {
            SellCollector = collector.GetImage("SellPanel").gameObject.GetReferenceCollector();
            sellInputField = SellCollector.GetInputField("SellInputField");
            SellCollector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                ShowSellPanel(false);
            });
            sellInputField.onValueChanged.AddSingleListener((Value) =>
            {
                sellgold = Value.ToInt32();
            });
            SellCollector.GetButton("SellBtn").onClick.AddSingleListener(() =>
            {
                if (sellgold == 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入金额！");
                    return;
                }
                else if (sellgold < 2)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "最低上架2魔晶！");
                    return;
                }
                //if (UnitEntityComponent.Instance.LocalRole.MaxMonthluCardTimeSpan.TotalSeconds <= 0)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "没有赞助卡禁止上架藏宝阁");
                //    return;
                //}
                //}else if (sellgold < 34)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "手续费不足1魔晶按1魔晶收！");
                //}
                SellAction?.Invoke(sellgold);
                ShowSellPanel(false);
            });
            ShowSellPanel(false);
        }
        public void ShowSellPanel(bool show)
        {
            SellCollector.gameObject.SetActive(show);
        }
    }

}
