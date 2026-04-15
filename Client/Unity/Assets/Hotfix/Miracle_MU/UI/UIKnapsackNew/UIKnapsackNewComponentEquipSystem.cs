using Aliyun.OSS;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

namespace ETHotfix
{

    [ObjectSystem]
    public class UIKnapsackNewComponentUpdata : UpdateSystem<UIKnapsackNewComponent>
    {
        public override void Update(UIKnapsackNewComponent self)
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

            if (!self.isDroping && self.pendingVisualRefreshFrames > 0)
            {
                self.RefreshDisplayedItems();
                self.pendingVisualRefreshFrames--;
            }

            //if (self.useItem != null && self.StartUse)
            //{
            //    //  Log.DebugBrown($"物品使用间隔时间：{self.userTime - Time.time} {self.dianjiImage.activeSelf}");
            //    if (Time.time >= self.userTime && self.IsUse == false)
            //    {
            //        self.IsUse = true;
            //        self.PlayerUserItemInTheBackpack(self.useItem.ItemData.Id).Coroutine();
            //    }
            //    else
            //    {

            //        // self.dianjiImage.SetActive(true);
            //        //self.filledImage.fillAmount = (self.userItemTime - (self.userTime - Time.time)) / self.userItemTime;

            //    }
            //}
        }

    }

    public static class UIKnapsackNewComponentEquipSystem
    {
        public static async ETTask InitEquip(this UIKnapsackNewComponent self)
        {

            string res = "EquipPlane";
            //加载对应面板
            await ResourcesComponent.Instance.LoadGameObjectAsync(res.StringToAB(), res);
            //实例化面板 
            GameObject equip = ResourcesComponent.Instance.LoadGameObject(res.StringToAB(), res);
            equip.transform.SetParent(self.plane.transform, false);
            equip.transform.localPosition = Vector3.zero;
            equip.transform.localScale = Vector3.one;

            self.EquipmentPartDic = new Dictionary<E_Grid_Type, KnapsackNewGrid>();
            self.equipCollector = equip.GetReferenceCollector();
            Transform equipGridRoot = self.equipCollector.GetGameObject("EquipGrid").transform;
            int length = equipGridRoot.childCount;
            self.coinText = self.equipCollector.GetText("coinTxt");
            self.ruibiText = self.equipCollector.GetText("yuanbaoTxt");
            self.qijibiText = self.equipCollector.GetText("qijibiTxt");
            self.InitCoin();//获取货币的信息

            // Pet(14) and WristBand(15) are disabled in the prefab by default.
            // Bring them back explicitly here and apply class-specific slot visibility by slot id
            // instead of fragile child indexes.
            SetEquipSlotActive(equipGridRoot, E_Grid_Type.Pet, true);
            SetEquipSlotActive(equipGridRoot, E_Grid_Type.WristBand, true);
            // Keep the wristband above the pet slot and share the same center line.
            AdjustEquipSlotLayout(equipGridRoot, E_Grid_Type.WristBand, new Vector2(-100f, 343.40002f), new Vector2(95f, 95f));
            SetEquipSlotActive(equipGridRoot, E_Grid_Type.TianYing, UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Holymentor);
            SetEquipSlotActive(equipGridRoot, E_Grid_Type.HandGuard, UnitEntityComponent.Instance.LocalRole.RoleType != E_RoleType.Gladiator);
            LogEquipSlotState(equipGridRoot, E_Grid_Type.Pet, "Pet");
            LogEquipSlotState(equipGridRoot, E_Grid_Type.WristBand, "WristBand");

            for (int i = 0; i < length; i++)
            {
                GameObject grid = equipGridRoot.GetChild(i).gameObject;
                grid.transform.Find("lev").gameObject.SetActive(false);
                grid.transform.Find("append").gameObject.SetActive(false);

                grid.transform.Find("lev").GetComponent<Text>().text = string.Empty;
                grid.transform.Find("append").GetComponent<Text>().text = string.Empty;
                E_Grid_Type part = (E_Grid_Type)int.Parse(grid.name);
                KnapsackNewGrid knapsackGrid = new KnapsackNewGrid()
                {
                    GridObj = null,
                    Image = grid.GetComponent<Image>(),
                    IsOccupy = false,
                    Grid_Type = part
                };
                self.EquipmentPartDic[part] = knapsackGrid;
                self.RegisterEvent(part, grid);
            }



            self.equipCollector.GetButton("BaiTanBtn").onClick.AddSingleListener(() =>
            {
                self.OnBaiTanClick().Coroutine();
            });

            self.equipCollector.GetButton("VipBtn").onClick.AddSingleListener(() =>
            {
                self.OnVipClick().Coroutine();
            });

         

            self.equipCollector.GetButton("SplitBtn").onClick.AddSingleListener(() =>
            {
                self.OnSplitClik().Coroutine();
            });

            //特权维修
            self.equipCollector.GetButton("RepairBtn").onClick.AddSingleListener(() =>
            {
                self.RepairEquips();
            });

            self.InitEquipGrid();
            //self.equipCollector.GetButton("ClearUpBtn").onClick.AddSingleListener(() =>
            //{
            //    self.FinishingBackpack().Coroutine();
            //});
            //try
            //{

            //    //string res = "EquipPlane";
            //    ////加载对应面板
            //    //await ResourcesComponent.Instance.LoadGameObjectAsync(res.StringToAB(), res);
            //    ////实例化面板 
            //    //GameObject equip = ResourcesComponent.Instance.LoadGameObject(res.StringToAB(), res);
            //    //equip.transform.parent = self.plane.transform;
            //    //equip.transform.localPosition = Vector3.zero;
            //    //equip.transform.localScale = Vector3.one;

            //    //self.EquipmentPartDic = new Dictionary<E_Grid_Type, KnapsackNewGrid>();
            //    //self.equipCollector = equip.GetReferenceCollector();
            //    //int length = self.equipCollector.GetGameObject("EquipGrid").transform.childCount;
            //    //if (UnitEntityComponent.Instance.LocalRole.RoleType != E_RoleType.Holymentor)
            //    //{
            //    //    self.equipCollector.GetGameObject("EquipGrid").transform.GetChild(15).gameObject.SetActive(false);
            //    //}
            //    //for (int i = 0; i < length; i++)
            //    //{
            //    //    GameObject grid = self.equipCollector.GetGameObject("EquipGrid").transform.GetChild(i).gameObject;
            //    //    grid.transform.Find("lev").gameObject.SetActive(false);
            //    //    grid.transform.Find("append").gameObject.SetActive(false);

            //    //    grid.transform.Find("lev").GetComponent<Text>().text = string.Empty;
            //    //    grid.transform.Find("append").GetComponent<Text>().text = string.Empty;
            //    //    E_Grid_Type part = (E_Grid_Type)int.Parse(grid.name);
            //    //    KnapsackNewGrid knapsackGrid = new KnapsackNewGrid()
            //    //    {
            //    //        GridObj = null,
            //    //        Image = grid.GetComponent<Image>(),
            //    //        IsOccupy = false,
            //    //        Grid_Type = part
            //    //    };
            //    //    self.EquipmentPartDic[part] = knapsackGrid;
            //    //    self.RegisterEvent(part, grid);
            //    //}

            //    //self.InitEquipGrid();


            //    //self.equipCollector.GetButton("BaiTanBtn").onClick.AddSingleListener(() =>
            //    //{
            //    //    self.OnBaiTanClick().Coroutine();
            //    //});

            //    //self.equipCollector.GetButton("VipBtn").onClick.AddSingleListener(() =>
            //    //{
            //    //    self.OnVipClick().Coroutine();
            //    //});

            //    //self.coinText = self.equipCollector.GetText("coinTxt");
            //    //self.ruibiText = self.equipCollector.GetText("yuanbaoTxt");
            //    //self.qijibiText = self.equipCollector.GetText("qijibiTxt");
            //    //self.InitCoin();

            //    //self.equipCollector.GetButton("SplitBtn").onClick.AddSingleListener(() =>
            //    //{
            //    //    self.OnSplitClik().Coroutine();
            //    //});

            //    ////特权维修
            //    //self.equipCollector.GetButton("RepairBtn").onClick.AddSingleListener(() =>
            //    //{
            //    //    self.RepairEquips();
            //    //});

            //    ////self.equipCollector.GetButton("ClearUpBtn").onClick.AddSingleListener(() =>
            //    ////{
            //    ////    self.FinishingBackpack().Coroutine();
            //    ////});
            //}
            //catch(Exception e)
            //{
            //    Log.Debug("Error --------------------------");
            //    Log.Debug(e.ToString());
            //}

        }

        private static void SetEquipSlotActive(Transform equipGridRoot, E_Grid_Type slot, bool isActive)
        {
            Transform slotTransform = equipGridRoot.Find(((int)slot).ToString());
            if (slotTransform != null)
            {
                slotTransform.gameObject.SetActive(isActive);
            }
        }

        private static void AdjustEquipSlotLayout(Transform equipGridRoot, E_Grid_Type slot, Vector2 anchoredPosition)
        {
            Transform slotTransform = equipGridRoot.Find(((int)slot).ToString());
            if (slotTransform is RectTransform rectTransform)
            {
                rectTransform.anchoredPosition = anchoredPosition;
            }
        }

        private static void AdjustEquipSlotLayout(Transform equipGridRoot, E_Grid_Type slot, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            Transform slotTransform = equipGridRoot.Find(((int)slot).ToString());
            if (slotTransform is RectTransform rectTransform)
            {
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = sizeDelta;
            }
        }

        private static void LogEquipSlotState(Transform equipGridRoot, E_Grid_Type slot, string tag)
        {
            Transform slotTransform = equipGridRoot.Find(((int)slot).ToString());
            if (slotTransform == null)
            {
                Log.Info($"#EquipSlotVerify# slot={tag} missing");
                return;
            }

            RectTransform rectTransform = slotTransform as RectTransform;
            string position = rectTransform != null
                ? $"{rectTransform.anchoredPosition.x},{rectTransform.anchoredPosition.y}"
                : "n/a";
            Log.Info($"#EquipSlotVerify# slot={tag} activeSelf={slotTransform.gameObject.activeSelf} activeInHierarchy={slotTransform.gameObject.activeInHierarchy} anchored={position}");
        }


        /// <summary>
        /// 初始化 金币、元宝
        /// </summary>
        public static void InitCoin(this UIKnapsackNewComponent self)
        {
           // Log.Info($"金币：{self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin)}");
            self.coinText.text = self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin).ToString();//.ToString("N")
            self.ruibiText.text = self.roleEntity.Property.GetProperValue(E_GameProperty.MoJing).ToString();
           // self.qijibiText.text = self.roleEntity.Property.GetProperValue(E_GameProperty.MiracleCoin).ToString();
            Game.EventCenter.EventListenner<long>(EventTypeId.GLOD_CHANGE, self.ChangeKnapsackCoin);
        }


        public static void ClearEquip(this UIKnapsackNewComponent self)
        {
            self.ReleaseGridDictionaryVisuals(self.EquipmentPartDic);
            if (self.equipCollector)
            {
                GameObject.Destroy(self.equipCollector.gameObject);
                self.equipCollector = null;
            }
        }

        public static void InitEquipGrid(this UIKnapsackNewComponent self)
        {
            
            Log.DebugGreen("InitEq uipGrid==>"+ self.roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic.Count);
            //foreach (var item in self.EquipmentComponent.curWareEquipsData_Dic)
            //{
            //    Debug.Log("打印数据" + item.Key + ":::");
            //}

            foreach (var item in self.roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic)
            {
                self.AddItem(item.Value, true);
            }
        }

        public static void ClearIntroduction(this UIKnapsackNewComponent self)
        {
            if (self.uIIntroduction != null)
            {
                UIComponent.Instance.Remove(UIType.UIIntroduction);
                self.uIIntroduction = null;
            }

        }

        #region 格子事件
        public static void RegisterEvent(this UIKnapsackNewComponent self, E_Grid_Type part, GameObject obj)
        {
            UGUITriggerProxy proxy = obj.GetComponent<UGUITriggerProxy>();
            proxy.OnBeginDragEvent += () => { self.OnBeginDrag(part); };
            proxy.OnEndDragEvent += () => { self.OnEndDrag(part); };
            proxy.OnPointerEnterEvent += () => { self.OnPointerEnter(part); };
            proxy.OnPointerExitEvent += () => { self.OnPointerExit(part); };
            proxy.OnPointerClickEvent += () => { self.OnPointerClickEvent(part); };
        }

        public static void OnBeginDrag(this UIKnapsackNewComponent self, E_Grid_Type part)
        {
            //if (curKnapsackState == E_KnapsackState.KS_Revision) return;

            if (self.IsSplit) return;

            self.ClearIntroduction();


            KnapsackGrid grid = self.EquipmentPartDic[part];
            if (grid.IsOccupy == false) return;

            if (grid.GridObj == null) return;



            self.originArea = grid.Data;
            self.originArea.Grid_Type = part;

            self.curDropObj = grid.GridObj;
            self.originObjPos = grid.GridObj.transform.localPosition;
            self.originObjRotation = grid.GridObj.transform.localRotation;

            self.isDroping = true;
        }

        public static void OnEndDrag(this UIKnapsackNewComponent self, E_Grid_Type part)
        {
            //if (curKnapsackState == E_KnapsackState.KS_Revision) return;

            if (self.isDroping == false) return;

            self.curChooseArea.UUID = self.originArea.UUID;
            Log.DebugWhtie($"拖拽结束：curChooseArea.Grid_Type：{self.curChooseArea.Grid_Type}");
            Log.DebugWhtie($"拖拽结束：originArea.UUID：{self.originArea.UUID}");
            //从装备栏 拖拽到背包
            if (self.curChooseArea.Grid_Type == E_Grid_Type.Knapsack)
            {
                if (self.curChooseArea.Point1.y >= UIKnapsackNewComponent.LENGTH_Knapsack_Y)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "移动失败");
                    self.ResetGridObj();
                    return;
                }
                if (self.ContainGridObj(self.curChooseArea.Point1.x, self.curChooseArea.Point1.y, self.curChooseArea.Point2.x, self.curChooseArea.Point2.y))
                {
                    self.ResetGridObj();

                }
                else
                {
                    //请求 服务器 卸载装备
                    self.UnLoadEquip((int)part, self.curChooseArea.Point1.x, self.curChooseArea.Point1.y).Coroutine();

                    /* curChooseArea.EquipmentPart = (int)part;

                     curChooseArea.ItemData = originArea.ItemData;

                     AddKnapsackItem(curChooseArea, curDropObj);//物品放入背包*/
                    self.RemoveWareEquipItem((int)part, true);//移除装备栏的物品
                    self.isDroping = false;
                    self.curDropObj = null;
                }
            }
            else
            {
                // self.ClearCurChooseArea();
                self.ResetGridObj();
            }
        }

        /// <summary>
        /// 卸载装备
        /// </summary>
        /// <param name="part">装备部位</param>
        /// <param name="isRecycle">是否回收部位 对应的Obj</param>
        public static void RemoveWareEquipItem(this UIKnapsackNewComponent self, int part, bool isRecycle = false)
        {
            KnapsackNewGrid grid = self.EquipmentPartDic[(E_Grid_Type)part];
            grid.Clear();
        }

        /// <summary>
        /// 卸载装备
        /// </summary>
        /// <param name="slotId">装备部位的</param>
        /// <param name="X"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static async ETVoid UnLoadEquip(this UIKnapsackNewComponent self, int slotId, int X, int y)
        {
            G2C_UnloadEquipItemResponse g2c_Unload = (G2C_UnloadEquipItemResponse)await SessionComponent.Instance.Session.Call(new C2G_UnloadEquipItemRequest
            {
                EquipPosition = slotId,
                PosInBackpackX = X,
                PosInBackpackY = y
            });
            if (g2c_Unload.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2c_Unload.Error.GetTipInfo()}");
            }
            else
            {
                //成功
                //会推送G2C_ItemsIntoBackpack_notice更新背包物品 
                //推送实体穿戴装备(周边一定范围玩家也广播) G2C_UnitEquipLoad_notice
            }
        }

        public static void OnPointerEnter(this UIKnapsackNewComponent self, E_Grid_Type part)
        {
            //if (curKnapsackState == E_KnapsackState.KS_Revision && EquipmentPartDic[part].IsOccupy)
            //{
            //    EquipmentPartDic[part].ReadyColor();
            //    return;
            //}

            if (self.IsSplit) return;
            if (!self.isDroping) return;

           // Log.DebugGreen($"当前格子类型{part.EnumToString<E_Grid_Type>()}");
            self.curChooseArea.Grid_Type = part;//当前选择的格子的类型
            self.EquipmentPartDic[part].ReadyColor();
            self.curWarePart = (int)part;
          //  Log.DebugBrown($"进入装备栏 起始格子类型:{self.originArea.Grid_Type}  目标格子类型:{self.curChooseArea.Grid_Type}  ");
        }

        public static void OnPointerExit(this UIKnapsackNewComponent self, E_Grid_Type part)
        {
            //if (curKnapsackState == E_KnapsackState.KS_Revision)
            //{
            //    EquipmentPartDic[part].ResetColor();
            //    return;
            //}
            if (!self.isDroping) return;

            self.ClearCurChooseArea();
        }

        public static void OnPointerClickEvent(this UIKnapsackNewComponent self, E_Grid_Type part)
        {
            if (self.IsSplit) return;
            KnapsackGrid grid = self.EquipmentPartDic[part];
            self.ClearIntroduction();
            if (!grid.IsOccupy)
            {
                return;
            }
            //if (grid.IsOccupy == false)
            //{
            //    if (uIIntroduction != null)
            //    {
            //        UIComponent.Instance.Remove(UIType.UIIntroduction);
            //        uIIntroduction = null;
            //    }
            //    return;
            //}
            KnapsackGridData data = grid.Data;

            if (self.curKnapsackState == E_KnapsackState.KS_Revision)
            {
                //if (Inherit.gameObject.activeSelf == true)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "请脱下装备再继承!");
                //    return;
                //}
                //self.ShowQiangHuaEquipObj(grid.Data, part);
                return;
            }

            Vector3 pos = new Vector3(grid.Image.transform.position.x, grid.Image.transform.position.y, 0);

            //显示 物品属性面板
            KnapsackDataItem dataItem = data.ItemData;

            self.uIIntroduction ??= UIComponent.Instance.VisibleUI(UIType.UIIntroduction).GetComponent<UIIntroductionComponent>();
            self.uIIntroduction.GetDataType(part);
            self.uIIntroduction.GetAllAtrs(dataItem);

            pos -= Vector3.left / 2;
            var screenPos = CameraComponent.Instance.UICamera.WorldToScreenPoint(pos);
            var pivot_x = 0;
            self.uIIntroduction.ChangeStartCorner(Corner.UpperLeft);
            if (screenPos.x > Screen.width - 700)
            {

                pivot_x = 1;
                pos += Vector3.left;
                self.uIIntroduction.ChangeStartCorner();
            }

            self.uIIntroduction.ShowAtrs();
            self.uIIntroduction.SetPos(pos, pivot_x);
        }


        public static void ClearCurChooseArea(this UIKnapsackNewComponent self)
        {
            if (self.curChooseArea.Grid_Type != E_Grid_Type.None)
            {
                if (self.EquipmentPartDic != null && self.EquipmentPartDic.TryGetValue(self.curChooseArea.Grid_Type, out KnapsackNewGrid targetGrid))
                {
                    targetGrid.ResetColor();
                }
            }
            self.curChooseArea.Grid_Type = E_Grid_Type.None;//当前选择的格子类型为None（即没有进入区域格子）
            self.curChooseArea.Point1 = self.originArea.Point1;
            self.curChooseArea.Point2 = self.originArea.Point2;
        }
        #endregion
    }
}
