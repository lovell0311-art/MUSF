using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class GM2Game_SendMailHandler : AMRpcHandler<GM2Game_SendMail, Game2GM_SendMail>
    {
        protected override async Task<bool> CodeAsync(Session session, GM2Game_SendMail b_Request, Game2GM_SendMail b_Response, Action<IMessage> b_Reply)
        {
            long gameUserId = b_Request.GameUserId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, gameUserId))
            {
                PlayerManageComponent playerManage = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();
                Player player = playerManage.Get(b_Request.ZoneId, b_Request.GameUserId);
                if(player == null)
                {
                    b_Response.Error = ErrorCode.ERR_AccountNotOffline;
                    b_Reply(b_Response);
                    return true;
                }
                if(player.OnlineStatus != EOnlineStatus.Online)
                {
                    b_Response.Error = ErrorCode.ERR_AccountNotOffline;
                    b_Reply(b_Response);
                    return true;
                }

                MailInfo mailinfo = new MailInfo();
                mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(b_Request.ZoneId);
                mailinfo.MailName = b_Request.Name;
                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                mailinfo.MailContent = b_Request.Content;
                mailinfo.MailState = 0;
                mailinfo.ReceiveOrNot = 0;
                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
    
                mailinfo.MailEnclosure.AddRange(MongoHelper.FromJson<List<MailItem>>(b_Request.MailItemsBson));

                await MailSystem.SendMail(gameUserId, mailinfo, b_Request.ZoneId);
            }

            b_Reply(b_Response);
            return true;
        }
    }
}