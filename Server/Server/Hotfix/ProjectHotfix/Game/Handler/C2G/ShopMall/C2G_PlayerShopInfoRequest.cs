using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Mrs.V20200910.Models;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PlayerShopInfoRequestHandler : AMActorRpcHandler<C2G_PlayerShopInfoRequest, G2C_PlayerShopInfoResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PlayerShopInfoRequest b_Request, G2C_PlayerShopInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PlayerShopInfoRequest b_Request, G2C_PlayerShopInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Reply(b_Response);
                return false;
            }
            var playerShop = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            if (playerShop == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();

            b_Response.Info = new PlayerShop();
            b_Response.Info.CurrentYuanbao = mGamePlayer.Player.Data.YuanbaoCoin;
            b_Response.Info.RechargeStatus = playerShop.dBPlayerShopMall.RechargeStatus;
            b_Response.Info.MinMCEndTime = playerShop.dBPlayerShopMall.MinMCEndTime;
            b_Response.Info.MaxMCEndTime = playerShop.dBPlayerShopMall.MaxMCEndTime;
            b_Response.Info.AccumulatedRecharge = playerShop.dBPlayerShopMall.AccumulatedRecharge;
            b_Response.Info.InSituCd = playerShop.dBPlayerShopMall.InSituCd;
            b_Response.Info.MiracleCoin = mGamePlayer.Data.MiracleCoin;
            b_Reply(b_Response);
            return true;
        }
    }
}