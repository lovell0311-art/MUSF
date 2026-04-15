using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Realm)]
    public class R2G_GetServerLineInfoResponseHandler : AMActorRpcHandler<G2R_GetServerLineInfoRequest, R2G_GetServerLineInfoResponse>
    {
        protected override async Task<bool> Run(G2R_GetServerLineInfoRequest b_Request, R2G_GetServerLineInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            var AutoArer = Root.MainFactory.GetCustomComponent<AutoAreaComponent>();
            if (AutoArer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Reply(b_Response);
                return false;
            }

            foreach (var inf in AutoArer.keyValuePairs)
            {
                G2C_LoginSystemServerInfoMessage ServerInfo = new G2C_LoginSystemServerInfoMessage();
                ServerInfo.GameAreaId = inf.Value.Id;
                ServerInfo.GameAreaNickName = inf.Value.Name;
                ServerInfo.GameAreaType = 0;
                ServerInfo.IsGameAreaState = inf.Value.State;
                b_Response.GameAreaInfos.Add(ServerInfo);
            }

            b_Reply(b_Response);
            return true;
        }
    }
}