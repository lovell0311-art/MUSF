using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 背包工具类
    /// </summary>
    public partial class UIKnapsackComponent
    {


        /// <summary>
        ///  初始化格子数量
        /// </summary>
        /// <param name="x">列</param>
        /// <param name="y">行</param>
        /// <param name="content">格子父对象</param>
        /// <param name="grid_Type">格子类型</param>
        /// <param name="RegisterAction">格子事件函数</param>
        public void CreatGrid(int x, int y, Transform content, E_Grid_Type grid_Type, ref KnapsackGrid[][] Grids)
        {
            RectTransform temp = content.GetChild(0).GetComponent<RectTransform>();
            temp.gameObject.SetActive(false);
            for (int i = 0; i < y; i++)//行数
            {
                for (int j = 0; j < x; j++)//列数
                {
                    RectTransform grid = GameObject.Instantiate<RectTransform>(temp, content);
                     grid.anchoredPosition = new Vector2(31 + (j * 66), -31 - (i * 66));
                    grid.name = $"{j}_{i}";//命名为：行_列

                    grid.gameObject.SetActive(true);
                    if (grid.Find("lock") != null)
                    {
                        grid.Find("lock").gameObject.SetActive(false);
                    }

                    Grids[j][i] = new KnapsackGrid
                    {
                        GridObj = null,
                        Image = grid.GetComponent<Image>(),
                        IsOccupy = false,
                        Grid_Type = grid_Type

                    };
                    //注册 拖拽、点击、、事件
                    RegisterEvent(j, i, grid.gameObject, grid_Type);
                }
            }
        }
        /// <summary>
        /// 得到当前面板的格子数组
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="_Grid_Type">当前格子 所属类型</param>
        private void GetKnapsackGrid(ref KnapsackGrid[][] grid, ref int x, ref int y, E_Grid_Type _Grid_Type)
        {
            switch (_Grid_Type)
            {
                case E_Grid_Type.Knapsack:
                    grid = BackGrids;
                    x = LENGTH_Knapsack_X;
                    y = LENGTH_Knapsack_Y;
                    break;
                case E_Grid_Type.Gem_Merge:
                    grid = MergerGrids;
                    x = LENGTH_Merger_X;
                    y =LENGTH_Merger_Y;
                    break;
                case E_Grid_Type.Ware_House:
                    grid = WareHouseGrids;
                    x = LENGTH_WareHouse_X;
                    y = LENGTH_WareHouse_Y;
                    break;
                case E_Grid_Type.Shop:
                    grid = NpcShopGrids;
                    x = LENGTH_NpcShop_X;
                    y = LENGTH_NpcShop_Y;
                    break;
                case E_Grid_Type.Stallup:
                    grid = StallUpGrids;
                    x = LENGTH_StallUp_X;
                    y = LENGTH_StallUp_Y;
                    break;
                case E_Grid_Type.Stallup_OtherPlayer:
                    grid = StallUp_OtherGrids;
                    x = LENGTH_StallUp_Other_X;
                    y = LENGTH_StallUp_Other_Y;
                    break;
                case E_Grid_Type.GiveCoin:
                    break;
                case E_Grid_Type.GiveGoods:
                    break;
                case E_Grid_Type.Consignment:
                    break;
                case E_Grid_Type.Reduction:
                    break;
                case E_Grid_Type.Trade:
                    grid = MyGrids;
                    x = LENGTH_Trade_X;
                    y = LENGTH_Trade_Y;
                    break;
                case E_Grid_Type.Trade_Other:
                    grid = OtherGrids;
                    x = LENGTH_Trade_X;
                    y = LENGTH_Trade_Y;
                    break;

                default:
                    break;
            }
        }
        /// <summary>
        /// 物品回到起始位置
        /// </summary>
        public void ResetGridObj()
        {
            if (curDropObj == null) return;
            curDropObj.transform.SetPositionAndRotation(originObjPos, originObjRotation);
            isDroping = false;
            curDropObj = null;
        }
        private Vector2Int GetCenterGrid()
        {
            Vector2Int offset = this.originArea.Point1 - this.originArea.Point2;
            offset = new Vector2Int(((int)(offset.x / 2f)), ((int)(offset.y / 2f)));
            return offset;
        }
        /// <summary>
        /// 判断格子是否已经有物品
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public bool ContainGridObj(int startX, int startY, int endX, int endY)
        {
            List<KnapsackGrid> results = GetAreaGrids(startX, startY, endX, endY, LENGTH_X, LENGTH_Y);
            foreach (var item in results)
            {
                if (item.IsOccupy && this.originArea.UUID != item.Data.UUID) return true;
            }
            return false;
        }
        public bool ContainCacheGridObj(int startX, int startY, int endX, int endY)
        {
            List<KnapsackGrid> results = GetAreaGrids(startX, startY, endX, endY, LENGTH_X, LENGTH_Y);
            foreach (var item in results)
            {
                if (item.IsOccupy) return true;
            }
            return false;
        }
        public Vector3 GetCenterPos(int startX, int startY, int endX, int endY)
        {
            try
            {
                Transform start = this.grids[startX][startY].Image.transform;
                Transform end = this.grids[endX][endY].Image.transform;
               

                Vector3 pos = end.position - start.position;
                float halfDistance = (Vector3.Distance(start.position, end.position) / 2);
                Vector3 result = start.position + (pos.normalized * halfDistance);
                result.z = 85;
              //  Log.DebugYellow($"start:{start.name} {start.position} end:{end.name} {end.position} result:{result}");
                return result;
            }
            catch (Exception e)
            {
                Log.Error($"startX:{startX}  startY:{startY}  endX:{endX}  endy:{endY}  \n {e}");
            }
            return Vector3.zero;
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startX">起始——x</param>
        /// <param name="startY">起始-y</param>
        /// <param name="endX">结束-x</param>
        /// <param name="endY">结束-y</param>
        /// <param name="count_x">列数</param>
        /// <param name="count_y">行数</param>
        /// <returns></returns>
        private List<KnapsackGrid> GetAreaGrids(int startX, int startY, int endX, int endY, int count_x, int count_y)
        {
            List<KnapsackGrid> results = new List<KnapsackGrid>();
            if (startX >= count_x || endX >= count_x || startY >= count_y || endY >= count_y ||
                startX < 0 || endX < 0 || startY < 0 || endY < 0)
            {
                return results;
            }
            //横向查询
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    try
                    {
                       
                        results.Add(this.grids[i][j]);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"i:{i}  j:{j}  count_x:{count_x} count_y:{count_y} : {e}");
                        return new List<KnapsackGrid>();
                    }
                }
            }

            return results;
        }
        /// <summary>
        /// 克隆出一个转给实体
        /// </summary>
        /// <param name="item"></param>
        /// <param name="knapsackDataItem"></param>
        private static void CloneKnasackItem(KnapsackDataItem item, out KnapsackDataItem knapsackDataItem)
        {
            knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(item.UUID);
            knapsackDataItem.GameUserId = item.GameUserId;
            knapsackDataItem.UUID = item.UUID;
            knapsackDataItem.ConfigId = item.ConfigId;
            knapsackDataItem.ItemType = item.ItemType;
            knapsackDataItem.PosInBackpackX = item.PosInBackpackX;
            knapsackDataItem.PosInBackpackY = item.PosInBackpackY;
            knapsackDataItem.X = item.X;
            knapsackDataItem.Y = item.Y;
            knapsackDataItem.ItemValueDic = item.ItemValueDic;
        }


        /// <summary>
        /// 获取玩家周围可使用点（十格以内）
        /// </summary>
        /// <returns></returns>
        public AstarNode GetNearNode()
        {

            AstarNode astarNode=null;
            for (int i = -5; i < 0; i++)
            {
                for (int j = -5; j <5; j++)
                {

                    if (Mathf.Abs(i) <= 2 && Mathf.Abs(j) <= 2) continue;
                    var nearNode = this.roleEntity.CurrentNodePos;
                    //AstarNode node = AstarComponent.Instance.GetNodeVector(vector.x, vector.z);
                    AstarNode node = AstarComponent.Instance.GetNode(nearNode.x + i, nearNode.z + j);
                    if (node.isWalkable)
                    {
                        //判断该点是否有实体

                        if (IsNull(node)==false)
                        {
                            continue;
                        }
                       
                        return node;
                    }
                }
            }
            return astarNode;

            bool IsNull(AstarNode node) 
            {
                List<KnapsackItemEntity> allentity = UnitEntityComponent.Instance.KnapsackItemEntityDic.Values.ToList();

                for (int k = 0; k < allentity.Count; k++)
                {
                    var item = allentity[k];
                    if (node.Compare(item.CurrentNodePos))
                    {
                        astarNode ??= node;
                        //当前格子有装备
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
