using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using ILRuntime.Runtime;

using System.Linq;



namespace ETHotfix
{

    /// <summary>
    /// 镶嵌模块
    /// </summary>
    public partial class UIKnapsackComponent
    {
        enum E_InlayType
        {
            YingZhiShiHeCheng,
            YingGuangBaoShiHeCheng,
            YingGuangBaoShiXiangQian,
            YingGuangBaoShiChouQu,
            YingGuangBaoShiQiangHua,
            Info
        }

        public ReferenceCollector collector_Inlay;
        ToggleGroup toggleGroup;

        public GameObject YingZhiShiHeChengPanel, YingGuangBaoShiHeChengPanel, YingGuangBaoShiXiangQianPanel, YingGuangBaoShiChouQuPanel, YingGuangBaoShiQiangHuaPanel, InfoPanel;
        /// <summary>
        /// 镶嵌面板上的 格子字典
        /// key:格子的名字
        /// value:格子
        /// </summary>
        Dictionary<string, KnapsackGrid> InlayGridDic = new Dictionary<string, KnapsackGrid>();
        /// <summary>
        ///镶嵌所需的物品字典
        ///key:物品序号
        ///value:物品的UUID集合
        /// </summary>
        Dictionary<int, List<long>> InlayItemDic = new Dictionary<int, List<long>>();
        /// <summary>
        /// 当前所选的 镶嵌类别
        /// </summary>
        E_InlayType curInlayType;
        KnapsackGrid CurInlayGrid;//当前所选的格子
        string CurInlayGridName;//当前格子的名字
        int CurInlayGridIndex;//当前格子的索引
        bool GlodCoinEnough;//金币是否足够
      
