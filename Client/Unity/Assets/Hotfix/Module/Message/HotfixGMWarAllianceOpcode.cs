using ETModel;
namespace ETHotfix
{
// Opcode = 32000
//GM创建战盟
	[Message(HotfixGMWarAllianceOpcode.G2M_WarAllianceEstablishRequest)]
	public partial class G2M_WarAllianceEstablishRequest : IActorRequest {}

//战盟名称
//创建者信息
//圣导师的统率
//战盟徽章
//创建者信息
//GM创建战盟 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_WarAllianceEstablishResponse)]
	public partial class M2G_WarAllianceEstablishResponse : IActorResponse {}

//解散战盟
	[Message(HotfixGMWarAllianceOpcode.G2M_DisbandTheWarAllianceRequest)]
	public partial class G2M_DisbandTheWarAllianceRequest : IActorRequest {}

//解散战盟 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_DisbandTheWarAllianceResponse)]
	public partial class M2G_DisbandTheWarAllianceResponse : IActorResponse {}

//GM获取成员列表
	[Message(HotfixGMWarAllianceOpcode.G2M_OpenMemberListRequest)]
	public partial class G2M_OpenMemberListRequest : IActorRequest {}

//战盟ID
//类型 0是成员 1是申请
//GMS获取成员列表 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_OpenMemberListResponse)]
	public partial class M2G_OpenMemberListResponse : IActorResponse {}

//打开战盟
	[Message(HotfixGMWarAllianceOpcode.G2M_OpenWarAllianceRequest)]
	public partial class G2M_OpenWarAllianceRequest : IActorRequest {}

//打开战盟 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_OpenWarAllianceResponse)]
	public partial class M2G_OpenWarAllianceResponse : IActorResponse {}

//修改公告
	[Message(HotfixGMWarAllianceOpcode.G2M_ModifyAnnouncementRequest)]
	public partial class G2M_ModifyAnnouncementRequest : IActorRequest {}

//公告
//修改公告 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_ModifyAnnouncementResponse)]
	public partial class M2G_ModifyAnnouncementResponse : IActorResponse {}

//任命
	[Message(HotfixGMWarAllianceOpcode.G2M_WarAllianceAppointmentRequest)]
	public partial class G2M_WarAllianceAppointmentRequest : IActorRequest {}

//UserID
//职务 0成员 1小队长 2副盟主 3盟主
//当前盟主的ID
//任命 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_WarAllianceAppointmentResponse)]
	public partial class M2G_WarAllianceAppointmentResponse : IActorResponse {}

//UserID
//任命通知
	[Message(HotfixGMWarAllianceOpcode.M2G_WarAllianceAppointmentNotice)]
	public partial class M2G_WarAllianceAppointmentNotice : IActorRequest {}

//UserID
//职务 0成员 1小队长 2副盟主 3盟主
//更新MGMT服务器上数据
	[Message(HotfixGMWarAllianceOpcode.G2M_UpdatePlayerWarAlliance)]
	public partial class G2M_UpdatePlayerWarAlliance : IActorMessage {}

//ID
//UserID
//level
//Serverid
//state
//Type申请过期删除 1是删除申请
//踢出
	[Message(HotfixGMWarAllianceOpcode.G2M_WarAllianceProposeRequest)]
	public partial class G2M_WarAllianceProposeRequest : IActorRequest {}

//UserID
//踢出 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_WarAllianceProposeResponse)]
	public partial class M2G_WarAllianceProposeResponse : IActorResponse {}

//同意或拒绝
	[Message(HotfixGMWarAllianceOpcode.G2M_WarAllianceAgreeOrRejectRequest)]
	public partial class G2M_WarAllianceAgreeOrRejectRequest : IActorRequest {}

//UserID 数组
//Type 0是拒绝 1是同意
//同意或拒绝 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_WarAllianceAgreeOrRejectResponse)]
	public partial class M2G_WarAllianceAgreeOrRejectResponse : IActorResponse {}

//同意或拒绝 通知
	[Message(HotfixGMWarAllianceOpcode.M2G_WarAllianceAgreeOrRejectNotice)]
	public partial class M2G_WarAllianceAgreeOrRejectNotice : IActorRequest {}

//申请
	[Message(HotfixGMWarAllianceOpcode.G2M_AddWarAllianceRequest)]
	public partial class G2M_AddWarAllianceRequest : IActorRequest {}

//申请 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_AddWarAllianceResponse)]
	public partial class M2G_AddWarAllianceResponse : IActorResponse {}

//申请 通知
	[Message(HotfixGMWarAllianceOpcode.M2G_AddWarAllianceNotice)]
	public partial class M2G_AddWarAllianceNotice : IActorRequest {}

//加入战盟
	[Message(HotfixGMWarAllianceOpcode.G2M_AddWarAllianceListRequest)]
	public partial class G2M_AddWarAllianceListRequest : IActorRequest {}

//加入战盟 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_AddWarAllianceListResponse)]
	public partial class M2G_AddWarAllianceListResponse : IActorResponse {}

//搜索战盟
	[Message(HotfixGMWarAllianceOpcode.M2G_SearchWarAllianceRequest)]
	public partial class M2G_SearchWarAllianceRequest : IActorRequest {}

//搜索战盟 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_SearchWarAllianceResponse)]
	public partial class M2G_SearchWarAllianceResponse : IActorResponse {}

//ID
//Name
//MemberCnt
//战盟名称
	[Message(HotfixGMWarAllianceOpcode.C2G_PlayerGetWarAlliacneNameRequest)]
	public partial class C2G_PlayerGetWarAlliacneNameRequest : IActorRequest {}

//战盟名称 返回
	[Message(HotfixGMWarAllianceOpcode.C2G_PlayerGetWarAlliacneNameResponse)]
	public partial class C2G_PlayerGetWarAlliacneNameResponse : IActorResponse {}

//退出
	[Message(HotfixGMWarAllianceOpcode.G2M_WarAllianceSignOutRequest)]
	public partial class G2M_WarAllianceSignOutRequest : IActorRequest {}

//退出 返回
	[Message(HotfixGMWarAllianceOpcode.M2G_WarAllianceSignOutResponse)]
	public partial class M2G_WarAllianceSignOutResponse : IActorResponse {}

//解散通知
	[Message(HotfixGMWarAllianceOpcode.M2G_DisbandTheWarAllianceNotice)]
	public partial class M2G_DisbandTheWarAllianceNotice : IActorRequest {}

// 战盟名称检查
	[Message(HotfixGMWarAllianceOpcode.G2M_CheackWarAllianceNameRequest)]
	public partial class G2M_CheackWarAllianceNameRequest : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_WCheackWarAllianceNameResponse)]
	public partial class M2G_WCheackWarAllianceNameResponse : IActorResponse {}

	[Message(HotfixGMWarAllianceOpcode.GMStruct_WarAllinceInfo)]
	public partial class GMStruct_WarAllinceInfo {}

