using ETModel;
using CustomFrameWork.Component;
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
//repeated GMStruct_MemberInfo GameMemeber =1;
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

//战盟聊天
	[Message(HotfixGMWarAllianceOpcode.G2M_SendMessageWarAllianceChatRequest)]
	public partial class G2M_SendMessageWarAllianceChatRequest : IActorRequest {}

//战盟聊天 返回
	[Message(HotfixGMWarAllianceOpcode.G2M_SendMessageWarAllianceChatResponse)]
	public partial class G2M_SendMessageWarAllianceChatResponse : IActorResponse {}

//战盟聊天 通知
	[Message(HotfixGMWarAllianceOpcode.G2M_SendMessageWarAllianceChatNotice)]
	public partial class G2M_SendMessageWarAllianceChatNotice : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.G2M_SendDeleteList)]
	public partial class G2M_SendDeleteList : IActorMessage {}

	[Message(HotfixGMWarAllianceOpcode.G2M_SendTitleWarAllinceMember)]
	public partial class G2M_SendTitleWarAllinceMember : IActorMessage {}

	[Message(HotfixGMWarAllianceOpcode.GMStruct_WarAllinceInfo)]
	public partial class GMStruct_WarAllinceInfo {}

//ID
//Name
//Badge
//MemberCnt
//Notice
//Post
//AllianceLeaderName
//当前战盟人数
	[Message(HotfixGMWarAllianceOpcode.GMStruct_MemberInfo)]
	public partial class GMStruct_MemberInfo {}

//UserID
//Name
//level
//ClassType
//Post
//state
//serverid
//藏宝阁
	[Message(HotfixGMWarAllianceOpcode.GMTreasureHouseItemInfo)]
	public partial class GMTreasureHouseItemInfo {}

//打开藏宝阁
	[Message(HotfixGMWarAllianceOpcode.G2M_OpenTreasureHouse)]
	public partial class G2M_OpenTreasureHouse : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_OpenTreasureHouse)]
	public partial class M2G_OpenTreasureHouse : IActorResponse {}

//收索道具
	[Message(HotfixGMWarAllianceOpcode.G2M_SearchForItems)]
	public partial class G2M_SearchForItems : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_SearchForItems)]
	public partial class M2G_SearchForItems : IActorResponse {}

//删除道具信息
	[Message(HotfixGMWarAllianceOpcode.G2M_DleTreasureHouseItemInfo)]
	public partial class G2M_DleTreasureHouseItemInfo : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_DleTreasureHouseItemInfo)]
	public partial class M2G_DleTreasureHouseItemInfo : IActorResponse {}

//查看下一页
	[Message(HotfixGMWarAllianceOpcode.G2M_NextPage)]
	public partial class G2M_NextPage : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_NextPage)]
	public partial class M2G_NextPage : IActorResponse {}

//添加物品
	[Message(HotfixGMWarAllianceOpcode.G2M_AddTreasureHouseItemInfo)]
	public partial class G2M_AddTreasureHouseItemInfo : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_AddTreasureHouseItemInfo)]
	public partial class M2G_AddTreasureHouseItemInfo : IActorResponse {}

//清除角色缓存
	[Message(HotfixGMWarAllianceOpcode.G2M_DeletePlayerTreasureHouse)]
	public partial class G2M_DeletePlayerTreasureHouse : IActorMessage {}

//物品上架时间到期
	[Message(HotfixGMWarAllianceOpcode.M2G_ItemeXpire)]
	public partial class M2G_ItemeXpire : IActorMessage {}

	[Message(HotfixGMWarAllianceOpcode.M2G_SetPlayerTransactionRecord)]
	public partial class M2G_SetPlayerTransactionRecord : IActorMessage {}

	[Message(HotfixGMWarAllianceOpcode.G2G_UpDataShopItemInfo)]
	public partial class G2G_UpDataShopItemInfo : IActorMessage {}

	[Message(HotfixGMWarAllianceOpcode.M2G_KickOutOfTheWarNotice)]
	public partial class M2G_KickOutOfTheWarNotice : IActorMessage {}

	[Message(HotfixGMWarAllianceOpcode.G2M_DisableTheAllianceCache)]
	public partial class G2M_DisableTheAllianceCache : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_DisableTheAllianceCache)]
	public partial class M2G_DisableTheAllianceCache : IActorResponse {}

