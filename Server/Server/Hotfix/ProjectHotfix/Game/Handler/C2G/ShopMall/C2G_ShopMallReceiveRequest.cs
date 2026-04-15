using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ShopMallReceiveRequestHandler : AMActorRpcHandler<C2G_ShopMallReceiveRequest, G2C_ShopMallReceiveResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ShopMallReceiveRequest b_Request, G2C_ShopMallReceiveResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ShopMallReceiveRequest b_Request, G2C_ShopMallReceiveResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色数据异常");
                b_Reply(b_Response);
                return false;
            }
            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (gameplayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
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

            if (TryGetFirstChargeType((PlayerShopQuotaType)b_Request.Type, out DeviationType deviationType))
            {
                int cost = PlayerShopQuota.GetPayValue((PlayerShopQuotaType)b_Request.Type);
                if (cost <= 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1608);
                    b_Reply(b_Response);
                    return false;
                }

                if (gameplayer.Player.Data.YuanbaoCoin < cost)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                    b_Response.Message = "魔晶不足";
                    b_Reply(b_Response);
                    return false;
                }

                playerShop.SetPageRecharge((int)deviationType, cost);
                playerShop.SetPlayerYuanBao(-cost, $"新人礼包购买消耗 quota:{(PlayerShopQuotaType)b_Request.Type} cost:{cost}");
                playerShop.FirstReceiveItem(deviationType);

                b_Reply(b_Response);
                return true;
            }
            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LimitedPurchase_RewardPropsConfigJson>().JsonDic;
            if (mJsonDic.TryGetValue(b_Request.Type, out var mItem) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1608);
                b_Reply(b_Response);
                return false;
            }

            if (mPlayer.Data.YuanbaoCoin < mItem.TypeId)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                b_Reply(b_Response);
                return false;
            }


            MailInfo mailinfo = new MailInfo();
            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
            mailinfo.MailName = "限时购买";
            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
            mailinfo.MailContent = "恭喜玩家获得道具";
            mailinfo.MailState = 0;
            mailinfo.ReceiveOrNot = 0;
            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;

            ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
            itemCreateAttr.Quantity = mItem.Quantity;
            MailItem mailItem = new MailItem();
            mailItem.ItemConfigID = mItem.ItemId;
            mailItem.ItemID = 0;
            mailItem.CreateAttr = itemCreateAttr;
            mailinfo.MailEnclosure.Add(mailItem);

            MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
            gameplayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -28, "购买现时礼包");
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
            dBProxy.Save(gameplayer.Player.Data).Coroutine();
            G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
            G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
            mBattleKVData.Key = (int)E_GameProperty.YuanbaoCoin;
            mBattleKVData.Value = gameplayer.GetNumerial(E_GameProperty.YuanbaoCoin);
            mChangeValue_notice.Info.Add(mBattleKVData);
            mPlayer.Send(mChangeValue_notice);
            b_Reply(b_Response);
            return true;
        }

        private static bool TryGetFirstChargeType(PlayerShopQuotaType quotaType, out DeviationType deviationType)
        {
            switch (quotaType)
            {
                case PlayerShopQuotaType.FirstTopUp:
                    deviationType = DeviationType.FirstCharge6;
                    return true;
                case PlayerShopQuotaType.SecondTopUp:
                    deviationType = DeviationType.FirstCharge38;
                    return true;
                case PlayerShopQuotaType.ThirdTopUp:
                    deviationType = DeviationType.FirstCharge68;
                    return true;
                case PlayerShopQuotaType.FourthlyTopUp:
                    deviationType = DeviationType.FirstCharge198;
                    return true;
                case PlayerShopQuotaType.FifthTopUp:
                    deviationType = DeviationType.FirstCharge288;
                    return true;
                default:
                    deviationType = default;
                    return false;
            }
        }
    }
}
