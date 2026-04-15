using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class M2G_WarAllianceAgreeOrRejectNoticeHandler : AMHandler<M2G_WarAllianceAgreeOrRejectNotice>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, M2G_WarAllianceAgreeOrRejectNotice b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(M2G_WarAllianceAgreeOrRejectNotice b_Request)
        {
            int mAreaId = (int)b_Request.AppendData;
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.GameUserID);
            if (mPlayer == null)
            {
                return false;
            }
            var WarAllianceInfo = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (WarAllianceInfo == null)
            {
                return false;
            }

            G2C_WarAllianceAgreeOrRejectNotice g2C_WarAllianceAgreeOrRejectNotice = new G2C_WarAllianceAgreeOrRejectNotice();
            g2C_WarAllianceAgreeOrRejectNotice.WAInfo = new Struct_WarAllinceInfo();

            if (b_Request.Type == 0)
            {
                g2C_WarAllianceAgreeOrRejectNotice.Message = "对方拒绝你的加入！";
                g2C_WarAllianceAgreeOrRejectNotice.WAInfo.WarAllianceID = b_Request.WAInfo.WarAllianceID;
                for (int i=0; i < WarAllianceInfo.WarAllianceList.Length;i++)
                {
                    if (WarAllianceInfo.WarAllianceList[i] == b_Request.WAInfo.WarAllianceID)
                    {
                        WarAllianceInfo.WarAllianceList[i] = 0;
                        break;
                    }
                }
                mPlayer.Send(g2C_WarAllianceAgreeOrRejectNotice);
            }
            else if (b_Request.Type == 1)
            {
                g2C_WarAllianceAgreeOrRejectNotice.Message = "对方同意你的加入！";
                for (int i = 0; i < 5; i++)
                {
                    if (WarAllianceInfo.WarAllianceList[i] == 0 && WarAllianceInfo.WarAllianceList[i] != b_Request.WAInfo.WarAllianceID)
                        WarAllianceInfo.UpDateWarAlliancePlayerInfo(0,1, WarAllianceInfo.WarAllianceList[i]);
                }
                WarAllianceInfo.WarAllianceList = new long[5] { 0,0,0,0,0};
                WarAllianceInfo.UpData(b_Request.WAInfo);
                 
                /*g2C_WarAllianceAgreeOrRejectNotice.WAInfo.WarAllianceID = b_Request.WAInfo.WarAllianceID;
                g2C_WarAllianceAgreeOrRejectNotice.WAInfo.WarAllianceName = b_Request.WAInfo.WarAllianceName;
                g2C_WarAllianceAgreeOrRejectNotice.WAInfo.WarAllianceLevel = b_Request.WAInfo.WarAllianceLevel;
                g2C_WarAllianceAgreeOrRejectNotice.WAInfo.WarAllianceBadge = b_Request.WAInfo.WarAllianceBadge;
                g2C_WarAllianceAgreeOrRejectNotice.WAInfo.LeaderName = b_Request.WAInfo.LeaderName;
                g2C_WarAllianceAgreeOrRejectNotice.WAInfo.MemberPost = b_Request.WAInfo.MemberPost;
                g2C_WarAllianceAgreeOrRejectNotice.WAInfo.WarAllianceNotice = b_Request.WAInfo.WarAllianceNotice;*/
                g2C_WarAllianceAgreeOrRejectNotice.WAInfo = WarAllianceInfo.GetInfo();
                mPlayer.GetCustomComponent<GamePlayer>().Data.WarAllianceID = b_Request.WAInfo.WarAllianceID;
                mPlayer.GetCustomComponent<GamePlayer>().Data.WallTile = b_Request.WAInfo.WarAllianceName;
                mPlayer.Send(g2C_WarAllianceAgreeOrRejectNotice);

                WarAllianceInfo.UpDateWarAlliancePlayerInfo();
            }
            return true;
        }
    }
}