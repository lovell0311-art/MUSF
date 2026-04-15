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
    public class C2G_LeaveYourSeatRequestHandler : AMActorRpcHandler<C2G_LeaveYourSeatRequest, G2C_LeaveYourSeatResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_LeaveYourSeatRequest b_Request, G2C_LeaveYourSeatResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_LeaveYourSeatRequest b_Request, G2C_LeaveYourSeatResponse b_Response, Action<IMessage> b_Reply)
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
            var Activities = mServerArea.GetCustomComponent<CitySiegeActivities>();
            if (Activities != null && Activities.HaveInHand)
            {
                if (Activities.SupremeThrone == mPlayer.GameUserId)
                {
                    Activities.LeaveTiem = Help_TimeHelper.GetNowSecond();
                    b_Reply(b_Response);
                    return true;
                }
            }
            b_Reply(b_Response);
            return false;
        }
    }
}