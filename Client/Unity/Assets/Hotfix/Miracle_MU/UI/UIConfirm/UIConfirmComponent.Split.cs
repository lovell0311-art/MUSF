using UnityEngine;
using ETModel;
using System;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 道具 分堆
    /// </summary>
    public partial class UIConfirmComponent
    {
        public Func<int> GetSplitFunc;//分堆的数量
        public Action SplitEventAction, SplitCancelAction;//分堆数量 回调函数
        public InputField SplitinputField;//分堆数量输入框
        public int splitCount=0;
        public KnapsackGridData splitItem;//要分堆的物品
        public Vector3 objPos;
        public Image objIcon;
        public GameObject SplitObj=null;

        public void Init_Split()
        {
            ReferenceCollector collector = SplitPanel.GetReferenceCollector();
            SplitinputField = collector.GetInputField("GlodInputField");
            objIcon = collector.GetImage("Icon");
            objPos = new Vector3(objIcon.transform.localPosition.x, objIcon.transform.localPosition.y,10);
            SplitinputField.onValueChanged.AddSingleListener(value =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    GetSplitFunc = null;
                    return;
                };

                if (int.TryParse(value, out splitCount))
                {
                    GetSplitFunc = GetPrice;
                }

                int GetPrice()
                {
                    return splitCount;
                }
            });
            collector.GetButton("SubBtn").onClick.AddSingleListener(() => 
            {
                if (splitItem == null) return;
                splitCount = Mathf.Clamp(--splitCount,1, splitItem.ItemData.GetProperValue(E_ItemValue.Quantity));
                SplitinputField.text = splitCount.ToString();
            });
            collector.GetButton("AddBtn").onClick.AddSingleListener(() => 
            {
                if (splitItem == null) return;
                splitCount = Mathf.Clamp(++splitCount,1, splitItem.ItemData.GetProperValue(E_ItemValue.Quantity));
                SplitinputField.text = splitCount.ToString();
            });
            collector.GetButton("YesBtn").onClick.AddSingleListener(() =>
            {
                SplitEventAction?.Invoke();
                SplitinputField.text = String.Empty;
            });
            collector.GetButton("NoBtn").onClick.AddSingleListener(() =>
            {
                SplitCancelAction?.Invoke();
                SplitinputField.text = String.Empty;
                HidePanel();
                if (SplitObj != null)
                {
                   // ResourcesComponent.Instance.RecycleGameObject(SplitObj);
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(SplitObj, SplitObj.name.StringToAB());
                }
            });
        }
    }
}