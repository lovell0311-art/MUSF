using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(Projector))]
    [RequireComponent(typeof(ShadowShaderReplacement))]
    public class MobileFastShadow : MonoBehaviour
    {
        public GameObject FollowTarget;
        /// <summary>
        /// 那个层级的物体需要投射阴影
        /// </summary>
        [Header("Shadow Layer")]
        [Tooltip("需要投射阴影的物体的层级")]
        private LayerMask LayerCaster;
        [Tooltip("需要接收阴影的物体的层级")]
        private LayerMask LayerIgnoreReceiver;


        //抗锯齿
        public enum AntiAliasing
        {
            None = 1,
            Samples2 = 2,
            Samples4 = 4,
            Samples8 = 8,
        }

        [Header("阴影细节设置")]
        [Tooltip("纹理大小")]
        public Vector2 Size = new Vector2(1024, 1024);
        [Tooltip("阴影采样时，如果想让边缘尽量平滑，就选择较高的样本，同样 性能也会下降")]
        public AntiAliasing RTAntiAliasing = AntiAliasing.None;
        [Tooltip("为了防止RenderedTarget边缘阴影被拉伸，有必要使用一种过度图片来处理它使它更加自然")]
        public Texture2D FalloffTex;
        [Range(0, 1)]
        [Tooltip("调整阴影透明度")]
        public float Intensity = 0.3f;
        [Header("Shadow Direction (Runtime)")]
        [Tooltip("To adjust the direction of the shadow.")]
        public Vector3 Direction = new Vector3(50, -30, -20);

        [Header("阴影正投影尺寸")]
        [Tooltip("数值越大，被阴影的对象就越多。它可以解决同一屏幕内阴影模糊问题。但是值越高也会导致阴影的质量下降。所以找一个适合的平衡。为了效率最大化运行时不支持调整Size of Projector和Camera 这两个值在运行时 后台会初始化，所以可以用这个值来调整初始化值")]
        public float ProjectionSize = 20;

        private Camera shadowCam;
        private Transform shadowCamTrans;
        private Projector projector;

        private Material shadowMat;
        private RenderTexture shadowRT;


        void Awake()
        {

            //默认阴影质量低
            RTAntiAliasing = AntiAliasing.Samples2;

            //LayerCaster = LayerNames.GetLayerInt(LayerNames.ROLE);
            // Log.DebugGreen($"LayerNames.GetLayerInt(LayerNames.ROLE):{LayerNames.GetLayerInt(LayerNames.ROLE)}");
            //LayerIgnoreReceiver = LayerNames.GetLayerInt(LayerNames.MAP);
            //设置衰减
            FalloffTex = (Texture2D)Resources.Load("Texture/shadow_falloff");

            //projector初始化
            projector = GetComponent<Projector>();
            if (projector == null)
                Debug.LogError("Projector Component Missing!!");
            projector.orthographic = true;
            projector.orthographicSize = ProjectionSize;
            projector.aspectRatio = Size.x / Size.y;
            shadowMat = new Material(Shader.Find("ShadowSystem/ProjectorShadow"));
            projector.material = shadowMat;
            shadowMat.SetTexture("_FalloffTex", FalloffTex);
            shadowMat.SetFloat("_Intensity", Intensity);
            //Debug.Log(LayerIgnoreReceiver.value + "---------------" + LayerNames.GetLayerInt(LayerNames.MAP));

            //projector.ignoreLayers = LayerIgnoreReceiver;

            //camera初始化
            shadowCam = GetComponent<Camera>();
            if (shadowCam == null)
                Debug.LogError("Camera Component Missing!!");
            shadowCamTrans = shadowCam.transform;
            shadowCam.clearFlags = CameraClearFlags.SolidColor;
            shadowCam.backgroundColor = new Color(0, 0, 0, 0);
            shadowCam.orthographic = true;
            shadowCam.orthographicSize = ProjectionSize;
            shadowCam.depth = int.MinValue;
            //shadowCam.cullingMask = LayerCaster;
            shadowRT = new RenderTexture((int)Size.x, (int)Size.y, 0, RenderTextureFormat.ARGB32);
            shadowRT.name = "ShadowRT";
            shadowRT.antiAliasing = (int)RTAntiAliasing;
            shadowRT.filterMode = FilterMode.Bilinear;
            shadowRT.wrapMode = TextureWrapMode.Clamp;
            shadowCam.targetTexture = shadowRT;
            shadowMat.SetTexture("_ShadowTex", shadowRT);

            //这边写死需要投射阴影的物体和需要接收阴影的物体,免得改来改去出错
            SetCustomLayer();

        }
        //实时调节相关参数
        private void LateUpdate()
        {
            if (FollowTarget == null)
                return;

            Vector3 pos = transform.forward;
            pos *= Direction.z;
            transform.position = FollowTarget.transform.position + pos;

            shadowCamTrans.rotation = Quaternion.Euler(Direction);
        }

        public void SetCustomLayer()
        {
            //需要投射阴影的物体
            List<string> LayerCasterList = new List<string>();
            LayerCasterList.Add(LayerNames.ROLE);
            LayerCasterList.Add(LayerNames.MONSTER);
            LayerCasterList.Add(LayerNames.NPC);
            //接收阴影的物体
            List<string> LayerIgnoreReceiverList = new List<string>();
            LayerIgnoreReceiverList.Add(LayerNames.MAP);
            SetLayer(LayerCasterList, LayerIgnoreReceiverList);
        }

        //设置层次
        public void SetLayer(List<string> LayerCasterList, List<string> LayerIgnoreReceiverList)
        {
            //LayerCaster
            for (int i = 0; i < LayerCasterList.Count; i++)
            {
                if (i == 0)
                    LayerCaster = 1 << LayerMask.NameToLayer((LayerCasterList[i]));
                LayerCaster = LayerCaster | 1 << LayerMask.NameToLayer((LayerCasterList[i]));
            }
            shadowCam.cullingMask = LayerCaster;

            //LayerIgnoreReceiver
            for (int i = 0; i < LayerIgnoreReceiverList.Count; i++)
            {
                if (i == 0)
                    LayerIgnoreReceiver = 1 << LayerMask.NameToLayer((LayerIgnoreReceiverList[i]));
                LayerIgnoreReceiver = LayerIgnoreReceiver | 1 << LayerMask.NameToLayer((LayerIgnoreReceiverList[i]));
            }
            LayerIgnoreReceiver = ~(LayerIgnoreReceiver);
            projector.ignoreLayers = LayerIgnoreReceiver;
        }
        //设置阴影质量
        public enum ShadowQuality
        {
            Low,
            Middle,
            High,
        }
        public void SelectShadowQuality(ShadowQuality quality)
        {
            switch (quality)
            {
                case ShadowQuality.Low:
                    RTAntiAliasing = AntiAliasing.Samples2;
                    shadowRT.width = 1024;
                    shadowRT.height = 1024;
                    break;
                case ShadowQuality.Middle:
                    RTAntiAliasing = AntiAliasing.Samples4;
                    shadowRT.width = 2048;
                    shadowRT.height = 2048;
                    break;
                case ShadowQuality.High:
                    RTAntiAliasing = AntiAliasing.Samples8;
                    shadowRT.width = 4096;
                    shadowRT.height = 4096;
                    break;
                default:
                    Debug.LogError("ShadowQuality Parameter Error!");
                    break;
            }
            shadowRT.antiAliasing = (int)RTAntiAliasing;
        }

    }
}