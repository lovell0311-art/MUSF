using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_EnterRankingRequestHandler : AMActorRpcHandler<G2M_EnterRankingRequest, M2G_EnterRankingResponse>
    {
        protected override async Task<bool> Run(G2M_EnterRankingRequest b_Request, M2G_EnterRankingResponse b_Response, Action<IMessage> b_Reply)
        {
            var Rankcomponent = Root.MainFactory.GetCustomComponent<RankComponent>();
            if (Rankcomponent == null)
            {
                return false;
            }
            int mAreaId = (int)(b_Request.AppendData >> 16);
            RankType Type = (RankType)b_Request.RankType;
            RankStructure rankStructure = RankClass.CreateRankStructure();
            switch (Type)
            { 
                case RankType.LevelRank:
                    rankStructure.SetRankType(Type);
                    rankStructure.SetGameUserId(b_Request.Value64A);
                    rankStructure.SetLevel(b_Request.Value32A);
                    rankStructure.SetReincarnate(b_Request.Value32B);
                    rankStructure.SetName(b_Request.StrA);
                    rankStructure.SetClassType(b_Request.Value32C.ToString());
                    break;
            }
            Rankcomponent.AddRank(Type, rankStructure);
            b_Reply(b_Response);
            return true;
        }
    }
}
