using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETModel
{
    /// <summary>
    /// 通过烘焙将生成的数据存储在单独文件中 避免烘焙贴图后 再拖拽成预制体后场景光照信息丢失
    /// </summary>
    public class PrefabLightmapData : MonoBehaviour
    {
        [System.Serializable]
        struct RendererInfo
        {
            public Renderer renderer;
            public int lightmapIndex;
            public Vector4 lightmapOffsetScale;
        }

        [SerializeField]
        RendererInfo[] m_RendererInfo;
        [SerializeField]
        Texture2D[] m_Lightmaps;

        void Awake()
        {
            if (m_RendererInfo == null || m_RendererInfo.Length == 0)
                return;

            var lightmaps = LightmapSettings.lightmaps;
            var combinedLightmaps = new LightmapData[lightmaps.Length + m_Lightmaps.Length];

            lightmaps.CopyTo(combinedLightmaps, 0);
            for (int i = 0; i < m_Lightmaps.Length; i++)
            {
                combinedLightmaps[i + lightmaps.Length] = new LightmapData();
                combinedLightmaps[i + lightmaps.Length].lightmapColor = m_Lightmaps[i];
            }

            ApplyRendererInfo(m_RendererInfo, lightmaps.Length);
            LightmapSettings.lightmaps = combinedLightmaps;
        }


        static void ApplyRendererInfo(RendererInfo[] infos, int lightmapOffsetIndex)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                info.renderer.lightmapIndex = info.lightmapIndex + lightmapOffsetIndex;
                info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("BakeTool/Bake Prefab Lightmaps")]
        [System.Obsolete]
        static void GenerateLightmapInfo()
        {
            if (UnityEditor.Lightmapping.giWorkflowMode != UnityEditor.Lightmapping.GIWorkflowMode.OnDemand)
            {
                Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
                return;
            }
            UnityEditor.Lightmapping.Bake();

            PrefabLightmapData[] prefabs = FindObjectsOfType<PrefabLightmapData>();

            foreach (var instance in prefabs)
            {
                var gameObject = instance.gameObject;
                var rendererInfos = new List<RendererInfo>();
                var lightmaps = new List<Texture2D>();

                GenerateLightmapInfo(gameObject, rendererInfos, lightmaps);

                instance.m_RendererInfo = rendererInfos.ToArray();
                instance.m_Lightmaps = lightmaps.ToArray();


                var targetPrefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject) as GameObject;
                if (targetPrefab != null)
                {
                    //UnityEditor.Prefab
                    UnityEditor.PrefabUtility.ReplacePrefab(gameObject, targetPrefab);
                }
            }
        }

        static void GenerateLightmapInfo(GameObject root, List<RendererInfo> rendererInfos, List<Texture2D> lightmaps)
        {
            var renderers = root.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                if (renderer.lightmapIndex != -1)
                {
                    RendererInfo info = new RendererInfo();
                    info.renderer = renderer;
                    info.lightmapOffsetScale = renderer.lightmapScaleOffset;

                    Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;

                    info.lightmapIndex = lightmaps.IndexOf(lightmap);
                    if (info.lightmapIndex == -1)
                    {
                        info.lightmapIndex = lightmaps.Count;
                        lightmaps.Add(lightmap);
                    }

                    rendererInfos.Add(info);
                }
            }
        }
#endif

    }
}
