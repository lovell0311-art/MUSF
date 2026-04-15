using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace ETHotfix
{
    public static class UIKnapsackNewComponentStallup_OtherPlayer
    {
        public static async ETTask InitStallupOther(this UIKnapsackNewComponent self)
        {
            if (self.isOpenStallUpOther) return;
            self.isOpenStallUpOther = true;
            string res = "Stallup_OtherPlayer";
            //加载对应面板
            await ResourcesComponent.Instance.LoadGameObjectAsync(res.StringToAB(), res);
            //实例化面板 
            GameObject stallUp = ResourcesComponent.Instance.LoadGameObject(res.StringToAB(), res);
            stallUp.transform.SetParent(self.plane.transform, false);
            stallUp.transform.localPosition = Vector3.zero;
            stallUp.transform.localScale = Vector3.one;
            self.stallUpCollector = stallUp.GetReferenceCollector();

            ///获取玩家的摆摊组件
            if (ClickSelectUnitEntityComponent.Instance.curSelectUnit is RoleEntity otherRole)
            {
                self.StallUpOtherComponent = otherRole.GetComponent<RoleStallUpComponent>();
                self.otherRole = otherRole;
            }

            self.StallUpOtherContent = self.stallUpCollector.GetGameObject("Grids");
            self.OtherStasllName = self.stallUpCollector.GetText("OtherStasllName");
            self.OtherStasllName.text = self.StallUpOtherComponent.curStallUpName;


            self.StallUp_OtherGrids = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_StallUp_Other_X][];
            for (int i = 0; i < self.StallUp_OtherGrids.Length; i++)
            {
                self.StallUp_OtherGrids[i] = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_StallUp_Other_Y];
            }
            //初始化格子
            self.CreatGrid(UIKnapsackNewComponent.LENGTH_StallUp_Other_X, UIKnapsackNewComponent.LENGTH_StallUp_Other_Y, self.StallUpOtherContent.transform, E_Grid_Type.Stallup_OtherPlayer, ref self.StallUp_OtherGrids);


            await self.Init_StallUpOtherEquip();
        }

        /// <summary>
        /// 初始化摊位的物品
        /// </summary>
        /// <param name="itemList">商店物品ID的 集合</param>
        public static async ETVoid Init_StallUpOtherEquip(this UIKnapsackNewComponent self)
        {
            G2C_BaiTanLookLookResponse g2C_BaiTanLookLook = (G2C_BaiTanLookLookResponse)await SessionComponent.Instance.Session.Call(new C2G_BaiTanLookLookRequest
            {
                BaiTanInstanceId = self.otherRole.Id
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
                    if (self.StallUpOtherComponent.StallUpItemDic.TryGetValue(item.ItemUUID, out KnapsackDataItem knapsackDataItem))
                    {
                        knapsackDataItem.UUID = item.ItemUUID;
                        knapsackDataItem.ConfigId = item.ConfigId;
                        knapsackDataItem.PosInBackpackX = item.PosInBackpackX;
                        knapsackDataItem.PosInBackpackY = item.PosInBackpackY;
                        knapsackDataItem.X = item_Info.X;
                        knapsackDataItem.Y = item_Info.Y;
                        knapsackDataItem.SetProperValue(E_ItemValue.Stall_BuyPrice, item.Price);
                        knapsackDataItem.SetProperValue(E_ItemValue.Stall_BuyMoJingPrice, item.Price2);
                        self.AddItem(knapsackDataItem, type: E_Grid_Type.Stallup_OtherPlayer);
                    }
                }
            }

        }

        /// <summary>
        /// 清理摊位
        /// </summary>
        public static void CleanStallUpOther(this UIKnapsackNewComponent self)
        {
            if (self.stallUpCollector)
            {
                self.GetKnapsackGrid(ref self.grids, ref self.LENGTH_X, ref self.LENGTH_Y, E_Grid_Type.Stallup_OtherPlayer);
                for (int i = 0; i < UIKnapsackNewComponent.LENGTH_StallUp_Other_Y; i++)
                {
                    for (int j = 0; j < UIKnapsackNewComponent.LENGTH_StallUp_Other_X; j++)
                    {
                        KnapsackGrid grid = self.grids[j][i];
                        if (grid.IsOccupy)
                        {
                            grid.Data.ItemData.Dispose();
                            self.RemoveItem(grid.Data, true);
                        }
                    }

                }

                foreach (var item in self.StallUpOtherComponent.StallUpItemDic.Values)
                {
                    item.Dispose();
                }
                self.StallUpOtherComponent.StallUpItemDic.Clear();

                GameObject.Destroy(self.stallUpCollector.gameObject);
                self.stallUpCollector = null;
                self.isOpenStallUpOther = false;
            }

        }
    }
}
