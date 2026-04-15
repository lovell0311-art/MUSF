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
    public class G2M_NextPageHandler : AMActorRpcHandler<G2M_NextPage, M2G_NextPage>
    {
        protected override async Task<bool> Run(G2M_NextPage b_Request, M2G_NextPage b_Response, Action<IMessage> b_Reply)
        {
            var component = Root.MainFactory.GetCustomComponent<MGMTTreasureHouse>();
            if (component == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                b_Reply(b_Response);
                return false;
            }

            var ShenList = component.GetPlayerPage(b_Request.GameUserID, b_Request.Page, out int MasterPage);
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3309);
                b_Reply(b_Response);
                return false;
            }
        }
    }
}