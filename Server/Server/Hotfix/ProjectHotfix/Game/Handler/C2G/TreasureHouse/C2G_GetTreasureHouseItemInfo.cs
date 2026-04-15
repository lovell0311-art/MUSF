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
    public class C2G_GetTreasureHouseItemInfoHandler : AMActorRpcHandler<C2G_GetTreasureHouseItemInfo, G2C_GetTreasureHouseItemInfo>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetTreasureHouseItemInfo b_Request, G2C_GetTreasureHouseItemInfo b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetTreasureHouseItemInfo b_Request, G2C_GetTreasureHouseItemInfo b_Response, Action<IMessage> b_Reply)
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
            if (TH == null) 
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                b_Reply(b_Response);
                return false;
            }
           
            var item =await TH.CreateFormDB(b_Request.Uid, b_Request.UserID, b_Request.AreaId);
            if (item != null)
            {
                G2C_SendTreasureHouseItemInfo g2C_SendTreasureHouseItemInfo = new G2C_SendTreasureHouseItemInfo();
                g2C_SendTreasureHouseItemInfo.AllProperty = item.ToItemAllProperty();
                mPlayer.Send(g2C_SendTreasureHouseItemInfo);
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3308);
                b_Reply(b_Response);
                return false;
            }
            b_Reply(b_Response);
            return true;
        }
    }
}