//ID
//Name
//Badge
//MemberCnt
//Notice
//Post
//AllianceLeaderName
	[Message(HotfixGMWarAllianceOpcode.GMStruct_MemberInfo)]
	public partial class GMStruct_MemberInfo {}

//UserID
//Name
//level
//ClassType
//Post
//state
//serverid
}
namespace ETHotfix
{
	public static partial class HotfixGMWarAllianceOpcode
	{
		 public const ushort G2M_WarAllianceEstablishRequest = 32001;
		 public const ushort M2G_WarAllianceEstablishResponse = 32002;
		 public const ushort G2M_DisbandTheWarAllianceRequest = 32003;
		 public const ushort M2G_DisbandTheWarAllianceResponse = 32004;
		 public const ushort G2M_OpenMemberListRequest = 32005;
		 public const ushort M2G_OpenMemberListResponse = 32006;
		 public const ushort G2M_OpenWarAllianceRequest = 32007;
		 public const ushort M2G_OpenWarAllianceResponse = 32008;
		 public const ushort G2M_ModifyAnnouncementRequest = 32009;
		 public const ushort M2G_ModifyAnnouncementResponse = 32010;
		 public const ushort G2M_WarAllianceAppointmentRequest = 32011;
		 public const ushort M2G_WarAllianceAppointmentResponse = 32012;
		 public const ushort M2G_WarAllianceAppointmentNotice = 32013;
		 public const ushort G2M_UpdatePlayerWarAlliance = 32014;
		 public const ushort G2M_WarAllianceProposeRequest = 32015;
		 public const ushort M2G_WarAllianceProposeResponse = 32016;
		 public const ushort G2M_WarAllianceAgreeOrRejectRequest = 32017;
		 public const ushort M2G_WarAllianceAgreeOrRejectResponse = 32018;
		 public const ushort M2G_WarAllianceAgreeOrRejectNotice = 32019;
		 public const ushort G2M_AddWarAllianceRequest = 32020;
		 public const ushort M2G_AddWarAllianceResponse = 32021;
		 public const ushort M2G_AddWarAllianceNotice = 32022;
		 public const ushort G2M_AddWarAllianceListRequest = 32023;
		 public const ushort M2G_AddWarAllianceListResponse = 32024;
		 public const ushort M2G_SearchWarAllianceRequest = 32025;
		 public const ushort M2G_SearchWarAllianceResponse = 32026;
		 public const ushort C2G_PlayerGetWarAlliacneNameRequest = 32027;
		 public const ushort C2G_PlayerGetWarAlliacneNameResponse = 32028;
		 public const ushort G2M_WarAllianceSignOutRequest = 32029;
		 public const ushort M2G_WarAllianceSignOutResponse = 32030;
		 public const ushort M2G_DisbandTheWarAllianceNotice = 32031;
		 public const ushort G2M_CheackWarAllianceNameRequest = 32032;
		 public const ushort M2G_WCheackWarAllianceNameResponse = 32033;
		 public const ushort GMStruct_WarAllinceInfo = 32034;
		 public const ushort GMStruct_MemberInfo = 32035;
	}
}
