using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ETModel
{
    /// <summary>
    /// 物品一直旋转
    /// </summary>
    public class ObjRotate : MonoBehaviour
    {
        [Header("绕着X轴旋转")]
        public bool rotate_X = false;
        [Header("绕着Y轴旋转")]
        public bool rotate_Y = true;
        [Header("绕着Z轴旋转")]
        public bool rotate_Z = false;
        // Start is called before the first frame update
        void Start()
        {
         
        }
        private void Update()
        {
            if(rotate_Y)
            transform.Rotate(new Vector3(0, 1, 0));  //绕y轴旋转
            else if(rotate_X)
                transform.Rotate(new Vector3(1, 0, 0));
            else if(rotate_Z)
                transform.Rotate(new Vector3(0, 0, 1));
        }

    }
}