using ETModel;
namespace ETHotfix
{
// Opcode = 36000
// 任务
	[Message(HotfixTaskOpcode.Struct_TaskInfo)]
	public partial class Struct_TaskInfo {}

// 任务配置id
// 任务类型: id / 100000
// 状态
// 0.无状态  还没领取的任务，当玩家满足条件时，自动领取
// 1.进行中
// 2.已完成
// 3.已领取
// 任务进度
// 0.不是第一次推送
// 1.任务刚开始。第一次推给玩家
// ==================== 任务接口 ================
// Opcode = 36100
// 请求 领取任务
// 领取成功，会推送 G2C_UpdateGameTaskNotice
	[Message(HotfixTaskOpcode.C2G_ReceiveTask)]
	public partial class C2G_ReceiveTask : IActorRequest {}

// 返回 领取任务
	[Message(HotfixTaskOpcode.G2C_ReceiveTask)]
	public partial class G2C_ReceiveTask : IActorResponse {}

// 请求 放弃任务
// 放弃成功后，不会推送 任务变动的通知
	[Message(HotfixTaskOpcode.C2G_AbandonTask)]
	public partial class C2G_AbandonTask : IActorRequest {}

// 返回 放弃任务
	[Message(HotfixTaskOpcode.G2C_AbandonTask)]
	public partial class G2C_AbandonTask : IActorResponse {}

// 请求 领取任务奖励
// 领取任务奖励成功后，会推送 G2C_UpdateGameTaskNotice
	[Message(HotfixTaskOpcode.C2G_ReceiveTaskReward)]
	public partial class C2G_ReceiveTaskReward : IActorRequest {}

// 返回 领取任务奖励
	[Message(HotfixTaskOpcode.G2C_ReceiveTaskReward)]
	public partial class G2C_ReceiveTaskReward : IActorResponse {}

// 请求 更新任务进度
	[Message(HotfixTaskOpcode.C2G_UpdateTaskProgress)]
	public partial class C2G_UpdateTaskProgress : IActorRequest {}

// 返回 更新任务进度
	[Message(HotfixTaskOpcode.G2C_UpdateTaskProgress)]
	public partial class G2C_UpdateTaskProgress : IActorResponse {}

// ================== 通知 =========================
// Opcode = 36500
// 通知 更新任务
// 领取任务、任务进度变动、任务完成、领取奖励 都会推送
	[Message(HotfixTaskOpcode.G2C_UpdateGameTaskNotice)]
	public partial class G2C_UpdateGameTaskNotice : IActorMessage {}

// 通知 更新全部任务
// 进入游戏时通知
	[Message(HotfixTaskOpcode.G2C_AllUpdateGameTaskNotice)]
	public partial class G2C_AllUpdateGameTaskNotice : IActorMessage {}

// ================== 新手引导 =========================
// Opcode = 36600
// 请求 获取新手引导状态
	[Message(HotfixTaskOpcode.C2G_GetBeginnerGuideStatus)]
	public partial class C2G_GetBeginnerGuideStatus : IActorRequest {}

	[Message(HotfixTaskOpcode.G2C_GetBeginnerGuideStatus)]
	public partial class G2C_GetBeginnerGuideStatus : IActorResponse {}

// 状态
// 请求 设置新手引导状态
	[Message(HotfixTaskOpcode.C2G_SetBeginnerGuideStatus)]
	public partial class C2G_SetBeginnerGuideStatus : IActorRequest {}

// 状态
	[Message(HotfixTaskOpcode.G2C_SetBeginnerGuideStatus)]
	public partial class G2C_SetBeginnerGuideStatus : IActorResponse {}

	[Message(HotfixTaskOpcode.C2G_PurchaseOfTradeCard)]
	public partial class C2G_PurchaseOfTradeCard : IActorRequest {}

	[Message(HotfixTaskOpcode.G2C_PurchaseOfTradeCard)]
	public partial class G2C_PurchaseOfTradeCard : IActorResponse {}

	[Message(HotfixTaskOpcode.C2G_ReincarnateRequest)]
	public partial class C2G_ReincarnateRequest : IActorRequest {}

	[Message(HotfixTaskOpcode.G2C_ReincarnateResponse)]
	public partial class G2C_ReincarnateResponse : IActorResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixTaskOpcode
	{
		 public const ushort Struct_TaskInfo = 36001;
		 public const ushort C2G_ReceiveTask = 36101;
		 public const ushort G2C_ReceiveTask = 36102;
		 public const ushort C2G_AbandonTask = 36103;
		 public const ushort G2C_AbandonTask = 36104;
		 public const ushort C2G_ReceiveTaskReward = 36105;
		 public const ushort G2C_ReceiveTaskReward = 36106;
		 public const ushort C2G_UpdateTaskProgress = 36107;
		 public const ushort G2C_UpdateTaskProgress = 36108;
		 public const ushort G2C_UpdateGameTaskNotice = 36501;
		 public const ushort G2C_AllUpdateGameTaskNotice = 36502;
		 public const ushort C2G_GetBeginnerGuideStatus = 36601;
		 public const ushort G2C_GetBeginnerGuideStatus = 36602;
		 public const ushort C2G_SetBeginnerGuideStatus = 36603;
		 public const ushort G2C_SetBeginnerGuideStatus = 36604;
		 public const ushort C2G_PurchaseOfTradeCard = 36605;
		 public const ushort G2C_PurchaseOfTradeCard = 36606;
		 public const ushort C2G_ReincarnateRequest = 36607;
		 public const ushort G2C_ReincarnateResponse = 36608;
	}
}
