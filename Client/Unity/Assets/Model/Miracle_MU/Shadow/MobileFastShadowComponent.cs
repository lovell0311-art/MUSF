using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class MobileFastShadowComponentAwake : AwakeSystem<MobileFastShadowComponent>
    {
        public override void Awake(MobileFastShadowComponent self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 阴影组件
    /// </summary>
    public class MobileFastShadowComponent : Component
    {
        public static MobileFastShadowComponent Instance
        { get; private set; }
        /// <summary>
        /// 全局阴影
        /// </summary>
        public MobileFastShadow GlobalShadow { get; private set; }
        /// <summary>
        /// 局部阴影(用于其他阴影计算)
        /// </summary>
        private MobileFastShadow mLocalShadow;
        /// <summary>
        /// 阴影对象池
        /// </summary>
        private Queue<MobileFastShadow> mobileFastShadows = new Queue<MobileFastShadow>();
        public void Awake()
        {
            Instance = this;
            if (GlobalShadow == null)
            {
                GameObject shadowObj = Resources.Load<GameObject>("MobileFastShadow");
                shadowObj = GameObject.Instantiate(shadowObj);
                GameObject.DontDestroyOnLoad(shadowObj);

                GlobalShadow = shadowObj.GetComponent<MobileFastShadow>();
            }

        }
        public MobileFastShadow GetLocalShadow()
        {
            if (mLocalShadow == null)
            {
                MobileFastShadow mShadow = GetShadowByListShadow();
                if (mShadow == null)
                {
                    GameObject shadowObj = Resources.Load<GameObject>("MobileFastShadow");
                    shadowObj = GameObject.Instantiate(shadowObj);
                    mLocalShadow = shadowObj.GetComponent<MobileFastShadow>();
                }
                else
                {
                    mLocalShadow = mShadow;
                }
            }
            return mLocalShadow;
        }
        /// <summary>
        /// 单独实例化一个阴影
        /// </summary>
        /// <returns></returns>
        public MobileFastShadow InitShadow()
        {
            MobileFastShadow mShadow = GetShadowByListShadow();
            if (mShadow == null)
            {
                GameObject shadowObj = Resources.Load<GameObject>("MobileFastShadow");
                shadowObj = GameObject.Instantiate(shadowObj);
                MobileFastShadow moShadow = shadowObj.GetComponent<MobileFastShadow>();
                return moShadow;
            }
            else
            {
                return mShadow;
            }
        }
        public void RecycleShadow(MobileFastShadow mobileFastShadow)
        {
            if (mobileFastShadow != null)
            {
                mobileFastShadow.gameObject.SetActive(false);
                mobileFastShadows.Enqueue(mobileFastShadow);
            }
        }
        private MobileFastShadow GetShadowByListShadow()
        {
            if (mobileFastShadows.Count != 0)
            {
                MobileFastShadow mShadow = mobileFastShadows.Dequeue();
                mShadow.gameObject.SetActive(true);
                return mShadow;
            }
            return null;
        }
    }
}
