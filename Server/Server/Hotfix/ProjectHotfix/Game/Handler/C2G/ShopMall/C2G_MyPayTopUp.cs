using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Threading.Tasks;
using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Domain;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TencentCloud.Batch.V20170312.Models;
using System.Web;
using TencentCloud.Mrs.V20200910.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_MyPayTopUpHandler : AMActorRpcHandler<C2G_MyPayTopUp, G2C_MyPayTopUp>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MyPayTopUp b_Request, G2C_MyPayTopUp b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_MyPayTopUp b_Request, G2C_MyPayTopUp b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Reply(b_Response);
                return false;
            }
            var PlayerShop = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            if (PlayerShop == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
            DBAccountInfo dbLoginInfo = null;
            if (mDBProxy != null)
            {
                var list = await mDBProxy.Query<DBAccountInfo>(p => p.Id == mPlayer.UserId);
                if (list.Count > 0)
                {
                    dbLoginInfo = list[0] as DBAccountInfo;
                }
            }

            long Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
            int Value = PlayerShopQuota.GetPayValue((PlayerShopQuotaType)b_Request.RechargeType);
            if (Value == -1)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            //if (Value == 0 && (PlayerShopQuotaType)b_Request.RechargeType == PlayerShopQuotaType.OneTimeRecharge)
            //{
            //    Value = PlayerShop.GetOneTimeRechargeValue();
            //}

            IAopClient client = new DefaultAopClient(MyPayTopUp.URL, MyPayTopUp.AppId, MyPayTopUp.PrivateKey, "json", "1.0", "RSA2", MyPayTopUp.AppPublicKey, "UTF-8", false);
            AlipayTradeAppPayRequest request = new AlipayTradeAppPayRequest();
            request.SetNotifyUrl(MyPayTopUp.AppCallbackUrl);
            Dictionary<string, object> bizContent = new Dictionary<string, object>();
            bizContent.Add("out_trade_no", Id.ToString());
            bizContent.Add("total_amount", Value);
            bizContent.Add("subject", "游戏魔晶充值");
            bizContent.Add("product_code", "QUICK_MSECURITY_PAY");

            //Dictionary<string, object> extendParams = new Dictionary<string, object>();
            string value = HttpUtility.UrlEncode(mPlayer.SourceGameAreaId.ToString(), Encoding.UTF8);
            //extendParams.Add("passback_params", value);
            //bizContent.Add("extend_params", extendParams);
            bizContent.Add("passback_params", value);
            string Contentjson = Help_JsonSerializeHelper.Serialize(bizContent);
            request.BizContent = Contentjson;
            AlipayTradeAppPayResponse response = client.SdkExecute(request);
            //Console.WriteLine(response.Body);
            b_Response.OrderStr = response.Body;

            DBPlayerPayOrderInfo mPlayerPayOrderInfo = new DBPlayerPayOrderInfo
            {
                Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                TradePlatform = TradePlatform.MyPay,
                App_Ordef_id = Id,
                Gid = 113,
                Uid = dbLoginInfo.Id,
                GUid = mPlayer.UserId,
                Rid = mPlayer.GameUserId,
                Product_id = b_Request.RechargeType,
                Money = Value,
                Time = Help_TimeHelper.GetNowSecond(),
                RName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName,
                Effective = true,
                ChannelId = dbLoginInfo.ChannelId
            };
            var mDBProxy2 = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);
            if (mDBProxy2 != null)
            {
                var list = await mDBProxy.Query<DBPlayerPayOrderInfo>(p => p.App_Ordef_id == mPlayerPayOrderInfo.App_Ordef_id);
                if (list.Count > 0)
                {
                    b_Reply(b_Response);
                    return false;
                }
                else
                {
                    await mDBProxy2.Save(mPlayerPayOrderInfo);
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}