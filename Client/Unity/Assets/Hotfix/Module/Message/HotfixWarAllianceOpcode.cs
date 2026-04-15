using ETModel;
namespace ETHotfix
{
// Opcode = 34000
//CG创建战盟
	[Message(HotfixWarAllianceOpcode.C2G_WarAllianceEstablishRequest)]
	public partial class C2G_WarAllianceEstablishRequest : IActorRequest {}

//战盟名称
//战盟徽章
//CG创建战盟  返回
	[Message(HotfixWarAllianceOpcode.G2C_WarAllianceEstablishResponse)]
	public partial class G2C_WarAllianceEstablishResponse : IActorResponse {}

//打开战盟
	[Message(HotfixWarAllianceOpcode.C2G_OpenWarAllianceRequest)]
	public partial class C2G_OpenWarAllianceRequest : IActorRequest {}

//打开战盟 返回
	[Message(HotfixWarAllianceOpcode.G2C_OpenWarAllianceResponse)]
	public partial class G2C_OpenWarAllianceResponse : IActorResponse {}

//解散战盟
	[Message(HotfixWarAllianceOpcode.C2G_DisbandTheWarAllianceRequest)]
	public partial class C2G_DisbandTheWarAllianceRequest : IActorRequest {}

//解散战盟 返回
	[Message(HotfixWarAllianceOpcode.G2C_DisbandTheWarAllianceResponse)]
	public partial class G2C_DisbandTheWarAllianceResponse : IActorResponse {}

//打开成员列表
	[Message(HotfixWarAllianceOpcode.C2G_OpenMemberListRequest)]
	public partial class C2G_OpenMemberListRequest : IActorRequest {}

//类型 0是成员 1是申请
//打开成员列表 返回
	[Message(HotfixWarAllianceOpcode.G2C_OpenMemberListResponse)]
	public partial class G2C_OpenMemberListResponse : IActorResponse {}

//修改公告
	[Message(HotfixWarAllianceOpcode.C2G_ModifyAnnouncementRequest)]
	public partial class C2G_ModifyAnnouncementRequest : IActorRequest {}

//公告
//修改公告 返回
	[Message(HotfixWarAllianceOpcode.G2C_ModifyAnnouncementResponse)]
	public partial class G2C_ModifyAnnouncementResponse : IActorResponse {}

//任命
	[Message(HotfixWarAllianceOpcode.C2G_WarAllianceAppointmentRequest)]
	public partial class C2G_WarAllianceAppointmentRequest : IActorRequest {}

//UserID
//职务 0成员 1小队长 2副盟主 3盟主
//任命 返回
	[Message(HotfixWarAllianceOpcode.G2C_WarAllianceAppointmentResponse)]
	public partial class G2C_WarAllianceAppointmentResponse : IActorResponse {}

//任命通知
	[Message(HotfixWarAllianceOpcode.G2C_WarAllianceAppointmentNotice)]
	public partial class G2C_WarAllianceAppointmentNotice : IActorMessage {}

//职务 0成员 1小队长 2副盟主 3盟主
//踢出
	[Message(HotfixWarAllianceOpcode.C2G_WarAllianceProposeRequest)]
	public partial class C2G_WarAllianceProposeRequest : IActorRequest {}

//UserID
//踢出 返回
	[Message(HotfixWarAllianceOpcode.G2C_WarAllianceProposeResponse)]
	public partial class G2C_WarAllianceProposeResponse : IActorResponse {}

//踢出 通知
	[Message(HotfixWarAllianceOpcode.G2C_WarAllianceProposeNotice)]
	public partial class G2C_WarAllianceProposeNotice : IActorMessage {}

//同意或拒绝
	[Message(HotfixWarAllianceOpcode.C2G_WarAllianceAgreeOrRejectRequest)]
	public partial class C2G_WarAllianceAgreeOrRejectRequest : IActorRequest {}

//UserID 数组
//Type 0是拒绝 1是同意
//同意或拒绝 返回
	[Message(HotfixWarAllianceOpcode.G2C_WarAllianceAgreeOrRejectResponse)]
	public partial class G2C_WarAllianceAgreeOrRejectResponse : IActorResponse {}

//同意或拒绝 通知
	[Message(HotfixWarAllianceOpcode.G2C_WarAllianceAgreeOrRejectNotice)]
	public partial class G2C_WarAllianceAgreeOrRejectNotice : IActorMessage {}

//退出
	[Message(HotfixWarAllianceOpcode.C2G_WarAllianceSignOutRequest)]
	public partial class C2G_WarAllianceSignOutRequest : IActorRequest {}

//退出 返回
	[Message(HotfixWarAllianceOpcode.G2C_WarAllianceSignOutResponse)]
	public partial class G2C_WarAllianceSignOutResponse : IActorResponse {}

//申请
	[Message(HotfixWarAllianceOpcode.C2G_AddWarAllianceRequest)]
	public partial class C2G_AddWarAllianceRequest : IActorRequest {}

//申请 返回
	[Message(HotfixWarAllianceOpcode.G2C_AddWarAllianceResponse)]
	public partial class G2C_AddWarAllianceResponse : IActorResponse {}

//申请 通知
	[Message(HotfixWarAllianceOpcode.G2C_AddWarAllianceNotice)]
	public partial class G2C_AddWarAllianceNotice : IActorMessage {}

//加入战盟
	[Message(HotfixWarAllianceOpcode.C2G_AddWarAllianceListRequest)]
	public partial class C2G_AddWarAllianceListRequest : IActorRequest {}

//加入战盟 返回
	[Message(HotfixWarAllianceOpcode.G2C_AddWarAllianceListResponse)]
	public partial class G2C_AddWarAllianceListResponse : IActorResponse {}

//搜索战盟
	[Message(HotfixWarAllianceOpcode.C2G_SearchWarAllianceRequest)]
	public partial class C2G_SearchWarAllianceRequest : IActorRequest {}

//搜索战盟 返回
	[Message(HotfixWarAllianceOpcode.G2C_SearchWarAllianceResponse)]
	public partial class G2C_SearchWarAllianceResponse : IActorResponse {}

//ID
//Name
//MemberCnt
//盟主名
//名字检查
	[Message(HotfixWarAllianceOpcode.C2G_CheackWarAllianceNameRequest)]
	public partial class C2G_CheackWarAllianceNameRequest : IActorRequest {}

//名字检查 返回
	[Message(HotfixWarAllianceOpcode.G2C_CheackWarAllianceNameResponse)]
	public partial class G2C_CheackWarAllianceNameResponse : IActorResponse {}

//战盟聊天
	[Message(HotfixWarAllianceOpcode.C2G_SendMessageWarAllianceChatRequest)]
	public partial class C2G_SendMessageWarAllianceChatRequest : IActorRequest {}

//战盟聊天 返回
	[Message(HotfixWarAllianceOpcode.G2C_SendMessageWarAllianceChatResponse)]
	public partial class G2C_SendMessageWarAllianceChatResponse : IActorResponse {}

//战盟聊天 通知
	[Message(HotfixWarAllianceOpcode.G2C_SendMessageWarAllianceChatNotice)]
	public partial class G2C_SendMessageWarAllianceChatNotice : IActorMessage {}

//是否加入战盟
	[Message(HotfixWarAllianceOpcode.C2G_JoinTheWarAllianceRequest)]
	public partial class C2G_JoinTheWarAllianceRequest : IActorRequest {}

