using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using LitJson;
using System;

namespace ETHotfix
{
    /// <summary>
    /// 安全 区域
    /// </summary>
    public class SafeAreaComponent : Component
    {
        public static SafeAreaComponent Instances;
        public string SafesResName = "Safes";

        private List<Vector2Int> safesList = new List<Vector2Int>();

        public ReferenceCollector collector;
        public string CurSceneName;
        public TextAsset safestr;

        public Func<string> CurrentSceneName;

        /// <summary>
        /// 获取当前 场景的安全区域
        /// </summary>
        public void GetCurSceneSafeAreas(string sceneName) 
        {
           
            if (CurSceneName == sceneName)
            {
                return;
            }
            if (sceneName == "ChooseRole")
            {
                return;
            }
            CurSceneName = sceneName;
            safesList.Clear();
            safestr = collector.GetTextAsset(sceneName + "_SafeArea");
            if (safestr == null)
            {
                
               // Log.DebugRed($"{CurrentSceneName?.Invoke()} 不存在安全区");
                return;
            }
            
            //转为json

            SpawnPoint[] data = JsonMapper.ToObject<SpawnPoint[]>(safestr.text);
            foreach (var item in data)
            {
           
                safesList.Add(new Vector2Int(item.PositionX,item.PositionY));
            }
            
        }
        /// <summary>
        /// 是否在安全区域
        /// </summary>
        /// <param name="vector2">当前的格子坐标</param>
        /// <returns>
        /// true 安全区
        /// false 非安全区
        /// </returns>
        public bool IsSafeAreas(Vector2Int vector2)
        {
            if (safesList == null || safesList.Count == 0) return false;
            if (safesList.Contains(vector2))
            {
                //Log.DebugYellow("安全区");
                return true;
            }
            //Log.DebugYellow($"非安全区:{vector2}");
            return false;
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            safesList = null;
            AssetBundleComponent.Instance.UnloadBundle(SafesResName.StringToAB());
        }
    }
}