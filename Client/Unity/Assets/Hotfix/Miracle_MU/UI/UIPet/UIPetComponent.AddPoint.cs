using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public Text Title;
        public InputField InputField;
        int addPointValue = 0;
        public void InitPoint()
        {
            InputField = addPointCollector.GetInputField("InputField");
            Title = addPointCollector.GetText("title");

            addPointCollector.GetButton("SureBtn").onClick.AddSingleListener(() =>
            {
                if(InputField.text == string.Empty)
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"请输入增加点数");
                    return;
                }
                if (lastClickItem.uIPetInfo.petsLVpoint > 0)
                {
                    AddAttributePointRequest(lastClickItem.uIPetInfo.petId, addPointValue, int.Parse(InputField.text)).Coroutine();
                }
                else
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"剩余点数不足");
                }
            });
            addPointCollector.GetButton("CancelBtn").onClick.AddSingleListener(() =>
            {
                InputField.text = string.Empty;
                addPointCollector.gameObject.SetActive(false);
            });
        }

        public void AddPointValue(int title)
        {
            addPointCollector.gameObject.SetActive(true);
            Title.text = $"请输入点数——{GetAttrbuteValue()}";
            addPointValue = title;
            string GetAttrbuteValue() => title switch
            {
                1 => "力量",
                2 => "智力",
                3 => "敏捷",
                4 => "体力",
                _=>string.Empty
            };
        }
        
    }

}
