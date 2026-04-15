using ETModel;
using ETModel.EventType;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Threading.Tasks;
using System.Linq;

namespace ETHotfix
{
    [Timer(TimerType.ItemValidTime)]
    public class ItemValidTimeTimer : ATimer<ItemValidTimeComponent>
    {
        public override void Run(ItemValidTimeComponent self)
        {
            ItemValidTimeComponentSystem.DeleteSelf(self.Parent, "到期删除").Coroutine();
            self.Parent.RemoveCustomComponent<ItemValidTimeComponent>();
        }
    }

    [EventMethod(typeof(ItemValidTimeComponent), EventSystemType.INIT)]
    public class ItemValidTimeComponentEventOnInit : ITEventMethodOnInit<ItemValidTimeComponent>
    {
        public void OnInit(ItemValidTimeComponent b_Component)
        {
            long expiryTime = b_Component.Parent.GetProp(EItemValue.ValidTime);
            if (expiryTime == 0)
            {
                b_Component.TimerId = 0;
                Log.Error($"这个物品不会过期:({b_Component.Parent.ToLogString()})");
                return;
            }
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewOnceTimer(1000 * expiryTime, TimerType.ItemValidTime, b_Component);
        }
    }

    [EventMethod(typeof(ItemValidTimeComponent), EventSystemType.DISPOSE)]
    public class ItemValidTimeComponentEventOnDispose : ITEventMethodOnDispose<ItemValidTimeComponent>
    {
        public override void OnDispose(ItemValidTimeComponent b_Component)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref b_Component.TimerId);
        }
    }

    #region Event
    /// <summary>
    /// 装备物品
    /// </summary>
    [EventMethod("EquipItem")]
    public class EquipItem_AddItemValidTimeComponent : ITEventMethodOnRun<EquipItem>
    {
        public void OnRun(EquipItem args)
        {
            if (EquipmentComponentSystem.IsTempSlot(args.position)) return;

            if (args.item.GetProp(EItemValue.ValidTime) > 0)
            {
                if (args.item.GetCustomComponent<ItemValidTimeComponent>() == null)
                {
                    args.item.AddCustomComponent<ItemValidTimeComponent>();
                }
            }
        }
    }

    /// <summary>
    /// 物品进入背包
    /// </summary>
    [EventMethod("BackpackAddItem")]
    public class BackpackAddItem_AddItemValidTimeComponent : ITEventMethodOnRun<BackpackAddItem>
    {
        public void OnRun(BackpackAddItem args)
        {
            if (args.item.GetProp(EItemValue.ValidTime) > 0)
            {
                if (args.item.GetCustomComponent<ItemValidTimeComponent>() == null)
                {
                    args.item.AddCustomComponent<ItemValidTimeComponent>();
                }
            }
        }
    }

    /// <summary>
    /// 玩家上线准备完成
    /// </summary>
    [EventMethod("PlayerReadyComplete")]
    public class PlayerReadyComplete_AddItemValidTimeComponent : ITEventMethodOnRun<PlayerReadyComplete>
    {
        public void OnRun(PlayerReadyComplete args)
        {
            // 背包中的物品添加ItemExpiryTimeComponent
            BackpackComponent backpack = args.player.GetCustomComponent<BackpackComponent>();
            var items = backpack.Where(p => p.GetProp(EItemValue.ValidTime) > 0);
            foreach (var item in items)
            {
                item.AddCustomComponent<ItemValidTimeComponent>();
            }

            // 装备栏中的物品添加ItemExpiryTimeComponent
            EquipmentComponent equipment = args.player.GetCustomComponent<EquipmentComponent>();
            items = equipment.Where(p => p.GetProp(EItemValue.ValidTime) > 0);
            foreach (var item in items)
            {
                item.AddCustomComponent<ItemValidTimeComponent>();
            }
        }
    }
    #endregion

    public static class ItemValidTimeComponentSystem
    {
        public static async Task DeleteSelf(Item item, string log)
        {
            // 找到物品所在的玩家
            GameUser gameUser = Root.MainFactory.GetCustomComponent<GameUserComponent>().GetPlayer(item.data.UserId);
            if (gameUser == null)
            {
                Log.Error($"物品到期，删除自己时没找到GameUser ({item.ToLogString()})");
                return;
            }
            Player player = gameUser.Player;
            if (player == null) return;

            long instanceId = player.Id;
            long gameUserId = player.GameUserId;

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, gameUserId))
            {
                if (instanceId != player.Id) return;
                if (player.OnlineStatus != EOnlineStatus.Online) return;    // 现在无法删除
                switch (item.data.InComponent)
                {
                    case EItemInComponent.Backpack:
                        {
                            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
                            if (backpack.RemoveItem(item, log) == null)
                            {
                                Log.Error($"a:{player.UserId} r:{player.GameUserId} 无法从背包删除物品 ({item.ToLogString()})");
                                break;
                            }
                            if (item.Type == EItemType.Mounts)
                            {
                                EquipmentComponent equipment = player.GetCustomComponent<EquipmentComponent>();
                                equipment.TempSlotDict.Remove((EquipPosition)item.data.posId);
                            }
                            await SendExpiryItemMail(player, item);
                            item.DisposeDB(log);
                            break;
                        }
                    case EItemInComponent.Equipment:
                        {
                            EquipmentComponent equipment = player.GetCustomComponent<EquipmentComponent>();
                            if (equipment.UnloadEquipItem((EquipPosition)item.data.posId, log) == null)
                            {
                                Log.Error($"a:{player.UserId} r:{player.GameUserId} 无法从装备栏删除物品 ({item.ToLogString()})");
                                break;
                            }
                            if ((EquipPosition)item.data.posId == EquipPosition.Pet)
                            {
                                var gameplayer = player.GetCustomComponent<GamePlayer>();
                                gameplayer.PetsTurnEquipmentTuo(item.ItemUID, item.ConfigData.PetsId);
                            }
                            await SendExpiryItemMail(player, item);
                            item.DisposeDB(log);
                            break;
                        }
                    default:
                        // 物品进入背包、装备栏时，才检测时间有没有到期
                        break;
                }

            }
        }

        public static async Task SendExpiryItemMail(Player player, Item item)
        {
            // 发送邮件
            MailInfo mailInfo = new MailInfo();
            mailInfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
            mailInfo.MailName = "游戏道具到期提醒";
            mailInfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
            mailInfo.MailContent = $"<color=#FFFFFF00>缩进</color>您的游戏道具 {item.GetClientName()} 已经到期并被系统删除";
            mailInfo.MailState = 0;
            mailInfo.ReceiveOrNot = 0;
            mailInfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
            await MailSystem.SendMail(player.GameUserId, mailInfo, player.GameAreaId);
        }
    }
}
