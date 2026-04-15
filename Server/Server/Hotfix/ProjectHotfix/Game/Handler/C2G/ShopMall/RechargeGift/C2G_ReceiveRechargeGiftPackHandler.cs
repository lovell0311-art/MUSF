using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Mrs.V20200910.Models;
using System.Linq;
using TencentCloud.Hcm.V20181106.Models;
using System.Net;
using MongoDB.Bson;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ReceiveRechargeGiftPackHandler : AMActorRpcHandler<C2G_ReceiveRechargeGiftPack, G2C_ReceiveRechargeGiftPack>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ReceiveRechargeGiftPack b_Request, G2C_ReceiveRechargeGiftPack b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }


        protected override async Task<bool> Run(C2G_ReceiveRechargeGiftPack b_Request, G2C_ReceiveRechargeGiftPack b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            Player player = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (player == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Reply(b_Response);
                return false;
            }
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();

            if (!readConfig.GetJson<CumulativeRecharge_TypeConfigJson>().JsonDic.TryGetValue(b_Request.ConfigId, out var config))
            {
                // 充值礼包不存在
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3400);
                b_Reply(b_Response);
                return false;
            }

            CumulativeRechargeComponent cumulativeRecharge = player.GetCustomComponent<CumulativeRechargeComponent>();
            if (config.Money > cumulativeRecharge.TotalAmount)
            {
                // 没达到领取条件
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3401);
                b_Reply(b_Response);
                return false;
            }

            if (player.Data.ReceivedCRGiftId.Contains(b_Request.ConfigId))
            {
                // 重复领取充值礼包
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3402);
                b_Reply(b_Response);
                return false;
            }
            GamePlayer gamePlayer = player.GetCustomComponent<GamePlayer>();
            CumulativeRechargeGiftComponent cumulativeRechargeGift = Root.MainFactory.GetCustomComponent<CumulativeRechargeGiftComponent>();

            var jsonDic = readConfig.GetJson<CumulativeRecharge_ItemInfoConfigJson>().JsonDic;

            List<int> configIds = cumulativeRechargeGift.GetAllItemInfoId(
                b_Request.ConfigId,
                b_Request.Id2,
                (E_GameOccupation)gamePlayer.Data.PlayerTypeId);
            if(configIds.Count == 0)
            {
                // 充值礼包不存在
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3400);
                b_Reply(b_Response);
                return false;
            }

            player.Data.ReceivedCRGiftId.Add(b_Request.ConfigId);
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(player.GameAreaId);
            mWriteDataComponent.Save(player.Data, dBProxy2).Coroutine();

            MailInfo mailInfo = new MailInfo();
            async Task SendMail()
            {
                // 发送邮件
                mailInfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                mailInfo.MailName = "累计充值奖励领取通知";
                mailInfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                mailInfo.MailContent = "<color=#FFFFFF00>缩进</color>您已成功领取了累计充值活动的丰厚奖励。以下是您所获得的道具:";
                mailInfo.MailState = 0;
                mailInfo.ReceiveOrNot = 0;
                mailInfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                await MailSystem.SendMail(player.GameUserId, mailInfo, player.GameAreaId);
            }

            foreach(int id in configIds)
            {
                if(!jsonDic.TryGetValue(id, out var info))
                {
                    player.PLog($"'CumulativeRecharge_ItemInfoConfig' 无法找到 ConfigId={id}");
                    Log.Error($"'CumulativeRecharge_ItemInfoConfig' 无法找到 ConfigId={id}");
                    continue;
                }
                if (mailInfo == null) mailInfo = new MailInfo();

                mailInfo.MailEnclosure.Add(new MailItem()
                {
                    ItemConfigID = info.ItemId,
                    ItemID = 0,
                    CreateAttr = info.ToItemCreateAttr(),
                });

                if (mailInfo.MailEnclosure.Count >= 6)
                {
                    // 一个邮件最多可以添加 6 个附件
                    await SendMail();
                    mailInfo = null;
                }
            }
            if (mailInfo != null)
            {
                await SendMail();
                mailInfo = null;
            }

            b_Reply(b_Response);
            return true;
        }
    }
}