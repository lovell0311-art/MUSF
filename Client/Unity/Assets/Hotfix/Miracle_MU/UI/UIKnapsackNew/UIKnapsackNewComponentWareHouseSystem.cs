using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace ETHotfix
{
    public static class UIKnapsackNewComponentWareHouseSystem
    {

        public static async ETTask InitWareHouse(this UIKnapsackNewComponent self)
        {
            string res = "WareHouse";
            //加载对应面板
            await ResourcesComponent.Instance.LoadGameObjectAsync(res.StringToAB(), res);
            //实例化面板 
            GameObject wareHouse = ResourcesComponent.Instance.LoadGameObject(res.StringToAB(), res);
            wareHouse.transform.SetParent(self.plane.transform, false);
            wareHouse.transform.localPosition = Vector3.zero;
            wareHouse.transform.localScale = Vector3.one;

            self.wareHouseCollector = wareHouse.GetReferenceCollector();

            
            self.WareHouseContent = self.wareHouseCollector.GetGameObject("Grids");
            self.wareCoin = self.wareHouseCollector.GetText("yuanbaoTxt");

            ///仓库中的金币
            self.wareCoin.text = $"{KnapsackItemsManager.WareGlodCoin}";

            Game.EventCenter.EventListenner<long>(EventTypeId.WARE_GOLDCOIN_CHANGE, self.ChangeCoin);
            ///存钱
            self.wareHouseCollector.GetButton("SaveMoneyBtn").onClick.AddSingleListener(() =>
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
                    if (glod > self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin))
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "输入的金币 大于 您当前所拥有的金币");
                        return;
                    }
                    G2C_AddGoldToWarehouse c_AddGoldToWarehouse = (G2C_AddGoldToWarehouse)await SessionComponent.Instance.Session.Call(new C2G_AddGoldToWarehouse
                    {
                        Gold = (int)glod
                    });
                    if (c_AddGoldToWarehouse.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, c_AddGoldToWarehouse.Error.GetTipInfo());
                    }
                    else
                    {

                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"成功存入 {glod} 金币 ");
                    }
                };
            });

            ///取钱
            self.wareHouseCollector.GetButton("TakeMoneyBtn").onClick.AddSingleListener(() =>
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
                        Gold = (int)glod
                    });
                    if (c_AddGoldToWarehouse.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, c_AddGoldToWarehouse.Error.GetTipInfo());
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"成功取出 {glod} 金币 ");
                    }
                };

            });

            ///页数
            self.pageText = self.wareHouseCollector.GetText("page");
            ///上一页
            self.wareHouseCollector.GetButton("PageUp").onClick.AddSingleListener(() => self.UpPage());
            ///下一页
            self.wareHouseCollector.GetButton("PageDown").onClick.AddSingleListener(() => self.NextPage());

            UIKnapsackNewComponent.LENGTH_WareHouse_Y = UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y;
            self.WareHouseGrids = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_WareHouse_X][];
            for (int i = 0; i < self.WareHouseGrids.Length; i++)
            {
                self.WareHouseGrids[i] = new KnapsackNewGrid[UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y];
            }

            //初始化格子
            self.CreatGrid(UIKnapsackNewComponent.LENGTH_WareHouse_X, UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y, self.WareHouseContent.transform, E_Grid_Type.Ware_House, ref self.WareHouseGrids);

            self.InitPage();

            self.Init_WareHouseItem();

            Game.EventCenter.EventListenner<KnapsackDataItem>(EventTypeId.WARE_ADD_ITEM, self.AddWareHouse);//监听 仓库新加物品
            self.isOpen = true;

        }


        private static void ChangeCoin(this UIKnapsackNewComponent self,long value)
        {
            Log.DebugBrown("当前厂库的金币数量" + value);
            self.wareCoin.text = value.ToString();
        }

        public static void ClearWareHouse(this UIKnapsackNewComponent self)
        {
            self.ReleaseGridCollectionVisuals(self.WareHouseGrids);
            if (self.wareHouseCollector)
            {
                GameObject.Destroy(self.wareHouseCollector.gameObject);
                self.wareHouseCollector = null;
            }
            
            Game.EventCenter.RemoveEvent<KnapsackDataItem>(EventTypeId.WARE_ADD_ITEM, self.AddWareHouse);//监听 仓库新加物品

            self.CleanWareHouse();

        }

        /// <summary>
        /// 上一页
        /// </summary>

        private static void UpPage(this UIKnapsackNewComponent self)
        {
            if (self.curPage == 1)//当前页数 已经是第一页了
            {
                return;
            }
            self.pageText.text = $"{--self.curPage}/{self.allPage}";
            self.CleanWareHouse();
            self.ChangeGrids(self.curPage);
            self.RefreshWareItems(self.curPage);
        }
        /// <summary>
        /// 下一页
        /// </summary>
        private static void NextPage(this UIKnapsackNewComponent self)
        {
            if (self.curPage == self.allPage)//当前页数 等于 最大页数
            {
                return;
            }
            self.pageText.text = $"{++self.curPage}/{self.allPage}";
            //清理上一页 的物品
            self.CleanWareHouse();
            self.ChangeGrids(self.curPage);
            self.RefreshWareItems(self.curPage);
        }

        /// <summary>
        /// 初始化页数
        /// </summary>
        private static void InitPage(this UIKnapsackNewComponent self)
        {
            self.PageList.Clear();
            var addpage = KnapsackItemsManager.WareHouseRows / UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y;

            /* allPage = KnapsackItemsManager.WareHouseRows / MAX_HOUSE_LENGSH_Y +
                 (addpage > 1 ? 1 : KnapsackItemsManager.WareHouseRows % MAX_HOUSE_LENGSH_Y != 0 ? 1 : 0);//得到总页数*/
           self. allPage = addpage + (KnapsackItemsManager.WareHouseRows % UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y != 0 ? 1 : 0);//得到总页数

            for (int i = 0; i < self.allPage; i++)
            {
                self.PageList.Add(new List<KnapsackDataItem>());
            }
            //页数
            self.pageText.text = $"{self.curPage}/{self.allPage}";
        }

        /// <summary>
        /// 初始化 仓库中的物品
        /// 默认显示第一页 的物品
        /// </summary>
        private static void Init_WareHouseItem(this UIKnapsackNewComponent self)
        {
            ///将物品 分页归类
            foreach (var item in KnapsackItemsManager.WareHouseItems.Values)
            {
                self.PageList[item.Page - 1].Add(item);//page-1 默认是从 0 开始
            }
            ///默认是显示第一页
            self.RefreshWareItems(1);

        }

        /// <summary>
        /// 仓库中添加物品
        /// </summary>
        /// <param name="dataItem"></param>
        public static void AddWareHouse(this UIKnapsackNewComponent self,KnapsackDataItem dataItem)
        {
            if (!self.isOpen) return;
            self.PageList[dataItem.Page - 1].Add(dataItem);//page-1 默认是从 0 开始
            self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, E_Grid_Type.Ware_House);

            self.AddItem(dataItem, type: E_Grid_Type.Ware_House);
        }

        /// <summary>
        /// 清理仓库面板
        /// </summary>
        public static async void CleanWareHouse(this UIKnapsackNewComponent self)
        {
            await CleanWareHouseAsync();

            ETTask CleanWareHouseAsync()
            {
                self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, E_Grid_Type.Ware_House);
                for (int i = 0; i < UIKnapsackNewComponent.LENGTH_WareHouse_Y; i++)
                {
                    for (int j = 0; j < UIKnapsackNewComponent.LENGTH_WareHouse_X; j++)
                    {
                        KnapsackGrid grid = self.grids[j][i];
                        if (grid.IsOccupy)
                        {
                            self.RemoveItem(grid.Data, true);
                        }
                    }
                }
                self.ClearIntroduction();
                return ETTask.CompletedTask;
            }
        }

        /// <summary>
        /// 根据页数 来显示当前页面的格子数
        /// </summary>
        /// <param name="page"></param>
        private static void ChangeGrids(this UIKnapsackNewComponent self,int page)
        {
            //LENGTH_WareHouse_Y 赋值 便于隐藏为解锁的格子
            if (page <= self.allPage - 1)
            {
                UIKnapsackNewComponent.LENGTH_WareHouse_Y = UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y;
            }
            else
            {
                UIKnapsackNewComponent.LENGTH_WareHouse_Y = KnapsackItemsManager.WareHouseRows % UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y == 0 ? UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y : KnapsackItemsManager.WareHouseRows % UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y;
            }
            for (int i = 0; i < UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y; i++)
            {
                for (int j = 0; j < UIKnapsackNewComponent.LENGTH_WareHouse_X; j++)
                {
                    self.WareHouseContent.transform.Find($"{j}_{i}").gameObject.SetActive(true);
                    if (self.WareHouseContent.transform.Find($"{j}_{i}/lock") != null)
                        self.WareHouseContent.transform.Find($"{j}_{i}/lock").gameObject.SetActive(i >= UIKnapsackNewComponent.LENGTH_WareHouse_Y);
                }
            }
        }

        /// <summary>
        /// 根据页数 刷新物品
        /// 默认是显示第一页
        /// </summary>
        /// <param name="page">页数</param>
        private static void RefreshWareItems(this UIKnapsackNewComponent self,int page = 0)
        {
            page -= 1;
            foreach (var item in self.PageList[page])
            {
                self.AddItem(item, type: E_Grid_Type.Ware_House);
            }
        }
    }
}
