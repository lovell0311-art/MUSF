using ETModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace ETHotfix
{
    [ObjectSystem]
    public class LuguageComponentAwake : AwakeSystem<LuguageComponent>
    {
        public override void Awake(LuguageComponent self)
        {
            LuguageComponent.Instance = self;
            Debug.Log("多语言组件初始化");
           // LuguageComponent.luguageMessageHandler = new LuguageMessageHandler();
           // ETModel.Game.EventSystem.RegisterEvent(ETModel.EventIdType.LuguageChange, LuguageComponent.luguageMessageHandler);
           // LuguageInfo luguageInfo =  LocalDataJsonComponent.Instance.LoadData<LuguageInfo>(LocalJsonDataKeys.LuguageInfo);
          //  LuguageComponent.Instance.CurrentLanguageIndex  = luguageInfo.index;
        }
    }

    public class LuguageComponent : Component
    {
        public static LuguageComponent Instance;

        public static LuguageMessageHandler luguageMessageHandler;
        /// <summary>
        /// 当前语言下标 0 中文 1 英文
        /// </summary>
        public int CurrentLanguageIndex
        {
            get;
            set;
        }


        public void SetKeyToLuguage(GameObject obj, string key)
        {
            if (!obj.GetComponent<LuguageMono>())
            {
                //没有多语言组件，添加多语言组件
                obj.AddComponent<LuguageMono>();
                obj.GetComponent<LuguageMono>().key = key;
            }
            //读取key的类型，是string还是sprite
            int type = 0;
            switch (type)
            {
                case 0:
                    string str = "";
                    obj.GetComponent<LuguageMono>().SetLuguageString(str);
                    break;
                case 1:
                    Sprite spr = null;
                    obj.GetComponent<LuguageMono>().SetLuguageSprite(spr);
                    break;
                default:
                    break;
            }



        }
    }

}
