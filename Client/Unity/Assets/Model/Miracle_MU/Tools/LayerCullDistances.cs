using UnityEngine;

namespace ETModel
{
    public class LayerCullDistances : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Camera camera = GetComponent<Camera>();
            float[] distances = new float[32];
          //  distances[LayerMask.NameToLayer("Map")] = 68;
            distances[LayerMask.NameToLayer("Monster")] = 55;
            distances[LayerMask.NameToLayer("NPC")] = 60;
            distances[LayerMask.NameToLayer("Role")] = 60;
            distances[LayerMask.NameToLayer("fx")] = 90;
            //每一层的剔除距离
            camera.layerCullDistances = distances;
            //好处：在相机转动时，不会影响哪些对象可见或不可见，也就是不会转动中，有些原来显示的对象被隐藏
            camera.layerCullSpherical = true;
        }

     
    }
}