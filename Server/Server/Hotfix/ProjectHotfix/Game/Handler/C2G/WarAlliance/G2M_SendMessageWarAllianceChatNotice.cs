using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class G2M_SendMessageWarAllianceChatNoticeHandler : AMHandler<G2M_SendMessageWarAllianceChatNotice>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, G2M_SendMessageWarAllianceChatNotice b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(G2M_SendMessageWarAllianceChatNotice b_Request)
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

            G2C_SendMessageWarAllianceChatNotice g2C_SendMessageWarAllianceChatNotice = new G2C_SendMessageWarAllianceChatNotice();
            g2C_SendMessageWarAllianceChatNotice.SendGameUserId = b_Request.SendGameUserId;
            g2C_SendMessageWarAllianceChatNotice.SendUserName = b_Request.SendUserName;
            g2C_SendMessageWarAllianceChatNotice.SendDataTime = Help_TimeHelper.GetNowSecond();
            g2C_SendMessageWarAllianceChatNotice.ChatMessage = b_Request.ChatMessage;
            mPlayer.Send(g2C_SendMessageWarAllianceChatNotice);
            return true;
        }
    }
}