	[Message(HotfixWarAllianceOpcode.G2C_JoinTheWarAllianceResponse)]
	public partial class G2C_JoinTheWarAllianceResponse : IActorResponse {}

	[Message(HotfixWarAllianceOpcode.G2C_KickOutOfTheWarNotice)]
	public partial class G2C_KickOutOfTheWarNotice : IActorMessage {}

	[Message(HotfixWarAllianceOpcode.C2G_DisableTheAllianceCache)]
	public partial class C2G_DisableTheAllianceCache : IActorRequest {}

	[Message(HotfixWarAllianceOpcode.G2C_DisableTheAllianceCache)]
	public partial class G2C_DisableTheAllianceCache : IActorResponse {}

	[Message(HotfixWarAllianceOpcode.Struct_WarAllinceInfo)]
	public partial class Struct_WarAllinceInfo {}

//ID
//Name
//Badge
//MemberCnt
//Notice
//Post
//AllianceLeaderName
//当前战盟人数
	[Message(HotfixWarAllianceOpcode.Struct_MemberInfo)]
	public partial class Struct_MemberInfo {}

//UserID
//Name
//level
//ClassType
//Post 0:成员 1:队长 2:副盟主  2:盟主
//state
//转职等级
}
namespace ETHotfix
{
	public static partial class HotfixWarAllianceOpcode
	{
		 public const ushort C2G_WarAllianceEstablishRequest = 34001;
		 public const ushort G2C_WarAllianceEstablishResponse = 34002;
		 public const ushort C2G_OpenWarAllianceRequest = 34003;
		 public const ushort G2C_OpenWarAllianceResponse = 34004;
		 public const ushort C2G_DisbandTheWarAllianceRequest = 34005;
		 public const ushort G2C_DisbandTheWarAllianceResponse = 34006;
		 public const ushort C2G_OpenMemberListRequest = 34007;
		 public const ushort G2C_OpenMemberListResponse = 34008;
		 public const ushort C2G_ModifyAnnouncementRequest = 34009;
		 public const ushort G2C_ModifyAnnouncementResponse = 34010;
		 public const ushort C2G_WarAllianceAppointmentRequest = 34011;
		 public const ushort G2C_WarAllianceAppointmentResponse = 34012;
		 public const ushort G2C_WarAllianceAppointmentNotice = 34013;
		 public const ushort C2G_WarAllianceProposeRequest = 34014;
		 public const ushort G2C_WarAllianceProposeResponse = 34015;
		 public const ushort G2C_WarAllianceProposeNotice = 34016;
		 public const ushort C2G_WarAllianceAgreeOrRejectRequest = 34017;
		 public const ushort G2C_WarAllianceAgreeOrRejectResponse = 34018;
		 public const ushort G2C_WarAllianceAgreeOrRejectNotice = 34019;
		 public const ushort C2G_WarAllianceSignOutRequest = 34020;
		 public const ushort G2C_WarAllianceSignOutResponse = 34021;
		 public const ushort C2G_AddWarAllianceRequest = 34022;
		 public const ushort G2C_AddWarAllianceResponse = 34023;
		 public const ushort G2C_AddWarAllianceNotice = 34024;
		 public const ushort C2G_AddWarAllianceListRequest = 34025;
		 public const ushort G2C_AddWarAllianceListResponse = 34026;
		 public const ushort C2G_SearchWarAllianceRequest = 34027;
		 public const ushort G2C_SearchWarAllianceResponse = 34028;
		 public const ushort C2G_CheackWarAllianceNameRequest = 34029;
		 public const ushort G2C_CheackWarAllianceNameResponse = 34030;
		 public const ushort C2G_SendMessageWarAllianceChatRequest = 34031;
		 public const ushort G2C_SendMessageWarAllianceChatResponse = 34032;
		 public const ushort G2C_SendMessageWarAllianceChatNotice = 34033;
		 public const ushort C2G_JoinTheWarAllianceRequest = 34034;
		 public const ushort G2C_JoinTheWarAllianceResponse = 34035;
		 public const ushort G2C_KickOutOfTheWarNotice = 34036;
		 public const ushort C2G_DisableTheAllianceCache = 34037;
		 public const ushort G2C_DisableTheAllianceCache = 34038;
		 public const ushort Struct_WarAllinceInfo = 34039;
		 public const ushort Struct_MemberInfo = 34040;
	}
}
