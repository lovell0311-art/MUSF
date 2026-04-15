using ETModel;
using ETModel.EventType;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Threading.Tasks;
using System.Linq;

namespace ETHotfix
{
    [Timer(TimerType.ItemTimeLimit)]
    public class ItemTimeLimitTimer : ATimer<ItemTimeLimitComponent>
    {
        public override void Run(ItemTimeLimitComponent self)
        {
            ItemTimeLimitComponentSystem.DeleteSelf(self.Parent, "到期删除").Coroutine();
            self.Parent.RemoveCustomComponent<ItemTimeLimitComponent>();
       

        }
    }

    [EventMethod(typeof(ItemTimeLimitComponent), EventSystemType.INIT)]
    public class ItemTimeLimitComponentEventOnInit : ITEventMethodOnInit<ItemTimeLimitComponent>
    {
        public void OnInit(ItemTimeLimitComponent b_Component)
        {
            long expiryTime = b_Component.Parent.GetProp(EItemValue.TimeLimit);
            if (expiryTime == 0)
            {
                b_Component.TimerId = 0;
                Log.Error($"这个物品不会过期:({b_Component.Parent.ToLogString()})");
                return;
            }
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewOnceTimer(1000 * expiryTime, TimerType.ItemTimeLimit, b_Component);
        }
    }

    [EventMethod(typeof(ItemTimeLimitComponent), EventSystemType.DISPOSE)]
    public class ItemTimeLimitComponentEventOnDispose : ITEventMethodOnDispose<ItemTimeLimitComponent>
    {
        public override void OnDispose(ItemTimeLimitComponent b_Component)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref b_Component.TimerId);
        }
    }

    public static class ItemTimeLimitComponentSystem
    {
        public static async Task DeleteSelf(Item item,string log)
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

            using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, gameUserId))
            {
                if (instanceId != player.Id) return;
                if (player.OnlineStatus != EOnlineStatus.Online) return;    // 现在无法删除
                switch (item.data.InComponent)
                {
                    case EItemInComponent.Backpack:
                        {
                            long itemUid = 0;
                            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
                            if(backpack.RemoveItem(item, log) == null)
                            {
                                Log.Error($"a:{player.UserId} r:{player.GameUserId} 无法从背包删除物品 ({item.ToLogString()})");
                                break;
                            }
                            await SendExpiryItemMail(player, item);
                            itemUid = item.ItemUID;
                            item.DisposeDB(log);

                            // 通知客户端，删除小地图icon
                            G2C_CangBaoTuPosClose_notice g2c_CangBaoTuPosClose_notice = new G2C_CangBaoTuPosClose_notice();
                            g2c_CangBaoTuPosClose_notice.Id = itemUid;
                            player.Send(g2c_CangBaoTuPosClose_notice);
                            break;
                        }
                    //case EItemInComponent.Equipment:
                    //    {
                    //        EquipmentComponent equipment = player.GetCustomComponent<EquipmentComponent>();
                    //        if (equipment.DeleteEquipItemByPosition((EquipPosition)item.data.posId, log) == null)
                    //        {
                    //            Log.Error($"a:{player.UserId} r:{player.GameUserId} 无法从装备栏删除物品 ({item.ToLogString()})");
                    //            break;
                    //        }
                    //        await SendExpiryItemMail(player, item);
                    //        item.DisposeDB(log);
                    //        break;
                    //    }
                    default:
                        // 物品进入背包、装备栏时，才检测时间有没有到期
                        break;
                }

            }
        }

        public static async Task SendExpiryItemMail(Player player,Item item)
        {
            // 发送邮件
            MailInfo mailInfo = new MailInfo();
            mailInfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
            mailInfo.MailName = "藏宝图消失提醒";
            mailInfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
            mailInfo.MailContent = $"<color=#FFFFFF00>缩进</color>您的游戏道具 {item.GetClientName()} 开启的藏宝图已到期，系统已将道具移除";
            mailInfo.MailState = 0;
            mailInfo.ReceiveOrNot = 0;
            mailInfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
            await MailSystem.SendMail(player.GameUserId, mailInfo, player.GameAreaId);
        }
    }
}
