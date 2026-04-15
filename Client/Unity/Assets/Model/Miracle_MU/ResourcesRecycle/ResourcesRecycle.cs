using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{

    /// <summary>
    /// 资源回收
    /// </summary>
    public class ResourcesRecycle : MonoBehaviour
    {
        [Header("时间戳：毫秒秒")]
        public float RecycleTime = 1000;
       /* private GameObject obj;

        public Action<int> CallBack;
        [HideInInspector]
        public int Index;*/
        /// <summary>
        /// 延迟回收时间
        /// 时间戳 秒
        /// </summary>
       /* public float DelayTime 
        {
            get { return RecycleTime; }
            set 
            {
                RecycleTime = value;
                if (IsInvoking(nameof(OnRecycle)))
                    CancelInvoke(nameof(OnRecycle));

                Invoke(nameof(OnRecycle),value);
            }
        }*/
       /* public void OnEnable()
        {
            obj = this.gameObject;
            Invoke(nameof(OnRecycle),RecycleTime);

        }

        private void OnRecycle() 
        {
            ResourcesComponent.Instance.RecycleGameObject(obj);
            if(obj!=null)
            CallBack?.Invoke(Index);
        }*/
    }
}