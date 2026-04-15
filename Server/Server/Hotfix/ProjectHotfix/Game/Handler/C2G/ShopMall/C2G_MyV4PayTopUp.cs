using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Web;


namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class G2C_MyV4PayTopUpHandler : AMActorRpcHandler<C2G_MyV4PayTopUp, G2C_MyV4PayTopUp>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MyV4PayTopUp b_Request, G2C_MyV4PayTopUp b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> CodeAsync(Session b_Connect, C2G_MyV4PayTopUp b_Request, G2C_MyV4PayTopUp b_Response, Action<IMessage> b_Reply)
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

            long Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
            int Value = PlayerShopQuota.GetPayValue((PlayerShopQuotaType)b_Request.RechargeType);
            //测试额度
            //Value = 1;
            if (Value == -1)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            DBPlayerPayOrderInfo mPlayerPayOrderInfo = new DBPlayerPayOrderInfo
            {
                Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                TradePlatform = TradePlatform.MyPay,
                App_Ordef_id = Id,
                Gid = 114,
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
            //微信支付单位分
            Value *= 100;
            string AreaIdStr = HttpUtility.UrlEncode(mPlayer.SourceGameAreaId.ToString(), Encoding.UTF8);
            string address = b_Connect.RemoteAddress.Address.ToString();

            Dictionary<string, string> bizContent = new Dictionary<string, string>();
            bizContent.Add("description", "魔晶");
            bizContent.Add("out_trade_no", Id.ToString());
            bizContent.Add("attach", AreaIdStr);
            bizContent.Add("total", Value.ToString());

            var data = new FormUrlEncodedContent(bizContent);
            HttpClient client = new HttpClient();
            string bodystr = "";
            using (var response = await client.PostAsync("http://103.91.208.168:83/wxpay.php", data))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();

                try
                {
                    // Check the response status code
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("success, return body = " + responseBody);
                        JObject jsonObject = JObject.Parse(responseBody);

                        // 提取"data"字段的值
                        bodystr = (string)jsonObject["data"];
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        Console.WriteLine("success");
                    }
                    else
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(responseBody);

                        // 提取"data"字段的值
                        bodystr = (string)jsonObject["data"];
                        //Console.WriteLine($"failed, resp code = {(int)response.StatusCode}, return body = {responseBody}");
                        //throw new HttpRequestException("Request failed");
                    }
                }
                finally
                {
                    response.Dispose();
                }
            }
            b_Response.OrderStrURL = bodystr;
            b_Reply(b_Response);
            return true;
        }
    }
}
