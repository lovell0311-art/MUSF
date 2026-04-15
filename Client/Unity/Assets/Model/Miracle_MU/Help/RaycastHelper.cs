using UnityEngine;

namespace ETModel
{
    public static class RaycastHelper
    {
        public static bool CastMapPoint(out Vector3 hitPoint)
        {
            return CastMapPoint(GameUtility.GetPrimaryPointerScreenPosition(), out hitPoint);
        }

        public static bool CastMapPoint(Vector3 screenPosition, out Vector3 hitPoint)
        {
            if (TryGetRay(screenPosition, out Ray ray) &&
                Physics.Raycast(ray, out RaycastHit hit, 500, 1 << LayerMask.NameToLayer(LayerNames.MAP)))
            {
                hitPoint = hit.point;
                return true;
            }
            hitPoint = Vector3.zero;
            return false;
        }

        public static bool CastUnitObj(out GameObject castObj)
        {
            return CastUnitObj(GameUtility.GetPrimaryPointerScreenPosition(), out castObj);
        }

        public static bool CastUnitObj(Vector3 screenPosition, out GameObject castObj)
        {
            if (TryGetRay(screenPosition, out Ray ray) &&
                Physics.Raycast(ray, out RaycastHit hit, 500,
                    (1 << LayerMask.NameToLayer(LayerNames.MONSTER)) |
                    (1 << LayerMask.NameToLayer(LayerNames.ROLE)) |
                    (1 << LayerMask.NameToLayer(LayerNames.DOOR))))
            {
                castObj = hit.collider.gameObject;
                return true;
            }
            castObj = null;
            return false;
        }

        public static bool CastMonsterObj(out GameObject castObj)
        {
            return CastMonsterObj(GameUtility.GetPrimaryPointerScreenPosition(), out castObj);
        }

        public static bool CastMonsterObj(Vector3 screenPosition, out GameObject castObj)
        {
            if (TryGetRay(screenPosition, out Ray ray) &&
                Physics.Raycast(ray, out RaycastHit hit, 500, 1 << LayerMask.NameToLayer(LayerNames.MONSTER)))
            {
                castObj = hit.collider.gameObject;
                return true;
            }
            castObj = null;
            return false;
        }
        public static bool CastNPCObj(out GameObject castObj)
        {
            return CastNPCObj(GameUtility.GetPrimaryPointerScreenPosition(), out castObj);
        }

        public static bool CastNPCObj(Vector3 screenPosition, out GameObject castObj)
        {
            if (TryGetRay(screenPosition, out Ray ray) &&
                Physics.Raycast(ray, out RaycastHit hit, 500, 1 << LayerMask.NameToLayer(LayerNames.NPC)))
            {
                castObj = hit.collider.gameObject;
                return true;
            }
            castObj = null;
            return false;
        }
        /// <summary>
        /// 鏄惁鐐瑰嚮閫夋嫨浜嗙帺瀹?
        /// </summary>
        /// <param name="castObj"></param>
        /// <returns></returns>
        public static bool CastRoleObj(out GameObject castObj)
        {
            return CastRoleObj(GameUtility.GetPrimaryPointerScreenPosition(), out castObj);
        }

        public static bool CastRoleObj(Vector3 screenPosition, out GameObject castObj)
        {
            if (!TryGetRay(screenPosition, out Ray ray))
            {
                castObj = null;
                return false;
            }
            Debug.DrawRay(ray.origin,ray.direction,Color.green);
            if (Physics.Raycast(ray, out RaycastHit hit, 500, 1 << LayerMask.NameToLayer(LayerNames.ROLE)))
            {
                castObj = hit.collider.gameObject;
                return true;
            }
            castObj = null;
            return false;
        }

        private static bool TryGetRay(Vector3 screenPosition, out Ray ray)
        {
            if (Camera.main == null)
            {
                ray = default;
                return false;
            }

            ray = Camera.main.ScreenPointToRay(screenPosition);
            return true;
        }
        static Vector3 pos;
        /// <summary>
        /// 鐭鍦伴潰楂樺害
        /// </summary>
        /// <param name="self"></param>
        public static void GroundPos(this Transform self)
        {
            pos = self.position + Vector3.up * 50;
          
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 500, 1 << LayerNames.GetLayerInt(LayerNames.MAP)))
            {
                self.transform.position = hit.point;
            }
        }
        public static Vector3 GroundPos(this Vector3 self)
        {
           
            pos = self + Vector3.up * 50;

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 500, 1 << LayerNames.GetLayerInt(LayerNames.MAP)))
            {
                return hit.point;
            }
            return self;
        }
        public static Vector3 GroundPos(this Vector3Int self)
        {

            pos = self + Vector3.up * 50;

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 500, 1 << LayerNames.GetLayerInt(LayerNames.MAP)))
            {
                return hit.point;
            }
            return self;
        }
        /// <summary>
        /// 鑾峰彇 鍦ㄥ湴闈笂鐨勯珮搴?
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static float GroundPos(this Vector3 self,float height=0)
        {
            pos = self + Vector3.up * 50;

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 500, 1 << LayerNames.GetLayerInt(LayerNames.MAP)))
            {
                return hit.point.y+ height;
            }
            return 0;
        }  
     
        /// <summary>
        /// 鑾峰彇 鐗╁搧 鍦ㄥ湴闈笂鐨勪綅缃?
        /// </summary>
        /// <param name="self">鐗╁搧鑷韩鐨勫潗鏍囦綅缃?/param>
        /// <param name="height">鐩稿鐨勫亸绉婚珮搴?/param>
        /// <returns></returns>
        public static Vector3 GroundVector3Pos(this Vector3 self, float height = 1)
        {
            pos = self + Vector3.up * 50;

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 500, 1 << LayerNames.GetLayerInt(LayerNames.MAP)))
            {
                return hit.point+Vector3.up*height;
            }
            return self + Vector3.up * height;
        }
    }
}