        /// <summary>
        /// 初始化镶嵌面板
        /// </summary>
        public void Init_Inlay()
        {
            collector_Inlay = InlayPanel.GetReferenceCollector();
            collector_Inlay.GetButton("CloseBtn_1").onClick.AddSingleListener(CloseKnapsack);
            InlayGridDic.Clear();

            InitPanel();
            ShowPanel(E_InlayType.Info);
            InitInlayTog();
            Init_YingZhiShiHeCheng();
            Init_YingGuangBaoShiXiangQian();
            Init_YingGuangBaoShiQiangHua();
            Init_YingGuangBaoShiHeCheng();
            Init_YingGuangBaoShiChouQu();

            void InitInlayTog()
            {
                Transform togs = collector_Inlay.GetImage("Btnbg").transform;
                toggleGroup = togs.GetComponent<ToggleGroup>();
                for (int i = 0, length = togs.childCount; i < length; i++)
                {
                    Toggle toggle = togs.GetChild(i).GetComponent<Toggle>();
                    toggle.isOn = false;
                    E_InlayType inlayType = (E_InlayType)i;
                    toggle.onValueChanged.AddSingleListener((value) =>
                    {
                        if (!value) return;
                        if (toggleGroup.allowSwitchOff)
                            toggleGroup.allowSwitchOff = false;

                        if (IsHaveInlayItem())
                        {
                            ClearInlayPanel();
                        }
                        InlayItemDic.Clear();

                        curInlayType = inlayType;

                        ShowPanel(inlayType);
                    });

                }
            }


            void InitPanel()
            {
                InfoPanel = collector_Inlay.GetGameObject("InfoPanel");
                YingZhiShiHeChengPanel = collector_Inlay.GetGameObject("YingZhiShiHeChengPanel");
                YingGuangBaoShiHeChengPanel = collector_Inlay.GetGameObject("YingGuangBaoShiHeChengPanel");
                YingGuangBaoShiXiangQianPanel = collector_Inlay.GetGameObject("YingGuangBaoShiXiangQian");
                YingGuangBaoShiChouQuPanel = collector_Inlay.GetGameObject("YingGuangBaoShiChouQu");
                YingGuangBaoShiQiangHuaPanel = collector_Inlay.GetGameObject("YingGuangBaoShiQiangHua");
            }


            void ShowPanel(E_InlayType inlayType)
            {
                InfoPanel.SetActive(inlayType == E_InlayType.Info);
                YingZhiShiHeChengPanel.SetActive(inlayType == E_InlayType.YingZhiShiHeCheng);
                YingGuangBaoShiHeChengPanel.SetActive(inlayType == E_InlayType.YingGuangBaoShiHeCheng);
                YingGuangBaoShiXiangQianPanel.SetActive(inlayType == E_InlayType.YingGuangBaoShiXiangQian);
                YingGuangBaoShiChouQuPanel.SetActive(inlayType == E_InlayType.YingGuangBaoShiChouQu);
                YingGuangBaoShiQiangHuaPanel.SetActive(inlayType == E_InlayType.YingGuangBaoShiQiangHua);
            }

        }
        /// <summary>
        /// 初始化 物品格子
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="itemcount"></param>
        /// <param name="inlayType"></param>
        /// <param name="glod"></param>
        /// <param name="success"></param>
        void InitItemGrid(ReferenceCollector collector, int itemcount, E_InlayType inlayType, int glod, int success)
        {
            for (int i = 1; i <= itemcount; i++)
            {
                GameObject itemGrid = collector.GetGameObject($"Item{i}");
                GameObject cell = itemGrid.transform.Find("cell").gameObject;
                int index = i;
                KnapsackGrid grid = new KnapsackGrid
                {
                    GridObj = null,
                    Image = cell.GetComponent<Image>(),
                    IsOccupy = false,
                    Grid_Type = E_Grid_Type.Inlay,
                    curCount = 0,
                    MaxCount = GetMaxCount(index)
                };
                InlayGridDic[$"{(int)inlayType}_{index}"] = grid;
                itemGrid.transform.Find("count").GetComponent<Text>().text = $"<color=red>{grid.curCount}</color>/{grid.MaxCount}";
                RegisterEvent(0, index, cell, E_Grid_Type.Inlay);
            }
            SetGlod_Success();
            ///设置金币和成功率
            void SetGlod_Success()
            {
                GlodCoinEnough = glod > roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin);
                string color = ColorTools.GetColorHtmlString(GlodCoinEnough ? Color.red : Color.white);
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
                foreach (var item in InlayItemDic)
                {
                    foreach (var id in item.Value)
                    {
                        Log.DebugGreen($"合成所需的物品：-{item.Key}-{item.Value.Count}-> {id}");
                    }

                }

                if (GlodCoinEnough)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"金币不足");
                    return;
                }
                bool isSecceed = true;
                switch (inlayType)
                {
                    case E_InlayType.YingZhiShiHeCheng:
                      
                        IsResEnough(5);
                        G2C_FluoreStoneCompose g2C_FluoreStone = (G2C_FluoreStoneCompose)await SessionComponent.Instance.Session.Call(new C2G_FluoreStoneCompose 
                        {
                            Add4SetEquipItemUID= GetInlayItem(1),
                            Add4ExcEquipItemUID =GetInlayItem(2),
                            RecycledGemsItemUID = GetInlayItem(3),
                            CreateGemsItemUID= GetInlayItem(4),
                            MayaGemsItemUID=GetInlayItem(5),
                        
                        });
                        if (g2C_FluoreStone.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_FluoreStone.Error.GetTipInfo());
                            isSecceed = false;
                        }
                        break;
                    case E_InlayType.YingGuangBaoShiHeCheng:
                       
