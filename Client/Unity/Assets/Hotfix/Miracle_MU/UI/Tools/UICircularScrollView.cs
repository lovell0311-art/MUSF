using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETModel;
namespace ETHotfix
{
    public enum E_Direction
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// UGUI cell循环复用组件
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class UICircularScrollView<T> : Component
    {

        //数据来源
        private List<T> items;
        public List<T> Items
        {
            get { return items; }
            set
            {
                items = value;
                InitItemCell();
            }
        }


        public E_Direction m_Direction = E_Direction.Horizontal;
        public bool m_IsShowArrow = true;

        public int m_Row = 1;

        public float Spacing_x = 0f; //水平间距
        public float Spacing_y = 0f; //垂直间距
        public bool IsPool = true;
        public GameObject m_CellGameObject; //指定的cell
        public GameObject m_CellPrefab;
        //item初始化信息回调函数
        public Action<GameObject, T> ItemInfoCallBack;
        //item 点击回调函数
        public Action<GameObject, T> ItemClickCallBack;
        //开始拖拽 回调函数
        public Action<GameObject, T> ItemBeginDragEventBack;
        //拖拽中
        public Action<GameObject, T> ItemDragEventBack;
        //拖拽结束
        public Action<GameObject, T> ItemEndDragEventBack;


        protected RectTransform rectTrans;

        protected float m_PlaneWidth;
        protected float m_PlaneHeight;

        protected float m_ContentWidth;
        protected float m_ContentHeight;

        protected float m_CellObjectWidth;
        protected float m_CellObjectHeight;

        protected GameObject m_Content;
        protected RectTransform m_ContentRectTrans;

        private bool m_isInited = false;

        //记录 物体的坐标 和 物体 
        protected struct CellInfo
        {
            public Vector3 pos;
            public GameObject obj;
        };
        protected CellInfo[] m_CellInfos;

        protected bool m_IsInited = false;

        protected ScrollRect m_ScrollRect;

        protected int m_MaxCount = -1; //列表数量

        protected int m_MinIndex = -1;
        protected int m_MaxIndex = -1;

        protected bool m_IsClearList = false; //是否清空列表

        /// <summary>
        /// 初始化格子信息
        /// </summary>
        /// <param name="direction">滑动方向</param>
        /// <param name="count">行、列 的数量</param>
        /// <param name="space_x">cell之间X间距</param>
        /// <param name="space_y">cell之间y间距</param>
        public void InitInfo(E_Direction direction, int count, int space_x, int space_y)
        {
            m_Direction = direction;
            m_Row = count;
            Spacing_x = space_x;
            Spacing_y = space_y;
        }
        /// <summary>
        /// 初始化 Conten ScrollRect
        /// </summary>
        /// <param name="conten">格子的父对象</param>
        /// <param name="scrollRect">ScrollRect 组件</param>
        public void IninContent(GameObject conten, ScrollRect scrollRect)
        {
            m_Content = conten;
            rectTrans = scrollRect.GetComponent<RectTransform>();
            m_ScrollRect = scrollRect;
            m_isInited = false;
            Init();
        }
        public void Init()
        {

            if (m_isInited)
                return;

            if (m_CellGameObject == null)
            {
                m_CellGameObject = m_Content.transform.GetChild(0).gameObject;
            }
            /* Cell 处理 */
            SetPoolsObj(m_CellGameObject);

            RectTransform cellRectTrans = m_CellGameObject.GetComponent<RectTransform>();
            cellRectTrans.pivot = new Vector2(0f, 1f);
            CheckAnchor(cellRectTrans);
            cellRectTrans.anchoredPosition = Vector2.zero;

            //记录 Cell 信息
            m_CellObjectHeight = cellRectTrans.rect.height;
            m_CellObjectWidth = cellRectTrans.rect.width;

            //记录 Plane 信息
            Rect planeRect = rectTrans.rect;
            m_PlaneHeight = planeRect.height;
            m_PlaneWidth = planeRect.width;

            //记录 Content 信息
            m_ContentRectTrans = m_Content.GetComponent<RectTransform>();
            Rect contentRect = m_ContentRectTrans.rect;
            m_ContentHeight = contentRect.height;
            m_ContentWidth = contentRect.width;
            m_ContentRectTrans.pivot = new Vector2(0f, 1f);
            CheckAnchor(m_ContentRectTrans);
            m_ScrollRect.onValueChanged.RemoveAllListeners();
            //添加滑动事件
            m_ScrollRect.onValueChanged.AddListener(delegate { ScrollRectListener(); });
            m_isInited = true;
        }


        //检查 Anchor 是否正确
        private void CheckAnchor(RectTransform rectTrans)
        {
            if (m_Direction == E_Direction.Vertical)
            {
                if (!((rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(0, 1)) ||
                         (rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(1, 1))))
                {
                    rectTrans.anchorMin = new Vector2(0, 1);
                    rectTrans.anchorMax = new Vector2(1, 1);
                }
            }
            else
            {
                if (!((rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(0, 1)) ||
                         (rectTrans.anchorMin == new Vector2(0, 0) && rectTrans.anchorMax == new Vector2(0, 1))))
                {
                    rectTrans.anchorMin = new Vector2(0, 0);
                    rectTrans.anchorMax = new Vector2(0, 1);
                }
            }
        }

        /// <summary>
        /// 初始化数据格子
        /// </summary>
        public void InitItemCell()
        {
            if (items == null)
            {
                for (int i = 0; i < m_Content.transform.childCount; i++)
                {
                    SetPoolsObj(m_Content.transform.GetChild(i).gameObject);
                }
                return;
            }

            int num = items.Count;
            m_MinIndex = -1;
            m_MaxIndex = -1;

            //-> 计算 Content 尺寸
            if (m_Direction == E_Direction.Vertical)
            {
                float contentSize = (Spacing_y + m_CellObjectHeight) * Mathf.CeilToInt((float)num / m_Row);
                m_ContentHeight = contentSize;
                m_ContentWidth = m_ContentRectTrans.sizeDelta.x;
                contentSize = contentSize < rectTrans.rect.height ? rectTrans.rect.height : contentSize;
                contentSize -= Spacing_y;
                m_ContentRectTrans.sizeDelta = new Vector2(m_ContentWidth, contentSize);
                if (num != m_MaxCount)
                {
                    m_ContentRectTrans.anchoredPosition = new Vector2(m_ContentRectTrans.anchoredPosition.x, 0);
                }
            }
            else
            {
                float contentSize = (Spacing_x + m_CellObjectWidth) * Mathf.CeilToInt((float)num / m_Row);
                m_ContentWidth = contentSize;
                m_ContentHeight = m_ContentRectTrans.sizeDelta.x;
                contentSize = contentSize < rectTrans.rect.width ? rectTrans.rect.width : contentSize;
                contentSize -= Spacing_x;
                m_ContentRectTrans.sizeDelta = new Vector2(contentSize, m_ContentHeight);
                if (num != m_MaxCount)
                {
                    m_ContentRectTrans.anchoredPosition = new Vector2(0, m_ContentRectTrans.anchoredPosition.y);
                }
            }
            //-> 计算 开始索引
            int lastEndIndex = 0;

            //-> 过多的物体 扔到对象池 ( 首次调 ShowList函数时则无效 数据刷新使用 )
            if (m_IsInited)
            {
                lastEndIndex = num - m_MaxCount > 0 ? m_MaxCount : num;
                lastEndIndex = m_IsClearList ? 0 : lastEndIndex;

                int count = m_IsClearList ? m_CellInfos.Length : m_MaxCount;
                for (int i = lastEndIndex; i < count; i++)
                {
                    if (m_CellInfos[i].obj != null)
                    {
                        SetPoolsObj(m_CellInfos[i].obj);
                        m_CellInfos[i].obj = null;
                    }
                }
            }

            //-> 以下四行代码 在for循环所用
            CellInfo[] tempCellInfos = m_CellInfos;
            m_CellInfos = new CellInfo[num];

            //-> 1: 计算 每个Cell坐标并存储 2: 显示范围内的 Cell
            for (int i = 0; i < num; i++)
            {
                int index = i;
                // * -> 存储 已有的数据 ( 首次调 ShowList函数时 则无效 )
                if (m_MaxCount != -1 && i < lastEndIndex)
                {
                    CellInfo tempCellInfo = tempCellInfos[i];
                    //-> 计算是否超出范围
                    float rPos = m_Direction == E_Direction.Vertical ? tempCellInfo.pos.y : tempCellInfo.pos.x;
                    if (!IsOutRange(rPos))
                    {
                        //-> 记录显示范围中的 首位index 和 末尾index
                        m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex; //首位index
                        m_MaxIndex = i; // 末尾index

                        if (tempCellInfo.obj == null)
                        {
                            tempCellInfo.obj = GetPoolsObj();
                        }
                        tempCellInfo.obj.transform.GetComponent<RectTransform>().anchoredPosition = tempCellInfo.pos;
                        tempCellInfo.obj.name = i.ToString();
                        tempCellInfo.obj.SetActive(true);

                        GameObject game = tempCellInfo.obj;
                        T item = items[index];

                        ItemInfoCallBack(game, item);//初始化cell信息
                        if (tempCellInfo.obj.GetComponent<Button>() is Button button)//Button 点击事件
                        {
                            button.onClick.AddSingleListener(delegate { ItemClickCallBack?.Invoke(game, item); });
                        }
                        if (tempCellInfo.obj.GetComponent<UGUITriggerProxy>() is UGUITriggerProxy proxy1)//UGUITriggerProxy 代理组件
                        {
                            proxy1.OnPointerClickEvent = delegate { ItemClickCallBack?.Invoke(game, item); };//点击 回调事件
                            proxy1.OnBeginDragEvent = delegate { ItemBeginDragEventBack?.Invoke(game, item); };// 开始拖拽 
                            proxy1.OnDragEvent = delegate { ItemDragEventBack?.Invoke(game, item); };// 拖拽中 
                            proxy1.OnEndDragEvent = delegate { ItemEndDragEventBack?.Invoke(game, item); };//结束拖拽
                        }


                    }
                    else
                    {
                        SetPoolsObj(tempCellInfo.obj);
                        tempCellInfo.obj = null;
                    }
                    m_CellInfos[i] = tempCellInfo;
                    continue;
                }

                CellInfo cellInfo = new CellInfo();
                float pos;
                float rowPos;
                // * -> 计算每个Cell坐标
                if (m_Direction == E_Direction.Vertical)
                {
                    pos = m_CellObjectHeight * Mathf.FloorToInt(i / m_Row) + Spacing_y * Mathf.FloorToInt(i / m_Row);
                    rowPos = m_CellObjectWidth * (i % m_Row) + Spacing_x * (i % m_Row);
                    cellInfo.pos = new Vector3(rowPos, -pos, 0);
                }
                else
                {
                    pos = m_CellObjectWidth * Mathf.FloorToInt(i / m_Row) + Spacing_x * Mathf.FloorToInt(i / m_Row);
                    rowPos = m_CellObjectHeight * (i % m_Row) + Spacing_y * (i % m_Row);
                    cellInfo.pos = new Vector3(pos, -rowPos, 0);
                }



                //-> 计算是否超出范围
                float cellPos = m_Direction == E_Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                if (IsOutRange(cellPos))
                {
                    cellInfo.obj = null;
                    m_CellInfos[i] = cellInfo;
                    continue;
                }

                //-> 记录显示范围中的 首位index 和 末尾index
                m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex; //首位index
                m_MaxIndex = i; // 末尾index

                //-> 取或创建 Cell
                GameObject cell = GetPoolsObj();
                cell.transform.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
                cell.name = i.ToString();
                //-> 存数据
                cellInfo.obj = cell;
                cellInfo.obj.transform.localPosition = cellInfo.pos;
                m_CellInfos[i] = cellInfo;

                //回调函数
                ItemInfoCallBack?.Invoke(cell, items[index]);
                if (cell.GetComponent<Button>() is Button btn)
                    btn.onClick.AddSingleListener(() => { ItemClickCallBack?.Invoke(cell, items[index]); });

                if (cell.GetComponent<UGUITriggerProxy>() is UGUITriggerProxy proxy)//UGUITriggerProxy 代理组件
                {
                    proxy.OnPointerClickEvent = delegate { ItemClickCallBack?.Invoke(cell, items[index]); };//点击 回调事件
                    proxy.OnBeginDragEvent = delegate { ItemBeginDragEventBack?.Invoke(cell, items[index]); };// 开始拖拽 
                    proxy.OnDragEvent = delegate { ItemDragEventBack?.Invoke(cell, items[index]); };// 拖拽中 
                    proxy.OnEndDragEvent = delegate { ItemEndDragEventBack?.Invoke(cell, items[index]); };//结束拖拽
                }

            }

            m_MaxCount = num;
            m_IsInited = true;


        }


        //滑动事件
        protected void ScrollRectListener()
        {
            if (m_CellInfos == null)
                return;
            //检查超出范围
            for (int i = 0, length = m_CellInfos.Length; i < length; i++)
            {
                CellInfo cellInfo = m_CellInfos[i];
                GameObject obj = cellInfo.obj;
                Vector3 pos = cellInfo.pos;

                float rangePos = m_Direction == E_Direction.Vertical ? pos.y : pos.x;
                //判断是否超出显示范围
                if (IsOutRange(rangePos))
                {
                    //把超出范围的cell 扔进 poolsObj里
                    if (obj != null)
                    {
                        SetPoolsObj(obj);
                        m_CellInfos[i].obj = null;
                    }
                }
                else
                {
                    if (obj == null)
                    {
                        //优先从 poolsObj中 取出 （poolsObj为空则返回 实例化的cell）
                        GameObject cell = GetPoolsObj();
                        cell.transform.localPosition = pos;
                        cell.gameObject.name = i.ToString();
                        m_CellInfos[i].obj = cell;
                        m_CellInfos[i].pos = new Vector3(pos.x, pos.y, 0);

                        T item = items[i];
                        ItemInfoCallBack?.Invoke(cell, item);
                        if (cell.GetComponent<Button>() is Button button)
                            button.onClick.AddSingleListener(() => { ItemClickCallBack?.Invoke(cell, item); });

                        if (cell.GetComponent<UGUITriggerProxy>() is UGUITriggerProxy proxy)//UGUITriggerProxy 代理组件
                        {
                            proxy.OnPointerClickEvent = delegate { ItemClickCallBack?.Invoke(cell, item); };//点击 回调事件
                            proxy.OnBeginDragEvent = delegate { ItemBeginDragEventBack?.Invoke(cell, item); };// 开始拖拽 
                            proxy.OnDragEvent = delegate { ItemDragEventBack?.Invoke(cell, item); };// 拖拽中 
                            proxy.OnEndDragEvent = delegate { ItemEndDragEventBack?.Invoke(cell, item); };//结束拖拽
                        }
                    }
                }
            }
        }


        //判断是否超出显示范围
        private bool IsOutRange(float pos)
        {
            Vector3 listP = m_ContentRectTrans.anchoredPosition;
            if (m_Direction == E_Direction.Vertical)
            {
                if (pos + listP.y > m_CellObjectHeight || pos + listP.y < -rectTrans.rect.height)
                {
                    return true;
                }
            }
            else
            {
                if (pos + listP.x < -m_CellObjectWidth || pos + listP.x > rectTrans.rect.width)
                {
                    return true;
                }
            }
            return false;
        }

        //对象池 机制  (存入， 取出) cell
        private Stack<GameObject> poolsObj = new Stack<GameObject>();
        //取出 cell
        protected virtual GameObject GetPoolsObj()
        {
            GameObject cell = null;


            if (IsPool)
            {
                if (poolsObj.Count > 0)
                {
                    cell = poolsObj.Pop();
                }

                if (cell == null)
                {
                    cell = GameObject.Instantiate<GameObject>(m_CellGameObject);
                }
            }
            else
            {
                cell = GameObject.Instantiate(m_CellPrefab, m_Content.transform);
            }

            cell.transform.SetParent(m_Content.transform);
            cell.transform.localScale = Vector3.one;
            cell.transform.localPosition = Vector3.zero;
            cell.SetActive(true);

            return cell;
        }
        //存入 cell
        protected virtual void SetPoolsObj(GameObject cell)
        {
            if (cell != null)
            {
                if (IsPool)
                {
                    poolsObj.Push(cell);
                    cell.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(cell);
                }
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            poolsObj.Clear();
            items = null;
            ItemInfoCallBack = null;
            ItemClickCallBack = null;

        }

    }
}
