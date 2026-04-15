using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_DisableTheAllianceCacheHandler : AMActorRpcHandler<C2G_DisableTheAllianceCache, G2C_DisableTheAllianceCache>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_DisableTheAllianceCache b_Request, G2C_DisableTheAllianceCache b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_DisableTheAllianceCache b_Request, G2C_DisableTheAllianceCache b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_AccountNotExists;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }

            PlayerWarAllianceComponent playerWarAllianceComponent = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (playerWarAllianceComponent == null || playerWarAllianceComponent.WarAllianceID != 0)
            {
                b_Reply(b_Response);
                return true;
            }

            G2M_DisableTheAllianceCache g2M_DisableTheAllianceCache = new G2M_DisableTheAllianceCache();
            g2M_DisableTheAllianceCache.AppendData = b_Request.AppendData;
            g2M_DisableTheAllianceCache.GameUserId = mPlayer.GameUserId;
            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_DisableTheAllianceCache) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;//Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2502);
                b_Reply(b_Response);
                return false;
            }
            else
            {
                b_Reply(b_Response);
                return true;
            }
        }
    }
}