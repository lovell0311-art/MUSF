using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_GetRankInfoRequestHandler : AMActorRpcHandler<G2M_GetRankInfoRequest, M2G_GetRankInfoResponse>
    {
        protected override async Task<bool> Run(G2M_GetRankInfoRequest b_Request, M2G_GetRankInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            var Rankcomponent = Root.MainFactory.GetCustomComponent<RankComponent>();
            if (Rankcomponent != null) 
            {
                var List = Rankcomponent.GetRankList((RankType)b_Request.RankType);
                foreach (var Info in List)
                {
                    Rank_status rank_Status = new Rank_status();
                    switch ((RankType)b_Request.RankType)
                    { 
                        case RankType.LevelRank:
                            {
                                rank_Status.Value64A = Info.GetGameUserId();
                                rank_Status.Value64B = int.Parse(Info.GetClassType());
                                rank_Status.Value32A = Info.GetReincarnate();
                                rank_Status.Value32B = Info.GetLevel();
                                rank_Status.StrA = Info.GetName();
                                rank_Status.StrB = Info.GetRanking().ToString();
                            }
                            break;
                        default:
                            continue;
                    }
                    b_Response.RankList.Add(rank_Status);
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}