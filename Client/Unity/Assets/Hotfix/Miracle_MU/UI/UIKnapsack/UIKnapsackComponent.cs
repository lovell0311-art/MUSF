using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;
using ILRuntime.Runtime;
using System.Linq;
using Codice.CM.Common;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIKnapsackComponentUpdata : UpdateSystem<UIKnapsackComponent>
    {
        public override void Update(UIKnapsackComponent self)
        {
            if (self.isDroping && self.curDropObj != null)
            {
                Vector3 pos = CameraComponent.Instance.UICamera.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 10;
                self.curDropObj.transform.Rotate(Vector3.down);
                self.curDropObj.transform.position = pos;
            }
            else if (self.curDropObj != null && self.curDropObj.transform.localEulerAngles.y != 0)
            {
                self.curDropObj.transform.localEulerAngles = new Vector3(self.curDropObj.transform.localEulerAngles.x, 0, self.curDropObj.transform.localEulerAngles.z);
            }

            if (self.useItem != null && self.StartUse)
            {
                //  Log.DebugBrown($"物品使用间隔时间：{self.userTime - Time.time} {self.dianjiImage.activeSelf}");
                if (Time.time >= self.userTime && self.IsUse == false)
                {
                    self.IsUse = true;
                    self.PlayerUserItemInTheBackpack(self.useItem.ItemData.Id).Coroutine();
                }
                else
                {

                    self.dianjiImage.SetActive(true);
                    self.filledImage.fillAmount = (self.userItemTime - (self.userTime - Time.time)) / self.userItemTime;

                }
            }
        }

    }

    [ObjectSystem]
    public class UIKnapsackComponentAwake : AwakeSystem<UIKnapsackComponent>
    {
        public override void Awake(UIKnapsackComponent self)
        {
            self.Init_Panel();
            self.KnapsackAwake();

        }
    }
    [ObjectSystem]
    public class UIKnapsackComponentStart : StartSystem<UIKnapsackComponent>
    {
        public override void Start(UIKnapsackComponent self)
        {
            self.Init_KnapsackItems();
        }
    }
    /// <summary>
    /// 背包
    /// </summary>
    public partial class UIKnapsackComponent : Component, IUGUIStatus
    {
        public GameObject UIBeginnerGuide, UIBeginnerGuideTwo, UIBeginnerGuideThree;
        public static UIKnapsackComponent Instance;
        private GameObject KnapsackGrids;//背包格子父对象

        private KnapsackGrid[][] BackGrids;//背包格子交错数组
        public const int LENGTH_Knapsack_X = 8;//列数(LEGNTH_X)
        public const int LENGTH_Knapsack_Y = 12;//行数(LEGNTH_Y)

        private const string UnitEnityTopItemCanvas = "UIUnitEnityTopItem";//数量显示


        public bool isDroping = false;//是否拖动中
        public GameObject curDropObj;//当前正在拖拽的物品
        private Vector3 originObjPos = Vector3.zero;//起始物品的坐标
        private Quaternion originObjRotation = Quaternion.identity;//起始物品的角度

        private GameObject deleteImage;//删除区域

        public GameObject dianjiImage;
        public Image filledImage;

      public  KnapsackGrid[][] grids = null;
        int LENGTH_X = 0;
        int LENGTH_Y = 0;


        public KnapsackGridData curChooseArea;//当前所选择的格子区域(目标区域)
        public KnapsackGridData originArea;//被选择的格子区域（起始区域）

        public bool StartUse = false;//开始使用
        public KnapsackGridData useItem;//长按三秒使用物品
        private GameObject userItemObj = null;//正在使用的物品的OBj
        public float userItemTime = 3;
        public float userTime = 0;
        public bool IsUse = false;

        RoleEntity roleEntity => UnitEntityComponent.Instance.LocalRole;//本地玩家

        public UIIntroductionComponent uIIntroduction;//物品简介组件

       
        public Button clearUp;//整理背包
      
        //是否使用垂直布局属性显示
        readonly bool isVertical = true;
        int ringSlot = 11;//已经穿戴的戒指卡槽
        /// <summary>
        /// 初始化背包格子信息
        /// </summary>
        public void KnapsackAwake()
        {
            Instance = this;
            ReferenceCollector referenceCollector_Knapsack = Knapsack.GetReferenceCollector();
            KnapsackGrids = referenceCollector_Knapsack.GetGameObject("Grids");
            //关闭背包
            referenceCollector_Knapsack.GetButton("CloseBtn").onClick.AddSingleListener(CloseKnapsack);
            //整理背包
            clearUp = referenceCollector_Knapsack.GetButton("ClearUpBtn");
            clearUp.onClick.AddSingleListener(FinishingBackpack);

            deleteImage = referenceCollector_Knapsack.GetImage("DeleteArea").gameObject;
            dianjiImage = referenceCollector_Knapsack.GetImage("dianji").gameObject;
            filledImage = referenceCollector_Knapsack.GetImage("filed");
            dianjiImage.SetActive(false);

            //一键出售白装
            referenceCollector_Knapsack.GetButton("SellEquipBtn").onClick.AddSingleListener(async () =>
            {
                G2C_RemoteOpenResponse g2C_RemoteOpen = (G2C_RemoteOpenResponse)await SessionComponent.Instance.Session.Call(new C2G_RemoteOpenRequest
                {
                    Type = 0
                });
                if (g2C_RemoteOpen.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RemoteOpen.Error.GetTipInfo());

                }
                else
                {
                    RecycleEquipTools.CurNpcUUid = g2C_RemoteOpen.NpcId;
                    CurNpcUUid = g2C_RemoteOpen.NpcId;
                    ChangePanel(E_KnapsackState.KS_Recycle);

                }

            });

            //摆摊
            referenceCollector_Knapsack.GetButton("baiTanBtn").onClick.AddSingleListener(async () =>
            {
                if (curKnapsackState == E_KnapsackState.KS_Trade)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "交易中 无法摆摊");
                    return;
                }

                G2C_BaiTanResponse g2C_BaiTanResponse = (G2C_BaiTanResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanRequest { });
                if (g2C_BaiTanResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanResponse.Error.GetTipInfo());

                }
                else
                {
                    if (curKnapsackState == E_KnapsackState.KS_Stallup) return;
                    ChangePanel(E_KnapsackState.KS_Stallup);
                }
            });

            //分摊
            referenceCollector_Knapsack.GetButton("SellEquipBtn").onClick.AddSingleListener(() =>
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
            });
            //批量买药
            referenceCollector_Knapsack.GetButton("buymediceBtn").onClick.AddSingleListener(() => 
            {

                if (SceneComponent.Instance.CurrentSceneIndex == (int)SceneName.ShiLianZhiDi)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"试炼期间 无法买药");
                    return;
                }

                if (roleEntity.MinMonthluCardTimeSpan.TotalSeconds <= 0&& roleEntity.MaxMonthluCardTimeSpan.TotalSeconds <= 0 && !TitleManager.allTitles.Exists(x => x.TitleId == 60005))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"权限不足、请购买赞助卡");
                   /* UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    confirmComponent.SetTipText("是否购买<color=red>小特权卡</color>、开启远程维修？");
                    confirmComponent.AddActionEvent(() =>
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIShop, (int)E_TopUpType.ShopTopUp);
                        CloseKnapsack();

                    });*/
                    return;
                }
                UIComponent.Instance.VisibleUI(UIType.UIBuyMedicine);
            });

            BackGrids = new KnapsackGrid[LENGTH_Knapsack_X][];//创建具有LENGTH_LINE个元素的交错数组；注意：相当于交错数组的行数
            for (int i = 0; i < BackGrids.Length; i++)
            {
                BackGrids[i] = new KnapsackGrid[LENGTH_Knapsack_Y];//每一个一维数组的长度为LENGTH_COLNUME
            }

            curChooseArea = new KnapsackGridData();
            originArea = new KnapsackGridData();
            useItem = null;
            //初始化格子
            CreatGrid(LENGTH_Knapsack_X, LENGTH_Knapsack_Y, KnapsackGrids.transform, E_Grid_Type.Knapsack, ref BackGrids);
            RegisterEvent(-1, -1, deleteImage, E_Grid_Type.Delete);

            Game.EventCenter.EventListenner<long>(EventTypeId.RemoveKnapsack, RemoveKnapsack);
        }
       private void ChangeGridColor(bool isSplit = true)
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
        /// <summary>
        /// 初始化背包物品
        /// </summary>
        public void Init_KnapsackItems()
        {
            var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
            foreach (KnapsackDataItem item in list)
            {
                if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
                AddItem(item, type: E_Grid_Type.Knapsack);
            }


            /*TimerComponent.Instance.RegisterTimeCallBack(200, () =>
            {
                var kanpsackList = KnapsackItemsManager.KnapsackItems.Values.ToList();
                for (int i = 0, length = kanpsackList.Count; i < length; i++)
                {
                    AddItem(kanpsackList[i], type: E_Grid_Type.Knapsack);
                }
            });*/

        }

        #region 格子事件
        /// <summary>
        /// 注册格子事件
        /// </summary>
        /// <param name="tr"></param>
        private void RegisterEvent(int x, int y, GameObject obj, E_Grid_Type type)
        {
            UGUITriggerProxy proxy = obj.GetComponent<UGUITriggerProxy>() ?? obj.AddComponent<UGUITriggerProxy>();

            proxy.OnBeginDragEvent += () => { OnBeginDrag(x, y, type); };
            proxy.OnEndDragEvent += () => { OnEndDrag(x, y, type); };
            proxy.OnPointerEnterEvent += () => { OnPointerEnter(x, y, type); };
            proxy.OnPointerClickEvent += () => { OnPointerClickEvent(x, y, type); };
            proxy.OnPointerExitEvent += () => { OnPointerExit(x, y, type); };
            proxy.OnPointerDownEvent += () => { OnPointerDownEvent(x, y, type); };
            proxy.OnPointerUpEvent += () => { OnPointerUpEvent(x, y, type); };
        }

        private void OnPointerUpEvent(int x, int y, E_Grid_Type grid_Type)
        {
            if (grid_Type == E_Grid_Type.Inlay)
            {
                ///镶嵌 面板

                return;
            }
            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, grid_Type);
            StopUseItem();
        }
        private void OnPointerDownEvent(int x, int y, E_Grid_Type grid_Type)
        {
            if (x == -1 && y == -1) return;
            if (grid_Type == E_Grid_Type.Inlay)
            {
                ///镶嵌 面板

                return;
            }

            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, grid_Type);

            KnapsackGrid grid = grids[x][y];

            if (grid.IsOccupy == false) return;
            if (curKnapsackState==E_KnapsackState.KS_Knapsack&&KnapsackItemsManager.KnapsackItems.TryGetValue(grid.Data.UUID, out KnapsackDataItem dataItem))
            {
                if (dataItem.ConfigId == 310102) return;//改名卡
                if (dataItem.ConfigId == 260015 || dataItem.ConfigId == 260019)
                {
                    //天鹰、烈火凤凰 属于装备
                    return;
                }
                //  Log.DebugGreen($"dataItem.Slot:{dataItem.ItemType}");
                //技能书、消耗品、坐骑 可以 长按使用
                if (dataItem.ItemType == (int)E_ItemType.SkillBooks
                    || dataItem.ItemType == (int)E_ItemType.Consumables
                    || dataItem.ItemType == (int)E_ItemType.Mounts
                    )//|| dataItem.ItemType == (int)E_ItemType.Pet
                {
                    if (dataItem.ItemType == (int)E_ItemType.Mounts)
                    {
                        if (roleEntity.IsSafetyZone&& dataItem.ConfigId != 260020)
                        {
                          


                            UIComponent.Instance.VisibleUI(UIType.UIHint, "安全区无法使用坐骑");
                                return;
                           
                        }

                        if (!dataItem.item_Info.IsCanUer((int)roleEntity.RoleType, roleEntity.ClassLev))
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "不满足使用要求");
                            return;
                        }
                    }
                    userItemObj = grid.GridObj;
                    StartUseItem(grid.Data);//使用物品

                }
            }

        }

        private void OnPointerExit(int x, int y, E_Grid_Type grid_Type)
        {

            curChooseArea.Grid_Type = E_Grid_Type.None;//当前选择的各子 类型为空
            if (x == -1) return;

            if (grid_Type == E_Grid_Type.Inlay)
            {
                ///镶嵌 面板
                OnPointerExit_Inlay();
                return;
            }

            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, grid_Type);
            //拖动中离开格子 清空记录的信息
            if (curChooseArea.IsSinglePoint)
            {
                grids[x][y].ResetColor();
            }
            else
            {
                Vector2Int centerOffset = GetCenterGrid();
                x += centerOffset.x;
                y += centerOffset.y;

                Vector2Int offset = new Vector2Int(x, y) - this.curChooseArea.Point1;
                Vector2Int endPoint = this.curChooseArea.Point2 + offset;

                List<KnapsackGrid> gridList = GetAreaGrids(x, y, endPoint.x, endPoint.y, LENGTH_X, LENGTH_Y);
                foreach (var item in gridList)
                {
                    item.ResetColor();
                }
            }
        }

        private void OnPointerClickEvent(int x, int y, E_Grid_Type grid_Type)
        {
            if (x == -1)
            {
                if (uIIntroduction != null)
                {
                    UIComponent.Instance.Remove(UIType.UIIntroduction);
                    uIIntroduction = null;
                }

                return;
            }
            if (grid_Type == E_Grid_Type.Inlay)
            {
                ///镶嵌 面板
                OnPointerClickEvent_Inlay(y);
                return;
            }

            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, grid_Type);

            KnapsackGrid grid = grids[x][y];
            originArea = grid.Data;
            originArea.Grid_Type = grid.Grid_Type;
            //可以分堆
            if (IsSplit && confirmComponent != null && originArea.ItemData != null)
            {
                if (originArea.ItemData.GetProperValue(E_ItemValue.Quantity) == 1)
                {
                    return;
                }
                confirmComponent.splitItem = originArea;
                if (confirmComponent.SplitObj != null)//&& confirmComponent.SplitObj.name != originArea.ItemData.item_Info.ResName
                {
                   // ResourcesComponent.Instance.RecycleGameObject(confirmComponent.SplitObj);
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(confirmComponent.SplitObj, confirmComponent.SplitObj.name.StringToAB());

                }

                if (grid == null) return;

                confirmComponent.SplitObj = ResourcesComponent.Instance.LoadGameObject(grid.GridObj.name.StringToAB(), grid.GridObj.name);
                confirmComponent.SplitObj.SetUI();
                confirmComponent.SplitObj.transform.SetParent(confirmComponent.objIcon.transform);
                confirmComponent.SplitObj.transform.localPosition = new Vector3(0, 0, -50);
                // confirmComponent.SplitObj.transform.position = confirmComponent.objPos;
                confirmComponent.SplitinputField.text = confirmComponent.splitItem.ItemData.GetProperValue(E_ItemValue.Quantity).ToString();//显示 物品的数量
                return;
            }
            if (IsSplit) return;

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


            Vector3 pos = data.IsSinglePoint ? new Vector3(grid.Image.transform.position.x, grid.Image.transform.position.y, 0) : GetCenterPos(data.Point1.x, data.Point1.y, data.Point2.x, data.Point2.y);

            //物品属性面板
            KnapsackDataItem dataItem = data.ItemData;
            uIIntroduction ??= UIComponent.Instance.VisibleUI(UIType.UIIntroduction).GetComponent<UIIntroductionComponent>();
            E_KnapsackIntroduceShowPrice e_Knapsack = GetPticeType();

            uIIntroduction.GetAllAtrs(dataItem, e_Knapsack);

            //装备对比
            if (curKnapsackState == E_KnapsackState.KS_Knapsack
                && 1 <= data.EquipmentPart &&
                data.EquipmentPart <= 13
                )
            {
                if (EquipmentComponent.curWareEquipsData_Dic.TryGetValue((E_Grid_Type)data.EquipmentPart, out KnapsackDataItem item))
                {

                    ShowEquipAtr();

                    uIIntroduction.IsListring(IsShangJia());
                }
                else if (IsRing())
                {
                    item = EquipmentComponent.curWareEquipsData_Dic[(E_Grid_Type)ringSlot];
                    ShowEquipAtr();

                    uIIntroduction.IsListring(IsShangJia());
                }
                else
                {
                    Log.DebugBrown("1111");
                    ShowBaseAtr();
                }
                //显示装备 对照属性
                void ShowEquipAtr()
                {
                    uIIntroduction.GetEquipAllAtrs(item, e_Knapsack);
                  //  uIIntroduction.ShowEquipInfo();
                    //丢弃物品
                    uIIntroduction.DiscarACtion = () =>
                    {
                        if (curKnapsackState == E_KnapsackState.KS_Knapsack)
                        {
                            SceneName sceneName = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>();
                            if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法丢弃");

                            }
                            else if (originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法丢弃");

                            }
                            else if (originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法丢弃");

                            }
                            else if (originArea.ItemData.ConfigId == 320106 && roleEntity.IsSafetyZone)
                            {
                                List<string> list = new List<string>();
                                originArea.ItemData.GetItemName(ref list);
                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"安全区不能使用{list.ListToString().Split(',')[1]}");

                            }
                            else if ((originArea.ItemData.ConfigId == 320106 && !roleEntity.IsSafetyZone) && (sceneName == SceneName.XueSeChengBao || sceneName == SceneName.EMoGuangChang || sceneName == SceneName.GuZhanChang || sceneName == SceneName.kalima_map))
                            {
                                List<string> list = new List<string>();
                                originArea.ItemData.GetItemName(ref list);
                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"副本里不能使用{list.ListToString().Split(',')[1]}");
                            }
                            //else if (!originArea.ItemData.IsCanTrade()) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                            //{
                            //    UIComponent.Instance.VisibleUI(UIType.UIHint, "该物品不能丢弃");
                            //    ResetGridObj();
                            //}
                            else
                            {
                                //丢弃物品
                                SendDiscardKnasackItemMessage().Coroutine();
                                UIComponent.Instance.Remove(UIType.UIIntroduction);
                                uIIntroduction = null;
                            }
                        }
                    };
                    //穿戴装备
                    uIIntroduction.WareAction = () =>
                    {

                        if (IsCanReplace())
                        {
                            curChooseArea.Grid_Type = (E_Grid_Type)data.EquipmentPart;
                            if (!IsCanWearEquip(ref curWarePart))
                            {
                                //不能穿戴
                                return;
                            }

                            if (IsRing())
                            {
                                RemoveWareEquipItem(ringSlot, true);//移除装备栏的物品
                                RequestReplaceEquip(ringSlot).Coroutine();

                            }
                            else
                            {

                              
                                RemoveWareEquipItem(data.EquipmentPart, true);//移除装备栏的物品
                                RequestReplaceEquip(data.EquipmentPart).Coroutine();


                            }
                        }
                        else
                        {
                            var pos = GetPosInKnapsack();
                            
                            if (pos.x ==-1)
                            {
                                item.ConfigId.GetItemInfo_Out(out Item_infoConfig equipconfig);
                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"背包中 没有 {equipconfig.X} x {equipconfig.Y} 未使用的格子");
                            }
                            else
                            {
                                UnLoadWareEquip(pos);
                                WareEquip();
                                UIComponent.Instance.Remove(UIType.UIIntroduction);
                                uIIntroduction = null;
                            }
                        }



                        //是否可以替换
                        bool IsCanReplace()
                        {
                            item.ConfigId.GetItemInfo_Out(out Item_infoConfig equipconfig);
                            dataItem.ConfigId.GetItemInfo_Out(out Item_infoConfig dataItemconfig);
                            return equipconfig.X <= dataItemconfig.X && equipconfig.Y <= dataItemconfig.Y;

                        }
                        //获取背包中的位置
                        Vector2Int GetPosInKnapsack()
                        {
                            item.ConfigId.GetItemInfo_Out(out Item_infoConfig config);
                            Vector2Int vector2Int = KnapsackTools.GetKnapsackItemUnoccupied(KnapsackTools.CreateGridData(0, 0, config.X - 1, config.Y - 1));
                            if (vector2Int.x == -1)
                            {

                                return new Vector2Int(-1,-1);
                            }
                            else
                            {
                                return vector2Int;
                            }

                        }

                        //卸载装备
                        void UnLoadWareEquip(Vector2Int? pos)
                        {
                            RemoveWareEquipItem(data.EquipmentPart, true);//移除装备栏的物品
                                                                          //请求 服务器 卸载装备
                            UnLoadEquip(data.EquipmentPart, pos.Value.x, pos.Value.y).Coroutine();

                        }

                        //穿戴装备
                        void WareEquip()
                        {
                            curChooseArea.Grid_Type = (E_Grid_Type)data.EquipmentPart;
                            if (!IsCanWearEquip(ref curWarePart))
                            {
                                //不能穿戴
                                // ResetGridObj();
                            }
                            else
                            {
                                curChooseArea.UUID = dataItem.UUID;
                                curChooseArea.ItemData = dataItem;
                                curWarePart = data.EquipmentPart;//当前选择的格子的类型
                                RequestWareEquip().Coroutine();  //请求穿戴装备
                            }
                        }
                    };
                    //分享
                    uIIntroduction.VerticalShareAction = async () =>
                    {
                        //dataItem.GetItemInfo_Out(out item_Info);
                        string lev = dataItem.GetProperValue(E_ItemValue.Level) > 0 ? "+" + dataItem.GetProperValue(E_ItemValue.Level) : "";
                        string itemName;
                        int color = -1;
                        if (dataItem.GetSuitName().Item1)//有套装属性
                        {
                            if (lev != "")
                                itemName = $"{dataItem.GetSuitName().Item2} {dataItem.item_Info.Name} {lev}";
                            else
                                itemName = $"{dataItem.GetSuitName().Item2} {dataItem.item_Info.Name}";
                            color = 5;
                        }
                        else if (dataItem.IsHaveExecllentEntry) //卓越属性
                        {
                            if (lev != "")
                                itemName = $"卓越的 {dataItem.item_Info.Name}\t{lev}";
                            else
                                itemName = $"卓越的 {dataItem.item_Info.Name}";
                            color = 4;
                        }
                        else
                        {
                            //默认装备
                            if (lev != "")
                                itemName = $"{dataItem.item_Info.Name} {lev}";
                            else
                                itemName = $"{dataItem.item_Info.Name}";
                            color = 3;
                        }
                        //Log.DebugGreen($"分享装备   id {this.originArea.UUID}   名字{itemName}");
                        G2C_BagShareResponse g2C_BagShare = (G2C_BagShareResponse)await SessionComponent.Instance.Session.Call(new C2G_BagShareRequest()
                        {
                            ShareItemId = this.originArea.UUID,
                            Color = color,
                            MessageInfo = itemName
                        });
                        if (g2C_BagShare.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BagShare.Error.GetTipInfo());
                        }
                        else
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "分享成功！");
                            UIComponent.Instance.Remove(UIType.UIIntroduction);
                            uIIntroduction = null;
                        }
                    };
                    uIIntroduction.SellAction = (int value) =>
                    {
                        Log.DebugGreen("上架1");
                        listingTreasureHouse(dataItem.Id,value).Coroutine();
                    };
                }
                //请求替换装备
                async ETVoid RequestReplaceEquip(int slot)
                {
                    G2C_ReplaceEquipItemResponse g2C_ReplaceEquip = (G2C_ReplaceEquipItemResponse)await SessionComponent.Instance.Session.Call(new C2G_ReplaceEquipItemRequest
                    {
                        ItemUUID = dataItem.Id,
                        EquipPosition = slot
                    });
                    if (g2C_ReplaceEquip.Error == 0)
                    {
                        UIComponent.Instance.Remove(UIType.UIIntroduction);
                        uIIntroduction = null;
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ReplaceEquip.Error.GetTipInfo());
                    }
                }
                //是否穿戴了戒指
                /* 
                   //IL2cpp 需要重定向（暂时注释掉）
                  (bool ishave,int slot) IsRing() 
                 {

                     (bool,int)  result= (false, -1);
                     if (data.EquipmentPart != 11&& data.EquipmentPart != 12)
                     {
                         return result;
                     }
                     for (int i = 11; i <= 12; i++)
                     {
                         if (EquipmentComponent.curWareEquipsData_Dic.ContainsKey((E_Grid_Type)i))
                         {   

                             result = (true, i);
                             break;
                         }
                     }
                     return result;
                 }*/
                bool IsRing()
                {
                    bool result = false;
                    if (data.EquipmentPart != 11 && data.EquipmentPart != 12)
                    {
                        return result;
                    }
                    for (int i = 11; i <= 12; i++)
                    {
                        if (EquipmentComponent.curWareEquipsData_Dic.ContainsKey((E_Grid_Type)i))
                        {

                            result = true;
                            ringSlot = i;
                            break;
                        }
                    }
                    return result;
                }
            }
            else
            {
              //  uIIntroduction.GetMerherType(E_Grid_Type.Gem_Merge);
                ShowBaseAtr();
            }


            void ShowBaseAtr()
            {
                pos += Vector3.left / 2;

                if (isVertical)
                {
                    //垂直显示装备属性
                    bool isShow = false;
                    bool isSell = false;
                    bool isBuy = false;
                    bool isUser = false;
                    bool isShare = false;
                    bool isListring = false;
                    bool isRenew = false;
                    if (curKnapsackState == E_KnapsackState.KS_Knapsack
                    && (int)E_Grid_Type.Weapon <= data.EquipmentPart &&
                    data.EquipmentPart <= (int)E_Grid_Type.TianYing
                    )
                    {
                        isShow = true;
                    }
                    if (curKnapsackState == E_KnapsackState.KS_Knapsack
                    && (dataItem.ItemType == (int)E_ItemType.SkillBooks
                                || dataItem.ItemType == (int)E_ItemType.Consumables
                                || dataItem.ItemType == (int)E_ItemType.Mounts)/* || dataItem.ItemType == (int)E_ItemType.Pet*/)
                    {
                       
                        isShow = true;
                        isUser = true;
                        if (dataItem.ConfigId == 260015 || dataItem.ConfigId == 260019 || dataItem.ItemType == (int)E_ItemType.Pet)
                        {
                            //天鹰、烈火凤凰 属于装备
                            isUser = false;
                        }
                    }

                    if (curKnapsackState == E_KnapsackState.KS_Shop && originArea.Grid_Type == E_Grid_Type.Knapsack)
                    {
                       
                        isSell = true;
                    }

                    if (curKnapsackState == E_KnapsackState.KS_Shop && originArea.Grid_Type == E_Grid_Type.Shop)
                    {
                        isBuy = true;
                    }

                    if (curKnapsackState == E_KnapsackState.KS_Knapsack && dataItem.ItemType >= (int)E_ItemType.Swords && dataItem.ItemType <= (int)E_ItemType.Mounts)
                    {
                        isShare = true;
                    }
                    originArea.ItemData.ConfigId.GetItemInfo_Out(out originArea.ItemData.item_Info);
                    isListring = false;
                    isListring = IsShangJia();
                    isRenew = false;
                    if (originArea.ItemData.ConfigId == 300001 || originArea.ItemData.ConfigId == 300002|| originArea.ItemData.ConfigId == 300003|| originArea.ItemData.ConfigId == 300004)
                    { 
                        Item_infoConfig item_Info;
                        originArea.ItemData.ConfigId.GetItemInfo_Out(out item_Info);
                        switch (originArea.ItemData.ConfigId)
                        {
                            case 300001:
                            case 300002:
                                uIIntroduction.SetRenewData(36,15, item_Info.Name);
                                break;
                            case 300003:
                            case 300004:
                                uIIntroduction.SetRenewData(56, 15, item_Info.Name);
                                break;
                            default:
                                break;
                        }
                        isRenew = true;
                    }
                    Log.DebugBrown("数据222" + isShow);
                    uIIntroduction.ShowAtr_Vertical(isShow, isSell, isBuy, isUser, isShare, isListring, isRenew);
                    uIIntroduction.SetVerticalPos(pos, 1);
                    //丢弃物品
                    uIIntroduction.VerticalDiscarACtion = () =>
                    {
                        if (curKnapsackState == E_KnapsackState.KS_Knapsack)
                        {
                            SceneName sceneName = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>();
                            if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法丢弃");

                            }
                            else if (originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法丢弃");

                            }
                            else if (originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法丢弃");

                            }
                            else if (originArea.ItemData.ConfigId == 320106 && roleEntity.IsSafetyZone)
                            {
                                List<string> list = new List<string>();
                                originArea.ItemData.GetItemName(ref list);
                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"安全区不能使用{list.ListToString().Split(',')[1]}");

                            }
                            else if ((originArea.ItemData.ConfigId == 320106 && !roleEntity.IsSafetyZone) && (sceneName == SceneName.XueSeChengBao || sceneName == SceneName.EMoGuangChang || sceneName == SceneName.GuZhanChang || sceneName == SceneName.kalima_map))
                            {
                                List<string> list = new List<string>();
                                originArea.ItemData.GetItemName(ref list);
                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"副本里不能使用{list.ListToString().Split(',')[1]}");
                            }
                            //else if (!originArea.ItemData.IsCanTrade()) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                            //{
                            //    UIComponent.Instance.VisibleUI(UIType.UIHint, "该物品不能丢弃");
                            //    ResetGridObj();
                            //}
                            else
                            {
                                //丢弃物品
                                SendDiscardKnasackItemMessage().Coroutine();
                                CloseIntroduction();
                             
                            }
                        }
                    };
                    //装备
                    uIIntroduction.VerticalWareAction = () =>
                    {
                        curChooseArea.Grid_Type = (E_Grid_Type)data.EquipmentPart;
                        if (!IsCanWearEquip(ref curWarePart))
                        {
                            //不能穿戴
                            // ResetGridObj();
                        }
                        else
                        {
                           
                            curChooseArea.UUID = dataItem.UUID;
                            curChooseArea.ItemData = dataItem;
                            curWarePart = data.EquipmentPart;//当前选择的格子的类型
                            RequestWareEquip().Coroutine();  //请求穿戴装备
                            UIComponent.Instance.Remove(UIType.UIIntroduction);
                            uIIntroduction = null;
                        }
                    };
                    //续费
                    uIIntroduction.VerticalRenewAction = () =>
                    {
                      //  RenewItemRequest().Coroutine();  //请求穿戴装备
                        UIComponent.Instance.Remove(UIType.UIIntroduction);
                        uIIntroduction = null;
                    };
                    //使用物品
                    uIIntroduction.VerticalUserAction = () =>
                    {
                        if (KnapsackItemsManager.KnapsackItems.TryGetValue(grid.Data.UUID, out KnapsackDataItem dataItem))
                        {
                            //技能书、消耗品、坐骑 可以 长按使用
                            if (dataItem.ItemType == (int)E_ItemType.SkillBooks
                                || dataItem.ItemType == (int)E_ItemType.Consumables
                                || dataItem.ItemType == (int)E_ItemType.Mounts)
                            {
                                if (dataItem.ItemType == (int)E_ItemType.Mounts)
                                {
                                    if (roleEntity.IsSafetyZone && dataItem.ConfigId != 260020)
                                    {
                                        UIComponent.Instance.VisibleUI(UIType.UIHint, "安全区无法使用坐骑");
                                        return;
                                    }

                                    if (!dataItem.item_Info.IsCanUer((int)roleEntity.RoleType, roleEntity.ClassLev))
                                    {
                                        UIComponent.Instance.VisibleUI(UIType.UIHint, "不满足使用要求");
                                        return;
                                    }
                                }
                                else if (dataItem.ConfigId == 310102)//改名卡
                                {
                                    CloseIntroduction();
                                    UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent(E_TipPanelType.ChangeName);
                                    confirmComponent.ChangeNameTitleTxt.text = "输入角色名";
                                    confirmComponent.ChangeNameEventAction = async () =>
                                    {
                                        string roleName = confirmComponent.ChangeNameFunc?.Invoke();
                                        if (string.IsNullOrEmpty(roleName))
                                        {
                                            UIComponent.Instance.VisibleUI(UIType.UIHint, "角色名 不能为空");
                                            return;
                                        }
                                        //判断输入的内容是否包含 违规字符
                                        if (SystemUtil.IsInvaild(roleName))
                                        {
                                            //替换掉违规字符
                                            roleName = SystemUtil.ReplaceStr(roleName);
                                        }

                                        G2C_BagChangeNameCardResponse g2C_BagChangeNameCard = (G2C_BagChangeNameCardResponse)await SessionComponent.Instance.Session.Call(new C2G_BagChangeNameCardRequest 
                                        {
                                          Name=roleName,
                                          ItemUUID= dataItem.Id,
                                        });
                                        if (g2C_BagChangeNameCard.Error != 0)
                                        {
                                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BagChangeNameCard.Error.GetTipInfo());
                                        }
                                        else
                                        {

                                            
                                            roleEntity.GetComponent<UIUnitEntityHpBarComponent>().SetEntityName($"<b>{roleName}</b>", ColorTools.GetColorHtmlString(Color.yellow));
                                            roleEntity.RoleName=roleName;

                                            UIComponent.Instance.Remove(UIType.UIConfirm);
                                            UIComponent.Instance.VisibleUI(UIType.UIHint, $"改名成功");

                                        }
                                    };

                                    return;
                                }

                                PlayerUserItemInTheBackpack(dataItem.Id).Coroutine();

                            }
                        }
                    };

                    //出售
                    uIIntroduction.VerticalSellAction = () =>
                    {

                        //if (originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                        //{
                        //    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法出售");

                        //}
                         if (originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法出售");

                        }
                        else if (originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定 无法出售");

                        }
                       /* else if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法出售");

                        }*/
                        else
                        {
                            CloseIntroduction();
                            //提示玩家是否要卖出 改物品
                            var confirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                            originArea.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                            confirm.SetTipText($"是否将<color=yellow>{item_Info.Name}</color> 以<color=red>{originArea.ItemData.GetProperValue(E_ItemValue.SellMoney)}金币</color>出售？");
                            confirm.AddActionEvent(async () =>
                            {
                                //确定将 物品 出售
                                G2C_SellingItemToNPCShop g2C_SellingItemToNPC = (G2C_SellingItemToNPCShop)await SessionComponent.Instance.Session.Call(new C2G_SellingItemToNPCShop
                                {
                                    NPCShopID = CurNpcUUid, //商店NPC Id
                                    ItemUUID = this.originArea.UUID //卖出的物品的 UUID
                                });
                                if (g2C_SellingItemToNPC.Error != 0)
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SellingItemToNPC.Error.GetTipInfo());
                                   
                                }
                            });
                        }
                    };
                    //购买
                    uIIntroduction.VerticalBuyAction = () =>
                    {
                        //判断金币是否足够 购买改物品
                        if (originArea.ItemData.GetProperValue(E_ItemValue.BuyMoney) > roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin))
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足 无法购买");

                            return;
                        }
                        //获取背包中的位置
                        originArea.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig config);
                        if (originArea.ItemData==null|| config == null)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "请重新选择 要购买的装备");
                            return;
                        }
                        Vector2Int vector2Int = KnapsackTools.GetKnapsackItemUnoccupied(KnapsackTools.CreateGridData(0, 0, config.X - 1, config.Y - 1));
                        if (vector2Int.x == -1)
                        {
                            vector2Int = KnapsackTools.GetKnapsackItemUnoccupied(KnapsackTools.CreateGridData(0, 0, config.X - 1, config.Y - 1));
                            if (vector2Int.x == -1)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "背包不足，无法购买");
                                return;
                            }
                            else
                            {
                                BuyItem().Coroutine();
                            }
                        }
                        else
                        {
                            BuyItem().Coroutine();
                        }


                        async ETVoid BuyItem() 
                        {
                            /////玩家购买NPC商店的物品到背包 购买成功会推送物品进入背包
                            G2C_BuyItemFromNPCShop g2C_BuyItemFromNPC = (G2C_BuyItemFromNPCShop)await SessionComponent.Instance.Session.Call(new C2G_BuyItemFromNPCShop
                            {
                                NPCUUID = CurNpcUUid,
                                ItemUUID = this.originArea.UUID,
                                PosInBackpackX = vector2Int.x,
                                PosInBackpackY = vector2Int.y,
                                RemoteBuy = (int)this.buyType  //0  默认购买=1//远程

                            });
                            if (g2C_BuyItemFromNPC.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BuyItemFromNPC.Error.GetTipInfo());

                            }
                            else
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "购买成功");
                            }
                        }
                    };
                    //分享
                    uIIntroduction.VerticalShareAction = async () =>
                    {
                        //dataItem.GetItemInfo_Out(out item_Info);
                        string lev = dataItem.GetProperValue(E_ItemValue.Level) > 0 ? "+" + dataItem.GetProperValue(E_ItemValue.Level) : "";
                        string itemName;
                        int color = -1;
                        if (dataItem.GetSuitName().Item1)//有套装属性
                        {
                            if (lev != "")
                                itemName = $"{dataItem.GetSuitName().Item2} {dataItem.item_Info.Name} {lev}";
                            else
                                itemName = $"{dataItem.GetSuitName().Item2} {dataItem.item_Info.Name}";
                            color = 5;
                        }
                        else if (dataItem.IsHaveExecllentEntry) //卓越属性
                        {
                            if (lev != "")
                                itemName = $"卓越的 {dataItem.item_Info.Name}\t{lev}";
                            else
                                itemName = $"卓越的 {dataItem.item_Info.Name}";
                            color = 4;
                        }
                        else
                        {
                            //默认装备
                            if (lev != "")
                                itemName = $"{dataItem.item_Info.Name} {lev}";
                            else
                                itemName = $"{dataItem.item_Info.Name}";
                            color = 3;
                        }
                       // Log.DebugGreen($"分享装备   id {this.originArea.UUID}   名字{itemName}");
                        G2C_BagShareResponse g2C_BagShare = (G2C_BagShareResponse)await SessionComponent.Instance.Session.Call(new C2G_BagShareRequest()
                        {
                            ShareItemId = this.originArea.UUID,
                            Color = color,
                            MessageInfo = itemName
                        });
                        if(g2C_BagShare.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BagShare.Error.GetTipInfo());
                        }
                        else
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "分享成功！");
                            UIComponent.Instance.Remove(UIType.UIIntroduction);
                            uIIntroduction = null;
                        }
                    };
                    uIIntroduction.SellAction = (int value) =>
                    {
                      //  Log.DebugGreen("上架2");
                        listingTreasureHouse(dataItem.Id, value).Coroutine();
                    };
                }
                else
                {
                    uIIntroduction.ShowAtrs();
                    uIIntroduction.SetPos(pos, 1);
                }

            }

            //获取价格显示
            E_KnapsackIntroduceShowPrice GetPticeType() => (curKnapsackState, grid_Type) switch
            {
                (E_KnapsackState.KS_Shop, E_Grid_Type.Shop) => E_KnapsackIntroduceShowPrice.BuyPrice,
                (E_KnapsackState.KS_Shop, E_Grid_Type.Knapsack) => E_KnapsackIntroduceShowPrice.SellPrice,
                (E_KnapsackState.KS_Stallup, E_Grid_Type.Stallup) => E_KnapsackIntroduceShowPrice.StallSellPrice,
                (E_KnapsackState.KS_Stallup_OtherPlayer, E_Grid_Type.Stallup_OtherPlayer) => E_KnapsackIntroduceShowPrice.StallBuyPrice,
                _ => E_KnapsackIntroduceShowPrice.None
            };
        }

        public bool IsShangJia()
        {
            bool isListring = false;
            if (originArea.ItemData.item_Info.Sell == 0)
            {
                isListring = false;
            }
            else
            {
                //Log.DebugBrown($"时间->{originArea.ItemData.GetProperValue(E_ItemValue.ValidTime)}");
                if (originArea.ItemData.GetProperValue(E_ItemValue.ValidTime) > 0)//拥有限时的物品不能上架
                {
                    return isListring;
                }
                if (curKnapsackState != E_KnapsackState.KS_Shop)
                    if (originArea.ItemData.GetProperValue(E_ItemValue.IsTask) != 1)//任务物品
                        if (originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) != 1)//锁定物品
                            if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) != 1)//未绑定物品
                            {
                                //装备类
                                if ((originArea.ItemData.ItemType <= (int)E_ItemType.Boots && originArea.ItemData.ItemType >= (int)E_ItemType.Swords) ||
                                    (originArea.ItemData.ItemType <= (int)E_ItemType.Dangler && originArea.ItemData.ItemType >= (int)E_ItemType.Rings) ||
                                    (originArea.ItemData.ItemType == (int)E_ItemType.QiZhi))
                                {
                                    if (originArea.ItemData.GetHaveInLayAtr() || (originArea.ItemData.GetProperValue(E_ItemValue.SetId) is int value1 && value1 != 0))//镶嵌装或者套装
                                    {
                                        isListring = true;
                                    }
                                    else
                                    if (originArea.ItemData.ExecllentEntryDic.Count >= 1)
                                    {
                                        isListring = true;
                                    }
                                }//其他类
                                else
                                {
                                    isListring = true;
                                }
                                if((int)originArea.ItemData.ConfigId == 350001)//试用宠物
                                {
                                    isListring = false;
                                }
                            }
            }
            if (isListring)
            {
             //   Log.DebugGreen("可以上架");
            }
            else
            {
                //Log.DebugGreen("不可以上架");
            }
            return isListring;
        }

        private void OnPointerEnter(int x, int y, E_Grid_Type type)
        {
            if (x == -1)
            {
                //删除区域
                curChooseArea.SetSinglePoint(new Vector2Int(-1, -1));
                curChooseArea.Grid_Type = E_Grid_Type.Delete;
               
                return;
            }

            if (type == E_Grid_Type.Inlay)
            {
                ///镶嵌 面板
                OnPointerEnter_Inlay(type, y);
                return;
            }
            //没有拖拽 直接返回
            if (!isDroping) return;

            //记录当前进入的格子 信息
            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, type);

            if (originArea.IsSinglePoint)
            {
                curChooseArea.SetSinglePoint(new Vector2Int(x, y));
                grids[x][y].ReadyColor();
                curChooseArea.Grid_Type = grids[x][y].Grid_Type;
            }
            else
            {
                Vector2Int centerOffer = GetCenterGrid();
                x += centerOffer.x;
                y += centerOffer.y;

                Vector2Int offset = new Vector2Int(x, y) - originArea.Point1;

                Vector2Int endPoint = originArea.Point2 + offset;

                List<KnapsackGrid> gridList = GetAreaGrids(x, y, endPoint.x, endPoint.y, LENGTH_X, LENGTH_Y);

                if (gridList.Count == 0) return;
                foreach (KnapsackGrid item in gridList)
                {
                    item.ReadyColor();
                }
                curChooseArea.Point1 = new Vector2Int(x, y);
                curChooseArea.Point2 = new Vector2Int(endPoint.x, endPoint.y);
                curChooseArea.Grid_Type = grids[x][y].Grid_Type;
            }
            //Log.DebugGreen($"进入背包格子：起始格子类型:{originArea.Grid_Type} 目标格子类型:{curChooseArea.Grid_Type}");
        }

        private void OnEndDrag(int x, int y, E_Grid_Type grid_Type)
        {

            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, curChooseArea.Grid_Type);
            if (isDroping == false || curDropObj == null) return;
            //起始区域是背包
            #region 从背包 拖拽到 其他面板
            //背包 拖到 丢弃
            if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Delete))
            {
                if (curKnapsackState == E_KnapsackState.KS_Knapsack)
                {
                    SceneName sceneName = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>();
                    if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法丢弃");
                        ResetGridObj();
                    }
                    //else if (originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                    //{
                    //    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法丢弃");
                    //    ResetGridObj();
                    //}
                    else if (originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法丢弃");
                        ResetGridObj();
                    }
                    else if (originArea.ItemData.ConfigId == 320106 && roleEntity.IsSafetyZone)
                    {
                        List<string> list = new List<string>();
                        originArea.ItemData.GetItemName(ref list);
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"安全区不能使用{list.ListToString().Split(',')[1]}");
                        ResetGridObj();
                    }
                    //else if (originArea.ItemData.ItemType == (int)E_ItemType.Gemstone || originArea.ItemData.ItemType == (int)E_ItemType.FGemstone)
                    //{
                    //    UIComponent.Instance.VisibleUI(UIType.UIHint, "宝石类，无法丢弃");
                    //    ResetGridObj();
                    //}
                    else if ((originArea.ItemData.ConfigId == 320106 && !roleEntity.IsSafetyZone) && (sceneName == SceneName.XueSeChengBao || sceneName == SceneName.EMoGuangChang || sceneName == SceneName.GuZhanChang || sceneName == SceneName.kalima_map))
                    {
                        List<string> list = new List<string>();
                        originArea.ItemData.GetItemName(ref list);
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"副本里不能使用{list.ListToString().Split(',')[1]}");
                        ResetGridObj();
                    }
                    //else if (!originArea.ItemData.IsCanTrade()) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                    //{
                    //    UIComponent.Instance.VisibleUI(UIType.UIHint, "该物品不能丢弃");
                    //    ResetGridObj();
                    //}
                    else
                    {
                        //丢弃物品
                        SendDiscardKnasackItemMessage().Coroutine();
                    }
                }
                else
                {
                    ResetGridObj();
                }
            }
            //不存在进入的格子 则回到原始位置  
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.None))
            {
                ResetGridObj();
            }
            // 背包 内部 拖拽
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Knapsack))
            {
                if (originArea.IsSinglePoint)//// 物品占 nxn 个格子
                {
                    if (grids[curChooseArea.Point1.x][curChooseArea.Point2.y].IsOccupy == false)//目标区域没有被占用
                    {
                        //请求服务端 改变物品的位置信息
                        SendMoveKnapsackItemMessage().Coroutine();

                    }
                    else//目标区域 被占用 则判断物品是否可以合并
                    {
                        long targetUid = BackGrids[curChooseArea.Point1.x][curChooseArea.Point1.y].Data.UUID;//目标物品的uuid
                                                                                                             //判断是否可以 合并
                        KnapsackDataItem targetKnapsackDataItem = BackGrids[curChooseArea.Point1.x][curChooseArea.Point1.y].Data.ItemData;

                        KnapsackItemsManager.KnapsackItems.TryGetValue(targetUid, out KnapsackDataItem targetItem); //目标物品的 实体类
                        KnapsackItemsManager.KnapsackItems.TryGetValue(originArea.UUID, out KnapsackDataItem usedataItem); //当前正在移动物品的 实体类
                        //配置表ID相等 堆叠数大于1 强化等级相等
                        targetItem.ConfigId.GetItemInfo_Out(out targetItem.item_Info);
                        usedataItem.ConfigId.GetItemInfo_Out(out usedataItem.item_Info);
                        //Log.DebugGreen($"{usedataItem.ConfigId}--->{targetKnapsackDataItem.ConfigId}");
                        if (usedataItem.ConfigId == targetItem.ConfigId && usedataItem.Id != targetItem.Id && usedataItem.item_Info.StackSize > 1 && targetItem.item_Info.StackSize > 1 && usedataItem.GetProperValue(E_ItemValue.Level) == targetItem.GetProperValue(E_ItemValue.Level))
                        {
                            //请求服务器 叠加物品
                            MergerItem().Coroutine();
                        }
                        //使用的物品是宝石 强化装备
                        else if (usedataItem.IsCanDragUser())
                        {
                            //请求服务器 使用物品
                            MergerItem().Coroutine();
                        }
                        else if (usedataItem.IsTheTresureKey()&& targetKnapsackDataItem.IsCanOpen())
                        {

                            if( (usedataItem.ConfigId == 320433 && targetKnapsackDataItem.ConfigId == 320407) ||
                                (usedataItem.ConfigId == 320434 && targetKnapsackDataItem.ConfigId == 320408) ||
                                (usedataItem.ConfigId == 320435 && targetKnapsackDataItem.ConfigId == 320409))
                            //宝箱钥匙打开宝箱
                            KeyOpenTreasure().Coroutine();
                            else
                            {
                                ResetGridObj();
                                return;
                            }
                        }
                        else
                        {
                            ResetGridObj();
                            return;
                        }

                        /*
                         合并、升级背包中单个物品
                         如两个 药水 数量未到上限，可以进行合并
                         武器、防具等升级+1~+9
                         武器、防具追加属性
                         武器、防具等再生属性添加、进化
                         */
                        async ETVoid MergerItem()
                        {
                            G2C_MergeSingleItems g2C_Merge = (G2C_MergeSingleItems)await SessionComponent.Instance.Session.Call(new C2G_MergeSingleItems
                            {
                                ItemUUID = originArea.UUID,
                                TargetItemUUID = targetUid
                            });
                            if (g2C_Merge.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Merge.Error.GetTipInfo());
                                ResetGridObj();
                            }
                            else
                            {
                                if(KnapsackItemsManager.KnapsackItems.ContainsKey(usedataItem.Id)&&usedataItem.GetProperValue(E_ItemValue.Quantity)>0)
                                ResetGridObj();
                            }
                            
                        }
                        //宝箱钥匙打开宝箱协议
                        async ETVoid KeyOpenTreasure() {
                            G2C_OpenTheSpecialTreasureChestResponse g2C_Open = (G2C_OpenTheSpecialTreasureChestResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenTheSpecialTreasureChestRequest
                            {
                                ChestkeyId = originArea.UUID,
                                TreasureChestId = targetKnapsackDataItem.UUID
                            }) ;
                            if (g2C_Open.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Open.Error.GetTipInfo());
                                ResetGridObj();
                            }
                            else
                            {
                                if (KnapsackItemsManager.KnapsackItems.ContainsKey(usedataItem.Id) && usedataItem.GetProperValue(E_ItemValue.Quantity) > 0)
                                    ResetGridObj();
                            }
                        }
                    }
                }
                else//物品占用的格子数 不是nxn 而是（2x3 ....等）
                {
                   
                    // bool isOrigin = targetUUID == originArea.UUID;//目标位置 是否与起始位置重合
                    if (ContainGridObj(curChooseArea.Point1.x, curChooseArea.Point1.y, curChooseArea.Point2.x, curChooseArea.Point2.y))//格子为被占用||isOrigin
                    {
                        //格子被占用 回到起始位置
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "目标格子已被占用");
                        ResetGridObj();
                    }
                    else
                    {
                        //请求服务端 改变物品的位置信息
                        SendMoveKnapsackItemMessage().Coroutine();
                    }
                }
            }
            //背包 拖到 装备栏
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Weapon)//武器
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Shield)//盾牌
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Helmet)//头盔
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Armor)//铠
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Leggings)//护腿
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.HandGuard)//护手
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Boots)//鞋子
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Wing)//翅膀
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Guard)//守护
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Necklace)//项链
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.LeftRing)//左戒指
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.RightRing)//右戒指
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Flag)//旗帜
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Pet)//宠物
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.WristBand)//手环
                  || (originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.TianYing)//天鹰
                )
            {
                KnapsackGrid grid = EquipmentPartDic[curChooseArea.Grid_Type];
                if (grid.IsOccupy)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "部位已有装备 请先卸载了 再装备");
                    ResetGridObj();
                }
                else
                {
                    if (!IsCanWearEquip(ref curWarePart))
                    {
                        //不能穿戴
                        ResetGridObj();
                    }
                    else
                    {
                        curChooseArea.UUID = originArea.UUID;
                        curChooseArea.ItemData = originArea.ItemData;

                        curWarePart = (int)curChooseArea.Grid_Type;//当前选择的格子的类型
                        //请求穿戴装备
                        RequestWareEquip().Coroutine();
                    }
                }
            }
            //背包 拖拽 到合成面板
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Gem_Merge))
            {
                Log.DebugGreen("拖拽到合成" + originArea.UUID);
                if (KnapsackItemsManager.KnapsackItems.TryGetValue(originArea.UUID, out KnapsackDataItem item))
                {
                    if (item.GetProperValue(E_ItemValue.IsUsing) == 1)//检查物品是否处于使用中
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法合成");
                        ResetGridObj();
                        return;
                    }
                    if (item.GetProperValue(E_ItemValue.IsLocking) == 1)//检查物品是否处于使用中
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定 无法合成");
                        ResetGridObj();
                        return;
                    }
                    //if (item.GetProperValue(E_ItemValue.IsBind) == 1)//检查物品是否处于使用中
                    //{
                    //    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法合成");
                    //    ResetGridObj();
                    //    return;
                    //}
                    if (item.GetProperValue(E_ItemValue.IsTask) == 1)//检查物品是否处于使用中
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法合成");
                        ResetGridObj();
                        return;
                    }
                    //刷新检测方法
                    RefreshMergerMethods(item);

                    if (curMergerMethod != null)
                    {
                        ChangeItemPos(async () =>
                        {
                            //将物品移动到合成缓存区域
                            G2C_MoveBackpackItemToCacheSpace g2C_MoveBackpackItemToCache = (G2C_MoveBackpackItemToCacheSpace)await SessionComponent.Instance.Session.Call(new C2G_MoveBackpackItemToCacheSpace { MovedItemUUID = originArea.UUID });
                            if (g2C_MoveBackpackItemToCache.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MoveBackpackItemToCache.Error.GetTipInfo());

                            }
                            else
                            {
                              
                                curChooseArea.UUID = originArea.UUID;
                                curChooseArea.ItemData = originArea.ItemData;
                              
                                AddKnapsackItem(curChooseArea, curDropObj);
                              
                                AddMergerItem(item);
                                  RemoveItem(originArea,getGrid:true);
                                UpdateTips();
                            }
                        });
                    }
                    else
                    {
                        RefreshMergerMethods(null);
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "只能放入可以合成的物品1");
                        ResetGridObj();
                    }

                }

            }
            // 背包   拖拽到仓库
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Ware_House))
            {
                //if (originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法移动到仓库");
                //    ResetGridObj();
                //}
                 if (originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法移动到仓库");
                    ResetGridObj();
                }
                //else if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "绑定物品 无法移动到仓库");
                //    ResetGridObj();
                //}
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "锁定物品 无法移动到仓库");
                    ResetGridObj();
                }
                else
                {
                    ChangeItemPos(() =>
                    {
                        SendKnapsackItem2WareHouseMessage().Coroutine();
                    });
                }

            }
            // 背包   拖拽到NPC 商城
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Shop))
            {

                //if (originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法出售");
                //    ResetGridObj();
                //}
                if (originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法出售");
                    ResetGridObj();
                }
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定 无法出售");
                    ResetGridObj();
                }
                //else if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法出售");
                //    ResetGridObj();
                //}
                else
                {
                    curDropObj.transform.SetPositionAndRotation(originObjPos, originObjRotation);
                    //提示玩家是否要卖出 改物品
                    var confirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                    originArea.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                    confirm.SetTipText($"是否将<color=yellow>{item_Info.Name}</color> 以<color=red>{originArea.ItemData.GetProperValue(E_ItemValue.SellMoney)}金币</color>出售？");
                    confirm.AddActionEvent(async () =>
                    {
                        //确定将 物品 出售
                        G2C_SellingItemToNPCShop g2C_SellingItemToNPC = (G2C_SellingItemToNPCShop)await SessionComponent.Instance.Session.Call(new C2G_SellingItemToNPCShop
                        {
                            NPCShopID = CurNpcUUid, //商店NPC Id
                            ItemUUID = this.originArea.UUID //卖出的物品的 UUID
                        });
                        if (g2C_SellingItemToNPC.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SellingItemToNPC.Error.GetTipInfo());
                            ResetGridObj();
                        }
                        else
                        {
                            //从背包中移除
                            ResourcesComponent.Instance.RecycleGameObject(curDropObj);
                        }
                    });
                    confirm.AddCancelEventAction(() =>
                    {
                        //取消物品出售
                        ResetGridObj();
                    });


                }
            }
            // 背包   拖拽到自己的摊位
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Stallup))
            {
                if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法摆摊出售");
                    ResetGridObj();
                }
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法摆摊出售");
                    ResetGridObj();
                }
                //else if (originArea.ItemData.ItemType == (int)E_ItemType.Gemstone ||originArea.ItemData.ItemType == (int)E_ItemType.FGemstone)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "宝石类，无法丢弃");
                //    ResetGridObj();
                //}
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用状态 无法摆摊出售");
                    ResetGridObj();
                }
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定状态 无法摆摊出售");
                    ResetGridObj();
                }
                else if (originArea.ItemData.GetProperValue(E_ItemValue.ValidTime) != 0) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "限时物品 无法摆摊出售");
                    ResetGridObj();
                }
                else
                {
                    ChangeItemPos(() =>
                    {
                        curDropObj.transform.SetPositionAndRotation(originObjPos, originObjRotation);
                        UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent(E_TipPanelType.StallUp);
                        confirmComponent.coinprice = 0;
                        confirmComponent.yuanbaoprice = 0;
                        confirmComponent.StallUpEventAction = async () =>
                        {
                            var price = confirmComponent.GetStallUpGoldFunc?.Invoke();
                            var yuanbaoprice = confirmComponent.GetStallUpYuanBaoFunc?.Invoke();
                         
                            StallUpData = ComponentFactory.CreateWithId<KnapsackDataItem>(originArea.UUID);
                            StallUpData.GameUserId = this.roleEntity.Id;
                            StallUpData.UUID = originArea.UUID;
                            StallUpData.ConfigId = originArea.ItemData.ConfigId;
                            StallUpData.PosInBackpackX = curChooseArea.Point1.x;
                            StallUpData.PosInBackpackY = curChooseArea.Point1.y;
                            StallUpData.X = originArea.ItemData.X;
                            StallUpData.Y = originArea.ItemData.Y;
                            StallUpData.SetProperValue(E_ItemValue.Stall_SellPrice, (int)price);
                            StallUpData.SetProperValue(E_ItemValue.Stall_SellMoJingPrice, (int)yuanbaoprice);

                            /* curChooseArea.Grid_Type = E_Grid_Type.Stallup;
                             curChooseArea.UUID = originArea.UUID;
                             curChooseArea.ItemData = originArea.ItemData;
                             StallUpData = curChooseArea.ItemData;*/
                            // var obj = curDropObj;
                            //确定上架物品
                            G2C_BaiTanAddItemResponse g2C_BaiTanAddItem = (G2C_BaiTanAddItemResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanAddItemRequest
                            {
                                Prop = new C2G_BaiTanItemMessage
                                {
                                    ItemUUID = StallUpData.UUID,
                                    ConfigId = StallUpData.ConfigId,
                                    PosInBackpackX = curChooseArea.Point1.x,
                                    PosInBackpackY = curChooseArea.Point1.y,
                                    Price = price.ToInt32(),//金币价格
                                    Price2 = yuanbaoprice.ToInt32(),//魔晶价格
                                }
                            });
                            if (g2C_BaiTanAddItem.Error != 0)
                            {
                                StallUpData.Dispose();
                                StallUpData = null;
                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanAddItem.Error.GetTipInfo());
                             
                            }
                            else
                            {

                                /*curChooseArea.Grid_Type = E_Grid_Type.Stallup;
                                curChooseArea.UUID = originArea.UUID;
                                curChooseArea.ItemData = originArea.ItemData;
                                curChooseArea.ItemData.SetProperValue(E_ItemValue.Stall_SellPrice, (int)price);
                                curChooseArea.ItemData.SetProperValue(E_ItemValue.Stall_SellMoJingPrice, (int)yuanbaoprice);
                                Log.DebugGreen($"确认上架物品{obj == null}  {curChooseArea.ItemData == null}");

                                AddKnapsackItem(curChooseArea, obj);*/

                                //  RemoveItem(originArea);
                                UIComponent.Instance.InVisibilityUI(UIType.UIConfirm);
                            }

                        };
                        confirmComponent.StallUpCancelAction = () =>
                        {

                        };

                    });
                }
            }
            // 背包   拖拽到镶嵌
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Inlay))
            {
                if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法镶嵌");
                    ResetGridObj();
                }
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法镶嵌");
                    ResetGridObj();
                }
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用状态 无法镶嵌");
                    ResetGridObj();
                }
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定状态 无法镶嵌");
                    ResetGridObj();
                }
                else
                {

                    if (IsUsedItem())//物品是否已经添加
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "已经使用了该物品");
                        ResetGridObj();
                        return;
                    }
                    if (CheckItem(originArea.ItemData))//检查物品是否符合条件
                    {
                        if (CurInlayGrid.IsOccupy)//当前格子已经有物品
                        {
                            if (CurInlayGrid.curCount == CurInlayGrid.MaxCount)//当前所需要的材料 以达到最大数量
                            {
                                //满足条件 直接替换
                                RemoveInlayItem();
                                ResourcesComponent.Instance.DestoryGameObjectImmediate(CurInlayGrid.GridObj, CurInlayGrid.GridObj.name.StringToAB());//删除之前的
                                AddInlayItem(originArea, ResourcesComponent.Instance.LoadGameObject(curDropObj.name.StringToAB(), curDropObj.name));
                            }

                        }
                        else
                        {
                            //满足条件
                            AddInlayItem(originArea, ResourcesComponent.Instance.LoadGameObject(curDropObj.name.StringToAB(), curDropObj.name));
                            //改变 下方的数量显示
                            ChangeCountText(originArea.ItemData.GetProperValue(E_ItemValue.Quantity));

                        }

                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "物品不满足条件");
                    }
                    //物品返回背包原位置
                    ResetGridObj();
                }

            }
            // 背包   拖拽到寄售
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Consignment))
            {
                /* ChangeItemPos(async () =>
                 {


                 });*/
            }
            // 背包   拖拽到交易
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Trade))
            {
                if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法交易");
                    ResetGridObj();
                }
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法交易");
                    ResetGridObj();
                }
                //else if (originArea.ItemData.ItemType == (int)E_ItemType.Gemstone || originArea.ItemData.ItemType == (int)E_ItemType.FGemstone)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "宝石类，无法丢弃");
                //    ResetGridObj();
                //}
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用状态 无法交易");
                    ResetGridObj();
                }
                else if (originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定状态 无法交易");
                    ResetGridObj();
                }
                else if (originArea.ItemData.GetProperValue(E_ItemValue.ValidTime) != 0) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "限时物品 无法交易");
                    ResetGridObj();
                }
                //else if (!originArea.ItemData.IsCanTrade()) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "该物品不能交易");
                //    ResetGridObj();
                //}
                else
                {
                    ChangeItemPos(async () =>
                    {
                        if (CanTrade())
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "请先选择交易对象");
                            ResetGridObj();
                            return;
                        }
                        //放入交易物品
                        G2C_AddExchangeItem g2C_AddExchange = (G2C_AddExchangeItem)await SessionComponent.Instance.Session.Call(new C2G_AddExchangeItem
                        {
                            ItemUUID = originArea.UUID,
                            PosInBackpackX = curChooseArea.Point1.x,
                            PosInBackpackY = curChooseArea.Point1.y
                        });
                        if (g2C_AddExchange.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_AddExchange.Error.GetTipInfo());
                            ResetGridObj();
                        }
                        else
                        {
                       
                            curChooseArea.UUID = originArea.UUID;
                            curChooseArea.ItemData = originArea.ItemData;
                            curChooseArea.Grid_Type = E_Grid_Type.Trade;
                            AddKnapsackItem(curChooseArea, curDropObj);
                            //从背包中移除
                            RemoveItem(originArea);
                        }
                    });
                }
            }
            #endregion

            #region 从其他面板拖拽到背包
            //仓库 拖到 背包
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Ware_House, E_Grid_Type.Knapsack))
            {
                ChangeItemPos(() => SendWareHouse2KnapsackItemMessage().Coroutine());
            }

            //合成面板 拖到 背包
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Gem_Merge, E_Grid_Type.Knapsack))
            {
                ChangeItemPos(async () =>
                {

                    //移除缓存区域
                    G2C_MoveCacheSpaceItemToBackpack g2C_MoveCacheSpaceItemToBackpack = (G2C_MoveCacheSpaceItemToBackpack)await SessionComponent.Instance.Session.Call(new C2G_MoveCacheSpaceItemToBackpack
                    {
                        MovedItemUUID = originArea.UUID,
                        PosInBackpackX = this.curChooseArea.Point1.x,
                        PosInBackpackY = this.curChooseArea.Point1.y
                    });
                    Log.DebugBrown("合成拖拽到背包:" + originArea.UUID + ":x:" + this.curChooseArea.Point1.x + "::y" + this.curChooseArea.Point1.y);
                    if (g2C_MoveCacheSpaceItemToBackpack.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MoveCacheSpaceItemToBackpack.Error.GetTipInfo());
                        ResetGridObj();
                    }
                    else
                    {

                        ResourcesComponent.Instance.RecycleGameObject(this.curDropObj);
                        RemoveMergerItem();
                        RefreshMergerMethods(null);
                        originArea.ItemData.Dispose();
                        RemoveItem(originArea);
                        UpdateTips();
                    }

                });
            }
            //NPC商城 拖到 背包
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Shop, E_Grid_Type.Knapsack))
            {
                ChangeItemPos(async () =>
                {
                    //判断金币是否足够 购买改物品
                    if (originArea.ItemData.GetProperValue(E_ItemValue.BuyMoney) > roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin))
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足 无法购买");
                        ResetGridObj();
                        return;
                    }

                    /////玩家购买NPC商店的物品到背包 购买成功会推送物品进入背包
                    G2C_BuyItemFromNPCShop g2C_BuyItemFromNPC = (G2C_BuyItemFromNPCShop)await SessionComponent.Instance.Session.Call(new C2G_BuyItemFromNPCShop
                    {
                        NPCUUID = CurNpcUUid,
                        ItemUUID = this.originArea.UUID,
                        PosInBackpackX = this.curChooseArea.Point1.x,
                        PosInBackpackY = this.curChooseArea.Point1.y,
                        RemoteBuy = (int)this.buyType  //0  默认购买=1//远程

                    });
                    if (g2C_BuyItemFromNPC.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BuyItemFromNPC.Error.GetTipInfo());
                        ResetGridObj();
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "购买成功");
                        //物品回到 上传面板
                        ResetGridObj();
                    }
                });
            }
            //自己摊位 拖到 背包
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Stallup, E_Grid_Type.Knapsack))
            {
                ChangeItemPos(async () =>
                {
                    G2C_BaiTanRemoveItemResponse g2C_BaiTanRemoveItem = (G2C_BaiTanRemoveItemResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanRemoveItemRequest
                    {
                        Prop = new C2G_BaiTanItemMessage
                        {
                            ItemUUID = this.originArea.UUID,
                            ConfigId = originArea.ItemData.ConfigId,
                            PosInBackpackX = curChooseArea.Point1.x,
                            PosInBackpackY = curChooseArea.Point1.y,
                            Price = 0//价格
                        }
                    });
                    if (g2C_BaiTanRemoveItem.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanRemoveItem.Error.GetTipInfo());
                    }
                    else
                    {
                        ResourcesComponent.Instance.RecycleGameObject(curDropObj);
                        RemoveStallUpItem(originArea.UUID);
                        RemoveItem(originArea);
                    }

                });
            }
            //其他玩家的摊位 拖到 背包
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Stallup_OtherPlayer, E_Grid_Type.Knapsack))
            {
                if (UnitEntityComponent.Instance.AllUnitEntityDic.ContainsKey(otherRole.Id) == false)
                {
                    ResetGridObj();
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "玩家已经下线 无法购买");

                }
                else if (otherRole.GetComponent<RoleStallUpComponent>().IsStallUp == false)
                {
                    ResetGridObj();
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "对方以摊位已关闭，无法购买");
                }
                else
                {
                    ChangeItemPos(() =>
                    {
                        UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                        originArea.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                        var jinbi = originArea.ItemData.GetProperValue(E_ItemValue.Stall_BuyPrice);
                        var yuanbao = originArea.ItemData.GetProperValue(E_ItemValue.Stall_BuyMoJingPrice);

                        if (jinbi != 0 && yuanbao != 0)
                        {
                            confirmComponent.SetTipText($"确定花费<color=red>{jinbi}</color>金币+<color=red>{yuanbao}</color>魔晶购买 <{item_Info.Name}>？");
                        }
                        else if (jinbi != 0)
                        {
                            confirmComponent.SetTipText($"确定花费<color=red>{jinbi}</color>金币购买 <{item_Info.Name}>？");
                        }
                        else if (yuanbao != 0)
                        {
                            confirmComponent.SetTipText($"确定花费<color=red>{yuanbao}</color>魔晶购买 <{item_Info.Name}>？");
                        }

                        confirmComponent.AddActionEvent((Action)(async () =>
                        {
                            //判断金币 是否足够
                            if (jinbi > roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin))
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足");
                                ResetGridObj();
                                return;
                            }
                            //判断元宝 是否足够
                            if (yuanbao > roleEntity.Property.GetProperValue((E_GameProperty)E_GameProperty.MoJing))
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "魔晶不足");
                                ResetGridObj();
                                return;
                            }

                            //请求购买
                           
                            G2C_BaiTanBuyItemResponse g2C_BaiTanBuyItemResponse = (G2C_BaiTanBuyItemResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanBuyItemRequest
                            {
                                BaiTanInstanceId = ClickSelectUnitEntityComponent.Instance.curSelectUnit.Id,
                                ItemUUID = this.originArea.UUID,
                                PosInBackpackX = curChooseArea.Point1.x,
                                PosInBackpackY = curChooseArea.Point1.y,
                            });
                            if (g2C_BaiTanBuyItemResponse.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanBuyItemResponse.Error.GetTipInfo());
                                ResetGridObj();
                            }
                            else
                            {
                                ResourcesComponent.Instance.RecycleGameObject(curDropObj);
                                RemoveItem(originArea);
                            }

                        }));
                        confirmComponent.AddCancelEventAction(() =>
                        {
                            ResetGridObj();
                        });


                    });
                }
            }
            //属性还原面板 拖到 背包
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Reduction, E_Grid_Type.Knapsack))
            {
                ChangeItemPos(async () =>
                {


                });
            }
            //镶嵌 拖到 背包
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Inlay, E_Grid_Type.Knapsack))
            {
                /*  ChangeItemPos(async () =>
                  {


                  });*/
            }
            //交易 拖到 背包
            else if ((originArea.Grid_Type, curChooseArea.Grid_Type) is (E_Grid_Type.Trade, E_Grid_Type.Knapsack))
            {
                ChangeItemPos(async () =>
                {
                    ///移除交易物品
                    G2C_ReMoveExchangeItem g2C_MoveExchange = (G2C_ReMoveExchangeItem)await SessionComponent.Instance.Session.Call(new C2G_ReMoveExchangeItem { ItemUUID = originArea.UUID });
                    if (g2C_MoveExchange.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MoveExchange.Error.GetTipInfo());
                    }
                    else
                    {
                        AddKnapsackItem(curChooseArea, curDropObj);
                        RemoveItem(originArea);
                    }

                });
            }
            #endregion
            //其他 面板 内部拖拽
            else if (originArea.Grid_Type == curChooseArea.Grid_Type)
            {
                ChangeItemPos(async () =>
                {
                    if (curChooseArea.Grid_Type == E_Grid_Type.Ware_House)//仓库 内部拖拽
                    {
                        //请求服务端 改变物品的位置信息
                        MoveWarehouseItemAsync().Coroutine();
                    }
                    else if (curChooseArea.Grid_Type == E_Grid_Type.Trade)//交易面板 内部拖拽
                    {

                        MoveExchangeItem().Coroutine();
                        ///移动交易物品的位置
                        async ETVoid MoveExchangeItem()
                        {
                            G2C_MoveExchangeItem g2C_MoveExchange = (G2C_MoveExchangeItem)await SessionComponent.Instance.Session.Call(new C2G_MoveExchangeItem
                            {
                                ItemUUID = originArea.UUID,
                                PosInBackpackX = curChooseArea.Point1.x,
                                PosInBackpackY = curChooseArea.Point1.y
                            });
                            if (g2C_MoveExchange.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MoveExchange.Error.GetTipInfo());
                                ResetGridObj();
                            }
                            else
                            {
                                AddKnapsackItem(curChooseArea, curDropObj);
                                RemoveItem(originArea);
                            }
                        }
                    }
                    else if (curChooseArea.Grid_Type == E_Grid_Type.Stallup)//摆摊面板 内部拖拽
                    {
                        G2C_BaiTanChangeDataResponse g2C_BaiTanChangeData = (G2C_BaiTanChangeDataResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanChangeDataRequest
                        {
                            Prop = new C2G_BaiTanItemMessage
                            {
                                ConfigId = originArea.ItemData.ConfigId,
                                ItemUUID = originArea.UUID,
                                PosInBackpackX = curChooseArea.Point1.x,
                                PosInBackpackY = curChooseArea.Point1.y
                            }
                        });
                        if (g2C_BaiTanChangeData.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanChangeData.Error.GetTipInfo());
                            ResetGridObj();
                        }
                        else
                        {
                            curChooseArea.UUID = originArea.UUID;
                            curChooseArea.ItemData = originArea.ItemData;
                            AddKnapsackItem(curChooseArea, curDropObj);
                            //改变物品的位置
                            RemoveItem(originArea);

                        }
                    }
                    else
                    {
                        //回到起始位置
                        ResetGridObj();
                    }
                });
            }
            //起始区域不是背包 不能丢弃
            else
            {
                ResetGridObj();
             
            }


            this.isDroping = false;
        }

        private void OnBeginDrag(int x, int y, E_Grid_Type grid_Type)
        {
            if (uIIntroduction != null)
            {
                UIComponent.Instance.Remove(UIType.UIIntroduction);
                uIIntroduction = null;
            }
            if (curChooseArea.Grid_Type == E_Grid_Type.Delete) return;

            if (grid_Type == E_Grid_Type.Inlay || grid_Type == E_Grid_Type.Trade_Other || IsSplit)
            {
                ///镶嵌 面板

                return;
            }

            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, grid_Type);
            KnapsackGrid grid = grids[x][y];//获取当前格子
            if (grid.IsOccupy == false)//判断当前格子是否有物品
                return;
            useItem = null;
            StopUseItem();//停止正在使用的物品

            isDroping = true;

            originArea = grid.Data;//记录当前选择的格子区域
            originArea.Grid_Type = grid.Grid_Type;
            originObjPos = grid.GridObj.transform.localPosition;
            originObjRotation = grid.GridObj.transform.localRotation;

            curDropObj = grid.GridObj;
           
        }
        #endregion

        /// <summary>
        /// 关闭简介面板
        /// </summary>
        public void CloseIntroduction()
        {
            if (uIIntroduction != null)
            {
                UIComponent.Instance.Remove(UIType.UIIntroduction);
                uIIntroduction = null;
            }
        }

        /// <summary>
        /// 改变物品的位置
        /// </summary>
        /// <param name="action">请求 服务端的回调函数</param>
        public void ChangeItemPos(Action action)
        {
            if (originArea.IsSinglePoint)
            {
                if (grids[curChooseArea.Point1.x][curChooseArea.Point2.y].IsOccupy == false)//目标区域没有被占用
                {
                    //请求服务端 
                    action?.Invoke();
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "此处无法放置物品");
                    ResetGridObj();
                }
            }
            else
            {
            //    long targetUUID = grids[curChooseArea.Point1.x][curChooseArea.Point1.y].Data.UUID;
              //  bool isOrigin = targetUUID == originArea.UUID;//目标位置 是否与起始位置重合
                if (ContainGridObj(curChooseArea.Point1.x, curChooseArea.Point1.y, curChooseArea.Point2.x, curChooseArea.Point2.y))//格子为被占用//|| isOrigin
                {
                    //格子被占用 回到起始位置
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "此处无法放置物品");
                    ResetGridObj();
                }
                else
                {

                    //请求服务端 物品从背包移动到合成面板
                    action?.Invoke();
                }
            }
        }

        /// <summary>
        /// 取消正在使用的物品
        /// </summary>
        private void StopUseItem()
        {
            StartUse = false;
            dianjiImage.SetActive(false);
            filledImage.fillAmount = 0;

        }
        public void StartUseItem(KnapsackGridData item)
        {
            StartUse = true;
            userTime = Time.time + userItemTime;
            useItem = item;
            IsUse = false;
        }
        /// <summary>
        /// 关闭背包
        /// </summary>
        public void CloseKnapsack()
        {
            if (IsHavaItem())
                return;
            PanelManager(false);
            CleanKnapsack();
            UIComponent.Instance.Remove(UIType.UIKnapsack);
        }


        /// <summary>
        /// 从背包中移除物品
        /// </summary>
        /// <param name="data"></param>
        public void RemoveItem(KnapsackGridData data, bool isRecycle = false,bool getGrid=false)
        {
            bool isReset = TargetNull(data);
            //lock (removeItemLock)
            {
                if (!isRecycle || getGrid)
                {
                    GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, data.Grid_Type);

                    if (data.Grid_Type == E_Grid_Type.Knapsack)
                    {
                        KnapsackTools.RemoveEquip(data);
                        grids[data.Point1.x][data.Point1.y].Image.transform.Find("up").gameObject.SetActive(false);

                    }
                }



                GameObject recycleGo = null;
                if (data.IsSinglePoint)
                {
                    recycleGo = grids[data.Point1.x][data.Point1.y].GridObj;

                    grids[data.Point1.x][data.Point1.y].GridObj = null;
                    grids[data.Point1.x][data.Point1.y].IsOccupy = false;
                    grids[data.Point1.x][data.Point1.y].Data.EquipmentPart = 0;
                    grids[data.Point1.x][data.Point1.y].Data.UUID = 0;

                    grids[data.Point1.x][data.Point1.y].Data.ItemData = null;


                }
                else
                {
                    List<KnapsackGrid> grids = GetAreaGrids(data.Point1.x, data.Point1.y, data.Point2.x, data.Point2.y, LENGTH_X, LENGTH_Y);
                    foreach (KnapsackGrid item in grids)
                    {
                        if (item.GridObj != null && recycleGo == null)
                            recycleGo = item.GridObj;

                        item.GridObj = null;
                        item.IsOccupy = false;
                        item.Data.EquipmentPart = 0;
                        item.Data.UUID = 0;
                        item.Data.ItemData = null;
                    }
                }
                if (isRecycle && recycleGo != null)
                {
                    if (data.Grid_Type == E_Grid_Type.Shop)
                    {
                        UntrackNpcShopVisual(recycleGo);
                    }

                   // ResourcesComponent.Instance.RecycleGameObject(recycleGo);
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(recycleGo,recycleGo.name.StringToAB());
                    if (isReset) {
                        curDropObj = null;
                    }
                }
            }

        }
        /// <summary>
        /// 情况1：宝箱钥匙开宝箱,宝箱消失，钥匙还有则为false
        /// </summary>
        public bool TargetNull(KnapsackGridData targetData)
        {
            if (targetData.ItemData.IsCanOpen())
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 放入背包
        /// </summary>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        /// <param name="IsOverridePro">是否 覆盖属性</param>
        public void AddKnapsackItem(KnapsackGridData data, GameObject obj, bool IsOverridePro = false)
        {
            if (IsOverridePro && KnapsackItemsManager.KnapsackItems.ContainsKey(data.ItemData.UUID))
            {
                KnapsackItemsManager.KnapsackItems[data.ItemData.UUID].OverrideProperties(curChooseArea.ItemData);
            }

            data.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);

            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, data.Grid_Type);
          
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;


            if (data.IsSinglePoint)
            {
                grids[data.Point1.x][data.Point1.y].GridObj = obj;
                grids[data.Point1.x][data.Point1.y].IsOccupy = true;
                KnapsackGrid grid = grids[data.Point1.x][data.Point1.y];
                obj.transform.position = new Vector3(grid.Image.transform.position.x, grid.Image.transform.position.y, 85);
                grid.Data.SetSinglePoint(new Vector2Int(data.Point1.x, data.Point1.y));
                grid.Data.UUID = data.UUID;
                grid.Data.EquipmentPart = data.EquipmentPart != item_Info.Slot ? item_Info.Slot : data.EquipmentPart;
                grid.Data.ItemData = data.ItemData;
                //显示物品数量
                if (data.ItemData != null && data.ItemData.GetProperValue(E_ItemValue.Quantity) > 1)
                {
                    ChangeItemCount(obj, data.ItemData.GetProperValue(E_ItemValue.Quantity));
                }
                else
                {
                    for (int i = 0, length = obj.transform.childCount; i < length; i++)
                    {
                        if (obj.transform.GetChild(i).name == UnitEnityTopItemCanvas)
                        {
                            obj.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                }

                if (curKnapsackState == E_KnapsackState.KS_Knapsack && data.Grid_Type == E_Grid_Type.Knapsack && CheckItemlev(data))
                {
                    grids[data.Point1.x][data.Point1.y].Image.transform.Find("up").gameObject.SetActive(true);
                }
            }
            else
            {
                List<KnapsackGrid> gris = GetAreaGrids(data.Point1.x, data.Point1.y, data.Point2.x, data.Point2.y, LENGTH_X, LENGTH_Y);
                foreach (var item in gris)
                {
                    item.GridObj = obj;
                    item.Data.EquipmentPart = data.EquipmentPart != item_Info.Slot ? item_Info.Slot : data.EquipmentPart;
                    item.Data.Point1 = new Vector2Int(data.Point1.x, data.Point1.y);
                    item.Data.Point2 = new Vector2Int(data.Point2.x, data.Point2.y);
                    item.Data.UUID = data.UUID;
                    item.Data.ItemData = data.ItemData;
                    item.IsOccupy = true;
                }
                obj.transform.position = GetCenterPos(data.Point1.x, data.Point1.y, data.Point2.x, data.Point2.y);

                //显示物品数量
                if (data.ItemData != null && data.ItemData.GetProperValue(E_ItemValue.Quantity) > 1)
                {
                    ChangeItemCount(obj, data.ItemData.GetProperValue(E_ItemValue.Quantity));
                }
                else
                {
                    for (int i = 0, length = obj.transform.childCount; i < length; i++)
                    {
                        if (obj.transform.GetChild(i).name == UnitEnityTopItemCanvas)
                        {
                            obj.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                }

                if (curKnapsackState == E_KnapsackState.KS_Knapsack && data.Grid_Type == E_Grid_Type.Knapsack && CheckItemlev(data))
                {
                    grids[data.Point1.x][data.Point1.y].Image.transform.Find("up").gameObject.SetActive(true);
                }
            }


        }
        /// <summary>
        /// 检查当前放入背包装备 是否 比当前穿戴的装备好
        /// </summary>
        /// <param name="dataItem">true 好  false 不好</param>
        /// <returns></returns>

        public bool CheckItemlev(KnapsackGridData dataItem)
        {


            EquipmentComponent ??= roleEntity.GetComponent<RoleEquipmentComponent>();
            dataItem.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
          
            if (EquipmentComponent.curWareEquipsData_Dic.TryGetValue((E_Grid_Type)item_Info.Slot, out KnapsackDataItem item))
            {
                //当前装备的攻击力 高于 已经穿戴装备
                if (item.GetProperValue(E_ItemValue.BuyMoney) < dataItem.ItemData.GetProperValue(E_ItemValue.BuyMoney))
                {
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// 改变物品的数量
        /// </summary>
        /// <param name="dataItem"></param>
        public void ChangeItemCount(GameObject obj, int count)
        {
            GameObject topcount = null;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                if (obj.transform.GetChild(i).name == UnitEnityTopItemCanvas)
                {
                    topcount = obj.transform.GetChild(i).gameObject;
                    break;
                }
            }
            if (topcount == null)
            {
                topcount = ResourcesComponent.Instance.LoadGameObject(UnitEnityTopItemCanvas.StringToAB(), UnitEnityTopItemCanvas);
                topcount.transform.SetParent(obj.transform);
                topcount.transform.localPosition = new Vector3(0, 0, -0.5f);
                topcount.transform.localScale = Vector3.one / 40;

            }
           
            topcount.SetActive(true);
            topcount.GetComponentInChildren<Text>().text = count.ToString();
        }
        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="data">物品数据</param>
        /// <param name="isAddEquip">是否是 店家到装备栏</param>
        /// <param name="type">添加到 的类型</param>
        public void AddItem(KnapsackDataItem data, bool isAddEquip = false, E_Grid_Type type = E_Grid_Type.Knapsack)
        {

            data.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
            if (item_Info == null)
            {
                Log.DebugRed($"配置表不存在-》{data.ConfigId} 的物品");
                return;
            }
            if (string.IsNullOrEmpty(item_Info.ResName))
            {
                item_Info.ResName = "Weapon_borenjian";

            }

            GameObject go = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
            if (go == null)
            {
                go = ResourcesComponent.Instance.LoadGameObject("Weapon_borenjian".StringToAB(), "Weapon_borenjian");
            }
            
            go.SetUI(data.GetProperValue(E_ItemValue.Level));
            if (!isAddEquip && type == E_Grid_Type.Shop)
            {
                TrackNpcShopVisual(go);
            }

            KnapsackGridData kdata = new KnapsackGridData()
            {
                UUID = data.UUID,
                ItemData = data,
                Point1 = new Vector2Int(data.PosInBackpackX, data.PosInBackpackY),
                Point2 = new Vector2Int(data.PosInBackpackX + item_Info.X - 1, data.PosInBackpackY + item_Info.Y - 1),
                EquipmentPart = item_Info.Slot,//卡槽位置

            };
            if (isAddEquip)
            {
                //添加装备
                kdata.EquipmentPart = data.Slot;
                WareEquipItem(kdata.EquipmentPart, kdata, go);
            }
            else
            {
                kdata.Grid_Type = type;
                
                AddKnapsackItem(kdata, go);
            }
        }
        public void ChangeKnapsackItemCount(long itemUUid, int count)
        {
            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Knapsack);
            bool isBreak = false;
            for (int i = 0; i < LENGTH_Knapsack_Y; i++)
            {
                for (int j = 0; j < LENGTH_Knapsack_X; j++)
                {
                    KnapsackGrid grid = grids[j][i];
                    if (grid.Data.UUID == itemUUid && grid.IsOccupy)
                    {
                        // RemoveItem(grid.Data, true);
                        ChangeItemCount(grid.GridObj, count);
                        break;
                    }
                }
                if (isBreak)
                {
                    break;
                }

            }

        }
        /// <summary>
        /// 移除背包中的物品
        /// </summary>
        /// <param name="itemUUid"></param>
        public void RemoveKnapsack(long itemUUid)
        {
            if (curKnapsackState == E_KnapsackState.KS_Gem_Merge) return;

            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Knapsack);
            bool isBreak = false;
            for (int i = 0; i < LENGTH_Knapsack_Y; i++)
            {
                for (int j = 0; j < LENGTH_Knapsack_X; j++)
                {
                    KnapsackGrid grid = grids[j][i];
                    grid.Grid_Type = E_Grid_Type.Knapsack;
                    /* if (useItem != null && useItem.UUID == itemUUid)
                     {
                         Log.DebugGreen($"userItem:{useItem.ItemData.ConfigId}");
                         continue;
                     }*/
                  //  Log.DebugRed($"grid.Data.UUID == itemUUid：{grid.Data.UUID == itemUUid} grid.IsOccupy:{grid.IsOccupy}");
                    if (grid.Data.UUID == itemUUid && grid.IsOccupy)
                    {
                       // Log.DebugRed($"移除背包中的物品：{grid.Data.ItemData.ConfigId}");
                        KnapsackTools.RemoveEquip(grid.Data);
                        grid.Image.transform.Find("up").gameObject.SetActive(false);
                         RemoveItem(grid.Data, true);
                        isBreak = true;
                        break;
                    }
                    
                }
                if (isBreak)
                {
                    break;
                }

            }

        }
        /// <summary>
        /// 清理背包
        /// </summary>
        public void CleanKnapsack()
        {
          
                GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Knapsack);
                for (int i = 0; i < LENGTH_Knapsack_Y; i++)
                {
                    for (int j = 0; j < LENGTH_Knapsack_X; j++)
                    {

                        KnapsackGrid grid = grids[j][i];
                        if (grid.IsOccupy)
                        {
                            RemoveItem(grid.Data, true);
                        }
                    }

                }
           
        }

        #region 面板显示、隐藏 逻辑
        public void OnVisible(object[] data)
        {
            if (curKnapsackState == (E_KnapsackState)Enum.Parse(typeof(E_KnapsackState), data[0].ToString()))
            {
                Log.DebugRed($"请勿重复打开：{curKnapsackState}");
                return;
            }
            curKnapsackState = (E_KnapsackState)Enum.Parse(typeof(E_KnapsackState), data[0].ToString());
            if (data.Length > 1)
            {
                PanelManager(true, data[1].ToString().ToEnum<SynthesisData>());
                return;
            }
            PanelManager(true);
        }

        public void OnVisible()
        {
        }

        public void OnInVisibility()
        {

        }
        #endregion

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            try
            {
                CleanNpcShop();
            }
            catch (Exception)
            {
            }
            base.Dispose();
            curChooseArea = null;
            originArea = null;
            BackGrids = null;
            NpcShopGrids = null;
            WareHouseGrids = null;
            ClickSelectUnitEntityComponent.Instance.ClearSelectUnit();
            if (uIIntroduction != null)
            {
                UIComponent.Instance.Remove(UIType.UIIntroduction);
                uIIntroduction = null;
            }
            ClearFinishGrids();
            useItem = null;
            Game.EventCenter.RemoveEvent<long>(EventTypeId.RemoveKnapsack, RemoveKnapsack);
            Instance = null;
        }
    }
}
