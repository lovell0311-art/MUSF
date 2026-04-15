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
    public class C2G_MailDeleteItemRequestHandler : AMActorRpcHandler<C2G_MailDeleteItemRequest, G2C_MailDeleteItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MailDeleteItemRequest b_Request, G2C_MailDeleteItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_MailDeleteItemRequest b_Request, G2C_MailDeleteItemResponse b_Response, Action<IMessage> b_Reply)
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
            if (playermail != null)
            {
                for (int i = 0; i < b_Request.MailId.Count; i++)
                {
                    if (playermail.mailInfos.ContainsKey(b_Request.MailId[i]))
                        playermail.DeleteMail(b_Request.MailId[i], mAreaId);
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}