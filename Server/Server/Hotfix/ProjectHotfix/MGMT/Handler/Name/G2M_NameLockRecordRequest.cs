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
    public class G2M_NameLockRecordRequestHandler : AMActorRpcHandler<G2M_NameLockRecordRequest, M2G_NameLockRecordResponse>
    {
        protected override async Task<bool> Run(G2M_NameLockRecordRequest b_Request, M2G_NameLockRecordResponse b_Response, Action<IMessage> b_Reply)
        {
            var NameLockComponent = Root.MainFactory.GetCustomComponent<NameLockComponent>();
            if (NameLockComponent == null)
            {
                b_Response.Error = 303;
                b_Reply(b_Response);
                return false;
            }

            if (NameLockComponent.strings.Contains(b_Request.Name))
            {
                b_Response.Error = 304;
                b_Reply(b_Response);
                return false;
            }

            NameLockComponent.strings.Add(b_Request.Name);
            
            b_Reply(b_Response);
            return true;

        }
    }
}