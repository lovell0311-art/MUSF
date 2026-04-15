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
    public class G2M_WarAllianceEstablishHandler : AMActorRpcHandler<G2M_WarAllianceEstablishRequest, M2G_WarAllianceEstablishResponse>
    {
        protected override async Task<bool> Run(G2M_WarAllianceEstablishRequest b_Request, M2G_WarAllianceEstablishResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent =Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2528);
                b_Reply(b_Response);
                return false;
            }

            int Cnt = b_Request.PlayerInfo.MemberLevel / 10;
            //if (b_Request.PlayerInfo.MemberClassType == (int)E_GameOccupation.Holyteacher)
            //{
            //    Cnt = b_Request.PlayerInfo.MemberLevel / 10 + b_Request.Command /10;
            //}
            //else
            //{
            //    Cnt = b_Request.PlayerInfo.MemberLevel / 10;
            //}
            int mAreaId = (int)(b_Request.AppendData >> 16);
            DBWarAllianceData warAllianceInfo =new DBWarAllianceData();
            warAllianceInfo.DBWarAllianceID = IdGeneraterNew.Instance.GenerateUnitId(mAreaId);
            warAllianceInfo.DBWarAllianceName = b_Request.WarAllianceName;
            warAllianceInfo.DBWarAllianceLevel = Cnt;
            warAllianceInfo.DBWarAllianceBadge = b_Request.WarAllianceBadge.array;
            warAllianceInfo.DBWarAllianAreaId = mAreaId;
            warAllianceInfo.DBWarAllianNotice = "";
            warAllianceInfo.IsDisabled = 0;

            DBMemberInfo memberInfo = new DBMemberInfo();
            memberInfo.DBWarAllianceID = warAllianceInfo.DBWarAllianceID;
            memberInfo.MemberID = b_Request.PlayerInfo.GameUserID;
            memberInfo.MemberName = b_Request.PlayerInfo.MemberName;
            memberInfo.MemberLevel = b_Request.PlayerInfo.MemberLevel;
            memberInfo.MemberPost = (int)PostType.AllianceLeader;
            memberInfo.MemberClassType = b_Request.PlayerInfo.MemberClassType;
            memberInfo.MemberAreaId = mAreaId;
            memberInfo.IsDisabled = 1;

            Waralliancecomponent.AddWarAlliance(warAllianceInfo, mAreaId);
            Waralliancecomponent.AddWarAllianceMember(memberInfo, b_Request.PlayerInfo.GameServerID);

            if (!await Waralliancecomponent.SetWarAllianceDB(warAllianceInfo, mAreaId))
            {
                Log.Error($"战盟数据写库失败 ID{warAllianceInfo.DBWarAllianceID} Name{warAllianceInfo.DBWarAllianceName}");
                
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2528);
                b_Reply(b_Response);
                return false;
            }

            if (!await Waralliancecomponent.SetWarAllianceMemberDB(memberInfo, mAreaId))
            {
                Log.Error($"战盟成员写库失败 战盟ID{memberInfo.DBWarAllianceID} 角色ID{memberInfo.MemberID} 角色Name{memberInfo.MemberName}");

                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2528);
                b_Reply(b_Response);
                return false;
            }
            b_Response.Info = new GMStruct_WarAllinceInfo();
            b_Response.Info.WarAllianceID = warAllianceInfo.DBWarAllianceID;
            b_Response.Info.WarAllianceName = warAllianceInfo.DBWarAllianceName;
            b_Response.Info.LeaderName = memberInfo.MemberName;
            b_Response.Info.WarAllianceLevel = warAllianceInfo.DBWarAllianceLevel;
            b_Response.Info.WarAllianceNotice = warAllianceInfo.DBWarAllianNotice;

            for (int i=0; i < warAllianceInfo.DBWarAllianceBadge.Length;i++)
            {
                b_Response.Info.WarAllianceBadge.Add(warAllianceInfo.DBWarAllianceBadge[i]);
            }
            b_Response.Info.MemberPost = (int)PostType.AllianceLeader;
            b_Reply(b_Response);
            return true;
        }
    }
}