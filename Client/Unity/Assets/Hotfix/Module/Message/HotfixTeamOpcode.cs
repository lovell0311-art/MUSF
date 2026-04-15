using ETModel;
namespace ETHotfix
{
// Opcode = 24000
//组队玩家属性
	[Message(HotfixTeamOpcode.PlayerInTeamProperty)]
	public partial class PlayerInTeamProperty : IMessage {}

//玩家在队伍中的状态
	[Message(HotfixTeamOpcode.PlayerInTeamStatus)]
	public partial class PlayerInTeamStatus : IMessage {}

// 是队长
//队伍列表数据
	[Message(HotfixTeamOpcode.TeamData)]
	public partial class TeamData : IMessage {}

// 玩家数据
	[Message(HotfixTeamOpcode.Team_PlayerData)]
	public partial class Team_PlayerData : IMessage {}

// Opcode = 24100
// 请求 创建队伍 成功会推送G2C_MySelfEnterTeam_notice
	[Message(HotfixTeamOpcode.C2G_CreateTeam)]
	public partial class C2G_CreateTeam : IActorRequest {}

// 返回 创建队伍
	[Message(HotfixTeamOpcode.G2C_CreateTeam)]
	public partial class G2C_CreateTeam : IActorResponse {}

// 请求 获取附近队伍列表
	[Message(HotfixTeamOpcode.C2G_GetNearbyTeamList)]
	public partial class C2G_GetNearbyTeamList : IActorRequest {}

// 返回 获取附近队伍列表
	[Message(HotfixTeamOpcode.G2C_GetNearbyTeamList)]
	public partial class G2C_GetNearbyTeamList : IActorResponse {}

// 请求 邀请其他玩家进入队伍  邀请结果推送至G2C_InvitePlayerEnterTeam_notice
	[Message(HotfixTeamOpcode.C2G_InvitePlayerEnterTeam)]
	public partial class C2G_InvitePlayerEnterTeam : IActorRequest {}

// 邀请进入队伍的玩家
// 返回 邀请其他玩家进入队伍
	[Message(HotfixTeamOpcode.G2C_InvitePlayerEnterTeam)]
	public partial class G2C_InvitePlayerEnterTeam : IActorResponse {}

// 请求 同意或拒绝玩家邀请
	[Message(HotfixTeamOpcode.C2G_ReplyPlayerInvitation)]
	public partial class C2G_ReplyPlayerInvitation : IActorRequest {}

// 邀请进入队伍的玩家
// false.拒绝  true.同意
// false.手动回应  true.自动回应 对方将不会收到返回消息
// 返回 同意或拒绝玩家邀请
	[Message(HotfixTeamOpcode.G2C_ReplyPlayerInvitation)]
	public partial class G2C_ReplyPlayerInvitation : IActorResponse {}

// 请求 将玩家踢出队伍
	[Message(HotfixTeamOpcode.C2G_KcikThePlayerOutOfTheTeam)]
	public partial class C2G_KcikThePlayerOutOfTheTeam : IActorRequest {}

// 要踢出的玩家
// 返回 将玩家踢出队伍
	[Message(HotfixTeamOpcode.G2C_KcikThePlayerOutOfTheTeam)]
	public partial class G2C_KcikThePlayerOutOfTheTeam : IActorResponse {}

// 请求 申请加入队伍  结果推送至G2C_ApplyToJoinTheTeam_notice
	[Message(HotfixTeamOpcode.C2G_ApplyToJoinTheTeam)]
	public partial class C2G_ApplyToJoinTheTeam : IActorRequest {}

// 要进入的队伍玩家ID
// 返回 申请加入队伍
	[Message(HotfixTeamOpcode.G2C_ApplyToJoinTheTeam)]
	public partial class G2C_ApplyToJoinTheTeam : IActorResponse {}

// 请求 同意或拒绝玩家申请
	[Message(HotfixTeamOpcode.C2G_ReplyPlayerApply)]
	public partial class C2G_ReplyPlayerApply : IActorRequest {}

// 邀请进入队伍的玩家
// false.拒绝  true.同意
// false.手动回应  true.自动回应 对方将不会收到返回消息
// 返回 同意或拒绝玩家申请
	[Message(HotfixTeamOpcode.G2C_ReplyPlayerApply)]
	public partial class G2C_ReplyPlayerApply : IActorResponse {}

// 请求 离开队伍
	[Message(HotfixTeamOpcode.C2G_LeaveTheTeam)]
	public partial class C2G_LeaveTheTeam : IActorRequest {}

// 返回 离开队伍
	[Message(HotfixTeamOpcode.G2C_LeaveTheTeam)]
	public partial class G2C_LeaveTheTeam : IActorResponse {}

// 请求 让位队长
	[Message(HotfixTeamOpcode.C2G_HandTheCaptain)]
	public partial class C2G_HandTheCaptain : IActorRequest {}

// 让位目标玩家
// 返回 让位队长
	[Message(HotfixTeamOpcode.G2C_HandTheCaptain)]
	public partial class G2C_HandTheCaptain : IActorResponse {}

// 请求 获取附近队伍列表
	[Message(HotfixTeamOpcode.C2G_GetNearbyPlayerList)]
	public partial class C2G_GetNearbyPlayerList : IActorRequest {}

// 返回 获取附近队伍列表
	[Message(HotfixTeamOpcode.G2C_GetNearbyPlayerList)]
	public partial class G2C_GetNearbyPlayerList : IActorResponse {}

//========================= 服务端推送 ==============================
// Opcode = 24500
//通知 其他玩家邀请加入团队
	[Message(HotfixTeamOpcode.G2C_PlayerInviteJoinTheTeam_notice)]
	public partial class G2C_PlayerInviteJoinTheTeam_notice : IActorMessage {}

// 邀请自己加入队伍的玩家
// 玩家实体昵称
//通知 其他玩家申请加入团队
	[Message(HotfixTeamOpcode.G2C_PlayerApplyJoinTheTeam_notice)]
	public partial class G2C_PlayerApplyJoinTheTeam_notice : IActorMessage {}

// 申请加入队伍的玩家
// 通知 自身进入队伍
// 如需队伍聊天时，HotfixChatMessage.proto->C2G_EnterChatRoom 通过此消息订阅 ChatRoomID 传递的房间。
// 房间id 需客户端进行记录,用于离开队伍时退订
	[Message(HotfixTeamOpcode.G2C_MySelfEnterTeam_notice)]
	public partial class G2C_MySelfEnterTeam_notice : IActorMessage {}

// 聊天房间id
// 自己的状态
// 其他玩家状态
//通知 自身离开队伍 收到消息后，记得退订聊天房间
	[Message(HotfixTeamOpcode.G2C_MySelfLeaveTeam_notice)]
	public partial class G2C_MySelfLeaveTeam_notice : IActorMessage {}

//通知 自己所在的队伍中，有其他玩家加入或状态改变时推送
	[Message(HotfixTeamOpcode.G2C_OtherPlayerEnterMyTeam_notice)]
	public partial class G2C_OtherPlayerEnterMyTeam_notice : IActorMessage {}

// 进入的玩家或玩家状态改变(状态改变包括自己,比如改变队长)
//通知 自己所在的队伍中，有其他玩家离开
	[Message(HotfixTeamOpcode.G2C_OtherPlayerLeaveMyTeam_notice)]
	public partial class G2C_OtherPlayerLeaveMyTeam_notice : IActorMessage {}

// 离开的玩家
//通知 队伍中实体的属性变动  （暂未实现，等待属性监听接口）
	[Message(HotfixTeamOpcode.G2C_PlayerPropChangeInTheTeam_notice)]
	public partial class G2C_PlayerPropChangeInTheTeam_notice : IActorMessage {}

// 变动的玩家
// 变动的玩家状态
//通知 广播其他玩家进入队伍
// 可视范围内，有玩家进入队伍时，此消息会到达
// 刚进入视野的玩家主角，如在队伍中。此消息会到达
// 用于区分，玩家是否在队伍中。
	[Message(HotfixTeamOpcode.G2C_PlayerEnterOtherTeam_notice)]
	public partial class G2C_PlayerEnterOtherTeam_notice : IActorMessage {}

// 进入其他队伍的玩家
//通知 广播其他玩家离开队伍
	[Message(HotfixTeamOpcode.G2C_PlayerLeaveOtherTeam_notice)]
	public partial class G2C_PlayerLeaveOtherTeam_notice : IActorMessage {}

// 离开其他队伍的玩家
//通知 邀请被拒绝或同意后推送
	[Message(HotfixTeamOpcode.G2C_InvitePlayerEnterTeam_notice)]
	public partial class G2C_InvitePlayerEnterTeam_notice : IActorMessage {}

// 被邀请的玩家
// 邀请状态
// true.玩家同意邀请
// false.玩家拒绝邀请
//通知 申请被拒绝或同意后推送
	[Message(HotfixTeamOpcode.G2C_ApplyToJoinTheTeam_notice)]
	public partial class G2C_ApplyToJoinTheTeam_notice : IActorMessage {}

// 申请加入队伍中的队长
// 申请状态
// true.队长同意申请
// false.队长拒绝申请
//队伍传送
	[Message(HotfixTeamOpcode.C2G_TeamDeliveryRequest)]
	public partial class C2G_TeamDeliveryRequest : IActorRequest {}

//ID
//好友传送 返回
	[Message(HotfixTeamOpcode.G2C_TeamDeliveryResponse)]
	public partial class G2C_TeamDeliveryResponse : IActorResponse {}