//解锁
	[Message(HotfixGMWarAllianceOpcode.M2G_NameLockRecord)]
	public partial class M2G_NameLockRecord : IActorMessage {}

//枷锁
	[Message(HotfixGMWarAllianceOpcode.G2M_NameLockRecordRequest)]
	public partial class G2M_NameLockRecordRequest : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_NameLockRecordResponse)]
	public partial class M2G_NameLockRecordResponse : IActorResponse {}

//获取账号的推广信息
	[Message(HotfixGMWarAllianceOpcode.G2M_GetPromotionCode)]
	public partial class G2M_GetPromotionCode : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_GetPromotionCode)]
	public partial class M2G_GetPromotionCode : IActorResponse {}

//更新等级
	[Message(HotfixGMWarAllianceOpcode.G2M_PromotionLevel)]
	public partial class G2M_PromotionLevel : IActorMessage {}

//推广码领奖验证
	[Message(HotfixGMWarAllianceOpcode.G2M_ClaimVerification)]
	public partial class G2M_ClaimVerification : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_ClaimVerification)]
	public partial class M2G_ClaimVerification : IActorResponse {}

//注册时有验证码就检查是否存在
	[Message(HotfixGMWarAllianceOpcode.G2M_CheckExtensionCode)]
	public partial class G2M_CheckExtensionCode : IActorRequest {}

	[Message(HotfixGMWarAllianceOpcode.M2G_CheckExtensionCode)]
	public partial class M2G_CheckExtensionCode : IActorResponse {}

	[Message(HotfixGMWarAllianceOpcode.GMInvitationInfo)]
	public partial class GMInvitationInfo {}

	[Message(HotfixGMWarAllianceOpcode.M2D_ExitDBServer)]
	public partial class M2D_ExitDBServer : IMessage {}

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
		 public const ushort G2M_SendMessageWarAllianceChatRequest = 32034;
		 public const ushort G2M_SendMessageWarAllianceChatResponse = 32035;
		 public const ushort G2M_SendMessageWarAllianceChatNotice = 32036;
		 public const ushort G2M_SendDeleteList = 32037;
		 public const ushort G2M_SendTitleWarAllinceMember = 32038;
		 public const ushort GMStruct_WarAllinceInfo = 32039;
		 public const ushort GMStruct_MemberInfo = 32040;
		 public const ushort GMTreasureHouseItemInfo = 32041;
		 public const ushort G2M_OpenTreasureHouse = 32042;
		 public const ushort M2G_OpenTreasureHouse = 32043;
		 public const ushort G2M_SearchForItems = 32044;
		 public const ushort M2G_SearchForItems = 32045;
		 public const ushort G2M_DleTreasureHouseItemInfo = 32046;
		 public const ushort M2G_DleTreasureHouseItemInfo = 32047;
		 public const ushort G2M_NextPage = 32048;
		 public const ushort M2G_NextPage = 32049;
		 public const ushort G2M_AddTreasureHouseItemInfo = 32050;
		 public const ushort M2G_AddTreasureHouseItemInfo = 32051;
		 public const ushort G2M_DeletePlayerTreasureHouse = 32052;
		 public const ushort M2G_ItemeXpire = 32053;
		 public const ushort M2G_SetPlayerTransactionRecord = 32054;
		 public const ushort G2G_UpDataShopItemInfo = 32055;
		 public const ushort M2G_KickOutOfTheWarNotice = 32056;
		 public const ushort G2M_DisableTheAllianceCache = 32057;
		 public const ushort M2G_DisableTheAllianceCache = 32058;
		 public const ushort M2G_NameLockRecord = 32059;
		 public const ushort G2M_NameLockRecordRequest = 32060;
		 public const ushort M2G_NameLockRecordResponse = 32061;
		 public const ushort G2M_GetPromotionCode = 32062;
		 public const ushort M2G_GetPromotionCode = 32063;
		 public const ushort G2M_PromotionLevel = 32064;
		 public const ushort G2M_ClaimVerification = 32065;
		 public const ushort M2G_ClaimVerification = 32066;
		 public const ushort G2M_CheckExtensionCode = 32067;
		 public const ushort M2G_CheckExtensionCode = 32068;
		 public const ushort GMInvitationInfo = 32069;
		 public const ushort M2D_ExitDBServer = 32070;
	}
}
