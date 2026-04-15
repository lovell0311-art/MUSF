using ETModel;
namespace ETHotfix
{
// Opcode = 41000
//进入战斗副本请求
	[Message(HotfixBattleCopyOpcode.C2G_EnterBattleCopyRequest)]
	public partial class C2G_EnterBattleCopyRequest : IActorRequest {}

//进入战斗副本 响应
	[Message(HotfixBattleCopyOpcode.G2C_EnterBattleCopyResponse)]
	public partial class G2C_EnterBattleCopyResponse : IActorResponse {}

//退出战斗副本请求
	[Message(HotfixBattleCopyOpcode.C2G_QuitBattleCopyRequest)]
	public partial class C2G_QuitBattleCopyRequest : IActorRequest {}

//退出战斗副本 响应
	[Message(HotfixBattleCopyOpcode.G2C_QuitBattleCopyResponse)]
	public partial class G2C_QuitBattleCopyResponse : IActorResponse {}

//副本状态
	[Message(HotfixBattleCopyOpcode.BattleCopyState)]
	public partial class BattleCopyState {}

//获取副本信息 请求
	[Message(HotfixBattleCopyOpcode.C2G_GetBattleCopyInfoRequest)]
	public partial class C2G_GetBattleCopyInfoRequest : IActorRequest {}

//获取副本信息 响应
	[Message(HotfixBattleCopyOpcode.G2C_GetBattleCopyInfoRequest)]
	public partial class G2C_GetBattleCopyInfoRequest : IActorResponse {}

// 房间变更  通知
	[Message(HotfixBattleCopyOpcode.G2C_BattleCopyStateUpdate_notice)]
	public partial class G2C_BattleCopyStateUpdate_notice : IActorMessage {}

//血色城堡房间状态  通知
	[Message(HotfixBattleCopyOpcode.G2C_CopyRoomStateUpdate_notice)]
	public partial class G2C_CopyRoomStateUpdate_notice : IActorMessage {}

//副本通用击杀怪物数量状态  通知
	[Message(HotfixBattleCopyOpcode.G2C_CopyRoomKillMonster_notice)]
	public partial class G2C_CopyRoomKillMonster_notice : IActorMessage {}

//repeated int32 AllMonsterId 	= 1;		// 怪物id
	[Message(HotfixBattleCopyOpcode.BatteCopyRankData)]
	public partial class BatteCopyRankData {}

// 副本排名通知
	[Message(HotfixBattleCopyOpcode.G2C_BattleCopyRanks_notice)]
	public partial class G2C_BattleCopyRanks_notice : IActorMessage {}

//攻击血色城堡(大门/水晶棺) 请求
	[Message(HotfixBattleCopyOpcode.C2G_AttackCopyObstacleRequest)]
	public partial class C2G_AttackCopyObstacleRequest : IActorRequest {}

//攻击血色城堡(大门/水晶棺) 响应
	[Message(HotfixBattleCopyOpcode.G2C_AttackCopyObstacleRequest)]
	public partial class G2C_AttackCopyObstacleRequest : IActorResponse {}

//提交大天使武器 请求
	[Message(HotfixBattleCopyOpcode.C2G_CommitArchangelWeaponRequest)]
	public partial class C2G_CommitArchangelWeaponRequest : IActorRequest {}

//提交大天使武器) 响应
	[Message(HotfixBattleCopyOpcode.G2C_CommitArchangelWeaponRequest)]
	public partial class G2C_CommitArchangelWeaponRequest : IActorResponse {}

// 副本排名通知
	[Message(HotfixBattleCopyOpcode.G2C_RedCastleSettlement_notice)]
	public partial class G2C_RedCastleSettlement_notice : IActorMessage {}

// 大门和棺材的攻击次数
	[Message(HotfixBattleCopyOpcode.G2C_NumberOfttacks_notice)]
	public partial class G2C_NumberOfttacks_notice : IActorMessage {}

//藏宝图位置  通知
	[Message(HotfixBattleCopyOpcode.G2C_CangBaoTuPosUpdate_notice)]
	public partial class G2C_CangBaoTuPosUpdate_notice : IActorMessage {}

//藏宝图开启 通知组队
	[Message(HotfixBattleCopyOpcode.G2C_CangBaoTuOpen_notice)]
	public partial class G2C_CangBaoTuOpen_notice : IActorMessage {}

// 藏宝图位置关闭  通知
	[Message(HotfixBattleCopyOpcode.G2C_CangBaoTuPosClose_notice)]
	public partial class G2C_CangBaoTuPosClose_notice : IActorMessage {}

//点击NPC打开界面查看信息
	[Message(HotfixBattleCopyOpcode.C2G_OpenClimbingTowerNPC)]
	public partial class C2G_OpenClimbingTowerNPC : IActorRequest {}

