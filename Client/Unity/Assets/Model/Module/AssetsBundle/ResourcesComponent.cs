using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ETModel
{

    [ObjectSystem]
    public class ResourcesComponentAwake : AwakeSystem<ResourcesComponent>
    {
        public override void Awake(ResourcesComponent self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 单个 对象池 类
    /// </summary>
    public class PoolData
    {
        //对象缓存字典
        public Dictionary<int, GameObject> PoolList = new Dictionary<int, GameObject>();

        public Queue<GameObject> _spawned = new Queue<GameObject>();
        public string AbResName;//ab资源名

        public long lastusetime = 0;
        //是否自动回收
        public bool cullDespawned = true;
        //回收上限
        public int cullAbove = 50;
        //回收的延迟（超上限后 开启定时回收） （秒）
        public int cullDelay = 30;
        //单次回收的个数（定时器到后 回收一波 如果还超上限 则重新定时）
        public int cullMaxPerPass = 5;
        public bool cullingActive = false;
        public int totalCount
        {
            get
            {
                return _spawned.Count;

            }
        }
        /// <summary>
        /// 是否处于使用中
        /// </summary>
        /// <returns>false:为闲置状态可以清理  true:使用中</returns>
        public bool IsUser()
        {
            if (TimeHelper.Now() - lastusetime > 2 * 60 * 1000)
            {
                return false;
            }

            return true;
        }
        public PoolData(GameObject obj, int delayTime = 0)
        {
            AbResName = obj.name;
            //  Log.DebugGreen($"新建对象池：{obj.name} {_spawned.Count}");
            PushObj(obj, delayTime);
        }


        /// <summary>
        /// 向对象池 添加对象
        /// </summary>
        /// <param name="obj">预制体</param>
        /// <param name="delayTime">延时 时间（毫秒）</param>
        public void PushObj(GameObject obj, int delayTime = 0)
        {
            string name = obj.name;
            //Log.DebugRed($"回收：{name}  Count:{_spawned.Count}");
            DelayHide().Coroutine();

            async ETVoid DelayHide()
            {
                await TimerComponent.Instance.WaitAsync(delayTime);
                if (obj == null)
                {
                    Log.DebugRed($"回收：错误 {name}");

                    return;
                }
                obj.SetActive(false);
                _spawned.Enqueue(obj);

                lastusetime = TimeHelper.Now();

                if (!this.cullingActive && // 是否开启了定时器 防止多次触发
                this.cullDespawned && //是否开启了自动释放
                this.totalCount > this.cullAbove) // 是否满足了回收条件 超过了设定的上线
                {
                    this.cullingActive = true;
                    //开启一个自动释放的 定时器
                    CullDespawned().Coroutine();
                }
            }
        }


        private async ETVoid CullDespawned()
        {
            //首次 先等待cullDelay秒
            await TimerComponent.Instance.WaitAsync(cullDelay * 1000);
            //while 循环 判断是否满足释放条件
            while (this.totalCount > this.cullAbove)
            {
                //int tempCount = this.totalCount;
                //尝试去释放cullMaxPerPass个数的对象
                for (int i = 0; i < this.cullMaxPerPass; i++)
                {
                    //条件不满足 打断循环
                    if (this.totalCount <= this.cullAbove)
                        break;
                    //销毁第一个对象
                    if (this._spawned.Count > 0)
                    {
                        GameObject inst = this._spawned.Dequeue();
                        UnityEngine.Object.Destroy(inst.gameObject);
                    }
                    else
                    {
                        //对象池暂时为空 打断循环
                        break;
                    }
                }
                //等待cullDelay秒后 再次检测
                await TimerComponent.Instance.WaitAsync(cullDelay * 1000);
            }
            //结束本次释放 重置变量
            this.cullingActive = false;

        }

        /// <summary>
        /// 从池子里 获取对象
        /// </summary>
        /// <returns>返回实例对象</returns>
        public GameObject GetObj()
        {
            /*  GameObject obj = PoolList.ElementAt(0).Value;
              PoolList.Remove(obj.GetInstanceID());
              obj.SetActive(true);
              return obj;*/

            if (_spawned.Count > 0)
            {
                try
                {
                    GameObject obj = _spawned.Dequeue();
                    //   Log.DebugRed($"{obj.name}");
                    obj.SetActive(true);
                    lastusetime = TimeHelper.Now();
                    //Log.Info("加载缓存--------------- " + obj.name);
                    return obj;
                }
                catch (Exception e)
                {

                    Log.DebugRed($"{e}");
                }

            }
            return null;
        }
        public void Realese(int id)
        {
            if (PoolList.ContainsKey(id))
            {
                PoolList.Remove(id);
                //  Log.DebugGreen($"释放-{objName}-字典：{PoolList.Count}");
            }
        }

        public void Clear()
        {
            for (int i = 0; i < PoolList.Values.Count; i++)
            {
                GameObject.Destroy(PoolList.ElementAt(i).Value);
            }
            PoolList.Clear();
            //卸载AB资源
            AssetBundleComponent.Instance.UnloadBundle(AbResName);
        }
        /// <summary>
        /// 清理释放资源
        /// </summary>
        public void ClearPool()
        {

            while (this._spawned.Count > 0)
            {
                var inst = _spawned.Dequeue();
                UnityEngine.Object.Destroy(inst);
            }
            _spawned.Clear();
            //卸载AB资源
            AssetBundleComponent.Instance.UnloadBundle(AbResName);

            //  GC.Collect();

        }
    }

    public class EffectData
    {
        public Queue<GameObject> _spawned = new Queue<GameObject>();
        public EffectData(GameObject obj)
        {
            _spawned.Enqueue(obj);
        }
        public void PushObj(GameObject obj)
        {
            _spawned.Enqueue(obj);
        }
        public void EffectDelay(int delayTime = 0)
        {
            // Log.DebugRed($"回收：{obj.name}  Count:{_spawned.Count}");
            DelayHide().Coroutine();

            async ETVoid DelayHide()
            {
                await TimerComponent.Instance.WaitAsync(delayTime);
                _spawned.Dequeue();
            }
        }
    }

    /// <summary>
    /// 资源管理类组件
    /// </summary>
    public class ResourcesComponent : Component
    {
        public static ResourcesComponent Instance { get; private set; }
        public Transform resourceRoot;
        //public Dictionary<string, EffectData> PoolEffectList = new Dictionary<string, EffectData>();
        public Dictionary<string, PoolData> PoolDic;
        public void Awake()
        {
            Instance = this;
            PoolDic = new Dictionary<string, PoolData>();
            resourceRoot = (new GameObject("ReourcesRoot")).transform;
            GameObject.DontDestroyOnLoad(resourceRoot);
            TimerComponent.Instance.RegisterTimeCallBack(delayClear * 60 * 1000, CheckIsOverPoolDic);
        }
        /// <summary>
        /// 标准缓存时间 时间戳 秒
        /// </summary>
        public int destoryTime = 15;

        //最大缓存个对象池
        public int PoolDicMaxCount = 5;

        //每隔15分钟检查一次
        public int delayClear = 15;
        //检查是否超过对象池的最大缓存数
        private void CheckIsOverPoolDic()
        {
            List<PoolData> poolList = new List<PoolData>();
            foreach (var s in PoolDic)
            {
                poolList.Add(s.Value);
            }
            foreach (var pool in poolList)
            {
                if (pool.IsUser() == false)
                {
                    pool.ClearPool();
                    PoolDic.Remove(pool.AbResName);
                    // Log.DebugYellow($"定时清理：{pool.AbResName}");
                }
            }
            poolList.Clear();
            poolList = null;



            /* for (int i = 0; i < PoolDic.Count; i++)
             {
                 var pool = PoolDic.ElementAt(i).Value;
                 if (pool.IsUser() == false)
                 {
                     pool.ClearPool();
                     PoolDic.Remove(pool.AbResName);
                     //await TimerComponent.Instance.WaitAsync(30 * 1000);
                 }
             }*/

            TimerComponent.Instance.RegisterTimeCallBack(delayClear * 60 * 1000, CheckIsOverPoolDic);

        }


        public async ETTask LoadGameObjectAsync(string abName, string assetName, Action action = null)
        {
            abName = abName.ToLower();
            if (PoolDic.ContainsKey(assetName) && PoolDic[assetName]._spawned.Count > 0)
            {

            }
            else
            {
                await AssetBundleComponent.Instance.LHLoadBundleAsync(abName);
            }
            await TimerComponent.Instance.WaitAsync(1);
            action?.Invoke();
        }

        public GameObject LoadGameObject(string abName, string assetName)
        {
            abName = abName.ToLower();
            GameObject obj = null;

            if (PoolDic.ContainsKey(assetName) && PoolDic[assetName]._spawned.Count > 0)
            {
                //Log.DebugGreen($"assetName:{assetName}  _spawned.Count:{PoolDic[assetName]._spawned.Count}");
                obj = PoolDic[assetName].GetObj();
                obj.transform.SetParent(resourceRoot);
            }
            else
            {
                obj = AssetBundleComponent.Instance.GetAsset(abName, assetName) as GameObject;
                if (obj == null)
                {
                    AssetBundleComponent.Instance.LoadBundle(abName);
                    obj = AssetBundleComponent.Instance.GetAsset(abName, assetName) as GameObject;
                    if (obj == null)
                    {
                        Log.Info("资源加载失败 " + abName);
                        return null;
                    }
                }

                //Log.Info($"实例化------------------3 abName={abName} assetName={assetName} ");
                obj = GameObject.Instantiate(obj, resourceRoot);
                obj.name = assetName;

            }

            return obj;

        }
        /// <summary>
        /// 获取特效资源（自动回收）
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="delayTime">自动回收时间（时间戳毫秒）</param>
        /// <returns></returns>
        public GameObject LoadEffectObject(string abName, string assetName, int delayTime = 1000)
        {
            //加载技能特效
            GameObject effect = LoadGameObject(abName, assetName);
            if (effect == null) return null;
            if (effect.GetComponent<PetSkillTrack>() != null)
            {
                return effect;
            }

            if (effect.TryGetComponent(out ResourcesRecycle resourcesRecycle))
            {
                if (resourcesRecycle.RecycleTime < 10)
                {
                    RecycleGameObject(effect, (int)resourcesRecycle.RecycleTime * 1000);
                }
                else
                {
                    RecycleGameObject(effect, (int)resourcesRecycle.RecycleTime);
                }
            }
            else
            {
                //默认500毫秒后回收
                RecycleGameObject(effect, delayTime);
            }
            return effect;
        }
        /// <summary>
        /// 放进缓存池
        /// </summary>
        /// <param name="gobj">需要被放入缓存池的OBj</param>
        /// <param name="delayTime">延迟时间隐藏（时间戳 秒）默认0秒</param>
        public void RecycleGameObject(GameObject gobj, int delayTime = 0)
        {
            //Log.Info("RecycleGameObject");

            if (gobj == null) return;
            //Log.Info($"销毁：{gobj.name}");
            //GameObject.Destroy(gobj);
            //return;
            string objname = gobj.name;
            gobj.transform.SetParent(resourceRoot);
            if (PoolDic.ContainsKey(objname))
            {
                PoolDic[objname].PushObj(gobj, delayTime);
            }
            else
            {
                PoolData poolData = new PoolData(gobj, delayTime);
                PoolDic[objname] = poolData;
            }
        }

        public void RecycleGameObject2(string name, GameObject gobj, int delayTime = 0)
        {
            //Log.Info("RecycleGameObject");

            if (gobj == null) return;
            Log.Info($"销毁：{gobj.name}");
            //GameObject.Destroy(gobj);
            //return;
            string objname = name;
            gobj.transform.SetParent(resourceRoot);
            Log.Info($"放入缓存池  ：{objname }  delayTime={delayTime}");

            if (PoolDic.ContainsKey(objname))
            {
                PoolDic[objname].PushObj(gobj, delayTime);
            }
            else
            {
                PoolData poolData = new PoolData(gobj, delayTime);
                PoolDic[objname] = poolData;
            }
        }

        /// <summary>
        /// 释放 资源
        /// 从缓存池中移除 
        /// 卸载对应的ab资源
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="Id"></param>
        public void RealeseGameObject(GameObject obj, int Id)
        {
            if (PoolDic.ContainsKey(obj.name))
            {
                //从缓存池 中移除
                PoolData poolData = PoolDic[obj.name];
                poolData.Realese(Id);
                //卸载 ab资源
                //AssetBundleComponent.Instance.UnloadBundle(obj.name.StringToAB());
            }

        }
        /// <summary>
        /// 立即销毁 不放进缓存池(从ab包中加载的物品)
        /// </summary>
        /// <param name="obj"></param>
        public void DestoryGameObjectImmediate(GameObject obj, string abresName)
        {
            UnityEngine.GameObject.DestroyImmediate(obj);
            //AssetBundleComponent.Instance.UnloadBundle(abresName);
        }

        public void GCClear()
        {
            Resources.UnloadUnusedAssets();
            // GC.Collect();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            foreach (var item in PoolDic.Values)
            {
                item.Clear();
            }
            PoolDic.Clear();
            PoolDic = null;
        }

    }

}