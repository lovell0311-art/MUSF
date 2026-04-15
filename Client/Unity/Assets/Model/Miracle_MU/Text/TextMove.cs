using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{   
    /// <summary>
    /// 滚动文本字幕
    /// </summary>
    public class TextMove : MonoBehaviour
    {
        public float speed=50;

        public RectTransform maskRec;
        public RectTransform rec;

        public bool isLoop = true;//是否循环
        float local_X;
        float local_Y;
        float local_Z;
        float txtWidth;
        public Action RollOveraction;//滚动完成
        void Start()
        {
            local_Y = transform.localPosition.y;
            local_Z = transform.localPosition.z;

            rec.anchoredPosition = new Vector2(maskRec.rect.width,0);
        }

        void Update()
        {
            if(speed!=0)
            {
                txtWidth = rec.rect.width;
                if (rec.anchoredPosition.x < -txtWidth)
                {
                    rec.anchoredPosition = new Vector2(maskRec.rect.width,0);
                    if (!isLoop)
                    {
                        maskRec.gameObject.SetActive(false);
                        speed = 0;
                    }
                    RollOveraction?.Invoke();
                }
                local_X = transform.localPosition.x - speed * Time.deltaTime;
                transform.localPosition = new Vector3(local_X,local_Y,local_Z);

            }

        }
    }
}
