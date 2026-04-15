using ETModel;
using ILRuntime.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

namespace ETHotfix
{
    /// <summary>
    /// 装备模块
    /// </summary>
    public partial class UIKnapsackComponent
    {

        private GameObject EquipGrid;
        private Dictionary<E_Grid_Type, KnapsackGrid> EquipmentPartDic;
        int curWarePart = 0;
        public RoleEquipmentComponent EquipmentComponent;
        public Text CoinText;//金币数量
        public Text ruibiText;//元宝数量
        public Text qijibiText;//奇迹币数量

        bool IsSplit = false;//是否可以分堆
        UIConfirmComponent confirmComponent;
        /// <summary>
        /// 初始化装备模块
        /// </summary>
        public void InitEquipGrids()
        {
            EquipmentComponent ??= roleEntity.GetComponent<RoleEquipmentComponent>();
            ReferenceCollector reference_Equip = EquipPanel.GetReferenceCollector();
            EquipGrid = reference_Equip.GetGameObject("EquipGrid");

            EquipmentPartDic = new Dictionary<E_Grid_Type, KnapsackGrid>();
            int length = EquipGrid.transform.childCount;
            for (int i = 0; i < length; i++)
            {
                GameObject grid = EquipGrid.transform.GetChild(i).gameObject;
                grid.transform.Find("lev").gameObject.SetActive(false);
                grid.transform.Find("append").gameObject.SetActive(false);

                grid.transform.Find("lev").GetComponent<Text>().text = string.Empty;
                grid.transform.Find("append").GetComponent<Text>().text = string.Empty;
                E_Grid_Type part = (E_Grid_Type)int.Parse(grid.name);
                KnapsackGrid knapsackGrid = new KnapsackGrid()
                {
                    GridObj = null,
                    Image = grid.GetComponent<Image>(),
                    IsOccupy = false,
                    Grid_Type = part
                };
                EquipmentPartDic[part] = knapsackGrid;
                RegisterEvent(part, grid);
            }
            reference_Equip.GetImage("6").gameObject.SetActive(roleEntity.RoleType != E_RoleType.Gladiator);//格斗家 隐藏护手
            reference_Equip.GetImage("16").gameObject.SetActive(roleEntity.RoleType == E_RoleType.Holymentor);
            CoinText = reference_Equip.GetText("coinTxt");
            ruibiText = reference_Equip.GetText("yuanbaoTxt");
            qijibiText = reference_Equip.GetText("qijibiTxt");

            //分堆
            reference_Equip.GetButton("SplitBtn").onClick.AddSingleListener(() =>
            {

                //是否有物品可以分堆
                bool issplit = false;
                for (int i = 0, length = KnapsackItemsManager.KnapsackItems.Count; i < length; i++)
                {
                    var item = KnapsackItemsManager.KnapsackItems.ElementAt(i).Value;
                    if (item.GetProperValue(E_ItemValue.Quantity) > 1)
                    {
                        issplit = true;
                        break;
                    }
                }
                if (issplit == false)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "没有装备 可以分堆");
                    return;
                }
                IsSplit = !IsSplit;
                if (IsSplit)
                {
                    confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent(E_TipPanelType.Split);
                    confirmComponent.splitCount = 1;

                    if (confirmComponent != null && originArea.ItemData != null)
                    {
                        CloseIntroduction();
                        if (originArea.ItemData.GetProperValue(E_ItemValue.Quantity) > 1)
                        {
                            confirmComponent.splitItem = originArea;
                            if (confirmComponent.SplitObj != null)//&& confirmComponent.SplitObj.name != originArea.ItemData.item_Info.ResName
                            {
                               // ResourcesComponent.Instance.RecycleGameObject(confirmComponent.SplitObj);
                                ResourcesComponent.Instance.DestoryGameObjectImmediate(confirmComponent.SplitObj, confirmComponent.SplitObj.name.StringToAB());

                            }
                          
                            originArea.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                            confirmComponent.SplitObj = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
                            confirmComponent.SplitObj.SetUI();
                            confirmComponent.SplitObj.transform.SetParent(confirmComponent.objIcon.transform);
                            confirmComponent.SplitObj.transform.localPosition = new Vector3(0, 0, -50);
                            // confirmComponent.SplitObj.transform.position = confirmComponent.objPos;
                            confirmComponent.SplitinputField.text = confirmComponent.splitItem.ItemData.GetProperValue(E_ItemValue.Quantity).ToString();//显示 物品的数量
                        }
                    }


                    confirmComponent.SplitEventAction = async () =>
                    {
                        int? count = confirmComponent.GetSplitFunc?.Invoke();

                        if (originArea == null)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "请您选择要分堆的物品");
                            return;
                        }
                        if (count == null)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "请您设置 物品分堆数量");
                            return;
                        }
                        if (count == originArea.ItemData.GetProperValue(E_ItemValue.Quantity))
                        {
                            confirmComponent.SplitinputField.text = 1.ToString();
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "分堆数量 应小于物品的最大数量");
                            return;
                        }
                        
                        //  请求分堆
                        G2C_SplitItems g2C_SplitItems = (G2C_SplitItems)await SessionComponent.Instance.Session.Call(new C2G_SplitItems
                        {
                            ItemUUID = originArea.UUID,
                            Count = count.ToInt32()
                        });
                        IsSplit = false;
                        if (g2C_SplitItems.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SplitItems.Error.GetTipInfo());
                            
                        }
                        else
                        {
                            UIComponent.Instance.Remove(UIType.UIConfirm);
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "拆分成功");
                        }
                    };
                    confirmComponent.SplitCancelAction = () =>
                    {
                        IsSplit = false;
                        ChangeGridColor(false);
                        confirmComponent = null;
                    };
                }
                ChangeGridColor();



                void ChangeGridColor(bool isSplit = true)
                {
                    GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Knapsack);
                    for (int i = 0; i < LENGTH_Knapsack_Y; i++)
                    {
                        for (int j = 0; j < LENGTH_Knapsack_X; j++)
                        {
                            KnapsackGrid grid = grids[j][i];
                            if (grid.IsOccupy)
                            {
                                if (grid.Data.ItemData.GetProperValue(E_ItemValue.Quantity) > 1)
                                {
                                    grid.Image.color = isSplit ? Color.yellow : Color.green;
                                }
                            }
                        }

                    }
                }

            });

            //特权仓库
            reference_Equip.GetButton("WarehouseBtn").onClick.AddSingleListener(async () =>
            {
                Log.DebugBrown("特权卡");
                //是否有月卡特权
                if (roleEntity.MaxMonthluCardTimeSpan.TotalSeconds <= 0 && !TitleManager.allTitles.Exists(x => x.TitleId == 60005))//roleEntity.MaxMonthluCardTimeSpan.TotalSeconds <= 0 && 
                {
                    UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    confirmComponent.SetTipText("是否购买<color=red>赞助卡</color>、开启远程仓库？");
                    confirmComponent.AddActionEvent(() => UIComponent.Instance.VisibleUI(UIType.UIShop, (int)E_TopUpType.ShopTopUp));
                    return;
                }
                G2C_RemoteOpenResponse g2C_RemoteOpen = (G2C_RemoteOpenResponse)await SessionComponent.Instance.Session.Call(new C2G_RemoteOpenRequest
                {
                    Type = 1
                });
                if (g2C_RemoteOpen.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RemoteOpen.Error.GetTipInfo());
                }
                else
                {
                    ChangePanel(E_KnapsackState.KS_Ware_House);
                }
            });
            //特权商城店
            reference_Equip.GetButton("ShopBtn").onClick.AddSingleListener(async () =>
            {
                Log.DebugBrown("特权卡``");
                //是否有月卡特权
                if (roleEntity.MaxMonthluCardTimeSpan.TotalSeconds <= 0 && !TitleManager.allTitles.Exists(x => x.TitleId == 60005))
                {
                    UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    confirmComponent.SetTipText("是否购买<color=red>赞助卡</color>、开启远程商店？");
                    confirmComponent.AddActionEvent(() =>
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIShop, (int)E_TopUpType.ShopTopUp);
                        CloseKnapsack();
                    });
                    return;
                }
                G2C_RemoteOpenResponse g2C_RemoteOpen = (G2C_RemoteOpenResponse)await SessionComponent.Instance.Session.Call(new C2G_RemoteOpenRequest
                {
                    Type = 0
                });
                if (g2C_RemoteOpen.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RemoteOpen.Error.GetTipInfo());
                    //Log.DebugGreen($"g2C_RemoteOpen.Error:{g2C_RemoteOpen.Error}");
                }
                else
                {
                   // Log.DebugGreen($"商城中的物品:{g2C_RemoteOpen.AllItems.Count}");
                    Dictionary<long, KnapsackDataItem> NPCShopDic = new Dictionary<long, KnapsackDataItem>();
                    for (int i = 0, length = g2C_RemoteOpen.AllItems.Count; i < length; i++)
                    {
                        var item = g2C_RemoteOpen.AllItems[i];
                       // Log.DebugGreen($"商店物品:{item.ConfigID} 位置：{item.PosInBackpackX}：{item.PosInBackpackY} 数量:{item.Quantity}");
                        KnapsackDataItem knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(item.ItemUID);
                        knapsackDataItem.GameUserId = item.GameUserId;//玩家的UID
                        knapsackDataItem.UUID = item.ItemUID;//装备的UID
                        knapsackDataItem.ConfigId = item.ConfigID;//装备配置表id
                        knapsackDataItem.ItemType = item.Type;//装备类型

                        knapsackDataItem.PosInBackpackX = item.PosInBackpackX;//装备在背包中的起始格子 坐标
                        knapsackDataItem.PosInBackpackY = item.PosInBackpackY;
                        knapsackDataItem.X = item.Width;//装备所占的格子
                        knapsackDataItem.Y = item.Height;
                        knapsackDataItem.SetProperValue(E_ItemValue.Quantity, item.Quantity);//装备的数量
                        knapsackDataItem.SetProperValue(E_ItemValue.Level, item.ItemLevel);//装备的等级
                        NPCShopDic[item.ItemUID] = knapsackDataItem;
                    }
                    //设置 NPC商城物品的属性
                    for (int i = 0, length = g2C_RemoteOpen.AllProperty.Count; i < length; i++)
                    {

                        var item = g2C_RemoteOpen.AllProperty[i];
                        //Log.DebugBrown($"item.ItemUUID:{item.ItemUUID}");
                        for (int p = 0, pcount = item.PropList.Count; p < pcount; p++)
                        {
                            NPCShopDic[item.ItemUUID].Set(item.PropList[p].PropID, item.PropList[p].Value);
                        }

                        for (int e = 0, count = item.ExecllentEntry.Count; e < count; e++)
                        {
                            NPCShopDic[item.ItemUUID].SetExecllentEntry(item.ExecllentEntry[e].PropId, item.ExecllentEntry[e].Level);
                        }
                    }

                    ChangePanel(E_KnapsackState.KS_Shop);
                    UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>().Init_ShopEquip(NPCShopDic, g2C_RemoteOpen.NpcId, E_BuyType.Remote);

                }
            });
            //特权维修
            reference_Equip.GetButton("RepairBtn").onClick.AddSingleListener(() =>
            {
                //是否有月卡特权
              /*  if (roleEntity.MinMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    confirmComponent.SetTipText("是否购买<color=red>小特权卡</color>、开启远程维修？");
                    confirmComponent.AddActionEvent(() =>
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIShop, (int)E_TopUpType.ShopTopUp);

                        CloseKnapsack();

                    });
                    return;
                }*/
                RepairEquips();
            });


            InitCoin();
        }

        /// <summary>
        /// 维修装备
        /// </summary>
        public void RepairEquips(params object[] args)
        {
            //那些装备要维修
            var RepairEquipList = new List<int>();//需要维修的装备
            var RepairNeedMoney = 0;//维修需要的价格
            for (E_Grid_Type i = E_Grid_Type.Weapon, length = E_Grid_Type.Pet; i < length; i++)
            {
                EquipmentComponent ??= roleEntity.GetComponent<RoleEquipmentComponent>();
                if (EquipmentComponent.curWareEquipsData_Dic.TryGetValue(i, out KnapsackDataItem knapsackData))
                {
                    if (i == E_Grid_Type.Guard)//守护不能维修
                        continue;

                    if (knapsackData.GetProperValue(E_ItemValue.RepairMoney) is int money)
                    {
                        if (money == 0)
                        {
                            //维修价格为 0 不能维修
                            continue;
                        }
                        else
                        {
                            RepairEquipList.Add((int)i);
                            RepairNeedMoney += money;
                        }
                    }
                }
            }
            if (RepairEquipList.Count == 0)
            {
                //没有装备需要维修
                UIComponent.Instance.VisibleUI(UIType.UIHint, "没有装备 需要维修");
                return;
            }
            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
            uIConfirm.SetTipText($"确定花费<color=red>{RepairNeedMoney}</color>金币 维修装备？");
            uIConfirm.AddActionEvent(async () =>
            {
                //金币是否 足够
                if (RepairNeedMoney > roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足");
                    return;
                }
                //请求维修
                G2C_RepairEquipItemResponse g2C_RepairEquip = (G2C_RepairEquipItemResponse)await SessionComponent.Instance.Session.Call(new C2G_RepairEquipItemRequest
                {
                    EquipPosition = new Google.Protobuf.Collections.RepeatedField<int> { RepairEquipList },//指定装备栏id (维修价格大于0 就可以维修)
                    NpcUID = args.Length == 0 ? 0 : (long)args[0],
                    NpcPosX = args.Length == 0 ? 0 : (int)args[1],
                    NpcPosY = args.Length == 0 ? 0 : (int)args[2],
                });
                if (g2C_RepairEquip.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RepairEquip.Error.GetTipInfo());
                }
                else
                {
                    RepairEquipList = null;
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "维修成功");
                }

            });
            uIConfirm.AddCancelEventAction(() =>
            {
                RepairEquipList = null;
            });
        }

        /// <summary>
        /// 初始化装备栏 已经穿戴的装备
        /// </summary>
        private void InitEquip()
        {
            CleanEquip();
            foreach (var item in EquipmentComponent.curWareEquipsData_Dic)
            {
                if (item.Key == E_Grid_Type.Mounts) continue;//装备面板坐骑不显示
                AddItem(item.Value, true);
            }
          /*  TimerComponent.Instance.RegisterTimeCallBack(300, () => 
            {
                var EquipList = EquipmentComponent.curWareEquipsData_Dic.Values.ToList();
                for (int i = 0,length=EquipList.Count; i < length; i++)
                {
                    if ((E_Grid_Type)EquipList[i].Slot == E_Grid_Type.Mounts) continue;//装备面板坐骑不显示
                    AddItem(EquipList[i], true);
                }
            });*/
        }
        #region 格子事件
        private void RegisterEvent(E_Grid_Type part, GameObject obj)
        {
            UGUITriggerProxy proxy = obj.GetComponent<UGUITriggerProxy>();
            proxy.OnBeginDragEvent += () => { OnBeginDrag(part); };
            proxy.OnEndDragEvent += () => { OnEndDrag(part); };
            proxy.OnPointerEnterEvent += () => { OnPointerEnter(part); };
            proxy.OnPointerExitEvent += () => { OnPointerExit(part); };
            proxy.OnPointerClickEvent += () => { OnPointerClickEvent(part); };
        }

        private void OnBeginDrag(E_Grid_Type part)
        {
            if (IsSplit) return;
            if (uIIntroduction != null)
            {
                UIComponent.Instance.Remove(UIType.UIIntroduction);
                uIIntroduction = null;
            }
            KnapsackGrid grid = EquipmentPartDic[part];
            if (grid.IsOccupy == false) return;


            originArea = grid.Data;
            originArea.Grid_Type = part;

            curDropObj = grid.GridObj;
            originObjPos = grid.GridObj.transform.localPosition;
            originObjRotation = grid.GridObj.transform.localRotation;

            isDroping = true;
            //Log.DebugGreen($"EquipmentPart:{originArea.EquipmentPart}    grid.Data:{ grid.Data.EquipmentPart}");
           // Log.DebugGreen($"EquipmentPart:{originArea.EquipmentPart}    grid.Data:{grid.Data.EquipmentPart}");
        }

        private void OnEndDrag(E_Grid_Type part)
        {
            if (isDroping == false) return;

            curChooseArea.UUID = originArea.UUID;
            //   Log.DebugWhtie($"拖拽结束：curChooseArea.Grid_Type：{curChooseArea.Grid_Type}");
            //Log.DebugWhtie($"拖拽结束：originArea.UUID：{originArea.UUID}");
            //从装备栏 拖拽到背包
            if (curChooseArea.Grid_Type == E_Grid_Type.Knapsack)
            {
                if (ContainGridObj(curChooseArea.Point1.x, curChooseArea.Point1.y, curChooseArea.Point2.x, curChooseArea.Point2.y))
                {
                    ResetGridObj();

                }
                else
                {
                    //请求 服务器 卸载装备
                    UnLoadEquip((int)part, curChooseArea.Point1.x, curChooseArea.Point1.y).Coroutine();

                   /* curChooseArea.EquipmentPart = (int)part;

                    curChooseArea.ItemData = originArea.ItemData;
                  
                    AddKnapsackItem(curChooseArea, curDropObj);//物品放入背包*/
                    RemoveWareEquipItem((int)part, true);//移除装备栏的物品
                    isDroping = false;
                    curDropObj = null;
                }
            }
            else
            {
                ResetGridObj();

            }
        }

        private void OnPointerEnter(E_Grid_Type part)
        {
            if (IsSplit) return;
            if (!isDroping) return;
            //Log.DebugGreen($"当前格子类型{part.EnumToString<E_Grid_Type>()}");
            curChooseArea.Grid_Type = part;//当前选择的格子的类型
            EquipmentPartDic[part].ReadyColor();
            curWarePart = (int)part;
            // Log.DebugBrown($"进入装备栏 起始格子类型:{originArea.Grid_Type}  目标格子类型:{curChooseArea.Grid_Type}  ");
        }

        private void OnPointerExit(E_Grid_Type part)
        {
            curChooseArea.Grid_Type = E_Grid_Type.None;//当前选择的格子类型为None（即没有进入区域格子）
            curChooseArea.Point1 = originArea.Point1;
            curChooseArea.Point2 = originArea.Point2;
            EquipmentPartDic[part].ResetColor();
        }

        private void OnPointerClickEvent(E_Grid_Type part)
        {
            if (IsSplit) return;
            KnapsackGrid grid = EquipmentPartDic[part];
            if (grid.IsOccupy == false)
            {
                if (uIIntroduction != null)
                {
                    UIComponent.Instance.Remove(UIType.UIIntroduction);
                    uIIntroduction = null;
                }
                return;
            }
            KnapsackGridData data = grid.Data;

            Vector3 pos = new Vector3(grid.Image.transform.position.x, grid.Image.transform.position.y, 0);

            //显示 物品属性面板
            KnapsackDataItem dataItem = data.ItemData;
            uIIntroduction ??= UIComponent.Instance.VisibleUI(UIType.UIIntroduction).GetComponent<UIIntroductionComponent>();
            uIIntroduction.GetDataType(part);
            uIIntroduction.GetAllAtrs(dataItem);
            pos -= Vector3.left / 2;
            var screenPos = CameraComponent.Instance.UICamera.WorldToScreenPoint(pos);
            var pivot_x = 0;
            uIIntroduction.ChangeStartCorner(Corner.UpperLeft);
            if (screenPos.x > Screen.width - 700)
            {

                pivot_x = 1;
                pos += Vector3.left;
                uIIntroduction.ChangeStartCorner();
            }

            uIIntroduction.ShowAtrs();
           // uIIntroduction.ShowAtr_Vertical();
            uIIntroduction.SetPos(pos, pivot_x);
        }
        #endregion

        /// <summary>
        /// 初始化 金币、元宝
        /// </summary>
        public void InitCoin()
        {
           // Log.DebugGreen($"金币：{roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin)}");
            CoinText.text = roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin).ToString();//.ToString("N")
            ruibiText.text = roleEntity.Property.GetProperValue(E_GameProperty.MoJing).ToString();
            qijibiText.text = roleEntity.Property.GetProperValue(E_GameProperty.MiracleCoin).ToString();
            Game.EventCenter.EventListenner<long>(EventTypeId.GLOD_CHANGE, ChangeKnapsackCoin);
        }

        /// <summary>
        /// 金币变动
        /// </summary>
        public void ChangeKnapsackCoin(long value) 
        {
            CoinText.text = value.ToString();
        }


        /// <summary>
        /// 穿戴装备
        /// </summary>
        /// <param name="part">装备部位</param>
        /// <param name="dataItem">装备数据</param>
        /// <param name="obj">装备obj</param>
        /// <param name="isInit">是否是初始化装备</param>
        private void WareEquipItem(int part, KnapsackGridData dataItem, GameObject obj)
        {
            if (EquipmentComponent==null)
                return;
            KnapsackGrid grid = EquipmentPartDic[(E_Grid_Type)part];
            grid.Data.EquipmentPart = part;
            grid.Data.Point1 = new Vector2Int(dataItem.Point1.x, dataItem.Point1.y);
            grid.Data.Point2 = new Vector2Int(dataItem.Point2.x, dataItem.Point2.y);
            grid.Data.UUID = dataItem.UUID;
            grid.GridObj = obj;
            grid.IsOccupy = true;
            grid.Data.ItemData = dataItem.ItemData;
            obj.transform.position = new Vector3(grid.Image.transform.position.x, grid.Image.transform.position.y, 85);
            if (dataItem.ItemData.GetProperValue(E_ItemValue.Level) != 0)
            {
                grid.Image.transform.Find("lev").GetComponent<Text>().text = dataItem.ItemData.GetProperValue(E_ItemValue.Level) != 0 ? $"{dataItem.ItemData.GetProperValue(E_ItemValue.Level)}" : string.Empty;
                grid.Image.transform.Find("lev").gameObject.SetActive(true);
            }
            
            if (dataItem.ItemData.GetProperValue(E_ItemValue.OptLevel) != 0)
            {
                grid.Image.transform.Find("append").GetComponent<Text>().text = dataItem.ItemData.GetProperValue(E_ItemValue.OptLevel) != 0 ? $"{dataItem.ItemData.GetProperValue(E_ItemValue.OptLevel)}" : string.Empty;
                grid.Image.transform.Find("append").gameObject.SetActive(true);
            }
            obj.transform.rotation = Quaternion.identity;
            obj.SetActive(true);
           
        }
        /// <summary>
        /// 卸载装备
        /// </summary>
        /// <param name="part">装备部位</param>
        /// <param name="isRecycle">是否回收部位 对应的Obj</param>
        public void RemoveWareEquipItem(int part, bool isRecycle = false)
        {
            KnapsackGrid grid = this.EquipmentPartDic[(E_Grid_Type)part];
            GameObject recycleGo = grid.GridObj;
            grid.GridObj = null;
            grid.IsOccupy = false;
            grid.Image.transform.Find("lev").GetComponent<Text>().text = string.Empty;
            grid.Image.transform.Find("append").GetComponent<Text>().text = string.Empty;
            grid.Image.transform.Find("lev").gameObject.SetActive(false);
            grid.Image.transform.Find("append").gameObject.SetActive(false);
            grid.Data.UUID = 0;
            grid.Data.EquipmentPart = 0;
            if (recycleGo != null && isRecycle)
            {
               // ResourcesComponent.Instance.RecycleGameObject(recycleGo);
                ResourcesComponent.Instance.DestoryGameObjectImmediate(recycleGo,recycleGo.name.StringToAB());
            }

        }
        /// <summary>
        /// 是否可以穿带该装备
        /// </summary>
        /// <returns></returns>
        public bool IsCanWearEquip(ref int curPart)
        {
          //  Log.DebugGreen($"{originArea.UUID} 是否可以使用KnapsackItemsManager.KnapsackItems:{KnapsackItemsManager.KnapsackItems.Count}");

            if (!KnapsackItemsManager.KnapsackItems.TryGetValue(originArea.UUID, out KnapsackDataItem item))
            {
                Log.DebugRed("背包中不存在 对应的装备");
                return false;
            }
            item.ConfigId.GetItemInfo_Out(out Item_infoConfig config); ;//读取装备配置信息
          //  Log.DebugRed($"config == null:{config == null} item.ConfigId:{item.ConfigId}");
            if (config == null) return false;

            ///该玩家是否可以穿戴该装备
            if (!config.IsCanUer((int)roleEntity.RoleType, roleEntity.ClassLev))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "玩家不可装备");
                return false;
            }

            ///检查 等级、力量、敏捷、智力、体力、统率 是否满足
            if (originArea.ItemData.CheckRequirel() == false)
            {
                return false;
            }

            curPart = curWarePart;

            //限制变身戒指
           /* if (curChooseArea.Grid_Type == E_Grid_Type.LeftRing || curChooseArea.Grid_Type == E_Grid_Type.RightRing)
            {
                //左右戒指
                for (E_Grid_Type i = E_Grid_Type.LeftRing; i <= E_Grid_Type.RightRing; i++)
                {
                    if (EquipmentComponent.curWareEquipsData_Dic.TryGetValue(i, out KnapsackDataItem ringitem))
                    {
                        Item_RingsConfig item_Ring = ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)ringitem.ConfigId);
                        Item_RingsConfig origin_Ring = ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)item.ConfigId);
                      //  Log.DebugGreen($"是否是变身戒指：{item_Ring.IsTransRing == 1}");
                        if (item_Ring.IsTransRing == 1&& origin_Ring.IsTransRing==1)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "变身戒指只能佩戴一个");
                            return false;
                        }
                    }
                }

            }*/

            if (curChooseArea.Grid_Type == E_Grid_Type.Weapon)//目标卡槽是 武器
            {
                if (EquipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Shield))//盾牌卡槽 已装备
                {
                    if (roleEntity.RoleType != E_RoleType.Archer)
                    {

                        KnapsackItemsManager.KnapsackItems[originArea.UUID].ConfigId.GetItemInfo_Out(out Item_infoConfig weaponConfig); //当前武器 的配置表信息
                        if (weaponConfig != null && weaponConfig.TwoHand == 1)//该武器是双手武器 
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "装备盾时 不能装备双手武器");
                            return false;
                        }
                    }
                    else
                    {
                        //弓箭手
                        Log.DebugBrown("" + EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ConfigId);
                        if (EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ConfigId == 60008|| EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ConfigId == 60000) return true;//黄金箭筒

                        //盾牌卡槽 装备了弓箭 武器卡槽只能装备弓 
                        if (EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ItemType == (int)E_ItemType.Arrow || EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ConfigId == 40019)
                        {
                            if (item.ItemType != (int)E_ItemType.Bows)
                            {
                                //当前装备 不是弓
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "只能装备 弓");
                                return false;
                            }
                        }
                        else
                        {
                            if (item.ItemType == (int)E_ItemType.Bows)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "不能装备 弓");
                                return false;
                            }
                            else if (item.ItemType == (int)E_ItemType.Crossbows)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "不能装备 驽");
                                return false;
                            }
                        }
                        //盾牌卡槽 装备了弩箭 武器卡槽只能装备弩
                        if (EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ItemType == (int)E_ItemType.Arrow || EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ConfigId == 50012)
                        {
                            if (item.ItemType != (int)E_ItemType.Crossbows)
                            {
                                //当前装备 不是驽
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "只能装备 驽");
                                return false;
                            }
                        }

                    }

                }
            }
            if (curChooseArea.Grid_Type == E_Grid_Type.Shield)//目标卡槽是 盾牌 
            {
             //   Log.DebugGreen($"目标卡槽为：{curChooseArea.Grid_Type.ToString()}  是否已经装备武器：{EquipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Weapon)}");
                if (EquipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Weapon))//武器卡槽 已装备
                {


                    EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Weapon].ConfigId.GetItemInfo_Out(out Item_infoConfig weaponConfig); //当前已经装备的武器 的配置表信息
                    KnapsackItemsManager.KnapsackItems[originArea.UUID].ConfigId.GetItemInfo_Out(out Item_infoConfig curweaponConfig); //当前武器 的配置表信息
                    if (roleEntity.RoleType == E_RoleType.Archer)  //弓箭手
                    {

                        if (config.Id == 60008) return true;//黄金箭筒

                        //武器卡槽 装备了弓 盾牌卡槽只能装备弓箭或箭筒
                        if (weaponConfig.Type == (int)E_ItemType.Bows)
                        {
                          //  Log.DebugGreen($"item.ItemType:{item.ItemType}");
                            if (item.ItemType != (int)E_ItemType.Arrow && !config.Name.Contains("弓箭"))
                            {
                                //当前装备 不是弓箭
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "只能装备 弓箭或箭筒");
                                return false;
                            }
                        }
                        //武器卡槽 装备了弩 武器卡槽只能装备弩箭
                        else if (weaponConfig.Type == (int)E_ItemType.Crossbows)
                        {
                            // 弩箭
                            if (!curweaponConfig.Name.Contains("弩箭"))
                            {
                                //当前装备 不是弓箭
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "只能装备 弩箭");
                                return false;
                            }
                        }
                        //武器卡槽装备了 武器  盾牌卡槽不能装备箭筒、弓箭、弩箭
                        else
                        {
                            if (item.ItemType == (int)E_ItemType.Arrow || config.Name.Contains("弓箭") || config.Name.Contains("弩箭"))
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "装备了武器 不能使用箭筒、弓箭、弩箭");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (weaponConfig != null && weaponConfig.TwoHand == 1)//主手武器是双手武器 
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "已装备双手武器 不可装备副武器");
                            return false;
                        }
                        else if (curweaponConfig != null && curweaponConfig.TwoHand == 1)//双手武器 不能装备在副武器卡槽
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "双手武器 不能装备在该位置");
                            return false;
                        }
                        else
                        {
                            //主武器是单手武器 副武器是单手武器

                            if (roleEntity.RoleType == E_RoleType.Summoner && item.ItemType == (int)E_ItemType.MagicBook)
                            {
                                //召唤术师 可以装备魔法书
                                return true;
                            }

                            //盾牌 
                            if (item.ItemType == (int)E_ItemType.Shields)
                            {
                                return true;
                            }

                            //（剑士 魔剑士 格斗家 可以装备两把单手武器（剑 魔杖 斧头））
                            if (roleEntity.RoleType != E_RoleType.Swordsman && roleEntity.RoleType != E_RoleType.Magicswordsman && roleEntity.RoleType != E_RoleType.Gladiator) // 当前角色是否是 剑士 魔剑士 格斗家
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{roleEntity.RoleType.GetRoleName(roleEntity.ClassLev)}  不可同时装备两把单手武器");
                                return false;
                            }

                            return true;
                        }

                    }
                }
            }
            //判断 当前移动的装备 是否属于当前选择装备卡槽
            bool isRing = originArea.EquipmentPart == 11 && (int)curChooseArea.Grid_Type == 12;
            bool isDoubleHandWeapon = originArea.EquipmentPart == 1 && (int)curChooseArea.Grid_Type == 2;//当前物品的装备类型为武器 目标卡槽为盾牌

           
            if ((int)curChooseArea.Grid_Type != (int)originArea.EquipmentPart && !isRing && !isDoubleHandWeapon)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "装备类型不匹配");
                return false;
            }
            return true;


        }
        /// <summary>
        /// 关闭装备界面需要发送
        /// </summary>
        /// <returns></returns>
        async ETVoid CloseSend()
        {
            G2C_CloseEquipUIResponse g2C_RepairEquip = (G2C_CloseEquipUIResponse)await SessionComponent.Instance.Session.Call(new C2G_CloseEquipUIRequest
            {

            });
            if (g2C_RepairEquip.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RepairEquip.Error.GetTipInfo());
            }
        }
        /// <summary>
        /// 清理装备面板
        /// </summary>
        public void CloseEquipPanel()
        {
            CloseSend().Coroutine();
            CleanEquip();
            EquipmentComponent = null;
        }
       
        public void CleanEquip()
        {
            foreach (var item in EquipmentPartDic)
            {  
                if (item.Value.IsOccupy)
                {
                    
                    RemoveWareEquipItem((int)item.Key, true);
                }
            }
           
            Game.EventCenter.RemoveEvent<long>(EventTypeId.GLOD_CHANGE, ChangeKnapsackCoin);
        }

    }
}
