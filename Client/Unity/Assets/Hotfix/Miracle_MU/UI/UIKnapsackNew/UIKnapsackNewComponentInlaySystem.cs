using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ETHotfix.UIKnapsackNewComponent;
using UnityEngine.UI;
using static NPOI.HSSF.Util.HSSFColor;

namespace ETHotfix
{
    public static class UIKnapsackNewComponentInlaySystem
    {
        public static async ETTask InitInlay(this UIKnapsackNewComponent self)
        {
            if (self.isOpenInlay)
            {
                Log.Info("自动回收界面已经打开");
                return;
            }
            self.isOpenInlay = true;
            string res = "Knapsack_InlayPanel";
            //加载对应面板
            await ResourcesComponent.Instance.LoadGameObjectAsync(res.StringToAB(), res);
            //实例化面板 
            GameObject obj = ResourcesComponent.Instance.LoadGameObject(res.StringToAB(), res);
            obj.transform.SetParent(self.plane.transform, false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            self.inlayCollector = obj.GetReferenceCollector();

            self.inlayCollector.GetButton("CloseBtn_1").onClick.AddSingleListener(self.ClearInlayPlane);
            self.InlayGridDic.Clear();

            self.InitInlayPlane();
            self.ShowPanel(E_InlayType.Info);
            self.InitInlayTog();

            self.Init_YingZhiShiHeCheng();
            self.Init_YingGuangBaoShiXiangQian();
            self.Init_YingGuangBaoShiQiangHua();
            self.Init_YingGuangBaoShiHeCheng();
            self.Init_YingGuangBaoShiChouQu();
        }

        public static void InitInlayPlane(this UIKnapsackNewComponent self)
        {
            self.InfoPanel = self.inlayCollector.GetGameObject("InfoPanel");

            self.YingZhiShiHeChengPanel = self.inlayCollector.GetGameObject("YingZhiShiHeChengPanel");
            self.YingGuangBaoShiHeChengPanel = self.inlayCollector.GetGameObject("YingGuangBaoShiHeChengPanel");
            self.YingGuangBaoShiXiangQianPanel = self.inlayCollector.GetGameObject("YingGuangBaoShiXiangQian");
            self.YingGuangBaoShiChouQuPanel = self.inlayCollector.GetGameObject("YingGuangBaoShiChouQu");
            self.YingGuangBaoShiQiangHuaPanel = self.inlayCollector.GetGameObject("YingGuangBaoShiQiangHua");
        }

        public static void InitInlayTog(this UIKnapsackNewComponent self)
        {
            Transform togs = self.inlayCollector.GetImage("Btnbg").transform;
            self.toggleGroup = togs.GetComponent<ToggleGroup>();
            for (int i = 0, length = togs.childCount; i < length; i++)
            {
                Toggle toggle = togs.GetChild(i).GetComponent<Toggle>();
                toggle.isOn = false;
                E_InlayType inlayType = (E_InlayType)i;
                toggle.onValueChanged.AddSingleListener((value) =>
                {
                    if (!value) return;
                    if (self.toggleGroup.allowSwitchOff)
                        self.toggleGroup.allowSwitchOff = false;

                    if (self.IsHaveInlayItem())
                    {
                        self.ClearInlayPanel();
                    }
                    self.InlayItemDic.Clear();

                    self.curInlayType = inlayType;

                    self.ShowPanel(inlayType);
                });

            }
        }

        public static void Init_YingZhiShiHeCheng(this UIKnapsackNewComponent self)
        {
            self.collector_YingZhiShiHeCheng = self.YingZhiShiHeChengPanel.GetReferenceCollector();
            self.InitItemGrid(self.collector_YingZhiShiHeCheng, 5, E_InlayType.YingZhiShiHeCheng, self.Glod, self.Success);
        }

        public static void Init_YingGuangBaoShiXiangQian(this UIKnapsackNewComponent self)
        {
            self.collector_YingGuangBaoShiXiangQian = self.YingGuangBaoShiXiangQianPanel.GetReferenceCollector();
            self.InitItemGrid(self.collector_YingGuangBaoShiXiangQian, 4, E_InlayType.YingGuangBaoShiXiangQian, self.Glod_XiangQian, self.Success_XiangQian);

        }

        public static void Init_YingGuangBaoShiQiangHua(this UIKnapsackNewComponent self)
        {
            self.collector_YingGuangBaoShiQiangHua = self.YingGuangBaoShiQiangHuaPanel.GetReferenceCollector();
            self.InitItemGrid(self.collector_YingGuangBaoShiQiangHua, 3, E_InlayType.YingGuangBaoShiQiangHua, self.Glod_QiangHua, self.Success_QiangHua);
            self.SecceedRateTxt = self.collector_YingGuangBaoShiQiangHua.GetText("Success");
        }

        public static void Init_YingGuangBaoShiHeCheng(this UIKnapsackNewComponent self)
        {
            self.collector_YingGuangBaoShiHeCheng = self.YingGuangBaoShiHeChengPanel.GetReferenceCollector();
            self.InitItemGrid(self.collector_YingGuangBaoShiHeCheng, 4, E_InlayType.YingGuangBaoShiHeCheng, self.Glod_HeCheng, self.Success_HeCheng);
        }

        public static void Init_YingGuangBaoShiChouQu(this UIKnapsackNewComponent self)
        {
            self.collector_YingGuangBaoShiChouQu = self.YingGuangBaoShiChouQuPanel.GetReferenceCollector();
            self.InitItemGrid(self.collector_YingGuangBaoShiChouQu, 4, E_InlayType.YingGuangBaoShiChouQu, self.Glod_ChouQu, self.Success_ChouQu);

            self.Atrs = self.collector_YingGuangBaoShiChouQu.GetGameObject("Atr").transform;
            InitAtr();

            ///初始化镶嵌属性条数
            void InitAtr()
            {
                self.AtrtoggleGroup = self.Atrs.GetComponent<ToggleGroup>();
                self.AtrtoggleGroup.allowSwitchOff = true;
                for (int i = 0, length = self.Atrs.childCount; i < length; i++)
                {
                    Toggle toggle = self.Atrs.GetChild(i).GetComponent<Toggle>();
                    int index = i + 1;
                    toggle.GetComponentInChildren<Text>().text = $"镶宝{index}：";
                    toggle.isOn = false;
                    toggle.interactable = false;
                    toggle.onValueChanged.AddSingleListener(value =>
                    {
                        if (!value) return;

                        if (self.AtrtoggleGroup.allowSwitchOff) self.AtrtoggleGroup.allowSwitchOff = false;

                        if (self.InlayItemDic.TryGetValue(1, out List<long> ItemList))
                        {
                            if (ItemList.Count == 2)
                            {
                                ItemList[1] = index;
                            }
                            else
                            {
                                ItemList.Add(index);
                            }
                        }
                    });

                }
            }
        }


        public static void ClearInlayPlane(this UIKnapsackNewComponent self)
        {
            if (self.inlayCollector != null)
            {
                GameObject.Destroy(self.inlayCollector.gameObject);
                self.inlayCollector = null;
            }
            self.isOpenInlay = false;
        }

        public static void ShowPanel(this UIKnapsackNewComponent self, E_InlayType inlayType)
        {
            self.InfoPanel.SetActive(inlayType == E_InlayType.Info);
            self.YingZhiShiHeChengPanel.SetActive(inlayType == E_InlayType.YingZhiShiHeCheng);
            self.YingGuangBaoShiHeChengPanel.SetActive(inlayType == E_InlayType.YingGuangBaoShiHeCheng);
            self.YingGuangBaoShiXiangQianPanel.SetActive(inlayType == E_InlayType.YingGuangBaoShiXiangQian);
            self.YingGuangBaoShiChouQuPanel.SetActive(inlayType == E_InlayType.YingGuangBaoShiChouQu);
            self.YingGuangBaoShiQiangHuaPanel.SetActive(inlayType == E_InlayType.YingGuangBaoShiQiangHua);
        }

        /// <summary>
        /// 初始化 物品格子
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="itemcount"></param>
        /// <param name="inlayType"></param>
        /// <param name="glod"></param>
        /// <param name="success"></param>
        public static void InitItemGrid(this UIKnapsackNewComponent self, ReferenceCollector collector, int itemcount, E_InlayType inlayType, int glod, int success)
        {
            for (int i = 1; i <= itemcount; i++)
            {
                GameObject itemGrid = collector.GetGameObject($"Item{i}");
                GameObject cell = itemGrid.transform.Find("cell").gameObject;
                int index = i;
                KnapsackNewGrid grid = new KnapsackNewGrid
                {
                    GridObj = null,
                    Image = cell.GetComponent<Image>(),
                    IsOccupy = false,
                    Grid_Type = E_Grid_Type.Inlay,
                    curCount = 0,
                    MaxCount = GetMaxCount(index)
                };
                self.InlayGridDic[$"{(int)inlayType}_{index}"] = grid;
                itemGrid.transform.Find("count").GetComponent<Text>().text = $"<color=red>{grid.curCount}</color>/{grid.MaxCount}";
                self.RegisterEvent(0, index, cell, E_Grid_Type.Inlay);
            }
            SetGlod_Success();
            ///设置金币和成功率
            void SetGlod_Success()
            {
                self.GlodCoinEnough = glod > self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin);
                string color = ColorTools.GetColorHtmlString(self.GlodCoinEnough ? Color.red : Color.white);
                collector.GetText("Glod").text = $"<color={color}>{glod:N}</color>";
                collector.GetText("Success").text = $"{success}%";
            }
            ///获取所需的最大数量
            int GetMaxCount(int index)
            {
                if (inlayType == E_InlayType.YingGuangBaoShiChouQu && index == 4)
                    return 5;
                return 1;
            }
            ///请求合成
            collector.GetButton("SureBtn").onClick.AddSingleListener(async () =>
            {
                foreach (var item in self.InlayItemDic)
                {
                    foreach (var id in item.Value)
                    {
                        Log.DebugGreen($"合成所需的物品：-{item.Key}-{item.Value.Count}-> {id}");
                    }

                }

                if (self.GlodCoinEnough)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足");
                    return;
                }
                bool isSecceed = true;
                switch (inlayType)
                {
                    case E_InlayType.YingZhiShiHeCheng:

                        IsResEnough(5);
                        G2C_FluoreStoneCompose g2C_FluoreStone = (G2C_FluoreStoneCompose)await SessionComponent.Instance.Session.Call(new C2G_FluoreStoneCompose
                        {
                            Add4SetEquipItemUID = self.GetInlayItem(1),
                            Add4ExcEquipItemUID = self.GetInlayItem(2),
                            RecycledGemsItemUID = self.GetInlayItem(3),
                            CreateGemsItemUID = self.GetInlayItem(4),
                            MayaGemsItemUID = self.GetInlayItem(5),

                        });
                        if (g2C_FluoreStone.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_FluoreStone.Error.GetTipInfo());
                            isSecceed = false;
                        }
                        break;
                    case E_InlayType.YingGuangBaoShiHeCheng:

                        IsResEnough(4);
                        G2C_FluoreGemsCompose g2C_FluoreGemsCompose = (G2C_FluoreGemsCompose)await SessionComponent.Instance.Session.Call(new C2G_FluoreGemsCompose
                        {
                            FluoreStoneItemUID = self.GetInlayItem(1),
                            LightGemsItemUID = self.GetInlayItem(2),
                            CreateGemsItemUID = self.GetInlayItem(3),
                            MayaGemsItemUID = self.GetInlayItem(4)
                        });
                        if (g2C_FluoreGemsCompose.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_FluoreGemsCompose.Error.GetTipInfo());
                            isSecceed = false;
                        }
                        break;
                    case E_InlayType.YingGuangBaoShiXiangQian:

                        IsResEnough(4);
                        G2C_FluoreGemsSet g2C_FluoreGemsSet = (G2C_FluoreGemsSet)await SessionComponent.Instance.Session.Call(new C2G_FluoreGemsSet
                        {
                            EquipItemUID = self.GetInlayItem(1),
                            FluoreGemsItemUID = self.GetInlayItem(2),
                            CreateGemsItemUID = self.GetInlayItem(3),
                            MayaGemsItemUID = self.GetInlayItem(4),
                        });
                        if (g2C_FluoreGemsSet.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_FluoreGemsSet.Error.GetTipInfo());
                            isSecceed = false;
                        }
                        break;
                    case E_InlayType.YingGuangBaoShiChouQu:

