
using UnityEngine;
using ETModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 导航扩展工具
    /// </summary>
    public static class AstartComponentExtend
    {
        private static bool TryGetWalkableNode(this AstarComponent self, int x, int z, out AstarNode node)
        {
            node = self.GetNode(x, z);
            return node != null && node.isWalkable;
        }

        /// <summary>
        /// 获取周围可行走的格子
        /// </summary>
        /// <param name="self"></param>
        /// <param name="lastNode">当前的格子位置</param>
        /// <param name="targetNode">目标点的格子位置</param>
        /// <returns></returns>
        public static AstarNode GetCanMoveAstarNode(this AstarComponent self, AstarNode lastNode, AstarNode targetNode)
        {
            int x = targetNode.x - lastNode.x;
            int y = targetNode.z - lastNode.z;
            try
            {
                AstarNode result = null;
                if (lastNode == null) return result;
                if (x > 0 && y > 0)
                { //第一象限
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x, lastNode.z + 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z - 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z + 1, out result)) return result;
                }
                else if (x == 0 && y > 0)
                {//1,2象限中间
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z + 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z + 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z, out result)) return result;
                }
                else if (x < 0 && y > 0)
                {//第二象限
                    if (self.TryGetWalkableNode(lastNode.x, lastNode.z + 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z + 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z - 1, out result)) return result;
                }
                else if (x < 0 && y == 0)
                {//2,3象限中间
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z + 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z - 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x, lastNode.z + 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x, lastNode.z - 1, out result)) return result;
                }
                else if (x < 0 && y < 0)
                {//第三象限
                    if (self.TryGetWalkableNode(lastNode.x, lastNode.z - 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z - 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z + 1, out result)) return result;
                }
                else if (x == 0 && y < 0)
                {//3,4象限中间
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z - 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z - 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z, out result)) return result;
                }
                else if (x > 0 && y < 0)
                {//第四象限
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x, lastNode.z - 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z + 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x - 1, lastNode.z - 1, out result)) return result;
                }
                else if (x > 0 && y == 0)
                {//4,1象限中间
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z + 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x + 1, lastNode.z - 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x, lastNode.z + 1, out result)) return result;
                    if (self.TryGetWalkableNode(lastNode.x, lastNode.z - 1, out result)) return result;
                }
            }
            catch (System.Exception e)
            {

                Log.DebugRed(e.ToString());
            }
            return null;


        }

        /// <summary>
        /// AstarNode 转为3D坐标
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector3Int AstarNodeToVector3(this AstarNode self)
        {
            return AstarComponent.Instance.GetVectory3(self.x, self.z);
        }
        /// <summary>
        /// Vector3 转为 AstarNode
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static AstarNode Vector3ToAstarNode(this Vector3 self)
        {
            return AstarComponent.Instance.GetNodeVector(self.x, self.z);
        }

     

        /// <summary>
        /// 视野更新
        /// </summary>
        /// <param name="b_StartFindTheWay">上一个格子</param>
        /// <param name="b_TargetFindTheWay">当前所在格子</param>
        /// <param name="b_CombatSource">移动实体</param>
        public static void MoveSendNotice(AstarNode b_StartFindTheWay, AstarNode b_TargetFindTheWay, long entityId)
        {
            List<int> leaveAroundField = null;
            List<int> currentAroundField = null;
            List<int> intoAroundField = null;
            // Log.DebugGreen($"b_StartFindTheWay:{b_StartFindTheWay} b_TargetFindTheWay:{b_TargetFindTheWay}  GetType:{b_CombatSource.GetType()}");
            MapCellFieldComponent mSourceCellField = null;
            MapCellFieldComponent mTargetCellField = null;

            if (b_StartFindTheWay != null)
            {
                mSourceCellField = b_StartFindTheWay.Map.GetMapAreaByAreaPos(b_StartFindTheWay.AreaPosX, b_StartFindTheWay.AreaPosY);
            }
            if (b_TargetFindTheWay != null)
            {
                mTargetCellField = b_TargetFindTheWay.Map.GetMapAreaByAreaPos(b_TargetFindTheWay.AreaPosX, b_TargetFindTheWay.AreaPosY);
            }
            if (mTargetCellField == null)
            {
                return;
            }

            if (b_StartFindTheWay == null)
            {
                // 可能是传送
                leaveAroundField = null;
                currentAroundField = null;
                intoAroundField = mTargetCellField.AroundField;

                mTargetCellField.Add(entityId);
            }
            else if (b_StartFindTheWay.Map.Id != b_TargetFindTheWay.Map.Id)
            {
                // 不是一个地图 可能是传送
                leaveAroundField = mSourceCellField.AroundField;
                currentAroundField = null;
                intoAroundField = mTargetCellField.AroundField;

                if (mSourceCellField != mTargetCellField)
                {

                    mSourceCellField.Remove(entityId);
                    mTargetCellField.Add(entityId);
                }
                else
                {
                    mTargetCellField.Add(entityId);
                }


            }
            else
            {


                var aroundFieldSource = mSourceCellField.AroundField;
                var targetFieldSource = mTargetCellField.AroundField;
                // Log.DebugBrown($"aroundFieldSource:{aroundFieldSource.Count}");
                if (AstarComponent.Instance.GetLocaRoleUUID!=null&& entityId == AstarComponent.Instance.GetLocaRoleUUID.Invoke())
                {
                    leaveAroundField = aroundFieldSource.Except(targetFieldSource).ToList();
                    currentAroundField = aroundFieldSource.Intersect(targetFieldSource).ToList();
                    intoAroundField = targetFieldSource.Except(aroundFieldSource).ToList();
                }

                if (mSourceCellField != mTargetCellField)
                {

                    mSourceCellField.Remove(entityId);
                    mTargetCellField.Add(entityId);
                }
                else
                {
                    mTargetCellField.Add(entityId);
                }


            }

            if (leaveAroundField != null && leaveAroundField.Count > 0)
            {
               if (AstarComponent.Instance.GetLocaRoleUUID != null && entityId == AstarComponent.Instance.GetLocaRoleUUID.Invoke())
                {

                    if (leaveAroundField != null && leaveAroundField.Count > 0)
                    {
                       // Log.DebugRed($"玩家==离开区域：{JsonHelper.ToJson(leaveAroundField)}  ");
                        // 告诉 离开了 
                        if (mSourceCellField != null)
                        {
                            try
                            {
                                for (int i = 0; i < leaveAroundField.Count; i++)
                                {
                                    int mleaveAroundFieldIndex = leaveAroundField[i];
                                    if (AstarComponent.Instance.AreaClear!=null&& mSourceCellField.AroundFieldDic.TryGetValue(mleaveAroundFieldIndex, out var mTemp))
                                    {
                                       AstarComponent.Instance.AreaClear.Invoke(mTemp.AreaIndex);
                                    }
                                }
                            }
                            catch (System.Exception e)
                            {

                                Log.DebugRed($"{e}");
                            }
                            //Log.DebugBrown($"离开区域：{JsonHelper.ToJson(leaveAroundField)}");
                        }
                    }
                }
            }

        }
    }
}
