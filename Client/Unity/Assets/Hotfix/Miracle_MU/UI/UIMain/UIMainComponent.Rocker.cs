using UnityEngine;
using UnityEngine.UI;
using ETModel;
using UnityEngine.EventSystems;
using System;
namespace ETHotfix
{
    /// <summary>
    /// 摇杆控制模块
    /// </summary>
    public partial class UIMainComponent 
    {

        Image TouchRect;//摇杆触发范围
        Image imageControl;//摇杆圆圈
        Image imageBg;//摇杆背景
        public float maxL = 140;//摇杆圆圈可移动的最大范围

        /// <summary>
        /// 初始化摇杆
        /// </summary>

        public void InitRocker()
        {
            ReferenceCollector ReferenceCollector_Rocker = ReferenceCollector_Main.GetGameObject("Rocker").GetReferenceCollector();
            TouchRect = ReferenceCollector_Rocker.GetImage("touchiarea");
            imageControl = ReferenceCollector_Rocker.GetImage("control");
            imageBg = ReferenceCollector_Rocker.GetImage("bg");
            RegisterEvent();
        }
        public void RegisterEvent() 
        {
            UIUtils.AddCustomEventListener(TouchRect,EventTriggerType.PointerUp,PointerUpEvent);
            UIUtils.AddCustomEventListener(TouchRect,EventTriggerType.Drag,DragEvent);
         //   UIUtils.AddCustomEventListener(TouchRect,EventTriggerType.EndDrag, EndDragEvent);
            UIUtils.AddCustomEventListener(TouchRect,EventTriggerType.PointerDown, PointerDownEvent);
        }

        private void EndDragEvent(BaseEventData arg0)
        {
            imageControl.transform.localPosition = Vector3.zero;//复位
                                                                //分发摇杆复位信息
           // Game.EventManager.BroadcastEvent(EventTypeId.UI_JOYSTICK_UP);
            Game.EventCenter.EventTrigger(EventTypeId.UI_JOYSTICK_UP);
        }

        /// <summary>
        /// 按下事件
        /// </summary>
        /// <param name="data"></param>
        private void PointerDownEvent(BaseEventData data)
        {
            
            //分发 摇杆按下事件
          //  Game.EventManager.BroadcastEvent(EventTypeId.UI_JOYSTICK_DOWN);
            Game.EventCenter.EventTrigger(EventTypeId.UI_JOYSTICK_DOWN);
        }
        /// <summary>
        /// 移动事件
        /// </summary>
        /// <param name="data"></param>
        private void DragEvent(BaseEventData data)
        {
         
            if (roleEntity.IsDead)
            {
                imageControl.transform.localPosition = Vector3.zero;//复位
              //  Game.EventManager.BroadCastEvent<Vector2>(EventTypeId.UI_JOYSTICK_VALUE, Vector2.zero);
                Game.EventCenter.EventTrigger<Vector2>(EventTypeId.UI_JOYSTICK_VALUE, Vector2.zero);
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                imageBg.rectTransform,//想要改变位置对象 的父对象
                (data as PointerEventData).position,//得到屏幕的鼠标位置
                (data as PointerEventData).pressEventCamera,//Ui用的相机
                out Vector2 localPos);//得到一个转换完成的相对坐标

            //Vector2 vector2 =  GetJoystickDirection(localPos);
            //更新中心圆圈的位置
            imageControl.transform.localPosition = localPos.magnitude > maxL ? localPos.normalized * maxL : localPos;
            // imageControl.transform.DOLocalMove(vector2.normalized * maxL,.1f);
            //分发 摇杆当前的方向信息
            // Log.DebugWhtie($"vector2.normalized:{vector2}");
            Vector2 vector2 = GetJoystickDirection(localPos);

            Game.EventCenter.EventTrigger<Vector2>(EventTypeId.UI_JOYSTICK_VALUE, vector2);
        }
        /// <summary>
        /// 抬起事件
        /// </summary>
        /// <param name="data"></param>
        private void PointerUpEvent(BaseEventData data)
        {
           
            imageControl.transform.localPosition = Vector3.zero;//复位
             //分发摇杆复位信息
           // Game.EventManager.BroadcastEvent(EventTypeId.UI_JOYSTICK_UP);
            Game.EventCenter.EventTrigger(EventTypeId.UI_JOYSTICK_UP);
        }
        /// <summary>
        /// 摇杆复位
        /// </summary>
        public void ResetRocker() 
        {
            imageControl.transform.localPosition = Vector3.zero;//复位
        }
       
        public Vector2 GetJoystickDirection(Vector2 pos)
        {
            var rad = Mathf.Atan2(pos.y,pos.x);
            if ((rad >= -Mathf.PI / 8 && rad < 0) || (rad >= 0 && rad < Mathf.PI / 8))
            {
                //Log.DebugBrown($"右");
                return new Vector2Int(1, 0);
                
            }
            else if (rad >= Math.PI / 8 && rad < 3 * Math.PI / 8)
            {
               // Log.DebugBrown($"右上");
                return new Vector2Int(1, 1);
            }
            else if (rad >= 3 * Math.PI / 8 && rad < 5 * Math.PI / 8)
            {
               
               // Log.DebugBrown($"上");
                return new Vector2Int(0, 1);
            }
            else if (rad >= 5 * Math.PI / 8 && rad < 7 * Math.PI / 8)
            {
               // Log.DebugBrown($"左上");
                return new Vector2Int(-1, 1);
            }
            else if ((rad >= 7 * Math.PI / 8 && rad < Math.PI) || (rad >= -Math.PI && rad < -7 * Math.PI / 8))
            {
               // Log.DebugBrown($"左");
                return new Vector2Int(-1, 0);
            }
            else if (rad >= -7 * Math.PI / 8 && rad < -5 * Math.PI / 8)
            {
               // Log.DebugBrown($"左下");
                return new Vector2Int(-1,-1);
            }
            else if (rad >= -5 * Math.PI / 8 && rad < -3 * Math.PI / 8)
            {
               // Log.DebugBrown($"下");
                return new Vector2Int(0, -1);
            }
            else
            {
               // Log.DebugBrown($"右下");
                return new Vector2Int(1, -1);
            }

        }
    }
}
