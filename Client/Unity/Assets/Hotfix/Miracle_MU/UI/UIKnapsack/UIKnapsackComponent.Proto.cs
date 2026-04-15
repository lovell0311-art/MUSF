using ETModel;
using NPOI.SS.Formula.Functions;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 璇锋眰鍗忚绫?
    /// </summary>
    public partial class UIKnapsackComponent
    {

 
        /// <summary>
        /// 璇锋眰鏈嶅姟鍣ㄤ涪寮冪墿鍝?
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async ETVoid SendDiscardKnasackItemMessage()
        {
           
                var node = GetNearNode();
            G2C_DelBackpackItemResponse g2C_Del = (G2C_DelBackpackItemResponse)await SessionComponent.Instance.Session.Call(new C2G_DelBackpackItemRequest
            {
                ItemUUID = originArea.UUID,
                PosInSceneX = node.x,
                PosInSceneY = node.z,
            });
            if (g2C_Del.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Del.Error.GetTipInfo());
                ResetGridObj();
            }
            else
            {
                //if (configId == 320106)
                //{
                //    TransformPointManager.Instance.AddPoint(GetNearNode().x, GetNearNode().z);
                //    //TransferPointTools.Instances.AddTransferAreas(new Vector2Int(dropData.PosX, dropData.PosY));
                //    GameObject attackEffect = ResourcesComponent.Instance.LoadGameObject(Men.StringToAB(), Men);
                //    attackEffect.transform.position = AstarComponent.Instance.GetVectory3(GetNearNode().x, GetNearNode().z);
                //}
                #region 涓㈠純鐨勬槸鑽搧 鍑忓皯鑽摱鐨勬暟閲?
                //UIMainComponent.Instance.ChangeNum(originArea.ItemData);

                #endregion
                //涓㈠純鎴愬姛鍚?浼氭帹閫?->G2C_ItemsLeaveBackpack_notice_Handler锛堢洿鎺ュ湪杩欓噷鍐?涔熻锛?
                ResourcesComponent.Instance.RecycleGameObject(this.curDropObj);
                // RemoveItem(originArea);

                //涓㈠純瀹濈
                //if (BeginnerGuideData.IsComplete(10))
                //{
                //    BeginnerGuideData.SetBeginnerGuide(10);
                //    UIMainComponent.Instance.SetBeginnerGuide();
                //}
                GuideComponent.Instance.CheckIsShowGuide(true);
            }

        }


        /// <summary>
        /// 閫氱煡鏈嶅姟绔?浣嶇疆鍙樺姩
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <returns></returns>
        public async ETVoid SendMoveKnapsackItemMessage()
        {
           
            G2C_MoveBackpackItemResponse g2C_Move = (G2C_MoveBackpackItemResponse)await SessionComponent.Instance.Session.Call(new C2G_MoveBackpackItemRequest
            {
                ItemUUID = this.originArea.UUID,
                PosInBackpackX = this.curChooseArea.Point1.x,
                PosInBackpackY = this.curChooseArea.Point1.y,
            });
            if (g2C_Move.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Move.Error.GetTipInfo());
                ResetGridObj();
            }
            else
            {

                curChooseArea.UUID = originArea.UUID;
                curChooseArea.ItemData = originArea.ItemData;
                //鏀瑰彉鐗╁搧鐨勪綅缃?
                RemoveItem(originArea);
                AddKnapsackItem(curChooseArea, curDropObj);

            }
        }
       

        /// <summary>
        /// 绌挎埓瑁呭
        /// </summary>
        /// <param name="equipId">瑁呭鐨刄UID</param>
        /// <param name="part">绌挎埓閮ㄤ綅 </param>
        /// <returns></returns>

        //public async ETVoid RenewItemRequest()
        //{

        //    G2C_RenewItemRequest g2C_Equip = (G2C_RenewItemRequest)await SessionComponent.Instance.Session.Call(new C2G_RenewItemRequest
        //    {
        //        ItemUUID = originArea.ItemData.UUID
        //    });
        //    if (g2C_Equip.Error != 0)
        //    {
        //        UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_Equip.Error.GetTipInfo()}");
        //    }
        //}
        

        /// <summary>
        /// 绌挎埓瑁呭
        /// </summary>
        /// <param name="equipId">瑁呭鐨刄UID</param>
        /// <param name="part">绌挎埓閮ㄤ綅 </param>
        /// <returns></returns>

        public async ETVoid RequestWareEquip()
        {
          

            G2C_EquipItemResponse g2C_Equip = (G2C_EquipItemResponse)await SessionComponent.Instance.Session.Call(new C2G_EquipItemRequest
            {
                ItemUUID = originArea.ItemData.UUID,
                EquipPosition = curWarePart

            });
            if (g2C_Equip.Error != 0)
            {
                Log.DebugRed($"g2C_Equip.Error:{g2C_Equip.Error}");
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_Equip.Error.GetTipInfo()}");
              
                ResetGridObj();
            }
            else
            {
                // 鎴愬姛
                // G2C_UnitEquipLoad_notice锛堟帹閫佸疄浣撶┛鎴磋澶?鍛ㄨ竟涓€瀹氳寖鍥寸帺瀹朵篃骞挎挱)锛?
                // 浼氭帹閫丟2C_ItemsIntoBackpack_notice鏇存柊鑳屽寘鐗╁搧
                //淇濆瓨 褰撳墠绌挎埓鐨勮澶?

                //WareEquipItem(curWarePart, curChooseArea, curDropObj);
                //  RemoveItem(originArea);
                GuideComponent.Instance.CheckIsShowGuide(true);
                ResourcesComponent.Instance.RecycleGameObject(curDropObj);
            }
        }

        /// <summary>
        /// 鍗歌浇瑁呭
        /// </summary>
        /// <param name="slotId">瑁呭閮ㄤ綅鐨?/param>
        /// <param name="X"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public async ETVoid UnLoadEquip(int slotId, int X, int y)
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
                //鎴愬姛
                //浼氭帹閫丟2C_ItemsIntoBackpack_notice鏇存柊鑳屽寘鐗╁搧 
                //鎺ㄩ€佸疄浣撶┛鎴磋澶?鍛ㄨ竟涓€瀹氳寖鍥寸帺瀹朵篃骞挎挱) G2C_UnitEquipLoad_notice
            }
        }

        /// <summary>
        /// 鐜╁闀挎寜浣跨敤鑳屽寘涓殑鐗╁搧
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async ETVoid PlayerUserItemInTheBackpack(long uid)
        {
            if (useItem == null) return;
           
            if (KnapsackItemsManager.MedicineHpIdList.Contains(useItem.ItemData.ConfigId))
            {
                UIMainComponent.Instance.medicineEntity_Hp.curMedicineUUID = uid;
            }
            else if (KnapsackItemsManager.MedicineMpIdList.Contains(useItem.ItemData.ConfigId))
            {
                UIMainComponent.Instance.medicineEntity_Mp.curMedicineUUID = uid;
            }

            if (useItem.ItemData != null && useItem.ItemData.ItemType == (int)E_ItemType.Mounts)
            {
                //缂撳瓨褰撳墠浣跨敤鐨勫潗楠慤UID
                UIMainComponent.Instance.curMountUUID = uid;
            }

            G2C_PlayerUseItemInTheBackpack g2C_PlayerUseItemIn = (G2C_PlayerUseItemInTheBackpack)await SessionComponent.Instance.Session.Call(new C2G_PlayerUseItemInTheBackpack { ItemUUID = uid });
            if (g2C_PlayerUseItemIn.Error != 0)
            {
                filledImage.fillAmount = 0;
                isDroping = false;
                dianjiImage.SetActive(false);
                useItem = null;
                userItemObj = null;
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_PlayerUseItemIn.Error.GetTipInfo());
               
            }
            else
            {
                isDroping = false;
                dianjiImage.SetActive(false);
                filledImage.fillAmount = 0;

                if (useItem?.ItemData != null && useItem.ItemData.ItemType == (int)E_ItemType.Mounts)
                {
                    if (UIMainComponent.Instance != null)
                    {
                        UIMainComponent.Instance.TryUseActivatedMount(uid);
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "坐骑已激活，可在坐骑界面中使用");
                    }
                }

              
               
             /*   if (useItem.ItemData != null&& useItem.ItemData.ItemType == (int)E_ItemType.Mounts)
                {
                    //缂撳瓨褰撳墠浣跨敤鐨勫潗楠慤UID
                    UIMainComponent.Instance.curMountUUID = uid;
                }*/
               /* else if (useItem.ItemData.ItemType == (int)E_ItemType.Consumables)//娑堣€楀搧
                {
                    //UIMainComponent.Instance.ChangeNum(useItem.ItemData,false);//鏀瑰彉涓荤晫闈笂 鑽摱鐨勬暟閲忔樉绀?
                  
                    if (useItem.ItemData.GetProperValue(E_ItemValue.Quantity) is int count && count > 1)
                    {
                        ChangeItemCount(userItemObj, count);
                    }

                }

                if (KnapsackItemsManager.KnapsackItems.ContainsKey(useItem.ItemData.UUID) == false) //鑳屽寘涓凡缁忕Щ闄?璇ョ墿鍝?
                {
                  
                    RemoveItem(useItem, true);
                }*/

                useItem = null;
                userItemObj = null;
            }
        }

        /// <summary>
        /// 涓婃灦瑁呭鍒拌棌瀹濋榿
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="sellgold"></param>
        /// <returns></returns>
        public async ETVoid listingTreasureHouse(long uid,int sellgold)
        {
            G2C_listingTreasureHouse g2C_ListingTreasure = (G2C_listingTreasureHouse)await SessionComponent.Instance.Session.Call(new C2G_listingTreasureHouse()
            {
                ItemUUID = uid,
                ItemPrice = sellgold
            });
            if (g2C_ListingTreasure.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ListingTreasure.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "上架成功");
                RemoveKnapsack(originArea.UUID);
                CloseIntroduction();
            }
        }


        #region 浠撳簱
        /// <summary>
        /// 鑳屽寘鎷栨嫿鍒颁粨搴?
        /// </summary>
        /// <returns></returns>
        public async ETVoid SendKnapsackItem2WareHouseMessage()
        {
            curChooseArea.UUID = originArea.UUID;
            curChooseArea.ItemData = originArea.ItemData;
           
            G2C_AddWarehouseItem g2C_AddWarehouse = (G2C_AddWarehouseItem)await SessionComponent.Instance.Session.Call(new C2G_AddWarehouseItem
            {
                ItemUUID = originArea.UUID,
                PosInBackpackX = curChooseArea.Point1.x,
                PosInBackpackY = curChooseArea.Point1.y + (MAX_HOUSE_LENGSH_Y * (curPage - 1)),
            });
            if (g2C_AddWarehouse.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_AddWarehouse.Error.GetTipInfo());
               
                ResetGridObj();
            }
            else
            {
               
                ResourcesComponent.Instance.RecycleGameObject(curDropObj);
                //鎺ㄩ€丟2C_AddWarehouseItem_notice 
                /*  curChooseArea.ItemData.PosInBackpackX = curChooseArea.Point1.x;
                  curChooseArea.ItemData.PosInBackpackY = curChooseArea.Point1.y + (MAX_HOUSE_LENGSH_Y * (curPage - 1));
               //   AddKnapsackItem(curChooseArea, curDropObj);
                  if (KnapsackItemsManager.WareHouseItems.ContainsKey(curChooseArea.UUID))
                  {
                      Log.DebugBrown($"浠撳簱 宸插寘鍚鐗╁搧");
                      KnapsackItemsManager.WareHouseItems[curChooseArea.UUID].ItemValueDic = curChooseArea.ItemData.ItemValueDic;
                  }
                  //浠庤儗鍖呬腑绉婚櫎
                  */

              //  RemoveItem(originArea);
            }
        }
        /// <summary>
        /// 浠撳簱 鍒拌儗鍖?
        /// </summary>
        /// <returns></returns>
        public async ETVoid SendWareHouse2KnapsackItemMessage()
        {
            G2C_DelWarehouseItem g2C_DelWarehouseItem = (G2C_DelWarehouseItem)await SessionComponent.Instance.Session.Call(new C2G_DelWarehouseItem
            {
                ItemUUID = this.originArea.UUID,
                PosInBackpackX = this.curChooseArea.Point1.x,
                PosInBackpackY = this.curChooseArea.Point1.y
            });
            if (g2C_DelWarehouseItem.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_DelWarehouseItem.Error.GetTipInfo());
                ResetGridObj();
            }
            else
            {
                ResourcesComponent.Instance.RecycleGameObject(curDropObj);
                curChooseArea.UUID = originArea.UUID;
                curChooseArea.ItemData = originArea.ItemData;

                //绉婚櫎 浠撳簱涓殑鐗╁搧
                RemoveWareHouse(originArea.UUID);
                //鏀瑰彉鐗╁搧鐨勪綅缃?
                RemoveItem(originArea);

            }
        }
        /// <summary>
        /// 绉诲姩浠撳簱鐗╁搧鐨勪綅缃?
        /// </summary>
        /// <returns></returns>
        public async ETVoid MoveWarehouseItemAsync()
        {
        
            G2C_MoveWarehouseItem g2C_MoveWarehouseItem = (G2C_MoveWarehouseItem)await SessionComponent.Instance.Session.Call(new C2G_MoveWarehouseItem
            {
                ItemUUID = this.originArea.UUID,
                PosInBackpackX = this.curChooseArea.Point1.x,
                PosInBackpackY = curChooseArea.Point1.y + (MAX_HOUSE_LENGSH_Y * (curPage - 1))
            });
            if (g2C_MoveWarehouseItem.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MoveWarehouseItem.Error.GetTipInfo());
              
                ResetGridObj();
            }
            else
            {
                //鎺ㄩ€丟2C_MoveWarehouseItem_notice 
                curChooseArea.UUID = originArea.UUID;
                curChooseArea.ItemData = originArea.ItemData;
               
                //鏀瑰彉鐗╁搧鐨勪綅缃?
                RemoveItem(originArea);
                AddKnapsackItem(curChooseArea, curDropObj);
            }
        }
        #endregion

    }
}
