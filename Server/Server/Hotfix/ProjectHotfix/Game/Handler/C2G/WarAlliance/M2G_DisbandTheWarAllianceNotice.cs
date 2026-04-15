using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class M2G_DisbandTheWarAllianceNoticeHandler : AMHandler<M2G_DisbandTheWarAllianceNotice>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, M2G_DisbandTheWarAllianceNotice b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(M2G_DisbandTheWarAllianceNotice b_Request)
        {
            int mAreaId = (int)b_Request.AppendData;
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.GameUserID);
            if (mPlayer == null)
            {
                return false;
            }

            var WarAllianceInfo = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (WarAllianceInfo.WarAllianceID == 0)
            {
                return false;
            }

            GMStruct_WarAllinceInfo gMStruct_WarAllinceInfo = new GMStruct_WarAllinceInfo();
            WarAllianceInfo.UpData(gMStruct_WarAllinceInfo);
            mPlayer.GetCustomComponent<GamePlayer>().Data.WarAllianceID = 0;
            return true;
        }
    }
}