using ETModel;
using UnityEngine;

namespace ETHotfix
{
   // [ObjectSystem]
    public class SitDownStoolsComponentUpdate : UpdateSystem<SitDownStoolsComponent>
    {
        public override void Update(SitDownStoolsComponent self)
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    self.ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    if (Physics.Raycast(self.ray, out self.hit))
            //    {
            //        Debug.Log(self.hit.collider.gameObject.name);
            //        self.obj = self.hit.collider.gameObject;
            //        //通过标签
            //        if (self.obj.tag == "Stools")
            //        {
            //            Debug.Log("点中" + self.obj.name);
            //        }
            //    }
            //}
        }
    }
    [ObjectSystem]
    public class SitDownStoolsComponentAwake : AwakeSystem<SitDownStoolsComponent>
    {
        public override void Awake(SitDownStoolsComponent self)
        {
            self.Awake();
        }
    }
    public partial class SitDownStoolsComponent : Component
    {
        public static SitDownStoolsComponent Instance;

        public Ray ray;
        public RaycastHit hit;
        public GameObject obj;
        internal void Awake()
        {
            Instance = this;
        }
    }

}
