using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ETModel 
{
public class UIUtils 
{
        /// <summary>
        /// 给控件添加自定义事件监听
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <param name="type">事件类型</param>
        /// <param name="callBack">事件的响应函数</param>
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
        {
            EventTrigger trigger = control.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = control.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(callBack);

            trigger.triggers.Add(entry);
        }
        /// <summary>
        /// 是否点击到UI上
        /// </summary>
        /// <returns></returns>
        public static bool IsCursorOverUserInterface()
        {
          
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return true;

          
            for (int i = 0; i < Input.touchCount; ++i)
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                    return true;

         
            return GUIUtility.hotControl != 0;
        }

        public static void AddCustomEventListener(Image rotacameraArea, EventTriggerType drag, object dragEvent)
        {
            throw new NotImplementedException();
        }
    }
}