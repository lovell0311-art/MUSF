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

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_CreateAnOrderResponseHandler : AMActorRpcHandler<C2G_CreateAnOrderRequest, C2G_CreateAnOrderResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_CreateAnOrderRequest b_Request, C2G_CreateAnOrderResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_CreateAnOrderRequest b_Request, C2G_CreateAnOrderResponse b_Response, Action<IMessage> b_Reply)
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
            DBPlayerPayOrderInfo mPlayerPayOrderInfo = new DBPlayerPayOrderInfo
            {
                Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                App_Ordef_id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                Gid = ConstXYSDK.Gid,
                Uid = long.Parse(dbLoginInfo.XYAccountNumber),
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
            b_Response.OrderId = mPlayerPayOrderInfo.App_Ordef_id.ToString();
            b_Response.AppCallbackUrl = ConstXYSDK.AppCallbackUrl;
            b_Response.PayRmb =Value.ToString();
            b_Response.Extra1 = mPlayer.SourceGameAreaId.ToString(); 
            b_Reply(b_Response);
            return true;
        }
    }
}