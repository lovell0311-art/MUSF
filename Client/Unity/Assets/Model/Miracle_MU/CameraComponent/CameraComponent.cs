using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;


namespace ETModel
{
    [ObjectSystem]
    public class CameraComponentAwake : AwakeSystem<CameraComponent>
    {
        public override void Awake(CameraComponent self)
        {
            self.Awake();
        }
    }
    public class CameraComponent : Component
    {
        public static CameraComponent Instance { get; private set; }

        public Camera MainCamera { get; private set; }
        public Camera UICamera { get; private set; }
        public Camera RenderCamera { get; private set; }

        public float dis = 50;

        public void Awake()
        {
            Instance = this;

            MainCamera = Component.Global.transform.Find("BackgroudCamera").gameObject.GetComponent<Camera>();
            UICamera = Component.Global.transform.Find("UICamera").gameObject.GetComponent<Camera>();
            RenderCamera = Component.Global.transform.Find("RenderCamera").gameObject.GetComponent<Camera>();
           
        }

    }
}