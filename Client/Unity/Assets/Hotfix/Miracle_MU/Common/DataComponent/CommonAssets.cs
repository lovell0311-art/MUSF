using ETModel;
using UnityEngine;
using System.Collections.Generic;

namespace ETHotfix
{
    public static class CommonAssetsKeys
    {



    }
    /// <summary>
    /// 公共资源
    /// </summary>
    public static class CommonAssets
    {
        public static readonly Dictionary<string, ReferenceCollector> MonoReferences = new Dictionary<string, ReferenceCollector>();
        public static async void LoadAssetAsync(string key)
        {
            if (MonoReferences.ContainsKey(key))
            {
                Log.Error("公共资源已经存在:" + key);
                return;
            }
            await AssetBundleComponent.Instance.LHLoadBundleAsync(key.StringToAB());
            ReferenceCollector monoReference = (AssetBundleComponent.Instance.GetAsset(key.StringToAB(), key) as GameObject).GetReferenceCollector();
            Log.DebugBrown($"monoReference-》{monoReference.name}");
            MonoReferences.Add(key, monoReference);
        }
        public static void AddAsset(string key, ReferenceCollector monoReference)
        {
            if (MonoReferences.ContainsKey(key))
            {
                Log.Error("公共资源已经存在:" + key);
                return;
            }
        }
        public static ReferenceCollector GetMonoReference(string key)
        {
            if (MonoReferences.TryGetValue(key, out ReferenceCollector monor))
            {
                return monor;
            }

            Log.Error("公共资源不存在:" + key);

            return null;
        }
    }
}
