using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [ObjectSystem]
    public class UISetCameraAtrComponentAwake : AwakeSystem<UISetCameraAtrComponent>
    {
        public override void Awake(UISetCameraAtrComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class UISetCameraAtrComponentUpdate : UpdateSystem<UISetCameraAtrComponent>
    {
        public override void Update(UISetCameraAtrComponent self)
        {
            self.Update();
        }
    }
    /// <summary>
    /// 自定义相机视角组件
    /// </summary>
    public class UISetCameraAtrComponent : Component
    {
        CameraFollowComponent followComponent;
        public float MinimumX = 0;
        public float MaximumX = 90;


        public float XSensitivity = 2f;
        public float YSensitivity = 1f;

        private float m_XRotateMax = 70f; //沿X轴最大角度
        private float m_XRotateMin = 5;   //沿X周最小角度


        public float zoomSpeedMouse = 1;
        public float zoomSpeedTouch = 0.01f;
        public float minDistance = 20;
        public float maxDistance = 45;//15

      
        float xExtra;
        float yExtra;

        public LayerMask viewBlockingLayers;

        /// <summary>
        /// 角色的高度
        /// </summary>
        public float roleHeight;
        public void Awake() 
        {
            //followComponent = UnitEntityComponent.Instance.LocalRole.GetComponent<CameraFollowComponent>();
            followComponent = ETModel.Game.Scene.GetComponent<CameraFollowComponent>();
            GetParent<UI>().GameObject.GetReferenceCollector().GetButton("saveBtn").onClick.AddSingleListener(Save);
            GetParent<UI>().GameObject.GetReferenceCollector().GetButton("resetBtn").onClick.AddSingleListener(Reset);
        }
        public void Update()
        {

#if UNITY_EDITOR_WIN
            if (Input.GetKeyDown(KeyCode.Z))
            {
                followComponent.h += .5f;
                Log.DebugBrown($"+h:{followComponent.h}  distance🍛😁:{followComponent.distance}");
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                followComponent.h -= .5f;
                Log.DebugBrown($"+h:{followComponent.h}  distance🍛😁:{followComponent.distance}");
            } 
#endif


            //if (Input.touchCount == 1)
            //{

            //    // 计算水平和垂直旋转 steps
            //    xExtra = Input.GetAxis("Mouse X") * XSensitivity;
            //    yExtra = Input.GetAxis("Mouse Y") * YSensitivity;
            //    followComponent.curAngleV -= yExtra;
            //    followComponent.curAngleH += xExtra;
            //    //限制x旋转 角度 进行Clamp限制
            //    followComponent.curAngleV = Mathf.Clamp(followComponent.curAngleV, m_XRotateMin, m_XRotateMax);

            //}



            //if (Input.GetMouseButton(1))//&& !GameUtility.IsPointerOverGameObject()
            //{

            //    // 计算水平和垂直旋转 steps
            //    xExtra = Input.GetAxis("Mouse X") * XSensitivity;
            //    yExtra = Input.GetAxis("Mouse Y") * YSensitivity;

            //    followComponent.curAngleV -= yExtra;
            //    followComponent.curAngleH += xExtra;
            //    //限制x旋转 角度 进行Clamp限制
            //    followComponent.curAngleV = Mathf.Clamp(followComponent.curAngleV, m_XRotateMin, m_XRotateMax);
            //}

            // zoom
            float speed = Input.mousePresent ? zoomSpeedMouse : zoomSpeedTouch;
            float step = GameUtility.GetZoomUniversal() * speed;
            followComponent.distance = Mathf.Clamp(followComponent.distance - step, minDistance, maxDistance);
         //   Log.DebugGreen($"followComponent.distance:{followComponent.distance}");
        }

        public void Save() 
        {
            CameraInfo cameraInfo = new CameraInfo
            {
                curAngleH = followComponent.curAngleH,
                curAngleV = followComponent.curAngleV,
                 distance = followComponent.distance
            };
            Log.DebugBrown("回复设置" + cameraInfo.curAngleH + ":v" + cameraInfo.curAngleV + ":d" + cameraInfo.distance);
            LocalDataJsonComponent.Instance.SavaData(cameraInfo,LocalJsonDataKeys.CameraInfo);
            UIComponent.Instance.Remove(UIType.UISetCameraAtr);
        }
        public  void Reset()
        {
            CameraInfo cameraInfo = new CameraInfo();
            followComponent.curAngleH = cameraInfo.curAngleH;
            followComponent.curAngleV = cameraInfo.curAngleV;
            followComponent.distance = cameraInfo.distance;
            Log.DebugBrown("回复默认"+cameraInfo.curAngleH+":v"+cameraInfo.curAngleV+":d"+cameraInfo.distance);
            LocalDataJsonComponent.Instance.SavaData(cameraInfo, LocalJsonDataKeys.CameraInfo);
        }
    }
}
