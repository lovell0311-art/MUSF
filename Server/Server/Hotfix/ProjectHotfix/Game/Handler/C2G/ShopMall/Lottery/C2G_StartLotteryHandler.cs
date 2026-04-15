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
    public class C2G_StartLotteryHandler : AMActorRpcHandler<C2G_StartLottery, G2C_StartLottery>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_StartLottery b_Request, G2C_StartLottery b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }


        protected override async Task<bool> Run(C2G_StartLottery b_Request, G2C_StartLottery b_Response, Action<IMessage> b_Reply)
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

            int yuanbaoCoin = 0;

            switch(b_Request.Count)
            {
                case 1: yuanbaoCoin = ConstLottery.SingleDrawConsumption; break;
                case 10: yuanbaoCoin = ConstLottery.TenDrawConsumption; break;
                default:
                    {
                        // 参数错误，不支持的抽奖次数
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3500);
                        b_Reply(b_Response);
                        return false;
                    }
                    break;
            }
            if(player.Data.YuanbaoCoin < yuanbaoCoin)
            {
                // 魔晶不足
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3501);
                b_Reply(b_Response);
                return false;
            }
            GamePlayer gamePlayer = player.GetCustomComponent<GamePlayer>();
            gamePlayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -yuanbaoCoin, "抽奖");
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)player.GameAreaId);
            mWriteDataComponent.Save(player.Data, dBProxy).Coroutine();
            {
                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                G2C_BattleKVData mMiracleCoinData = new G2C_BattleKVData();
                mMiracleCoinData.Key = (int)E_GameProperty.YuanbaoChange;
                mMiracleCoinData.Value = -yuanbaoCoin;

                mMiracleCoinData = new G2C_BattleKVData();
                mMiracleCoinData.Key = (int)E_GameProperty.YuanbaoCoin;
                mMiracleCoinData.Value = gamePlayer.Player.Data.YuanbaoCoin;

                mChangeValueMessage.Info.Add(mMiracleCoinData);

                player.Send(mChangeValueMessage);
            }


            LotteryPoolComponent lotteryPool = Root.MainFactory.GetCustomComponent<LotteryPoolComponent>();

            var ret = await lotteryPool.StartLottery(player, b_Request.Count);
            if(ret.err != 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(ret.err);
                b_Reply(b_Response);
                return false;
            }

            LotteryComponent lottery = player.GetCustomComponent<LotteryComponent>();
            b_Response.Ids.AddRange(ret.ids);
            b_Response.TotalCount = lottery.Data.TotalCount;
            b_Response.TotalCountMax = ConstLottery.LotteryCountMax;
            b_Response.IsSendMail = ret.isSendMail;
            b_Reply(b_Response);
            return true;
        }
    }
}