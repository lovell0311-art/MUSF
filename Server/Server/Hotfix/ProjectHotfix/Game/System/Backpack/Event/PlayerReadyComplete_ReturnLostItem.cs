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
    public class PlayerReadyComplete_ReturnLostItem : ITEventMethodOnRun<PlayerReadyComplete>
    {
        public void OnRun(PlayerReadyComplete args)
        {
            ReturnLostItem(args.player).Coroutine();
        }

        private async Task ReturnLostItem(Player player)
        {
            DataCacheManageComponent dataCacheManage = player.GetCustomComponent<DataCacheManageComponent>();
            var dbItemDataCache = dataCacheManage.Get<DBItemData>();
            List<DBItemData> dbItemDataList = dbItemDataCache.DataQuery(item => item.InComponent == EItemInComponent.Lost && item.GameUserId == player.GameUserId && item.IsDispose == 0);

            if (dbItemDataList.Count == 0) return;  // 没有丢失物品

            long instanceId = player.Id;
            long gameUserId = player.GameUserId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId,gameUserId))
            {
                if (instanceId != player.Id) return;    // 不是原来的对象
                if (player.OnlineStatus != EOnlineStatus.Online) return;    // 玩家正在走下线流程

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, player.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(player.GameAreaId);

                MailInfo mailInfo = null;
                async Task SendMail()
                {
                    // 发送邮件
                    mailInfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                    mailInfo.MailName = "丢失的游戏物品";
                    mailInfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                    mailInfo.MailContent = "<color=#FFFFFF00>缩进</color>我们的游戏系统检测到您账户中存在丢失的游戏物品。非常抱歉给您带来了不便。\r\n<color=#FFFFFF00>缩进</color>为了确保您的游戏体验不受影响，已经通过邮件方式恢复了这些物品。\r\n\r\n<color=#FFFFFF00>缩进</color>祝您游戏愉快！";
                    mailInfo.MailState = 0;
                    mailInfo.ReceiveOrNot = 0;
                    mailInfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    await MailSystem.SendMail(player.GameUserId, mailInfo, player.GameAreaId);
                }

                foreach (DBItemData dbItemData in dbItemDataList)
                {
                    if (mailInfo == null) mailInfo = new MailInfo();
                    dbItemData.InComponent = EItemInComponent.Mail;
                    dbItemData.GameUserId = player.GameUserId;
                    dbItemData.posX = 0;
                    dbItemData.posY = 0;
                    dbItemData.posId = 0;
                    await dbItemData.SaveDBNow();
                    // 有锁，不需要担心后面的数据在预想之外
                    dbItemDataCache.DataRemove(dbItemData.Id);

                    if (!dbItemData.PropertyData.TryGetValue((int)EItemValue.Quantity, out int quantity))
                    {
                        quantity = 1;
                    }


                    mailInfo.MailEnclosure.Add(dbItemData.ToMailItem());

                    if(mailInfo.MailEnclosure.Count >= 6)
                    {
                        // 一个邮件最多可以添加 6 个附件
                        await SendMail();
                        mailInfo = null;
                    }
                }
                if(mailInfo != null)
                {
                    await SendMail();
                    mailInfo = null;
                }
            }
        }
    }
}