	[Message(HotfixTeamOpcode.C2G_OpenNewEnhanceItem)]
	public partial class C2G_OpenNewEnhanceItem : IActorRequest {}

//ID
	[Message(HotfixTeamOpcode.G2C_OpenNewEnhanceItem)]
	public partial class G2C_OpenNewEnhanceItem : IActorResponse {}

	[Message(HotfixTeamOpcode.C2G_NewEnhanceItem)]
	public partial class C2G_NewEnhanceItem : IActorRequest {}

	[Message(HotfixTeamOpcode.G2C_NewEnhanceItem)]
	public partial class G2C_NewEnhanceItem : IActorResponse {}

	[Message(HotfixTeamOpcode.NewEnhanceItemInfo)]
	public partial class NewEnhanceItemInfo {}

	[Message(HotfixTeamOpcode.ExpendItemInfo)]
	public partial class ExpendItemInfo {}

	[Message(HotfixTeamOpcode.C2G_OpenItemCrafting)]
	public partial class C2G_OpenItemCrafting : IActorRequest {}

//ID
	[Message(HotfixTeamOpcode.G2C_OpenItemCrafting)]
	public partial class G2C_OpenItemCrafting : IActorResponse {}

	[Message(HotfixTeamOpcode.C2G_ItemCrafting)]
	public partial class C2G_ItemCrafting : IActorRequest {}

