using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Xml.Linq;
using TencentCloud.Hcm.V20181106.Models;
using Org.BouncyCastle.Asn1.Ocsp;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_DeletePlayerTreasureHouseHandler : AMActorRpcHandler<C2G_DeletePlayerTreasureHouse, G2C_DeletePlayerTreasureHouse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_DeletePlayerTreasureHouse b_Request, G2C_DeletePlayerTreasureHouse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_DeletePlayerTreasureHouse b_Request, G2C_DeletePlayerTreasureHouse b_Response, Action<IMessage> b_Reply)
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

            G2M_DeletePlayerTreasureHouse g2M_DeletePlayerTreasureHouse = new G2M_DeletePlayerTreasureHouse { GameUserID = mPlayer.GameUserId };
            mPlayer.GetSessionMGMT().Send(g2M_DeletePlayerTreasureHouse);
           
            b_Reply(b_Response);
            return true;
        }
    }
}