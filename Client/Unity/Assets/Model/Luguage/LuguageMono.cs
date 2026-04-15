using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETHotfix;
using UnityEngine.UI;

namespace ETModel
{
    [Event(EventIdType.LuguageChange)]
    public class LuguageMessage: AEvent<LuguageMessage>
    {
        public LuguageMono luguageMono;
        public override void Run(LuguageMessage a)
        {
        }
    }
    public class LuguageMono : MonoBehaviour
    {
        public string key;

        private void OnEnable()
        {
            if (this.key == null && this.key == "")
            {
                return;
            }
            RefreshLuguageUIEvent();
        }
        public void SetKey(string key)
        {
            this.key = key;
            RefreshLuguageUIEvent();
        }
        public void SetLuguageString(string str)
        {
            try
            {
                this.GetComponent<Text>().text = str;
            }
            catch
            {
                Log.Error("SetLuguageString error");
            }
        }
        public void SetLuguageSprite(Sprite spr)
        {
            try
            {
                this.GetComponent<Image>().sprite = spr;
            }
            catch
            {
                Log.Error("SetLuguageSprit error");
            }
        }

        public void RefreshLuguageUIEvent()
        {
            Debug.Log("脚本执行多语言刷新事件");
            LuguageMessage message = new LuguageMessage { luguageMono = this };
            Game.EventSystem.Run(EventIdType.LuguageChange, message);
        }
    }
}
