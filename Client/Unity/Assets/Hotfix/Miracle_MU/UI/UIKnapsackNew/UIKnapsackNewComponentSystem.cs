using Codice.CM.Common;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

namespace ETHotfix
{

    [ObjectSystem]
    public class UIKnapsackNewComponentAwake : AwakeSystem<UIKnapsackNewComponent>
    {
        public override void Awake(UIKnapsackNewComponent self)
        {
            self.InitPanel();
        }
    }


    public static class UIKnapsackNewComponentSystem
    {
        public static void InitPanel(this UIKnapsackNewComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.plane = self.collector.GetGameObject("Panel");
            if (self.plane != null)
            {
                self.plane.SetActive(false);
            }

            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(() => { self.OnCloseClick(); });
            self.curChooseArea = new KnapsackGridData();
            self.originArea = new KnapsackGridData();
            self.RegisterEvent(-1, -1, self.collector.GetImage("DeleteArea").gameObject, E_Grid_Type.Delete);
            Game.EventCenter.EventListenner<long>(EventTypeId.RemoveKnapsack, self.RemoveKnapsack);

        }
        public static async ETTask Show(this UIKnapsackNewComponent self)
        {
            if (self.isBuildingView)
            {
                return;
            }

            self.isBuildingView = true;
            self.pendingVisualRefreshFrames = 0;
            self.SetPlaneVisible(false);
            string showStage = "start";
            //Log.Info("Show -- " + self.curKnapsackState);
            try
            {
                if(self.curKnapsackState == E_KnapsackState.KS_Stallup_OtherPlayer)
                {
                    //初始化背包
                    showStage = "InitPackage";
                    await self.InitPackage();

                    showStage = "InitStallupOther";
                    await self.InitStallupOther();
                }else if(self.curKnapsackState == E_KnapsackState.KS_Revision)
                {
                    showStage = "LoadPackage";
                    await self.LoadPackage();
                    //await self.InitQianHua();
                    showStage = "InitEquip";
                    await self.InitEquip();
                }else if(self.curKnapsackState == E_KnapsackState.KS_Inlay)
                {
                    showStage = "InitPackage";
                    await self.InitPackage();
                    showStage = "InitInlay";
                    await self.InitInlay();
                    self.HidePackageBtns();
                    self.HidePackageCoin();
                }
                else if(self.curKnapsackState == E_KnapsackState.KS_Gem_Merge)
                {
                    //await self.InitHeCheng();
                    self.collector.GetButton("CloseBtn").gameObject.SetActive(false);
                }else if(self.curKnapsackState == E_KnapsackState.KS_Ware_House)
                {
                    showStage = "InitPackage";
                    await self.InitPackage();
                    showStage = "InitWareHouse";
                    await self.InitWareHouse();
                    self.HidePackageBtns();

                }
                else
                {
                    //初始化背包
                    showStage = "InitPackage";
                    await self.InitPackage();
                    //初始化装备
                    showStage = "InitEquip";
                    await self.InitEquip();
                }

                self.SetPlaneVisible(true);
                self.pendingVisualRefreshFrames = 8;
            }
            catch (Exception e)
            {
                Log.Error($"#KnapsackShow# stage={showStage} ex={e}");
                throw;
            }
            finally
            {
                self.SetPlaneVisible(true);
                self.isBuildingView = false;
            }
        }

        public static void SetPlaneVisible(this UIKnapsackNewComponent self, bool isVisible)
        {
            if (self.plane != null)
            {
                self.plane.SetActive(isVisible);
            }

            self.SetDetachedGridVisualsVisible(isVisible);
        }

        public static void SetDetachedGridVisualsVisible(this UIKnapsackNewComponent self, bool isVisible)
        {
            HashSet<int> handled = new HashSet<int>();
            self.SetGridCollectionVisible(self.BackGrids, handled, isVisible);
            self.SetGridDictionaryVisible(self.EquipmentPartDic, handled, isVisible);
            self.SetGridCollectionVisible(self.NpcShopGrids, handled, isVisible);
            self.SetGridCollectionVisible(self.WareHouseGrids, handled, isVisible);
            self.SetGridCollectionVisible(self.StallUpGrids, handled, isVisible);
            self.SetGridCollectionVisible(self.StallUp_OtherGrids, handled, isVisible);

            if (self.curDropObj != null)
            {
                int instanceId = self.curDropObj.GetInstanceID();
                if (handled.Add(instanceId))
                {
                    self.curDropObj.SetActive(isVisible);
                }
            }
        }

        public static void SetGridCollectionVisible(this UIKnapsackNewComponent self, KnapsackNewGrid[][] sourceGrids, HashSet<int> handled, bool isVisible)
        {
            if (sourceGrids == null)
            {
                return;
            }

            foreach (KnapsackNewGrid[] column in sourceGrids)
            {
                if (column == null)
                {
                    continue;
                }

                foreach (KnapsackNewGrid grid in column)
                {
                    GameObject obj = grid?.GridObj;
                    if (obj == null)
                    {
                        continue;
                    }

                    int instanceId = obj.GetInstanceID();
                    if (!handled.Add(instanceId))
                    {
                        continue;
                    }

                    obj.SetActive(isVisible);
                }
            }
        }

        public static void SetGridDictionaryVisible(this UIKnapsackNewComponent self, Dictionary<E_Grid_Type, KnapsackNewGrid> sourceGrids, HashSet<int> handled, bool isVisible)
        {
            if (sourceGrids == null)
            {
                return;
            }

            foreach (KnapsackNewGrid grid in sourceGrids.Values)
            {
                GameObject obj = grid?.GridObj;
                if (obj == null)
                {
                    continue;
                }

                int instanceId = obj.GetInstanceID();
                if (!handled.Add(instanceId))
                {
                    continue;
                }

                obj.SetActive(isVisible);
            }
        }

        public static void ReleaseAllDetachedGridVisuals(this UIKnapsackNewComponent self)
        {
            HashSet<int> released = new HashSet<int>();
            self.ReleaseGridCollectionVisuals(self.BackGrids, released);
            self.ReleaseGridDictionaryVisuals(self.EquipmentPartDic, released);
            self.ReleaseGridCollectionVisuals(self.NpcShopGrids, released);
            self.ReleaseGridCollectionVisuals(self.WareHouseGrids, released);
            self.ReleaseGridCollectionVisuals(self.StallUpGrids, released);
            self.ReleaseGridCollectionVisuals(self.StallUp_OtherGrids, released);

            if (self.curDropObj != null)
            {
                int instanceId = self.curDropObj.GetInstanceID();
                if (released.Add(instanceId))
                {
                    GameObject.Destroy(self.curDropObj);
                }

                self.curDropObj = null;
                self.isDroping = false;
                self.originObjPos = Vector3.zero;
                self.originObjRotation = Quaternion.identity;
            }

        }

        public static void ReleaseGridCollectionVisuals(this UIKnapsackNewComponent self, KnapsackNewGrid[][] sourceGrids)
        {
            self.ReleaseGridCollectionVisuals(sourceGrids, new HashSet<int>());
        }

        public static void ReleaseGridCollectionVisuals(this UIKnapsackNewComponent self, KnapsackNewGrid[][] sourceGrids, HashSet<int> released)
        {
            if (sourceGrids == null)
            {
                return;
            }

            foreach (KnapsackNewGrid[] column in sourceGrids)
            {
                if (column == null)
                {
                    continue;
                }

                foreach (KnapsackNewGrid grid in column)
                {
                    GameObject obj = grid?.GridObj;
                    if (obj == null)
                    {
                        continue;
                    }

                    int instanceId = obj.GetInstanceID();
                    if (released.Add(instanceId))
                    {
                        GameObject.Destroy(obj);
                    }

                    grid.GridObj = null;
                }
            }
        }

        public static void ReleaseGridDictionaryVisuals(this UIKnapsackNewComponent self, Dictionary<E_Grid_Type, KnapsackNewGrid> sourceGrids)
        {
            self.ReleaseGridDictionaryVisuals(sourceGrids, new HashSet<int>());
        }

        public static void ReleaseGridDictionaryVisuals(this UIKnapsackNewComponent self, Dictionary<E_Grid_Type, KnapsackNewGrid> sourceGrids, HashSet<int> released)
        {
            if (sourceGrids == null)
            {
                return;
            }

            foreach (KnapsackNewGrid grid in sourceGrids.Values)
            {
                GameObject obj = grid?.GridObj;
                if (obj == null)
                {
                    continue;
                }

                int instanceId = obj.GetInstanceID();
                if (released.Add(instanceId))
                {
                    GameObject.Destroy(obj);
                }

                grid.GridObj = null;
            }
        }

        public static void HidePackageBtns(this UIKnapsackNewComponent self)
        {
            if (self.packageCollector)
            {
                self.packageCollector.GetButton("ClearUpBtn").gameObject.SetActive(false);
                self.packageCollector.GetButton("RepairBtn").gameObject.SetActive(false);
                self.packageCollector.GetButton("VipBtn").gameObject.SetActive(false);
                self.packageCollector.GetButton("BaiTanBtn").gameObject.SetActive(false);
                self.packageCollector.GetButton("SplitBtn").gameObject.SetActive(false);
            }
        }

        public static void HidePackageCoin(this UIKnapsackNewComponent self)
        {
            if (self.packageCollector)
            {
                self.packageCollector.GetImage("Coin").gameObject.SetActive(false);
                self.packageCollector.GetImage("qijibi").gameObject.SetActive(false);
                self.packageCollector.GetImage("yuanbao").gameObject.SetActive(false);
            }
        }

