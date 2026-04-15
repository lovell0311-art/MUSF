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
    public class G2M_CheackWarAllianceNameRequestHandler : AMActorRpcHandler<G2M_CheackWarAllianceNameRequest, M2G_WCheackWarAllianceNameResponse>
    {
        protected override async Task<bool> Run(G2M_CheackWarAllianceNameRequest b_Request, M2G_WCheackWarAllianceNameResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2506); ;
                b_Reply(b_Response);
                return false;
            }
           
            foreach (int ID in Waralliancecomponent.DBID)
            {
                var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, ID);
                var WarAllianceInfo = await mDBProxy.Query<DBWarAllianceData>(p => p.DBWarAllianceName == b_Request.WarAllianceName && p.IsDisabled != 1);

                if (WarAllianceInfo.Count >= 1)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2519); 
                    b_Reply(b_Response);
                    return false;
                }

            }

            b_Reply(b_Response);
            return true;

        }
    }
}