                        IsResEnough(4);
                        G2C_FluoreGemsCompose g2C_FluoreGemsCompose = (G2C_FluoreGemsCompose) await SessionComponent.Instance.Session.Call(new C2G_FluoreGemsCompose 
                        {
                        FluoreStoneItemUID = GetInlayItem(1),
                            LightGemsItemUID = GetInlayItem(2),
                            CreateGemsItemUID = GetInlayItem(3),
                            MayaGemsItemUID = GetInlayItem(4)
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
                            EquipItemUID = GetInlayItem(1),
                            FluoreGemsItemUID = GetInlayItem(2),
                            CreateGemsItemUID = GetInlayItem(3),
                            MayaGemsItemUID = GetInlayItem(4),
                        });
                        if (g2C_FluoreGemsSet.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_FluoreGemsSet.Error.GetTipInfo());
                            isSecceed = false;
                        }
                        break;
                    case E_InlayType.YingGuangBaoShiChouQu:
                      
                        IsResEnough(4);
                        if (GetInlayItemList(1).Count != 2)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint,"请选择要提取的卡槽");
                            return;
                        }
                        G2C_FluoreGemsRecover g2C_FluoreGemsRecover = (G2C_FluoreGemsRecover)await SessionComponent.Instance.Session.Call(new C2G_FluoreGemsRecover 
                        {
                            EquipItemUID = GetInlayItemList(1)[0],//有镶宝属性的装备
                            MountID = GetInlayItemList(1)[1].ToInt32(),//第几个插槽
                            GuardianGemsItemUID = GetInlayItem(2),//守护宝石
                            RecycledGemsItemUID = GetInlayItem(3),//再生宝石
                            MayaGemsItemUID = GetInlayItem(4),//玛雅宝石x5
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
                            MainFluoreGemsItemUID = GetInlayItem(1),//强化主荧光宝石(属性)
                            FluoreGemsItemUID = GetInlayItem(2),//同级材料荧光宝石(属性)
                            LightGemsRuneItemUID = GetInlayItem(3),//光之石强化符文
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
                    ClearInlayPanel();
                    InlayItemDic.Clear();
                }

                ///所需材料是否满足条件
                void IsResEnough(int count)
                {
                   
                
                    if (InlayItemDic.Count != count)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "物品材料不足");
                        return;
                    }
                }
            });
        }

        /// <summary>
        /// 检查装备 是否满足条件
        /// </summary>
        /// <param name="knapsackDataItem">装备数据</param>
        /// <returns>true 满足条件 false 不满足条件</returns>
        public bool CheckItem(KnapsackDataItem knapsackDataItem)
        {
            switch (curInlayType)
            {
                case E_InlayType.YingZhiShiHeCheng:
                    switch (CurInlayGridIndex)
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
                    switch (CurInlayGridIndex)
                    {
                        case 1://荧之石(属性)
                            ChangeTitleText(IsYingZhiShi().Item2);
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
                    switch (CurInlayGridIndex)
                    {
                        case 1://有插槽的装备
                            return IsHaveSlot();
                        case 2://荧光宝石(属性)
                            ChangeTitleText(IsYingGuangBaoShi().Item2);
                            return IsYingGuangBaoShi().Item1;
                        case 3://创造宝石
                            return IsChuangZaoGem();
                        case 4://玛雅宝石
                            return IsMaYaGem();
                    }
                    break;
                case E_InlayType.YingGuangBaoShiChouQu:
                    switch (CurInlayGridIndex)
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
                    switch (CurInlayGridIndex)
                    {
                        case 1://强化主荧光宝石(属性)
                            ChangeTitleText(IsHaveYingGuangBaoShi().Item2);
                            ChangeSecced(GetRate(knapsackDataItem.GetProperValue(E_ItemValue.Level)));//改变 合成概率
                            return IsHaveYingGuangBaoShi().Item1;
                        case 2://级材料荧光宝石(属性)
                            ChangeTitleText(IsEquallevYingGuangBaoShi().Item2);
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
                if (knapsackDataItem.GetProperValue(E_ItemValue.Level) >= 4&&knapsackDataItem.GetProperValue(E_ItemValue.SetId)!=0)
                {
                    isSuit = true;

                    if (IsInlayYingGuangGem())
                    {
                        UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                        confirmComponent.SetTipText("装备已经镶嵌荧光宝石，是否继续操作？");
                        confirmComponent.AddCancelEventAction(() =>
                        {
                            RemoveGridObj();
                        });
                    }
                }
                return isSuit;
            }
            ///是否是卓越装备
            bool IsExcellence()
            {
                if (knapsackDataItem.GetProperValue(E_ItemValue.Level) >= 4&&knapsackDataItem.IsHaveExecllentEntry)
                {
                    return true;
                }
                return false;
            }
            ///是否是再生宝石
            bool IsZaiShengGem()
            {
                return knapsackDataItem.ConfigId ==GemItemConfigId.RECYCLED_GEMS.ToInt64();
            }
            ///是否是创造宝石
            bool IsChuangZaoGem()
            {
                return knapsackDataItem.ConfigId == GemItemConfigId.CREATE_GEMS.ToInt64();
            }
            ///是否是玛雅宝石
            bool IsMaYaGem(int count=1)
            {
                return knapsackDataItem.ConfigId == GemItemConfigId.MAYA_GEMS.ToInt64()&&knapsackDataItem.GetProperValue(E_ItemValue.Quantity)>= count;
            }
            ///是否是荧之石
            (bool,string) IsYingZhiShi() 
            {
                if (GemItemConfigId.YING_ZHI_SHI_DIC.TryGetValue(knapsackDataItem.ConfigId, out string name))
                    return (true, name);
                return (false, "荧之石");
            }
            ///是否是守护宝石
            bool IsShouHuGem()
            {
                return knapsackDataItem.ConfigId == GemItemConfigId.GUARDIAN_GEMS.ToInt64();
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
                return knapsackDataItem.ConfigId == GemItemConfigId.GUANG_ZHI_SHI.ToInt64();
            }
            ///是否已经镶嵌荧光宝石
            bool IsInlayYingGuangGem()
            {
                return knapsackDataItem.GetProperValue(E_ItemValue.FluoreSlotCount)>0;
            }
            ///是否有插槽
            bool IsHaveSlot() 
            {
                return knapsackDataItem.GetProperValue(E_ItemValue.FluoreSlotCount)!=0;
            }
            ///是否有镶嵌宝石属性
            bool IsHaveInlayGem() 
            {
                ResetAtrs();
                bool isHave=false;
                if (knapsackDataItem.GetProperValue(E_ItemValue.FluoreSlotCount)is int count && count>0)//有卡槽
                {
                    for (int i = 0; i < count; i++)
                    {
                        var PropId = knapsackDataItem.GetProperValue(E_ItemValue.FluoreSlot1 + i);
                        if (PropId == 0)
                        {
                            continue;
                        }
                        isHave = true;
                        SetEquipAtr(knapsackDataItem,i, PropId);
                    }
                }
                return isHave;
            }

            ///主荧光宝石（属性）
            (bool,string) IsHaveYingGuangBaoShi() 
            {
                if (GemItemConfigId.YING_GUANG_GEM_DIC.TryGetValue(knapsackDataItem.ConfigId, out string name))
                {
                   
                    return (true, $"{name}+{knapsackDataItem.GetProperValue(E_ItemValue.Level)}");
                }
                return (false, "荧光宝石");
            }

            ///是否是同级荧光宝石(属性)
            (bool,string) IsEquallevYingGuangBaoShi() 
            {
                (bool ishave, string gemName) result=(false, "同级荧光宝石");
                if (InlayGridDic.TryGetValue($"{curInlayType.ToInt32()}_{1}", out KnapsackGrid grid))//前一个格子 是否已经添加荧光宝石
                {
                    if (knapsackDataItem.GetProperValue(E_ItemValue.Level) == grid.Data.ItemData.GetProperValue(E_ItemValue.Level))//两个等级是否相等
                    {
                        if (GemItemConfigId.YING_GUANG_GEM_DIC.TryGetValue(knapsackDataItem.ConfigId, out string name))
                        {
                            result= (true, $"{name}+{knapsackDataItem.GetProperValue(E_ItemValue.Level)}");
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
        /// 添加镶嵌 材料物品
        /// </summary>
        /// <param name="index"></param>
        /// <param name="gridData"></param>
        /// <param name="obj"></param>
        void AddInlayItem(KnapsackGridData gridData, GameObject obj)
        {
            obj.SetUI(gridData.ItemData.GetProperValue(E_ItemValue.Level));
            if (InlayGridDic.TryGetValue(CurInlayGridName, out KnapsackGrid grid))
            {
                grid.Data.Point1 = new Vector2Int(gridData.Point1.x, gridData.Point1.y);
                grid.Data.Point2 = new Vector2Int(gridData.Point2.x, gridData.Point2.y);
                grid.Data.UUID = gridData.UUID;
                grid.IsOccupy = true;
                grid.GridObj = obj;
                grid.Data.ItemData = gridData.ItemData;
                obj.transform.position = new Vector3(grid.Image.transform.position.x, grid.Image.transform.position.y, 10);
                AddInlayItem(gridData.UUID);
                Log.DebugGreen($"物品位置设置完成：{grid.GridObj.name}");
            }
        }
        ///添加镶嵌 材料
        void AddInlayItem(long uuid)
        {
            if (InlayItemDic.ContainsKey(CurInlayGridIndex))
            {
                InlayItemDic[CurInlayGridIndex].Add(uuid);
            }
            else
            {
                InlayItemDic[CurInlayGridIndex] = new List<long> { };
                InlayItemDic[CurInlayGridIndex].Add(uuid);
            }
            Log.DebugBrown($"添加后的数量：{InlayItemDic.Count}");
        }
        //移除镶嵌材料
        void RemoveInlayItem()
        {
            if(InlayItemDic.TryGetValue(CurInlayGridIndex,out List<long> ItemList))
            {
                ///移除后添加的物品UUID
                ItemList.Clear();
            }
          
        }
       long GetInlayItem(int Index)
        {
            if (InlayItemDic.TryGetValue(Index, out List<long> ItemList))
            {
                if(ItemList.Count!=0)
                return ItemList.First();
            }
            return 0;
        }
        /// <summary>
        /// 根据序号获取镶嵌 物品的UUID集合
        /// </summary>
        /// <param name="Index">物品序号 从0开始的</param>
        /// <returns></returns>
        List<long> GetInlayItemList(int Index)
        {
            if (InlayItemDic.TryGetValue(Index, out List<long> ItemList))
            {
                return ItemList.Count != 0?ItemList:null;
            }
            return null;
        }
        /// <summary>
        /// 是否已经使用了改物品
        /// </summary>
        /// <returns>true 已经使用了该物品</returns>
        bool IsUsedItem()
        {
            var Items = InlayItemDic.Values.ToList();
            foreach (var item in Items)
            {
                if (item.Exists(r => r == originArea.UUID))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 改变数量文本
        /// </summary>
        /// <param name="index"></param>
        void ChangeCountText(int count = 1)
        {
          
            Log.DebugGreen($"改变数量：{count}  CurInlayGridName：{CurInlayGridName}");
            if (InlayGridDic.TryGetValue(CurInlayGridName, out KnapsackGrid grid))
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
        void ChangeTitleText(string title)
        {
            if (InlayGridDic.TryGetValue(CurInlayGridName, out KnapsackGrid grid))
            {
                GameObject obj = grid.Image.transform.parent.gameObject;
                obj.transform.Find("title").GetComponent<Text>().text =title;
            }

        }
        /// <summary>
        /// 改变合成率
        /// </summary>
        void ChangeSecced(int seccedRate) 
        {
           
            SecceedRateTxt.text = $"{seccedRate}%";

        }
        /// <summary>
        /// 获取成功率
        /// </summary>
        /// <param name="lev"></param>
        /// <returns></returns>
        int GetRate(int lev)=>lev switch 
        {
         0=>100,
            1=>90,
            2=>80,
            3=>70,
            4=>60,
            5=>50,
            6=>40,
            7=>30,
            8=>20,
            _=>0

        };
      

        #region 格子事件
        private void OnPointerEnter_Inlay(E_Grid_Type part, int index)
        {
            if (InlayGridDic.ContainsKey($"{curInlayType.ToInt32()}_{index}") == false) return;
            curChooseArea.Grid_Type = part;//当前选择的格子的类型
            CurInlayGrid = InlayGridDic[$"{curInlayType.ToInt32()}_{index}"];
            CurInlayGridName = $"{curInlayType.ToInt32()}_{index}";
            CurInlayGridIndex = index;
            if (!isDroping) return;
            CurInlayGrid.ReadyColor();
            Log.DebugBrown($"进入镶嵌 起始格子类型:{originArea.Grid_Type}  目标格子类型:{curChooseArea.Grid_Type} index:{index} ");
        }

        private void OnPointerExit_Inlay()
        {
            if (CurInlayGrid == null) return;
            curChooseArea.Grid_Type = E_Grid_Type.None;//当前选择的格子类型为None（即没有进入区域格子）
            CurInlayGrid.ResetColor();
        }

        private void OnPointerClickEvent_Inlay(int index)
        {
            RemoveGridObj();
           
        }
        void RemoveGridObj() 
        {
            if (CurInlayGrid.GridObj != null)
            {
                RemoveInlayItem(); ///移除已经添加的材料
                ChangeCountText(CurInlayGrid.Data.ItemData.GetProperValue(E_ItemValue.Quantity) * -1);
           

                if (curInlayType == E_InlayType.YingGuangBaoShiHeCheng && CurInlayGridIndex == 1)
                    ChangeTitleText("荧之石");
                else if (curInlayType == E_InlayType.YingGuangBaoShiChouQu && CurInlayGridIndex == 1)
                    ResetAtrs();
                else if (curInlayType == E_InlayType.YingGuangBaoShiQiangHua) 
                {
                    if (CurInlayGridIndex == 1)
                    {
                        ChangeSecced(0);
                        ChangeTitleText("荧光宝石");
                    }
                    else if (CurInlayGridIndex == 2) 
                    {
                        ChangeTitleText("同级荧光宝石");
                    }
                }

                ///当前数量为0
                if (CurInlayGrid.curCount == 0)
                {
                    CurInlayGrid.IsOccupy = false;
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(CurInlayGrid.GridObj, CurInlayGrid.GridObj.name.StringToAB());
                }
            }
        }

        #endregion

        /// <summary>
        /// 检查镶嵌 面板上是否有镶嵌物品
        /// </summary>
        /// <returns></returns>
        bool IsHaveInlayItem()
        {
            return InlayItemDic.Count != 0;
        }
        /// <summary>
        /// 清除镶嵌面板
        /// </summary>
        void ClearInlayPanel()
        {
            if (InlayGridDic.Count == 0) return;
            ResetAtrs();
            for (int i = 0, length = InlayGridDic.Count; i < length; i++)
            { 
                KnapsackGrid grid = InlayGridDic.ElementAt(i).Value;
                if (!grid.IsOccupy) continue;
                GameObject obj = grid.GridObj;
                grid.IsOccupy = false;
                grid.curCount = 0;
                GameObject objparent = grid.Image.transform.parent.gameObject;
                string color = ColorTools.GetColorHtmlString(grid.curCount >= grid.MaxCount ? Color.green : Color.red);
                objparent.transform.Find("count").GetComponent<Text>().text = $"<color={color}>{grid.curCount}</color>/{grid.MaxCount}";
                ResourcesComponent.Instance.DestoryGameObjectImmediate(obj, obj.name.StringToAB());
            }
        }
    }
}
