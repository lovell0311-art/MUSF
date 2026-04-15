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
    public class C2G_DouYinCreateAnOrderRequestHandler : AMActorRpcHandler<C2G_DouYinCreateAnOrderRequest, C2G_DouYinCreateAnOrderResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_DouYinCreateAnOrderRequest b_Request, C2G_DouYinCreateAnOrderResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }


        protected override async Task<bool> Run(C2G_DouYinCreateAnOrderRequest b_Request, C2G_DouYinCreateAnOrderResponse b_Response, Action<IMessage> b_Reply)
        {
            //b_Reply(b_Response);
            //return true;
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

            int Value= PlayerShopQuota.GetPayValue((PlayerShopQuotaType)b_Request.RechargeType);
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
            var moneyFen  = Value * 100;
            int app_id = ConstDouYinSDK.app_id;
            string secretKey = ConstDouYinSDK.secretKey;
            string AppCallbackUrl = ConstDouYinSDK.AppCallbackUrl;

            long App_Ordef_id = ETModel.IdGeneraterNew.Instance.GenerateUnitId(1);

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                ["aid"] = app_id.ToString(),
                ["cp_order_id"] = App_Ordef_id.ToString(),
                ["product_id"] = "10000",
                ["product_name"] = "魔晶",
                ["product_desc"] = "物品魔晶",
                ["product_amount"] = moneyFen.ToString(),
                ["sdk_open_id"] = dbLoginInfo.DouyinAccountNumber,
                ["callback_url"] = AppCallbackUrl,
                ["actual_amount"] = moneyFen.ToString(),
                ["risk_control_info"] = b_Request.ControlInfo,
                ["trade_type"] = "2",
                //["user_agent"] = "555",
                //["coupon_id"] = "0",
                //["server_id"] = "12",
                //["role_id"] = "566666",
                //["role_name"] = "55555",
                //["role_level"] = "400",
                //["role_vip_level"] = "2",
                ["extra_info"] = mPlayer.SourceGameAreaId.ToString(),
                //["client_ip"] = "120.24.45.40",
            };
            string jsonString = String.Join("&", data.OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}"));
            string sign = "";
            using (HMACSHA1 hmacsha1 = new HMACSHA1())
            {
                hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(secretKey);

                byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(jsonString);
                byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
                sign = Convert.ToBase64String(hashBytes);
            }

            var mp = new Dictionary<string, string>
            {
                { "aid", data["aid"] },
                { "cp_order_id", data["cp_order_id"] },
                { "product_id", data["product_id"] },
                { "product_name", data["product_name"] },
                { "product_desc", data["product_desc"]},
                { "product_amount", data["product_amount"] },
                { "sdk_open_id", data["sdk_open_id"] },
                { "callback_url", data["callback_url"] },
                { "actual_amount", data["actual_amount"] },
                { "risk_control_info", data["risk_control_info"] },
                { "trade_type", data["trade_type"] },
                //{ "user_agent", data["user_agent"] },
                //{ "coupon_id", data["coupon_id"] },
                //{ "server_id", data["server_id"] },
                //{ "role_id", data["role_id"] },
                //{ "role_name", data["role_name"] },
                //{ "role_level", data["role_level"] },
                //{ "role_vip_level", data["role_vip_level"] },
                { "extra_info", data["extra_info"] },

                { "client_ip", b_Request.Clinetip },
                { "sign", sign },
            };

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://usdk.dailygn.com/gsdk/usdk/payment/live_pre_order"),
                Content = new FormUrlEncodedContent(mp),
            };
            string sdk_param = null;
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                var responseObject = Help_JsonSerializeHelper.DeSerialize<DouYinPrePayResult>(body);
                if (responseObject == null)
                {
                    // 请求 API 接口出错
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(116);
                    b_Reply(b_Response);
                    return false;
                }

                if (responseObject.code != 0)
                {
                    // 参数被篡改
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(115);
                    b_Reply(b_Response);
                    return false;
                }
                sdk_param = responseObject.sdk_param;
            }

            DBPlayerPayOrderInfo mPlayerPayOrderInfo = new DBPlayerPayOrderInfo();
            mPlayerPayOrderInfo.TradePlatform = TradePlatform.DYPre;
            mPlayerPayOrderInfo.Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
            mPlayerPayOrderInfo.App_Ordef_id = App_Ordef_id;
            mPlayerPayOrderInfo.Gid = ConstDouYinSDK.app_id;
            mPlayerPayOrderInfo.Uid = long.Parse(dbLoginInfo.DouyinAccountNumber);
            mPlayerPayOrderInfo.GUid = mPlayer.UserId;
            mPlayerPayOrderInfo.Rid = mPlayer.GameUserId;
            mPlayerPayOrderInfo.Product_id = b_Request.RechargeType;
            mPlayerPayOrderInfo.Money = Value;
            mPlayerPayOrderInfo.MoneyFen = moneyFen;
            mPlayerPayOrderInfo.Time = Help_TimeHelper.GetNowSecond();
            mPlayerPayOrderInfo.RName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
            mPlayerPayOrderInfo.Effective = true;
            mPlayerPayOrderInfo.ChannelId = dbLoginInfo.ChannelId;


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
            b_Response.OrderId = mPlayerPayOrderInfo.App_Ordef_id.ToString();
            b_Response.AppCallbackUrl = sdk_param;
            b_Response.PayRmb = Value;
            b_Reply(b_Response);
            return true;
        }
    }
}