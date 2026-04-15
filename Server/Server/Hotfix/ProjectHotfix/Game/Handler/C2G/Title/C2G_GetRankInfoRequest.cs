using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetRankInfoRequestHandler : AMActorRpcHandler<C2G_GetRankInfoRequest, G2C_GetRankInfoResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetRankInfoRequest b_Request, G2C_GetRankInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetRankInfoRequest b_Request, G2C_GetRankInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2500);
                b_Reply(b_Response);
                return false;
            }

            G2M_GetRankInfoRequest g2M_GetRankInfoRequest = new G2M_GetRankInfoRequest();
            g2M_GetRankInfoRequest.RankType = b_Request.RankType;
            IResponse Message = await mPlayer.GetSessionMGMT().Call(g2M_GetRankInfoRequest) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2201);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;//Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2502);
                b_Reply(b_Response);
                return false;
            }
            else
            {
                M2G_GetRankInfoResponse m2G_GetRankInfoResponse = Message as M2G_GetRankInfoResponse;
                if (m2G_GetRankInfoResponse == null) return false;
                b_Response.GameUserID = mPlayer.GameUserId;
                foreach (var Info in m2G_GetRankInfoResponse.RankList)
                {
                    Rank_status rank_status = new Rank_status();
                    rank_status.Value64A = Info.Value64A;
                    rank_status.Value64B = Info.Value64B;
                    rank_status.Value32A = Info.Value32A;
                    rank_status.Value32B = Info.Value32B;
                    rank_status.StrA = Info.StrA;
                    rank_status.StrB = Info.StrB;
                    b_Response.RankList.Add(rank_status);
                }
                b_Reply(b_Response);
                return true;
            }
        }
    }
}