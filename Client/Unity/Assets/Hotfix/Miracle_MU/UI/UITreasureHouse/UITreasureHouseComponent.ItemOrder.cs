using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace ETHotfix
{
    public partial class UITreasureHouseComponent
    {
        public ReferenceCollector itemCollector;
        public Transform OneGradeItem;//一级标题
        public Transform TwoGradeItem;//二级标题
        public Transform ItemOrderContent;//列表栏
        public Transform ItemPools;//对象保存的父对象
        public Transform selectIndexOne = null;//当前选择的一级标题
        public Transform selectIndexTwo = null;//当前选择的二级标题
        public Dictionary<int, Queue<Transform>> poolItemDic = new Dictionary<int, Queue<Transform>>();//对象池，存储和读取
        public Dictionary<int, List<Transform>> saveItemDic = new Dictionary<int, List<Transform>>();//保存的临时二级标题
        public ItemSort itemOne;
        public Transform itemOrderOne;
        public int LastLickId = 0;//避免重复申请同一个
        public void ItemOrder()
        {
            itemCollector = collector.GetImage("LeftPanel").gameObject.GetReferenceCollector();
            OneGradeItem = itemCollector.GetGameObject("OneGradeItem").transform;
            TwoGradeItem = itemCollector.GetGameObject("TwoGradeItem").transform;
            ItemPools = itemCollector.GetGameObject("ItemPools").transform;
            ItemOrderContent = itemCollector.GetGameObject("Content").transform;
            InitTreasureHouseData();
            InitItemOneOrder();
        }
        /// <summary>
        /// 初始化物品信息
        /// </summary>
        public void InitTreasureHouseData()
        {
            if (UITreasureHouseData.itemOneList.Count == 0)
            {
                for (int i = 1; i < 8; i++)
                {
                    ItemSort itemSort =  new ItemSort()
                    {
                        Id = i,
                        Name = ItemType.GetItenOneType(i),
                        Order = 1,
                        ParentId = 0
                    };
                    UITreasureHouseData.itemOneList.Add(itemSort);
                }
            }
            if (UITreasureHouseData.itemTwoList.Count == 0)
            {
                for (int i = 1; i < 26; i++)
                {
                    if (i == 22) continue;
                    ItemSort itemSort;
                    if (i <= 15)
                    {
                        itemSort = new ItemSort()
                        {
                            Id = i,
                            Name = ItemType.GetItenTwoType(i),
                            Order = 2,
                            ParentId = 1
                        };
                    }
                    else
                    {
                        itemSort = new ItemSort()
                        {
                            Id = i,
                            Name = ItemType.GetItenTwoType(i),
                            Order = 2,
                            ParentId = 2
                        };
                    }
                    UITreasureHouseData.itemTwoList.Add(itemSort);
                }
                ItemSort itemSort1 = new ItemSort()
                {
                    Id = 36,
                    Name = ItemType.GetItenTwoType(36),
                    Order = 2,
                    ParentId = 2
                };
                UITreasureHouseData.itemTwoList.Add(itemSort1);
            }
        }
        /// <summary>
        /// 初始化一级标题
        /// </summary>
        public void InitItemOneOrder()
        {
            foreach (var item in UITreasureHouseData.itemOneList)
            {
                Transform itemOrder = GetItemByInde(item.Order);
                itemOrder.gameObject.SetActive(true);
                Button button = itemOrder.GetComponentInChildren<Button>();
                button.onClick.AddSingleListener(() =>
                {
                    ButtonOneClick(item, itemOrder);
                });
                itemOrder.Find("OneGrade/Text").GetComponent<Text>().text = item.Name;
                if (selectIndexOne == null)
                {
                    itemOne = item;
                    itemOrderOne = itemOrder;
                    ButtonOneClick(item, itemOrder);
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(ItemOrderContent.GetComponent<RectTransform>());
        }
        /// <summary>
        /// 点击一级标题
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemOrder"></param>
        public void ButtonOneClick(ItemSort item,Transform itemOrder)
        {
            RecoveryItem(2); 
            if (selectIndexOne != null)
            {
                selectIndexOne.Find("OneGrade/Background/Select").gameObject.SetActive(false);
            }
            if (selectIndexOne != null && selectIndexOne.Find("OneGrade/Text").GetComponent<Text>().text == item.Name)
            {
                selectIndexOne = null;
                LayoutRebuilder.ForceRebuildLayoutImmediate(itemOrder.GetComponent<RectTransform>());
                return;
            }
            selectIndexOne = itemOrder;
            //设置当前物体高亮
            itemOrder.Find("OneGrade/Background/Select").gameObject.SetActive(true);

            int count = 0;
            foreach (var itemsortList in UITreasureHouseData.itemTwoList)
            {
                if(item.Id == itemsortList.ParentId)
                {
                    count++;
                    Transform itemTwoOrder = GetItemByInde(itemsortList.Order);
                    itemTwoOrder.parent = itemOrder;
                    itemTwoOrder.gameObject.SetActive(true);
                    Button button = itemTwoOrder.GetComponentInChildren<Button>();
                    itemTwoOrder.Find("OneGrade/Text").GetComponent<Text>().text = itemsortList.Name;
                    button.onClick.AddSingleListener(() =>
                    {
                        ButtonTwoClick(item,itemsortList, itemTwoOrder);
                    });
                    if (!saveItemDic.ContainsKey(2))
                    {
                        saveItemDic[2] = new List<Transform>();
                    }
                    saveItemDic[2].Add(itemTwoOrder);
                    if (selectIndexTwo == null)
                    {
                        ButtonTwoClick(item,itemsortList, itemTwoOrder);
                    }
                }
            }
            if(count == 0)//没有子物体的一级标题
            {
                if(LastLickId != item.Id)
                    GetItemInfo(item,null);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(ItemOrderContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(selectIndexOne.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemOrder.GetComponent<RectTransform>());

        }
        /// <summary>
        /// 点击第二标题
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemOrder"></param>
        public void ButtonTwoClick(ItemSort item, ItemSort itemsortList, Transform itemOrder)
        {   
            //消除上一个点击二级标题的物体高亮
            if (selectIndexTwo != null)
            {
                selectIndexTwo.Find("OneGrade/Background/Select").gameObject.SetActive(false);
            }
            selectIndexTwo = itemOrder;
            itemOrder.Find("OneGrade/Background/Select").gameObject.SetActive(true);
            //服务器请求数据
            if (LastLickId != itemsortList.Id)
                GetItemInfo(item,itemsortList);
        }
        //服务器请求数据
        public void GetItemInfo(ItemSort item1, ItemSort item2)
        {
            filtrateData.Page = item1.Id;
            if (item2 != null)
            {
                //Log.DebugGreen($"请求的类别 => {item2.Name}");
                LastLickId = item2.Id;
                OpenTreasureHouse(item1.Id, item2.Id).Coroutine();
            }
            else
            {
                //Log.DebugGreen($"请求的类别 => {item1.Name}");
                LastLickId = item1.Id;
                OpenTreasureHouse(item1.Id, 0).Coroutine();
            }
            SetInitFiltrate(filtrateData);//默认筛选
        }


        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="index"></param>
        public void RecoveryItem(int index = 0)
        {
            //回收二级标题
            if (saveItemDic.ContainsKey(2))
            {
                List<Transform> listTran = saveItemDic[2];
                if (listTran != null && listTran.Count > 0)
                {
                    for (int i = 0; i < listTran.Count; i++)
                    {
                        Transform t = listTran[i];
                        RecoveryPool(t, 2);
                    }
                }
            }
            //重置二级标题选中显示
            if (selectIndexTwo != null)
            {
                Transform tempTran = selectIndexTwo.Find("OneGrade/Background/Select");
                tempTran.gameObject.SetActive(false);
                selectIndexTwo = null;
            }
        }
        /// <summary>
        /// 从对象池里取出条目
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Transform GetItemByInde(int index)
        {
            if (!poolItemDic.ContainsKey(index))
            {
                poolItemDic[index] = new Queue<Transform>();
            }
            if (poolItemDic[index].Count > 0)
                return poolItemDic[index].Dequeue();
            Transform tempItem = null;
            if (index == 1)
            {
                tempItem = GameObject.Instantiate<Transform>(OneGradeItem);
                tempItem.name = OneGradeItem.name;
            }
            else if (index == 2)
            {
                tempItem = GameObject.Instantiate<Transform>(TwoGradeItem);
                tempItem.name = TwoGradeItem.name;
            }
            tempItem.parent = ItemOrderContent;
            tempItem.localScale = Vector3.one;
            tempItem.localPosition = new Vector3(tempItem.localPosition.x, tempItem.localPosition.y, 0);
            return tempItem;

        }
        /// <summary>
        /// 放入对象池
        /// </summary>
        /// <param name="tempItem"></param>
        /// <param name="index"></param>
        public void RecoveryPool(Transform tempItem, int index)
        {
            if (!poolItemDic.ContainsKey(index))
            {
                poolItemDic[index] = new Queue<Transform>();
            }
            //GameObject.Destroy(tempItem);
            tempItem.parent = ItemPools;
            tempItem.gameObject.SetActive(false);
            if (!poolItemDic[index].Contains(tempItem))
            {
                poolItemDic[index].Enqueue(tempItem);
            }
        }

    }
}
