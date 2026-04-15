using Codice.CM.Common;
using ETModel;
using System;
using System.Linq;
using UnityEngine;
using UwaProjScan.Tools.ExcelDataReader.Log.Logger;
using static UnityEditor.Progress;

namespace ETHotfix
{
    public partial class UITreasureHouseComponent
    {
        /// <summary>
        /// 打开藏宝阁
        /// </summary>
        /// <returns></returns>
        public async ETVoid OpenTreasureHouse(int maxType,int minType)
        {
            UITreasureHouseData.treasureHouseItems.Clear();
            G2C_OpenTreasureHouse g2C_OpenTreasure = (G2C_OpenTreasureHouse)await SessionComponent.Instance.Session.Call(new C2G_OpenTreasureHouse()
            {
                MaxType = maxType,
                MinType = minType
            });
            if (g2C_OpenTreasure.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenTreasure.Error.GetTipInfo()); 
                CurAllPage = 1;
                SetCurPage(1);
            }
            else
            {
                for (int i = 0, length = g2C_OpenTreasure.ItemList.Count; i < length; i++)
                {
                    UITreasureHouseData.treasureHouseItems.Add(g2C_OpenTreasure.ItemList[i]);
                }

                CurAllPage = g2C_OpenTreasure.Page;
                SetCurPage(1);
            }
            if (UITreasureHouseData.treasureHouseItems.Count == 0)
            {
                //uICircular_Item.Items = null;
                lastClickItemInfo = null;
            }
            else
            {
                lastClickItemInfo = UITreasureHouseData.treasureHouseItems.First();
                GetTreasureHouseItemInfo(lastClickItemInfo).Coroutine();
            }
            ModelHide();
            uICircular_Item.Items = UITreasureHouseData.treasureHouseItems;
        }
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="filtrate"></param>
        /// <returns></returns>
        public async ETVoid SearchForItems(FiltrateData filtrate)
        {
            UITreasureHouseData.treasureHouseItems.Clear();
            //Log.DebugGreen($"搜索物品名:{filtrate.Name}类别:{GetRoleType()}职业:{filtrate.RoleClass}卓越:{filtrate.Excellent}强化:{filtrate.Enhance}:套装:{filtrate.Readdition}顺序:{filtrate.SortType}");
            G2C_SearchForItems g2C_SearchFor = (G2C_SearchForItems)await SessionComponent.Instance.Session.Call(new C2G_SearchForItems()
            {
                MaxType = filtrate.Page,
                Name = filtrate.Name,
                Class = GetRoleType(),
                Excellent = filtrate.Excellent,
                Enhance = filtrate.Enhance,
                Readdition = filtrate.Readdition,
                SortType = filtrate.SortType,
                Type = pageType
            });
            if (g2C_SearchFor.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SearchFor.Error.GetTipInfo());
            }
            else
            {
                for (int i = 0, length = g2C_SearchFor.ItemList.Count; i < length; i++)
                {
                    UITreasureHouseData.treasureHouseItems.Add(g2C_SearchFor.ItemList[i]);
                }
            }
            if (UITreasureHouseData.treasureHouseItems.Count == 0)
            {
                //uICircular_Item.Items = null;
                lastClickItemInfo = null;
            }
            else
            {
                lastClickItemInfo = UITreasureHouseData.treasureHouseItems.First();
                GetTreasureHouseItemInfo(lastClickItemInfo).Coroutine();
            }
            ModelHide();
            uICircular_Item.Items = UITreasureHouseData.treasureHouseItems;
            CurAllPage = g2C_SearchFor.Page;
            SetCurPage(1);

