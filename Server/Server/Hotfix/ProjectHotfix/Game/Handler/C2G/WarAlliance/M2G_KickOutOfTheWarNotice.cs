using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class M2G_KickOutOfTheWarNoticeHandler : AMHandler<M2G_KickOutOfTheWarNotice>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, M2G_KickOutOfTheWarNotice b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(M2G_KickOutOfTheWarNotice b_Request)
        {
            int mAreaId = (int)b_Request.AppendData;
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.GameUserId);
            if (mPlayer == null)
            {
                return false;
            }
            var WarAllianceInfo = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (WarAllianceInfo == null)
            {
                return false;
            }
            if(WarAllianceInfo.WarAllianceID == b_Request.WarAllianceID) 
            {
                G2C_KickOutOfTheWarNotice g2C_KickOutOfTheWarNotice = new G2C_KickOutOfTheWarNotice();
                g2C_KickOutOfTheWarNotice.WarAllianceID = b_Request.WarAllianceID;

                GMStruct_WarAllinceInfo Info = new GMStruct_WarAllinceInfo();
                Info.WarAllianceID = 0;
                Info.WarAllianceName = "";
                Info.WarAllianceNotice = "";
                Info.WarAllianceBadge = null;
                Info.MemberPost = 0;
                Info.Currentquantity = 0;
                Info.LeaderName = "";
                Info.WarAllianceLevel = 0;
                WarAllianceInfo.UpData(Info);

                mPlayer.GetCustomComponent<GamePlayer>().Data.WarAllianceID = 0;
                mPlayer.GetCustomComponent<GamePlayer>().Data.WallTile = "";
                mPlayer.Send(g2C_KickOutOfTheWarNotice);
            }
           
            return true;
        }
    }
}