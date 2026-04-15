using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{

    public static class MeshSize
    {
        private static readonly Stack<Transform> allTran = new Stack<Transform>();

        public struct Result
        {
            public Vector3 BoundsMin;
            public Vector3 BoundsMax;
            public Vector3 CenterPos;

            /// <summary>
            /// ??????????
            /// </summary>
            /// <param name="rect">??????</param>
            /// <returns></returns>
            public float GetWorldScaleFactor(Vector2 rect)
            {
                float width = BoundsMax.x - BoundsMin.x;
                float hiegth = BoundsMax.y - BoundsMin.y;
                if (width + hiegth == 0) return 1f;

                return Mathf.Min(rect.x / width, rect.y / hiegth);
            }

            /// <summary>
            /// ??????????
            /// </summary>
            /// <param name="rect">??????</param>
            /// <returns></returns>
            public float GetScreenScaleFactor(Vector2 rect)
            {
                Vector3 min = CameraComponent.Instance.UICamera.WorldToScreenPoint(BoundsMin);
                Vector3 max = CameraComponent.Instance.UICamera.WorldToScreenPoint(BoundsMax);
                float width = max.x - min.x;
                float hiegth = max.y - min.y;
                if (width + hiegth == 0) return 1f;

                return Mathf.Min(rect.x / width, rect.y / hiegth);
            }

            public void DebugDraw(float duration = 0f)
            {
                Debug.DrawLine(BoundsMin, BoundsMax, Color.green, duration);


                Debug.DrawLine(BoundsMin, BoundsMin.SetY(BoundsMax.y), Color.green, duration);
                Debug.DrawLine(BoundsMin, BoundsMin.SetX(BoundsMax.x), Color.green, duration);
                Debug.DrawLine(BoundsMin, BoundsMin.SetZ(BoundsMax.z), Color.green, duration);

                Debug.DrawLine(BoundsMax, BoundsMax.SetY(BoundsMin.y), Color.green, duration);
                Debug.DrawLine(BoundsMax, BoundsMax.SetX(BoundsMin.x), Color.green, duration);
                Debug.DrawLine(BoundsMax, BoundsMax.SetZ(BoundsMin.z), Color.green, duration);


                Debug.DrawLine(BoundsMin.SetY(BoundsMax.y), BoundsMin.SetY(BoundsMax.y).SetZ(BoundsMax.z), Color.green, duration);
                Debug.DrawLine(BoundsMin.SetY(BoundsMax.y), BoundsMin.SetY(BoundsMax.y).SetX(BoundsMax.x), Color.green, duration);

                Debug.DrawLine(BoundsMin.SetX(BoundsMax.x), BoundsMin.SetX(BoundsMax.x).SetY(BoundsMax.y), Color.green, duration);
                Debug.DrawLine(BoundsMin.SetX(BoundsMax.x), BoundsMin.SetX(BoundsMax.x).SetZ(BoundsMax.z), Color.green, duration);

                Debug.DrawLine(BoundsMin.SetZ(BoundsMax.z), BoundsMin.SetZ(BoundsMax.z).SetX(BoundsMax.x), Color.green, duration);
                Debug.DrawLine(BoundsMin.SetZ(BoundsMax.z), BoundsMin.SetZ(BoundsMax.z).SetY(BoundsMax.y), Color.green, duration);

                Debug.DrawLine(CenterPos, CenterPos + Vector3.up, Color.green, duration);
                Debug.DrawLine(CenterPos, CenterPos + Vector3.right, Color.red, duration);
                Debug.DrawLine(CenterPos, CenterPos + Vector3.forward, Color.blue, duration);
            }

            public void setMin(Vector3 v)
            {
                if (BoundsMin == Vector3.zero)
                {
                    BoundsMin = v;
                    return;
                }
                if (BoundsMin.x > v.x) BoundsMin.x = v.x;
                if (BoundsMin.y > v.y) BoundsMin.y = v.y;
                if (BoundsMin.z > v.z) BoundsMin.z = v.z;
            }

            public void setMax(Vector3 v)
            {
                if (BoundsMax == Vector3.zero)
                {
                    BoundsMax = v;
                    return;
                }
                if (BoundsMax.x < v.x) BoundsMax.x = v.x;
                if (BoundsMax.y < v.y) BoundsMax.y = v.y;
                if (BoundsMax.z < v.z) BoundsMax.z = v.z;
            }

        }

        



        public static void GetMeshSize(this Transform transform,int layer,ref Result result)
        {
            allTran.Clear();
            allTran.Push(transform);

            result.BoundsMin = Vector3.zero;
            result.BoundsMax = Vector3.zero;


            while (allTran.Count > 0)
            {
                Transform tran = allTran.Pop();
                for (int i = 0; i < tran.childCount; i++)
                {
                    Transform child = tran.GetChild(i);
                    if (child.gameObject.activeSelf == false) continue;
                    allTran.Push(child);
                }
                if (tran.gameObject.layer != layer) continue;

                MeshFilter meshFilter = tran.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Mesh mesh = meshFilter.sharedMesh;
                    if (mesh != null)
                    {
                        EncapsulateBounds(ref result, tran.localToWorldMatrix, mesh.bounds);
                    }
                }
                else
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = tran.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
                    {
                        EncapsulateBounds(ref result, tran.localToWorldMatrix, skinnedMeshRenderer.sharedMesh.bounds);
                    }
                }



            }
            result.CenterPos = ((result.BoundsMax + result.BoundsMin) / 2);
        }

        /// <summary>
        /// ?????????
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static Result GetMeshSize(this Transform transform, int layer)
        {
            Result result = new Result();
            GetMeshSize(transform,layer,ref result);
            return result;
        }

        private static void EncapsulateBounds(ref Result result, Matrix4x4 localToWorld, Bounds bounds)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            AddPoint(ref result, localToWorld.MultiplyPoint(new Vector3(min.x, min.y, min.z)));
            AddPoint(ref result, localToWorld.MultiplyPoint(new Vector3(min.x, min.y, max.z)));
            AddPoint(ref result, localToWorld.MultiplyPoint(new Vector3(min.x, max.y, min.z)));
            AddPoint(ref result, localToWorld.MultiplyPoint(new Vector3(min.x, max.y, max.z)));
            AddPoint(ref result, localToWorld.MultiplyPoint(new Vector3(max.x, min.y, min.z)));
            AddPoint(ref result, localToWorld.MultiplyPoint(new Vector3(max.x, min.y, max.z)));
            AddPoint(ref result, localToWorld.MultiplyPoint(new Vector3(max.x, max.y, min.z)));
            AddPoint(ref result, localToWorld.MultiplyPoint(new Vector3(max.x, max.y, max.z)));
        }

        private static void AddPoint(ref Result result, Vector3 point)
        {
            result.setMin(point);
            result.setMax(point);
        }
    }


}
