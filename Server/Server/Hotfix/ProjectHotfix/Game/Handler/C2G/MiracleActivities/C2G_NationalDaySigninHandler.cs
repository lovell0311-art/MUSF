using Aop.Api.Domain;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_NationalDaySigninHandler : AMActorRpcHandler<C2G_NationalDaySignin, G2C_NationalDaySignin>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_NationalDaySignin b_Request, G2C_NationalDaySignin b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_NationalDaySignin b_Request, G2C_NationalDaySignin b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
            if (!ActivitiesInfo.GetActivitState(7))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2400);
                b_Reply(b_Response);
                return false;
            }
            var ShopInfo = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            if (ShopInfo != null)
            {
                ShopInfo.SetSevenDay(0, out int Day);
                if (Day != 0)
                {
                    if (!ShopInfo.NationalDayClaim(Day))
                    {
                        b_Response.Error = 2406;
                        b_Reply(b_Response);
                        return true;
                    }
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}