                        IsResEnough(4);
                        if (self.GetInlayItemList(1).Count != 2)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "请选择要提取的卡槽");
                            return;
                        }
                        G2C_FluoreGemsRecover g2C_FluoreGemsRecover = (G2C_FluoreGemsRecover)await SessionComponent.Instance.Session.Call(new C2G_FluoreGemsRecover
                        {
                            EquipItemUID = self.GetInlayItemList(1)[0],//有镶宝属性的装备
                            MountID = (int)self.GetInlayItemList(1)[1],//第几个插槽
                            GuardianGemsItemUID = self.GetInlayItem(2),//守护宝石
                            RecycledGemsItemUID = self.GetInlayItem(3),//再生宝石
                            MayaGemsItemUID = self.GetInlayItem(4),//玛雅宝石x5
                        });
                        if (g2C_FluoreGemsRecover.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_FluoreGemsRecover.Error.GetTipInfo());
                            isSecceed = false;
                        }
                        break;
                    case E_InlayType.YingGuangBaoShiQiangHua:

                        G2C_FluoreGemsStrengthen g2C_FluoreGemsStrengthen = (G2C_FluoreGemsStrengthen)await SessionComponent.Instance.Session.Call(new C2G_FluoreGemsStrengthen
                        {
                            MainFluoreGemsItemUID = self.GetInlayItem(1),//强化主荧光宝石(属性)
                            FluoreGemsItemUID = self.GetInlayItem(2),//同级材料荧光宝石(属性)
                            LightGemsRuneItemUID = self.GetInlayItem(3),//光之石强化符文
                        });
                        if (g2C_FluoreGemsStrengthen.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_FluoreGemsStrengthen.Error.GetTipInfo());
                            isSecceed = false;
                        }
                        break;
                }
                if (isSecceed)
                {
                    //成功后 则移除背包中的物品
                    self.ClearInlayPanel();
                    self.InlayItemDic.Clear();
                }

                ///所需材料是否满足条件
                void IsResEnough(int count)
                {


                    if (self.InlayItemDic.Count != count)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "物品材料不足");
                        return;
                    }
                }
            });
        }


        public static long GetInlayItem(this UIKnapsackNewComponent self, int Index)
        {
            if (self.InlayItemDic.TryGetValue(Index, out List<long> ItemList))
            {
                if (ItemList.Count != 0)
                    return ItemList.First();
            }
            return 0;
        }

        /// <summary>
        /// 根据序号获取镶嵌 物品的UUID集合
        /// </summary>
        /// <param name="Index">物品序号 从0开始的</param>
        /// <returns></returns>
        public static List<long> GetInlayItemList(this UIKnapsackNewComponent self, int Index)
        {
            if (self.InlayItemDic.TryGetValue(Index, out List<long> ItemList))
            {
                return ItemList.Count != 0 ? ItemList : null;
            }
            return null;
        }


        // <summary>
        /// 检查镶嵌 面板上是否有镶嵌物品
        /// </summary>
        /// <returns></returns>
        public static bool IsHaveInlayItem(this UIKnapsackNewComponent self)
        {
            return self.InlayItemDic.Count != 0;
        }

        /// <summary>
        /// 清除镶嵌面板
        /// </summary>
        public static void ClearInlayPanel(this UIKnapsackNewComponent self)
        {
            if (self.InlayGridDic.Count == 0) return;
            self.ResetAtrs();
            for (int i = 0, length = self.InlayGridDic.Count; i < length; i++)
            {
                KnapsackNewGrid grid = self.InlayGridDic.ElementAt(i).Value;
                if (!grid.IsOccupy) continue;
                //GameObject obj = grid.GridObj;
                //grid.IsOccupy = false;
                //grid.curCount = 0;
                //GameObject objparent = grid.Image.transform.parent.gameObject;
                //string color = ColorTools.GetColorHtmlString(grid.curCount >= grid.MaxCount ? Color.green : Color.red);
                //objparent.transform.Find("count").GetComponent<Text>().text = $"<color={color}>{grid.curCount}</color>/{grid.MaxCount}";
                //ResourcesComponent.Instance.RecycleGameObject(obj);
                grid.Clear();

                //ResourcesComponent.Instance.DestoryGameObjectImmediate(obj, obj.name.StringToAB());
            }
        }

        /// <summary>
        /// 重置 装备的镶嵌属性
        /// </summary>
        public static void ResetAtrs(this UIKnapsackNewComponent self)
        {
            self.AtrtoggleGroup.allowSwitchOff = true;
            for (int i = 0, length = self.Atrs.childCount; i < length; i++)
            {
                Toggle toggle = self.Atrs.GetChild(i).GetComponent<Toggle>();
                toggle.GetComponentInChildren<Text>().text = $"镶宝{i + 1}：";
                toggle.isOn = false;
                toggle.interactable = false;
            }
        }

        public static void OnPointerEnter_Inlay(this UIKnapsackNewComponent self, E_Grid_Type part, int index)
        {
            if (self.InlayGridDic.ContainsKey($"{(int)self.curInlayType}_{index}") == false) return;
            self.curChooseArea.Grid_Type = part;//当前选择的格子的类型
            self.CurInlayGrid = self.InlayGridDic[$"{(int)self.curInlayType}_{index}"];
            self.CurInlayGridName = $"{(int)self.curInlayType}_{index}";
            self.CurInlayGridIndex = index;
            if (!self.isDroping) return;
            self.CurInlayGrid.ReadyColor();
            Log.DebugBrown($"进入镶嵌 起始格子类型:{self.originArea.Grid_Type}  目标格子类型:{self.curChooseArea.Grid_Type} index:{index} ");
        }

        public static void OnPointerExit_Inlay(this UIKnapsackNewComponent self)
        {
            if (self.CurInlayGrid == null) return;
            self.curChooseArea.Grid_Type = E_Grid_Type.None;//当前选择的格子类型为None（即没有进入区域格子）
            self.CurInlayGrid.ResetColor();
        }

        public static void OnPointerClickEvent_Inlay(this UIKnapsackNewComponent self, int index)
        {
            self.RemoveGridObj();

        }

        public static void RemoveGridObj(this UIKnapsackNewComponent self)
        {
            if (self.CurInlayGrid.GridObj != null)
            {
                self.RemoveInlayItem(); ///移除已经添加的材料
                self.ChangeCountText(self.CurInlayGrid.Data.ItemData.GetProperValue(E_ItemValue.Quantity) * -1);


                if (self.curInlayType == E_InlayType.YingGuangBaoShiHeCheng && self.CurInlayGridIndex == 1)
                    self.ChangeTitleText("荧之石");
                else if (self.curInlayType == E_InlayType.YingGuangBaoShiChouQu && self.CurInlayGridIndex == 1)
                    self.ResetAtrs();
                else if (self.curInlayType == E_InlayType.YingGuangBaoShiQiangHua)
                {
                    if (self.CurInlayGridIndex == 1)
                    {
                        self.ChangeSecced(0);
                        self.ChangeTitleText("荧光宝石");
                    }
                    else if (self.CurInlayGridIndex == 2)
                    {
                        self.ChangeTitleText("同级荧光宝石");
                    }
                }

                ///当前数量为0
                if (self.CurInlayGrid.curCount == 0)
                {
                    self.CurInlayGrid.IsOccupy = false;
                    ResourcesComponent.Instance.RecycleGameObject(self.CurInlayGrid.GridObj);

                    //ResourcesComponent.Instance.DestoryGameObjectImmediate(CurInlayGrid.GridObj, CurInlayGrid.GridObj.name.StringToAB());
                }
            }
        }

        //移除镶嵌材料
        public static void RemoveInlayItem(this UIKnapsackNewComponent self)
        {
            if (self.InlayItemDic.TryGetValue(self.CurInlayGridIndex, out List<long> ItemList))
            {
                ///移除后添加的物品UUID
                ItemList.Clear();
            }

        }

        /// <summary>
        /// 改变数量文本
        /// </summary>
        /// <param name="index"></param>
        public static void ChangeCountText(this UIKnapsackNewComponent self,int count = 1)
        {

            Log.DebugGreen($"改变数量：{count}  CurInlayGridName：{self.CurInlayGridName}");
            if (self.InlayGridDic.TryGetValue(self.CurInlayGridName, out KnapsackNewGrid grid))
            {
                grid.curCount += count;
                GameObject obj = grid.Image.transform.parent.gameObject;
                string color = ColorTools.GetColorHtmlString(grid.curCount >= grid.MaxCount ? Color.green : Color.red);
                obj.transform.Find("count").GetComponent<Text>().text = $"<color={color}>{grid.curCount}</color>/{grid.MaxCount}";
            }
        }

        /// <summary>
        /// 改变物品的名字
        /// </summary>
        /// <param name="title"></param>
        public static void ChangeTitleText(this UIKnapsackNewComponent self, string title)
        {
            if (self.InlayGridDic.TryGetValue(self.CurInlayGridName, out KnapsackNewGrid grid))
            {
                GameObject obj = grid.Image.transform.parent.gameObject;
                obj.transform.Find("title").GetComponent<Text>().text = title;
            }

        }

        /// <summary>
        /// 改变合成率
        /// </summary>
        public static void ChangeSecced(this UIKnapsackNewComponent self,int seccedRate)
        {

            self.SecceedRateTxt.text = $"{seccedRate}%";

        }


        public static void OnPackageDrogToInlayPlane(this UIKnapsackNewComponent self)
        {
            //if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
            //{
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法镶嵌");
            //    self.ResetGridObj();
            //}
           // else
            if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法镶嵌");
                self.ResetGridObj();
            }
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用状态 无法镶嵌");
                self.ResetGridObj();
            }
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定状态 无法镶嵌");
                self.ResetGridObj();
            }
            else
            {

                if (self.IsUsedItem())//物品是否已经添加
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "已经使用了该物品");
                    self.ResetGridObj();
                    return;
                }
                if (self.CheckItem(self.originArea.ItemData))//检查物品是否符合条件
                {
                    if (self.CurInlayGrid.IsOccupy)//当前格子已经有物品
                    {
                        if (self.CurInlayGrid.curCount == self.CurInlayGrid.MaxCount)//当前所需要的材料 以达到最大数量
                        {
                            //满足条件 直接替换
                            self.RemoveInlayItem();
                            ResourcesComponent.Instance.RecycleGameObject(self.CurInlayGrid.GridObj);
                            //ResourcesComponent.Instance.DestoryGameObjectImmediate(CurInlayGrid.GridObj, CurInlayGrid.GridObj.name.StringToAB());//删除之前的
                            self.AddInlayItem(self.originArea, ResourcesComponent.Instance.LoadGameObject(self.curDropObj.name.StringToAB(), self.curDropObj.name));
                        }

                    }
                    else
                    {
                        //满足条件
                        self.AddInlayItem(self.originArea, ResourcesComponent.Instance.LoadGameObject(self.curDropObj.name.StringToAB(), self.curDropObj.name));
                        //改变 下方的数量显示
                        self.ChangeCountText(self.originArea.ItemData.GetProperValue(E_ItemValue.Quantity));

                    }

                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品不满足条件");
                }
                //物品返回背包原位置
                self.ResetGridObj();
            }

        }
        /// <summary>
        /// 是否已经使用了改物品
        /// </summary>
        /// <returns>true 已经使用了该物品</returns>
        public static bool IsUsedItem(this UIKnapsackNewComponent self)
        {
            var Items = self.InlayItemDic.Values.ToList();
            foreach (var item in Items)
            {
                if (item.Exists(r => r == self.originArea.UUID))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查装备 是否满足条件
        /// </summary>
        /// <param name="knapsackDataItem">装备数据</param>
        /// <returns>true 满足条件 false 不满足条件</returns>
        public static bool CheckItem(this UIKnapsackNewComponent self,KnapsackDataItem knapsackDataItem)
        {
            switch (self.curInlayType)
            {
                case E_InlayType.YingZhiShiHeCheng:
                    switch (self.CurInlayGridIndex)
                    {
                        case 1://+4以上套装装备UID
                            return IsSuitItem();
                        case 2://+4以上卓越装备UID
                            return IsExcellence();
                        case 3://再生宝石
                            return IsZaiShengGem();
                        case 4://创造宝石
                            return IsChuangZaoGem();
                        case 5://玛雅宝石
                            return IsMaYaGem();
                    }
                    break;
                case E_InlayType.YingGuangBaoShiHeCheng:
                    switch (self.CurInlayGridIndex)
                    {
                        case 1://荧之石(属性)
                            self.ChangeTitleText(IsYingZhiShi().Item2);
                            return IsYingZhiShi().Item1;
                        case 2://光之石
                            return IsGuangZhiShi();
                        case 3://创造宝石
                            return IsChuangZaoGem();
                        case 4://玛雅宝石
                            return IsMaYaGem();
                    }
                    break;
                case E_InlayType.YingGuangBaoShiXiangQian:
                    switch (self.CurInlayGridIndex)
                    {
                        case 1://有插槽的装备
                            return IsHaveSlot();
                        case 2://荧光宝石(属性)
                            self.ChangeTitleText(IsYingGuangBaoShi().Item2);
                            return IsYingGuangBaoShi().Item1;
                        case 3://创造宝石
                            return IsChuangZaoGem();
                        case 4://玛雅宝石
                            return IsMaYaGem();
                    }
                    break;
                case E_InlayType.YingGuangBaoShiChouQu:
                    switch (self.CurInlayGridIndex)
                    {
                        case 1://有镶宝属性的装备
                            return IsHaveInlayGem();
                        case 2://守护宝石
                            return IsShouHuGem();
                        case 3://再生宝石
                            return IsZaiShengGem();
                        case 4://玛雅宝石 x5
                            return IsMaYaGem(5);
                    }
                    break;
                case E_InlayType.YingGuangBaoShiQiangHua:
                    switch (self.CurInlayGridIndex)
                    {
                        case 1://强化主荧光宝石(属性)
                            self.ChangeTitleText(IsHaveYingGuangBaoShi().Item2);
                            self.ChangeSecced(self.GetRate(knapsackDataItem.GetProperValue(E_ItemValue.Level)));//改变 合成概率
                            return IsHaveYingGuangBaoShi().Item1;
                        case 2://级材料荧光宝石(属性)
                            self.ChangeTitleText(IsEquallevYingGuangBaoShi().Item2);
                            return IsEquallevYingGuangBaoShi().Item1;
                        case 3://光之石强化符文
                            return IsGuangZhiShiQiangHuaFuWen();
                    }
                    break;
            }
            return false;


            ///是否石+4以上的套装装备
            bool IsSuitItem()
            {
                bool isSuit = false;
                if (knapsackDataItem.GetProperValue(E_ItemValue.Level) >= 4 && knapsackDataItem.GetProperValue(E_ItemValue.SetId) != 0)
                {
                    isSuit = true;

                    if (IsInlayYingGuangGem())
                    {
                        UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                        confirmComponent.SetTipText("装备已经镶嵌荧光宝石，是否继续操作？");
                        confirmComponent.AddCancelEventAction(() =>
                        {
                            self.RemoveGridObj();
                        });
                    }
                }
                return isSuit;
            }
            ///是否是卓越装备
            bool IsExcellence()
            {
                if (knapsackDataItem.GetProperValue(E_ItemValue.Level) >= 4 && knapsackDataItem.IsHaveExecllentEntry)
                {
                    return true;
                }
                return false;
            }
            ///是否是再生宝石
            bool IsZaiShengGem()
            {
                return knapsackDataItem.ConfigId == GemItemConfigId.RECYCLED_GEMS;
            }
            ///是否是创造宝石
            bool IsChuangZaoGem()
            {
                return knapsackDataItem.ConfigId == GemItemConfigId.CREATE_GEMS;
            }
            ///是否是玛雅宝石
            bool IsMaYaGem(int count = 1)
            {
                return knapsackDataItem.ConfigId == GemItemConfigId.MAYA_GEMS && knapsackDataItem.GetProperValue(E_ItemValue.Quantity) >= count;
            }
            ///是否是荧之石
            (bool, string) IsYingZhiShi()
            {
                if (GemItemConfigId.YING_ZHI_SHI_DIC.TryGetValue(knapsackDataItem.ConfigId, out string name))
                    return (true, name);
                return (false, "荧之石");
            }
            ///是否是守护宝石
            bool IsShouHuGem()
            {
                return knapsackDataItem.ConfigId == GemItemConfigId.GUARDIAN_GEMS;
            }
            ///是否是荧光宝石
            (bool, string) IsYingGuangBaoShi()
            {
                if (GemItemConfigId.YING_GUANG_GEM_DIC.TryGetValue(knapsackDataItem.ConfigId, out string name))
                    return (true, name);
                return (false, "荧光宝石");
            }
            ///光之石
            bool IsGuangZhiShi()
            {
                return knapsackDataItem.ConfigId == GemItemConfigId.GUANG_ZHI_SHI;
            }
            ///是否已经镶嵌荧光宝石
            bool IsInlayYingGuangGem()
            {
                return knapsackDataItem.GetProperValue(E_ItemValue.FluoreSlotCount) > 0;
            }
            ///是否有插槽
            bool IsHaveSlot()
            {
                return knapsackDataItem.GetProperValue(E_ItemValue.FluoreSlotCount) != 0;
            }
            ///是否有镶嵌宝石属性
            bool IsHaveInlayGem()
            {
                self.ResetAtrs();
                bool isHave = false;
                if (knapsackDataItem.GetProperValue(E_ItemValue.FluoreSlotCount) is int count && count > 0)//有卡槽
                {
                    for (int i = 0; i < count; i++)
                    {
                        var PropId = knapsackDataItem.GetProperValue(E_ItemValue.FluoreSlot1 + i);
                        if (PropId == 0)
                        {
                            continue;
                        }
                        isHave = true;
                        self.SetEquipAtr(knapsackDataItem, i, PropId);
                    }
                }
                return isHave;
            }

            ///主荧光宝石（属性）
            (bool, string) IsHaveYingGuangBaoShi()
            {
                if (GemItemConfigId.YING_GUANG_GEM_DIC.TryGetValue(knapsackDataItem.ConfigId, out string name))
                {

                    return (true, $"{name}+{knapsackDataItem.GetProperValue(E_ItemValue.Level)}");
                }
                return (false, "荧光宝石");
            }

            ///是否是同级荧光宝石(属性)
            (bool, string) IsEquallevYingGuangBaoShi()
            {
                (bool ishave, string gemName) result = (false, "同级荧光宝石");
                if (self.InlayGridDic.TryGetValue($"{(int)self.curInlayType}_{1}", out KnapsackNewGrid grid))//前一个格子 是否已经添加荧光宝石
                {
                    if (knapsackDataItem.GetProperValue(E_ItemValue.Level) == grid.Data.ItemData.GetProperValue(E_ItemValue.Level))//两个等级是否相等
                    {
                        if (GemItemConfigId.YING_GUANG_GEM_DIC.TryGetValue(knapsackDataItem.ConfigId, out string name))
                        {
                            result = (true, $"{name}+{knapsackDataItem.GetProperValue(E_ItemValue.Level)}");
                        }
                    }
                }
                return result;
            }
            ///是否是光之石 强化符文
            bool IsGuangZhiShiQiangHuaFuWen()
            {
                return knapsackDataItem.ConfigId == GemItemConfigId.GUANG_ZHI_SHI_QIANGHUA_FUWEN;
            }

        }
        /// <summary>
        /// 设置卡槽的荧光宝石属性
        /// </summary>
        /// <param name="index"></param>
        /// <param name="PropId"></param>
        public static void SetEquipAtr(this UIKnapsackNewComponent self, KnapsackDataItem dataItem, int index, long PropId)
        {
            //id:value/100  level:value%100
            FluoreSet_AttrConfig fluoreSet_AttrConfig = ConfigComponent.Instance.GetItem<FluoreSet_AttrConfig>((int)(PropId / 100));//获取荧光宝石 属性配置表
            self.Atrs.GetChild(index).GetComponentInChildren<Text>().text = $"镶宝{index + 1}:{dataItem.GetYingGuangBaoShiAtr(fluoreSet_AttrConfig.fluore)}({string.Format(fluoreSet_AttrConfig.Info, dataItem.GetAtrValue(fluoreSet_AttrConfig, (int)(PropId % 100)))})";
            self.Atrs.GetChild(index).GetComponent<Toggle>().interactable = true;
        }
        /// <summary>
        /// 获取成功率
        /// </summary>
        /// <param name="lev"></param>
        /// <returns></returns>
        public static int GetRate(this UIKnapsackNewComponent self, int lev) => lev switch
        {
            0 => 100,
            1 => 90,
            2 => 80,
            3 => 70,
            4 => 60,
            5 => 50,
            6 => 40,
            7 => 30,
            8 => 20,
            _ => 0

        };

        /// <summary>
        /// 添加镶嵌 材料物品
        /// </summary>
        /// <param name="index"></param>
        /// <param name="gridData"></param>
        /// <param name="obj"></param>
        public static void AddInlayItem(this UIKnapsackNewComponent self, KnapsackGridData gridData, GameObject obj)
        {
            obj.SetUI(gridData.ItemData.GetProperValue(E_ItemValue.Level));
            if (self.InlayGridDic.TryGetValue(self.CurInlayGridName, out KnapsackNewGrid grid))
            {
                grid.Data.Point1 = new Vector2Int(gridData.Point1.x, gridData.Point1.y);
                grid.Data.Point2 = new Vector2Int(gridData.Point2.x, gridData.Point2.y);
                grid.Data.UUID = gridData.UUID;
                grid.IsOccupy = true;
                grid.GridObj = obj;
                grid.Data.ItemData = gridData.ItemData;
                obj.transform.position = new Vector3(grid.Image.transform.position.x, grid.Image.transform.position.y, 10);
                self.AddInlayItem(gridData.UUID);
                Log.DebugGreen($"物品位置设置完成：{grid.GridObj.name}");
            }
        }
        ///添加镶嵌 材料
        public static void AddInlayItem(this UIKnapsackNewComponent self, long uuid)
        {
            if (self.InlayItemDic.ContainsKey(self.CurInlayGridIndex))
            {
                self.InlayItemDic[self.CurInlayGridIndex].Add(uuid);
            }
            else
            {
                self.InlayItemDic[self.CurInlayGridIndex] = new List<long> { };
                self.InlayItemDic[self.CurInlayGridIndex].Add(uuid);
            }
            Log.DebugBrown($"添加后的数量：{self.InlayItemDic.Count}");
        }
    }
}
