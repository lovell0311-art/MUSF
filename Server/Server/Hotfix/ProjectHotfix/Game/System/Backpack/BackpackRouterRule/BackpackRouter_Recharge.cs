using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using ETModel.EventType;

namespace ETHotfix
{
    [BackpackRouter(320414)]    // 充值
    public class BackpackRouter_Recharge : IBackpackRouterHandler
    {
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log)
        {
            // 先将物品锁住，添加到背包
            item.SetProp(EItemValue.IsLocking, 1);
            ItemRechargeHelper.UseRechargeItem(backpack.Parent, item).Coroutine();
        }
    }

    [EventMethod("PlayerReadyComplete")]
    public class PlayerReadyComplete_Recharge : ITEventMethodOnRun<PlayerReadyComplete>
    {
        public void OnRun(PlayerReadyComplete args)
        {
            // 先将物品锁住
            Player player = args.player;
            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
            var allItemDict = backpack.GetAllItemByConfigID(320414);
            if(allItemDict != null)
            {
                foreach (Item item in allItemDict.Values)
                {
                    item.SetProp(EItemValue.IsLocking, 1);
                    ItemRechargeHelper.UseRechargeItem(player, item).Coroutine();
                }
            }
        }
    }


    public static class ItemRechargeHelper
    {
        public static async Task UseRechargeItem(Player player,Item targetItem)
        {
            long playerInstanceId = player.Id;
            long gameUserId = player.GameUserId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, gameUserId))
            {
                if (playerInstanceId != player.Id) return;
                if (player.OnlineStatus != EOnlineStatus.Online) return;

                BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
                PlayerShopMallComponent playerShopMall = player.GetCustomComponent<PlayerShopMallComponent>();

                int money = targetItem.GetProp(EItemValue.Quantity);

                await playerShopMall.GMPayInfo(money);

                backpack.DeleteItem(targetItem, "充值已到账");
            }

        }
    }


}
