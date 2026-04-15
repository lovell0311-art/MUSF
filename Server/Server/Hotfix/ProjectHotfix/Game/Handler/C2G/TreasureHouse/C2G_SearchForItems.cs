using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Xml.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SearchForItemsHandler : AMActorRpcHandler<C2G_SearchForItems, G2C_SearchForItems>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SearchForItems b_Request, G2C_SearchForItems b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SearchForItems b_Request, G2C_SearchForItems b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }
            var TH = mPlayer.GetCustomComponent<TreasureHouseComponent>();
            if (TH != null)
            {
                if (b_Request.Type == 0)
                {
                    G2M_SearchForItems g2M_SearchForItems = new G2M_SearchForItems();
                    g2M_SearchForItems.AppendData = b_Request.AppendData;
                    g2M_SearchForItems.Name = b_Request.Name;
                    g2M_SearchForItems.Class = b_Request.Class;
                    g2M_SearchForItems.Excellent = b_Request.Excellent;
                    g2M_SearchForItems.Enhance = b_Request.Enhance;
                    g2M_SearchForItems.Readdition = b_Request.Readdition;
                    g2M_SearchForItems.MaxType = b_Request.MaxType;
                    g2M_SearchForItems.GameUserID = mPlayer.GameUserId;
                    g2M_SearchForItems.SortType = b_Request.SortType;
                    IResponse Message = await TH.SendGM(g2M_SearchForItems);
                    if (Message == null)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                        b_Reply(b_Response);
                        return false;
                    }
                    else if (Message.Error != 0)
                    {
                        b_Response.Error = Message.Error;
                        b_Reply(b_Response);
                        return false;
                    }
                    else
                    {
                        M2G_SearchForItems m2G_OpenTreasureHouse = Message as M2G_SearchForItems;
                        b_Response.Page = m2G_OpenTreasureHouse.Page;
                        foreach (var info in m2G_OpenTreasureHouse.ItemList)
                        {
                            TreasureHouseItemInfo treasureHouseItemInfo = new TreasureHouseItemInfo
                            {
                                Uid = info.Uid,
                                UserID = info.UserID,
                                Price = info.Price,
                                Enhance = info.Enhance,
                                Readdition = info.Readdition,
                                Excellent = info.Excellent,
                                Class = info.Class,
                                Name = info.Name,
                                AreaId = info.AreaId,
                                ConfigID = info.ConfigID,
                                Cnt = info.Cnt,
                            };

                            b_Response.ItemList.Add(treasureHouseItemInfo);
                        }
                        b_Reply(b_Response);
                        return true;
                    }
                }
                else
                {
                    var Item = TH.ConditionsGetItem(b_Request.Name, b_Request.Class, b_Request.Excellent, b_Request.Enhance, b_Request.Readdition);
                    b_Response.Page = 1;
                    foreach (var info in Item)
                    {
                        TreasureHouseItemInfo treasureHouseItemInfo = new TreasureHouseItemInfo
                        {
                            Uid = info.Uid,
                            UserID = info.UserID,
                            Price = info.Price,
                            Enhance = info.Enhance,
                            Readdition = info.Readdition,
                            Excellent = info.Excellent,
                            Class = info.Class,
                            Name = info.Name,
                            AreaId = info.mAreaId,
                            ConfigID = info.ConfigId,
                            EndTime = info.ListingTime
                        };

                        b_Response.ItemList.Add(treasureHouseItemInfo);
                    }
                    b_Reply(b_Response);
                    return true;
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}