	[Message(HotfixBattleCopyOpcode.G2C_OpenClimbingTowerNPC)]
	public partial class G2C_OpenClimbingTowerNPC : IActorResponse {}

//进入试炼之地
	[Message(HotfixBattleCopyOpcode.C2G_EnterTheTestTower)]
	public partial class C2G_EnterTheTestTower : IActorRequest {}

	[Message(HotfixBattleCopyOpcode.G2C_EnterTheTestTower)]
	public partial class G2C_EnterTheTestTower : IActorResponse {}

//本层怪物清理完毕会通知一下前端
	[Message(HotfixBattleCopyOpcode.C2G_NotificationReward)]
	public partial class C2G_NotificationReward : IActorMessage {}

//领奖进入下一层或者领奖退出
	[Message(HotfixBattleCopyOpcode.C2G_SetOutToChallenge)]
	public partial class C2G_SetOutToChallenge : IActorRequest {}

	[Message(HotfixBattleCopyOpcode.G2C_SetOutToChallenge)]
	public partial class G2C_SetOutToChallenge : IActorResponse {}

	[Message(HotfixBattleCopyOpcode.G2C_SendDamageRanking)]
	public partial class G2C_SendDamageRanking : IActorMessage {}

	[Message(HotfixBattleCopyOpcode.RanKingInfo)]
	public partial class RanKingInfo {}

	[Message(HotfixBattleCopyOpcode.C2G_SweepTheTrialGrounds)]
	public partial class C2G_SweepTheTrialGrounds : IActorRequest {}

	[Message(HotfixBattleCopyOpcode.G2C_SweepTheTrialGrounds)]
	public partial class G2C_SweepTheTrialGrounds : IActorResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixBattleCopyOpcode
	{
		 public const ushort C2G_EnterBattleCopyRequest = 41001;
		 public const ushort G2C_EnterBattleCopyResponse = 41002;
		 public const ushort C2G_QuitBattleCopyRequest = 41003;
		 public const ushort G2C_QuitBattleCopyResponse = 41004;
		 public const ushort BattleCopyState = 41005;
		 public const ushort C2G_GetBattleCopyInfoRequest = 41006;
		 public const ushort G2C_GetBattleCopyInfoRequest = 41007;
		 public const ushort G2C_BattleCopyStateUpdate_notice = 41008;
		 public const ushort G2C_CopyRoomStateUpdate_notice = 41009;
		 public const ushort G2C_CopyRoomKillMonster_notice = 41010;
		 public const ushort BatteCopyRankData = 41011;
		 public const ushort G2C_BattleCopyRanks_notice = 41012;
		 public const ushort C2G_AttackCopyObstacleRequest = 41013;
		 public const ushort G2C_AttackCopyObstacleRequest = 41014;
		 public const ushort C2G_CommitArchangelWeaponRequest = 41015;
		 public const ushort G2C_CommitArchangelWeaponRequest = 41016;
		 public const ushort G2C_RedCastleSettlement_notice = 41017;
		 public const ushort G2C_NumberOfttacks_notice = 41018;
		 public const ushort G2C_CangBaoTuPosUpdate_notice = 41019;
		 public const ushort G2C_CangBaoTuOpen_notice = 41020;
		 public const ushort G2C_CangBaoTuPosClose_notice = 41021;
		 public const ushort C2G_OpenClimbingTowerNPC = 41022;
		 public const ushort G2C_OpenClimbingTowerNPC = 41023;
		 public const ushort C2G_EnterTheTestTower = 41024;
		 public const ushort G2C_EnterTheTestTower = 41025;
		 public const ushort C2G_NotificationReward = 41026;
		 public const ushort C2G_SetOutToChallenge = 41027;
		 public const ushort G2C_SetOutToChallenge = 41028;
		 public const ushort G2C_SendDamageRanking = 41029;
		 public const ushort RanKingInfo = 41030;
		 public const ushort C2G_SweepTheTrialGrounds = 41031;
		 public const ushort G2C_SweepTheTrialGrounds = 41032;
	}
}
