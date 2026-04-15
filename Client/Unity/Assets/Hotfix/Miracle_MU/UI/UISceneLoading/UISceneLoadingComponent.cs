using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETModel;
using DG.Tweening;


namespace ETHotfix
{
    [ObjectSystem]
    public class UISceneLoadingComponentAwake : AwakeSystem<UISceneLoadingComponent>
    {
        public override void Awake(UISceneLoadingComponent self)
        {
           // self.text = self.GetParent<UI>().GameObject.GetReferenceCollector().GetText("Handle");
            self.progre = self.GetParent<UI>().GameObject.GetReferenceCollector().GetText("progre");
           // self.progre.text = $"10%";
            self.slider = self.GetParent<UI>().GameObject.GetReferenceCollector().GetGameObject("Slider").GetComponent<Slider>();
            self.slider.value = 50;
            self.progre.text = $"{(int)(self.slider.value)}%";

          

            self.GetParent<UI>().GameObject.SetLayer(LayerNames.RENDER);
            self.Icon = self.GetParent<UI>().GameObject.GetReferenceCollector().GetImage("IconProgre");
            self.Icon.fillAmount = 1;
            self.renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            CameraComponent.Instance.MainCamera.targetTexture = self.renderTexture;
            CameraComponent.Instance.UICamera.cullingMask &= ~(1 << 5);//在原基础上 关闭 5 层
            CameraComponent.Instance.UICamera.cullingMask |= (1 << 10);//在原基础上 打开 10层
           
        }
    }
    [ObjectSystem]
    public class UISceneLoadingComponentStart : StartSystem<UISceneLoadingComponent>
    {

        SceneChangeComponent sceneChangeComponent;
        TimerComponent timerComponent;

         
        public override void Start(UISceneLoadingComponent self)
        {
            sceneChangeComponent = ETModel.Game.Scene.GetComponent<SceneChangeComponent>();
            timerComponent = ETModel.Game.Scene.GetComponent<TimerComponent>();

           // StartAsync(self).Coroutine();

            Game.EventCenter.EventListenner<float>(EventTypeId.LOAD_SCENE_PROGRESS, self.LoadSceneProgress);

           
        }
        public async ETVoid StartAsync(UISceneLoadingComponent self)
        {
           
            long instanceId = self.InstanceId;
            while (true)
            {
                //等待100毫秒
                await timerComponent.WaitAsync(100);
                if (self.InstanceId != instanceId)
                    return;
              
                if (sceneChangeComponent == null) continue;
                self.slider.value = sceneChangeComponent.Process;
                //self.text.text = $"{sceneChangeComponent.Process}%";
                self.progre.text = $"{sceneChangeComponent.Process}%";

            }

        }
    }
    /// <summary>
    /// 场景加载进度面板
    /// </summary>
    public class UISceneLoadingComponent : Component
    {
        public Text text, progre;
        public Slider slider;

        public Image Icon;
        public RenderTexture renderTexture;
        bool isChange = false;

        public void LoadSceneProgress(float progress)
        {
            slider.value = progress*100;
            progre.text = $"{(int)(slider.value)}%";

            Icon.fillAmount = progress;
            if (isChange == false)
            {
                
                isChange = true;
                RenderTexture.ReleaseTemporary(renderTexture);
                renderTexture = null;
                renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
              
                CameraComponent.Instance.MainCamera.targetTexture = renderTexture;
            }
        } 
      
        public override void Dispose()
        {
            
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();
            isChange = false;
            Game.EventCenter.RemoveEvent<float>(EventTypeId.LOAD_SCENE_PROGRESS, LoadSceneProgress);

            CameraComponent.Instance.MainCamera.targetTexture = null;
            RenderTexture.ReleaseTemporary(renderTexture);
            renderTexture = null;
            CameraComponent.Instance.UICamera.cullingMask |= (1 << 5);
            CameraComponent.Instance.UICamera.cullingMask &= ~(1 << 10);

        }

    }
}