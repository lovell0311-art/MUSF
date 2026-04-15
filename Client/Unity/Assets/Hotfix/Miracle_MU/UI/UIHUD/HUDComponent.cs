using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class HUDComponentAwake : AwakeSystem<HUDComponent>
    {
        public override void Awake(HUDComponent self)
        {
            HUDComponent.Instance = self;
            self.reference = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.parentTrs = self.GetParent<UI>().GameObject.transform;
        }
    }
    /// <summary>
    /// 装备名字显示
    /// </summary>
    public class HUDComponent : Component
    {
        public static HUDComponent Instance;

        public ReferenceCollector reference;

        public Transform parentTrs;
       

        private Queue<GameObject> toolTipPool = new Queue<GameObject>();

        public GameObject FetchToolTip()
        {
            if (toolTipPool.Count != 0)
            {
                GameObject go = toolTipPool.Dequeue();
                go.SetActive(true);
                return go;
            }
            else
            {
                GameObject go = GameObject.Instantiate<GameObject>(reference.GetImage("ToolTip").gameObject, parentTrs);
                go.SetActive(true);
                return go;
            }
        }

        public void RecycleToolTip(GameObject go)
        {
            if (toolTipPool.Count < 10)
            {
                go.SetActive(false);
                toolTipPool.Enqueue(go);
            }
            else
            {
                GameObject.Destroy(go);
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            toolTipPool.Clear();
            Instance = null;
            base.Dispose();
        }
    }
}
