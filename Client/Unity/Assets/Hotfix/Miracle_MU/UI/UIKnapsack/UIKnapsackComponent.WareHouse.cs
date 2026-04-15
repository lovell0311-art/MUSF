using ETModel;
using ILRuntime.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{

    /// <summary>
    /// 仓库
    /// </summary>
    public partial class UIKnapsackComponent
    {
        private GameObject WareHouseContent;

        private KnapsackGrid[][] WareHouseGrids;
        public const int LENGTH_WareHouse_X = 8;
        public static int LENGTH_WareHouse_Y = 11;
        public static int MAX_HOUSE_LENGSH_Y = 11;//每页最大行数 11

        /// <summary>
        /// 仓库 页数列表
        /// </summary>
        private readonly List<List<KnapsackDataItem>> PageList = new List<List<KnapsackDataItem>>();

        private bool isOpen = false;//仓库面板是否打开
        private int curPage = 1;//当前页数
        private int allPage = 1;//总页数
        private Text pageText;//页数

        public Text wareCoin;//仓库金币

        private void Init_WareHouse()
        {
            WareHouseContent = WareHouse.GetReferenceCollector().GetGameObject("Grids");
            ReferenceCollector collector = WareHouse.GetReferenceCollector();
            wareCoin = collector.GetText("yuanbaoTxt");
            ///仓库中的金币
            wareCoin.text = $"{KnapsackItemsManager.WareGlodCoin}";
            Game.EventCenter.EventListenner<long>(EventTypeId.WARE_GOLDCOIN_CHANGE, ChangeCoin);
            ///存钱
            collector.GetButton("SaveMoneyBtn").onClick.AddSingleListener(() =>
            {
                UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent(E_TipPanelType.WareHouse);
                confirmComponent.WareInfoTitleTxt.text = "输入要存入的金币数量";
                confirmComponent.WareHouseEventAction = async () =>
                {
                    int? glod = confirmComponent.GetGoldFunc?.Invoke();
                    if (glod == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "先输入要存入的金币");
                        return;
                    }
                    //要存金币是否大于玩家所拥有的金币
                    if (glod > roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin))
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "输入的金币 大于 您当前所拥有的金币");
                        return;
                    }
                    G2C_AddGoldToWarehouse c_AddGoldToWarehouse = (G2C_AddGoldToWarehouse)await SessionComponent.Instance.Session.Call(new C2G_AddGoldToWarehouse
                    {
                        Gold = glod.ToInt64()
                    });
                    if (c_AddGoldToWarehouse.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, c_AddGoldToWarehouse.Error.GetTipInfo());
                    }
                    else
                    {

                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"成功存入 {glod.ToInt64()} 金币 ");
                    }
                };
            });
            ///取钱
            collector.GetButton("TakeMoneyBtn").onClick.AddSingleListener(() =>
            {
                UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent(E_TipPanelType.WareHouse);
                confirmComponent.WareInfoTitleTxt.text = "输入要取出的金币数量";
                confirmComponent.WareHouseEventAction = async () =>
                {
                    int? glod = confirmComponent.GetGoldFunc?.Invoke();
                    Log.DebugGreen($"取钱 {glod}");

                    if (glod == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "先输入要取出的金币");
                        return;
                    }
                    //要取金币是否大于玩家所缓存的的金币
                    Log.DebugGreen($"glod > KnapsackItemsManager.WareGlodCoin:{glod > KnapsackItemsManager.WareGlodCoin}");
                    if (glod > KnapsackItemsManager.WareGlodCoin)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "输入的金币 大于 您当前所存的金币");
                        return;
                    }
                    Log.DebugBrown($"需要取出:{glod}");
                    G2C_TackOutGoldInWarehouse c_AddGoldToWarehouse = (G2C_TackOutGoldInWarehouse)await SessionComponent.Instance.Session.Call(new C2G_TackOutGoldInWarehouse
                    {
                        Gold = glod.ToInt64()
                    });
                    if (c_AddGoldToWarehouse.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, c_AddGoldToWarehouse.Error.GetTipInfo());
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"成功取出 {glod.ToInt64()} 金币 ");
                    }
                };

            });

            ///页数
            pageText = collector.GetText("page");
            ///上一页
            collector.GetButton("PageUp").onClick.AddSingleListener(() => UpPage());
            ///下一页
            collector.GetButton("PageDown").onClick.AddSingleListener(() => NextPage());

            LENGTH_WareHouse_Y = MAX_HOUSE_LENGSH_Y;
            WareHouseGrids = new KnapsackGrid[LENGTH_WareHouse_X][];
            for (int i = 0; i < WareHouseGrids.Length; i++)
            {
                WareHouseGrids[i] = new KnapsackGrid[MAX_HOUSE_LENGSH_Y];
            }
            //初始化格子
            CreatGrid(LENGTH_WareHouse_X, MAX_HOUSE_LENGSH_Y, WareHouseContent.transform, E_Grid_Type.Ware_House, ref WareHouseGrids);

            InitPage();

            Init_WareHouseItem();

            Game.EventCenter.EventListenner<KnapsackDataItem>(EventTypeId.WARE_ADD_ITEM, AddWareHouse);//监听 仓库新加物品
            isOpen = true;

        }

        private void ChangeCoin(long value)
        {
            wareCoin.text = $"{value}";
        }
        /// <summary>
        /// 初始化 仓库中的物品
        /// 默认显示第一页 的物品
        /// </summary>
        private void Init_WareHouseItem()
        {
            ///将物品 分页归类
            foreach (var item in KnapsackItemsManager.WareHouseItems.Values)
            {
                PageList[item.Page - 1].Add(item);//page-1 默认是从 0 开始
            }
            ///默认是显示第一页
            RefreshWareItems(1);
          
        }
        /// <summary>
        /// 仓库中添加物品
        /// </summary>
        /// <param name="dataItem"></param>
        public void AddWareHouse(KnapsackDataItem dataItem)
        {
            if (!isOpen) return;
            PageList[dataItem.Page - 1].Add(dataItem);//page-1 默认是从 0 开始
            AddItem(dataItem, type: E_Grid_Type.Ware_House);
        }
        /// <summary>
        /// 仓库 移除物品
        /// </summary>
        /// <param name="removedataItemUUID"></param>
        public void RemoveWareHouse(long removedataItemUUID)
        {
            if (PageList[curPage - 1].Exists(r => r.Id == removedataItemUUID))
            {
                var item = PageList[curPage - 1].Find(r => r.Id == removedataItemUUID);
               // item.Dispose();
                PageList[curPage - 1].Remove(item);
            }
        }

        /// <summary>
        /// 根据页数 刷新物品
        /// 默认是显示第一页
        /// </summary>
        /// <param name="page">页数</param>
        private void RefreshWareItems(int page = 0)
        {
            page -= 1;
            foreach (var item in PageList[page])
            {
                AddItem(item, type: E_Grid_Type.Ware_House);
            }
        }

        /// <summary>
        /// 下一页
        /// </summary>
        private void NextPage()
        {
            if (curPage == allPage)//当前页数 等于 最大页数
            {
                return;
            }
            pageText.text = $"{++curPage}/{allPage}";
            //清理上一页 的物品
             CleanWareHouse();
            ChangeGrids(curPage);
            RefreshWareItems(curPage);
        }
        /// <summary>
        /// 上一页
        /// </summary>

        private void UpPage()
        {
            if (curPage == 1)//当前页数 已经是第一页了
            {
                return;
            }
            pageText.text = $"{--curPage}/{allPage}";
            CleanWareHouse();
            ChangeGrids(curPage);
            RefreshWareItems(curPage);
        }

        /// <summary>
        /// 根据页数 来显示当前页面的格子数
        /// </summary>
        /// <param name="page"></param>
        private void ChangeGrids(int page)
        {
            //LENGTH_WareHouse_Y 赋值 便于隐藏为解锁的格子
            if (page <= allPage - 1)
            {
                LENGTH_WareHouse_Y = MAX_HOUSE_LENGSH_Y;
            }
            else
            {
                LENGTH_WareHouse_Y = KnapsackItemsManager.WareHouseRows % MAX_HOUSE_LENGSH_Y == 0 ? MAX_HOUSE_LENGSH_Y : KnapsackItemsManager.WareHouseRows % MAX_HOUSE_LENGSH_Y;
            }
            for (int i = 0; i < MAX_HOUSE_LENGSH_Y; i++)
            {
                for (int j = 0; j < LENGTH_WareHouse_X; j++)
                {
                    WareHouseContent.transform.Find($"{j}_{i}").gameObject.SetActive(true);
                    if (WareHouseContent.transform.Find($"{j}_{i}/lock") != null)
                        WareHouseContent.transform.Find($"{j}_{i}/lock").gameObject.SetActive(i >= LENGTH_WareHouse_Y);
                }
            }
        }

        /// <summary>
        /// 初始化页数
        /// </summary>
        private void InitPage()
        {
            PageList.Clear();
            var addpage = KnapsackItemsManager.WareHouseRows / MAX_HOUSE_LENGSH_Y;
          
           /* allPage = KnapsackItemsManager.WareHouseRows / MAX_HOUSE_LENGSH_Y +
                (addpage > 1 ? 1 : KnapsackItemsManager.WareHouseRows % MAX_HOUSE_LENGSH_Y != 0 ? 1 : 0);//得到总页数*/
            allPage = addpage + (KnapsackItemsManager.WareHouseRows % MAX_HOUSE_LENGSH_Y != 0 ? 1 : 0);//得到总页数

            for (int i = 0; i < allPage; i++)
            {
                PageList.Add(new List<KnapsackDataItem>());
            }
            //页数
            pageText.text = $"{curPage}/{allPage}";
        }
        /// <summary>
        /// 清理仓库面板
        /// </summary>
        public  async void CleanWareHouse()
        {
           await CleanWareHouseAsync();

            ETTask CleanWareHouseAsync()
            {
                GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Ware_House);
                for (int i = 0; i < LENGTH_WareHouse_Y; i++)
                {
                    for (int j = 0; j < LENGTH_WareHouse_X; j++)
                    {
                        KnapsackGrid grid = grids[j][i];
                        if (grid.IsOccupy)
                        {
                            RemoveItem(grid.Data, true);
                        }
                    }
                }

                if (uIIntroduction != null)
                {
                    UIComponent.Instance.Remove(UIType.UIIntroduction);
                    uIIntroduction = null;
                }
                return ETTask.CompletedTask;
            }
        }

        public void CloseWareHouse() 
        {
            Game.EventCenter.RemoveEvent<KnapsackDataItem>(EventTypeId.WARE_ADD_ITEM, AddWareHouse);
            Game.EventCenter.RemoveEvent<long>(EventTypeId.WARE_GOLDCOIN_CHANGE, ChangeCoin);
        }

    }
}
