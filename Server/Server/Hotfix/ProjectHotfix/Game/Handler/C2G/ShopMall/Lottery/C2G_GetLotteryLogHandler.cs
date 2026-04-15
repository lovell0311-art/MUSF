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
    public class C2G_GetLotteryLogHandler : AMActorRpcHandler<C2G_GetLotteryLog, G2C_GetLotteryLog>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetLotteryLog b_Request, G2C_GetLotteryLog b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }


        protected override async Task<bool> Run(C2G_GetLotteryLog b_Request, G2C_GetLotteryLog b_Response, Action<IMessage> b_Reply)
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
            if(b_Request.Count <= 0)
            {
                b_Response.AllLog.Clear();
                b_Response.EndLogId = b_Request.EndLogId;
                b_Reply(b_Response);
                return false;
            }

            LotteryPoolComponent lotteryPool = Root.MainFactory.GetCustomComponent<LotteryPoolComponent>();

            List<LotteryLog> lotteryLogs = await lotteryPool.GetHistoryLotteryLog(player, b_Request.GameUserId, b_Request.EndLogId, b_Request.Count);

            if(lotteryLogs.Count != 0)
            {
                b_Response.AllLog.AddRange(lotteryLogs);
                b_Response.EndLogId = lotteryLogs[lotteryLogs.Count - 1].LogId;
            }
            else
            {
                b_Response.EndLogId = b_Request.EndLogId;
            }
            b_Reply(b_Response);
            return true;
        }
    }
}