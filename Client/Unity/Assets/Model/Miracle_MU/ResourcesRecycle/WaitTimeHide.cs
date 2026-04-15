using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 延时隐藏
    /// </summary>
    public class WaitTimeHide : MonoBehaviour
    {
        [HideInInspector]
        public GameObject obj;
        /// <summary>
        /// 延时时间 时间戳秒
        /// </summary>
        [Tooltip("时间戳 秒")]
        public float time=3;
        private void OnEnable()
        {
            obj = this.gameObject;
            Invoke(nameof(Hide), time);
        }

        public void Hide()
        {
            obj.SetActive(false);
        }
    }
}
