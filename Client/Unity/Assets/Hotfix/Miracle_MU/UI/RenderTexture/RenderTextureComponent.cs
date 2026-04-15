using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System;

namespace ETHotfix
{
    [ObjectSystem]
    public class RenderTextureComponentAwake : AwakeSystem<RenderTextureComponent>
    {
        public override void Awake(RenderTextureComponent self)
        {
            self.Awake();
        }
    }
    /// <summary>
    /// RenderTexture管理组件
    /// 一个摄像机渲染多个RenderTexture(一个摄像机渲染多个模型)
    /// </summary>
    public class RenderTextureComponent : Component
    {
        private Camera RenderCamera { get; set; }


        public void Awake()
        {
            RenderCamera = ETModel.Game.Scene.GetComponent<CameraComponent>().RenderCamera;
            RenderCamera.transform.position = new Vector3(0, 0.55f, 3.5f);
            RenderCamera.transform.localRotation = Quaternion.Euler(0, 180, 0);//透视15
            RenderCamera.clearFlags = CameraClearFlags.SolidColor;
            RenderCamera.backgroundColor = Color.clear;
            RenderCamera.orthographic = false;
            //RenderCamera.cullingMask = 1 << 10;
            RenderCamera.farClipPlane = 20;
            RenderCamera.fieldOfView = 25;
            RenderCamera.enabled = false;//手动控制每一次渲染 需要将摄像机禁用掉 然后手动调用摄像机的Render进行渲染

        }
        /// <summary>
        /// 渲染
        /// </summary>
        /// <param name="renderTexture"></param>
        /// <param name="target"></param>
        public void Render(RenderTexture renderTexture, Transform target)
        {
            //if (renderTexture == null || target == null) return;
            RenderCamera.enabled = true;
            //获取所有render记录layer
            Dictionary<Renderer, int> layerDict = new Dictionary<Renderer, int>();
            Renderer[] children = target.GetComponentsInChildren<Renderer>();
            for (int i = children.Length; --i >= 0;)
            {
                Renderer child = children[i];
                layerDict[child] = child.gameObject.layer;  
                child.gameObject.layer = 10;
            }
            //设置renderTexture
            RenderCamera.targetTexture = renderTexture;
            //主动调用摄像机渲染（RenderCamera.enable=false 都可以渲染）
            RenderCamera.Render();
            //还原渲染层
            for (int i = children.Length; --i >= 0;)
            {
                Renderer child = children[i];
                child.gameObject.layer = layerDict[child];
            }
            layerDict.Clear();

        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            RenderCamera = null;
        }
    }
}
