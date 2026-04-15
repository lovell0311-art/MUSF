using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class G2G_SendFullServiceMailMessageHandler : AMHandler<G2G_SendFullServiceMailMessage>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, G2G_SendFullServiceMailMessage b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(G2G_SendFullServiceMailMessage b_Request)
        {
            List<MailItem> MailItems = Help_JsonSerializeHelper.DeSerialize<List<MailItem>>(b_Request.MailItems);
            MailInfo mailinfo = new MailInfo();
            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(b_Request.ZoneId);
            mailinfo.MailName = b_Request.Name;
            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
            mailinfo.MailContent = b_Request.Content;
            mailinfo.MailState = 0;
            mailinfo.ReceiveOrNot = 0;
            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
            mailinfo.MailEnclosure.AddRange(MailItems);

            MailSystem.SendFullServiceMail(b_Request.ZoneId, mailinfo).Coroutine();
            return true;
        }
    }
}