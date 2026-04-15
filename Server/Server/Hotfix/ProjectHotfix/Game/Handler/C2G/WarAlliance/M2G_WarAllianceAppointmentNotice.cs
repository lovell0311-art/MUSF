using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class M2G_WarAllianceAppointmentNoticeHandler : AMHandler<M2G_WarAllianceAppointmentNotice>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, M2G_WarAllianceAppointmentNotice b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(M2G_WarAllianceAppointmentNotice b_Request)
        {
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(OptionComponent.Options.ZoneId, b_Request.GameUserID);
            if (mPlayer == null)
            {
                return false;
            }
            Log.Debug($"Name{mPlayer.GetCustomComponent<GamePlayer>().Data.NickName} ID{b_Request.GameUserID}");
            G2C_WarAllianceAppointmentNotice g2C_WarAllianceAppointmentNotice = new G2C_WarAllianceAppointmentNotice();
            g2C_WarAllianceAppointmentNotice.CharName = b_Request.CharName;
            g2C_WarAllianceAppointmentNotice.ClassType = b_Request.ClassType; 

            mPlayer.Send(g2C_WarAllianceAppointmentNotice);
            return true;
        }
    }
}