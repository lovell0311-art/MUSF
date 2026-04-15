using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// UGUI 组件扩展类
    /// </summary>
    public static class UGUIExtend 
    {
        public static void AddSingleListener(this Button.ButtonClickedEvent self, UnityAction action)
        {
            self.RemoveAllListeners();
            self.AddListener(action);
        }
        public static void AddSingleListener(this Toggle.ToggleEvent self, UnityAction<bool> action)
        {
            self.RemoveAllListeners();
            self.AddListener(action);
        }
        public static void AddSingleListener(this Slider.SliderEvent self, UnityAction<float> action)
        {
            self.RemoveAllListeners();
            self.AddListener(action);
        }
        public static void AddSingleListener(this InputField.SubmitEvent self, UnityAction<string> action)
        {
            self.RemoveAllListeners();
            self.AddListener(action);
        }
        public static void AddSingleListener(this InputField.OnChangeEvent self, UnityAction<string> action)
        {
            self.RemoveAllListeners();
            self.AddListener(action);
        }
    }
}
