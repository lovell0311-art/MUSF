using ETModel;
using CustomFrameWork.Component;
namespace ETHotfix
{
// Opcode = 26000
// 请求 邀请交易  邀请结果推送G2C_ApplyToJoinTheTeam_notice  done
	[Message(HotfixExchangeOpcode.C2G_InvitePlayerExchange)]
	public partial class C2G_InvitePlayerExchange : IActorRequest {}

// 邀请交易的目标玩家
// 返回 邀请交易  done
	[Message(HotfixExchangeOpcode.G2C_InvitePlayerExchange)]
	public partial class G2C_InvitePlayerExchange : IActorResponse {}

// 请求 同意或拒绝玩家邀请  done
	[Message(HotfixExchangeOpcode.C2G_ReplyPlayerExchangeInvite)]
	public partial class C2G_ReplyPlayerExchangeInvite : IActorRequest {}

// 邀请交易的玩家
// false.拒绝  true.同意
// 返回 同意或拒绝玩家申请  返回成功则开始交易，打开交易面板  done
	[Message(HotfixExchangeOpcode.G2C_ReplyPlayerExchangeInvite)]
	public partial class G2C_ReplyPlayerExchangeInvite : IActorResponse {}

// 请求 放入交易物品  done
	[Message(HotfixExchangeOpcode.C2G_AddExchangeItem)]
	public partial class C2G_AddExchangeItem : IActorRequest {}

// 返回  放入交易物品  done
	[Message(HotfixExchangeOpcode.G2C_AddExchangeItem)]
	public partial class G2C_AddExchangeItem : IActorResponse {}

// 请求 移动交易物品  done
	[Message(HotfixExchangeOpcode.C2G_MoveExchangeItem)]
	public partial class C2G_MoveExchangeItem : IActorRequest {}

// 返回 移动交易物品  done
	[Message(HotfixExchangeOpcode.G2C_MoveExchangeItem)]
	public partial class G2C_MoveExchangeItem : IActorResponse {}

// 请求 移除交易物品  done
	[Message(HotfixExchangeOpcode.C2G_ReMoveExchangeItem)]
	public partial class C2G_ReMoveExchangeItem : IActorRequest {}

// 返回 移除交易物品  done
	[Message(HotfixExchangeOpcode.G2C_ReMoveExchangeItem)]
	public partial class G2C_ReMoveExchangeItem : IActorResponse {}

// 请求 设置交易金币数
	[Message(HotfixExchangeOpcode.C2G_SetExchangeGold)]
	public partial class C2G_SetExchangeGold : IActorRequest {}

// 返回 设置交易金币数
	[Message(HotfixExchangeOpcode.G2C_SetExchangeGold)]
	public partial class G2C_SetExchangeGold : IActorResponse {}

// 请求 交易状态改变
	[Message(HotfixExchangeOpcode.C2G_LockExchangeItem)]
	public partial class C2G_LockExchangeItem : IActorRequest {}

// 锁定状态
// true.锁定交易物品
// false.取消锁定
// 返回 交易状态改变
	[Message(HotfixExchangeOpcode.G2C_LockExchangeItem)]
	public partial class G2C_LockExchangeItem : IActorResponse {}

// 请求 取消交易
	[Message(HotfixExchangeOpcode.C2G_CancelExchange)]
	public partial class C2G_CancelExchange : IActorRequest {}

// 返回 取消交易
	[Message(HotfixExchangeOpcode.G2C_CancelExchange)]
	public partial class G2C_CancelExchange : IActorResponse {}

//========================= 服务端推送 ==============================
//通知 交易邀请推送 done
	[Message(HotfixExchangeOpcode.G2C_InvitePlayerExchange_notice)]
	public partial class G2C_InvitePlayerExchange_notice : IActorMessage {}

// 邀请自己交易的玩家
// 玩家实体昵称
//通知 交易邀请被拒绝或同意后推送  同意则打开交易窗口   done
	[Message(HotfixExchangeOpcode.G2C_InvitePlayerExchangeResult_notice)]
	public partial class G2C_InvitePlayerExchangeResult_notice : IActorMessage {}

// 被邀请交易的玩家
// 回复结果
// true.同意交易
// false.拒绝交易
//通知 有物品进入交易栏内  done
	[Message(HotfixExchangeOpcode.G2C_AddExchangeItem_notice)]
	public partial class G2C_AddExchangeItem_notice : IActorMessage {}

//通知 有物品在交易栏移动  done
	[Message(HotfixExchangeOpcode.G2C_MoveExchangeItem_notice)]
	public partial class G2C_MoveExchangeItem_notice : IActorMessage {}

//通知 有物品在交易栏离开  done
	[Message(HotfixExchangeOpcode.G2C_ReMoveExchange_notice)]
	public partial class G2C_ReMoveExchange_notice : IActorMessage {}

//通知 玩家锁定交易物品
	[Message(HotfixExchangeOpcode.G2C_LockExchangeItem_notice)]
	public partial class G2C_LockExchangeItem_notice : IActorMessage {}

// 锁定状态
// true.锁定交易物品
// false.取消锁定
//通知 交易结果 交易被终止时也推送此通知，交易结果为false
	[Message(HotfixExchangeOpcode.G2C_ExchangeResult_notice)]
	public partial class G2C_ExchangeResult_notice : IActorMessage {}

// 交易结果
// true.交易成功
// false.交易失败
//通知 对方交易金币变动
	[Message(HotfixExchangeOpcode.G2C_TargetSetExchangeGold_notice)]
	public partial class G2C_TargetSetExchangeGold_notice : IActorMessage {}

//---------------------------------血脉觉醒-----------------------------------
	[Message(HotfixExchangeOpcode.RingInfo)]
	public partial class RingInfo {}

//环Id
//当前环激活了的节点
	[Message(HotfixExchangeOpcode.BloodVesselInfo)]
	public partial class BloodVesselInfo {}

//血脉ID
//下一环开启时间
//使用状态
//环信息
//打开血脉觉醒界面
	[Message(HotfixExchangeOpcode.C2G_BloodVesselInterface)]
	public partial class C2G_BloodVesselInterface : IActorRequest {}

