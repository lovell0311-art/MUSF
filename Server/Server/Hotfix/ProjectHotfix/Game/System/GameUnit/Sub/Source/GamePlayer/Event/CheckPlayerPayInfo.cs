using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using ETModel.EventType;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;

namespace ETHotfix
{

    [EventMethod("PlayerReadyComplete")]
    public class PlayerCheckPayInfo : ITEventMethodOnRun<PlayerReadyComplete>
    {
        public void OnRun(PlayerReadyComplete args)
        {
            CheckPayInfo(args.player).Coroutine();
        }

        private async Task CheckPayInfo(Player player)
        {
            long instanceId = player.Id;
            long gameUserId = player.GameUserId;

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, gameUserId))
            {
                if (instanceId != player.Id) return;
                if (player.OnlineStatus != EOnlineStatus.Online) return;    // 现在无法添加

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, player.GameAreaId);

                var PlayerShop = player.GetCustomComponent<PlayerShopMallComponent>();
                var PayInfo = await dBProxy2.Query<DBPlayerPayOrderInfo>(P => P.GUid == player.UserId && P.Success == false && P.SuccessTime != 0 && P.Effective == true);
                if (PayInfo != null && PayInfo.Count > 0)
                {
                    foreach (var item in PayInfo)
                    {
                        DBPlayerPayOrderInfo dBPlayerPayOrderInfo = (DBPlayerPayOrderInfo)item;

                        //dBPlayerPayOrderInfo.SuccessTime = Help_TimeHelper.GetNowSecond();
                        //dBPlayerPayOrderInfo.Success = true;
                        //await dBProxy2.Save(dBPlayerPayOrderInfo);

                        await PlayerShop.SetPayInfo(dBPlayerPayOrderInfo.App_Ordef_id, dBPlayerPayOrderInfo.Money, dBPlayerPayOrderInfo.Uid);
                    }
                }
            }
        }
    }
}
