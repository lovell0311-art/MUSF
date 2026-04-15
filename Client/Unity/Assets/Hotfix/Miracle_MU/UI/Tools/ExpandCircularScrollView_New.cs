//*****************************-》 可收展 循环列表 《-****************************
//author: Kim
//初始化:
//       Init(CallBack)   //不需要 回调Cell点击函数时用此 Init()
//       Init(CallBack, OnClickCallBack)  //需要回调 Cell点击函数时用此 Init() 
//刷新列表:
//      ShowList(数量格式: string = "5|5|6")
//回调:
//Func(GameObject = 收展按钮, GameObject = Cell, int = 收展按钮索引 Index, int = 子cell索引) 
//OnClickCell(GameObject = Cell, int = Index)    //点击Cell 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETModel;


namespace ETHotfix
{
    public class UIUtils1
    {
        public static void SetActive(GameObject obj, bool isActive)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }

        }
    }

    public enum e_Direction
    {
        Horizontal,
        Vertical
    }

    public class ExpandCircularScrollView : Component
    {

        ////数据来源
        //private List items;
        //public List Items
        //{
        //    get { return items; }
        //    set
        //    {
        //        items = value;
        //    }
        //}
        public GameObject m_ExpandButton;
        public float m_BackgroundMargin;
        public bool m_IsExpand = false;

        private float m_ExpandButtonX;
        private float m_ExpandButtonY;
        private float m_ExpandButtonWidth;
        private float m_ExpandButtonHeight;

        private Vector2 m_BackgroundOriginSize;

        #region
        public GameObject m_PointingFirstArrow;
        public GameObject m_PointingEndArrow;
        public e_Direction m_Direction = e_Direction.Horizontal;
        public bool m_IsShowArrow = true;

        public int m_Row = 1;

        public float m_Spacing = 0f; //间距
        public GameObject m_CellGameObject; //指定的cell

        private Action<GameObject, GameObject,int, int> m_FuncCallBackFunc;
        protected Action<GameObject, GameObject,int, int> m_FuncOnClickCallBack;
        protected Action<int, bool, GameObject> m_FuncOnButtonClickCallBack;

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
        #endregion

        //展开信息
        struct ExpandInfo
        {
            public GameObject button;
            public bool isExpand; 
            public CellInfo[] cellInfos; 
            public float size; 
            public int cellCount;
        }
        private ExpandInfo[] m_ExpandInfos = null;

        private Dictionary<GameObject, bool> m_IsAddedListener = new Dictionary<GameObject, bool>(); //用于 判断是否重复添加 点击事件

        public void Init(Action<GameObject, GameObject, int, int> callBack)
        {
            Init(callBack, null);
        }

        public void Init(Action<GameObject, GameObject,int, int> callBack, Action<GameObject, GameObject, int, int> onClickCallBack, Action<int,bool, GameObject> onButtonClickCallBack)
        {
            m_FuncOnButtonClickCallBack = onButtonClickCallBack;
            Init(callBack, onClickCallBack);
        }

        public void Init(Action<GameObject, GameObject,int, int> callBack, Action<GameObject, GameObject, int, int> onClickCallBack)
        {
            InitBase();
            m_FuncCallBackFunc = callBack;

            /* Button 处理 */
            if (m_ExpandButton == null)
            {
                if (m_Content.transform.Find("Button") != null)
                {
                    m_ExpandButton = m_Content.transform.Find("Button").gameObject;
                }
            }

            if (m_ExpandButton != null)
            {
                RectTransform rectTrans = m_ExpandButton.transform.GetComponent<RectTransform>();
                m_ExpandButtonX = rectTrans.anchoredPosition.x;
                m_ExpandButtonY = rectTrans.anchoredPosition.y;

                SetPoolsButtonObj(m_ExpandButton);

                m_ExpandButtonWidth = rectTrans.rect.width;
                m_ExpandButtonHeight = rectTrans.rect.height;

                var background = m_ExpandButton.transform.Find("background");
                if (background != null)
                {
                    m_BackgroundOriginSize = background.GetComponent<RectTransform>().sizeDelta;
                }
            }
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
        }
        //记录信息
        public void InitBase()
        {
            DisposeAll();

            if (m_isInited)
                return;


            //m_Content = this.GetComponent<ScrollRect>().content.gameObject;

            if (m_CellGameObject == null)
            {
                m_CellGameObject = m_Content.transform.GetChild(0).gameObject;
            }
            /* Cell 处理 */
            //m_CellGameObject.transform.SetParent(m_Content.transform.parent, false);
            SetPoolsObj(m_CellGameObject);

            RectTransform cellRectTrans = m_CellGameObject.GetComponent<RectTransform>();
            cellRectTrans.pivot = new Vector2(0f, 1f);
            CheckAnchor(cellRectTrans);
            cellRectTrans.anchoredPosition = Vector2.zero;

            //记录 Cell 信息
            m_CellObjectHeight = cellRectTrans.rect.height;
            m_CellObjectWidth = cellRectTrans.rect.width;

            //记录 Plane 信息
            //rectTrans = GetComponent<RectTransform>();
            Rect planeRect = rectTrans.rect;
            m_PlaneHeight = planeRect.height;
            m_PlaneWidth = planeRect.width;

            //记录 Content 信息
            m_ContentRectTrans = m_Content.GetComponent<RectTransform>();
            Rect contentRect = m_ContentRectTrans.rect;
            m_ContentHeight = contentRect.height;
            m_ContentWidth = contentRect.width;

            m_ContentRectTrans.pivot = new Vector2(0f, 1f);
            //m_ContentRectTrans.sizeDelta = new Vector2 (planeRect.width, planeRect.height);
            //m_ContentRectTrans.anchoredPosition = Vector2.zero;
            CheckAnchor(m_ContentRectTrans);

            //m_ScrollRect = this.GetComponent<ScrollRect>();

            m_ScrollRect.onValueChanged.RemoveAllListeners();
            //添加滑动事件
            m_ScrollRect.onValueChanged.AddListener(delegate (Vector2 value) { ScrollRectListener(value); });

            if (m_PointingFirstArrow != null || m_PointingEndArrow != null)
            {
                m_ScrollRect.onValueChanged.AddListener(delegate (Vector2 value) { OnDragListener(value); });
                OnDragListener(Vector2.zero);
            }

            //InitScrollBarGameObject(); // 废弃

            m_isInited = true;
        }
        protected void OnDragListener(Vector2 value)
        {
            float normalizedPos = m_Direction == e_Direction.Vertical ? m_ScrollRect.verticalNormalizedPosition : m_ScrollRect.horizontalNormalizedPosition;

            if (m_Direction == e_Direction.Vertical)
            {
                if (m_ContentHeight - rectTrans.rect.height < 10)
                {
                    UIUtils1.SetActive(m_PointingFirstArrow, false);
                    UIUtils1.SetActive(m_PointingEndArrow, false);
                    return;
                }
            }
            else
            {
                if (m_ContentWidth - rectTrans.rect.width < 10)
                {
                    UIUtils1.SetActive(m_PointingFirstArrow, false);
                    UIUtils1.SetActive(m_PointingEndArrow, false);
                    return;
                }
            }

            if (normalizedPos >= 0.9)
            {
                UIUtils1.SetActive(m_PointingFirstArrow, false);
                UIUtils1.SetActive(m_PointingEndArrow, true);
            }
            else if (normalizedPos <= 0.1)
            {
                UIUtils1.SetActive(m_PointingFirstArrow, true);
                UIUtils1.SetActive(m_PointingEndArrow, false);
            }
            else
            {
                UIUtils1.SetActive(m_PointingFirstArrow, true);
                UIUtils1.SetActive(m_PointingEndArrow, true);
            }

        }
        //检查 Anchor 是否正确
        private void CheckAnchor(RectTransform rectTrans)
        {
            if (m_Direction == e_Direction.Vertical)
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

        //清理所有委托
        public void DisposeAll()
        {
            if (m_FuncCallBackFunc != null)
            {
                m_FuncCallBackFunc = null;
            }
            if (m_FuncOnClickCallBack != null)
            {
                m_FuncOnClickCallBack = null;
            }
        }
        public void ShowList(string numStr)
        {
            ClearCell(); //清除所有Cell (非首次调Showlist时执行)

            int totalCount = 0;

            int beforeCellCount = 0; 

            string[] numArray = numStr.Split('|');
            int buttonCount = numArray.Length;

            bool isReset;
            if(m_IsInited && m_ExpandInfos.Length == buttonCount)
            {
                isReset = false;
            }
            else
            {
                m_ExpandInfos = new ExpandInfo[buttonCount];
                isReset = true;
            }

            for (int k = 0; k < buttonCount; k++)
            {
                //-> Button 物体处理
                GameObject button = GetPoolsButtonObj();
                button.name = k.ToString();
                Button buttonComponent = button.GetComponent<Button>();
                if (!m_IsAddedListener.ContainsKey(button) && buttonComponent != null)
                {
                    m_IsAddedListener[button] = true;
                    buttonComponent.onClick.AddListener(delegate () { OnClickExpand(button); });
                    button.transform.SetSiblingIndex(0);
                }

                float pos = 0;  //坐标( isVertical ? 记录Y : 记录X )

                //-> 计算 Button 坐标
                if(m_Direction == e_Direction.Vertical)
                {
                    pos = m_ExpandButtonHeight * k + m_Spacing * (k + 1);
                    pos += k > 0 ? (m_CellObjectHeight + m_Spacing) * Mathf.CeilToInt( (float)beforeCellCount / m_Row ) : 0;
                    button.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(m_ExpandButtonX, -pos, 0);
                }
                else
                {
                    pos = m_ExpandButtonWidth * k + m_Spacing * (k + 1);
                    pos += k > 0 ? (m_CellObjectWidth + m_Spacing) * Mathf.CeilToInt( (float)beforeCellCount / m_Row ) : 0;
                    button.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(pos, m_ExpandButtonY, 0);
                }

                int count = int.Parse(numArray[k]);
                totalCount += count;

                //-> 存储数据
                ExpandInfo expandInfo = isReset ? new ExpandInfo() : m_ExpandInfos[k];
                expandInfo.button = button;
                expandInfo.cellCount = count;
                expandInfo.cellInfos = new CellInfo[count];
  
                expandInfo.isExpand = isReset ? m_IsExpand : expandInfo.isExpand;
                expandInfo.size = m_Direction == e_Direction.Vertical ? (m_CellObjectHeight + m_Spacing) * Mathf.CeilToInt((float)count / m_Row) : (m_CellObjectWidth + m_Spacing) * Mathf.CeilToInt((float)count / m_Row); //计算 需展开的尺寸

                //-> 遍历每个按钮下的 Cell
                for (int i = 0; i < count; i++)
                {
                    if (!expandInfo.isExpand) break;

                    CellInfo cellInfo = new CellInfo();

                    float rowPos = 0; //计算每排里面的cell 坐标

                    //-> 计算Cell坐标
                    if(m_Direction == e_Direction.Vertical)
                    {
                        pos = m_CellObjectHeight * Mathf.FloorToInt( i / m_Row ) + m_Spacing * ( Mathf.FloorToInt(i / m_Row) + 1 );
                        pos += (m_ExpandButtonHeight + m_Spacing) * (k + 1);
                        pos += (m_CellObjectHeight + m_Spacing) * Mathf.CeilToInt((float)beforeCellCount /m_Row);
                        rowPos = m_CellObjectWidth * ( i % m_Row ) + m_Spacing * ( i % m_Row);
                        cellInfo.pos = new Vector3(rowPos, -pos, 0);
                    }
                    else
                    {
                        pos = m_CellObjectWidth * Mathf.FloorToInt(i / m_Row) + m_Spacing * ( Mathf.FloorToInt(i / m_Row) + 1 );
                        pos += (m_ExpandButtonWidth + m_Spacing) * (k + 1);
                        pos += (m_CellObjectHeight + m_Spacing) * Mathf.CeilToInt( (float)beforeCellCount /m_Row );
                        rowPos = m_CellObjectHeight * (i % m_Row) + m_Spacing * (i % m_Row);
                        cellInfo.pos = new Vector3(pos, -rowPos, 0);
                    }

                    //-> 计算是否超出范围
                    float cellPos = m_Direction == e_Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                    if (IsOutRange(cellPos))
                    {
                        cellInfo.obj = null;
                        expandInfo.cellInfos[i] = cellInfo;
                        continue;
                    }

                    //-> 取或创建 Cell
                    GameObject cell = GetPoolsObj();
                    cell.transform.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
                    cell.gameObject.name = k + "-" + i.ToString();

                    Button cellButtonComponent = cell.GetComponent<Button>();
                    if (!m_IsAddedListener.ContainsKey(cell) && cellButtonComponent != null)
                    {
                        m_IsAddedListener[cell] = true;
                        cellButtonComponent.onClick.AddListener(delegate () { OnClickCell(cell); });
                    }

                    //-> 存数据
                    cellInfo.obj = cell;
                    expandInfo.cellInfos[i] = cellInfo;

                    //-> 回调  函数
                    Func(m_FuncCallBackFunc, button, cell, expandInfo.isExpand);
                }

                beforeCellCount += expandInfo.isExpand ? count : 0;

                m_ExpandInfos[k] = expandInfo;

                Func(m_FuncCallBackFunc, button, null, expandInfo.isExpand);
            }

            if (!m_IsInited)
            {
                //-> 计算 Content 尺寸
                if(m_Direction == e_Direction.Vertical)
                {
                    float contentSize = m_IsExpand ? (m_Spacing + m_CellObjectHeight) * Mathf.CeilToInt((float)totalCount / m_Row) : 0;
                    contentSize += (m_Spacing + m_ExpandButtonHeight) * buttonCount;
                    m_ContentRectTrans.sizeDelta = new Vector2(m_ContentRectTrans.sizeDelta.x, contentSize);
                }
                else
                {
                    float contentSize = m_IsExpand ? (m_Spacing + m_CellObjectWidth) * Mathf.CeilToInt((float)totalCount / m_Row) : 0;
                    contentSize += (m_Spacing + m_ExpandButtonWidth) * buttonCount;
                    m_ContentRectTrans.sizeDelta = new Vector2(contentSize, m_ContentRectTrans.sizeDelta.y);
                }
            }

            m_IsInited = true;
        }

        //判断是否超出显示范围
        protected bool IsOutRange(float pos)
        {
            Vector3 listP = m_ContentRectTrans.anchoredPosition;
            if (m_Direction == e_Direction.Vertical)
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
        protected Stack<GameObject> poolsObj = new Stack<GameObject>();
        protected virtual GameObject GetPoolsObj()
        {
            GameObject cell = null;
            if (poolsObj.Count > 0)
            {
                cell = poolsObj.Pop();
            }

            if (cell == null)
            {
                cell = GameObject.Instantiate<GameObject>(m_CellGameObject);
            }
            cell.transform.SetParent(m_Content.transform);
            cell.transform.localScale = Vector3.one;
            UIUtils1.SetActive(cell, true);

            return cell;
        }
        //存入 cell
        protected virtual void SetPoolsObj(GameObject cell)
        {
            if (cell != null)
            {
                poolsObj.Push(cell);
                UIUtils1.SetActive(cell, false);
            }
        }
        //清除所有Cell (扔到对象池)
        private void ClearCell()
        {
            if (!m_IsInited) return;

            for (int i = 0, length = m_ExpandInfos.Length; i < length; i++)
            {

                if (m_ExpandInfos[i].button != null)
                {
                    SetPoolsButtonObj(m_ExpandInfos[i].button);
                    m_ExpandInfos[i].button = null;
                }
                for (int j = 0, count = m_ExpandInfos[i].cellInfos.Length; j < count; j++)
                    if (m_ExpandInfos[i].cellInfos[j].obj != null)
                    {
                        SetPoolsObj(m_ExpandInfos[i].cellInfos[j].obj);
                        m_ExpandInfos[i].cellInfos[j].obj = null;
                    }
            }
        }

        //Cell 点击事件
        public void OnClickCell(GameObject cell)
        {
            int index = int.Parse(((cell.name).Split('-'))[0]);
            Func(m_FuncOnClickCallBack, m_ExpandInfos[index].button, cell, m_ExpandInfos[index].isExpand);
        }

        //收展按钮 点击事件
        private void OnClickExpand(GameObject button)
        {
            int index = int.Parse(button.name) + 1;
            OnClickExpand(index);
            if (m_FuncOnButtonClickCallBack != null)
            {
				//m_FuncOnButtonClickCallBack(index, m_ExpandInfos[index - 1].isExpand, button);
            }
        }
        public void OnClickExpand(int index)
        {
            index = index - 1;
            m_ExpandInfos[index].isExpand = !m_ExpandInfos[index].isExpand;

            //-> 计算 Contant Size
            Vector2 size = m_ContentRectTrans.sizeDelta;
            if(m_Direction == e_Direction.Vertical)
            {
                float height = m_ExpandInfos[index].isExpand ? size.y + m_ExpandInfos[index].size : size.y - m_ExpandInfos[index].size;
                m_ContentRectTrans.sizeDelta = new Vector2(size.x, height);
            }
            else
            {
                float width = m_ExpandInfos[index].isExpand ? size.x + m_ExpandInfos[index].size : size.x - m_ExpandInfos[index].size;
                m_ContentRectTrans.sizeDelta = new Vector2(width, size.y);
            }

            int beforeCellCount = 0;
            float pos = 0;
            float rowPos = 0;

            //-> 重新计算坐标 并 显示处理
            for (int k = 0, length = m_ExpandInfos.Length; k < length; k++)
            {
                int count = m_ExpandInfos[k].cellCount;

                if (k >= index)
                {
                    //-> 计算 按钮位置
                    GameObject button = m_ExpandInfos[k].button;
                    if(m_Direction == e_Direction.Vertical)
                    {
                        pos = m_ExpandButtonHeight * k + m_Spacing * (k + 1);
                        pos += (m_CellObjectHeight + m_Spacing) * Mathf.CeilToInt((float)beforeCellCount / m_Row);
                        button.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(m_ExpandButtonX, -pos, 0);
                    }
                    else
                    {
                        pos = m_ExpandButtonWidth * k + m_Spacing * (k + 1);
                        pos += (m_CellObjectWidth + m_Spacing) * Mathf.CeilToInt((float)beforeCellCount / m_Row);
                        button.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(pos, m_ExpandButtonY, 0);
                    }

                    ExpandInfo expandInfo = m_ExpandInfos[k];
                    for (int i = 0; i < count; i++) 
                    {
                        //-> 按钮 收 状态时
                        if(!expandInfo.isExpand)
                        {
                            if (expandInfo.cellInfos[i].obj != null)
                            {
                                SetPoolsObj(expandInfo.cellInfos[i].obj);
                                m_ExpandInfos[k].cellInfos[i].obj = null;
                            }
                            continue;
                        }

                        CellInfo cellInfo = expandInfo.cellInfos[i];

                        // * -> 计算每个Cell坐标
                        if(m_Direction == e_Direction.Vertical)
                        {
                            pos = m_CellObjectHeight * Mathf.FloorToInt(i / m_Row) + m_Spacing * (Mathf.FloorToInt(i / m_Row) + 1);
                            pos += (m_ExpandButtonHeight + m_Spacing) * (k + 1);
                            pos += (m_CellObjectHeight + m_Spacing) * Mathf.CeilToInt((float)beforeCellCount / m_Row);
                            rowPos = m_CellObjectWidth * (i % m_Row) + m_Spacing * (i % m_Row);
                            cellInfo.pos = new Vector3(rowPos, -pos, 0);
                        }
                        else
                        {
                            pos = m_CellObjectWidth * Mathf.FloorToInt(i / m_Row) + m_Spacing * (Mathf.FloorToInt(i / m_Row) + 1);
                            pos += (m_ExpandButtonWidth + m_Spacing) * (k + 1);
                            pos += (m_CellObjectWidth + m_Spacing) * Mathf.CeilToInt((float)beforeCellCount / m_Row);
                            rowPos = m_CellObjectHeight * (i % m_Row) + m_Spacing * (i % m_Row);
                            cellInfo.pos = new Vector3(pos, -rowPos, 0);
                        }

                        //-> 计算是否超出范围
                        float cellPos = m_Direction == e_Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                        if (IsOutRange(cellPos))
                        {
                            SetPoolsObj(cellInfo.obj);
                            cellInfo.obj = null;
                            m_ExpandInfos[k].cellInfos[i] = cellInfo;
                            continue;
                        }

                        GameObject cell = cellInfo.obj != null ? cellInfo.obj : GetPoolsObj();
                        cell.transform.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
                        cell.gameObject.name = k + "-" + i.ToString();

                        //-> 回调
                        if(cellInfo.obj == null)
                        {
                            Func(m_FuncCallBackFunc, button, cell, expandInfo.isExpand);
                        }

                        //-> 添加按钮事件
                        Button cellButtonComponent = cell.GetComponent<Button>();
                        if (!m_IsAddedListener.ContainsKey(cell) && cellButtonComponent != null)
                        {
                            m_IsAddedListener[cell] = true;
                            cellButtonComponent.onClick.AddListener(delegate () { OnClickCell(cell); });
                        }

                        //-> 存数据
                        cellInfo.obj = cell;
                        m_ExpandInfos[k].cellInfos[i] = cellInfo;
                    }
                }

                if (m_ExpandInfos[k].isExpand)
                {
                    beforeCellCount += count;
                }
            }

            //展开时候的背景图
            ExpandBackground(m_ExpandInfos[index]);
            Func(m_FuncCallBackFunc, m_ExpandInfos[index].button, null, m_ExpandInfos[index].isExpand);
        }

        //展开时候的背景图
        private void ExpandBackground(ExpandInfo expandInfo)
        {
            //收展时的 list尺寸变化
            if (expandInfo.isExpand == false)
            {
                var background = expandInfo.button.transform.Find("background");
                if (background != null)
                {
                    RectTransform backgroundTransform = background.GetComponent<RectTransform>();
                    backgroundTransform.sizeDelta = m_BackgroundOriginSize;
                }
            }
            else
            {
                var background = expandInfo.button.transform.Find("background");
                if (background != null)
                {
                    RectTransform backgroundTransform = background.GetComponent<RectTransform>();
                    float total_h = expandInfo.size;
                    if(m_Direction == e_Direction.Vertical)
                    {
                        if (total_h > 3)
                        {
                            backgroundTransform.sizeDelta = new Vector2(m_BackgroundOriginSize.x, m_BackgroundOriginSize.y + total_h + m_BackgroundMargin);
                        }
                        else
                        {
                            backgroundTransform.sizeDelta = new Vector2(m_BackgroundOriginSize.x, m_BackgroundOriginSize.y);
                        }
                    }
                    else
                    {
                        backgroundTransform.sizeDelta = new Vector2(m_BackgroundOriginSize.x + total_h + m_BackgroundMargin, m_BackgroundOriginSize.y);
                    }
                }
            }
        }

        protected void ScrollRectListener(Vector2 value)
        {
            Vector3 contentP = m_ContentRectTrans.anchoredPosition;

            if (m_ExpandInfos == null) return;

            for(int k = 0, length = m_ExpandInfos.Length; k < length; k++)
            {
                ExpandInfo expandInfo = m_ExpandInfos[k];
                if(!expandInfo.isExpand)
                {
                    continue;
                }

                int count = expandInfo.cellCount;
                for(int i = 0; i < count; i++)
                {
                    CellInfo cellInfo = expandInfo.cellInfos[i];
                    float rangePos = m_Direction == e_Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                    if(IsOutRange(rangePos))
                    {
                        SetPoolsObj(cellInfo.obj);
                        m_ExpandInfos[k].cellInfos[i].obj = null;
                    }
                    else
                    {
                        if (cellInfo.obj == null)
                        {
                            GameObject cell = GetPoolsObj();
                            cell.transform.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
                            cell.name = k + "-" + i;

                            Button cellButtonComponent = cell.GetComponent<Button>();
                            if (!m_IsAddedListener.ContainsKey(cell) && cellButtonComponent != null)
                            {
                                m_IsAddedListener[cell] = true;
                                cellButtonComponent.onClick.AddListener(delegate () { OnClickCell(cell); });
                            }

                            cellInfo.obj = cell;

                            m_ExpandInfos[k].cellInfos[i] = cellInfo;

                            Func(m_FuncCallBackFunc, expandInfo.button, cell, expandInfo.isExpand);
                        }
                    }
                }
            }
        }

        private Stack<GameObject> buttonPoolsObj = new Stack<GameObject>();
        //取出 button
        private GameObject GetPoolsButtonObj()
        {
            GameObject button = null;
            if (buttonPoolsObj.Count > 0)
            {
                button = buttonPoolsObj.Pop();
            }
            if (button == null)
            {
                button = GameObject.Instantiate<GameObject>(m_ExpandButton);
            }
            button.transform.SetParent(m_Content.transform);
            button.transform.localScale = Vector3.one;
            UIUtils1.SetActive(button, true);

            return button;
        }
        //存入 button
        private void SetPoolsButtonObj(GameObject button)
        {
            if (button != null)
            {
                buttonPoolsObj.Push(button);
                UIUtils1.SetActive(button, false);
            }
        }

        private void Func(Action<GameObject, GameObject,int, int> Func, GameObject button, GameObject selectObject, bool isShow)
        {
            string[] objName = { "1", "-2" };
            if (selectObject != null)
            {
                objName = (selectObject.name).Split('-');
            }
            int buttonNum = int.Parse(button.name) + 1;
            int num = int.Parse(objName[1]) + 1;

            if (Func != null)
            {
                if (selectObject != null)
                {
                    Func(button, selectObject, buttonNum, num);

                }
                else
                {
                    Func(button, selectObject, buttonNum, -1);

                }
            }
        }

    }
}