	[Message(HotfixExchangeOpcode.G2C_BloodVesselInterface)]
	public partial class G2C_BloodVesselInterface : IActorResponse {}

//拥有的所有血脉
//激活血脉
	[Message(HotfixExchangeOpcode.C2G_ActivateBloodVessels)]
	public partial class C2G_ActivateBloodVessels : IActorRequest {}

	[Message(HotfixExchangeOpcode.G2C_ActivateBloodVessels)]
	public partial class G2C_ActivateBloodVessels : IActorResponse {}

//净化血脉
	[Message(HotfixExchangeOpcode.C2G_PurifyBloodVessels)]
	public partial class C2G_PurifyBloodVessels : IActorRequest {}

	[Message(HotfixExchangeOpcode.G2C_PurifyBloodVessels)]
	public partial class G2C_PurifyBloodVessels : IActorResponse {}

//净化提速
	[Message(HotfixExchangeOpcode.C2G_CleanUpSpeedVessels)]
	public partial class C2G_CleanUpSpeedVessels : IActorRequest {}

	[Message(HotfixExchangeOpcode.G2C_CleanUpSpeedVessels)]
	public partial class G2C_CleanUpSpeedVessels : IActorResponse {}

//节点激活
	[Message(HotfixExchangeOpcode.C2G_NodeActivationVessels)]
	public partial class C2G_NodeActivationVessels : IActorRequest {}

//血脉Id
//环Id
//节点ID
	[Message(HotfixExchangeOpcode.G2C_NodeActivationVessels)]
	public partial class G2C_NodeActivationVessels : IActorResponse {}

//使用血脉
	[Message(HotfixExchangeOpcode.C2G_UseBloodLineVessels)]
	public partial class C2G_UseBloodLineVessels : IActorRequest {}

//血脉Id
	[Message(HotfixExchangeOpcode.G2C_UseBloodLineVessels)]
	public partial class G2C_UseBloodLineVessels : IActorResponse {}

//获取当前血脉信息
	[Message(HotfixExchangeOpcode.C2G_GetCurrentBloodInfoVessels)]
	public partial class C2G_GetCurrentBloodInfoVessels : IActorRequest {}

//血脉Id
	[Message(HotfixExchangeOpcode.G2C_GetCurrentBloodInfoVessels)]
	public partial class G2C_GetCurrentBloodInfoVessels : IActorResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixExchangeOpcode
	{
		 public const ushort C2G_InvitePlayerExchange = 26001;
		 public const ushort G2C_InvitePlayerExchange = 26002;
		 public const ushort C2G_ReplyPlayerExchangeInvite = 26003;
		 public const ushort G2C_ReplyPlayerExchangeInvite = 26004;
		 public const ushort C2G_AddExchangeItem = 26005;
		 public const ushort G2C_AddExchangeItem = 26006;
		 public const ushort C2G_MoveExchangeItem = 26007;
		 public const ushort G2C_MoveExchangeItem = 26008;
		 public const ushort C2G_ReMoveExchangeItem = 26009;
		 public const ushort G2C_ReMoveExchangeItem = 26010;
		 public const ushort C2G_SetExchangeGold = 26011;
		 public const ushort G2C_SetExchangeGold = 26012;
		 public const ushort C2G_LockExchangeItem = 26013;
		 public const ushort G2C_LockExchangeItem = 26014;
		 public const ushort C2G_CancelExchange = 26015;
		 public const ushort G2C_CancelExchange = 26016;
		 public const ushort G2C_InvitePlayerExchange_notice = 26017;
		 public const ushort G2C_InvitePlayerExchangeResult_notice = 26018;
		 public const ushort G2C_AddExchangeItem_notice = 26019;
		 public const ushort G2C_MoveExchangeItem_notice = 26020;
		 public const ushort G2C_ReMoveExchange_notice = 26021;
		 public const ushort G2C_LockExchangeItem_notice = 26022;
		 public const ushort G2C_ExchangeResult_notice = 26023;
		 public const ushort G2C_TargetSetExchangeGold_notice = 26024;
		 public const ushort RingInfo = 26025;
		 public const ushort BloodVesselInfo = 26026;
		 public const ushort C2G_BloodVesselInterface = 26027;
		 public const ushort G2C_BloodVesselInterface = 26028;
		 public const ushort C2G_ActivateBloodVessels = 26029;
		 public const ushort G2C_ActivateBloodVessels = 26030;
		 public const ushort C2G_PurifyBloodVessels = 26031;
		 public const ushort G2C_PurifyBloodVessels = 26032;
		 public const ushort C2G_CleanUpSpeedVessels = 26033;
		 public const ushort G2C_CleanUpSpeedVessels = 26034;
		 public const ushort C2G_NodeActivationVessels = 26035;
		 public const ushort G2C_NodeActivationVessels = 26036;
		 public const ushort C2G_UseBloodLineVessels = 26037;
		 public const ushort G2C_UseBloodLineVessels = 26038;
		 public const ushort C2G_GetCurrentBloodInfoVessels = 26039;
		 public const ushort G2C_GetCurrentBloodInfoVessels = 26040;
	}
}
