using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Smpn.V20190822.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_WarAllianceAppointmentRequestHandler : AMActorRpcHandler<G2M_WarAllianceAppointmentRequest, M2G_WarAllianceAppointmentResponse>
    {
        protected override async Task<bool> Run(G2M_WarAllianceAppointmentRequest b_Request, M2G_WarAllianceAppointmentResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2527);
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
            var MemberInfo = warAllianceMmber.GetMemberInfo(b_Request.GameUserID);
            if (MemberInfo == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2525);
                b_Reply(b_Response);
                return false;
            }
            if (b_Request.ClassType == (int)PostType.AllianceLeader)
            {
                warAllianceMmber.WarAllianceLevel = MemberInfo.MemberLevel>= 10 ?MemberInfo.MemberLevel / 10:1;
            }
            DBMemberInfo dBMemberInfo = new DBMemberInfo();
            dBMemberInfo.DBWarAllianceID = b_Request.WarAllianceID;
            dBMemberInfo.MemberID = MemberInfo.MemberID;
            dBMemberInfo.MemberName = MemberInfo.MemberName;
            dBMemberInfo.MemberLevel = MemberInfo.MemberLevel;
            dBMemberInfo.MemberPost = b_Request.ClassType;
            dBMemberInfo.MemberClassType = MemberInfo.MemberClassType;
            dBMemberInfo.AllianceScore = MemberInfo.AllianceScore;
            dBMemberInfo.IsDisabled = 1;

            if (!warAllianceMmber.UpDateMemberInfo(dBMemberInfo))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2525);
                b_Reply(b_Response);
                return false;
            }
            if (!await Waralliancecomponent.SetWarAllianceMemberDB(dBMemberInfo, warAllianceMmber.mAreaId))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2525);
                b_Reply(b_Response);
                return false;
            }
            if (b_Request.ClassType == (int)PostType.AllianceLeader)
            {
                var MemberInfo2 = warAllianceMmber.GetMemberInfo(b_Request.LeaderUserID);
                if (MemberInfo2 == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2525);
                    b_Reply(b_Response);
                    return false;
                }
                DBMemberInfo dBMemberInfo2 = new DBMemberInfo();
                dBMemberInfo2.DBWarAllianceID = b_Request.WarAllianceID;
                dBMemberInfo2.MemberID = MemberInfo2.MemberID;
                dBMemberInfo2.MemberName = MemberInfo2.MemberName;
                dBMemberInfo2.MemberLevel = MemberInfo2.MemberLevel;
                dBMemberInfo2.MemberPost = 0;
                dBMemberInfo2.MemberClassType = MemberInfo2.MemberClassType;
                dBMemberInfo2.AllianceScore = MemberInfo2.AllianceScore;
                dBMemberInfo2.IsDisabled = 1;

                if (!warAllianceMmber.UpDateMemberInfo(dBMemberInfo2))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2525);
                    b_Reply(b_Response);
                    return false;
                }
                warAllianceMmber.AllianceLeaderName = dBMemberInfo.MemberName;
                if (!await Waralliancecomponent.SetWarAllianceMemberDB(dBMemberInfo2, warAllianceMmber.mAreaId))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2525);
                    b_Reply(b_Response);
                    return false;
                }
            }

            if (b_Request.ClassType != 0)
            {
               var MemberList = Waralliancecomponent.GetWarAllianceMember(b_Request.WarAllianceID);
                if (MemberList != null)
                {
                    foreach (var Member in MemberList)
                    {
                        if (Member.Value.MeberState == 0)
                        {
                            M2G_WarAllianceAppointmentNotice m2G_WarAllianceAppointmentNotice = new M2G_WarAllianceAppointmentNotice();
                            m2G_WarAllianceAppointmentNotice.ActorId = Member.Value.MemberID;
                            m2G_WarAllianceAppointmentNotice.CharName = MemberInfo.MemberName;
                            m2G_WarAllianceAppointmentNotice.ClassType = b_Request.ClassType;
                            m2G_WarAllianceAppointmentNotice.GameUserID = Member.Value.MemberID;

                            warAllianceMmber.SendNoticeMember(m2G_WarAllianceAppointmentNotice, Member.Value.MeberServerID);
                        }
                    }
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}