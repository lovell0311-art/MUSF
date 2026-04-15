using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;
using System.Linq;
using System;
using static UnityEditor.LightingExplorerTableColumn;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIIntroductionComponentAwake : AwakeSystem<UIIntroductionComponent>
    {
        public override void Awake(UIIntroductionComponent self)
        {
            self.Awake();
            self.Sell();
            self.Renew();
        }
    }
    /// <summary>
    /// 物品简介组件
    /// </summary>
    public partial class UIIntroductionComponent : Component
    {
        public ReferenceCollector collector;
        public Transform Introcontent;

        private RectTransform RECT;
        public MergerMethod curMergerMethod = null;
        public int TopPadding = 20;//顶部间距
        public int Bottom = 20;//底部间距
        public Vector2 Spaceing = new Vector2(0,5);//间距
        public int ItemHeight = 33;//一列最多显示的行数
        public Vector2 CellSize = new Vector2(300,25);
        Transform modeItem;
        GridLayoutGroup gridLayout;

        public List<string> ItemAtrList;//物品属性列表

        public KnapsackDataItem data;//物品数据
        private int Pos_x = -1, Pos_y = -1;
        public E_Grid_Type datatype,Merger;

        private bool Satisfy = true,MergerSatisfy=false;
        GameObject EquipInfo,obj,Mergerobj;
        Transform InfoContent,contrastContent;
        Transform equipItem,contrastItem;
        public List<string> EquipItemAtrList;//装备物品属性列表
        //丢弃、穿戴装备 委托
        public Action WareAction, DiscarACtion;
        Button useBtn, wareBtn,shareBtn,shareItemBtn, ListringBtn;

        //垂直滚动简介信息
       public GameObject Intro_Vertical, Vertical_Btns, Vertical_SellBtn, Vertical_BuyBtn, Vertical_Renew;
        Transform VerticalContent,verticaItem;
        RectTransform VerticalView;
        public Action VerticalWareAction, VerticalDiscarACtion, VerticalUserAction, VerticalSellAction, VerticalBuyAction, VerticalShareAction;
        public Action VerticalRenewAction;
        public Action<int> SellAction;



        private void Merger_Pos()
        {
            if (UIKnapsackComponent.Instance!=null)
            {
                for (int i = 0; i < UIKnapsackComponent.LENGTH_Merger_Y; i++)
                {
                    for (int j = 0; j < UIKnapsackComponent.LENGTH_Merger_X; j++)
                    {

                        KnapsackGrid grid = UIKnapsackComponent.Instance.grids[j][i];
                        if (grid.IsOccupy)
                        {
                            // Debug.Log("grid" + grid.IsOccupy + "x"+j+"::y"+i);
                        }
                        if (grid.IsOccupy == false)
                        {
                            bool gridbool = false;
                            for (int q = 0; q < data.item_Info.Y; q++)//3
                            {
                                for (int w = 0; w < data.item_Info.X; w++)//1
                                {
                                    if ((j + w) <= 7 && (i + q) <= 3)
                                    {
                                        KnapsackGrid grids = UIKnapsackComponent.Instance.grids[j + w][i + q];
                                        if (grids.IsOccupy == true)//有被占用
                                        {
                                            gridbool = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (gridbool == false)
                            {
                                bool jk = false;
                                MergerSatisfy = true;
                                Pos_x = j;
                                Pos_y = i;
                                for (int w1 = 0; w1 < data.item_Info.Y; w1++)
                                {
                                    for (int q1 = 0; q1 < data.item_Info.X; q1++)
                                    {
                                        if ((i + w1) > 3 || (q1 + j) > 7)
                                        {
                                            jk = true;
                                            break;
                                        }
                                    }
                                }
                                if (jk == false)
                                {
                                    MergerSatisfy = false;
                                    return;
                                }
                            }
                        }
                    }

                }
            }
        }


        private void OnPos()
        {
            //if (UIKnapsackComponent.Instance != null)
            //{
            //    for (int i = 0; i < UIKnapsackComponent.LENGTH_Knapsack_Y; i++)
            //    {
            //        for (int j = 0; j < UIKnapsackComponent.LENGTH_Knapsack_X; j++)
            //        {

            //            KnapsackGrid grid = UIKnapsackComponent.Instance.grids[j][i];
            //            if (grid.IsOccupy)
            //            {
            //                // Debug.Log("grid" + grid.IsOccupy + "x"+j+"::y"+i);
            //            }
            //            if (grid.IsOccupy == false)
            //            {
            //                bool gridbool = false;
            //                for (int q = 0; q < data.item_Info.Y; q++)//3
            //                {
            //                    for (int w = 0; w < data.item_Info.X; w++)//1
            //                    {
            //                        if ((j + w) <= 7 && (i + q) <= 11)
            //                        {
            //                            KnapsackGrid grids = UIKnapsackComponent.Instance.grids[j + w][i + q];
            //                            if (grids.IsOccupy == true)//有被占用
            //                            {
            //                                gridbool = true;
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //                if (gridbool == false)
            //                {
            //                    bool jk = false;
            //                    Satisfy = true;
            //                    Pos_x = j;
            //                    Pos_y = i;
            //                    for (int w1 = 0; w1 < data.item_Info.Y; w1++)
            //                    {
            //                        for (int q1 = 0; q1 < data.item_Info.X; q1++)
            //                        {
            //                            if ((i + w1) > 11 || (q1 + j) > 7)
            //                            {
            //                                jk = true;
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    if (jk == false)
            //                    {
            //                        Satisfy = false;
            //                        return;
            //                    }
            //                }
            //            }
            //        }

            //    }
            //}
            UI uI = UIComponent.Instance.Get(UIType.UIKnapsackNew);
            if (uI != null)
            {
                UIKnapsackNewComponent uIKnapsackNewComponent = uI.GetComponent<UIKnapsackNewComponent>();
                for (int i = 0; i < UIKnapsackNewComponent.LENGTH_Knapsack_Y; i++)
                {
                    for (int j = 0; j < UIKnapsackNewComponent.LENGTH_Knapsack_X; j++)
                    {

                        KnapsackNewGrid grid = uIKnapsackNewComponent.grids[j][i];
                        if (grid.IsOccupy)
                        {
                            // Debug.Log("grid" + grid.IsOccupy + "x"+j+"::y"+i);
                        }
                        if (grid.IsOccupy == false)
                        {
                            bool gridbool = false;
                            for (int q = 0; q < data.item_Info.Y; q++)//3
                            {
                                for (int w = 0; w < data.item_Info.X; w++)//1
                                {
                                    if ((j + w) <= 7 && (i + q) <= 11)
                                    {
                                        KnapsackNewGrid grids = uIKnapsackNewComponent.grids[j + w][i + q];
                                        if (grids.IsOccupy == true)//有被占用
                                        {
                                            gridbool = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (gridbool == false)
                            {
                                bool jk = false;
                                Satisfy = true;
                                Pos_x = j;
                                Pos_y = i;
                                for (int w1 = 0; w1 < data.item_Info.Y; w1++)
                                {
                                    for (int q1 = 0; q1 < data.item_Info.X; q1++)
                                    {
                                        if ((i + w1) > 11 || (q1 + j) > 7)
                                        {
                                            jk = true;
                                            break;
                                        }
                                    }
                                }
                                if (jk == false)
                                {
                                    Satisfy = false;
                                    return;
                                }
                            }
                        }
                    }

                }
            }

        }
        public void Awake()
        {
            ItemAtrList = new List<string>(100);
            ItemAtrList.Clear();
            collector = GetParent<UI>().GameObject.GetReferenceCollector();
            //collector.GetButton("Closebtn").onClick.AddSingleListener(() => { UIComponent.Instance.Remove(UIType.UIIntroduction);});
            Introcontent = collector.GetImage("Intro").transform;
            modeItem = Introcontent.GetChild(0);

            RECT = collector.GetComponent<RectTransform>();
            obj = collector.GetGameObject("obj");
            //Mergerobj = collector.GetGameObject("Mergerobj");
            GetParent<UI>().GameObject.GetComponent<Canvas>().planeDistance = 5;
            InitGroup();
            InitContrastAtr();
            Init_Vertical();
            datatype = E_Grid_Type.None;
            Merger= E_Grid_Type.None;
            void InitGroup() 
            {
                gridLayout = Introcontent.GetComponent<GridLayoutGroup>();
                gridLayout.padding.top = TopPadding;
                gridLayout.padding.bottom = Bottom;
                gridLayout.spacing = Spaceing;
                gridLayout.cellSize = CellSize;

                obj.transform.Find("Btn").GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    //   Debug.Log("点击了物品" + data.item_Info.Name+":::"+data.item_Info.X+ "::y:"+data.item_Info.Y+ "::(int)data.item_Info.Type"+data.item_Info.Type);
                    //   UIKnapsackComponent.Instance.FinishingBackpack();
                    OnPos();
                    if (Satisfy==true)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "背包空间不足");
                        return;
                    }
                    KnapsackTools.GetKnapsackItemPos(data.item_Info.X, data.item_Info.Y);

                    //UIKnapsackComponent.Instance.RemoveWareEquipItem((int)datatype, true);//移除装备栏的物品
                    //UIKnapsackComponent.Instance.UnLoadEquip((int)datatype, Pos_x, Pos_y).Coroutine();

                    if (UIKnapsackComponent.Instance != null)
                    {
                        UIKnapsackComponent.Instance.UnLoadEquip((int)datatype, Pos_x, Pos_y).Coroutine();
                        UIKnapsackComponent.Instance.RemoveWareEquipItem((int)datatype, true);//移除装备栏的物品
                    }

                    UI uI = UIComponent.Instance.Get(UIType.UIKnapsackNew);
                    if (uI != null)
                    {
                        UIKnapsackNewComponent uIKnapsackNewComponent = uI.GetComponent<UIKnapsackNewComponent>();
                        uIKnapsackNewComponent.UnLoadEquip((int)datatype, Pos_x, Pos_y).Coroutine();
                        uIKnapsackNewComponent.RemoveWareEquipItem((int)datatype, true);//移除装备栏的物品
                        uIKnapsackNewComponent.ClearIntroduction();
                    }

                });
            }

            //装备属性对比
           void InitContrastAtr() 
            {
                EquipInfo = collector.GetGameObject("EquipInfo");
                EquipInfo.SetActive(false);
                InfoContent = collector.GetGameObject("InfoContent").transform;
                equipItem = InfoContent.GetChild(0);
                contrastContent = collector.GetGameObject("ConotrastContent").transform;
                contrastItem = contrastContent.GetChild(0);
                shareItemBtn = collector.GetButton("ShareItemBtn");
                ListringBtn = collector.GetButton("ListringBtn");




                collector.GetButton("DiscardBtn").onClick.AddSingleListener(() =>
                {
                    //丢弃
                    DiscarACtion?.Invoke();
                });
               
                collector.GetButton("WearBtn").onClick.AddSingleListener(() => 
                {
                    //穿戴
                    WareAction?.Invoke();

                });
                shareItemBtn.onClick.AddSingleListener(() =>
                {
                    //分享
                    VerticalShareAction?.Invoke();
                });
                ListringBtn.onClick.AddSingleListener(() =>
                {
                    //上架
                    //VerticalListingAction?.Invoke();
                    ShowSellPanel(true);
                });
               
            }

            //装备属性垂直显示
            void Init_Vertical() 
            {
                Intro_Vertical = collector.GetImage("Intro_Vertical").gameObject;
                VerticalContent = collector.GetGameObject("VerticalContent").transform;
                verticaItem=VerticalContent.GetChild(0);
                VerticalView = collector.GetImage("VerticalView").GetComponent<RectTransform>();
                Vertical_Btns = collector.GetGameObject("Vertical_Btns");
                Vertical_SellBtn = collector.GetGameObject("Vertical_SellBtn");
                Vertical_BuyBtn = collector.GetGameObject("Vertical_BuyBtn");
                Vertical_Renew = collector.GetGameObject("Vertical_Renew");

                useBtn = Vertical_Btns.transform.Find("UseBtn").GetComponent<Button>();
                wareBtn = Vertical_Btns.transform.Find("WareBtn").GetComponent<Button>();
                shareBtn = Vertical_Btns.transform.Find("ShareBtn").GetComponent<Button>();
                ListringBtn = Intro_Vertical.transform.Find("ListringBtn/Listring").GetComponent<Button>();

                Vertical_Btns.transform.Find("DisCarBtn").GetComponent<Button>().onClick.AddSingleListener(()=>VerticalDiscarACtion?.Invoke());
                wareBtn.GetComponent<Button>().onClick.AddSingleListener(()=> VerticalWareAction?.Invoke());
                useBtn.GetComponent<Button>().onClick.AddSingleListener(()=> VerticalUserAction?.Invoke());
                Vertical_SellBtn.transform.Find("SellBtn").GetComponent<Button>().onClick.AddSingleListener(()=> VerticalSellAction?.Invoke());
                Vertical_BuyBtn.transform.Find("SellBtn").GetComponent<Button>().onClick.AddSingleListener(()=> VerticalBuyAction?.Invoke());
                Vertical_Renew.transform.Find("Renew").GetComponent<Button>().onClick.AddSingleListener(()=> ShowRenewPanel(true));
                shareBtn.GetComponent<Button>().onClick.AddSingleListener(()=> VerticalShareAction?.Invoke());
                ListringBtn.GetComponent<Button>().onClick.AddSingleListener(()=> ShowSellPanel(true));
            }
        }
        /// <summary>
        /// 改变GridLayoutGroup的StartCorner
        /// </summary>
        public void IsListring(bool isShow)
        {
            ListringBtn = collector.GetButton("ListringBtn");
            ListringBtn.gameObject.SetActive(isShow);
        }
        public void ChangeStartCorner(Corner corner= Corner.UpperRight) 
        {
            gridLayout.startCorner = corner;
        }
        /// <summary>
        /// 获取 所有属性
        /// </summary>
        /// <param name="dataItem">装备实体</param>
        /// <param name="e_Knapsack"></param>
        public void GetAllAtrs(KnapsackDataItem dataItem, E_KnapsackIntroduceShowPrice e_Knapsack=E_KnapsackIntroduceShowPrice.None)
        {
            if (ItemAtrList == null)
            {
                ItemAtrList = new List<string>(100);
            }
            ItemAtrList.Clear();
            dataItem.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);

            dataItem.GetItemName(ref ItemAtrList);//装备 名字
            dataItem.GetItemCount(ref ItemAtrList);//数量
            data = dataItem;
            //打开商城时 才显示价格
            if (e_Knapsack == E_KnapsackIntroduceShowPrice.BuyPrice)//NPC 商城显示出购买格
                dataItem.GetItemBuyPrice(ref ItemAtrList);
            else if (e_Knapsack == E_KnapsackIntroduceShowPrice.SellPrice)//背包显示出售价格
                dataItem.GetItemSellPrice(ref ItemAtrList);
            else if(e_Knapsack==E_KnapsackIntroduceShowPrice.StallBuyPrice)//摊位购买价格
                dataItem.GetItemsStallBuyPrice(ref ItemAtrList); 
            else if(e_Knapsack==E_KnapsackIntroduceShowPrice.StallSellPrice)//摊位出售价格
                dataItem.GetItemsStallSellPrice(ref ItemAtrList);

            if (dataItem.ConfigId == 260012)//黑王马
            {
                dataItem.GetHeiWangMaAtrs(ref ItemAtrList);
            }
            else if (dataItem.ConfigId == 260015)//天鹰
            {

                dataItem.GetTianYingAtr(ref ItemAtrList);

            }
            else if (dataItem.ConfigId == 260019)//烈火凤凰
            {

                dataItem.GetLieHuoFengHuangAtr(ref ItemAtrList);

            }
            else if (dataItem.ConfigId == 260011)//炎狼兽之角 +幻影
            {
                dataItem.GetYangLangShouZhiJiaoHuanYingAtrs(ref ItemAtrList);
            }
            else if(dataItem.IsTreasureItem())
            {
                dataItem.GetTreasureAtrs(ref ItemAtrList);
            }
            else
            {
                dataItem.GetBaseAtrs(ref ItemAtrList);//基本属性(读表)
                dataItem.GetItemCommonBaseAtr(ref ItemAtrList);//基本属性
                dataItem.GetGemsAtr(ref ItemAtrList);//荧光宝石属性
               // dataItem.GetLevNeed(ref ItemAtrList);//等级需求
                dataItem.GetUserType(ref ItemAtrList);//职业限制
                dataItem.GetExtraEntryAtr(ref ItemAtrList);//套装附带的额外属性
                dataItem.GetItemSkill(ref ItemAtrList);//技能
                dataItem.GetLuckyAtr(ref ItemAtrList);//幸运属性
                dataItem.GetAppendAtr(ref ItemAtrList);//追加属性
                dataItem.GetExecllentEntry(ref ItemAtrList);//卓越属性
                dataItem.GetSpecialEntry(ref ItemAtrList);//特殊属性-翅膀
                dataItem.GetReginAtr(ref ItemAtrList);//再生属性
                dataItem.GetInlayAtr(ref ItemAtrList);//镶嵌属性
                dataItem.GetSuitAtr(ref ItemAtrList);//套装属性
                dataItem.GetVaildTime(ref ItemAtrList);//有效时间
                dataItem.GetRemarks(ref ItemAtrList);//备注提示信息
                dataItem.GetAdmissionTicketOpenTime(ref ItemAtrList);//副本开放时间
            }

        }
        public void GetEquipAllAtrs(KnapsackDataItem dataItem, E_KnapsackIntroduceShowPrice e_Knapsack = E_KnapsackIntroduceShowPrice.None)
        {
            if (EquipItemAtrList == null)
            {
                EquipItemAtrList = new List<string>(100);
                EquipItemAtrList.Clear();
            }
            EquipItemAtrList.Clear();
            dataItem.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);

            dataItem.GetItemName(ref EquipItemAtrList);//装备 名字
            dataItem.GetItemCount(ref EquipItemAtrList);//数量

            //打开商城时 才显示价格
            if (e_Knapsack == E_KnapsackIntroduceShowPrice.BuyPrice)//NPC 商城显示出购买格
                dataItem.GetItemBuyPrice(ref EquipItemAtrList);
            else if (e_Knapsack == E_KnapsackIntroduceShowPrice.SellPrice)//背包显示出售价格
                dataItem.GetItemSellPrice(ref EquipItemAtrList);
            else if (e_Knapsack == E_KnapsackIntroduceShowPrice.StallBuyPrice)//摊位购买价格
                dataItem.GetItemsStallBuyPrice(ref EquipItemAtrList);
            else if (e_Knapsack == E_KnapsackIntroduceShowPrice.StallSellPrice)//摊位出售价格
                dataItem.GetItemsStallSellPrice(ref EquipItemAtrList);

            if (dataItem.ConfigId == 260012)//黑王马
            {
                dataItem.GetHeiWangMaAtrs(ref EquipItemAtrList);
            }
            else if (dataItem.ConfigId == 260011)//炎狼兽之角 +幻影
            {
                dataItem.GetYangLangShouZhiJiaoHuanYingAtrs(ref EquipItemAtrList);
            }
            else if (dataItem.IsTreasureItem())
            {
                dataItem.GetTreasureAtrs(ref ItemAtrList);
            }
            else
            {
                dataItem.GetBaseAtrs(ref EquipItemAtrList);//基本属性(读表)
                dataItem.GetItemCommonBaseAtr(ref EquipItemAtrList);//基本属性
                dataItem.GetGemsAtr(ref EquipItemAtrList);//荧光宝石属性
                                                     // dataItem.GetLevNeed(ref ItemAtrList);//等级需求
                dataItem.GetUserType(ref EquipItemAtrList);//职业限制
                dataItem.GetExtraEntryAtr(ref EquipItemAtrList);//套装附带的额外属性
                dataItem.GetItemSkill(ref EquipItemAtrList);//技能
                dataItem.GetLuckyAtr(ref EquipItemAtrList);//幸运属性
                dataItem.GetAppendAtr(ref EquipItemAtrList);//追加属性
                dataItem.GetExecllentEntry(ref EquipItemAtrList);//卓越属性
                dataItem.GetSpecialEntry(ref EquipItemAtrList);//特殊属性-翅膀
                dataItem.GetReginAtr(ref EquipItemAtrList);//再生属性
                dataItem.GetInlayAtr(ref EquipItemAtrList);//镶嵌属性
                dataItem.GetSuitAtr(ref EquipItemAtrList);//套装属性
                dataItem.GetVaildTime(ref EquipItemAtrList);//有效时间
                dataItem.GetRemarks(ref EquipItemAtrList);//备注提示信息
            }

        }
        /// <summary>
        /// 添加自定义属性
        /// </summary>
        /// <param name="ItemAtrList"></param>
        public void AddDiyAtr(List<string> ItemAtrList) 
        {
            this.ItemAtrList.AddRange(ItemAtrList);
        }

        /// <summary>
        /// 显示属性面板
        /// 0 默认属性GridGroup面板
        /// 1 装备属性对比
        /// 2 Vertical滚动属性面板
        /// </summary>
        /// <param name="type"></param>
        public void ChangeInfoState(int type=0)
        {
            Introcontent.gameObject.SetActive(type==0);
            EquipInfo.SetActive(type==1);
            Intro_Vertical.SetActive(type==2);
            obj.transform.SetParent(collector.transform);
           // Log.DebugBrown("当前属性提示" + type+"::背包"+datatype);
            if (datatype != E_Grid_Type.None && type == 0)
            {
                obj.gameObject.SetActive(true);
                //Introcontent.transform.SetSiblingIndex(9999);
            }
            else
            {

                obj.gameObject.SetActive(false);
            }
           // Mergerobj.gameObject.SetActive(Merger == E_Grid_Type.Gem_Merge);
        }

        public void ShowEquipInfo() 
        {
            
            ShowAtr(ItemAtrList, InfoContent, equipItem);
            ShowAtr(EquipItemAtrList, contrastContent, contrastItem);

            //显示属性
            void ShowAtr(List<string> list,Transform content,Transform childitem) 
            {
                int atrCount = list.Count;
                int introChildCount = content.childCount;
                if (introChildCount > atrCount)//隐藏多余的Item
                {
                    for (int i = atrCount; i < introChildCount; i++)
                    {
                        content.GetChild(i).gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < atrCount; i++)
                {
                    Transform item;
                    if (i < introChildCount)
                    {
                        item = content.GetChild(i);
                        item.gameObject.SetActive(true);

                    }
                    else
                    {
                        item = GameObject.Instantiate<Transform>(childitem, content);
                    }
                    item.GetComponent<Text>().text = list[i].ToString();

                }
            }
            ChangeInfoState(1);
        }
       
        /// <summary>
        /// 显示属性
        /// </summary>
        public void ShowAtrs()
        {
            ChangeInfoState(0);
            int atrCount = ItemAtrList.Count;
            //最后一列未空 
            if (ItemAtrList.Count== ItemHeight+1&& string.IsNullOrEmpty(ItemAtrList.Last()))
            {
                atrCount--;
            }
            int introChildCount = Introcontent.childCount;
            if (introChildCount > atrCount)//隐藏多余的Item
            {
                for (int i = atrCount; i < introChildCount; i++)
                {
                    Introcontent.GetChild(i).gameObject.SetActive(false);
                }
            }
            
            gridLayout.constraintCount = atrCount > ItemHeight ? ItemHeight : atrCount;
            for (int i = 0; i < atrCount; i++)
            {
                Transform item;
                if (i < introChildCount)
                {
                    item = Introcontent.GetChild(i);
                    item.gameObject.SetActive(true);

                }
                else
                {
                    item = GameObject.Instantiate<Transform>(modeItem, Introcontent);
                }
                //if (item.name != "obj")
                //{

                //    item.GetComponent<Text>().text = ItemAtrList[i].ToString();
                //}
                item.GetComponent<Text>().text = ItemAtrList[i].ToString();
                //if (datatype != E_Grid_Type.None)
                //{
                //    Introcontent.transform.Find("obj").SetSiblingIndex(9999);
                //}
                
            }

            Introcontent.GetChild(0).gameObject.SetActive(false);
            obj.transform.SetParent(Introcontent);

        }

      /// <summary>
      /// 垂直滚动显示属性
      /// </summary>
      /// <param name="isShowBtn">是否开启功能按钮</param>
      /// <param name="isShowBtn">是否开启出售按钮</param>
        public void ShowAtr_Vertical(bool isShowBtn=false,bool issell=false,bool isbuy=false,bool IsUse=false,bool isShare = false, bool isListring = false, bool isRenew = false) 
        {
            int atrCount = ItemAtrList.Count;
           
            int introChildCount = VerticalContent.childCount;
            if (introChildCount > atrCount)//隐藏多余的Item
            {
                for (int i = atrCount; i < introChildCount; i++)
                {
                    VerticalContent.GetChild(i).gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < atrCount; i++)
            {
                Transform item;
                if (i < introChildCount)
                {
                    item = VerticalContent.GetChild(i);
                    item.gameObject.SetActive(true);

                }
                else
                {
                    item = GameObject.Instantiate<Transform>(verticaItem, VerticalContent);
                }
                item.GetComponent<Text>().text = ItemAtrList[i].ToString();

            }
          
            useBtn.gameObject.SetActive(IsUse);
            wareBtn.gameObject.SetActive(!IsUse);
            Vertical_Btns.SetActive(isShowBtn);
            Vertical_SellBtn.SetActive(issell);
            Vertical_BuyBtn.SetActive(isbuy);
            shareBtn.gameObject.SetActive(isShare);
            //Log.DebugGreen("是否可以上架"+ isListring);
            ListringBtn.gameObject.SetActive(isListring);
           // Vertical_Renew.gameObject.SetActive(isRenew);


           
            Canvas.ForceUpdateCanvases();
            ChangeInfoState(2);
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="pivot_x"></param>
        public void SetPos(Vector3 pos,float pivot_x)
        {

            Vector3 screenPos = CameraComponent.Instance.UICamera.WorldToScreenPoint(pos);
            Vector2 pivot = Vector2.one * .5f;
            pivot.x = pivot_x;
            pivot.y = screenPos.y / Screen.height;
            screenPos = CameraComponent.Instance.UICamera.WorldToScreenPoint(pos);
            Introcontent.GetComponent<RectTransform>().pivot = pivot;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RECT, screenPos, CameraComponent.Instance.UICamera, out Vector2 localPos);
            Introcontent.transform.localPosition = localPos;
         
        }

        public void SetVerticalPos(Vector3 pos, float pivot_x)
        {
            Canvas.ForceUpdateCanvases();
            var height = Mathf.Clamp(VerticalContent.GetComponent<RectTransform>().rect.height, 50, 800);
            VerticalView.sizeDelta = new Vector2(VerticalView.sizeDelta.x, height);

            Vector3 screenPos = CameraComponent.Instance.UICamera.WorldToScreenPoint(pos);
            Vector2 pivot = Vector2.one * .5f;
            pivot.x = pivot_x;
            pivot.y = screenPos.y / Screen.height;
            screenPos = CameraComponent.Instance.UICamera.WorldToScreenPoint(pos);
            Intro_Vertical.GetComponent<RectTransform>().pivot = pivot;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RECT, screenPos, CameraComponent.Instance.UICamera, out Vector2 localPos);
            Intro_Vertical.transform.localPosition = localPos;

        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();

            
            this.ItemAtrList.Clear();
            ItemAtrList = null;
            EquipItemAtrList?.Clear();
            EquipItemAtrList = null;
            DiscarACtion = null;
            WareAction = null;
            VerticalDiscarACtion = null;
            VerticalWareAction = null;
            VerticalShareAction = null;
            SellAction = null;
            VerticalRenewAction = null;

        }
        internal void GetDataType(E_Grid_Type part)
        {
            datatype = part;
        }
        internal void GetMerherType(E_Grid_Type part)
        {
            Merger = part;
        }
    }
}
