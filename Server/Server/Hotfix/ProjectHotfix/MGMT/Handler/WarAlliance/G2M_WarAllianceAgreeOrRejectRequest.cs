using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Ecm.V20190719.Models;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_WarAllianceAgreeOrRejectRequestHandler : AMActorRpcHandler<G2M_WarAllianceAgreeOrRejectRequest, M2G_WarAllianceAgreeOrRejectResponse>
    {
        protected override async Task<bool> Run(G2M_WarAllianceAgreeOrRejectRequest b_Request, M2G_WarAllianceAgreeOrRejectResponse b_Response, Action<IMessage> b_Reply)
        {
            //int mAreaId = (int)(b_Request.AppendData >> 16);
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520);
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
            long[] MemberList = b_Request.GameMemeber.array;
            if(MemberList.Length == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2516);
                b_Reply(b_Response);
                return false;
            }
            if (warAllianceMmber.WarAllianceLevel <= warAllianceMmber.MemberList.Count)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520);
                b_Reply(b_Response);
                return false;
            }
            M2G_WarAllianceAgreeOrRejectNotice m2G_WarAllianceAgreeOrReject = new M2G_WarAllianceAgreeOrRejectNotice();
            m2G_WarAllianceAgreeOrReject.WAInfo = new GMStruct_WarAllinceInfo();
            m2G_WarAllianceAgreeOrReject.Type = b_Request.Type;
            m2G_WarAllianceAgreeOrReject.WAInfo.WarAllianceID = warAllianceMmber.WarAllianceID;
            m2G_WarAllianceAgreeOrReject.WAInfo.WarAllianceName = warAllianceMmber.WarAllianceName;
            m2G_WarAllianceAgreeOrReject.WAInfo.WarAllianceLevel = warAllianceMmber.WarAllianceLevel;
            m2G_WarAllianceAgreeOrReject.WAInfo.WarAllianceBadge.AddRange(warAllianceMmber.WarAllianceBadge);
            m2G_WarAllianceAgreeOrReject.WAInfo.LeaderName = warAllianceMmber.AllianceLeaderName;
            m2G_WarAllianceAgreeOrReject.WAInfo.MemberPost = 0;
            m2G_WarAllianceAgreeOrReject.WAInfo.WarAllianceNotice = warAllianceMmber.WarAllianNotice;

            MemberInfo Info;
            foreach (var MemberID in MemberList)
            {
                if (MemberID == 0) break;

                DBMemberInfo dBMemberInfo = new DBMemberInfo();

                if (!warAllianceMmber.CheckMember(MemberID, out Info))
                    continue;

                dBMemberInfo.MemberLevel = Info.MemberLevel;
                dBMemberInfo.MemberName = Info.MemberName;
                dBMemberInfo.MemberID = Info.MemberID;
                dBMemberInfo.MemberPost = Info.MemberPost;
                dBMemberInfo.DBWarAllianceID = warAllianceMmber.WarAllianceID;
                dBMemberInfo.DeleteTime = 0;
                dBMemberInfo.MemberClassType = Info.MemberClassType;


                m2G_WarAllianceAgreeOrReject.ActorId = Info.MemberID;
                m2G_WarAllianceAgreeOrReject.GameUserID = Info.MemberID;

                if (b_Request.Type == 1)
                {
                    dBMemberInfo.IsDisabled = 1;
                    if (warAllianceMmber.AddMember(dBMemberInfo.MemberID, Info.MeberServerID, Info.MeberState))
                    {
                        await Waralliancecomponent.SetWarAllianceMemberDB(dBMemberInfo, OptionComponent.Options.ZoneId);

                        //if (Info.MeberState == 0)
                        warAllianceMmber.SendNoticeMember(m2G_WarAllianceAgreeOrReject, Info.MeberServerID);
                    }
                }
                else if (b_Request.Type == 0)
                {
                    dBMemberInfo.IsDisabled = 3;
                    var Memebr = warAllianceMmber.DeleteMember(MemberID, 1);

                    await Waralliancecomponent.SetWarAllianceMemberDB(dBMemberInfo, OptionComponent.Options.ZoneId);

                    //if (Info.MeberState == 0)
                    warAllianceMmber.SendNoticeMember(m2G_WarAllianceAgreeOrReject, Info.MeberServerID);
                }
            }
            b_Reply(b_Response);
            return true;
            /*GMStruct_MemberInfo[] gMStruct_MemberInfo = b_Request.GameMemeber.array;
            if (gMStruct_MemberInfo.Length == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2516);
                b_Reply(b_Response);
                return false;
            }

            M2G_WarAllianceAgreeOrRejectNotice m2G_WarAllianceAgreeOrReject = new M2G_WarAllianceAgreeOrRejectNotice();
            m2G_WarAllianceAgreeOrReject.WAInfo =new GMStruct_WarAllinceInfo();
            m2G_WarAllianceAgreeOrReject.Type = b_Request.Type;
            m2G_WarAllianceAgreeOrReject.WAInfo.WarAllianceID = warAllianceMmber.WarAllianceID;
            m2G_WarAllianceAgreeOrReject.WAInfo.WarAllianceName = warAllianceMmber.WarAllianceName;
            m2G_WarAllianceAgreeOrReject.WAInfo.WarAllianceLevel = warAllianceMmber.WarAllianceLevel;
            m2G_WarAllianceAgreeOrReject.WAInfo.WarAllianceBadge.AddRange(warAllianceMmber.WarAllianceBadge);
            m2G_WarAllianceAgreeOrReject.WAInfo.LeaderName = warAllianceMmber.AllianceLeaderName;
            m2G_WarAllianceAgreeOrReject.WAInfo.MemberPost = 0;
            m2G_WarAllianceAgreeOrReject.WAInfo.WarAllianceNotice = warAllianceMmber.WarAllianNotice;
            int AreaId ;
            foreach (var MemberInfo in gMStruct_MemberInfo)
            {
                if (MemberInfo == null) break;

                DBMemberInfo dBMemberInfo = new DBMemberInfo();

                if (!warAllianceMmber.CheckMember(MemberInfo.GameUserID,out AreaId))
                    continue;

                dBMemberInfo.MemberLevel = MemberInfo.MemberLevel;
                dBMemberInfo.MemberName = MemberInfo.MemberName;
                dBMemberInfo.MemberID = MemberInfo.GameUserID;
                dBMemberInfo.MemberPost = MemberInfo.MemberPost;
                dBMemberInfo.DBWarAllianceID = b_Request.WarAllianceID;
                dBMemberInfo.DeleteTime = 0;
                dBMemberInfo.MemberClassType = MemberInfo.MemberClassType;
                dBMemberInfo.MemberAreaId = AreaId;


                m2G_WarAllianceAgreeOrReject.AppendData = AreaId;
                m2G_WarAllianceAgreeOrReject.GameUserID = MemberInfo.GameUserID;

                if (b_Request.Type == 1)
                {
                    dBMemberInfo.IsDisabled = 1;
                    if (warAllianceMmber.AddMember(dBMemberInfo, MemberInfo.GameServerID, MemberInfo.MeberState))
                    {
                        Waralliancecomponent.SetWarAllianceMemberDB(dBMemberInfo, AreaId);

                        if(MemberInfo.MeberState == 0)
                            warAllianceMmber.SendNoticeMember(m2G_WarAllianceAgreeOrReject, MemberInfo.GameServerID);
                    }
                } else if (b_Request.Type == 0)
                {
                    dBMemberInfo.IsDisabled = 3;
                    var Memebr = warAllianceMmber.DeleteMember(MemberInfo.GameUserID, 1);

                    Waralliancecomponent.SetWarAllianceMemberDB(dBMemberInfo, AreaId);

                    if (MemberInfo.MeberState == 0)
                        warAllianceMmber.SendNoticeMember(m2G_WarAllianceAgreeOrReject, MemberInfo.GameServerID);
                }
            }
        b_Reply(b_Response);
        return true;*/
        }
    }
}