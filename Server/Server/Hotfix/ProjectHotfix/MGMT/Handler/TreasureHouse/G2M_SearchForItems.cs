using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_SearchForItemsHandler : AMActorRpcHandler<G2M_SearchForItems, M2G_SearchForItems>
    {
        protected override async Task<bool> Run(G2M_SearchForItems b_Request, M2G_SearchForItems b_Response, Action<IMessage> b_Reply)
        {
            var component = Root.MainFactory.GetCustomComponent<MGMTTreasureHouse>();
            if (component == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                b_Reply(b_Response);
                return false;
            }

            int mAreaId = (int)(b_Request.AppendData >> 16);

            var ItemList = component.ConditionsGetItem(b_Request.MaxType, b_Request.Name, b_Request.Class, b_Request.Excellent, b_Request.Enhance, b_Request.Readdition);
            if (ItemList != null)
            {
                List<THItemInfo>  ItemList1 = new List<THItemInfo>();
                if (b_Request.SortType == 1)
                    ItemList1 = ItemList.OrderBy(a => a.Price).ToList();//升序
                else if(b_Request.SortType == 0)
                    ItemList1 = ItemList.OrderByDescending(a => a.Price).ToList();//降序

                if (component.CreatePlayerItemList(b_Request.GameUserID, ItemList1))
                {
                    var ShenList = component.GetPlayerPage(b_Request.GameUserID, 1, out int MasterPage);
                    if (ShenList != null)
                    {
                        b_Response.Page = MasterPage;
                        foreach (var Item in ShenList)
                        {
                            GMTreasureHouseItemInfo gMTreasureHouseItemInfo = new GMTreasureHouseItemInfo();
                            gMTreasureHouseItemInfo.UserID = Item.UserID;
                            gMTreasureHouseItemInfo.Uid = Item.Uid;
                            gMTreasureHouseItemInfo.Name = Item.Name;
                            gMTreasureHouseItemInfo.Class = Item.Class;
                            gMTreasureHouseItemInfo.Excellent = Item.Excellent;
                            gMTreasureHouseItemInfo.Enhance = Item.Enhance;
                            gMTreasureHouseItemInfo.Readdition = Item.Readdition;
                            gMTreasureHouseItemInfo.Price = Item.Price;
                            gMTreasureHouseItemInfo.AreaId = Item.mAreaId;
                            gMTreasureHouseItemInfo.ConfigID = Item.ConfigId;
                            gMTreasureHouseItemInfo.EndTime = Item.ListingTime;
                            gMTreasureHouseItemInfo.Cnt = Item.Cnt;
                            b_Response.ItemList.Add(gMTreasureHouseItemInfo);
                        }
                        b_Reply(b_Response);
                        return true;
                    }
                    else
                    {
                        b_Response.Page = 1;
                        b_Reply(b_Response);
                        return true;
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3308);
                    b_Reply(b_Response);
                    return false;
                }
            }
            b_Response.Page = 1;
            //b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3307);
            b_Reply(b_Response);
            return true;
        }
    }
}