            string GetRoleType() => filtrate.RoleClass switch
            {
                "1" => "{1:0}",
                "2" => "{2:0}",
                "3" => "{3:0}",
                "4" => "{4:0}",
                "5" => "{5:0}",
                "6" => "{6:0}",
                "7" => "{7:0}",
                "8" => "{8:0}",
                "9" => "{9:0}",
                "10" => "{10:0}",
                "11" => "{11:0}",
                "12" => "{12:0}",
                "13" => "{13:0}",
                _ => string.Empty
            };
        }
        /// <summary>
        /// 翻页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async ETVoid NextPage(int page,int type)
        {
            UITreasureHouseData.treasureHouseItems.Clear();
            G2C_NextPage g2C_NextPage = (G2C_NextPage)await SessionComponent.Instance.Session.Call(new C2G_NextPage()
            {
                Page = page,
                Type = type
            });
            if (g2C_NextPage.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_NextPage.Error.GetTipInfo());
            }
            else
            {
                for (int i = 0,length = g2C_NextPage.ItemList.Count; i < length; i++)
                {
                    UITreasureHouseData.treasureHouseItems.Add(g2C_NextPage.ItemList[i]);
                }
            }
            if (UITreasureHouseData.treasureHouseItems.Count == 0)
            {
                //uICircular_Item.Items = null;
                lastClickItemInfo = null;
            }
            else
            {
                lastClickItemInfo = UITreasureHouseData.treasureHouseItems.First();
                GetTreasureHouseItemInfo(lastClickItemInfo).Coroutine();
            }
            ModelHide();
            uICircular_Item.Items = UITreasureHouseData.treasureHouseItems;
            CurAllPage = g2C_NextPage.Page;
            SetCurPage(page);
        }
        /// <summary>
        /// 查看物品信息
        /// </summary>
        /// <param name="Uid"></param>
        /// <param name="UserId"></param>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        public async ETVoid GetTreasureHouseItemInfo(TreasureHouseItemInfo houseItemInfo)
        {
            if (houseItemInfo == null) return;
            G2C_GetTreasureHouseItemInfo g2C_GetTreasureHouse = (G2C_GetTreasureHouseItemInfo)await SessionComponent.Instance.Session.Call(new C2G_GetTreasureHouseItemInfo()
            {
                Uid = houseItemInfo.Uid,
                UserID = houseItemInfo.UserID,
                AreaId = houseItemInfo.AreaId
            });
            if(g2C_GetTreasureHouse.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetTreasureHouse.Error.GetTipInfo());
            }
        }
        /// <summary>
        /// 购买
        /// </summary>
        /// <param name="houseItemInfo"></param>
        /// <returns></returns>
        public async ETVoid BuyTreasureHouseItemInfo(TreasureHouseItemInfo houseItemInfo)
        {
            G2C_BuyTreasureHouseItemInfo buyTreasure = (G2C_BuyTreasureHouseItemInfo)await SessionComponent.Instance.Session.Call(new C2G_BuyTreasureHouseItemInfo()
            {
                Uid = houseItemInfo.Uid,
                UserID = houseItemInfo.UserID,
                AreaId = houseItemInfo.AreaId
            });
            if (buyTreasure.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, buyTreasure.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "购买成功！");
                lastClickItem.transform.Find("yigoumai").gameObject.SetActive(true);
                SetGoldCount();
            }
        }
        /// <summary>
        /// 打开寄售
        /// </summary>
        /// <returns></returns>
        public async ETVoid OpenConsign()
        {
            UITreasureHouseData.treasureHouseItems.Clear();
            RecordsList.Clear();
            G2C_OpenConsign openConsign = (G2C_OpenConsign)await SessionComponent.Instance.Session.Call(new C2G_OpenConsign(){  });
            if (openConsign.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, openConsign.Error.GetTipInfo());
            }
            else
            {
                for (int i = 0, length = openConsign.ItemList.Count; i < length; i++)
                {
                    UITreasureHouseData.treasureHouseItems.Add(openConsign.ItemList[i]);
                }
                for (int i = 0, length = openConsign.Records.Count; i < length; i++)
                {
                    RecordsList.Add(openConsign.Records[i]);
                }

                uICircular_Records.Items = RecordsList;
                CurAllPage = openConsign.Page;
                SetCurPage(1);
                SetGoldCount();
            }
            if (UITreasureHouseData.treasureHouseItems.Count == 0)
            {
                //uICircular_Item.Items = null;
                lastClickItemInfo = null;
            }
            else
            {
                lastClickItemInfo = UITreasureHouseData.treasureHouseItems.First();
            }
            ModelHide();
            uICircular_Item.Items = UITreasureHouseData.treasureHouseItems;
        }
        /// <summary>
        /// 下架
        /// </summary>
        /// <param name="houseItemInfo"></param>
        /// <returns></returns>
        public async ETVoid RemovedItems(long uid)
        {
            G2C_RemovedItems removedItems = (G2C_RemovedItems)await SessionComponent.Instance.Session.Call(new C2G_RemovedItems()
            {
                Uid = uid,
                Page = CurPage
            });
            if (removedItems.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, removedItems.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "下架成功！");
                lastClickItem.transform.Find("yixiajia").gameObject.SetActive(true);
            }
        }
        /// <summary>
        /// 清除角色缓存
        /// </summary>
        /// <param name="houseItemInfo"></param>
        /// <returns></returns>
        public async ETVoid DeletePlayerTreasureHouse()
        {
            G2C_DeletePlayerTreasureHouse deletePlayerTreasureHouse = (G2C_DeletePlayerTreasureHouse)await SessionComponent.Instance.Session.Call(new C2G_DeletePlayerTreasureHouse(){});
            if (deletePlayerTreasureHouse.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, deletePlayerTreasureHouse.Error.GetTipInfo());
            }
        }
    }

}
