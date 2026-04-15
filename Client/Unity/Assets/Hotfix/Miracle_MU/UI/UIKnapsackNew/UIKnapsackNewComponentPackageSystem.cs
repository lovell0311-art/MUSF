using Codice.Client.Commands;
using Codice.CM.Common;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace ETHotfix
{

    public static class UIKnapsackNewComponentPackageSystem
    {

        public static async ETTask LoadPackage(this UIKnapsackNewComponent self)
        {
            string res = "PackagePlane";
            //鍔犺浇瀵瑰簲闈㈡澘
            await ResourcesComponent.Instance.LoadGameObjectAsync(res.StringToAB(), res);
            //瀹炰緥鍖栭潰鏉?
            GameObject package = ResourcesComponent.Instance.LoadGameObject(res.StringToAB(), res);
            package.transform.SetParent(self.plane.transform, false);
            package.transform.localPosition = Vector3.zero;
            package.transform.localScale = Vector3.one;

            self.packageCollector = package.GetReferenceCollector();
        }

        /// <summary>
        /// 鍒濆鍖栬儗鍖?
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask InitPackage(this UIKnapsackNewComponent self)
        {
            string packageStage = "LoadPackage";
            try
            {
                await self.LoadPackage();
                packageStage = "CreateBackGridArray";
                self.BackGrids = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_Knapsack_X][];//鍒涘缓鍏锋湁LENGTH_LINE涓厓绱犵殑浜ら敊鏁扮粍锛涙敞鎰忥細鐩稿綋浜庝氦閿欐暟缁勭殑琛屾暟
                for (int i = 0; i < self.BackGrids.Length; i++)
                {
                    self.BackGrids[i] = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_Knapsack_Y];//姣忎竴涓竴缁存暟缁勭殑闀垮害涓篖ENGTH_COLNUME
                }

                packageStage = "CreatGrid";
                // 初始化背包格子。
                self.CreatGrid(
                    UIKnapsackNewComponent.LENGTH_Knapsack_X,
                    UIKnapsackNewComponent.LENGTH_Knapsack_Y,
                    self.packageCollector.GetGameObject("Grids").transform,
                    E_Grid_Type.Knapsack,
                    ref self.BackGrids);
                
                packageStage = "InitPackageGrid";
                self.InitPackageGrid();
                //self.packageCollector.GetButton("BaiTanBtn").onClick.AddSingleListener(() =>
                //{
                //    self.OnBaiTanClick().Coroutine();
                //});
                //self.packageCollector.GetButton("VipBtn").onClick.AddSingleListener(() =>
                //{
                //    self.OnVipClick().Coroutine();
                //});
                //self.coinText = self.packageCollector.GetText("coinTxt");
                //self.ruibiText = self.packageCollector.GetText("yuanbaoTxt");
                //self.qijibiText = self.packageCollector.GetText("qijibiTxt");
                //self.InitCoin();

                //self.packageCollector.GetButton("SplitBtn").onClick.AddSingleListener(() =>
                //{
                //    self.OnSplitClik().Coroutine();
                //});

                ////鐗规潈缁翠慨
                //self.packageCollector.GetButton("RepairBtn").onClick.AddSingleListener(() =>
                //{
                //    self.RepairEquips();
                //});
                packageStage = "BindPackageButtons";
                self.packageCollector.GetButton("SplitBtn").onClick.AddSingleListener(() =>
                {
                    self.OnSplitClik().Coroutine();
                });
                self.packageCollector.GetButton("ClearUpBtn").onClick.AddSingleListener(() =>
                {
                    self.FinishingBackpack().Coroutine();
                });

                self.packageCollector.GetButton("VipBtn").onClick.AddSingleListener(() =>
                {
                    self.OnVipClick().Coroutine();
                });
            }
            catch (Exception e)
            {
                Log.Error($"#KnapsackInitPackage# stage={packageStage} ex={e}");
                throw;
            }
        }


        public static void ClearPackage(this UIKnapsackNewComponent self)
        {
            self.ReleaseGridCollectionVisuals(self.BackGrids);
            if (self.packageCollector != null)
            {
                GameObject.Destroy(self.packageCollector.gameObject);
                self.packageCollector = null;
            }
            Game.EventCenter.RemoveEvent<long>(EventTypeId.GLOD_CHANGE, self.ChangeKnapsackCoin);

        }

        /// <summary>
        /// 閲戝竵鍙樺姩
        /// </summary>
        public static void ChangeKnapsackCoin(this UIKnapsackNewComponent self,long value)
        {
           // self.coinText.text = value.ToString();
        }

        public static void InitPackageGrid(this UIKnapsackNewComponent self)
        {
            var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
            foreach (KnapsackDataItem item in list)
            {
                if (item == null)
                {
                    Log.Error("#KnapsackInitPackageItem# stage=NullItem");
                    continue;
                }

                if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
                try
                {
                    self.AddItem(item, type: E_Grid_Type.Knapsack);
                }
                catch (Exception e)
                {
                    Log.Error($"#KnapsackInitPackageItem# uuid={item.UUID} config={item.ConfigId} pos=({item.PosInBackpackX},{item.PosInBackpackY}) size=({item.X},{item.Y}) type={item.ItemType} ex={e}");
                }
            }
        }

        public static async ETTask OnBaiTanClick(this UIKnapsackNewComponent self)
        {
            Debug.Log("当前状态: " + self.curKnapsackState);
            //if (self.curKnapsackState != E_KnapsackState.KS_Knapsack)
            //{
            //    return;
            //}
            if (self.curKnapsackState == E_KnapsackState.KS_Trade)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "浜ゆ槗涓?鏃犳硶鎽嗘憡");
                return;
            }
            if (self.isOpenStallUp)
            {
                Log.Info("鐣岄潰宸茬粡鎵撳紑");
                return;
            }
            self.isOpenStallUp = true;
            Debug.Log("g2C_BaiTanResponse.Error6666");
            G2C_BaiTanResponse g2C_BaiTanResponse = (G2C_BaiTanResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanRequest { });

            Debug.Log("g2C_BaiTanResponse.Error" + g2C_BaiTanResponse.Error);
            if (g2C_BaiTanResponse.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanResponse.Error.GetTipInfo());
                self.isOpenStallUp = false;
            }
            else
            {
                
                await self.InitStallUp();
                self.ClearEquip();
                self.ClearVip();
                self.ClearSell();
            }
        }

        public static async ETTask OnVipClick(this UIKnapsackNewComponent self)
        {
            if (self.isOpenVipPlane)
            {
                Log.Debug("鐗规潈鍗＄晫闈㈠凡缁忔墦寮€ ");
                return;
            }
            self.isOpenVipPlane = true;
            //鏄惁鏈夋湀鍗＄壒鏉?
            if (self.roleEntity.MaxMonthluCardTimeSpan.TotalSeconds <= 0 && !TitleManager.allTitles.Exists(x => x.TitleId == 60005))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"鏈縺娲昏禐鍔╁崱");
                //UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                //confirmComponent.SetTipText("鏄惁璐拱<color=red>鏈堝崱</color>銆佸紑鍚繙绋嬪晢搴楋紵");
                //confirmComponent.AddActionEvent(() =>
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIShop, (int)E_TopUpType.ShopTopUp);
                //    self.OnCloseClick();
                //});
                return;
            }
            G2C_RemoteOpenResponse g2C_RemoteOpen = (G2C_RemoteOpenResponse)await SessionComponent.Instance.Session.Call(new C2G_RemoteOpenRequest
            {
                Type = 0
            });
            if (g2C_RemoteOpen.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RemoteOpen.Error.GetTipInfo());
                self.isOpenVipPlane = false;
            }
            else
            {
                // Log.DebugGreen($"鍟嗗煄涓殑鐗╁搧:{g2C_RemoteOpen.AllItems.Count}");
                Dictionary<long, KnapsackDataItem> NPCShopDic = new Dictionary<long, KnapsackDataItem>();
                for (int i = 0, length = g2C_RemoteOpen.AllItems.Count; i < length; i++)
                {
                    var item = g2C_RemoteOpen.AllItems[i];
                    // Log.DebugGreen($"鍟嗗簵鐗╁搧:{item.ConfigID} 浣嶇疆锛歿item.PosInBackpackX}锛歿item.PosInBackpackY} 鏁伴噺:{item.Quantity}");
                    KnapsackDataItem knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(item.ItemUID);
                    knapsackDataItem.GameUserId = item.GameUserId;//鐜╁鐨刄ID
                    knapsackDataItem.UUID = item.ItemUID;//瑁呭鐨刄ID
                    knapsackDataItem.ConfigId = item.ConfigID;//瑁呭閰嶇疆琛╥d
                    knapsackDataItem.ItemType = item.Type;//瑁呭绫诲瀷

                    knapsackDataItem.PosInBackpackX = item.PosInBackpackX;//瑁呭鍦ㄨ儗鍖呬腑鐨勮捣濮嬫牸瀛?鍧愭爣
                    knapsackDataItem.PosInBackpackY = item.PosInBackpackY;
                    knapsackDataItem.X = item.Width;//瑁呭鎵€鍗犵殑鏍煎瓙
                    knapsackDataItem.Y = item.Height;
                    knapsackDataItem.SetProperValue(E_ItemValue.Quantity, item.Quantity);//瑁呭鐨勬暟閲?
                    knapsackDataItem.SetProperValue(E_ItemValue.Level, item.ItemLevel);//瑁呭鐨勭瓑绾?
                    NPCShopDic[item.ItemUID] = knapsackDataItem;
                }
                //璁剧疆 NPC鍟嗗煄鐗╁搧鐨勫睘鎬?
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

                await self.InitVip();
                self.InitShopEquip(NPCShopDic, g2C_RemoteOpen.NpcId, E_BuyType.Remote);
                await self.InitSell();

                self.ClearEquip();

                self.ClearStallUp();

                self.ClearWareHouse();
            }
        }

        public static async ETTask OnSplitClik(this UIKnapsackNewComponent self)
        {
            //鏄惁鏈夌墿鍝佸彲浠ュ垎鍫?
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
                UIComponent.Instance.VisibleUI(UIType.UIHint, "娌℃湁瑁呭 鍙互鍒嗗爢");
                return;
            }
           self. IsSplit = !self.IsSplit;
            if (self.IsSplit)
            {
                self.confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent(E_TipPanelType.Split);
                self.confirmComponent.splitCount = 1;

                if (self.confirmComponent != null && self.originArea.ItemData != null)
                {
                    self.ClearIntroduction();
                    if (self.originArea.ItemData.GetProperValue(E_ItemValue.Quantity) > 1)
                    {
                        self.confirmComponent.splitItem = self.originArea;
                        if (self.confirmComponent.SplitObj != null)//&& confirmComponent.SplitObj.name != originArea.ItemData.item_Info.ResName
                        {
                            ResourcesComponent.Instance.RecycleGameObject(self.confirmComponent.SplitObj);
                            //ResourcesComponent.Instance.DestoryGameObjectImmediate(confirmComponent.SplitObj, confirmComponent.SplitObj.name.StringToAB());

                        }

                        self.originArea.ItemData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                        self.confirmComponent.SplitObj = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
                        self.confirmComponent.SplitObj.SetUI();
                        self.confirmComponent.SplitObj.transform.SetParent(self.confirmComponent.objIcon.transform, false);
                        self.confirmComponent.SplitObj.transform.localPosition = new Vector3(0, 0, -50);
                        // confirmComponent.SplitObj.transform.position = confirmComponent.objPos;
                        self.confirmComponent.SplitinputField.text = self.confirmComponent.splitItem.ItemData.GetProperValue(E_ItemValue.Quantity).ToString();//鏄剧ず 鐗╁搧鐨勬暟閲?
                    }
                }


                self.confirmComponent.SplitEventAction = async () =>
                {
                    int? count = self.confirmComponent.GetSplitFunc?.Invoke();

                    if (self.originArea == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "璇锋偍閫夋嫨瑕佸垎鍫嗙殑鐗╁搧");
                        return;
                    }
                    if (count == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "璇锋偍璁剧疆 鐗╁搧鍒嗗爢鏁伴噺");
                        return;
                    }
                    if (count == self.originArea.ItemData.GetProperValue(E_ItemValue.Quantity))
                    {
                        self.confirmComponent.SplitinputField.text = 1.ToString();
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "分堆数量应小于物品的最大数量");
                        return;
                    }

                    //  璇锋眰鍒嗗爢
                    G2C_SplitItems g2C_SplitItems = (G2C_SplitItems)await SessionComponent.Instance.Session.Call(new C2G_SplitItems
                    {
                        ItemUUID = self.originArea.UUID,
                        Count = (int)count
                    });
                    self.IsSplit = false;
                    if (g2C_SplitItems.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SplitItems.Error.GetTipInfo());

                    }
                    else
                    {
                        UIComponent.Instance.Remove(UIType.UIConfirm);
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "鎷嗗垎鎴愬姛");
                    }
                };
                self.confirmComponent.SplitCancelAction = () =>
                {
                    self.IsSplit = false;
                    ChangeGridColor(false);
                    self.confirmComponent = null;
                };
            }
            ChangeGridColor();



            void ChangeGridColor(bool isSplit = true)
            {
                self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, E_Grid_Type.Knapsack);
                for (int i = 0; i < UIKnapsackNewComponent.LENGTH_Knapsack_Y; i++)
                {
                    for (int j = 0; j < UIKnapsackNewComponent.LENGTH_Knapsack_X; j++)
                    {
                        KnapsackGrid grid = self.grids[j][i];
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


            await ETTask.CompletedTask;
        }

        public static async ETTask FinishingBackpack(this UIKnapsackNewComponent self)
        {
            if (self.curKnapsackState == E_KnapsackState.KS_Trade)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "浜ゆ槗鐘舵€?鏃犳硶鏁寸悊");
                return;
            }

            if (Time.time < KnapsackItemsManager.PackBackpackTime)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请稍后再整理");
                //  Log.DebugGreen($"璇风◢鍚庡啀鏁寸悊");
                return;
            }
            KnapsackItemsManager.IsPackBackpack = true;
            KnapsackItemsManager.PackBackpackTime = Time.time + KnapsackItemsManager.PackBackpackSpaceTime;

            self.InitFinishGrids();

            self.KnapsackItemSort();

            if (self.IsCanFinish == false)
            {
                KnapsackItemsManager.IsPackBackpack = false;
                for (int i = 0, length = self.knapdataList.Count; i < length; i++)
                {
                    self.knapdataList[i].PosInBackpackX = self.knapdataList[i].tempPosInBackpackX;
                    self.knapdataList[i].PosInBackpackY = self.knapdataList[i].tempPosInBackpackY;
                }
                UIComponent.Instance.VisibleUI(UIType.UIHint, "璇峰厛棰勭暀涓よ鏍煎瓙 鍐嶆暣鐞嗭紒");
                return;
            }
            else
            {
                await origanizeBackpack();
                KnapsackItemsManager.IsPackBackpack = false;
            }

            async ETTask origanizeBackpack()
            {
                G2C_OrganizeBackpack g2C_Organize = (G2C_OrganizeBackpack)await SessionComponent.Instance.Session.Call(new C2G_OrganizeBackpack
                {
                    ItemsNewPosition = new Google.Protobuf.Collections.RepeatedField<ItemPositionInBackpack> { self.itemPositionInBackpacks }
                });

                if (g2C_Organize.Error != 0)
                {
                    // Log.DebugBrown($"{g2C_Organize.Error}->");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Organize.Error.GetTipInfo());

                }
                else
                {
                    //鏀瑰彉鐗╁搧浣嶇疆
                    self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, E_Grid_Type.Knapsack);
                    //娓呯悊鑳屽寘
                    for (int i = 0; i < UIKnapsackNewComponent.LENGTH_Knapsack_Y; i++)
                    {
                        for (int j = 0; j < UIKnapsackNewComponent.LENGTH_Knapsack_X; j++)
                        {

                            KnapsackGrid grid = self.grids[j][i];
                            if (grid.IsOccupy)
                            {
                                self.RemoveItem(grid.Data, true);
                                KnapsackTools.RemoveEquip(grid.Data);
                            }
                        }

                    }
                    //閲嶆柊鍔犲叆鐗╁搧
                    // for (int i = 0, length = LineList.Count; i < length; i++)
                    for (int i = 0, length = self.knapdataList.Count; i < length; i++)
                    {
                        var item = self.knapdataList[i];
                        self.AddItem(item, type: E_Grid_Type.Knapsack);
                        //鏀瑰彉鐗╁搧鐨?浣嶇疆淇℃伅
                        if (KnapsackItemsManager.KnapsackItems.TryGetValue(item.Id, out KnapsackDataItem dataItem))
                        {

                            dataItem.PosInBackpackX = item.PosInBackpackX;
                            dataItem.PosInBackpackY = item.PosInBackpackY;
                            KnapsackTools.AddEquip(dataItem);
                        }
                    }
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "整理完成");
                    //RoleOnHookComponent.Instance.AutoPickUpItem();
                }
            }
        }

        //鍒濆鍖栨牸瀛?
        public static void InitFinishGrids(this UIKnapsackNewComponent self)
        {
            //lock (InitFinishLock)
            {
                self.IsCanFinish = true;
                self.FinishGrids = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_Knapsack_X][];
                self.itemPositionInBackpacks = new List<ItemPositionInBackpack>();
                self.MedicineList = new List<KnapsackDataItem>();
                //瀛楀吀杞崲涓?涓€涓摼琛?鍥犱负瀛楀吀娌℃硶鐩存帴杩涜鎺掑簭鎿嶄綔
                //  LineList = new List<KeyValuePair<long, KnapsackDataItem>>(KnapsackItemsManager.KnapsackItems);
                self.knapdataList = KnapsackItemsManager.KnapsackItems.Values.ToList();

                for (int i = 0; i < self.FinishGrids.Length; i++)
                {
                    self.FinishGrids[i] = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_Knapsack_Y];
                }
                for (int j = 0; j < UIKnapsackNewComponent.LENGTH_Knapsack_Y; j++)
                {
                    for (int i = 0; i < UIKnapsackNewComponent.LENGTH_Knapsack_X; i++)
                    {
                        self.FinishGrids[i][j] = new KnapsackNewGrid { isOccupy = false };
                    }
                }
            }
        }
        public static void KnapsackItemSort(this UIKnapsackNewComponent self)
        {
            {

                //鍒╃敤閾捐〃鐨凷ort鏂规硶杩涜 鎺掑簭
                //il2cpp 涓嶆敮鎸?
                /*   LineList.Sort(delegate (KeyValuePair<long, KnapsackDataItem> item_1, KeyValuePair<long, KnapsackDataItem> item_2)
                   {
                       Log.DebugWhtie($"{item_1.Value.ItemType} : {item_2.Value.ItemType}");
                       //item_1 涓?item_2 姣旇緝 澶т簬0 璇存槑item_1 澶т簬 item_2 锛岀瓑浜?0 璇存槑鏄?鐢卞皬鍒板ぇ鐨勯『搴?

                       //鎺掑簭 杩欓噷鍐冲畾浣犵殑鎺掑簭椤哄簭鏄粠澶уぇ灏忚繕鏄粠灏忓埌澶э紝杩欓噷鐨勬帓鍒楅『搴忔槸浠庡皬鍒板ぇ銆傚鏋滄兂鏀瑰彉鎺掑簭鏂瑰紡锛岀洿鎺ュ鎹1鍜宻2灏辫浜嗐€?

                       if (item_1.Value.ConfigId.CompareTo(item_2.Value.ConfigId) > 0)//鎸夎澶囩被鍨?鐢卞皬鍒板ぇ鎺掑簭
                       {
                           return 1;
                       }
                       else
                       {
                            return -1;

                       }
                   });*/


                /*
                 //鍐掓场鎺掑簭
                 KnapsackDataItem temp;
                 for (int i = 0, length = knapdataList.Count-1; i < length; i++)
                 {
                     if (knapdataList[i].ConfigId > knapdataList[i + 1].ConfigId)
                     {
                         temp = knapdataList[i];
                         knapdataList[i] = knapdataList[i + 1];
                         knapdataList[i + 1] = temp;
                     }
                 }*/
                self.knapdataList.Sort((item_1, item_2) =>
                {
                    return item_1.ConfigId.CompareTo(item_2.ConfigId);
                    /* if (item_1.ConfigId.CompareTo(item_2.ConfigId) > 0)//鎸夎澶囩被鍨?鐢卞皬鍒板ぇ鎺掑簭
                     {
                         return 1;
                     }
                     else
                     {
                         return -1;

                     }*/
                });

                self.itemPositionInBackpacks.Clear();
                // for (int i = 0, length = LineList.Count; i < length; i++)
                for (int i = 0, length = self.knapdataList.Count; i < length; i++)
                {
                    var item = self.knapdataList[i];

                    if (KnapsackItemsManager.MedicineHpIdList.Contains(item.ConfigId) || KnapsackItemsManager.MedicineMpIdList.Contains(item.ConfigId))
                    {
                        self.MedicineList.Add(item);
                        continue;
                    }

                    item.ConfigId.GetItemInfo_Out(out item.item_Info);
                    var pos = self.GetAutoIndex(item);
                    if (pos == null)
                    {
                        item.tempPosInBackpackX = item.PosInBackpackX;
                        item.tempPosInBackpackY = item.PosInBackpackY;

                        self.IsCanFinish = false;
                        break;
                    }
                    else
                    {

                        self.itemPositionInBackpacks.Add(new ItemPositionInBackpack
                        {
                            ItemUID = item.Id,
                            PosInBackpackX = pos.Value.x,
                            PosInBackpackY = pos.Value.y
                        });

                        item.tempPosInBackpackX = item.PosInBackpackX;
                        item.tempPosInBackpackY = item.PosInBackpackY;

                        item.PosInBackpackX = pos.Value.x;
                        item.PosInBackpackY = pos.Value.y;
                        self.AddEquip(item);

                    }
                }
                //娣诲姞鑽摱
                for (int i = 0, length = self.MedicineList.Count; i < length; i++)
                {
                    var item = self.MedicineList[i];
                    item.ConfigId.GetItemInfo_Out(out item.item_Info);
                    var pos = self.GetAutoIndex(item);
                    if (pos == null)
                    {

                        item.tempPosInBackpackX = item.PosInBackpackX;
                        item.tempPosInBackpackY = item.PosInBackpackY;

                        self.IsCanFinish = false;
                        break;
                    }
                    else
                    {

                        self.itemPositionInBackpacks.Add(new ItemPositionInBackpack
                        {
                            ItemUID = item.Id,
                            PosInBackpackX = pos.Value.x,
                            PosInBackpackY = pos.Value.y
                        });

                        item.tempPosInBackpackX = item.PosInBackpackX;
                        item.tempPosInBackpackY = item.PosInBackpackY;

                        item.PosInBackpackX = pos.Value.x;
                        item.PosInBackpackY = pos.Value.y;
                        self.AddEquip(item);

                    }
                }
            }
        }

        //鑾峰彇涓存椂绌洪棿鐨勪綅缃?
        public static Vector2Int? GetAutoIndex(this UIKnapsackNewComponent self,KnapsackDataItem dataItem)
        {
            var Point1 = Vector2.zero;
            var Point2 = new Vector2Int(dataItem.X - 1, dataItem.Y - 1);
            bool IsSinglePoint = Point1 == Point2;

            //閬嶅巻鍔犲叆锛屾槸鍚﹀彲浠ュ姞鍏?
            for (int j = 0; j < UIKnapsackNewComponent. LENGTH_Knapsack_Y; j++)
            {

                for (int i = 0; i < UIKnapsackNewComponent. LENGTH_Knapsack_X; i++)
                {

                    //鍗曚釜鐗╀綋
                    if (IsSinglePoint)
                    {
                        if (self.FinishGrids[i][j].isOccupy == false)
                        {
                            return new Vector2Int(i, j);
                        }
                    }
                    //鑼冨洿鎬х墿浣?
                    else
                    {
                        //濡傛灉瓒呭嚭杈圭紭鐩存帴杩囨护
                        if (!KnapsackTools.ContainGridObj(i, j, i + Point2.x, j + Point2.y, self.FinishGrids))
                        {
                            return new Vector2Int(i, j);
                        }
                    }
                }
            }
            return new Vector2Int(0,0);
          
        }
        //娣诲姞鍒颁复鏃剁┖闂?
        public static void AddEquip(this UIKnapsackNewComponent self,KnapsackDataItem dataItem)
        {
            var Point1 = new Vector2Int(dataItem.PosInBackpackX, dataItem.PosInBackpackY);
            var Point2 = new Vector2Int(dataItem.PosInBackpackX + dataItem.X - 1, dataItem.PosInBackpackY + dataItem.Y - 1);
            bool IsSinglePoint = Point1 == Point2;
            if (IsSinglePoint)
            {
                self.FinishGrids[Point1.x][Point1.y].isOccupy = true;
            }
            else
            {
                //   List<KnapsackGrid> gris = KnapsackTools.GetAreaGrids(Point1.x, Point1.y, Point2.x, Point2.y, FinishGrids);
                List<Vector2> gris = KnapsackTools.GetAreaGrids(Point1.x, Point1.y, Point2.x, Point2.y, self.FinishGrids);

                for (int i = 0, length = gris.Count; i < length; i++)
                {
                    //  gris[i].isOccupy = true;
                    self.FinishGrids[(int)gris[i].x][(int)gris[i].y].isOccupy = true;
                }

            }
        }

        /// <summary>
        /// 鐜╁闀挎寜浣跨敤鑳屽寘涓殑鐗╁搧
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static async ETVoid PlayerUserItemInTheBackpack(this UIKnapsackNewComponent self, KnapsackDataItem dataItem)
        {
            //if (self.useItem == null) return;

            if (KnapsackItemsManager.MedicineHpIdList.Contains(dataItem.ConfigId))
            {
                UIMainComponent.Instance.medicineEntity_Hp.curMedicineUUID = dataItem.UUID;
            }
            else if (KnapsackItemsManager.MedicineMpIdList.Contains(dataItem.ConfigId))
            {
                UIMainComponent.Instance.medicineEntity_Mp.curMedicineUUID = dataItem.UUID;
            }

            if (dataItem != null && dataItem.ItemType == (int)E_ItemType.Mounts)
            {
                //缂撳瓨褰撳墠浣跨敤鐨勫潗楠慤UID
                UIMainComponent.Instance.curMountUUID = dataItem.UUID;
            }

            G2C_PlayerUseItemInTheBackpack g2C_PlayerUseItemIn = (G2C_PlayerUseItemInTheBackpack)await SessionComponent.Instance.Session.Call(new C2G_PlayerUseItemInTheBackpack { ItemUUID = dataItem.UUID });
            Log.DebugGreen("PlayerUserItemInTheBackpack" + g2C_PlayerUseItemIn.Error+"::"+ dataItem.ConfigId + ":name"+ dataItem.item_Info.Name);
            if (g2C_PlayerUseItemIn.Error==99&& dataItem.ConfigId==310046)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "璇ョ墿鍝佸凡鍒拌揪浣跨敤涓婇檺");
            }
            if (g2C_PlayerUseItemIn.Error != 0)
            {
               
            }
            else
            {
                if (dataItem.ItemType == (int)E_ItemType.Mounts)
                {
                    if (UIMainComponent.Instance != null)
                    {
                        UIMainComponent.Instance.TryUseActivatedMount(dataItem.UUID);
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "坐骑已激活，可在坐骑界面中使用");
                    }
                }
            }
            self.ClearIntroduction();
        }
    }
}
