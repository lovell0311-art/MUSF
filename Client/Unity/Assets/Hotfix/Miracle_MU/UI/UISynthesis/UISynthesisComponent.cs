using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using ILRuntime.Runtime;
using System;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// ºÏ³É
    /// </summary>
    public partial class UISynthesisComponent : Component, IUGUIStatus
    {
        public ReferenceCollector collector;

        internal void Init(SynthesisType synthesisType)
        {
            collector = GetParent<UI>().GameObject.GetReferenceCollector();
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UISynthesis);
            });
            GameObject Items = collector.GetGameObject("Items");
            GameObject ItemsZhuoYue = collector.GetGameObject("ItemsZhuoYue");
            for (int i = 0, length = Items.transform.childCount; i < length; i++)
            {
                int index = i + 1;
                Items.transform.GetChild(i).GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge, (SynthesisData)index);
                    UIComponent.Instance.Remove(UIType.UISynthesis);
                });
            }
            for (int i = 0, length = ItemsZhuoYue.transform.childCount; i < length; i++)
            {
                int index = i + 1;
                ItemsZhuoYue.transform.GetChild(i).GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge, (SynthesisData)(index + Items.transform.childCount));
                    UIComponent.Instance.Remove(UIType.UISynthesis);
                });
            }
            ItemsZhuoYue.SetActive(synthesisType == SynthesisType.ZhuoYue);
            Items.SetActive(synthesisType == SynthesisType.General);
        }


        public void OnInVisibility()
        {

        }

        public void OnVisible(object[] data)
        {
            if (data.Length >= 1)
            {
                Init(data[0].ToString().ToEnum<SynthesisType>());
                return;
            }
        }
        public void OnVisible()
        {

        }

    }
}
