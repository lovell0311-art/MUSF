using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Xml.Linq;
using TencentCloud.Ame.V20190916.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_NextPageHandler : AMActorRpcHandler<C2G_NextPage, G2C_NextPage>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_NextPage b_Request, G2C_NextPage b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_NextPage b_Request, G2C_NextPage b_Response, Action<IMessage> b_Reply)
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
                    G2M_NextPage g2M_NextPage = new G2M_NextPage
                    {
                        AppendData = b_Request.AppendData,
                        Page = b_Request.Page,
                        GameUserID = mPlayer.GameUserId
                    };
                    IResponse Message = await TH.SendGM(g2M_NextPage);
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
                        M2G_NextPage g2G_NextPage = Message as M2G_NextPage;
                        b_Response.Page = g2G_NextPage.Page;
                        foreach (var info in g2G_NextPage.ItemList)
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
                    if (TH.keyValuePairs.ContainsKey(b_Request.Page))
                    {
                        b_Response.Page = TH.keyValuePairs.Count;
                        foreach (var info in TH.keyValuePairs[b_Request.Page])
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
                                EndTime = info.ListingTime,
                                Cnt = info.Cnt,
                            };
                            b_Response.ItemList.Add(treasureHouseItemInfo);
                        }
                        b_Reply(b_Response);
                        return true;
                    }
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}