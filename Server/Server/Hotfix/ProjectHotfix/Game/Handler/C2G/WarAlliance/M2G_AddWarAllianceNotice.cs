using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class M2G_AddWarAllianceNoticeHandler : AMHandler<M2G_AddWarAllianceNotice>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, M2G_AddWarAllianceNotice b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(M2G_AddWarAllianceNotice b_Request)
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

            G2C_AddWarAllianceNotice g2C_AddWarAllianceNotice = new G2C_AddWarAllianceNotice();
            g2C_AddWarAllianceNotice.Member = new Struct_MemberInfo();
            g2C_AddWarAllianceNotice.Member.GameUserID = b_Request.Member.GameUserID;
            g2C_AddWarAllianceNotice.Member.MemberName = b_Request.Member.MemberName;
            g2C_AddWarAllianceNotice.Member.MemberLevel = b_Request.Member.MemberLevel;
            g2C_AddWarAllianceNotice.Member.MemberClassType = b_Request.Member.MemberClassType;
            g2C_AddWarAllianceNotice.Member.MemberPost = b_Request.Member.MemberPost;
            g2C_AddWarAllianceNotice.Member.ClassTypeLevel = 0;
            g2C_AddWarAllianceNotice.Member.MeberState = b_Request.Member.MeberState;
            mPlayer.Send(g2C_AddWarAllianceNotice);
            return true;
        }
    }
}