	[Message(HotfixTeamOpcode.G2C_ItemCrafting)]
	public partial class G2C_ItemCrafting : IActorResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixTeamOpcode
	{
		 public const ushort PlayerInTeamProperty = 24001;
		 public const ushort PlayerInTeamStatus = 24002;
		 public const ushort TeamData = 24003;
		 public const ushort Team_PlayerData = 24004;
		 public const ushort C2G_CreateTeam = 24101;
		 public const ushort G2C_CreateTeam = 24102;
		 public const ushort C2G_GetNearbyTeamList = 24103;
		 public const ushort G2C_GetNearbyTeamList = 24104;
		 public const ushort C2G_InvitePlayerEnterTeam = 24105;
		 public const ushort G2C_InvitePlayerEnterTeam = 24106;
		 public const ushort C2G_ReplyPlayerInvitation = 24107;
		 public const ushort G2C_ReplyPlayerInvitation = 24108;
		 public const ushort C2G_KcikThePlayerOutOfTheTeam = 24109;
		 public const ushort G2C_KcikThePlayerOutOfTheTeam = 24110;
		 public const ushort C2G_ApplyToJoinTheTeam = 24111;
		 public const ushort G2C_ApplyToJoinTheTeam = 24112;
		 public const ushort C2G_ReplyPlayerApply = 24113;
		 public const ushort G2C_ReplyPlayerApply = 24114;
		 public const ushort C2G_LeaveTheTeam = 24115;
		 public const ushort G2C_LeaveTheTeam = 24116;
		 public const ushort C2G_HandTheCaptain = 24117;
		 public const ushort G2C_HandTheCaptain = 24118;
		 public const ushort C2G_GetNearbyPlayerList = 24119;
		 public const ushort G2C_GetNearbyPlayerList = 24120;
		 public const ushort G2C_PlayerInviteJoinTheTeam_notice = 24501;
		 public const ushort G2C_PlayerApplyJoinTheTeam_notice = 24502;
		 public const ushort G2C_MySelfEnterTeam_notice = 24503;
		 public const ushort G2C_MySelfLeaveTeam_notice = 24504;
		 public const ushort G2C_OtherPlayerEnterMyTeam_notice = 24505;
		 public const ushort G2C_OtherPlayerLeaveMyTeam_notice = 24506;
		 public const ushort G2C_PlayerPropChangeInTheTeam_notice = 24507;
		 public const ushort G2C_PlayerEnterOtherTeam_notice = 24508;
		 public const ushort G2C_PlayerLeaveOtherTeam_notice = 24509;
		 public const ushort G2C_InvitePlayerEnterTeam_notice = 24510;
		 public const ushort G2C_ApplyToJoinTheTeam_notice = 24511;
		 public const ushort C2G_TeamDeliveryRequest = 24512;
		 public const ushort G2C_TeamDeliveryResponse = 24513;
		 public const ushort C2G_OpenNewEnhanceItem = 24514;
		 public const ushort G2C_OpenNewEnhanceItem = 24515;
		 public const ushort C2G_NewEnhanceItem = 24516;
		 public const ushort G2C_NewEnhanceItem = 24517;
		 public const ushort NewEnhanceItemInfo = 24518;
		 public const ushort ExpendItemInfo = 24519;
		 public const ushort C2G_OpenItemCrafting = 24520;
		 public const ushort G2C_OpenItemCrafting = 24521;
		 public const ushort C2G_ItemCrafting = 24522;
		 public const ushort G2C_ItemCrafting = 24523;
	}
}
