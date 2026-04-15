using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{

    [ObjectSystem]
    public class PreLoadComponentAwake : AwakeSystem<PreLoadComponent>
    {
        public override void Awake(PreLoadComponent self)
        {
            PreLoadComponent.Instance = self;

        }
    }

    /// <summary>
    /// 预加载 组件
    /// </summary>
    public class PreLoadComponent : Component
    {

        public static PreLoadComponent Instance;
        //场景 怪物字典
        private Dictionary<int, List<GameObject>> MapMonsterDic = new Dictionary<int, List<GameObject>>();

        public void PreLoadScene()
        {
            AssetBundleComponent.Instance.LoadBundle(SceneName.YongZheDaLu.ToString());
            AssetBundleComponent.Instance.LoadBundle(SceneName.XianZongLin.ToString());
            AssetBundleComponent.Instance.LoadBundle(SceneName.HuanShuYuan.ToString());
        }
        /// <summary>
        /// 预加载怪物
        /// </summary>
        /// <param name="sceneId">场景ID</param>
        public void PreLoad(int sceneId, string monsterName)
        {

            PreLoadObj(monsterName, sceneId).Coroutine();
        }

        public void UnLoad(int sceneId)
        {

            if (MapMonsterDic.TryGetValue(sceneId, out List<GameObject> monsterList))
            {
                foreach (var resobj in monsterList)
                {
                    //    Log.DebugRed($"清理上一个场景：{sceneId} ->{resobj.name}");
                    //ResourcesComponent.Instance.DestoryGameObjectImmediate(resobj,resobj.name.StringToAB());
                    ResourcesComponent.Instance.RecycleGameObject(resobj);

                }

            }
        }

        /// <summary>
        /// 预加载模型
        /// </summary>
        /// <param name="res"></param>
        public async ETVoid PreLoadObj(string res, int sceneId)
        {
            await ResourcesComponent.Instance.LoadGameObjectAsync(res.StringToAB(), res);
            GameObject obj = ResourcesComponent.Instance.LoadGameObject(res.StringToAB(), res);

            if (MapMonsterDic.TryGetValue(sceneId, out List<GameObject> monstetList))
            {
                monstetList.Add(obj);
            }
            else
            {
                MapMonsterDic[sceneId] = new List<GameObject>() { obj };
            }
            ResourcesComponent.Instance.RecycleGameObject(obj);
        }
    }
}
