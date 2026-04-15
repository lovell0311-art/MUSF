using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PurifyBloodVesselsHandler : AMActorRpcHandler<C2G_PurifyBloodVessels, G2C_PurifyBloodVessels>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PurifyBloodVessels b_Request, G2C_PurifyBloodVessels b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_PurifyBloodVessels b_Request, G2C_PurifyBloodVessels b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mServerArea.GameAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0 || b_Request.BloodId == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                b_Reply(b_Response);
                return false;
            }

            var mPlayerBloodAwakening = mPlayer.GetCustomComponent<PlayerBloodAwakeningComponent>();
            if (mPlayerBloodAwakening != null)
            {
                int ErrorId = mPlayerBloodAwakening.PurityBloodAwakening(b_Request.BloodId,b_Request.RingId);

                b_Response.Error = ErrorId;
                b_Response.BloodInfo = mPlayerBloodAwakening.GetBloodSendInfo(b_Request.BloodId);
                b_Reply(b_Response);
                return true;
            }
            b_Response.Error = 3803;
            b_Reply(b_Response);
            return true;
        }
    }
}