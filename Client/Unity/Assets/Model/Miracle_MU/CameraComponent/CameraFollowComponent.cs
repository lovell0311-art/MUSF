using UnityEngine;
using DG.Tweening;
using ETModel;
using System;

namespace ETModel
{

    /// <summary>
    /// 摄像机的的信息
    /// </summary>
    [Serializable]
    public class CameraInfo
    {
        public float curAngleH;
        public float curAngleV;
        public float distance;

        public CameraInfo()
        {
            curAngleH = -135f;//Y
            curAngleV = 45;//X
            distance = 35;
        }
    }

    [ObjectSystem]
    public class CameraFollowComponentAwake : AwakeSystem<CameraFollowComponent>
    {
        public override void Awake(CameraFollowComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class CameraFollowComponentStart : StartSystem<CameraFollowComponent>
    {
        public override void Start(CameraFollowComponent self)
        {
          // self.followTarget =self.GetParent<RoleEntity>().Game_Object.transform;
           self.cameraTransform = CameraComponent.Instance.MainCamera.transform;
        }
    }
    //[ObjectSystem]
    public class CameraFollowComponentUpdate : UpdateSystem<CameraFollowComponent>
    {
        public override void Update(CameraFollowComponent self)
        {
            self.LateUpdate();
        }
    }


   [ObjectSystem]
    public class CameraFollowComponentLaterUpdate : LateUpdateSystem<CameraFollowComponent>
    {
        public override void LateUpdate(CameraFollowComponent self)
        {
            self.LateUpdate();
        }
    }
    /// <summary>
    /// 相机跟随组件
    /// </summary>
    public class CameraFollowComponent : Component
    {
        private const float CameraCollisionRadius = 0.35f;
        private const float CameraCollisionPadding = 0.4f;
        private const float CameraMinDistance = 6f;
        private const float CameraFollowSmoothTime = 0.08f;
        private const float CameraSnapDistance = 8f;

        public static CameraFollowComponent Instance;
        /// <summary>
        /// 相机跟随的目标对象
        /// </summary>
        public Transform followTarget;

        /// <summary>
        /// 相机的旋转角度
        /// </summary>
        public  float curAngleH = -135f;//Y
        public  float curAngleV = 45;//X

        /// <summary>
        /// 相机相对玩家的距离
        /// </summary>
        public  float distance = 35;
        /// <summary>
        /// 当前的相机
        /// </summary>
        public Transform cameraTransform;

        

        public LayerMask viewBlockingLayers;

        /// <summary>
        /// 角色的高度
        /// </summary>
        public float roleHeight;

        public float h = 1f;
        private int cachedViewBlockingMask;
        private bool cachedViewBlockingMaskReady;
        private Vector3 followVelocity;


        public void Awake()
        {
            Instance = this;
            CameraInfo cameraInfo = LocalDataJsonComponent.Instance.LoadData<CameraInfo>(LocalJsonDataKeys.CameraInfo) ?? new CameraInfo();
            curAngleH = cameraInfo.curAngleH;
            curAngleV = cameraInfo.curAngleV;
            distance = cameraInfo.distance;
        }
        Vector3 pos=Vector3.zero;
        public bool ChangeScene = false;
        public void LateUpdate()
        {
            if (followTarget == null)
            {
               
                return;
            }
         
            Quaternion animRotation = Quaternion.Euler(curAngleV, curAngleH, 0.0f);
            Quaternion camYRotation = Quaternion.Euler(0.0f, curAngleH, 0.0f);
            //设置相机的旋转角度
            cameraTransform.rotation = animRotation;
            float followHeight = Mathf.Max(1.6f, h);
            Vector3 lookatpos = followTarget.position + camYRotation * Vector3.up * followHeight;
            Vector3 camdir = animRotation * Vector3.back;
            camdir.Normalize();
            Vector3 desiredCameraPosition = ResolveCameraPosition(lookatpos, lookatpos + camdir * distance);
            if (ChangeScene)
            {
                cameraTransform.position = desiredCameraPosition;
                followVelocity = Vector3.zero;
                pos = followTarget.position;
                ChangeScene = false;
              
            }
            else
            {
                float snapDistanceSqr = CameraSnapDistance * CameraSnapDistance;
                if ((cameraTransform.position - desiredCameraPosition).sqrMagnitude >= snapDistanceSqr)
                {
                    cameraTransform.position = desiredCameraPosition;
                    followVelocity = Vector3.zero;
                }
                else
                {
                    cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position,
                        desiredCameraPosition,
                        ref followVelocity,
                        CameraFollowSmoothTime);
                }
            }
            pos = followTarget.position;

        }

        private Vector3 ResolveCameraPosition(Vector3 lookatpos, Vector3 desiredCameraPosition)
        {
            Vector3 offset = desiredCameraPosition - lookatpos;
            float desiredDistance = offset.magnitude;
            if (desiredDistance <= 0.01f)
            {
                return desiredCameraPosition;
            }

            int viewMask = GetViewBlockingMask();
            Vector3 direction = offset / desiredDistance;
            if (Physics.SphereCast(lookatpos, CameraCollisionRadius, direction, out RaycastHit hit, desiredDistance, viewMask, QueryTriggerInteraction.Ignore))
            {
                float resolvedDistance = Mathf.Max(CameraMinDistance, hit.distance - CameraCollisionPadding);
                return lookatpos + direction * resolvedDistance;
            }

            return desiredCameraPosition;
        }

        private int GetViewBlockingMask()
        {
            if (viewBlockingLayers.value != 0)
            {
                return viewBlockingLayers.value;
            }

            if (cachedViewBlockingMaskReady)
            {
                return cachedViewBlockingMask;
            }

            int mask = ~0;
            ExcludeLayer(ref mask, "UI");
            ExcludeLayer(ref mask, "Role");
            ExcludeLayer(ref mask, "LocalRole");
            ExcludeLayer(ref mask, "Monster");
            ExcludeLayer(ref mask, "NPC");
            ExcludeLayer(ref mask, "fx");
            ExcludeLayer(ref mask, "RenderRole");
            cachedViewBlockingMask = mask;
            cachedViewBlockingMaskReady = true;
            return cachedViewBlockingMask;
        }

        private static void ExcludeLayer(ref int mask, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer >= 0)
            {
                mask &= ~(1 << layer);
            }
        }



        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();

        }

        public void ResetChooseRolePresentation()
        {
            followTarget = null;
            ChangeScene = false;
            roleHeight = 0f;
            h = 1f;
            curAngleH = -135f;
            curAngleV = 45f;
            distance = 35f;
            followVelocity = Vector3.zero;
            pos = Vector3.zero;

            if (CameraComponent.Instance?.MainCamera == null)
            {
                return;
            }

            if (cameraTransform == null)
            {
                cameraTransform = CameraComponent.Instance.MainCamera.transform;
            }

            CameraComponent.Instance.MainCamera.targetTexture = null;
            CameraComponent.Instance.MainCamera.farClipPlane = 1000f;
            CameraComponent.Instance.MainCamera.fieldOfView = 45f;
            cameraTransform.SetPositionAndRotation(new Vector3(-5f, 7f, 25f), Quaternion.Euler(0f, 180f, 0f));
        }

    }
}
