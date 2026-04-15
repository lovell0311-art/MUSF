using ETModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ETHotfix
{
    /// <summary>
    ///整理背包
    /// </summary>
    public partial class UIKnapsackComponent
    {
        KnapsackGrid[][] FinishGrids = null;
        bool IsCanFinish = true;
        List<ItemPositionInBackpack> itemPositionInBackpacks = null;
       // List<KeyValuePair<long, KnapsackDataItem>> LineList = null;

        List<KnapsackDataItem> knapdataList = null;

        List<KnapsackDataItem> MedicineList = null;
        
 

        //背包中的物品排序
        public void KnapsackItemSort()
        {
           // lock (thisLock)
            {

                //利用链表的Sort方法进行 排序
                //il2cpp 不支持
                /*   LineList.Sort(delegate (KeyValuePair<long, KnapsackDataItem> item_1, KeyValuePair<long, KnapsackDataItem> item_2)
                   {
                       Log.DebugWhtie($"{item_1.Value.ItemType} : {item_2.Value.ItemType}");
                       //item_1 与 item_2 比较 大于0 说明item_1 大于 item_2 ，等于 0 说明是 由小到大的顺序

                       //排序 这里决定你的排序顺序是从大大小还是从小到大，这里的排列顺序是从小到大。如果想改变排序方式，直接对换s1和s2就行了。

                       if (item_1.Value.ConfigId.CompareTo(item_2.Value.ConfigId) > 0)//按装备类型 由小到大排序
                       {
                           return 1;
                       }
                       else
                       {
                            return -1;

                       }
                   });*/


                /*
                 //冒泡排序
                 KnapsackDataItem temp;
                 for (int i = 0, length = knapdataList.Count-1; i < length; i++)
                 {
                     if (knapdataList[i].ConfigId > knapdataList[i + 1].ConfigId)
                     {
                         temp = knapdataList[i];
                         knapdataList[i] = knapdataList[i + 1];
                         knapdataList[i + 1] = temp;
                     }
                 }*/
                knapdataList.Sort((item_1, item_2) =>
                {
                    return item_1.ConfigId.CompareTo(item_2.ConfigId);
                    /* if (item_1.ConfigId.CompareTo(item_2.ConfigId) > 0)//按装备类型 由小到大排序
                     {
                         return 1;
                     }
                     else
                     {
                         return -1;

                     }*/
                });

                itemPositionInBackpacks.Clear();
                // for (int i = 0, length = LineList.Count; i < length; i++)
                for (int i = 0, length = knapdataList.Count; i < length; i++)
                {
                    var item = knapdataList[i];

                    if (KnapsackItemsManager.MedicineHpIdList.Contains(item.ConfigId) || KnapsackItemsManager.MedicineMpIdList.Contains(item.ConfigId))
                    {
                        MedicineList.Add(item);
                        continue;
                    }

                    item.ConfigId.GetItemInfo_Out(out item.item_Info);
                    var pos = GetAutoIndex(item);
                    if (pos == null)
                    {
                        item.tempPosInBackpackX = item.PosInBackpackX;
                        item.tempPosInBackpackY = item.PosInBackpackY;

                        IsCanFinish = false;
                        break;
                    }
                    else
                    {

                        itemPositionInBackpacks.Add(new ItemPositionInBackpack
                        {
                            ItemUID = item.Id,
                            PosInBackpackX = pos.Value.x,
                            PosInBackpackY = pos.Value.y
                        });

                        item.tempPosInBackpackX = item.PosInBackpackX;
                        item.tempPosInBackpackY = item.PosInBackpackY;

                        item.PosInBackpackX = pos.Value.x;
                        item.PosInBackpackY = pos.Value.y;
                        AddEquip(item);

                    }
                }
                //添加药瓶
                for (int i = 0, length = MedicineList.Count; i < length; i++)
                {
                    var item = MedicineList[i];
                    item.ConfigId.GetItemInfo_Out(out item.item_Info);
                    var pos = GetAutoIndex(item);
                    if (pos == null)
                    {

                        item.tempPosInBackpackX = item.PosInBackpackX;
                        item.tempPosInBackpackY = item.PosInBackpackY;

                        IsCanFinish = false;
                        break;
                    }
                    else
                    {

                        itemPositionInBackpacks.Add(new ItemPositionInBackpack
                        {
                            ItemUID = item.Id,
                            PosInBackpackX = pos.Value.x,
                            PosInBackpackY = pos.Value.y
                        });

                        item.tempPosInBackpackX = item.PosInBackpackX;
                        item.tempPosInBackpackY = item.PosInBackpackY;

                        item.PosInBackpackX = pos.Value.x;
                        item.PosInBackpackY = pos.Value.y;
                        AddEquip(item);

                    }
                }
            }
        }
        //添加到临时空间
        public void AddEquip(KnapsackDataItem dataItem)
        {
            var Point1 = new Vector2Int(dataItem.PosInBackpackX, dataItem.PosInBackpackY);
            var Point2 = new Vector2Int(dataItem.PosInBackpackX + dataItem.X - 1, dataItem.PosInBackpackY + dataItem.Y - 1);
            bool IsSinglePoint = Point1 == Point2;
            if (IsSinglePoint)
            {
                FinishGrids[Point1.x][Point1.y].isOccupy = true;
            }
            else
            {
             //   List<KnapsackGrid> gris = KnapsackTools.GetAreaGrids(Point1.x, Point1.y, Point2.x, Point2.y, FinishGrids);
                List<Vector2> gris = KnapsackTools.GetAreaGrids(Point1.x, Point1.y, Point2.x, Point2.y, FinishGrids);

                for (int i = 0, length = gris.Count; i < length; i++)
                {
                    //  gris[i].isOccupy = true;
                    FinishGrids[(int)gris[i].x][(int)gris[i].y].isOccupy = true;
                }

            }
        }
        //获取临时空间的位置
        public Vector2Int? GetAutoIndex(KnapsackDataItem dataItem)
        {
            var Point1 = Vector2.zero;
            var Point2 = new Vector2Int(dataItem.X - 1, dataItem.Y - 1);
            bool IsSinglePoint = Point1 == Point2;

            //遍历加入，是否可以加入
            for (int j = 0; j < LENGTH_Knapsack_Y; j++)
            {

                for (int i = 0; i < LENGTH_Knapsack_X; i++)
                {

                    //单个物体
                    if (IsSinglePoint)
                    {
                        if (FinishGrids[i][j].isOccupy == false)
                        {
                            return new Vector2Int(i, j);
                        }
                    }
                    //范围性物体
                    else
                    {
                        //如果超出边缘直接过滤
                        if (!KnapsackTools.ContainGridObj(i, j, i + Point2.x, j + Point2.y, FinishGrids))
                        {
                            return new Vector2Int(i, j);
                        }
                    }

                }

            }
            return null;
        }

        public async void FinishingBackpack()
        {
            if (curKnapsackState == E_KnapsackState.KS_Trade)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"交易状态 无法整理");
                return;
            }

            if (Time.time < KnapsackItemsManager.PackBackpackTime)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请稍后再整理！");
              //  Log.DebugGreen($"请稍后再整理");
                return;
            }
            KnapsackItemsManager.IsPackBackpack = true;
            KnapsackItemsManager.PackBackpackTime=Time.time+KnapsackItemsManager.PackBackpackSpaceTime;

            InitFinishGrids();
            
            KnapsackItemSort();

            if (IsCanFinish == false)
            {
                KnapsackItemsManager.IsPackBackpack = false;
                for (int i = 0, length = knapdataList.Count; i < length; i++)
                {
                    knapdataList[i].PosInBackpackX = knapdataList[i].tempPosInBackpackX;
                    knapdataList[i].PosInBackpackY = knapdataList[i].tempPosInBackpackY;
                }
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请先预留两行格子 再整理！");
                return;
            }
            else
            {
             await  origanizeBackpack();
             KnapsackItemsManager.IsPackBackpack = false;
            }

            async ETTask origanizeBackpack()
            {
                G2C_OrganizeBackpack g2C_Organize = (G2C_OrganizeBackpack)await SessionComponent.Instance.Session.Call(new C2G_OrganizeBackpack
                {
                    ItemsNewPosition = new Google.Protobuf.Collections.RepeatedField<ItemPositionInBackpack> { itemPositionInBackpacks }
                });
               
                if (g2C_Organize.Error != 0)
                {
                   // Log.DebugBrown($"{g2C_Organize.Error}->");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Organize.Error.GetTipInfo());
                    
                }
                else
                {
                    //改变物品位置
                    GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Knapsack);
                    //清理背包
                    for (int i = 0; i < LENGTH_Knapsack_Y; i++)
                    {
                        for (int j = 0; j < LENGTH_Knapsack_X; j++)
                        {

                            KnapsackGrid grid = grids[j][i];
                            if (grid.IsOccupy)
                            {
                                RemoveItem(grid.Data, true);
                                KnapsackTools.RemoveEquip(grid.Data);
                            }
                        }

                    }
                    //重新加入物品
                   // for (int i = 0, length = LineList.Count; i < length; i++)
                    for (int i = 0, length = knapdataList.Count; i < length; i++)
                    {
                        var item = knapdataList[i];
                        AddItem(item, type: E_Grid_Type.Knapsack);
                        //改变物品的 位置信息
                        if (KnapsackItemsManager.KnapsackItems.TryGetValue(item.Id, out KnapsackDataItem dataItem))
                        {
                           
                            dataItem.PosInBackpackX = item.PosInBackpackX;
                            dataItem.PosInBackpackY = item.PosInBackpackY;
                            KnapsackTools.AddEquip(dataItem);
                        }
                    }
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "整理完成！");
                   //RoleOnHookComponent.Instance.AutoPickUpItem();
                }
            }
        }
        private static readonly System.Object InitFinishLock=new System.Object();
        //初始化格子
        private void InitFinishGrids()
        {
            //lock (InitFinishLock)
            {
                IsCanFinish = true;
                FinishGrids = new KnapsackGrid[LENGTH_Knapsack_X][];
                itemPositionInBackpacks = new List<ItemPositionInBackpack>();
                MedicineList = new List<KnapsackDataItem>();
                //字典转换为 一个链表 因为字典没法直接进行排序操作
                //  LineList = new List<KeyValuePair<long, KnapsackDataItem>>(KnapsackItemsManager.KnapsackItems);
                knapdataList = KnapsackItemsManager.KnapsackItems.Values.ToList();

                for (int i = 0; i < FinishGrids.Length; i++)
                {
                    FinishGrids[i] = new KnapsackGrid[LENGTH_Knapsack_Y];
                }
                for (int j = 0; j < LENGTH_Knapsack_Y; j++)
                {
                    for (int i = 0; i < LENGTH_Knapsack_X; i++)
                    {
                        FinishGrids[i][j] = new KnapsackGrid { isOccupy = false };
                    }
                }
            }
        }
        public void ClearFinishGrids()
        {
            FinishGrids = null;
            itemPositionInBackpacks?.Clear();
            itemPositionInBackpacks = null;
            // LineList?.Clear();
            //LineList = null;
            knapdataList?.Clear();
            knapdataList = null;
            MedicineList?.Clear();
            MedicineList = null;
        }

    }
}