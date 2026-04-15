using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 其他玩家的摊位
    /// </summary>
    public partial class UIKnapsackComponent
    {
        RoleStallUpComponent StallUpOtherComponent;
        private GameObject StallUpOtherContent;

        private KnapsackGrid[][] StallUp_OtherGrids;
        public const int LENGTH_StallUp_Other_X = 8;
        public const int LENGTH_StallUp_Other_Y = 11;
        public Text OtherStasllName;

        public RoleEntity otherRole;

        public void Init_StallOther()
        {
            ///获取玩家的摆摊组件
            if (ClickSelectUnitEntityComponent.Instance.curSelectUnit is RoleEntity otherRole)
            {
                StallUpOtherComponent = otherRole.GetComponent<RoleStallUpComponent>();
                this.otherRole = otherRole;
            }
            ReferenceCollector collector = Stallup_OtherPlayer.GetReferenceCollector();
            StallUpOtherContent = collector.GetGameObject("Grids");
            OtherStasllName = collector.GetText("OtherStasllName");
            OtherStasllName.text = StallUpOtherComponent.curStallUpName;
          

            StallUp_OtherGrids = new KnapsackGrid[LENGTH_StallUp_Other_X][];
            for (int i = 0; i < StallUp_OtherGrids.Length; i++)
            {
                StallUp_OtherGrids[i] = new KnapsackGrid[LENGTH_StallUp_Other_Y];
            }
            //初始化格子
            CreatGrid(LENGTH_StallUp_Other_X, LENGTH_StallUp_Other_Y, StallUpOtherContent.transform, E_Grid_Type.Stallup_OtherPlayer, ref StallUp_OtherGrids);
            Init_StallUpOtherEquip().Coroutine();


        }
        /// <summary>
        /// 初始化摊位的物品
        /// </summary>
        /// <param name="itemList">商店物品ID的 集合</param>
        public async ETVoid Init_StallUpOtherEquip()
        {
            G2C_BaiTanLookLookResponse g2C_BaiTanLookLook = (G2C_BaiTanLookLookResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanLookLookRequest
            {
                BaiTanInstanceId = otherRole.Id
            });
            if (g2C_BaiTanLookLook.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BaiTanLookLook.Error.GetTipInfo());
            }
            else
            {
               
                for (int i = 0, length = g2C_BaiTanLookLook.Prop.count; i < length; i++)
                {
                    var item = g2C_BaiTanLookLook.Prop[i];
                    item.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                    if (StallUpOtherComponent.StallUpItemDic.TryGetValue(item.ItemUUID, out KnapsackDataItem knapsackDataItem))
                    {
                        knapsackDataItem.UUID = item.ItemUUID;
                        knapsackDataItem.ConfigId = item.ConfigId;
                        knapsackDataItem.PosInBackpackX = item.PosInBackpackX;
                        knapsackDataItem.PosInBackpackY = item.PosInBackpackY;
                        knapsackDataItem.X = item_Info.X;
                        knapsackDataItem.Y = item_Info.Y;
                        knapsackDataItem.SetProperValue(E_ItemValue.Stall_BuyPrice, item.Price);
                        knapsackDataItem.SetProperValue(E_ItemValue.Stall_BuyMoJingPrice, item.Price2);
                        AddItem(knapsackDataItem, type: E_Grid_Type.Stallup_OtherPlayer);
                    }
                }
            }

        }
        /// <summary>
        /// 清理摊位
        /// </summary>
        public void CleanStallUpOther()
        {
            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Stallup_OtherPlayer);
            for (int i = 0; i < LENGTH_StallUp_Other_Y; i++)
            {
                for (int j = 0; j < LENGTH_StallUp_Other_X; j++)
                {
                    KnapsackGrid grid = grids[j][i];
                    if (grid.IsOccupy)
                    {
                        grid.Data.ItemData.Dispose();
                        RemoveItem(grid.Data, true);
                    }
                }

            }

            foreach (var item in StallUpOtherComponent.StallUpItemDic.Values)
            {
                item.Dispose();
            }
            StallUpOtherComponent.StallUpItemDic.Clear();
        }
    }
}
