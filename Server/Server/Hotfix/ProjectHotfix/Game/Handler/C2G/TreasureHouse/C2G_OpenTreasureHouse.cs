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
    public class C2G_OpenTreasureHouseHandler : AMActorRpcHandler<C2G_OpenTreasureHouse, G2C_OpenTreasureHouse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenTreasureHouse b_Request, G2C_OpenTreasureHouse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenTreasureHouse b_Request, G2C_OpenTreasureHouse b_Response, Action<IMessage> b_Reply)
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
                G2M_OpenTreasureHouse g2M_OpenTreasureHouse = new G2M_OpenTreasureHouse();
                g2M_OpenTreasureHouse.AppendData = b_Request.AppendData;
                g2M_OpenTreasureHouse.MaxType = b_Request.MaxType;
                g2M_OpenTreasureHouse.MinType = b_Request.MinType;
                g2M_OpenTreasureHouse.GameUserID = mPlayer.GameUserId;
                IResponse Message = await TH.SendGM(g2M_OpenTreasureHouse);
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
                    M2G_OpenTreasureHouse m2G_OpenTreasureHouse = Message as M2G_OpenTreasureHouse;
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
                            EndTime = info.EndTime,
                            Cnt = info.Cnt,
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