using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using Aop.Api.Util;
using System.Net.Http;

//using MySqlX.XDevAPI;

namespace ETHotfix
{
    [HttpHandler(AppType.GM | AppType.PayServer, "/api/Pay/")]
    public class Http_GM_Pay : AHttpHandler
    {
        [Post] // Url-> /api/Pay/MyWeChatPayInfo
        public async Task<object> PayInfoXT(HttpListenerRequest req, string postBody)
        {
            Dictionary<string, string> data = postBody.Split('&').Select(item => item.Split('=')).ToDictionary(parts => parts[0], parts => parts[1]);
            Log.Warning($"栩腾:{postBody}");
            if (data.TryGetValue("extInfo", out var cid) == false)
            {
                return "FAILURE";
            }
           
            cid = System.Web.HttpUtility.UrlDecode(cid, Encoding.UTF8);
            List<string> list = cid.Split(',').ToList();

            int mExtra_info = UnitIdStruct.GetUnitZone(long.Parse(list[1]));
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mExtra_info);
            if (dBProxy2 == null)
            {
                return "FAILURE";
            }
            var pay = await dBProxy2.Query<DBPlayerPayOrderInfo>(P => P.App_Ordef_id == long.Parse(list[1]));
            if (pay == null || pay.Count <= 0)
            {
                return "FAILURE";
            }
            DBPlayerPayOrderInfo dBPlayerPayOrderInfo = pay[0] as DBPlayerPayOrderInfo;

            dBPlayerPayOrderInfo.TradePlatform = TradePlatform.MyPay;
            dBPlayerPayOrderInfo.SuccessTime = Help_TimeHelper.GetNowSecond();
            dBPlayerPayOrderInfo.Ordef_id = "";
            await dBProxy2.Save(dBPlayerPayOrderInfo);

            GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
            gM2C_PayReturnMessage.MAreaId = int.Parse(list[0]);
            gM2C_PayReturnMessage.AppOrderId = long.Parse(list[1]);
            gM2C_PayReturnMessage.GameUserID = dBPlayerPayOrderInfo.Rid;
            gM2C_PayReturnMessage.LogInfo = "栩腾";
            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = dBPlayerPayOrderInfo.GUid
                });
                l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
                if (l2sGetLoginRecord == null)
                {
                    return "FAILURE";
                }
            }
            catch (Exception e)
            {
                return "FAILURE";
            }

            if (l2sGetLoginRecord.GameUserId == dBPlayerPayOrderInfo.Rid)
            {
                var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, l2sGetLoginRecord.GameServerId);
                Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
                IResponse response2 = await gameSession.Call(gM2C_PayReturnMessage);
                C2GM_PayReturnResponse game2gm_SendMail = response2 as C2GM_PayReturnResponse;
                if (game2gm_SendMail == null)
                {
                    Log.Debug($"在线充值返回失败 角色:{dBPlayerPayOrderInfo.Rid}");
                }
            }
            return "SUCCESS";
        }
        [Post] // Url-> /api/Pay/MyWeChatPayInfo
        public async Task<object> MyWeChatPayInfo(HttpListenerRequest req, string postBody)
        {
            Dictionary<string, string> data = V4PayTopUp.ParseFormData(postBody);
            Log.Warning("微信支付："+postBody);
            if (data.Count == 0)
                data = postBody.Split('&').Select(item => item.Split('=')).ToDictionary(parts => parts[0], parts => parts[1]);

            if (data.TryGetValue("out_trade_no", out var cp_order_id) == false)
            {
                return "fail";
            }
            if (long.TryParse(cp_order_id, out var mCp_order_id) == false)
            {
                return "fail";
            }

            if (data.TryGetValue("attach", out var cid) == false)
            {
                return "fail";
            }
            if (int.TryParse(cid, out var mCp_id) == false)
            {
                return "fail";
            }

            int mExtra_info = UnitIdStruct.GetUnitZone(mCp_order_id);
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mExtra_info);
            if (dBProxy2 == null)
            {
                return "fail";
            }
            var pay = await dBProxy2.Query<DBPlayerPayOrderInfo>(P => P.App_Ordef_id == mCp_order_id);
            if (pay == null || pay.Count <= 0)
            {
                return "fail";
            }
            DBPlayerPayOrderInfo dBPlayerPayOrderInfo = pay[0] as DBPlayerPayOrderInfo;
            dBPlayerPayOrderInfo.TradePlatform = TradePlatform.MyPay;
            dBPlayerPayOrderInfo.SuccessTime = Help_TimeHelper.GetNowSecond();
            dBPlayerPayOrderInfo.Ordef_id = "";
            await dBProxy2.Save(dBPlayerPayOrderInfo);

            GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
            gM2C_PayReturnMessage.MAreaId = mCp_id;
            gM2C_PayReturnMessage.AppOrderId = mCp_order_id;
            gM2C_PayReturnMessage.GameUserID = dBPlayerPayOrderInfo.Rid;
            gM2C_PayReturnMessage.LogInfo = "微信充值";
            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = dBPlayerPayOrderInfo.GUid
                });
                l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
                if (l2sGetLoginRecord == null)
                {
                    return "fail";
                }
            }
            catch (Exception e)
            {
                return "fail";
            }

            if (l2sGetLoginRecord.GameUserId == dBPlayerPayOrderInfo.Rid)
            {
                var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, l2sGetLoginRecord.GameServerId);
                Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
                IResponse response2 = await gameSession.Call(gM2C_PayReturnMessage);
                C2GM_PayReturnResponse game2gm_SendMail = response2 as C2GM_PayReturnResponse;
                if (game2gm_SendMail == null)
                {
                    Log.Debug($"在线充值返回失败 角色:{dBPlayerPayOrderInfo.Rid}");
                }
            }
            return "success";
        }
        [Post] // Url-> /api/Pay/MyV4PayInfo
        public async Task<object> MyV4PayInfo(HttpListenerRequest req, string postBody)
        {
            Dictionary<string, string> data = V4PayTopUp.ParseFormData(postBody);
           
            if(data.Count == 0)
                data = postBody.Split('&').Select(item => item.Split('=')).ToDictionary(parts => parts[0], parts => parts[1]);

            if (data.TryGetValue("out_trade_no", out var cp_order_id) == false)
            {
                return "fail";
            }
            if (long.TryParse(cp_order_id, out var mCp_order_id) == false)
            {
                return "fail";
            }

            if (data.TryGetValue("attach", out var cid) == false)
            {
                return "fail";
            }
            if (int.TryParse(cid, out var mCp_id) == false)
            {
                return "fail";
            }

            bool signVerified = AlipaySignature.RSACheckV2(data, V4PayTopUp.V4Key, "utf-8", "RSA2",false);
            if (signVerified)
            {
                // TODO 验签成功后 
                //按照支付结果异步通知中的描述，对支付结果中的业务内容进行1\2\3\4二次校验，校验成功后在response中返回success 
                if (data.TryGetValue("trade_no", out var trade_no) == false)
                {
                    return "fail";
                }
                int mExtra_info = UnitIdStruct.GetUnitZone(mCp_order_id);
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mExtra_info);
                if (dBProxy2 == null)
                {
                    return "fail";
                }
                var pay = await dBProxy2.Query<DBPlayerPayOrderInfo>(P => P.App_Ordef_id == mCp_order_id);
                if (pay == null || pay.Count <= 0)
                {
                    return "fail";
                }
                DBPlayerPayOrderInfo dBPlayerPayOrderInfo = pay[0] as DBPlayerPayOrderInfo;
                dBPlayerPayOrderInfo.TradePlatform = TradePlatform.MyPay;
                dBPlayerPayOrderInfo.SuccessTime = Help_TimeHelper.GetNowSecond();
                dBPlayerPayOrderInfo.Ordef_id = trade_no;
                await dBProxy2.Save(dBPlayerPayOrderInfo);

                GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
                gM2C_PayReturnMessage.MAreaId = mCp_id;
                gM2C_PayReturnMessage.AppOrderId = mCp_order_id;
                gM2C_PayReturnMessage.GameUserID = dBPlayerPayOrderInfo.Rid;
                gM2C_PayReturnMessage.LogInfo = "精秀第三方充值";
                LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
                try
                {
                    var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                    Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                    IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                    {
                        UserId = dBPlayerPayOrderInfo.GUid
                    });
                    l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
                    if (l2sGetLoginRecord == null)
                    {
                        return "fail";
                    }
                }
                catch (Exception e)
                {
                    return "fail";
                }

                if (l2sGetLoginRecord.GameUserId == dBPlayerPayOrderInfo.Rid)
                {
                    var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, l2sGetLoginRecord.GameServerId);
                    Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
                    IResponse response2 = await gameSession.Call(gM2C_PayReturnMessage);
                    C2GM_PayReturnResponse game2gm_SendMail = response2 as C2GM_PayReturnResponse;
                    if (game2gm_SendMail == null)
                    {
                        Log.Debug($"在线充值返回失败 角色:{dBPlayerPayOrderInfo.Rid}");
                    }
                }
                return "success";
            }
            else
            {
                // TODO 验签失败则记录异常日志，并在response中返回fail. 
                Log.Info($"订单:{mCp_order_id}验签失败");
                return postBody;
            }
        }

        [Post] // Url-> /api/Pay/MyPayInfo
        public async Task<object> MyPayInfo(HttpListenerRequest req, string postBody)
        {
            Log.Warning($"支付宝：{postBody}");
            string[] PayList = postBody.Split("&");
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var ing in PayList)
            {
                var Value = ing.Split("=");
                var key = Value[0];
                var value = Value[1];
                value = System.Web.HttpUtility.UrlDecode(value, Encoding.UTF8);

                data.Add(key, value);
            }

            bool signVerified = AlipaySignature.RSACheckV1(data, MyPayTopUp.AppPublicKey, "UTF-8", "RSA2",false);
            if (signVerified)
            {
                
                // TODO 验签成功后 
                //按照支付结果异步通知中的描述，对支付结果中的业务内容进行1\2\3\4二次校验，校验成功后在response中返回success 
                if (data.TryGetValue("out_trade_no", out var cp_order_id) == false)
                {
                    Log.Warning("支付宝：获取 out_trade_no 错误");
                    return "fail";
                }
                if (long.TryParse(cp_order_id, out var mCp_order_id) == false)
                {
                    Log.Warning("支付宝：cp_order_id 转 long 错误");
                    return "fail";
                }
                if (data.TryGetValue("trade_no", out var order_id) == false)
                {
                    Log.Warning("支付宝：获取 trade_no 错误");
                    return "fail";
                }
                if (data.TryGetValue("passback_params", out var passback_params) == false)
                {
                    Log.Warning("支付宝：获取 passback_params 错误");
                    return "fail";
                }
                string passback = System.Web.HttpUtility.UrlDecode(passback_params, Encoding.UTF8);
                if (int.TryParse(passback, out int paramss) == false)
                {
                    Log.Warning("支付宝： passback_params 转 int 错误");
                    return "fail";
                }

                int mExtra_info = UnitIdStruct.GetUnitZone(mCp_order_id);
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mExtra_info);
                if (dBProxy2 == null)
                {
                    Log.Warning("支付宝：数据库获取错误");
                    return "fail";
                }
                var pay = await dBProxy2.Query<DBPlayerPayOrderInfo>(P => P.App_Ordef_id == mCp_order_id);
                if (pay == null || pay.Count <= 0)
                {
                    Log.Warning("支付宝：订单不存在");
                    return "fail";
                }
                DBPlayerPayOrderInfo dBPlayerPayOrderInfo = pay[0] as DBPlayerPayOrderInfo;
                dBPlayerPayOrderInfo.TradePlatform = TradePlatform.MyPay;
                dBPlayerPayOrderInfo.SuccessTime = Help_TimeHelper.GetNowSecond();
                dBPlayerPayOrderInfo.Ordef_id = order_id;
                await dBProxy2.Save(dBPlayerPayOrderInfo);

                GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
                gM2C_PayReturnMessage.MAreaId = paramss;
                gM2C_PayReturnMessage.AppOrderId = mCp_order_id;
                gM2C_PayReturnMessage.GameUserID = dBPlayerPayOrderInfo.Rid;
                gM2C_PayReturnMessage.LogInfo = "支付宝充值";
                LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
                try
                {
                    var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                    Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                    IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                    {
                        UserId = dBPlayerPayOrderInfo.GUid
                    });
                    l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
                    if (l2sGetLoginRecord == null)
                    {
                        Log.Warning("支付宝：玩家不在线");
                        return "fail";
                    }
                }
                catch (Exception e)
                {
                    Log.Warning("支付宝：异常");
                    return "fail";
                }

                if (l2sGetLoginRecord.GameUserId == dBPlayerPayOrderInfo.Rid)
                {
                    var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, l2sGetLoginRecord.GameServerId);
                    Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
                    IResponse response2 = await gameSession.Call(gM2C_PayReturnMessage);
                    C2GM_PayReturnResponse game2gm_SendMail = response2 as C2GM_PayReturnResponse;
                    if (game2gm_SendMail == null)
                    {
                        Log.Debug($"在线充值返回失败 角色:{dBPlayerPayOrderInfo.Rid}");
                    }
                }
                return "success";
            }
            else
            {
                Log.Warning($"支付宝：验签失败   {postBody}   ;");
                return "fail";
            }
        }
        [Post] // Url-> /api/Pay/PayInfoShouQ
        public async Task<object> PayInfoShouQ(HttpListenerRequest req, string postBody)
        {
            //Console.WriteLine("接收到订单");
            string[] PayList = postBody.Split("&");
            string mSign = "";
            Dictionary<string, string> data = new Dictionary<string, string>();
            data = Help_JsonSerializeHelper.DeSerialize<Dictionary<string, string>>(postBody);
            //foreach (var ing in PayList)
            //{
            //    var Value = ing.Split("=");
            //    var key = Value[0];
            //    var value = Value[1];
            //    value = System.Web.HttpUtility.UrlDecode(value, Encoding.UTF8);

            //    if (key == "sig")
            //    {
            //        mSign = value;
            //        continue;
            //    }
            //    data.Add(key, value);
            //}
            if (data.TryGetValue("sig", out mSign) == false)
            {
                return new
                {
                    code = -2,
                    success = false,
                    errMsg = "sig Error"
                };
            }
            if (data.TryGetValue("appOrderSn", out var cp_order_id) == false)
            {
                return new
                {
                    code = -2,
                    success = false,
                    errMsg = "AppOrderSn Error"
                };
            }
            data.Remove("sig");
            string jsonString = String.Join("&", data.OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}"));

            static string JoinCallbackSig(string requestData, string url, string hmacKey)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("POST").Append("&");
                string urlPath = new Uri(url).AbsolutePath;
                sb.Append(Uri.EscapeDataString(urlPath));
                //sb.Append($"POST&%2Forder_notify%2Fsandbox_mobile_q");
                var sortedDictionary = requestData.Split('&').Select(kv => kv.Split('='))
            .ToDictionary(kv => kv[0], kv => kv.Length > 1 ? kv[1] : "");
                var sortedMap = new SortedDictionary<string, string>(sortedDictionary);

                foreach (var entry in sortedMap)
                {
                    if (entry.Value != null && entry.Key != "sig")
                    {
                        sb.Append($"&{entry.Key}={entry.Value}");
                    }
                }

               // Console.WriteLine($"生成回调的签名前的字符串为: {sb}");

                StringBuilder returnSig = new StringBuilder();
                try
                {
                    using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(hmacKey)))
                    {
                        byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                        foreach (byte item in signatureBytes)
                        {
                            returnSig.Append($"{item:X2}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("加密HMACSHA256失败");
                    Console.WriteLine(e);
                }

                return returnSig.ToString().ToLower();
            }
            string sign = JoinCallbackSig(jsonString, "http://shouq.api.ieg123.com:15080/api/Pay/PayInfoShouQ", cp_order_id);
            //Console.WriteLine($"生成回调的签名前的字符串为: {sign}");
            if (sign != mSign)
            {
                return new
                {
                    code = -2,
                    success = false,
                    errMsg = ""
                };
            }

            //if (data.TryGetValue("serverId", out var extra_info) == false)
            //{
            //    return new
            //    {
            //        code = -2,
            //        success = false,
            //        rrMsg = "ServerId Error"
            //    };
            //}
            //if (int.TryParse(extra_info, out int mExtra_info) == false)
            //{
            //    return new
            //    {
            //        code = -2,
            //        success = false,
            //        rrMsg = "ServerId Error"
            //    };
            //}

           
            if (long.TryParse(cp_order_id, out var mCp_order_id) == false)
            {
                return new
                {
                    code = -2,
                    success = false,
                    errMsg = "AppOrderSn Error"
                };
            }
            if (data.TryGetValue("exInfo", out var exInfo) == false)
            {
                return new
                {
                    code = -2,
                    success = false,
                    errMsg = "exInfo Error"
                };
            }
            if (int.TryParse(exInfo, out var exInfo_Id) == false)
            {
                return new
                {
                    code = -2,
                    success = false,
                    errMsg = "exInfo Error"
                };
            }
            var mExtra_info = exInfo_Id >> 16;
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mExtra_info);
            if (dBProxy2 == null)
            {
                return new
                {
                    code = -2,
                    success = false,
                    errMsg = "DB Error"
                };
            }
            var pay = await dBProxy2.Query<DBPlayerPayOrderInfo>(P => P.App_Ordef_id == mCp_order_id);
            if (pay == null || pay.Count <= 0)
            {
                return new
                {
                    code = -2,
                    success = false,
                    errMsg = "Order Error"
                };
            }
            DBPlayerPayOrderInfo dBPlayerPayOrderInfo = pay[0] as DBPlayerPayOrderInfo;
            dBPlayerPayOrderInfo.TradePlatform = TradePlatform.ShouQ;
            dBPlayerPayOrderInfo.SuccessTime = Help_TimeHelper.GetNowSecond();
            await dBProxy2.Save(dBPlayerPayOrderInfo);

            GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
            gM2C_PayReturnMessage.MAreaId = exInfo_Id;
            gM2C_PayReturnMessage.AppOrderId = mCp_order_id;
            gM2C_PayReturnMessage.GameUserID = dBPlayerPayOrderInfo.Rid;
            gM2C_PayReturnMessage.LogInfo = "手Q第三方充值";
            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = dBPlayerPayOrderInfo.GUid
                });
                l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
                if (l2sGetLoginRecord == null)
                {
                    return Error(msg: "LoginCenter 没开启");
                }
            }
            catch (Exception e)
            {
                return Error(msg: e.ToString());
            }

            if (l2sGetLoginRecord.GameUserId == dBPlayerPayOrderInfo.Rid)
            {
                var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, l2sGetLoginRecord.GameServerId);
                Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
                IResponse response2 = await gameSession.Call(gM2C_PayReturnMessage);
                C2GM_PayReturnResponse game2gm_SendMail = response2 as C2GM_PayReturnResponse;
                if (game2gm_SendMail == null)
                {
                    Log.Debug($"在线充值返回失败 角色:{dBPlayerPayOrderInfo.Rid}");
                }
            }
            return new
            {
                code = 200,
                success = true,
                errMsg = ""
            };
        }

        [Post] // Url-> /api/Pay/PayInfoDY
        public async Task<object> PayInfoDY(HttpListenerRequest req, string postBody)
        {
            Log.Warning($"接收到订单:{postBody}");
            string[] PayList = postBody.Split("&");
            string mSign = "";
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var ing in PayList)
            {
                var Value = ing.Split("=");
                var key = Value[0];
                var value = Value[1];
                value = System.Web.HttpUtility.UrlDecode(value, Encoding.UTF8);

                if (key == "sign")
                {
                    mSign = value;
                    continue;
                }
                data.Add(key, value);
            }

            #region sign error
            string jsonString = String.Join("&", data.OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}"));
            string sign = "";
            using (HMACSHA1 hmacsha1 = new HMACSHA1())
            {
                hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(ConstDouYinSDK.secretKey);

                byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(jsonString);
                byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
                sign = Convert.ToBase64String(hashBytes);
            }
            mSign = System.Web.HttpUtility.UrlDecode(mSign, Encoding.UTF8);

            if (sign != mSign)
            {
                return new
                {
                    status = -2,
                    msg = "签名验证错误"
                };
            }
            #endregion         

            #region app_id error
            if (data.TryGetValue("app_id", out var app_id) == false)
            {
                return new
                {
                    status = -3,
                    msg = "app_id error"
                };
            }
            if (int.TryParse(app_id, out var mAppid) == false)
            {
                return new
                {
                    status = -3,
                    msg = "app_id error"
                };
            }
            #endregion     
            if (mAppid != ConstDouYinSDK.app_id)
            {
                return new
                {
                    status = -3,
                    msg = "app_id error"
                };
            }
            #region status error
            if (data.TryGetValue("status", out var status) == false)
            {
                return new
                {
                    status = -100,
                    msg = "status 不存在"
                };
            }
            if (int.TryParse(status, out var mstatus) == false)
            {
                return new
                {
                    status = -100,
                    msg = "status error"
                };
            }
            #endregion
            if (mstatus != 2)
            {
                return new
                {
                    status = -100,
                    msg = "status error :" + status
                };
            }
            #region 订单 error
            if (data.TryGetValue("cp_order_id", out var cp_order_id) == false)
            {
                return new
                {
                    status = -4,
                    msg = "cp_order_id 不存在"
                };
            }
            if (long.TryParse(cp_order_id, out var mCp_order_id) == false)
            {
                return new
                {
                    status = -4,
                    msg = "cp_order_id error"
                };
            }
            #endregion

            #region 自定义信息 error
            if (data.TryGetValue("extra_info", out var extra_info) == false)
            {
                return new
                {
                    status = -5,
                    msg = "extra_info 不存在"
                };
            }
            if (int.TryParse(extra_info, out int mExtra_info) == false)
            {
                return new
                {
                    status = -5,
                    msg = "extra_info error"
                };
            }
            #endregion

            #region 总支付金额 分
            if (data.TryGetValue("amount", out var amount) == false)
            {
                return new
                {
                    status = -6,
                    msg = "amount 不存在"
                };
            }
            if (int.TryParse(amount, out int mAmount) == false)
            {
                return new
                {
                    status = -6,
                    msg = "amount error"
                };
            }
            #endregion
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mExtra_info >> 16);
            if (dBProxy2 == null)
            {
                return new
                {
                    status = -2,
                    msg = "数据库异常"
                };
            }
            var pay = await dBProxy2.Query<DBPlayerPayOrderInfo>(P => P.Gid == ConstDouYinSDK.app_id && P.App_Ordef_id == mCp_order_id);
            if (pay == null || pay.Count <= 0)
            {
                return new
                {
                    status = -2,
                    msg = "数据库无订单"
                };
            }
            DBPlayerPayOrderInfo dBPlayerPayOrderInfo = pay[0] as DBPlayerPayOrderInfo;
            if (dBPlayerPayOrderInfo.MoneyFen != mAmount)
            {
                return new
                {
                    status = -2,
                    msg = "充值金额异常"
                };
            }
            dBPlayerPayOrderInfo.TradePlatform = TradePlatform.DY;
            dBPlayerPayOrderInfo.SuccessTime = Help_TimeHelper.GetNowSecond();
            await dBProxy2.Save(dBPlayerPayOrderInfo);
  
            GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
            gM2C_PayReturnMessage.MAreaId = mExtra_info;
            gM2C_PayReturnMessage.AppOrderId = mCp_order_id;
            gM2C_PayReturnMessage.GameUserID = dBPlayerPayOrderInfo.Rid;
            gM2C_PayReturnMessage.LogInfo = "抖音第三方充值";
            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = dBPlayerPayOrderInfo.GUid
                });
                l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
                if (l2sGetLoginRecord == null)
                {
                    return Error(msg: "LoginCenter 没开启");
                }
            }
            catch (Exception e)
            {
                return Error(msg: e.ToString());
            }

            if (l2sGetLoginRecord.GameUserId == dBPlayerPayOrderInfo.Rid)
            {
                var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, l2sGetLoginRecord.GameServerId);
                Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
                IResponse response2 = await gameSession.Call(gM2C_PayReturnMessage);
                C2GM_PayReturnResponse game2gm_SendMail = response2 as C2GM_PayReturnResponse;
                if (game2gm_SendMail == null)
                {
                    Log.Debug($"在线充值返回失败 角色:{dBPlayerPayOrderInfo.Rid}");
                }
            }
            //Console.WriteLine("接收到订单 成功");

            return "SUCCESS";
        }

        // 游戏服务器状态
        [Get]   // Url-> /api/server/PayInfo
        public async Task<object> PayInfo(HttpListenerRequest req)
        {
            string[] PayList = req.QueryString.AllKeys;
            string Value = "";
            string Sign = "";
            SortedDictionary<string, string> keyValuePairs = new SortedDictionary<string, string>();

            foreach (var ing in PayList)
            {
                Value = req.QueryString[ing];

                if (ing == "sign")
                {
                    Sign = Value;
                    continue;
                }
                keyValuePairs.Add(ing, Value);
            }
            string Json = String.Join("&", keyValuePairs.Select(p => $"{p.Key}={p.Value}")) + "#" + ConstXYSDK.Key;
            string Chek = MD5Helper.GetMD5Hash(Json).ToLower();
            if (Chek != Sign)
            {
                return new
                {
                    status = -2,
                    msg = "签名验证错误"
                };
            }
            int mAreaId = 0;
            long AppOrderId = 0;
            string OrderId = "";
            int Money = 0;
            long GamePlayerID = 0;
            int SourceGameAreaId = 0;
            foreach (var ing in keyValuePairs)
            {
                if (ing.Key == "sid")
                    mAreaId = int.Parse(ing.Value);
                if (ing.Key == "app_order_id")
                    AppOrderId = long.Parse(ing.Value);
                if (ing.Key == "order_id")
                    OrderId = ing.Value;
                if (ing.Key == "money")
                    Money = int.Parse(ing.Value);
                if (ing.Key == "rid")
                    GamePlayerID = long.Parse(ing.Value);
                if(ing.Key == "extra1")
                    SourceGameAreaId = int.Parse(ing.Value);
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
            if (dBProxy2 != null)
            {
                var pay = await dBProxy2.Query<DBPlayerPayOrderInfo>(P => P.App_Ordef_id == AppOrderId);
                if (pay != null && pay.Count > 0)
                {
                    DBPlayerPayOrderInfo dBPlayerPayOrderInfo = pay[0] as DBPlayerPayOrderInfo;
                    if (dBPlayerPayOrderInfo.Money != Money)
                    {
                        return new
                        {
                            status = -2,
                            msg = "充值金额异常"
                        };
                    }
                    dBPlayerPayOrderInfo.Ordef_id = OrderId;
                    dBPlayerPayOrderInfo.SuccessTime = Help_TimeHelper.GetNowSecond();
                    await dBProxy2.Save(dBPlayerPayOrderInfo);

                    GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
                    gM2C_PayReturnMessage.MAreaId = SourceGameAreaId;
                    gM2C_PayReturnMessage.AppOrderId = AppOrderId;
                    gM2C_PayReturnMessage.GameUserID = dBPlayerPayOrderInfo.Rid;
                    gM2C_PayReturnMessage.LogInfo = "恺英第三方充值";
                    LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
                    try
                    {
                        var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                        Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                        IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                        {
                            UserId = dBPlayerPayOrderInfo.GUid
                        });
                        l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
                        if (l2sGetLoginRecord == null)
                        {
                            return Error(msg: "LoginCenter 没开启");
                        }
                    }
                    catch (Exception e)
                    {
                        return Error(msg: e.ToString());
                    }

                    if (l2sGetLoginRecord.GameUserId == GamePlayerID)
                    {
                        var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, l2sGetLoginRecord.GameServerId);
                        Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
                        IResponse response2 = await gameSession.Call(gM2C_PayReturnMessage);
                        C2GM_PayReturnResponse game2gm_SendMail = response2 as C2GM_PayReturnResponse;
                        if (game2gm_SendMail == null)
                        {
                            Log.Debug($"在线充值返回失败 角色:{GamePlayerID}");
                        }
                    }

                }
                else
                {
                    return new
                    {
                        status = -2,
                        msg = "数据库异常"
                    };
                }
            }
            else
            {
                return new
                {
                    status = -2,
                    msg = "数据库异常"
                };
            }
            return new
            {
                status = 0,
                msg = "充值成功"
            };
        }



    }
}
