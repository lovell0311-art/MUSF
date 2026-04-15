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
    public class G2M_OpenMemberListRequestHandler : AMActorRpcHandler<G2M_OpenMemberListRequest, M2G_OpenMemberListResponse>
    {
        protected override async Task<bool> Run(G2M_OpenMemberListRequest b_Request, M2G_OpenMemberListResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2512);;// .ERR_WarAllianceDataError;
                b_Reply(b_Response);
                return false;
            }

            var warAllianceMmber = Waralliancecomponent.GetWarAllianceMember(b_Request.WarAllianceID, b_Request.Type);
            if (warAllianceMmber == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520);// .ERR_WarAllianceInfoError;
                b_Reply(b_Response);
                return false;
            }

            foreach (var Info in warAllianceMmber)
            {
                GMStruct_MemberInfo gMStruct_MemberInfo = new GMStruct_MemberInfo();
                gMStruct_MemberInfo.GameUserID = Info.Value.MemberID;
                gMStruct_MemberInfo.MemberName = Info.Value.MemberName;
                gMStruct_MemberInfo.MemberLevel = Info.Value.MemberLevel;
                gMStruct_MemberInfo.MemberClassType = Info.Value.MemberClassType;
                gMStruct_MemberInfo.MemberPost = Info.Value.MemberPost;
                gMStruct_MemberInfo.MeberState = Info.Value.MeberState;

                b_Response.MemberList.Add(gMStruct_MemberInfo);
            }
            b_Reply(b_Response);
            return true;

        }
    }
}