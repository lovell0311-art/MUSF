using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenMailRequestHandler : AMActorRpcHandler<C2G_OpenMailRequest, G2C_OpenMailResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenMailRequest b_Request, G2C_OpenMailResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenMailRequest b_Request, G2C_OpenMailResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                b_Reply(b_Response);
                return true;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                b_Reply(b_Response);
                return false;
            }

            var playermail = mPlayer.GetCustomComponent<PlayerMailComponent>();
            if (playermail == null)
            {
                playermail = mPlayer.AddCustomComponent<PlayerMailComponent>();
                await playermail.PlayerLoadMail(mAreaId);
            }
            int j = 300;//一次最多300个邮件
            foreach (var mail in playermail.mailInfos)
            {
                Mailinfo mailinfo = new Mailinfo();
                mailinfo.MailId = mail.Value.MailId;
                mailinfo.MailName = mail.Value.MailName;
                mailinfo.MailValidTime = mail.Value.MailValidTime;
                mailinfo.MailContent = mail.Value.MailContent;
                mailinfo.MailAcceptanceTime = mail.Value.MailAcceptanceTime;
                mailinfo.MailState = mail.Value.MailState;
                mailinfo.ReceiveOrNot = mail.Value.ReceiveOrNot;
                for (int i = 0; i < mail.Value.MailEnclosure.Count; i++)
                {
                    Iteminfo iteminfo = new Iteminfo();
                    iteminfo.ItemConfigID = mail.Value.MailEnclosure[i].ItemConfigID;
                    iteminfo.ItemID = mail.Value.MailEnclosure[i].ItemID;
                    iteminfo.ItemCnt = mail.Value.MailEnclosure[i].CreateAttr.Quantity;
                    mailinfo.MailEnclosure.Add(iteminfo);
                }
                b_Response.MailList.Add(mailinfo);
                j--;
                if (j == 0) break;
            }

            b_Reply(b_Response);
            return true;
        }
    }
}