        public static void RemoveKnapsack(this UIKnapsackNewComponent self, long uuid)
        {
           if (self.curKnapsackState == E_KnapsackState.KS_Gem_Merge) return;

            self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, E_Grid_Type.Knapsack);
            for (int i = 0; i < UIKnapsackNewComponent.LENGTH_Knapsack_Y; i++)
            {
                for (int j = 0; j < UIKnapsackNewComponent.LENGTH_Knapsack_X; j++)
                {
                    KnapsackNewGrid grid = self.grids[j][i];
                    grid.Grid_Type = E_Grid_Type.Knapsack;
                  //  Log.DebugBrown("移除"+ grid.Data.UUID+":uid"+uuid);
                    if (grid.Data.UUID == uuid && grid.IsOccupy)
                    {
                        Log.DebugRed($"移除背包中的物品：{grid.Data.ItemData.ConfigId}");
                        KnapsackTools.RemoveEquip(grid.Data);
                        grid.Image.transform.Find("up")?.gameObject.SetActive(false);
                        self.RemoveItem(grid.Data, true);
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// 情况1：宝箱钥匙开宝箱,宝箱消失，钥匙还有则为false
        /// </summary>
        public static bool TargetNull(this UIKnapsackNewComponent self, KnapsackGridData targetData)
        {
            if (targetData.ItemData.IsCanOpen())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从背包中移除物品
        /// </summary>
        /// <param name="data"></param>
        public static void RemoveItem(this UIKnapsackNewComponent self, KnapsackGridData data, bool isRecycle = false, bool getGrid = false)
        {
            if (!isRecycle || getGrid)
            {
                self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, data.Grid_Type);

                if (data.Grid_Type == E_Grid_Type.Knapsack)
                {
                    KnapsackTools.RemoveEquip(data);
                    self.grids[data.Point1.x][data.Point1.y].Image.transform.Find("up")?.gameObject.SetActive(false);
                    //KnapsackItemsManager.KnapsackItems.Remove(data.UUID);
                }
            }

            //GameObject recycleGo = null;
            if (data.IsSinglePoint)
            {
                self.grids[data.Point1.x][data.Point1.y].Clear();
            }
            else
            {
                List<KnapsackNewGrid> grids = self.GetAreaGrids(data.Point1.x, data.Point1.y, data.Point2.x, data.Point2.y, self.LENGTH_X, self.LENGTH_Y);
                foreach (KnapsackNewGrid item in grids)
                {
                    item.Clear();
                }
            }
            //if (isRecycle && recycleGo != null)
            //{
            //    self.ClearDropObj();
            //}
        }

        /// <summary>
        ///  初始化格子数量
        /// </summary>
        /// <param name="x">列</param>
        /// <param name="y">行</param>
        /// <param name="content">格子父对象</param>
        /// <param name="grid_Type">格子类型</param>
        /// <param name="RegisterAction">格子事件函数</param>
        public static void CreatGrid(this UIKnapsackNewComponent self, int x, int y, Transform content, E_Grid_Type grid_Type, ref KnapsackNewGrid[][] Grids)
        {
            RectTransform temp = content.GetChild(0).GetComponent<RectTransform>();
            temp.gameObject.SetActive(false);
            temp.gameObject.name = "fab";
            Vector2 tmp = new Vector2();
            RectTransform rect = temp.GetComponent<RectTransform>();
            tmp.x = rect.sizeDelta.x;
            tmp.y = rect.sizeDelta.y;
            for (int i = 0; i < y; i++)//行数
            {
                for (int j = 0; j < x; j++)//列数
                {
                    RectTransform grid = GameObject.Instantiate<RectTransform>(temp, content);
                    grid.gameObject.SetActive(true);
                    grid.anchoredPosition = new Vector2(31 + (j * 62), -31 - (i * 66));
                    grid.name = $"{j}_{i}";//命名为：行_列
                    if (grid.Find("lock") != null)
                    {
                        grid.Find("lock").gameObject.SetActive(false);
                    }

                    Grids[j][i] = new KnapsackNewGrid
                    {
                        GridObj = null,
                        Image = grid.GetComponent<UnityEngine.UI.Image>(),
                        IsOccupy = false,
                        Grid_Type = grid_Type,
                        Num = grid.Find("num").GetComponent<UnityEngine.UI.Text>(),
                        Size = tmp
                    };


                    //注册 拖拽、点击、、事件
                    self.RegisterEvent(j, i, grid.gameObject, grid_Type);
                }
            }

        }

        /// <summary>
        /// 得到当前面板的格子数组
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="_Grid_Type">当前格子 所属类型</param>
        public static void GetKnapsackGrid(this UIKnapsackNewComponent self, ref KnapsackNewGrid[][] grid, ref int x, ref int y, E_Grid_Type _Grid_Type)
        {
            switch (_Grid_Type)
            {
                case E_Grid_Type.Knapsack:
                    grid = self.BackGrids;
                    x = UIKnapsackNewComponent.LENGTH_Knapsack_X;
                    y = UIKnapsackNewComponent.LENGTH_Knapsack_Y;
                    break;
                case E_Grid_Type.Gem_Merge:
                    //grid = self.MergerGrids;
                    //x = LENGTH_Merger_X;
                    //y = LENGTH_Merger_Y;
                    break;
                case E_Grid_Type.Ware_House:
                    grid = self.WareHouseGrids;
                    x = UIKnapsackNewComponent.LENGTH_WareHouse_X;
                    y = UIKnapsackNewComponent.LENGTH_WareHouse_Y;
                    break;
                case E_Grid_Type.Shop:
                    grid = self.NpcShopGrids;
                    x = UIKnapsackNewComponent.LENGTH_NpcShop_X;
                    y = UIKnapsackNewComponent.LENGTH_NpcShop_Y;
                    break;
                case E_Grid_Type.Stallup:
                    grid = self.StallUpGrids;
                    x = UIKnapsackNewComponent.LENGTH_StallUp_X;
                    y = UIKnapsackNewComponent.LENGTH_StallUp_Y;
                    break;
                case E_Grid_Type.Stallup_OtherPlayer:
                    grid = self.StallUp_OtherGrids;
                    x = UIKnapsackNewComponent.LENGTH_StallUp_Other_X;
                    y = UIKnapsackNewComponent.LENGTH_StallUp_Other_Y;
                    break;
                case E_Grid_Type.GiveCoin:
                    break;
                case E_Grid_Type.GiveGoods:
                    break;
                case E_Grid_Type.Consignment:
                    break;
                case E_Grid_Type.Reduction:
                    break;
                case E_Grid_Type.Trade:
                    //grid = self.MyGrids;
                    //x = LENGTH_Trade_X;
                    //y = LENGTH_Trade_Y;
                    break;
                case E_Grid_Type.Trade_Other:
                    //grid = OtherGrids;
                    //x = LENGTH_Trade_X;
                    //y = LENGTH_Trade_Y;
                    break;
                default:
                    break;
            }
        }
       
        /// <summary>
        /// 注册格子事件
        /// </summary>
        /// <param name="tr"></param>
        public static void RegisterEvent(this UIKnapsackNewComponent self, int x, int y, GameObject obj, E_Grid_Type type)
        {
            UGUITriggerProxy proxy = obj.GetComponent<UGUITriggerProxy>() ?? obj.AddComponent<UGUITriggerProxy>();

            proxy.OnBeginDragEvent += () => { self.OnBeginDrag(x, y, type); };
            proxy.OnEndDragEvent += () => { self.OnEndDrag(x, y, type); };
            proxy.OnPointerEnterEvent += () => { self.OnPointerEnter(x, y, type); };
            proxy.OnPointerClickEvent += () => { self.OnPointerClickEvent(x, y, type); };
            proxy.OnPointerExitEvent += () => { self.OnPointerExit(x, y, type); };
            proxy.OnPointerDownEvent += () => { self.OnPointerDownEvent(x, y, type); };
            proxy.OnPointerUpEvent += () => { self.OnPointerUpEvent(x, y, type); };
        }

        public static void OnBeginDrag(this UIKnapsackNewComponent self, int x, int y, E_Grid_Type type)
        {
            if (self.curKnapsackState == E_KnapsackState.KS_Revision) return;
            if (self.curChooseArea.Grid_Type == E_Grid_Type.Delete) return;

            self.ClearIntroduction();
            self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, type);
            KnapsackGrid grid = self.grids[x][y];//获取当前格子
            if (grid.IsOccupy == false)//判断当前格子是否有物品
                return;
            //useItem = null;
            //StopUseItem();//停止正在使用的物品

            self.isDroping = true;

            self.originArea = grid.Data;//记录当前选择的格子区域
            self.originArea.Grid_Type = grid.Grid_Type;
            self.originObjPos = grid.GridObj.transform.localPosition;
            self.originObjRotation = grid.GridObj.transform.localRotation;

            self.curDropObj = grid.GridObj;
        }

        /// <summary>
        /// 从背包拖拽到删除面板
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask OnPackageDragToDelete(this UIKnapsackNewComponent self)
        {
            Log.DebugBrown("这是背包丢弃逻辑");
            SceneName sceneName = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>();
            //if (self.originArea.ItemData.ItemType == E_ItemType.Gemstone||self.originArea.ItemData.ItemType == E_ItemType.FGemstone)
            //{
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, "宝石类，无法丢弃");
            //    self.ResetGridObj();
            //}
            if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法丢弃");
                self.ResetGridObj();
            }
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法丢弃");
                self.ResetGridObj();
            }
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法丢弃");
                self.ResetGridObj();
            }
            //else if (self.originArea.ItemData.ItemType == (int)E_ItemType.Gemstone|| self.originArea.ItemData.ItemType == (int)E_ItemType.FGemstone)
            //{
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, "宝石类，无法丢弃");
            //        self.ResetGridObj();
            //}
            else if (self.originArea.ItemData.ConfigId == 320106 && self.roleEntity.IsSafetyZone)
            {
                List<string> list = new List<string>();
                self.originArea.ItemData.GetItemName(ref list);
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"安全区不能使用{list.ListToString().Split(',')[1]}");
                self.ResetGridObj();
            }
            else if ((self.originArea.ItemData.ConfigId == 320106 && !self.roleEntity.IsSafetyZone) && (sceneName == SceneName.XueSeChengBao || sceneName == SceneName.EMoGuangChang || sceneName == SceneName.GuZhanChang || sceneName == SceneName.kalima_map))
            {
                List<string> list = new List<string>();
                self.originArea.ItemData.GetItemName(ref list);
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"副本里不能使用{list.ListToString().Split(',')[1]}");
                self.ResetGridObj();
            }
            else if(SpecialGood(self.originArea.ItemData))
            {
                Log.DebugBrown("达成条件");
                await self.SendDiscardKnasackItemMessage();

            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品无法丢弃");
                self.ResetGridObj();
                //丢弃物品
                //   await self.SendDiscardKnasackItemMessage();
            }
            await ETTask.CompletedTask;
        }


        public static bool SpecialGood(this KnapsackDataItem item)
        {
            //不能丢弃的类型
            if (item.ItemType == (int)E_ItemType.Pet || item.ItemType == (int)E_ItemType.Mounts || item.ItemType == (int)E_ItemType.Guard || item.ItemType == (int)E_ItemType.Gemstone || item.ItemType == (int)E_ItemType.FGemstone || item.ItemType == (int)E_ItemType.Wing)
            {
                return false;
            }
            //不能丢弃的玛雅装备
            if (item.ConfigId == 30005 || item.ConfigId == 80007 || item.ConfigId == 40006)//玛雅装备不能回收
            {
                return false;
            }
            //丢弃合成的装备
            if (item.ConfigId == 320097 || item.ConfigId == 320098 || item.ConfigId == 320005 || item.ConfigId == 320004)//回收指定装备
            {
                return true;
            }
            //可以丢弃的技能书，回复品
            if (item.ItemType == (int)E_ItemType.Consumables || item.ItemType == (int)E_ItemType.SkillBooks) return true;

            return item.ExecllentEntryDic.Count == 0 && item.GetProperValue(E_ItemValue.SetId) == 0
        && item.GetProperValue(E_ItemValue.FluoreSlotCount) == 0 && item.GetProperValue(E_ItemValue.OptLevel) == 0 && item.IsHaveReginAtr == false
        && item.GetProperValue(E_ItemValue.Level) < 7 && item.ItemType != (int)E_ItemType.Other; ;
        }



        /// <summary>
        /// 请求服务器丢弃物品
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static async ETTask SendDiscardKnasackItemMessage(this UIKnapsackNewComponent self)
        {

            var node = self.GetNearNode();
            G2C_DelBackpackItemResponse g2C_Del = (G2C_DelBackpackItemResponse)await SessionComponent.Instance.Session.Call(new C2G_DelBackpackItemRequest
            {
                ItemUUID = self.originArea.UUID,
                PosInSceneX = node.x,
                PosInSceneY = node.z,
            });
            if (g2C_Del.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Del.Error.GetTipInfo());
                self.ResetGridObj();
            }
            else
            {
                self.ClearDropObj();

                GuideComponent.Instance.CheckIsShowGuide(true);
            }
        }

        /// <summary>
        /// 获取玩家周围可使用点（十格以内）
        /// </summary>
        /// <returns></returns>
        public static AstarNode GetNearNode(this UIKnapsackNewComponent self)
        {

            AstarNode astarNode = null;
            for (int i = -5; i < 0; i++)
            {
                for (int j = -5; j < 5; j++)
                {

                    if (Mathf.Abs(i) <= 2 && Mathf.Abs(j) <= 2) continue;
                    var nearNode = self.roleEntity.CurrentNodePos;
                    //AstarNode node = AstarComponent.Instance.GetNodeVector(vector.x, vector.z);
                    AstarNode node = AstarComponent.Instance.GetNode(nearNode.x + i, nearNode.z + j);
                    if (node.isWalkable)
                    {
                        //判断该点是否有实体

                        if (IsNull(node) == false)
                        {
                            continue;
                        }

                        return node;
                    }
                }
            }
            return astarNode;

            bool IsNull(AstarNode node)
            {
                List<KnapsackItemEntity> allentity = UnitEntityComponent.Instance.KnapsackItemEntityDic.Values.ToList();

                for (int k = 0; k < allentity.Count; k++)
                {
                    var item = allentity[k];
                    if (node.Compare(item.CurrentNodePos))
                    {
                        astarNode ??= node;
                        //当前格子有装备
                        return false;
                    }
                }
                return true;
            }
        }
       
        /// <summary>
        /// 背包 内部 拖拽
        /// </summary>
        /// <param name="self"></param>
        public static async ETTask OnPackageToPackage(this UIKnapsackNewComponent self)
        {
            if (self.originArea.IsSinglePoint)
            {
                if (self.grids[self.curChooseArea.Point1.x][self.curChooseArea.Point2.y].IsOccupy == false)
                {
                    //目标区域没有被占用
                    //请求服务端 改变物品的位置信息
                    await self.SendMoveKnapsackItemMessage();
                }
                else
                {
                    //目标区域 被占用 则判断物品是否可以合并
                    long targetUid = self.BackGrids[self.curChooseArea.Point1.x][self.curChooseArea.Point1.y].Data.UUID;//目标物品的uuid
                                                                                                                        //判断是否可以 合并
                    KnapsackDataItem targetKnapsackDataItem = self.BackGrids[self.curChooseArea.Point1.x][self.curChooseArea.Point1.y].Data.ItemData;

                    Log.DebugBrown("目标" + targetKnapsackDataItem.GetProperValue(E_ItemValue.IsBind));
                    KnapsackItemsManager.KnapsackItems.TryGetValue(targetUid, out KnapsackDataItem targetItem); //目标物品的 实体类
                        //Log.Info("OnPackageToPackage111  " + targetUid);
                    KnapsackItemsManager.KnapsackItems.TryGetValue(self.originArea.UUID, out KnapsackDataItem usedataItem); //当前正在移动物品的 实体类
                        //Log.Info("OnPackageToPackage222   " + self.originArea.UUID);
                    //配置表ID相等 堆叠数大于1 强化等级相等
                    targetItem.ConfigId.GetItemInfo_Out(out targetItem.item_Info);
                       // Log.Info("OnPackageToPackage333");
                    usedataItem.ConfigId.GetItemInfo_Out(out usedataItem.item_Info);
                    //Log.Info("OnPackageToPackage444");

                    if (usedataItem.ConfigId == targetItem.ConfigId && usedataItem.Id != targetItem.Id && usedataItem.item_Info.StackSize > 1 && targetItem.item_Info.StackSize > 1 && usedataItem.GetProperValue(E_ItemValue.Level) == targetItem.GetProperValue(E_ItemValue.Level))
                    {
                        Log.Info("叠加物品1");
                        //请求服务器 叠加物品
                        self.MergerItem(targetUid, usedataItem).Coroutine();
                    }
                    else if (usedataItem.IsCanDragUser())
                    {
                        Log.Info("使用物品");
                        //请求服务器 使用物品
                        self.MergerItem(targetUid, usedataItem).Coroutine();
                    }
                    else if (usedataItem.IsTheTresureKey() && targetKnapsackDataItem.IsCanOpen())
                    {
                        Log.Info("宝箱钥匙打开宝箱");
                        //宝箱钥匙打开宝箱
                        self.KeyOpenTreasure(targetKnapsackDataItem, usedataItem).Coroutine();
                    }
                    else
                    {
                        Log.Info("叠加物品2");
                        self.ResetGridObj();
                    }
                }
            }
            else
            {

                self.SendMoveKnapsackItemMessage().Coroutine();
                Log.DebugBrown("多对多");
                long targetUid = self.BackGrids[self.curChooseArea.Point1.x][self.curChooseArea.Point1.y].Data.UUID;//目标物品的uuid
                                                                                                                    //判断是否可以 合并
                KnapsackDataItem targetKnapsackDataItem = self.BackGrids[self.curChooseArea.Point1.x][self.curChooseArea.Point1.y].Data.ItemData;

                Log.DebugBrown("目标" + targetKnapsackDataItem.GetProperValue(E_ItemValue.IsBind));
                KnapsackItemsManager.KnapsackItems.TryGetValue(targetUid, out KnapsackDataItem targetItem); //目标物品的 实体类
                                                                                                            //Log.Info("OnPackageToPackage111  " + targetUid);
                KnapsackItemsManager.KnapsackItems.TryGetValue(self.originArea.UUID, out KnapsackDataItem usedataItem); //当前正在移动物品的 实体类
                                                                                                                        //Log.Info("OnPackageToPackage222   " + self.originArea.UUID);
                                                                                                                        //配置表ID相等 堆叠数大于1 强化等级相等
                targetItem.ConfigId.GetItemInfo_Out(out targetItem.item_Info);
                // Log.Info("OnPackageToPackage333");
                usedataItem.ConfigId.GetItemInfo_Out(out usedataItem.item_Info);
                //Log.Info("OnPackageToPackage444");

                if (usedataItem.ConfigId == targetItem.ConfigId && usedataItem.Id != targetItem.Id && usedataItem.item_Info.StackSize > 1 && targetItem.item_Info.StackSize > 1 && usedataItem.GetProperValue(E_ItemValue.Level) == targetItem.GetProperValue(E_ItemValue.Level))
                {
                    Log.Info("叠加物品3");
                    //请求服务器 叠加物品
                    self.MergerItem(targetUid, usedataItem).Coroutine();
                }
                else if (usedataItem.IsCanDragUser())
                {
                    Log.Info("使用物品");
                    //请求服务器 使用物品
                    self.MergerItem(targetUid, usedataItem).Coroutine();
                }
                else if (usedataItem.IsTheTresureKey() && targetKnapsackDataItem.IsCanOpen())
                {
                    Log.Info("宝箱钥匙打开宝箱");
                    //宝箱钥匙打开宝箱
                    self.KeyOpenTreasure(targetKnapsackDataItem, usedataItem).Coroutine();
                }
                else
                {
                    self.SendMoveKnapsackItemMessage().Coroutine();
                    Log.Info("叠加物品4");
                   // self.ResetGridObj();
                }
                //---
                //物品占用的格子数 不是nxn 而是（2x3 ....等）
                //bool isOrigin = targetUUID == originArea.UUID;//目标位置 是否与起始位置重合
                if (self.ContainGridObj(self.curChooseArea.Point1.x, self.curChooseArea.Point1.y, self.curChooseArea.Point2.x, self.curChooseArea.Point2.y))//格子为被占用||isOrigin
                {
                    //格子被占用 回到起始位置
                    //UIComponent.Instance.VisibleUI(UIType.UIHint, "1目标格子已被占用");
                    //self.ResetGridObj();
                }
                else
                {

                    //请求服务端 改变物品的位置信息
                    // self.SendMoveKnapsackItemMessage().Coroutine();
                    //await self.SendMoveKnapsackItemMessage();
                    //if (self.grids[self.curChooseArea.Point1.x][self.curChooseArea.Point2.y].IsOccupy == false)
                    //{
                    //    //目标区域没有被占用
                    //    //请求服务端 改变物品的位置信息
                    //   // await self.SendMoveKnapsackItemMessage();
                    //}
                    //else
                    //{
                    //    //目标区域 被占用 则判断物品是否可以合并
                    //    long targetUid = self.BackGrids[self.curChooseArea.Point1.x][self.curChooseArea.Point1.y].Data.UUID;//目标物品的uuid
                    //                                                                                                        //判断是否可以 合并
                    //    KnapsackDataItem targetKnapsackDataItem = self.BackGrids[self.curChooseArea.Point1.x][self.curChooseArea.Point1.y].Data.ItemData;

                    //    Log.DebugBrown("目标" + targetKnapsackDataItem.GetProperValue(E_ItemValue.IsBind));
                    //    KnapsackItemsManager.KnapsackItems.TryGetValue(targetUid, out KnapsackDataItem targetItem); //目标物品的 实体类
                    //                                                                                                //Log.Info("OnPackageToPackage111  " + targetUid);
                    //    KnapsackItemsManager.KnapsackItems.TryGetValue(self.originArea.UUID, out KnapsackDataItem usedataItem); //当前正在移动物品的 实体类
                    //                                                                                                            //Log.Info("OnPackageToPackage222   " + self.originArea.UUID);
                    //                                                                                                            //配置表ID相等 堆叠数大于1 强化等级相等
                    //    targetItem.ConfigId.GetItemInfo_Out(out targetItem.item_Info);
                    //    // Log.Info("OnPackageToPackage333");
                    //    usedataItem.ConfigId.GetItemInfo_Out(out usedataItem.item_Info);
                    //    //Log.Info("OnPackageToPackage444");

                    //    if (usedataItem.ConfigId == targetItem.ConfigId && usedataItem.Id != targetItem.Id && usedataItem.item_Info.StackSize > 1 && targetItem.item_Info.StackSize > 1 && usedataItem.GetProperValue(E_ItemValue.Level) == targetItem.GetProperValue(E_ItemValue.Level))
                    //    {
                    //        Log.Info("叠加物品");
                    //        //请求服务器 叠加物品
                    //        self.MergerItem(targetUid, usedataItem).Coroutine();
                    //    }
                    //    else if (usedataItem.IsCanDragUser())
                    //    {
                    //        Log.Info("使用物品");
                    //        //请求服务器 使用物品
                    //        self.MergerItem(targetUid, usedataItem).Coroutine();
                    //    }
                    //    else if (usedataItem.IsTheTresureKey() && targetKnapsackDataItem.IsCanOpen())
                    //    {
                    //        Log.Info("宝箱钥匙打开宝箱");
                    //        //宝箱钥匙打开宝箱
                    //        self.KeyOpenTreasure(targetKnapsackDataItem, usedataItem).Coroutine();
                    //    }
                    //    else if (usedataItem.IsRelieve())
                    //    {
                    //        if (targetKnapsackDataItem.GetProperValue(E_ItemValue.IsBind) == 1 || targetKnapsackDataItem.GetProperValue(E_ItemValue.IsBind) == 2)
                    //        {

                    //            Log.Info("请求服务器 使用物品");
                    //            //请求服务器 使用物品
                    //            self.MergerItem(targetUid, usedataItem).Coroutine();
                    //            Log.DebugBrown("解绑");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Log.Info("叠加物品");
                    //        self.ResetGridObj();
                    //    }
                    //    }
                    //目标区域 被占用 则判断物品是否可以合并
                 
                }
            }
        }

        /*
        合并、升级背包中单个物品
        如两个 药水 数量未到上限，可以进行合并
        武器、防具等升级+1~+9
        武器、防具追加属性
        武器、防具等再生属性添加、进化
        */
        public static async ETTask MergerItem(this UIKnapsackNewComponent self, long targetUid, KnapsackDataItem usedataItem)
        {
            G2C_MergeSingleItems g2C_Merge = (G2C_MergeSingleItems)await SessionComponent.Instance.Session.Call(new C2G_MergeSingleItems
            {
                ItemUUID = self.originArea.UUID,
                TargetItemUUID = targetUid
            });
            Log.DebugBrown("使用物品的错误码" + g2C_Merge.Error + "::::self.originArea.UUID" + self.originArea.UUID + "::targetUid" + targetUid);
            if (g2C_Merge.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Merge.Error.GetTipInfo());
                self.ResetGridObj();
            }
            else
            {
                if (KnapsackItemsManager.KnapsackItems.ContainsKey(usedataItem.Id) && usedataItem.GetProperValue(E_ItemValue.Quantity) > 0)
                    self.ResetGridObj();
            }
        }

        //宝箱钥匙打开宝箱协议
        public static async ETTask KeyOpenTreasure(this UIKnapsackNewComponent self, KnapsackDataItem targetKnapsackDataItem, KnapsackDataItem usedataItem)
        {
            G2C_OpenTheSpecialTreasureChestResponse g2C_Open = (G2C_OpenTheSpecialTreasureChestResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenTheSpecialTreasureChestRequest
            {
                ChestkeyId = self.originArea.UUID,
                TreasureChestId = targetKnapsackDataItem.UUID
            });
            if (g2C_Open.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Open.Error.GetTipInfo());
                self.ResetGridObj();
            }
            else
            {
                if (KnapsackItemsManager.KnapsackItems.ContainsKey(usedataItem.Id) && usedataItem.GetProperValue(E_ItemValue.Quantity) > 0)
                    self.ResetGridObj();
            }
        }

        /// <summary>
        /// 通知服务端 位置变动
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <returns></returns>
        public static async ETTask SendMoveKnapsackItemMessage(this UIKnapsackNewComponent self)
        {

            G2C_MoveBackpackItemResponse g2C_Move = (G2C_MoveBackpackItemResponse)await SessionComponent.Instance.Session.Call(new C2G_MoveBackpackItemRequest
            {
                ItemUUID = self.originArea.UUID,
                PosInBackpackX = self.curChooseArea.Point1.x,
                PosInBackpackY = self.curChooseArea.Point1.y,
            });
            Log.DebugBrown("SendMoveKnapsackItemMessage" + g2C_Move.Error);
            if (g2C_Move.Error != 0)
            {
               // UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Move.Error.GetTipInfo());
                self.ResetGridObj();
                
            }
            else
            {
                self.curChooseArea.UUID = self.originArea.UUID;
                self.curChooseArea.ItemData = self.originArea.ItemData;


                //改变物品的位置
                self.RemoveItem(self.originArea);

                self.AddKnapsackItem(self.curChooseArea);

                self.ClearOriginArea();
            }
        }

        public static void ClearOriginArea(this UIKnapsackNewComponent self)
        {
            self.originArea = new KnapsackGridData();
            self.curChooseArea = new KnapsackGridData();
        }

        public static async ETTask OnPackageDrawToEquip(this UIKnapsackNewComponent self)
        {
            // Log.Info("背包拖拽到装备栏");
            KnapsackGrid grid = self.EquipmentPartDic[self.curChooseArea.Grid_Type];

            if (!self.IsCanWearEquip(ref self.curWarePart))
            {
                self.ResetGridObj();
                return;
            }

            self.curChooseArea.UUID = self.originArea.UUID;
            self.curChooseArea.ItemData = self.originArea.ItemData;
            self.curWarePart = (int)self.curChooseArea.Grid_Type;

            if (grid.IsOccupy)
            {
                await self.RequestReplaceEquipFromBackpack(self.curWarePart);
            }
            else
            {
                await self.RequestWareEquip();
            }
        }

        /// <summary>
        /// 穿戴装备
        /// </summary>
        /// <param name="equipId">装备的UUID</param>
        /// <param name="part">穿戴部位 </param>
        /// <returns></returns>
        public static async ETVoid RequestWareEquip(this UIKnapsackNewComponent self)
        {

            G2C_EquipItemResponse g2C_Equip = (G2C_EquipItemResponse)await SessionComponent.Instance.Session.Call(new C2G_EquipItemRequest
            {
                ItemUUID = self.originArea.ItemData.UUID,
                EquipPosition = self.curWarePart

            });
            if (g2C_Equip.Error != 0)
            {
                Log.DebugRed($"g2C_Equip.Error:{g2C_Equip.Error}");
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_Equip.Error.GetTipInfo()}");

                self.ResetGridObj();
                if(((self.curChooseArea.Grid_Type >= E_Grid_Type.Weapon && self.curChooseArea.Grid_Type <= E_Grid_Type.TianYing) || self.curChooseArea.Grid_Type == E_Grid_Type.Mounts))
                {
                    KnapsackGrid grid = self.EquipmentPartDic[self.curChooseArea.Grid_Type];
                    grid.ResetColor();
                }
                

                //self.curChooseArea
            }
            else
            {
                GuideComponent.Instance.CheckIsShowGuide(true);
                self.ClearDropObj();
            }
        }

        public static async ETVoid RequestReplaceEquipFromBackpack(this UIKnapsackNewComponent self, int slot)
        {
            G2C_ReplaceEquipItemResponse response = (G2C_ReplaceEquipItemResponse)await SessionComponent.Instance.Session.Call(new C2G_ReplaceEquipItemRequest
            {
                ItemUUID = self.originArea.UUID,
                EquipPosition = slot
            });

            if (response.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, response.Error.GetTipInfo());
                self.ResetGridObj();
                if ((self.curChooseArea.Grid_Type >= E_Grid_Type.Weapon && self.curChooseArea.Grid_Type <= E_Grid_Type.TianYing) || self.curChooseArea.Grid_Type == E_Grid_Type.Mounts)
                {
                    self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                }
            }
            else
            {
                GuideComponent.Instance.CheckIsShowGuide(true);
                self.ClearDropObj();
            }
        }
        
        /// <summary>
        /// 是否可以穿带该装备
        /// </summary>
        /// <returns></returns>
        public static bool IsCanWearEquip(this UIKnapsackNewComponent self, ref int curPart)
        {
            //  Log.DebugGreen($"{originArea.UUID} 是否可以使用KnapsackItemsManager.KnapsackItems:{KnapsackItemsManager.KnapsackItems.Count}");

            if (!KnapsackItemsManager.KnapsackItems.TryGetValue(self.originArea.UUID, out KnapsackDataItem item))
            {
                Log.DebugRed("背包中不存在 对应的装备");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                return false;
            }
            item.ConfigId.GetItemInfo_Out(out Item_infoConfig config); ;//读取装备配置信息
                                                                        //  Log.DebugRed($"config == null:{config == null} item.ConfigId:{item.ConfigId}");
            if (config == null) return false;

            ///该玩家是否可以穿戴该装备
            if (!config.IsCanUer((int)self.roleEntity.RoleType, self.roleEntity.ClassLev))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "玩家不可装备");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                return false;
            }

            ///检查 等级、力量、敏捷、智力、体力、统率 是否满足
            if (self.originArea.ItemData.CheckRequirel() == false)
            {
                return false;
            }

            curPart = self.curWarePart;

            if (self.curChooseArea.Grid_Type == E_Grid_Type.Weapon)//目标卡槽是 武器
            {
                if (self.EquipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Shield))//盾牌卡槽 已装备
                {
                    if (self.roleEntity.RoleType != E_RoleType.Archer)
                    {

                        KnapsackItemsManager.KnapsackItems[self.originArea.UUID].ConfigId.GetItemInfo_Out(out Item_infoConfig weaponConfig); //当前武器 的配置表信息
                        if (weaponConfig != null && weaponConfig.TwoHand == 1)//该武器是双手武器 
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "装备盾时 不能装备双手武器");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                            return false;
                        }
                    }
                    else
                    {
                        //弓箭手

                        if (self.EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ConfigId == 60008|| self.EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ConfigId == 60000) return true;//黄金箭筒

                        //盾牌卡槽 装备了弓箭 武器卡槽只能装备弓 
                        if (self.EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ItemType == (int)E_ItemType.Arrow || self.EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ConfigId == 40019)
                        {
                            if (item.ItemType != (int)E_ItemType.Bows)
                            {
                                //当前装备 不是弓
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "只能装备 弓");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                                return false;
                            }
                        }
                        else
                        {
                            if (item.ItemType == (int)E_ItemType.Bows)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "不能装备 弓");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                                return false;
                            }
                            else if (item.ItemType == (int)E_ItemType.Crossbows)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "不能装备 驽");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                                return false;
                            }
                        }
                        //盾牌卡槽 装备了弩箭 武器卡槽只能装备弩
                        if (self.EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ItemType == (int)E_ItemType.Arrow || self.EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Shield].ConfigId == 50012)
                        {
                            if (item.ItemType != (int)E_ItemType.Crossbows)
                            {
                                //当前装备 不是驽
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "只能装备 驽");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                                return false;
                            }
                        }

                    }

                }
            }
            if (self.curChooseArea.Grid_Type == E_Grid_Type.Shield)//目标卡槽是 盾牌 
            {
                //   Log.DebugGreen($"目标卡槽为：{curChooseArea.Grid_Type.ToString()}  是否已经装备武器：{EquipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Weapon)}");
                if (self.EquipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Weapon))//武器卡槽 已装备
                {


                    self.EquipmentComponent.curWareEquipsData_Dic[E_Grid_Type.Weapon].ConfigId.GetItemInfo_Out(out Item_infoConfig weaponConfig); //当前已经装备的武器 的配置表信息
                    KnapsackItemsManager.KnapsackItems[self.originArea.UUID].ConfigId.GetItemInfo_Out(out Item_infoConfig curweaponConfig); //当前武器 的配置表信息
                    if (self.roleEntity.RoleType == E_RoleType.Archer)  //弓箭手
                    {

                        if (config.Id == 60008|| config.Id == 60000) return true;//黄金箭筒

                        //武器卡槽 装备了弓 盾牌卡槽只能装备弓箭或箭筒
                        if (weaponConfig.Type == (int)E_ItemType.Bows)
                        {
                            //  Log.DebugGreen($"item.ItemType:{item.ItemType}");
                            if (item.ItemType != (int)E_ItemType.Arrow && !config.Name.Contains("弓箭"))
                            {
                                //当前装备 不是弓箭
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "只能装备 弓箭或箭筒");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
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
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                                return false;
                            }
                        }
                        //武器卡槽装备了 武器  盾牌卡槽不能装备箭筒、弓箭、弩箭
                        else
                        {
                            if (item.ItemType == (int)E_ItemType.Arrow || config.Name.Contains("弓箭") || config.Name.Contains("弩箭"))
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "装备了武器 不能使用箭筒、弓箭、弩箭");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (weaponConfig != null && weaponConfig.TwoHand == 1)//主手武器是双手武器 
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "已装备双手武器 不可装备副武器");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                            return false;
                        }
                        else if (curweaponConfig != null && curweaponConfig.TwoHand == 1)//双手武器 不能装备在副武器卡槽
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "双手武器 不能装备在该位置");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                            return false;
                        }
                        else
                        {
                            //主武器是单手武器 副武器是单手武器

                            if (self.roleEntity.RoleType == E_RoleType.Summoner && item.ItemType == (int)E_ItemType.MagicBook)
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
                            if (self.roleEntity.RoleType != E_RoleType.Swordsman && self.roleEntity.RoleType != E_RoleType.Magicswordsman && self.roleEntity.RoleType != E_RoleType.Gladiator) // 当前角色是否是 剑士 魔剑士 格斗家
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{self.roleEntity.RoleType.GetRoleName(self.roleEntity.ClassLev)}  不可同时装备两把单手武器");
                                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();

                                return false;
                            }

                            return true;
                        }

                    }
                }
            }
            //判断 当前移动的装备 是否属于当前选择装备卡槽
            bool isRing = self.originArea.EquipmentPart == 11 && (int)self.curChooseArea.Grid_Type == 12;
            bool isDoubleHandWeapon = self.originArea.EquipmentPart == 1 && (int)self.curChooseArea.Grid_Type == 2;//当前物品的装备类型为武器 目标卡槽为盾牌


            if ((int)self.curChooseArea.Grid_Type != (int)self.originArea.EquipmentPart && !isRing && !isDoubleHandWeapon)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "装备类型不匹配");
                self.EquipmentPartDic[self.curChooseArea.Grid_Type].ResetColor();
                return false;
            }
            return true;
        }

        public static void OnPackageDragToHeCheng(this UIKnapsackNewComponent self)
        {
            if (KnapsackItemsManager.KnapsackItems.TryGetValue(self.originArea.UUID, out KnapsackDataItem item))
            {
                if (item.GetProperValue(E_ItemValue.IsUsing) == 1)//检查物品是否处于使用中
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法合成");
                    self.ResetGridObj();
                    return;
                }
                if (item.GetProperValue(E_ItemValue.IsLocking) == 1)//检查物品是否处于使用中
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定 无法合成");
                    self.ResetGridObj();
                    return;
                }
                if (item.GetProperValue(E_ItemValue.IsTask) == 1)//检查物品是否处于使用中
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法合成");
                    self.ResetGridObj();
                    return;
                }
                //刷新检测方法
                self.RefreshMergerMethods(item);

                if (self.curMergerMethod != null)
                {
                    self.ChangeItemPos(async () =>
                    {
                        //将物品移动到合成缓存区域
                        G2C_MoveBackpackItemToCacheSpace g2C_MoveBackpackItemToCache = (G2C_MoveBackpackItemToCacheSpace)await SessionComponent.Instance.Session.Call(new C2G_MoveBackpackItemToCacheSpace { MovedItemUUID = self.originArea.UUID });
                        if (g2C_MoveBackpackItemToCache.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MoveBackpackItemToCache.Error.GetTipInfo());

                        }
                        else
                        {

                            self.curChooseArea.UUID = self.originArea.UUID;
                            self.curChooseArea.ItemData = self.originArea.ItemData;

                            self.AddKnapsackItem(self.curChooseArea, self.curDropObj);

                            self.AddMergerItem(item);
                            self.RemoveItem(self.originArea, getGrid: true);
                            self.UpdateTips();
                        }
                    });
                }
                else
                {
                    self.RefreshMergerMethods(null);
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "只能放入可以合成的物品2");
                    self.ResetGridObj();
                }
            }
            else
            {

            }
        }
       
        /// <summary>
        /// 更新面板上的显示信息
        /// </summary>
        public static void UpdateTips(this UIKnapsackNewComponent self)
        {
            //Log.DebugBrown($"curMergerMethod == null:{curMergerMethod == null}");
            if (self.curMergerMethod == null)
            {
                self.ResetTips();
                return;
            }
            //显示合成标题
            self.TopTitle.text = self.curMergerMethod.Title;
            //成功率
            self.TopTipSucceed.text = $"合成时成功率：{self.curMergerMethod.SuccessRate}%";
            //所需金币
            string color = self.curMergerMethod.Money > self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin) ? ColorTools.GetColorHtmlString(Color.red) : ColorTools.GetColorHtmlString(Color.white);
            self.TopTipMoney.text = $"必要的金币：<color={color}>{self.curMergerMethod.Money:N}</color>({self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin):N})";
            //更新显示 所需材料
            self.Tips.text = string.Empty;
            for (int i = 0, length = self.curMergerMethod.textBom.Count; i < length; i++)
            {
                self.Tips.text += self.curMergerMethod.textBom[i] + "\n";
            }
            //强化失败装备是否销毁
            self.FailedDeleTip.gameObject.SetActive(self.curMergerMethod.FailedDelete);
            //是否可以合成
            self.MergeBtn.interactable = self.curMergerMethod.IsCanMerger;
        }
       
        //重置面板显示的文字
        public static void ResetTips(this UIKnapsackNewComponent self)
        {
            self.TopTitle.text = string.Empty;
            self.TopTipSucceed.text = $"合成时成功率：{0}%";
            self.TopTipMoney.text = $"必要的金币：0({self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin):N})";
            self.Tips.text = $"<color=red>请把合成材料放上去</color>";
            //是否可以合成
            self.MergeBtn.interactable = false;
        }

        public static void AddMergerItem(this UIKnapsackNewComponent self, KnapsackDataItem item)
        {
            if (!self.alreadyAddMergerItems.Exists(r => r.UUID == item.UUID))
            {
                self.alreadyAddMergerItems.Add(item);

            }

        }
       
        /// <summary>
        /// 改变物品的位置
        /// </summary>
        /// <param name="action">请求 服务端的回调函数</param>
        public static void ChangeItemPos(this UIKnapsackNewComponent self, Action action)
        {
            if (self.originArea.IsSinglePoint)
            {
                if (self.grids[self.curChooseArea.Point1.x][self.curChooseArea.Point2.y].IsOccupy == false)//目标区域没有被占用
                {
                    //请求服务端 
                    action?.Invoke();
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "此处无法放置物品");
                    self.ResetGridObj();
                }
            }
            else
            {
                //    long targetUUID = grids[curChooseArea.Point1.x][curChooseArea.Point1.y].Data.UUID;
                //  bool isOrigin = targetUUID == originArea.UUID;//目标位置 是否与起始位置重合
                if (self.ContainGridObj(self.curChooseArea.Point1.x, self.curChooseArea.Point1.y, self.curChooseArea.Point2.x, self.curChooseArea.Point2.y))//格子为被占用//|| isOrigin
                {
                    //格子被占用 回到起始位置
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "此处无法放置物品");
                    self.ResetGridObj();
                }
                else
                {

                    //请求服务端 物品从背包移动到合成面板
                    action?.Invoke();
                }
            }
        }
      
        /// <summary>
        /// 刷新 合成方法
        /// </summary>
        private static void RefreshMergerMethods(this UIKnapsackNewComponent self, KnapsackDataItem item)
        {
            self.curMergerMethod = null;
            if (item == null && self.alreadyAddMergerItems.Count == 0)
            {

                return;
            }
            for (int i = 0, length = self.allMergerMethod.Count; i < length; i++)
            {
                var mergermethod = self.allMergerMethod[i].Init(self.alreadyAddMergerItems);
                // Log.DebugRed($"allMergerMethod[i].Init(alreadyAddMergerItems):{mergermethod.CheckItems.Count}");
                mergermethod.AddCheackItem(item);
                // Log.DebugRed($"allMergerMethod[i].AddCheackItem(alreadyAddMergerItems):{mergermethod.CheckItems.Count}");
                if (mergermethod.CanUserThisMergerMethod())
                {
                    //Log.DebugBrown($"curMergerMethod方法 {mergermethod}");
                    self.curMergerMethod = mergermethod;
                    if (mergermethod.IsHideSynTopSucess)//隐藏成功 几率
                    {
                        self.TopTipSucceed.gameObject.SetActive(false);
                    }
                    break;
                }
            }
        }

        public static void OnPackageDragToHouse(this UIKnapsackNewComponent self)
        {
            if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法移动到仓库");
                self.ResetGridObj();
            }
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法移动到仓库");
                self.ResetGridObj();
            }
            //else if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)
            //{
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, "绑定物品 无法移动到仓库");
            //    ResetGridObj();
            //}
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "锁定物品 无法移动到仓库");
                self.ResetGridObj();
            }
            else
            {
                self.ChangeItemPos(() =>
                {
                    self.SendKnapsackItem2WareHouseMessage().Coroutine();
                });
            }
        }

        /// <summary>
        /// 背包拖拽到仓库
        /// </summary>
        /// <returns></returns>
        public static async ETVoid SendKnapsackItem2WareHouseMessage(this UIKnapsackNewComponent self)
        {
            self.curChooseArea.UUID = self.originArea.UUID;
            self.curChooseArea.ItemData = self.originArea.ItemData;

            G2C_AddWarehouseItem g2C_AddWarehouse = (G2C_AddWarehouseItem)await SessionComponent.Instance.Session.Call(new C2G_AddWarehouseItem
            {
                ItemUUID = self.originArea.UUID,
                PosInBackpackX = self.curChooseArea.Point1.x,
                PosInBackpackY = self.curChooseArea.Point1.y + (UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y * (self.curPage - 1)),
            });
            if (g2C_AddWarehouse.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_AddWarehouse.Error.GetTipInfo());

               self. ResetGridObj();
            }
            else
            {

                self.ClearDropObj();
            }
        }
       
        /// <summary>
        /// 仓库 到背包
        /// </summary>
        /// <returns></returns>
        public static async ETVoid SendWareHouse2KnapsackItemMessage(this UIKnapsackNewComponent self)
        {
            G2C_DelWarehouseItem g2C_DelWarehouseItem = (G2C_DelWarehouseItem)await SessionComponent.Instance.Session.Call(new C2G_DelWarehouseItem
            {
                ItemUUID = self.originArea.UUID,
                PosInBackpackX = self.curChooseArea.Point1.x,
                PosInBackpackY = self.curChooseArea.Point1.y
            });
            if (g2C_DelWarehouseItem.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_DelWarehouseItem.Error.GetTipInfo());
                self.ResetGridObj();
            }
            else
            {
                self.ClearDropObj();
                self. curChooseArea.UUID = self.originArea.UUID;
                self. curChooseArea.ItemData = self.originArea.ItemData;

                //移除 仓库中的物品
                self.RemoveWareHouse(self.originArea.UUID);
                //改变物品的位置
                self.RemoveItem(self.originArea);

            }
        }

        /// <summary>
        /// 仓库 移除物品
        /// </summary>
        /// <param name="removedataItemUUID"></param>
        public static void RemoveWareHouse(this UIKnapsackNewComponent self,long removedataItemUUID)
        {
            if (self.PageList[self.curPage - 1].Exists(r => r.Id == removedataItemUUID))
            {
                var item = self.PageList[self.curPage - 1].Find(r => r.Id == removedataItemUUID);
                // item.Dispose();
                self.PageList[self.curPage - 1].Remove(item);
            }
        }

        /// <summary>
        /// 移动仓库物品的位置 
        /// </summary>
        /// <returns></returns>
        public static async ETVoid MoveWarehouseItemAsync(this UIKnapsackNewComponent self)
        {

            G2C_MoveWarehouseItem g2C_MoveWarehouseItem = (G2C_MoveWarehouseItem)await SessionComponent.Instance.Session.Call(new C2G_MoveWarehouseItem
            {
                ItemUUID = self.originArea.UUID,
                PosInBackpackX = self.curChooseArea.Point1.x,
                PosInBackpackY = self.curChooseArea.Point1.y + (UIKnapsackNewComponent.MAX_HOUSE_LENGSH_Y * (self.curPage - 1))
            });
            if (g2C_MoveWarehouseItem.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MoveWarehouseItem.Error.GetTipInfo());

                self.ResetGridObj();
            }
            else
            {
                //推送G2C_MoveWarehouseItem_notice 
                self.curChooseArea.UUID = self.originArea.UUID;
                self.curChooseArea.ItemData = self.originArea.ItemData;

                //改变物品的位置
                self.RemoveItem(self.originArea);
                self.AddKnapsackItem(self.curChooseArea, self.curDropObj);
            }
        }

        public static void OnPackageDragToShop(this UIKnapsackNewComponent self)
        {
            //if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            //{
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法出售");
            //    self.ResetGridObj();
            //}
            if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法出售");
                self.ResetGridObj();
            }
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定 无法出售");
                self.ResetGridObj();
            }
            else
            {
                self.curDropObj.transform.SetPositionAndRotation(self.originObjPos, self.originObjRotation);
                //提示玩家是否要卖出 改物品
                var confirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                self.originArea.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                confirm.SetTipText($"是否将<color=yellow>{item_Info.Name}</color> 以<color=red>{self.originArea.ItemData.GetProperValue(E_ItemValue.SellMoney)}金币</color>出售？");
                confirm.AddActionEvent(async () =>
                {
                    //确定将 物品 出售
                    G2C_SellingItemToNPCShop g2C_SellingItemToNPC = (G2C_SellingItemToNPCShop)await SessionComponent.Instance.Session.Call(new C2G_SellingItemToNPCShop
                    {
                        NPCShopID = self.CurNpcUUid, //商店NPC Id
                        ItemUUID = self.originArea.UUID //卖出的物品的 UUID
                    });
                    if (g2C_SellingItemToNPC.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SellingItemToNPC.Error.GetTipInfo());
                        self.ResetGridObj();
                    }
                    else
                    {
                        //从背包中移除
                        //ResourcesComponent.Instance.RecycleGameObject(self.curDropObj);
                        //self.ClearDropObj();
                    }
                });
                confirm.AddCancelEventAction(() =>
                {
                    //取消物品出售
                    self.ResetGridObj();
                });
            }
        }
       
        /// <summary>
        /// 自己摊位 拖到 背包
        /// </summary>
        /// <param name="self"></param>
        public static void OnStallUpDragToPackage(this UIKnapsackNewComponent self)
        {
            self.ChangeItemPos(async () =>
            {
                G2C_BaiTanRemoveItemResponse g2C_BaiTanRemoveItem = (G2C_BaiTanRemoveItemResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanRemoveItemRequest
                {
                    Prop = new C2G_BaiTanItemMessage
                    {
                        ItemUUID = self.originArea.UUID,
                        ConfigId = self.originArea.ItemData.ConfigId,
                        PosInBackpackX = self.curChooseArea.Point1.x,
                        PosInBackpackY = self.curChooseArea.Point1.y,
                        Price = 0//价格
                    }
                });
                if (g2C_BaiTanRemoveItem.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanRemoveItem.Error.GetTipInfo());
                }
                else
                {
                    self.ClearDropObj();
                    self.RemoveStallUpItem(self.originArea.UUID);
                    self.RemoveItem(self.originArea);
                }

            });
        }
       
        /// <summary>
        ///移除摊位上的物品
        /// </summary>
        /// <param name="itemUUId"></param>
        public static void RemoveStallUpItem(this UIKnapsackNewComponent self, long itemUUId)
        {
            if (self.stallUpComponent.StallUpItemDic.TryGetValue(itemUUId, out KnapsackDataItem knapsack))
            {
                knapsack.Dispose();
            }
            self.stallUpComponent.StallUpItemDic.Remove(itemUUId);
        }

        public static void OnPackageDragToStallUp(this UIKnapsackNewComponent self)
        {
            if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法摆摊出售");
                self.ResetGridObj();
            }
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法摆摊出售");
                self.ResetGridObj();
            }
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用状态 无法摆摊出售");
                self.ResetGridObj();
            }
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定状态 无法摆摊出售");
                self.ResetGridObj();
            }
            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.ValidTime) != 0) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "限时物品 无法摆摊出售");
                self.ResetGridObj();
            }
            else
            {
                self.ChangeItemPos(() =>
                {
                    //self.curDropObj.transform.SetPositionAndRotation(self.originObjPos, self.originObjRotation);
                    self.ResetGridObj();
                    UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent(E_TipPanelType.StallUp);
                    confirmComponent.coinprice = 0;
                    confirmComponent.yuanbaoprice = 0;
                    confirmComponent.StallUpEventAction = async () =>
                    {
                        var price = confirmComponent.GetStallUpGoldFunc?.Invoke();
                        var yuanbaoprice = confirmComponent.GetStallUpYuanBaoFunc?.Invoke();

                        self.StallUpData = ComponentFactory.CreateWithId<KnapsackDataItem>(self.originArea.UUID);
                        self.StallUpData.GameUserId = self.roleEntity.Id;
                        self.StallUpData.UUID = self.originArea.UUID;
                        self.StallUpData.ConfigId = self.originArea.ItemData.ConfigId;
                        self.StallUpData.PosInBackpackX = self.curChooseArea.Point1.x;
                        self.StallUpData.PosInBackpackY = self.curChooseArea.Point1.y;
                        self.StallUpData.X = self.originArea.ItemData.X;
                        self.StallUpData.Y = self.originArea.ItemData.Y;
                        self.StallUpData.SetProperValue(E_ItemValue.Stall_SellPrice, (int)price);
                        self.StallUpData.SetProperValue(E_ItemValue.Stall_SellMoJingPrice, (int)yuanbaoprice);

                        //确定上架物品
                        G2C_BaiTanAddItemResponse g2C_BaiTanAddItem = (G2C_BaiTanAddItemResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanAddItemRequest
                        {
                            Prop = new C2G_BaiTanItemMessage
                            {
                                ItemUUID = self.StallUpData.UUID,
                                ConfigId = self.StallUpData.ConfigId,
                                PosInBackpackX = self.curChooseArea.Point1.x,
                                PosInBackpackY = self.curChooseArea.Point1.y,
                                Price = (int)price,//金币价格
                                Price2 = (int)yuanbaoprice,//魔晶价格
                            }
                        });
                        if (g2C_BaiTanAddItem.Error != 0)
                        {
                            self.StallUpData.Dispose();
                            self.StallUpData = null;
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanAddItem.Error.GetTipInfo());
                        }
                        else
                        {
                            UIComponent.Instance.InVisibilityUI(UIType.UIConfirm);
                        }

                    };
                    confirmComponent.StallUpCancelAction = () =>
                    {

                    };

                });
            }
        }

        public static void OnPackageDragToStallUpOtherPlayer(this UIKnapsackNewComponent self)
        {
            if (UnitEntityComponent.Instance.AllUnitEntityDic.ContainsKey(self.otherRole.Id) == false)
            {
                self.ResetGridObj();
                UIComponent.Instance.VisibleUI(UIType.UIHint, "玩家已经下线 无法购买");

            }
            else if (self.otherRole.GetComponent<RoleStallUpComponent>().IsStallUp == false)
            {
                self.ResetGridObj();
                UIComponent.Instance.VisibleUI(UIType.UIHint, "对方以摊位已关闭，无法购买");
            }
            else
            {
                self.ChangeItemPos(() =>
                {
                    UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    self.originArea.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                    var jinbi = self.originArea.ItemData.GetProperValue(E_ItemValue.Stall_BuyPrice);
                    var yuanbao = self.originArea.ItemData.GetProperValue(E_ItemValue.Stall_BuyMoJingPrice);

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
                        if (jinbi > self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin))
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足");
                            self.ResetGridObj();
                            return;
                        }
                        //判断元宝 是否足够
                        if (yuanbao > self.roleEntity.Property.GetProperValue((E_GameProperty)E_GameProperty.MoJing))
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "魔晶不足");
                            self.ResetGridObj();
                            return;
                        }

                        //请求购买

                        G2C_BaiTanBuyItemResponse g2C_BaiTanBuyItemResponse = (G2C_BaiTanBuyItemResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanBuyItemRequest
                        {
                            BaiTanInstanceId = ClickSelectUnitEntityComponent.Instance.curSelectUnit.Id,
                            ItemUUID = self.originArea.UUID,
                            PosInBackpackX = self.curChooseArea.Point1.x,
                            PosInBackpackY = self.curChooseArea.Point1.y,
                        });
                        if (g2C_BaiTanBuyItemResponse.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanBuyItemResponse.Error.GetTipInfo());
                            self.ResetGridObj();
                        }
                        else
                        {
                            self.ClearDropObj();
                            self.RemoveItem(self.originArea);
                        }

                    }));
                    confirmComponent.AddCancelEventAction(() =>
                    {
                        self.ResetGridObj();
                    });
                });
            }
        }


        public static void OnEndDrag(this UIKnapsackNewComponent self, int x, int y, E_Grid_Type type)
        {
            if (self.curKnapsackState == E_KnapsackState.KS_Revision) return;


            self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, self.curChooseArea.Grid_Type);
            //Log.Info("OnEndDrag    " + isDroping);
            if (self.isDroping == false || self.curDropObj == null) return;
            //起始区域是背包
            Log.Info("OnEndDrag  originArea.Grid_Type 2   " + self.originArea.Grid_Type);
            Log.Info("OnEndDrag  curChooseArea.Grid_Type 2   " + self.curChooseArea.Grid_Type);
            do
            {

                #region 从背包到其他面板
                //从背包拖到丢弃
                if (self.originArea.Grid_Type == E_Grid_Type.Knapsack && self.curChooseArea.Grid_Type == E_Grid_Type.Delete)
                {
                    self.OnPackageDragToDelete().Coroutine();
                    break;
                }

                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.None))
                {
                    self.ResetGridObj();
                    break;
                }

                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Knapsack))
                {
                    self.OnPackageToPackage().Coroutine();
                    break;
                }
                //背包 拖到 装备栏
                if (self.originArea.Grid_Type == E_Grid_Type.Knapsack && ((self.curChooseArea.Grid_Type >= E_Grid_Type.Weapon && self.curChooseArea.Grid_Type <= E_Grid_Type.TianYing) || self.curChooseArea.Grid_Type == E_Grid_Type.Mounts) )
                {
                    self.OnPackageDrawToEquip().Coroutine();
                    break;
                }
                //背包 拖拽 到合成面板
                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Gem_Merge))
                {
                    self.OnPackageDragToHeCheng();
                    break;
                }
                // 背包   拖拽到仓库
                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Ware_House))
                {
                    self.OnPackageDragToHouse();
                    break;
                }
                // 背包   拖拽到NPC 商城
                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Shop))
                {
                    self.OnPackageDragToShop();
                    break;
                }
                // 背包   拖拽到自己的摊位
                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack,E_Grid_Type.Stallup ))
                {
                    self.OnPackageDragToStallUp();
                    break;
                }

                if((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Stallup_OtherPlayer, E_Grid_Type.Knapsack))
                {
                    self.OnPackageDragToStallUpOtherPlayer();
                    break;
                }

                if((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Reduction, E_Grid_Type.Knapsack))
                {
                    //属性还原面板 拖到 背包
                    self.ChangeItemPos(() =>
                    {

                    });
                    break;
                }
                //背包 拖到 镶嵌
                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Knapsack, E_Grid_Type.Inlay))
                {
                    self.OnPackageDrogToInlayPlane();
                    break;
                }

                if((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Trade, E_Grid_Type.Knapsack))
                {
                    self.ChangeItemPos(async () =>
                    {
                        ///移除交易物品
                        G2C_ReMoveExchangeItem g2C_MoveExchange = (G2C_ReMoveExchangeItem)await SessionComponent.Instance.Session.Call(new C2G_ReMoveExchangeItem { ItemUUID = self.originArea.UUID });
                        if (g2C_MoveExchange.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MoveExchange.Error.GetTipInfo());
                        }
                        else
                        {
                            self.AddKnapsackItem(self.curChooseArea, self.curDropObj);
                            self.RemoveItem(self.originArea);
                        }
                    });
                    break;
                }

                #endregion

                #region 从其他面板拖拽到背包

                //仓库 拖到 背包
                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Ware_House, E_Grid_Type.Knapsack))
                {
                    self.ChangeItemPos(() => self.SendWareHouse2KnapsackItemMessage().Coroutine());
                    break;
                }

                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Stallup, E_Grid_Type.Knapsack))
                {
                    self.OnStallUpDragToPackage();
                    break;
                }

                //合成面板 拖到 背包
                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Gem_Merge, E_Grid_Type.Knapsack))
                {
                    self.ChangeItemPos(async () =>
                    {
                        //移除缓存区域
                        G2C_MoveCacheSpaceItemToBackpack g2C_MoveCacheSpaceItemToBackpack = (G2C_MoveCacheSpaceItemToBackpack)await SessionComponent.Instance.Session.Call(new C2G_MoveCacheSpaceItemToBackpack
                        {
                            MovedItemUUID = self.originArea.UUID,
                            PosInBackpackX = self.curChooseArea.Point1.x,
                            PosInBackpackY = self.curChooseArea.Point1.y
                        });
                        if (g2C_MoveCacheSpaceItemToBackpack.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MoveCacheSpaceItemToBackpack.Error.GetTipInfo());
                            self.ResetGridObj();
                        }
                        else
                        {

                            self.RemoveMergerItem();
                            self. RefreshMergerMethods(null);
                            self. originArea.ItemData.Dispose();
                            self.RemoveItem(self.originArea);
                            self.UpdateTips();
                        }
                    });
                    break;
                }

                //NPC商城 拖到 背包
                if ((self.originArea.Grid_Type, self.curChooseArea.Grid_Type) is (E_Grid_Type.Shop, E_Grid_Type.Knapsack))
                {
                    self.OnVipDragToPackage();
                    break;
                }

                #endregion


                #region 其他面板内部拖拽
                if (self.originArea.Grid_Type == self.curChooseArea.Grid_Type)
                {
                    self.ChangeItemPos(async () =>
                    {
                        if (self.curChooseArea.Grid_Type == E_Grid_Type.Ware_House)//仓库 内部拖拽
                        {
                            //请求服务端 改变物品的位置信息
                            self.MoveWarehouseItemAsync().Coroutine();
                        }
                        else if (self.curChooseArea.Grid_Type == E_Grid_Type.Trade)//交易面板 内部拖拽
                        {

                            MoveExchangeItem().Coroutine();
                            ///移动交易物品的位置
                            async ETVoid MoveExchangeItem()
                            {
                                G2C_MoveExchangeItem g2C_MoveExchange = (G2C_MoveExchangeItem)await SessionComponent.Instance.Session.Call(new C2G_MoveExchangeItem
                                {
                                    ItemUUID = self.originArea.UUID,
                                    PosInBackpackX = self.curChooseArea.Point1.x,
                                    PosInBackpackY = self.curChooseArea.Point1.y
                                });
                                if (g2C_MoveExchange.Error != 0)
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MoveExchange.Error.GetTipInfo());
                                    self.ResetGridObj();
                                }
                                else
                                {
                                    self.AddKnapsackItem(self.curChooseArea, self.curDropObj);
                                    self.RemoveItem(self.originArea);
                                }
                            }
                        }
                        else if (self.curChooseArea.Grid_Type == E_Grid_Type.Stallup)//摆摊面板 内部拖拽
                        {
                            G2C_BaiTanChangeDataResponse g2C_BaiTanChangeData = (G2C_BaiTanChangeDataResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanChangeDataRequest
                            {
                                Prop = new C2G_BaiTanItemMessage
                                {
                                    ConfigId = self.originArea.ItemData.ConfigId,
                                    ItemUUID = self.originArea.UUID,
                                    PosInBackpackX = self.curChooseArea.Point1.x,
                                    PosInBackpackY = self.curChooseArea.Point1.y
                                }
                            });
                            if (g2C_BaiTanChangeData.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanChangeData.Error.GetTipInfo());
                                self.ResetGridObj();
                            }
                            else
                            {
                                self.curChooseArea.UUID = self.originArea.UUID;
                                self.curChooseArea.ItemData = self.originArea.ItemData;
                                self.AddKnapsackItem(self.curChooseArea, self.curDropObj);
                                //改变物品的位置
                                self.RemoveItem(self.originArea);

                            }
                        }
                        else
                        {
                            //回到起始位置
                            self.ResetGridObj();
                        }
                    });
                    break;
                }
                #endregion

                self.ResetGridObj();

            } while (false);
            self.isDroping = false;

        }

        /// <summary>
        /// 移除已经放入合成面板的物品
        /// </summary>
        public static void RemoveMergerItem(this UIKnapsackNewComponent self)
        {
            if (self.alreadyAddMergerItems.Exists(r => r.UUID == self.originArea.UUID))
            {
                var item = self.alreadyAddMergerItems.Find(r => r.UUID == self.originArea.UUID);
                item.Dispose();
                self.alreadyAddMergerItems.Remove(item);
            }

        }

        public static void OnPointerEnter(this UIKnapsackNewComponent self, int x, int y, E_Grid_Type type)
        {
            if (self.curKnapsackState == E_KnapsackState.KS_Revision) return;

            if (x == -1)
            {
                //删除区域
                self.curChooseArea.SetSinglePoint(new Vector2Int(-1, -1));
                self.curChooseArea.Grid_Type = E_Grid_Type.Delete;

                return;
            }

            if (type == E_Grid_Type.Inlay)
            {
                ///镶嵌 面板
               self.OnPointerEnter_Inlay(type, y);
                return;
            }
            //没有拖拽 直接返回
            if (!self.isDroping) return;

            //记录当前进入的格子 信息
            self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, type);

            if (self.originArea.IsSinglePoint)
            {
                self.curChooseArea.SetSinglePoint(new Vector2Int(x, y));
                self.grids[x][y].ReadyColor();
                self.curChooseArea.Grid_Type = self.grids[x][y].Grid_Type;
            }
            else
            {
                Vector2Int centerOffer = self.GetCenterGrid();
                x += centerOffer.x;
                y += centerOffer.y;

                Vector2Int offset = new Vector2Int(x, y) - self.originArea.Point1;

                Vector2Int endPoint = self.originArea.Point2 + offset;

                List<KnapsackNewGrid> gridList = self.GetAreaGrids(x, y, endPoint.x, endPoint.y, self.LENGTH_X, self.LENGTH_Y);

                if (gridList.Count == 0) return;
                foreach (KnapsackGrid item in gridList)
                {
                    item.ReadyColor();
                }
                self.curChooseArea.Point1 = new Vector2Int(x, y);
                self.curChooseArea.Point2 = new Vector2Int(endPoint.x, endPoint.y);
                self.curChooseArea.Grid_Type = self.grids[x][y].Grid_Type;
            }
            Log.DebugGreen($"进入背包格子：起始格子类型:{self.originArea.Grid_Type} 目标格子类型:{self.curChooseArea.Grid_Type}");
        }

        private static Vector2Int GetCenterGrid(this UIKnapsackNewComponent self)
        {
            Vector2Int offset = self.originArea.Point1 - self.originArea.Point2;
            offset = new Vector2Int(((int)(offset.x / 2f)), ((int)(offset.y / 2f)));
            return offset;
        }

        public static void OnPointerClickEvent(this UIKnapsackNewComponent self, int x, int y, E_Grid_Type type)
        {
            do
            {
                if (x == -1)
                {
                    self.ClearIntroduction();
                    break;
                }

                self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, type);

                KnapsackGrid grid = self.grids[x][y];
                self.originArea = grid.Data;
                self.originArea.Grid_Type = grid.Grid_Type;

                //可以分堆
                if (self.IsSplit)
                {
                    if (self.confirmComponent != null && self.originArea.ItemData != null)
                    {
                        if (self.originArea.ItemData.GetProperValue(E_ItemValue.Quantity) == 1)
                        {
                            break;
                        }
                        self.confirmComponent.splitItem = self.originArea;
                        if (self.confirmComponent.SplitObj != null)//&& confirmComponent.SplitObj.name != originArea.ItemData.item_Info.ResName
                        {
                            ResourcesComponent.Instance.RecycleGameObject(self.confirmComponent.SplitObj);
                            //ResourcesComponent.Instance.DestoryGameObjectImmediate(confirmComponent.SplitObj, confirmComponent.SplitObj.name.StringToAB());
                        }

                        if (grid == null) break;

                        self.confirmComponent.SplitObj = ResourcesComponent.Instance.LoadGameObject(grid.GridObj.name.StringToAB(), grid.GridObj.name);
                        self.confirmComponent.SplitObj.SetUI();
                        self.confirmComponent.SplitObj.transform.SetParent(self.confirmComponent.objIcon.transform, false);
                        self.confirmComponent.SplitObj.transform.localPosition = new Vector3(0, 0, -50);
                        // confirmComponent.SplitObj.transform.position = confirmComponent.objPos;
                        self.confirmComponent.SplitinputField.text = self.confirmComponent.splitItem.ItemData.GetProperValue(E_ItemValue.Quantity).ToString();//显示 物品的数量
                    }
                    break;

                }

                if (grid.IsOccupy == false)
                {
                    self.ClearIntroduction();
                    break;
                }

                KnapsackGridData data = grid.Data;

                
                Vector3 pos = data.IsSinglePoint ? new Vector3(grid.Image.transform.position.x, grid.Image.transform.position.y, 0) : self.GetCenterPos(data.Point1.x, data.Point1.y, data.Point2.x, data.Point2.y);

                //物品属性面板
                KnapsackDataItem dataItem = data.ItemData;
                self.uIIntroduction ??= UIComponent.Instance.VisibleUI(UIType.UIIntroduction).GetComponent<UIIntroductionComponent>();
                E_KnapsackIntroduceShowPrice e_Knapsack = GetPticeType();
                self.uIIntroduction.GetAllAtrs(dataItem, e_Knapsack);

                //if (self.curKnapsackState == E_KnapsackState.KS_Knapsack &&  1 <= data.EquipmentPart &&  data.EquipmentPart <= 13)
                if (1 <= data.EquipmentPart &&  data.EquipmentPart <= 13 && type == E_Grid_Type.Knapsack || data.EquipmentPart == (int)E_Grid_Type.Mounts)
                {
                    if (self.EquipmentComponent.curWareEquipsData_Dic.TryGetValue((E_Grid_Type)data.EquipmentPart, out KnapsackDataItem item))
                    {

                        ShowEquipAtr();

                        self.uIIntroduction.IsListring(self.IsShangJia());
                    }
                    else if (IsRing())
                    {
                        item = self.EquipmentComponent.curWareEquipsData_Dic[(E_Grid_Type)self.ringSlot];
                        ShowEquipAtr();

                        self.uIIntroduction.IsListring(self.IsShangJia());
                    }
                    else
                    {
                        ShowBaseAtr();
                    }

                    //显示装备 对照属性
                    void ShowEquipAtr()
                    {
                        self.uIIntroduction.GetEquipAllAtrs(item, e_Knapsack);
                        self.uIIntroduction.ShowEquipInfo();
                        //丢弃物品
                        self.uIIntroduction.DiscarACtion = () =>
                        {
                            if (type == E_Grid_Type.Knapsack)
                            {
                                Log.DebugGreen($"ID->{self.originArea.ItemData.ConfigId}");
                                SceneName sceneName = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>();
                                if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法丢弃");

                                }
                                else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法丢弃");

                                }
                                else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法丢弃");

                                }
                                else if (self.originArea.ItemData.ConfigId == 320106 && self.roleEntity.IsSafetyZone)
                                {
                                    List<string> list = new List<string>();
                                    self.originArea.ItemData.GetItemName(ref list);
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"安全区不能使用{list.ListToString().Split(',')[1]}");

                                }
                                else if ((self.originArea.ItemData.ConfigId == 320106 && !self.roleEntity.IsSafetyZone) && (sceneName == SceneName.XueSeChengBao || sceneName == SceneName.EMoGuangChang || sceneName == SceneName.GuZhanChang || sceneName == SceneName.kalima_map))
                                {
                                    List<string> list = new List<string>();
                                    self.originArea.ItemData.GetItemName(ref list);
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"副本里不能使用{list.ListToString().Split(',')[1]}");
                                }
                                else if (!self.originArea.ItemData.IsCanTrade() && self.originArea.ItemData.ConfigId != 320162) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "该物品不能丢弃1");
                                    self.ResetGridObj();
                                }
                                else
                                {
                                    //丢弃物品
                                    self.SendDiscardKnasackItemMessage().Coroutine();
                                    self.ClearIntroduction();
                                }
                            }
                        };
                        //穿戴装备
                        self.uIIntroduction.WareAction = () =>
                        {

                            if (IsCanReplace())
                            {
                                self.curChooseArea.Grid_Type = (E_Grid_Type)data.EquipmentPart;
                                if (!self.IsCanWearEquip(ref self.curWarePart))
                                {
                                    //不能穿戴
                                    return;
                                }

                                if (IsRing())
                                {
                                    self.RemoveWareEquipItem(self.ringSlot, true);//移除装备栏的物品
                                    RequestReplaceEquip(self.ringSlot).Coroutine();

                                }
                                else
                                {


                                    self.RemoveWareEquipItem(data.EquipmentPart, true);//移除装备栏的物品
                                    RequestReplaceEquip(data.EquipmentPart).Coroutine();


                                }
                            }
                            else
                            {
                                var pos = GetPosInKnapsack();

                                if (pos.x == -1)
                                {
                                    item.ConfigId.GetItemInfo_Out(out Item_infoConfig equipconfig);
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"背包中 没有 {equipconfig.X} x {equipconfig.Y} 未使用的格子");
                                }
                                else
                                {
                                    UnLoadWareEquip(pos);
                                    WareEquip();
                                    self.ClearIntroduction();
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

                                    return new Vector2Int(-1, -1);
                                }
                                else
                                {
                                    return vector2Int;
                                }

                            }

                            //卸载装备
                            void UnLoadWareEquip(Vector2Int? pos)
                            {
                                self.RemoveWareEquipItem(data.EquipmentPart, true);//移除装备栏的物品
                                                                                   //请求 服务器 卸载装备
                                self.UnLoadEquip(data.EquipmentPart, pos.Value.x, pos.Value.y).Coroutine();

                            }

                            //穿戴装备
                            void WareEquip()
                            {
                                self.curChooseArea.Grid_Type = (E_Grid_Type)data.EquipmentPart;
                                if (!self.IsCanWearEquip(ref self.curWarePart))
                                {
                                    //不能穿戴
                                    // ResetGridObj();
                                }
                                else
                                {
                                    self.curChooseArea.UUID = dataItem.UUID;
                                    self.curChooseArea.ItemData = dataItem;
                                    self.curWarePart = data.EquipmentPart;//当前选择的格子的类型
                                    self.RequestWareEquip().Coroutine();  //请求穿戴装备
                                }
                            }
                        };
                        //分享
                        self.uIIntroduction.VerticalShareAction = async () =>
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
                                ShareItemId = self.originArea.UUID,
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
                                
                                self.ClearIntroduction();
                            }
                        };

                        self.uIIntroduction.SellAction = (int value) =>
                        {
                            Log.DebugGreen("上架1");
                            self.listingTreasureHouse(dataItem.Id, value).Coroutine();
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
                            
                            self.ClearIntroduction();
                        }
                        else
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ReplaceEquip.Error.GetTipInfo());
                        }
                    }

                    bool IsRing()
                    {
                        bool result = false;
                        if (data.EquipmentPart != 11 && data.EquipmentPart != 12)
                        {
                            return result;
                        }
                        for (int i = 11; i <= 12; i++)
                        {
                            if (self.EquipmentComponent.curWareEquipsData_Dic.ContainsKey((E_Grid_Type)i))
                            {

                                result = true;
                                self.ringSlot = i;
                                break;
                            }
                        }
                        return result;
                    }


                }  //获取价格显示
                else
                {

                    ShowBaseAtr();
                }
                void ShowBaseAtr()
                {
                    Log.Info("ShowBaseAtr " + self.isVertical);
                    pos += Vector3.left / 2;

                    if (self.isVertical)
                    {
                        //垂直显示装备属性
                        bool isShow = false;
                        bool isSell = false;
                        bool isBuy = false;
                        bool isUser = false;
                        bool isShare = false;
                        bool isListring = false;
                        if (self.curKnapsackState == E_KnapsackState.KS_Knapsack
                        && ((int)E_Grid_Type.Weapon <= data.EquipmentPart &&
                        data.EquipmentPart <= (int)E_Grid_Type.TianYing || data.EquipmentPart == (int)E_Grid_Type.Mounts))
                        {
                            isShow = true;
                        }

                        if (self.curKnapsackState == E_KnapsackState.KS_Knapsack
                        && (dataItem.ItemType == (int)E_ItemType.SkillBooks||dataItem.ItemType == (int)E_ItemType.Consumables)||dataItem.ItemType == (int)E_ItemType.Pet)
                        {

                            isShow = true;
                            isUser = true;
                            if (dataItem.ConfigId == 260015 || dataItem.ConfigId == 260019 /*|| dataItem.ItemType == (int)E_ItemType.Pet*/)
                            {
                                //天鹰、烈火凤凰 属于装备
                                isUser = false;
                            }
                        }

                        Log.Debug("数据类型" + dataItem.ItemType+"::格子类型"+ self.originArea.Grid_Type+ ":::self.curKnapsackState");
                        if (dataItem.ItemType == (int)E_ItemType.Mounts)
                        {
                               isShow = true;
                               isUser = true;
                        }
                        if (self.curKnapsackState == E_KnapsackState.KS_Shop && self.originArea.Grid_Type == E_Grid_Type.Knapsack)
                        {

                            isSell = true;
                        }

                        if (self.curKnapsackState == E_KnapsackState.KS_Shop && self.originArea.Grid_Type == E_Grid_Type.Shop)
                        {
                            isBuy = true;
                        }

                        if (self.curKnapsackState == E_KnapsackState.KS_Knapsack && dataItem.ItemType >= (int)E_ItemType.Swords && dataItem.ItemType <= (int)E_ItemType.Mounts)
                        {
                            isShare = true;
                          //  isUser = true;

                        }
                        self.originArea.ItemData.ConfigId.GetItemInfo_Out(out self.originArea.ItemData.item_Info);
                        isListring = false;
                        if (self.originArea.Grid_Type == E_Grid_Type.Knapsack|| self.originArea.Grid_Type == E_Grid_Type.Ware_House)
                        {

                            isListring = self.IsShangJia();
                        }

                        if (self.curKnapsackState == E_KnapsackState.KS_Revision)
                        {
                            isShow = false;
                            isSell = false;
                            isBuy = false;
                            isUser = false;
                            isShare = false;
                            isListring = false;
                        }
                        Log.DebugBrown("数据使用isuser" + isUser+"::::isbuy"+isBuy);
                        self.uIIntroduction.ShowAtr_Vertical(isShow, isSell, isBuy, isUser, isShare, isListring);
                        self.uIIntroduction.SetVerticalPos(pos, 1);
                        //丢弃物品
                        self.uIIntroduction.VerticalDiscarACtion = () =>
                        {
                            if (self.curKnapsackState == E_KnapsackState.KS_Knapsack)
                            {
                                SceneName sceneName = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>();
                                if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1)//处于绑定状态 无法交易、丢弃、摆摊
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法丢弃");

                                }
                                else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法丢弃");

                                }
                                else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法丢弃");

                                }
                                else if (self.originArea.ItemData.ConfigId == 320106 && self.roleEntity.IsSafetyZone)
                                {
                                    List<string> list = new List<string>();
                                    self.originArea.ItemData.GetItemName(ref list);
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"安全区不能使用{list.ListToString().Split(',')[1]}");

                                }
                                else if ((self.originArea.ItemData.ConfigId == 320106 && !self.roleEntity.IsSafetyZone) && (sceneName == SceneName.XueSeChengBao || sceneName == SceneName.EMoGuangChang || sceneName == SceneName.GuZhanChang || sceneName == SceneName.kalima_map))
                                {
                                    List<string> list = new List<string>();
                                    self.originArea.ItemData.GetItemName(ref list);
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"副本里不能使用{list.ListToString().Split(',')[1]}");
                                }

                                else if (!self.originArea.ItemData.IsCanTrade() && self.originArea.ItemData.ConfigId != 320162) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "该物品不能丢弃2");
                                    self.ResetGridObj();
                                }
                                else
                                {
                                    //丢弃物品
                                    self.SendDiscardKnasackItemMessage().Coroutine();
                                    self.ClearIntroduction();

                                }
                            }
                        };
                        //装备
                        self.uIIntroduction.VerticalWareAction = () =>
                        {
                            self.curChooseArea.Grid_Type = (E_Grid_Type)data.EquipmentPart;
                            if (!self.IsCanWearEquip(ref self.curWarePart))
                            {
                                //不能穿戴
                                // ResetGridObj();
                            }
                            else
                            {

                                self.curChooseArea.UUID = dataItem.UUID;
                                self.curChooseArea.ItemData = dataItem;
                                self.curWarePart = data.EquipmentPart;//当前选择的格子的类型
                                self.RequestWareEquip().Coroutine();  //请求穿戴装备

                                self.ClearIntroduction();
                            }
                        };
                        //使用物品
                        self.uIIntroduction.VerticalUserAction = () =>
                        {
                            Log.Info("使用物品-----------------------" + grid.Data.UUID + "  " + grid.Data.ItemData.ConfigId+":名字"+grid.Data.ItemData.name); ;
                            if (KnapsackItemsManager.KnapsackItems.TryGetValue(grid.Data.UUID, out KnapsackDataItem dataItem))
                            {
                                    if (dataItem.ItemType == (int)E_ItemType.Mounts)
                                    {
                                        if (self.roleEntity.IsSafetyZone && dataItem.ConfigId != 260020)
                                        {
                                            UIComponent.Instance.VisibleUI(UIType.UIHint, "安全区无法使用坐骑");
                                            return;
                                        }

                                        if (!dataItem.item_Info.IsCanUer((int)self.roleEntity.RoleType, self.roleEntity.ClassLev))
                                        {
                                            UIComponent.Instance.VisibleUI(UIType.UIHint, "不满足使用要求");
                                            return;
                                        }
                                    }
                                    else if (dataItem.ConfigId == 310102)//改名卡
                                    {
                                        self.ClearIntroduction();
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
                                                Name = roleName,
                                                ItemUUID = dataItem.Id,
                                            });
                                            if (g2C_BagChangeNameCard.Error != 0)
                                            {
                                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BagChangeNameCard.Error.GetTipInfo());
                                            }
                                            else
                                            {


                                                self.roleEntity.GetComponent<UIUnitEntityHpBarComponent>().SetEntityName($"<b>{roleName}</b>", ColorTools.GetColorHtmlString(Color.yellow));
                                                self.roleEntity.RoleName = roleName;

                                                UIComponent.Instance.Remove(UIType.UIConfirm);
                                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"改名成功");

                                            }
                                        };
                                    return;
                                }
                                self.PlayerUserItemInTheBackpack(dataItem).Coroutine();

                            }
                            else
                            {
                                Log.Info("使用物品错误-----------------------" + grid.Data.UUID + "  " + grid.Data.ItemData.ConfigId); ;

                            }
                        };

                        //出售
                        self.uIIntroduction.VerticalSellAction = () =>
                        {

                            if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsTask) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "任务物品 无法出售");

                            }
                            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsUsing) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于使用中 无法出售");

                            }
                            else if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于锁定 无法出售");

                            }
                            /* else if (originArea.ItemData.GetProperValue(E_ItemValue.IsBind) == 1) //任务物品 无法交易、丢弃、摆摊、移动到仓库、出售
                             {
                                 UIComponent.Instance.VisibleUI(UIType.UIHint, "物品处于绑定状态 无法出售");

                             }*/
                            else
                            {
                                self.ClearIntroduction();
                                //提示玩家是否要卖出 改物品
                                var confirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                                self.originArea.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                                confirm.SetTipText($"是否将<color=yellow>{item_Info.Name}</color> 以<color=red>{self.originArea.ItemData.GetProperValue(E_ItemValue.SellMoney)}金币</color>出售？");
                                confirm.AddActionEvent(async () =>
                                {
                                    //确定将 物品 出售
                                    G2C_SellingItemToNPCShop g2C_SellingItemToNPC = (G2C_SellingItemToNPCShop)await SessionComponent.Instance.Session.Call(new C2G_SellingItemToNPCShop
                                    {
                                        NPCShopID = self.CurNpcUUid, //商店NPC Id
                                        ItemUUID = self.originArea.UUID //卖出的物品的 UUID
                                    });
                                    if (g2C_SellingItemToNPC.Error != 0)
                                    {
                                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SellingItemToNPC.Error.GetTipInfo());

                                    }
                                });
                            }
                        };
                        //购买
                        self.uIIntroduction.VerticalBuyAction = () =>
                        {
                            //判断金币是否足够 购买改物品
                            if (self.originArea.ItemData.GetProperValue(E_ItemValue.BuyMoney) > self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin))
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足 无法购买");

                                return;
                            }
                            //获取背包中的位置
                            self.originArea.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig config);
                            if (self.originArea.ItemData == null || config == null)
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
                                    NPCUUID = self.CurNpcUUid,
                                    ItemUUID = self.originArea.UUID,
                                    PosInBackpackX = vector2Int.x,
                                    PosInBackpackY = vector2Int.y,
                                    RemoteBuy = (int)self.buyType  //0  默认购买=1//远程

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
                        self.uIIntroduction.VerticalShareAction = async () =>
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
                                ShareItemId = self.originArea.UUID,
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

                                self.ClearIntroduction();
                            }
                        };
                        self.uIIntroduction.SellAction = (int value) =>
                        {
                            //  Log.DebugGreen("上架2");
                            self.listingTreasureHouse(dataItem.Id, value).Coroutine();
                        };
                    }
                    else
                    {
                        self.uIIntroduction.ShowAtrs();
                        self.uIIntroduction.SetPos(pos, 1);
                    }

                }

                E_KnapsackIntroduceShowPrice GetPticeType() => (self.curKnapsackState, type) switch
                {
                    (E_KnapsackState.KS_Shop, E_Grid_Type.Shop) => E_KnapsackIntroduceShowPrice.BuyPrice,
                    (E_KnapsackState.KS_Shop, E_Grid_Type.Knapsack) => E_KnapsackIntroduceShowPrice.SellPrice,
                    (E_KnapsackState.KS_Stallup, E_Grid_Type.Stallup) => E_KnapsackIntroduceShowPrice.StallSellPrice,
                    (E_KnapsackState.KS_Stallup_OtherPlayer, E_Grid_Type.Stallup_OtherPlayer) => E_KnapsackIntroduceShowPrice.StallBuyPrice,
                    _ => E_KnapsackIntroduceShowPrice.None
                };


            } while (false);
        }
       
        /// <summary>
        /// 上架装备到藏宝阁
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="sellgold"></param>
        /// <returns></returns>
        public static async ETVoid listingTreasureHouse(this UIKnapsackNewComponent self,long uid, int sellgold)
        {
            G2C_listingTreasureHouse g2C_ListingTreasure = (G2C_listingTreasureHouse)await SessionComponent.Instance.Session.Call(new C2G_listingTreasureHouse()
            {
                ItemUUID = uid,
                ItemPrice = sellgold
            });
            Log.DebugBrown("藏宝阁上架listingTreasureHouse" + g2C_ListingTreasure.Error);
            if (g2C_ListingTreasure.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ListingTreasure.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "上架成功！");
                self.RemoveKnapsack(self.originArea.UUID);
                self.ClearIntroduction();
            }
        }

        public static bool IsShangJia(this UIKnapsackNewComponent self)
        {
            bool isListring = false;
            if (self.originArea.ItemData.item_Info.Sell == 0)
            {
                isListring = false;
            }
            else
            {
                //if (self.curKnapsackState != E_KnapsackState.KS_Shop)
                    if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsTask) != 1)//任务物品
                        if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsLocking) != 1)//锁定物品
                            if (self.originArea.ItemData.GetProperValue(E_ItemValue.IsBind) != 1)//未绑定物品
                            {
                                //装备类
                                if ((self.originArea.ItemData.ItemType <= (int)E_ItemType.Boots && self.originArea.ItemData.ItemType >= (int)E_ItemType.Swords) ||
                                    (self.originArea.ItemData.ItemType <= (int)E_ItemType.Dangler && self.originArea.ItemData.ItemType >= (int)E_ItemType.Rings) ||
                                    (self.originArea.ItemData.ItemType == (int)E_ItemType.QiZhi))
                                {
                                    if (self.originArea.ItemData.GetHaveInLayAtr() || (self.originArea.ItemData.GetProperValue(E_ItemValue.SetId) is int value1 && value1 != 0))//镶嵌装或者套装
                                    {
                                        isListring = true;
                                    }
                                    else
                                    if (self.originArea.ItemData.ExecllentEntryDic.Count >= 1)
                                    {
                                        isListring = true;
                                }
                                }//其他类
                            else
                            {
                                isListring = true;
                            }
                            if ((int)self.originArea.ItemData.ConfigId == 350001)//试用宠物
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
        public static void OnPointerExit(this UIKnapsackNewComponent self, int x, int y, E_Grid_Type type)
        {
            if (self.curKnapsackState == E_KnapsackState.KS_Revision) return;

            self.curChooseArea.Grid_Type = E_Grid_Type.None;//当前选择的各子 类型为空
            if (x == -1) return;

            if (type == E_Grid_Type.Inlay)
            {
                ///镶嵌 面板
                self.OnPointerExit_Inlay();
                return;
            }

            self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, type);
            //拖动中离开格子 清空记录的信息
            if (self.curChooseArea.IsSinglePoint)
            {
                self.grids[x][y].ResetColor();
            }
            else
            {
                Vector2Int centerOffset = self.GetCenterGrid();
                x += centerOffset.x;
                y += centerOffset.y;

                Vector2Int offset = new Vector2Int(x, y) - self.curChooseArea.Point1;
                Vector2Int endPoint = self.curChooseArea.Point2 + offset;

                List<KnapsackNewGrid> gridList = self.GetAreaGrids(x, y, endPoint.x, endPoint.y, self.LENGTH_X, self.LENGTH_Y);
                foreach (var item in gridList)
                {
                    item.ResetColor();
                }
            }
        }
        public static void OnPointerDownEvent(this UIKnapsackNewComponent self, int x, int y, E_Grid_Type type)
        {

        }

        public static void OnPointerUpEvent(this UIKnapsackNewComponent self, int x, int y, E_Grid_Type type)
        {
            if (self.curKnapsackState == E_KnapsackState.KS_Revision) return;

        }


        /// <summary>
        /// 检查当前放入背包装备 是否 比当前穿戴的装备好
        /// </summary>
        /// <param name="dataItem">true 好  false 不好</param>
        /// <returns></returns>

        public static bool CheckItemlev(this UIKnapsackNewComponent self, KnapsackGridData dataItem)
        {
            self.EquipmentComponent ??= self.roleEntity.GetComponent<RoleEquipmentComponent>();
            dataItem.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);

            if (self.EquipmentComponent.curWareEquipsData_Dic.TryGetValue((E_Grid_Type)item_Info.Slot, out KnapsackDataItem item))
            {
                //当前装备的攻击力 高于 已经穿戴装备
                if (item.GetProperValue(E_ItemValue.BuyMoney) < dataItem.ItemData.GetProperValue(E_ItemValue.BuyMoney))
                {
                    return true;
                }
            }
            return false;
        }

        public static void OnCloseClick(this UIKnapsackNewComponent self)
        {
            self.SetPlaneVisible(false);
            self.ReleaseAllDetachedGridVisuals();
            Game.EventCenter.RemoveEvent<long>(EventTypeId.RemoveKnapsack, self.RemoveKnapsack);
            self.ClearPackage();
            self.ClearVip();
            self.ClearEquip();
            self.ClearWareHouse();
            self.ClearSell();
            self.ClearStallUp();
            self.ClearIntroduction();
            self.CleanStallUpOther();

            UIComponent.Instance.Remove(UIType.UIKnapsackNew);
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="data">物品数据</param>
        /// <param name="isAddEquip">是否是 店家到装备栏</param>
        /// <param name="type">添加到 的类型</param>
        public static void AddItem(this UIKnapsackNewComponent self, KnapsackDataItem data, bool isAddEquip = false, E_Grid_Type type = E_Grid_Type.Knapsack)
        {
            data.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
            if (item_Info == null || item_Info.Id == 0)
            {
                Log.DebugRed($"配置表不存在-》{data.ConfigId} 的物品");
                return;
            }

            if (string.IsNullOrEmpty(item_Info.ResName))
            {
                item_Info.ResName = "Weapon_borenjian";
            }
            data.item_Info = item_Info;
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
                self.WareEquipItem(kdata.EquipmentPart, kdata);
            }
            else
            {
                kdata.Grid_Type = type;

                self.AddKnapsackItem(kdata);
            }
        }

        public static void ChangeKnapsackItemCount(this UIKnapsackNewComponent self,long itemUUid, int count)
        {
            if (self.curKnapsackState == E_KnapsackState.KS_Gem_Merge) return;
            self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, E_Grid_Type.Knapsack);
            bool isBreak = false;
            for (int i = 0; i < UIKnapsackNewComponent.LENGTH_Knapsack_Y; i++)
            {
                for (int j = 0; j < UIKnapsackNewComponent.LENGTH_Knapsack_X; j++)
                {
                    KnapsackNewGrid grid = self.grids[j][i];
                    if (grid.Data.UUID == itemUUid && grid.IsOccupy)
                    {
                        // RemoveItem(grid.Data, true);
                        grid.SetNum(count.ToString());
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
        /// 穿戴装备
        /// </summary>
        /// <param name="part">装备部位</param>
        /// <param name="dataItem">装备数据</param>
        /// <param name="obj">装备obj</param>
        /// <param name="isInit">是否是初始化装备</param>
        public static void WareEquipItem(this UIKnapsackNewComponent self, int part, KnapsackGridData dataItem)
        {
            if (self.EquipmentComponent == null)
                return;
            //Log.DebugBrown("穿戴装备部位" + part);
            //Log.DebugBrown("穿戴装备数据" + dataItem.EquipmentPart);
            if (part == 101)
            {
                return;
            }
            KnapsackNewGrid grid = self.EquipmentPartDic[(E_Grid_Type)part];
            grid.Data.EquipmentPart = part;
            grid.Data.Point1 = new Vector2Int(dataItem.Point1.x, dataItem.Point1.y);
            grid.Data.Point2 = new Vector2Int(dataItem.Point2.x, dataItem.Point2.y);
            grid.Data.UUID = dataItem.UUID;
            grid.IsOccupy = true;
            grid.Data.ItemData = dataItem.ItemData;
            if (dataItem.ItemData.GetProperValue(E_ItemValue.Level) != 0)
            {
                grid.Image.transform.Find("lev").GetComponent<UnityEngine.UI.Text>().text = dataItem.ItemData.GetProperValue(E_ItemValue.Level) != 0 ? $"{dataItem.ItemData.GetProperValue(E_ItemValue.Level)}" : string.Empty;
                grid.Image.transform.Find("lev").gameObject.SetActive(true);
            }

            if (dataItem.ItemData.GetProperValue(E_ItemValue.OptLevel) != 0)
            {
                grid.Image.transform.Find("append").GetComponent<UnityEngine.UI.Text>().text = dataItem.ItemData.GetProperValue(E_ItemValue.OptLevel) != 0 ? $"{dataItem.ItemData.GetProperValue(E_ItemValue.OptLevel)}" : string.Empty;
                grid.Image.transform.Find("append").gameObject.SetActive(true);
            }

            self.GetGridDisplayBounds(grid, grid, out Vector3 equipMinBounds, out Vector3 equipMaxBounds);
            grid.Load((GameObject obj) =>
            {
                self.FitItemToGrid(
                    obj,
                    grid.Image.transform.position,
                    self.GetGridDisplayRect(grid),
                    equipMinBounds,
                    equipMaxBounds);
                self.pendingVisualRefreshFrames = Mathf.Max(self.pendingVisualRefreshFrames, 4);
            }, grid.Image.transform).Coroutine();

        }

        public static Vector3 GetCenterPos(this UIKnapsackNewComponent self, KnapsackNewGrid startGrid, KnapsackNewGrid endGrid)
        {
            if (startGrid?.Image == null)
            {
                return Vector3.zero;
            }

            if (endGrid?.Image == null || ReferenceEquals(startGrid, endGrid))
            {
                Vector3 singlePos = startGrid.Image.transform.position;
                singlePos.z = 90;
                return singlePos;
            }

            Transform start = startGrid.Image.transform;
            Transform end = endGrid.Image.transform;
            Vector3 delta = end.position - start.position;
            float halfDistance = Vector3.Distance(start.position, end.position) / 2f;
            Vector3 result = start.position + (delta.normalized * halfDistance);
            result.z = 90;
            return result;
        }

        public static Vector3 GetCenterPos(this UIKnapsackNewComponent self,int startX, int startY, int endX, int endY,bool isS=true)
        {
            try
            {
                KnapsackNewGrid startGrid = self.grids[startX][startY];
                KnapsackNewGrid endGrid = self.grids[endX][endY];

                if (!isS && startGrid.Size.x !=0)
                {
                    Log.Info($"startX={startX}  startY={startY}   endX={endX}  endY={endY}");
                    Vector3 p = new Vector3(Math.Abs(endX - startX) * startGrid.Size.x / 2, (Math.Abs(endY - startY) + 0.5f) * startGrid.Size.y / 2 * -1, -100);
                    Log.Info(p.ToString());
                    return p;
                }
                else
                {
                    return self.GetCenterPos(startGrid, endGrid);
                }
            }
            catch (Exception e)
            {
                Log.Error($"startX:{startX}  startY:{startY}  endX:{endX}  endy:{endY}  \n {e}");
            }
            return Vector3.zero;
        }

        public static Vector2 GetGridDisplayRect(this UIKnapsackNewComponent self, KnapsackNewGrid startGrid, KnapsackNewGrid endGrid = null)
        {
            RectTransform startRect = startGrid?.Image?.GetComponent<RectTransform>();
            if (startRect == null)
            {
                return new Vector2(60f, 60f);
            }

            if (endGrid == null || ReferenceEquals(startGrid, endGrid))
            {
                return startRect.rect.size;
            }

            RectTransform endRect = endGrid.Image.GetComponent<RectTransform>();
            float width = Mathf.Abs(endRect.anchoredPosition.x - startRect.anchoredPosition.x) + startRect.rect.width;
            float height = Mathf.Abs(endRect.anchoredPosition.y - startRect.anchoredPosition.y) + startRect.rect.height;
            return new Vector2(width, height);
        }

        public static void GetGridDisplayBounds(this UIKnapsackNewComponent self, KnapsackNewGrid startGrid, KnapsackNewGrid endGrid, out Vector3 minBounds, out Vector3 maxBounds)
        {
            minBounds = Vector3.zero;
            maxBounds = Vector3.zero;

            RectTransform startRect = startGrid?.Image?.GetComponent<RectTransform>();
            if (startRect == null)
            {
                return;
            }

            RectTransform endRect = endGrid?.Image?.GetComponent<RectTransform>() ?? startRect;
            Vector3[] startCorners = new Vector3[4];
            Vector3[] endCorners = new Vector3[4];
            startRect.GetWorldCorners(startCorners);
            endRect.GetWorldCorners(endCorners);

            float minX = Mathf.Min(startCorners[0].x, endCorners[0].x);
            float minY = Mathf.Min(startCorners[0].y, endCorners[0].y);
            float maxX = Mathf.Max(startCorners[2].x, endCorners[2].x);
            float maxY = Mathf.Max(startCorners[2].y, endCorners[2].y);

            minBounds = new Vector3(minX, minY, startCorners[0].z);
            maxBounds = new Vector3(maxX, maxY, startCorners[2].z);
        }

        public static void RefreshDisplayedItems(this UIKnapsackNewComponent self)
        {
            HashSet<int> refreshed = new HashSet<int>();

            self.RefreshGridCollection(self.BackGrids, refreshed);
            self.RefreshGridCollection(self.WareHouseGrids, refreshed);
            self.RefreshGridCollection(self.NpcShopGrids, refreshed);
            self.RefreshGridCollection(self.StallUpGrids, refreshed);
            self.RefreshGridCollection(self.StallUp_OtherGrids, refreshed);

            if (self.EquipmentPartDic == null)
            {
                return;
            }

            foreach (KnapsackNewGrid grid in self.EquipmentPartDic.Values)
            {
                if (grid == null || grid.GridObj == null || grid.Data == null || grid.Data.UUID == 0)
                {
                    continue;
                }

                int instanceId = grid.GridObj.GetInstanceID();
                if (!refreshed.Add(instanceId))
                {
                    continue;
                }

                self.GetGridDisplayBounds(grid, grid, out Vector3 equipMinBounds, out Vector3 equipMaxBounds);
                self.FitItemToGrid(
                    grid.GridObj,
                    grid.Image.transform.position,
                    self.GetGridDisplayRect(grid),
                    equipMinBounds,
                    equipMaxBounds);
            }
        }

        public static void RefreshGridCollection(this UIKnapsackNewComponent self, KnapsackNewGrid[][] sourceGrids, HashSet<int> refreshed)
        {
            if (sourceGrids == null)
            {
                return;
            }

            for (int x = 0; x < sourceGrids.Length; x++)
            {
                KnapsackNewGrid[] column = sourceGrids[x];
                if (column == null)
                {
                    continue;
                }

                for (int y = 0; y < column.Length; y++)
                {
                    KnapsackNewGrid grid = column[y];
                    if (grid == null || grid.GridObj == null || grid.Data == null || grid.Data.UUID == 0)
                    {
                        continue;
                    }

                    if (grid.Data.Point1.x != x || grid.Data.Point1.y != y)
                    {
                        continue;
                    }

                    int instanceId = grid.GridObj.GetInstanceID();
                    if (!refreshed.Add(instanceId))
                    {
                        continue;
                    }

                    try
                    {
                        KnapsackNewGrid endGrid = sourceGrids[grid.Data.Point2.x][grid.Data.Point2.y];
                        self.GetGridDisplayBounds(grid, endGrid, out Vector3 minBounds, out Vector3 maxBounds);

                        Vector3 targetPos = grid.Data.IsSinglePoint
                            ? grid.Image.transform.position
                            : self.GetCenterPos(grid, endGrid);
                        Vector2 targetRect = grid.Data.IsSinglePoint
                            ? self.GetGridDisplayRect(grid)
                            : self.GetGridDisplayRect(grid, endGrid);

                        self.FitItemToGrid(
                            grid.GridObj,
                            targetPos,
                            targetRect,
                            minBounds,
                            maxBounds);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"#KnapsackRefreshGrid# name={grid.GridObj?.name} uuid={grid.Data?.UUID ?? 0} ex={e}");
                    }
                }
            }
        }

        public static bool TryGetMeshSizeResult(this UIKnapsackNewComponent self, GameObject obj, int layer, out MeshSize.Result result)
        {
            result = default;
            if (obj == null)
            {
                return false;
            }

            try
            {
                result = MeshSize.GetMeshSize(obj.transform, layer);
                return !(result.BoundsMin == Vector3.zero && result.BoundsMax == Vector3.zero);
            }
            catch (Exception e)
            {
                Log.Error($"#KnapsackMeshSize# name={obj.name} layer={layer} ex={e}");
                return false;
            }
        }

        public static bool TryGetVisibleBounds(this UIKnapsackNewComponent self, GameObject obj, out Vector3 minBounds, out Vector3 maxBounds, out Vector3 centerPos)
        {
            minBounds = Vector3.zero;
            maxBounds = Vector3.zero;
            centerPos = Vector3.zero;

            if (obj == null)
            {
                return false;
            }

            int uiLayer = LayerMask.NameToLayer(LayerNames.UI);
            if (!self.TryGetMeshSizeResult(obj, uiLayer, out MeshSize.Result result))
            {
                return false;
            }

            minBounds = result.BoundsMin;
            maxBounds = result.BoundsMax;
            centerPos = result.CenterPos;
            return true;
        }

        public static bool TryProjectScreenBounds(this UIKnapsackNewComponent self, Vector3 minBounds, Vector3 maxBounds, out Vector2 minScreen, out Vector2 maxScreen, out Vector2 centerScreen)
        {
            minScreen = Vector2.zero;
            maxScreen = Vector2.zero;
            centerScreen = Vector2.zero;

            Camera uiCamera = CameraComponent.Instance.UICamera;
            if (uiCamera == null)
            {
                return false;
            }

            Vector3[] corners = new Vector3[8]
            {
                new Vector3(minBounds.x, minBounds.y, minBounds.z),
                new Vector3(minBounds.x, minBounds.y, maxBounds.z),
                new Vector3(minBounds.x, maxBounds.y, minBounds.z),
                new Vector3(minBounds.x, maxBounds.y, maxBounds.z),
                new Vector3(maxBounds.x, minBounds.y, minBounds.z),
                new Vector3(maxBounds.x, minBounds.y, maxBounds.z),
                new Vector3(maxBounds.x, maxBounds.y, minBounds.z),
                new Vector3(maxBounds.x, maxBounds.y, maxBounds.z)
            };

            bool hasPoint = false;
            foreach (Vector3 corner in corners)
            {
                Vector3 screenPoint = uiCamera.WorldToScreenPoint(corner);
                if (screenPoint.z <= 0f)
                {
                    continue;
                }

                if (!hasPoint)
                {
                    minScreen = screenPoint;
                    maxScreen = screenPoint;
                    hasPoint = true;
                    continue;
                }

                if (screenPoint.x < minScreen.x) minScreen.x = screenPoint.x;
                if (screenPoint.y < minScreen.y) minScreen.y = screenPoint.y;
                if (screenPoint.x > maxScreen.x) maxScreen.x = screenPoint.x;
                if (screenPoint.y > maxScreen.y) maxScreen.y = screenPoint.y;
            }

            if (!hasPoint)
            {
                return false;
            }

            centerScreen = (minScreen + maxScreen) / 2f;
            return true;
        }

        public static bool TryGetVisibleScreenBounds(this UIKnapsackNewComponent self, GameObject obj, out Vector2 minScreen, out Vector2 maxScreen, out Vector2 centerScreen)
        {
            minScreen = Vector2.zero;
            maxScreen = Vector2.zero;
            centerScreen = Vector2.zero;

            return self.TryGetVisibleBounds(obj, out Vector3 minBounds, out Vector3 maxBounds, out _)
                && self.TryProjectScreenBounds(minBounds, maxBounds, out minScreen, out maxScreen, out centerScreen);
        }

        public static void ApplyScreenDelta(this UIKnapsackNewComponent self, GameObject obj, Vector2 deltaScreen)
        {
            if (obj == null || deltaScreen.sqrMagnitude <= 0.01f)
            {
                return;
            }

            Camera uiCamera = CameraComponent.Instance.UICamera;
            if (uiCamera == null)
            {
                return;
            }

            Vector3 originScreen = uiCamera.WorldToScreenPoint(obj.transform.position);
            Vector3 originWorld = uiCamera.ScreenToWorldPoint(originScreen);
            Vector3 targetWorld = uiCamera.ScreenToWorldPoint(new Vector3(originScreen.x + deltaScreen.x, originScreen.y + deltaScreen.y, originScreen.z));
            Vector3 worldOffset = targetWorld - originWorld;
            worldOffset.z = 0f;
            obj.transform.position += worldOffset;
        }

        public static void HidePreviewEffects(this UIKnapsackNewComponent self, GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            Transform[] transforms = obj.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in transforms)
            {
                if (child == null || child == obj.transform)
                {
                    continue;
                }

                string childName = child.name;
                string childNameLower = childName.ToLowerInvariant();
                bool isVisualEffect = child.GetComponent<ParticleSystem>() != null
                    || child.GetComponent<TrailRenderer>() != null
                    || child.GetComponent<LineRenderer>() != null
                    || childName.IndexOf("Particle System", StringComparison.OrdinalIgnoreCase) >= 0;

                if (isVisualEffect
                    || childName.StartsWith("Stage_", StringComparison.OrdinalIgnoreCase)
                    || childName.StartsWith("Angle_", StringComparison.OrdinalIgnoreCase)
                    || childName.StartsWith("Hand_", StringComparison.OrdinalIgnoreCase)
                    || childName.Equals("UIUnitEnityTopItem", StringComparison.OrdinalIgnoreCase)
                    || childName.Equals("Guang_Head", StringComparison.OrdinalIgnoreCase)
                    || childName.Equals("CaiDai", StringComparison.OrdinalIgnoreCase)
                    || childName.Equals("huo", StringComparison.OrdinalIgnoreCase)
                    || childNameLower.Contains("bangding")
                    || childNameLower.Contains("chuanshuozhizhang_xingxing")
                    || childNameLower.Contains("glow"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        private static float GetPreviewScaleMultiplier(GameObject obj)
        {
            if (obj == null)
            {
                return 1f;
            }

            string objName = obj.name ?? string.Empty;
            if (objName.IndexOf("Wing", StringComparison.OrdinalIgnoreCase) >= 0
                || objName.IndexOf("Flag", StringComparison.OrdinalIgnoreCase) >= 0
                || objName.IndexOf("Guard", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 0.8f;
            }

            if (objName.IndexOf("Weapon", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 0.9f;
            }

            return 0.95f;
        }

        public static void FitItemToGrid(this UIKnapsackNewComponent self, GameObject obj, Vector3 targetPos, Vector2 targetRect, Vector3 minBounds, Vector3 maxBounds, int z = 85, float scalePadding = 0.9f)
        {
            if (obj == null)
            {
                return;
            }

            self.HidePreviewEffects(obj);
            obj.transform.SetParent(null);
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            Vector3 targetCenter = targetPos;
            if (minBounds != Vector3.zero || maxBounds != Vector3.zero)
            {
                targetCenter = (minBounds + maxBounds) * 0.5f;
            }

            targetCenter.z = z;
            obj.transform.position = targetCenter;

            float previewScale = GetPreviewScaleMultiplier(obj);
            if (self.TryGetVisibleBounds(obj, out Vector3 itemMinBounds, out Vector3 itemMaxBounds, out Vector3 itemCenter))
            {
                float itemWidth = Mathf.Max(0.001f, itemMaxBounds.x - itemMinBounds.x);
                float itemHeight = Mathf.Max(0.001f, itemMaxBounds.y - itemMinBounds.y);
                float targetWidth = Mathf.Max(0.001f, maxBounds.x - minBounds.x);
                float targetHeight = Mathf.Max(0.001f, maxBounds.y - minBounds.y);
                float fitScale = Mathf.Min(targetWidth / itemWidth, targetHeight / itemHeight) * scalePadding;

                if (!float.IsNaN(fitScale) && !float.IsInfinity(fitScale) && fitScale > 0.001f)
                {
                    obj.transform.localScale = Vector3.one * Mathf.Min(fitScale, 1f) * previewScale;

                    if (self.TryGetVisibleBounds(obj, out itemMinBounds, out itemMaxBounds, out itemCenter))
                    {
                        Vector3 offset = targetCenter - itemCenter;
                        offset.z = 0f;
                        obj.transform.position += offset;
                    }
                }
                else
                {
                    obj.transform.localScale = Vector3.one * previewScale;
                }
            }
            else
            {
                obj.transform.localScale = Vector3.one * previewScale;
            }

            Vector3 finalPos = obj.transform.position;
            finalPos.z = z;
            obj.transform.position = finalPos;
            obj.SetActive(self.plane == null || self.plane.activeInHierarchy);
        }

        /// <summary>
        /// 放入背包
        /// </summary>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        /// <param name="IsOverridePro">是否 覆盖属性</param>
        public static void AddKnapsackItem(this UIKnapsackNewComponent self, KnapsackGridData data, bool IsOverridePro = false)
        {
            if (self.curKnapsackState == E_KnapsackState.KS_Gem_Merge) return;
            self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, data.Grid_Type);

            if (data == null || data.ItemData == null)
            {
                Log.Error("#KnapsackAddItem# invalid data");
                return;
            }

            if (self.grids == null)
            {
                Log.Error($"#KnapsackAddItem# grids null type={data.Grid_Type} uuid={data.UUID} config={data.ItemData.ConfigId}");
                return;
            }

            data.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
            if (item_Info == null || item_Info.Id == 0)
            {
                Log.Error($"#KnapsackAddItem# item info missing uuid={data.UUID} config={data.ItemData.ConfigId}");
                return;
            }
            data.ItemData.item_Info = item_Info;

            if (data.Point1.x < 0 || data.Point1.y < 0 || data.Point2.x < 0 || data.Point2.y < 0
                || data.Point1.x >= self.LENGTH_X || data.Point2.x >= self.LENGTH_X
                || data.Point1.y >= self.LENGTH_Y || data.Point2.y >= self.LENGTH_Y)
            {
                Log.Error($"#KnapsackAddItem# out of range uuid={data.UUID} config={data.ItemData.ConfigId} point1=({data.Point1.x},{data.Point1.y}) point2=({data.Point2.x},{data.Point2.y}) size=({self.LENGTH_X},{self.LENGTH_Y})");
                return;
            }

            if (data.IsSinglePoint)
            {
                KnapsackNewGrid grid = self.grids[data.Point1.x][data.Point1.y];
                if (grid == null)
                {
                    Log.Error($"#KnapsackAddItem# start grid null uuid={data.UUID} config={data.ItemData.ConfigId} point=({data.Point1.x},{data.Point1.y})");
                    return;
                }

                grid.Data = data;
                //grid.Data.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                grid.Data.ItemData = data.ItemData;
                grid.Data.ItemData.item_Info = item_Info;
                grid.Data.UUID = data.UUID;
                grid.Data.SetSinglePoint(new Vector2Int(data.Point1.x, data.Point1.y));
                grid.Data.EquipmentPart = data.EquipmentPart != item_Info.Slot ? item_Info.Slot : data.EquipmentPart;

                self.GetGridDisplayBounds(grid, grid, out Vector3 singleMinBounds, out Vector3 singleMaxBounds);
                grid.Load((GameObject go) =>
                {
                    go.SetUI(grid.Data.ItemData.GetProperValue(E_ItemValue.Level));
                    self.FitItemToGrid(
                        go,
                        grid.Image.transform.position,
                        self.GetGridDisplayRect(grid),
                        singleMinBounds,
                        singleMaxBounds);
                    self.pendingVisualRefreshFrames = Mathf.Max(self.pendingVisualRefreshFrames, 4);
                }, grid.Image.transform).Coroutine();
                grid.IsOccupy = true;
                grid.ResetNum();
                if (data.Grid_Type == E_Grid_Type.Knapsack && self.CheckItemlev(data))
                {
                    Transform recommend = grid.Image?.transform?.Find("up");
                    if (recommend != null)
                    {
                        recommend.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                KnapsackNewGrid grid = self.grids[data.Point1.x][data.Point1.y];
                if (grid == null)
                {
                    Log.Error($"#KnapsackAddItem# start grid null uuid={data.UUID} config={data.ItemData.ConfigId} point=({data.Point1.x},{data.Point1.y})");
                    return;
                }

                List<KnapsackNewGrid> gris = self.GetAreaGrids(data.Point1.x, data.Point1.y, data.Point2.x, data.Point2.y, self.LENGTH_X, self.LENGTH_Y);
                if (gris == null || gris.Count == 0)
                {
                    Log.Error($"#KnapsackAddItem# area grids empty uuid={data.UUID} config={data.ItemData.ConfigId} point1=({data.Point1.x},{data.Point1.y}) point2=({data.Point2.x},{data.Point2.y})");
                    return;
                }

                foreach (var item in gris)
                {
                    if (item == null)
                    {
                        Log.Error($"#KnapsackAddItem# area grid null uuid={data.UUID} config={data.ItemData.ConfigId}");
                        return;
                    }

                    item.Data.EquipmentPart = data.EquipmentPart != data.ItemData.item_Info.Slot ? data.ItemData.item_Info.Slot : data.EquipmentPart;
                    item.Data.Point1 = new Vector2Int(data.Point1.x, data.Point1.y);
                    item.Data.Point2 = new Vector2Int(data.Point2.x, data.Point2.y);
                    item.Data.UUID = data.UUID;
                    item.Data.ItemData = data.ItemData;
                    item.IsOccupy = true;
                }

                KnapsackNewGrid endGrid = self.grids[data.Point2.x][data.Point2.y];
                if (endGrid == null)
                {
                    Log.Error($"#KnapsackAddItem# end grid null uuid={data.UUID} config={data.ItemData.ConfigId} point=({data.Point2.x},{data.Point2.y})");
                    return;
                }

                self.GetGridDisplayBounds(grid, endGrid, out Vector3 areaMinBounds, out Vector3 areaMaxBounds);
                grid.Load((GameObject go) =>
                {
                    Log.Info("============ " + go.name);
                    go.SetUI(grid.Data.ItemData.GetProperValue(E_ItemValue.Level));
                    self.FitItemToGrid(
                        go,
                        self.GetCenterPos(grid, endGrid),
                        self.GetGridDisplayRect(grid, endGrid),
                        areaMinBounds,
                        areaMaxBounds);
                    foreach (var item in gris)
                    {
                        item.GridObj = go;
                    }
                    self.pendingVisualRefreshFrames = Mathf.Max(self.pendingVisualRefreshFrames, 4);
                }, grid.Image.transform).Coroutine();

                if (data.Grid_Type == E_Grid_Type.Knapsack && self.CheckItemlev(data))
                {
                    Transform recommend = self.grids[data.Point1.x][data.Point1.y].Image?.transform?.Find("up");
                    if (recommend != null)
                    {
                        recommend.gameObject.SetActive(true);
                    }
                }

                //显示物品数量
                if (data.ItemData != null && data.ItemData.GetProperValue(E_ItemValue.Quantity) > 1)
                {
                    self.grids[data.Point2.x][data.Point1.y].ResetNum();
                }
            }

        }

        /// <summary>
        /// 物品回到起始位置
        /// </summary>
        public static void ResetGridObj(this UIKnapsackNewComponent self)
        {
            if (self.curDropObj == null) return;
            self.curDropObj.transform.localPosition = self.originObjPos;
            self.curDropObj.transform.localRotation = self.originObjRotation;
            self.isDroping = false;
            self.curDropObj = null;
        }

        public static void ClearDropObj(this UIKnapsackNewComponent self)
        {
            if (self.curDropObj)
            {
                GameObject.Destroy(self.curDropObj);
                self.curDropObj = null;
            }
        }

        /// <summary>
        /// 判断格子是否已经有物品
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public static bool ContainGridObj(this UIKnapsackNewComponent self, int startX, int startY, int endX, int endY)
        {
            List<KnapsackNewGrid> results = self.GetAreaGrids(startX, startY, endX, endY, self.LENGTH_X, self.LENGTH_Y);
            foreach (var item in results)
            {
                if (item.IsOccupy && self.originArea.UUID != item.Data.UUID) return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startX">起始——x</param>
        /// <param name="startY">起始-y</param>
        /// <param name="endX">结束-x</param>
        /// <param name="endY">结束-y</param>
        /// <param name="count_x">列数</param>
        /// <param name="count_y">行数</param>
        /// <returns></returns>
        public static List<KnapsackNewGrid> GetAreaGrids(this UIKnapsackNewComponent self, int startX, int startY, int endX, int endY, int count_x, int count_y)
        {
            List<KnapsackNewGrid> results = new List<KnapsackNewGrid>();
            if (startX >= count_x || endX >= count_x || startY >= count_y || endY >= count_y ||
                startX < 0 || endX < 0 || startY < 0 || endY < 0)
            {
                return results;
            }
            //横向查询
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    try
                    {

                        results.Add(self.grids[i][j]);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"i:{i}  j:{j}  count_x:{count_x} count_y:{count_y} : {e}");
                        return new List<KnapsackNewGrid>();
                    }
                }
            }

            return results;
        }

        public static KnapsackDataItem  CreateStallUpData(this UIKnapsackNewComponent self,long itemuuid)
        {
            KnapsackDataItem item = new KnapsackDataItem();
            item.UUID = itemuuid;
            
            return item;
        }

        //添加摊位上的物品
        public static KnapsackDataItem AddStallUpItem(this UIKnapsackNewComponent self)
        {
            //KnapsackDataItem knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(itemUUID);
            // knapsackDataItem.UUID = itemUUID;
            // knapsackDataItem.ConfigId = StallUpData.ConfigId;
            // knapsackDataItem.PosInBackpackX = StallUpData.PosInBackpackX;
            // knapsackDataItem.PosInBackpackY = StallUpData.PosInBackpackY;
            // knapsackDataItem.X = StallUpData.X;
            // knapsackDataItem.Y = StallUpData.Y;
             //knapsackDataItem.SetProperValue(E_ItemValue.Stall_SellPrice, item.Price);
          //   knapsackDataItem.SetProperValue(E_ItemValue.Stall_SellMoJingPrice, item.Price2);
            //Log.Info("AddStallUpItem -- " + self.StallUpData.ConfigId);
            if (self.StallUpData == null) return null;
            self.AddItem(self.StallUpData, type: E_Grid_Type.Stallup);
            self.stallUpComponent.StallUpItemDic[self.StallUpData.Id] = self.StallUpData;
            return self.StallUpData;
        }



        /// <summary>
        /// 金币显示转换
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string FormatNum(this UIKnapsackNewComponent self,int num)
        {
            if (num >= 100000000)//亿
            {
                return string.Format("{0:F0}亿", num / 100000000);
            }
            else if (num >= 10000000)//千万
            {
                return string.Format("{0:F0}千万", num / 10000000);
            }
            else if (num >= 1000000)//百万
            {
                return string.Format("{0:F0}百万", num / 1000000);
            }
            else if (num >= 100000)//十万
            {
                return string.Format("{0:F0}万", num / 10000);
            }
            else if (num >= 10000)//万
            {
                return string.Format("{0:F0}万", num / 10000);
            }
            else if (num >= 1000)//千
            {
                return string.Format("{0:F0}千", num / 1000);
            }
            else if (num >= 100)//百
            {
                return string.Format("{0:F0}百", num / 100);
            }

            return num.ToString();
        }

        /// <summary>
        /// 显示模型
        /// </summary>
        /// <param name="resName">模型资源名</param>
        /// <param name="pos">位置</param>
        /// <param name="lev">等级</param>
        public static GameObject ShowMode(this UIKnapsackNewComponent self, string resName, Transform pos, int z = 80, int lev = 1)
        {
            GameObject obj = ResourcesComponent.Instance.LoadGameObject(resName.StringToAB(), resName);
            if (obj == null) return null;
            obj.SetUI(lev);
            Vector3 vector = pos.position;
            RectTransform rect = pos.GetComponent<RectTransform>();
            MeshSize.Result result = MeshSize.GetMeshSize(obj.transform, LayerMask.NameToLayer(LayerNames.UI));
            float scale = result.GetScreenScaleFactor(new Vector2(rect.rect.width, rect.rect.height) * 1.2f); // 让装备比ui卡槽大一点
            if (scale > 1f) scale = 1f; // 只缩小，不放大
            vector.z = z;
            obj.transform.position = vector;
            obj.transform.localScale *= scale;
            obj.SetActive(true);
            obj.transform.SetParent(pos, true);
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                if (obj.transform.GetChild(i).name == "UIUnitEnityTopItem")
                {
                    obj.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            return obj;
        }
    }

    public class KnapsackNewGrid : KnapsackGrid
    {
        public UnityEngine.UI.Text Num { get; set; }

        public Vector2 Size;

        public async ETTask Load(Action<GameObject> action,Transform parent)
        {
            int itemId = (int)Data.ItemData.item_Info.Id;
            //await TimerComponent.Instance.WaitAsync(Data.Point1.x + Data.Point1.y);
            await ResourcesComponent.Instance.LoadGameObjectAsync(Data.ItemData.item_Info.ResName.StringToAB(), Data.ItemData.item_Info.ResName);
            int index = 0;
            while (AssetBundleComponent.Instance.GetAsset(Data.ItemData.item_Info.ResName.StringToAB().ToLower(), Data.ItemData.item_Info.ResName) == null)
            {
                await TimerComponent.Instance.WaitAsync(10);
                index++;
                if (index >= 40)
                {
                    return;
                }

                if (itemId != Data.ItemData.item_Info.Id)
                {
                    Log.Debug("已经移除了");
                    return;
                }

                if (Data.UUID == 0)
                {
                    Log.Debug("物品已经清理了");
                    return;
                }

                if (this.Image && this.Image.IsDestroyed())
                {
                    Log.Debug("页面已经销毁");
                    return;
                }
            }

            if (Data.UUID == 0 || Data.ItemData == null || Data.ItemData.item_Info == null)
            {
                return;
            }

            if (this.Image && this.Image.IsDestroyed())
            {
                Log.Debug("页面已经销毁");
                return;
            }

            //await TimerComponent.Instance.WaitAsync(Data.Point1.x + Data.Point1.y);
            GameObject go = ResourcesComponent.Instance.LoadGameObject(Data.ItemData.item_Info.ResName.StringToAB(), Data.ItemData.item_Info.ResName);
            if (go == null)
            {
                go = ResourcesComponent.Instance.LoadGameObject("Weapon_borenjian".StringToAB(), "Weapon_borenjian");
            }

            if (go == null)
            {
                return;
            }

            GridObj = go;
            go.SetUI();
            go.transform.SetParent(null);
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            if (Data.UUID == 0 || (this.Image && this.Image.IsDestroyed()))
            {
                GameObject.Destroy(go);
                GridObj = null;
                return;
            }

            action.Invoke(go);
        }

        public void ResetNum()
        {
            if (Num)
            {
                if (Data.ItemData!=null)
                {
                    Num.text = Data.ItemData.GetProperValue(E_ItemValue.Quantity).ToString();
                }
                else
                {
                    Num.text = "";
                }
            }
        }

        public void SetNum(string v)
        {
            if (Num)
            {
               Num.text = v;
            }
        }
        public void Clear()
        {
            Log.Info("移除物品--- " + Data.ItemData.name);

            IsOccupy = false;
            Transform lev = Image.transform.Find("lev");
            if(lev != null)
            {
                lev.GetComponent<Text>().text = string.Empty;
                lev.gameObject.SetActive(false);
            }

            Transform append = Image.transform.Find("append");
            if(append != null)
            {
                append.GetComponent<Text>().text = string.Empty;
                append.gameObject.SetActive(false);
            }
           
            Data.UUID = 0;
            Data.EquipmentPart = 0;
            Data.ItemData = null;

            //ResetNum();
            SetNum("");
            if (GridObj)
            {
                GameObject.Destroy(GridObj);
            }
            GridObj = null;
        }
    }

}
