using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 延时隐藏、销毁
    /// </summary>
    public class ActiveHide : MonoBehaviour
    {

        /// <summary>
        /// 缓存时间 时间戳 秒 
        /// 15秒没有使用 就销毁该Obj
        /// </summary>
        public float delayTime = 1;
        public GameObject obj;
        private void Awake()
        {
            obj = this.gameObject;
            delayTime = ResourcesComponent.Instance.destoryTime;
        }
        //回调函数
        public Action callBack;
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="go">回收对象</param>
        /// <param name="delay">等待时间（时间戳 秒）</param>
        public void CollectObject(GameObject go, float delay = 0)
        {
            if (this.gameObject.activeSelf == false) return;
            //延迟调用
            StartCoroutine(CollectObjectDelay(go, delay));

        }
        private IEnumerator CollectObjectDelay(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
            go.SetActive(false);
        }


        //不可见时 等待销毁
        private void OnDisable()
        {
            // Log.DebugBrown($"等待 {delayTime}s 销毁{obj.name}-{obj.GetInstanceID()}：");

            Invoke(nameof(WaitDestroy), delayTime);
        }
        //使用时 取消销毁
        private void OnEnable()
        {
            if (IsInvoking(nameof(WaitDestroy)))//WaitDestroy 方法是否挂起
                CancelInvoke(nameof(WaitDestroy));
            //  Log.DebugRed($"取消销毁：{obj.name}-{obj.GetInstanceID()}");
        }
        public void WaitDestroy()
        {
          
            ResourcesComponent.Instance.RealeseGameObject(obj, obj.GetInstanceID());
            Destroy(obj);

        }
    }

}