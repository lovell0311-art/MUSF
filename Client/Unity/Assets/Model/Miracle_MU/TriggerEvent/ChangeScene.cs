using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 传送门 切换场景
    /// </summary>
    public class ChangeScene : MonoBehaviour
    { 
        /// <summary>
        /// 场景名字
        /// </summary>
        public int nextsceneId;
        private void OnTriggerEnter(Collider other)
        {
            TriggerEvents.Instance.RoleActionEnter?.Invoke(other.transform);
            /*if (nextsceneId!=0 && other.CompareTag("LocaRole"))
            {
                TriggerEvents.Instance.ChangeSceneAction?.Invoke(nextsceneId);
            }
            if (other.CompareTag("LocaRole") || other.CompareTag("Player"))
            {
                TriggerEvents.Instance.Enteraction?.Invoke(other.transform, TriggerEvents.Instance.UUId);
            }*/
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerEvents.Instance.RoleActionLevea?.Invoke(other.transform);
            //if (other.CompareTag("LocaRole") || other.CompareTag("Player"))
            //{
            //    TriggerEvents.Instance.Leaveaction?.Invoke(other.transform, TriggerEvents.Instance.UUId);
            //}
        }
        private void OnTriggerStay(Collider other)
        {
            TriggerEvents.Instance.RoleActionStay?.Invoke(other.transform);
            //if (other.CompareTag("LocaRole") || other.CompareTag("Player"))
            //{
            //    TriggerEvents.Instance.Leaveaction?.Invoke(other.transform, TriggerEvents.Instance.UUId);
            //}
        }
    }
}
