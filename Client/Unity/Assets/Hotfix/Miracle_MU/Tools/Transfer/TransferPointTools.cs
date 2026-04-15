using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;
using ETModel;


namespace ETHotfix
{

    [ObjectSystem]
    public class TransferPointToolsAwake : AwakeSystem<TransferPointTools>
    {
        public override void Awake(TransferPointTools self)
        {
           self.Awake();
        }
    }
    public class TransferPointTools : Component
    {
        public static TransferPointTools Instances;
        public string PointResName = "TransferPoints";

        private List<Vector2Int> pointList = new List<Vector2Int>();

        public ReferenceCollector collector;
        public string CurSceneName;
        public TextAsset safestr;

        public Func<string> CurrentSceneName;

        public void Awake() 
        {
            Instances = this;
            AssetBundleComponent.Instance.LoadBundle(PointResName.StringToAB());
            GameObject safes = (GameObject)AssetBundleComponent.Instance.GetAsset(PointResName.StringToAB(), PointResName);
            collector = safes.GetReferenceCollector();
        }

        /// <summary>
        /// 获取当前 场景的传送区域
        /// </summary>
        public void GetCurSceneTransferPoints(string sceneName)
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
            pointList.Clear();
            safestr = collector.GetTextAsset(sceneName + "_TransferPoint");//XianZongLin_TransferPoint
            if (safestr == null)
            {

              //  Log.DebugRed($"{CurrentSceneName?.Invoke()} 不存在传送区域");
                return;
            }

            //转为json

            SpawnPoint[] data = JsonMapper.ToObject<SpawnPoint[]>(safestr.text);


            foreach (var item in data)
            {

                pointList.Add(new Vector2Int(item.PositionX, item.PositionY));
            }

        }
        /// <summary>
        /// 是否在传送区域
        /// </summary>
        /// <param name="vector2">当前的格子坐标</param>
        /// <returns>
        /// true 安全区
        /// false 非安全区
        /// </returns>
        public bool IsTransferAreas(Vector2Int vector2)
        {
            if (pointList.Contains(vector2))
            {
                // Log.DebugYellow("安全区");
                return true;
            }
            // Log.DebugYellow($"非安全区:{vector2}");
            return false;
        }
        public void AddTransferAreas(Vector2Int vector2)
        {
            pointList.Add(vector2);
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            pointList = null;
            AssetBundleComponent.Instance.UnloadBundle(PointResName.StringToAB());
        }
    }
}
