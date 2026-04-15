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
    public class G2M_AddWarAllianceListRequestHandler : AMActorRpcHandler<G2M_AddWarAllianceListRequest, M2G_AddWarAllianceListResponse>
    {
        protected override async Task<bool> Run(G2M_AddWarAllianceListRequest b_Request, M2G_AddWarAllianceListResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2502);
                b_Reply(b_Response);
                return false;
            }
            var bEId = Waralliancecomponent.CheckGameUser(b_Request.GameUserID);
            var mList = Waralliancecomponent.WarAllianceList.Keys.ToList();
            List<long> longs = new List<long>();
            if (b_Request.Type == 0 && bEId.Item1 != 0)
            {
                int index = mList.FindIndex(value => value == bEId.Item1);
                if (index == 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (mList.Count <= i) break;

                        longs.Add(mList[i]);
                    }
                }
                for (int i = 1; i <= 10; i++)
                {
                    if (index - i < 0) break;

                    longs.Add(mList[index - i]);
                }
                if (index != 0)
                {
                    longs.Reverse();
                }
            }
            else if(b_Request.Type == 1)
            {
                if (bEId.Item2 == 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (mList.Count <= i) break;

                        longs.Add(mList[i]);
                    }
                }
                else
                {
                    int index = mList.FindIndex(value => value == bEId.Item2);
                    for (int i = 1; i <= 10; i++)
                    {
                        if (mList.Count <= index + i) break;
                        
                        longs.Add(mList[index+i]);
                    }
                }
            }
            if (longs.Count <= 0)
            {
                b_Response.Error = 2532;
                b_Reply(b_Response);
                return true;
            }
            (long, long) EBId = (longs.First(), longs[longs.Count - 1]);
            Waralliancecomponent.UpGameUser(b_Request.GameUserID, EBId);
            foreach (var Id in longs)
            {
                if (Waralliancecomponent.WarAllianceList.TryGetValue(Id, out var mWarAllianceInfo))
                {
                    GMStruct_WarAllinceInfo gMStruct_WarAllinceInfo = new GMStruct_WarAllinceInfo();
                    gMStruct_WarAllinceInfo.WarAllianceID = mWarAllianceInfo.WarAllianceID;
                    gMStruct_WarAllinceInfo.WarAllianceName = mWarAllianceInfo.WarAllianceName;
                    gMStruct_WarAllinceInfo.WarAllianceLevel = mWarAllianceInfo.WarAllianceLevel;
                    gMStruct_WarAllinceInfo.LeaderName = mWarAllianceInfo.AllianceLeaderName;
                    gMStruct_WarAllinceInfo.Currentquantity = mWarAllianceInfo.MemberList.Count;
                    gMStruct_WarAllinceInfo.WarAllianceBadge.AddRange(mWarAllianceInfo.WarAllianceBadge);
                    gMStruct_WarAllinceInfo.WarAllianceNotice = mWarAllianceInfo.WarAllianNotice;
                    
                    b_Response.WAInfo.Add(gMStruct_WarAllinceInfo);
                }
            }

            b_Reply(b_Response);
            return true;

        }
    }
}