using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_DisableTheAllianceCacheHandler : AMActorRpcHandler<G2M_DisableTheAllianceCache, M2G_DisableTheAllianceCache>
    {
        protected override async Task<bool> Run(G2M_DisableTheAllianceCache b_Request, M2G_DisableTheAllianceCache b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2502);
                b_Reply(b_Response);
                return false;
            }
            Waralliancecomponent.DleGameUser(b_Request.GameUserId);
            b_Reply(b_Response);
            return true;

        }
    }
}