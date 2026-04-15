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
    public class G2M_AddWarAllianceRequestHandler : AMActorRpcHandler<G2M_AddWarAllianceRequest, M2G_AddWarAllianceResponse>
    {
        protected override async Task<bool> Run(G2M_AddWarAllianceRequest b_Request, M2G_AddWarAllianceResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2505);
                b_Reply(b_Response);
                return false;
            }

            var warAllianceMmber = Waralliancecomponent.GetWarAlliance(b_Request.WarAllianceID);
            if (warAllianceMmber == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520);
                b_Reply(b_Response);
                return false;
            }
            if (warAllianceMmber.MemberList.Count >= warAllianceMmber.WarAllianceLevel)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2531);
                b_Reply(b_Response);
                return false;
            }
            GMStruct_MemberInfo gMStruct_MemberInfo = b_Request.GameMemeber;
            if (gMStruct_MemberInfo == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2516);
                b_Reply(b_Response);
                return false;
            }

            if (warAllianceMmber.CheckMember(gMStruct_MemberInfo.GameUserID,out MemberInfo member))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2521);
                b_Reply(b_Response);
                return false;
            }

            DBMemberInfo dBMemberInfo = new DBMemberInfo();
            dBMemberInfo.MemberID = gMStruct_MemberInfo.GameUserID;
            dBMemberInfo.MemberName = gMStruct_MemberInfo.MemberName;
            dBMemberInfo.MemberLevel = gMStruct_MemberInfo.MemberLevel;
            dBMemberInfo.MemberClassType = gMStruct_MemberInfo.MemberClassType;
            dBMemberInfo.MemberPost = 0;
            dBMemberInfo.DBWarAllianceID = b_Request.WarAllianceID;
            dBMemberInfo.DeleteTime = Help_TimeHelper.GetNowSecond();
            dBMemberInfo.MemberAreaId = (int)(b_Request.AppendData >> 16);
            dBMemberInfo.AllianceScore = 0;
            dBMemberInfo.IsDisabled = 0;

            if (!Waralliancecomponent.AddWarAllianceMember(dBMemberInfo, gMStruct_MemberInfo.GameServerID))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2522);
                b_Reply(b_Response);
                return false;
            }
            int mAreaId = (int)(b_Request.AppendData >> 16);
            await Waralliancecomponent.SetWarAllianceMemberDB(dBMemberInfo, mAreaId);

            foreach (var Member in warAllianceMmber.MemberList)
            {
                if(Member.Value.MemberID == gMStruct_MemberInfo.GameUserID)continue;

                if (Member.Value.MemberPost == (int)PostType.AllianceLeader || Member.Value.MemberPost == (int)PostType.ViceAllianceLeader)
                {
                    if (Member.Value.MeberState == 0)
                    {
                        M2G_AddWarAllianceNotice m2G_AddWarAllianceNotice = new M2G_AddWarAllianceNotice();
                        m2G_AddWarAllianceNotice.ActorId = Member.Value.MemberID;

                        m2G_AddWarAllianceNotice.Member = new GMStruct_MemberInfo();
                        m2G_AddWarAllianceNotice.GameUserID = Member.Value.MemberID;
                        m2G_AddWarAllianceNotice.Member.GameUserID = dBMemberInfo.MemberID;
                        m2G_AddWarAllianceNotice.Member.MemberName = dBMemberInfo.MemberName;
                        m2G_AddWarAllianceNotice.Member.MemberLevel = dBMemberInfo.MemberLevel;
                        m2G_AddWarAllianceNotice.Member.MemberClassType = dBMemberInfo.MemberClassType;
                        m2G_AddWarAllianceNotice.Member.MemberPost = dBMemberInfo.MemberPost;
                        m2G_AddWarAllianceNotice.Member.MeberState = 0;

                        warAllianceMmber.SendNoticeMember(m2G_AddWarAllianceNotice, Member.Value.MeberServerID);
                    }
                }
            }
            b_Reply(b_Response);
            return true;

        }
    }
}