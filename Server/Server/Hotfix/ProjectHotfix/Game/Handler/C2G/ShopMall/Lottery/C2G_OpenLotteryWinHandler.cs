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
    public class C2G_OpenLotteryWinHandler : AMActorRpcHandler<C2G_OpenLotteryWin, G2C_OpenLotteryWin>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenLotteryWin b_Request, G2C_OpenLotteryWin b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }


        protected override async Task<bool> Run(C2G_OpenLotteryWin b_Request, G2C_OpenLotteryWin b_Response, Action<IMessage> b_Reply)
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


            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDic = readConfig.GetJson<Lottery_ItemInfoConfigJson>().JsonDic;

            foreach(var config in jsonDic.Values)
            {
                b_Response.AllGiftInfo.Add(new LotteryGiftInfo()
                {
                    Id = config.Id,
                    ItemIcon = config.IconName,
                    ItemName = config.ItemName,
                });
            }

            LotteryComponent lottery = player.GetCustomComponent<LotteryComponent>();
            b_Response.TotalCount = lottery.Data.TotalCount;
            b_Response.TotalCountMax = ConstLottery.LotteryCountMax;
            b_Reply(b_Response);
            return true;
        }
    }
}