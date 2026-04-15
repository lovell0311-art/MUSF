using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Xml.Linq;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenConsignHandler : AMActorRpcHandler<C2G_OpenConsign, G2C_OpenConsign>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenConsign b_Request, G2C_OpenConsign b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenConsign b_Request, G2C_OpenConsign b_Response, Action<IMessage> b_Reply)
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
                var ItemList = TH.keyValuePairs;
                b_Response.Page = ItemList.Count;
                foreach (var T in ItemList.First().Value)
                {
                    TreasureHouseItemInfo treasureHouseItemInfo = new TreasureHouseItemInfo
                    {
                        Uid = T.Uid,
                        Cnt = T.Cnt,
                        Name = T.Name,
                        Price = T.Price,
                        Class = T.Class,
                        UserID = T.UserID,
                        AreaId = T.mAreaId,
                        EndTime = T.ListingTime,
                        Enhance = T.Enhance,
                        ConfigID = T.ConfigId,
                        Excellent = T.Excellent,
                        Readdition = T.Readdition,
                    };
                    b_Response.ItemList.Add(treasureHouseItemInfo);

                }

                foreach (var T in TH.tHRecords)
                {
                    TransactionRecord transactionRecord = new TransactionRecord
                    {
                        ItemName = T.ItemName,
                        Price = T.Price,
                        ActualPrice = T.ActualPrice,
                        Type = T.Type,
                    };
                    b_Response.Records.Add(transactionRecord);
                }

                b_Reply(b_Response);
                return true;
            }
            b_Response.Error = 3308;
            b_Reply(b_Response);
            return false;
        }
    }
}