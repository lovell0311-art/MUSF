using UnityEngine;
using ETModel;
namespace ETModel
{
    public static class AstarExtend
    {

        public static Vector3Int ToVector3(this AstarNode self)
        {
            return AstarComponent.Instance.GetNextVectory3(self.x, self.z);
        }

    }
}
