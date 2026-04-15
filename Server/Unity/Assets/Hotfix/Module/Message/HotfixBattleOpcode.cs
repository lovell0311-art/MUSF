using ETModel;
using CustomFrameWork.Component;
namespace ETHotfix
{
// Opcode = 20000
// 移动位置
	[Message(HotfixBattleOpcode.C2G_MovePosRequest)]
	public partial class C2G_MovePosRequest : IActorRequest {}

// 移动位置  返回
	[Message(HotfixBattleOpcode.G2C_MovePosResponse)]
	public partial class G2C_MovePosResponse : IActorResponse {}

	[Message(HotfixBattleOpcode.G2C_MovePos_notice)]
	public partial class G2C_MovePos_notice : IActorListMessage {}

// 攻击
	[Message(HotfixBattleOpcode.C2G_AttackRequest)]
	public partial class C2G_AttackRequest : IActorRequest {}

// 攻击返回
	[Message(HotfixBattleOpcode.G2C_AttackResponse)]
	public partial class G2C_AttackResponse : IActorResponse {}

// 攻击返回通知
	[Message(HotfixBattleOpcode.G2C_AttackStart_notice)]
	public partial class G2C_AttackStart_notice : IActorMessage {}

// 攻击返回通知
	[Message(HotfixBattleOpcode.G2C_AttackResult_notice)]
	public partial class G2C_AttackResult_notice : IActorMessage {}

// 攻击返回通知
	[Message(HotfixBattleOpcode.G2C_AttackBuffer_notice)]
	public partial class G2C_AttackBuffer_notice : IActorMessage {}

	[Message(HotfixBattleOpcode.G2C_AttackBufferEnd_notice)]
	public partial class G2C_AttackBufferEnd_notice : IActorMessage {}

// 键值对
	[Message(HotfixBattleOpcode.G2C_BattleKVData)]
	public partial class G2C_BattleKVData : IMessage {}

	[Message(HotfixBattleOpcode.G2C_ChangeValue_notice)]
	public partial class G2C_ChangeValue_notice : IActorMessage {}

// 属性界面
	[Message(HotfixBattleOpcode.C2G_PlayerPropertyRequest)]
	public partial class C2G_PlayerPropertyRequest : IActorRequest {}

// 属性界面 返回
	[Message(HotfixBattleOpcode.G2C_PlayerPropertyResponse)]
	public partial class G2C_PlayerPropertyResponse : IActorResponse {}

// 属性加点
	[Message(HotfixBattleOpcode.C2G_BattlePropertyAddPointRequest)]
	public partial class C2G_BattlePropertyAddPointRequest : IActorRequest {}

// 属性加点 返回
	[Message(HotfixBattleOpcode.G2C_BattlePropertyAddPointResponse)]
	public partial class G2C_BattlePropertyAddPointResponse : IActorResponse {}

// 掉落通知
	[Message(HotfixBattleOpcode.G2C_ItemDropData)]
	public partial class G2C_ItemDropData : IMessage {}

// 1 << 0 .有技能
// 1 << 2 .有幸运
// 1 << 3 .有卓越
// 1 << 4 .有套装
// 1 << 5 .有镶嵌
// (Quality & 1 << ? == 1 << ?) ? true : false
// 拾取保护时间 时间戳 utc 秒
// 击杀玩家列表，谁可以拾取
// 物品强化等级
// 套装id
// 生成类型
// 0.其他
// 1.玩家丢弃
// 2.怪物掉落
// 3.宝箱掉落
// 物品追加等级
// 掉落通知
	[Message(HotfixBattleOpcode.G2C_ItemDrop_notice)]
	public partial class G2C_ItemDrop_notice : IActorMessage {}

// 物品拾取
	[Message(HotfixBattleOpcode.C2G_BattlePickUpDropItemRequest)]
	public partial class C2G_BattlePickUpDropItemRequest : IActorRequest {}

// 在背包中的位置
// 物品拾取 返回
	[Message(HotfixBattleOpcode.G2C_BattlePickUpDropItemResponse)]
	public partial class G2C_BattlePickUpDropItemResponse : IActorResponse {}

// 拾取到背包的物品uid
	[Message(HotfixBattleOpcode.G2C_BattlePickUpDropItem_notice)]
	public partial class G2C_BattlePickUpDropItem_notice : IActorMessage {}

// 地图传送
	[Message(HotfixBattleOpcode.C2G_MapDeliveryRequest)]
	public partial class C2G_MapDeliveryRequest : IActorRequest {}

// 地图传送 返回
	[Message(HotfixBattleOpcode.G2C_MapDeliveryResponse)]
	public partial class G2C_MapDeliveryResponse : IActorResponse {}

// 好友地图传送
	[Message(HotfixBattleOpcode.C2G_FriendMapDeliveryRequest)]
	public partial class C2G_FriendMapDeliveryRequest : IActorRequest {}

// 好友地图传送 返回
	[Message(HotfixBattleOpcode.G2C_FriendMapDeliveryResponse)]
	public partial class G2C_FriendMapDeliveryResponse : IActorResponse {}

// 单位离开视野
	[Message(HotfixBattleOpcode.G2C_UnitLeaveView_notice)]
	public partial class G2C_UnitLeaveView_notice : IActorMessage {}

// Opcode = 21000
// 转职完成
	[Message(HotfixBattleOpcode.G2C_CareerChangeComplete_notice)]
	public partial class G2C_CareerChangeComplete_notice : IActorMessage {}

// 设置pk模式
	[Message(HotfixBattleOpcode.C2G_SetPlayerKillingRequest)]
	public partial class C2G_SetPlayerKillingRequest : IActorRequest {}

// 设置pk模式 返回
	[Message(HotfixBattleOpcode.G2C_SetPlayerKillingResponse)]
	public partial class G2C_SetPlayerKillingResponse : IActorResponse {}

// 领取新手buff
	[Message(HotfixBattleOpcode.C2G_XinShouBuffRequest)]
	public partial class C2G_XinShouBuffRequest : IActorRequest {}

// 领取新手buff 返回
	[Message(HotfixBattleOpcode.G2C_XinShouBuffResponse)]
	public partial class G2C_XinShouBuffResponse : IActorResponse {}

// 客户端加载场景完成通知
	[Message(HotfixBattleOpcode.C2G_LoadSceneCompleteRequest)]
	public partial class C2G_LoadSceneCompleteRequest : IActorRequest {}

// 客户端加载场景完成通知 返回
	[Message(HotfixBattleOpcode.G2C_LoadSceneCompleteResponse)]
	public partial class G2C_LoadSceneCompleteResponse : IActorResponse {}

	[Message(HotfixBattleOpcode.G2C_AttackDistance_notice)]
	public partial class G2C_AttackDistance_notice : IActorMessage {}

// 选择单人
	[Message(HotfixBattleOpcode.C2G_SelectSingleTargetRequest)]
	public partial class C2G_SelectSingleTargetRequest : IActorRequest {}

// 选择单人 返回
	[Message(HotfixBattleOpcode.G2C_SelectSingleTargetResponse)]
	public partial class G2C_SelectSingleTargetResponse : IActorResponse {}

// 击杀公告
	[Message(HotfixBattleOpcode.G2C_KillResult_notice)]
	public partial class G2C_KillResult_notice : IActorMessage {}

//获取玩家二级属性
	[Message(HotfixBattleOpcode.C2G_GetPlayerSecondaryAttribute)]
	public partial class C2G_GetPlayerSecondaryAttribute : IActorRequest {}

	[Message(HotfixBattleOpcode.G2C_GetPlayerSecondaryAttribute)]
	public partial class G2C_GetPlayerSecondaryAttribute : IActorResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixBattleOpcode
	{
		 public const ushort C2G_MovePosRequest = 20001;
		 public const ushort G2C_MovePosResponse = 20002;
		 public const ushort G2C_MovePos_notice = 20003;
		 public const ushort C2G_AttackRequest = 20004;
		 public const ushort G2C_AttackResponse = 20005;
		 public const ushort G2C_AttackStart_notice = 20006;
		 public const ushort G2C_AttackResult_notice = 20007;
		 public const ushort G2C_AttackBuffer_notice = 20008;
		 public const ushort G2C_AttackBufferEnd_notice = 20009;
		 public const ushort G2C_BattleKVData = 20010;
		 public const ushort G2C_ChangeValue_notice = 20011;
		 public const ushort C2G_PlayerPropertyRequest = 20012;
		 public const ushort G2C_PlayerPropertyResponse = 20013;
		 public const ushort C2G_BattlePropertyAddPointRequest = 20014;
		 public const ushort G2C_BattlePropertyAddPointResponse = 20015;
		 public const ushort G2C_ItemDropData = 20016;
		 public const ushort G2C_ItemDrop_notice = 20017;
		 public const ushort C2G_BattlePickUpDropItemRequest = 20018;
		 public const ushort G2C_BattlePickUpDropItemResponse = 20019;
		 public const ushort G2C_BattlePickUpDropItem_notice = 20020;
		 public const ushort C2G_MapDeliveryRequest = 20021;
		 public const ushort G2C_MapDeliveryResponse = 20022;
		 public const ushort C2G_FriendMapDeliveryRequest = 20023;
		 public const ushort G2C_FriendMapDeliveryResponse = 20024;
		 public const ushort G2C_UnitLeaveView_notice = 20025;
		 public const ushort G2C_CareerChangeComplete_notice = 21001;
		 public const ushort C2G_SetPlayerKillingRequest = 21002;
		 public const ushort G2C_SetPlayerKillingResponse = 21003;
		 public const ushort C2G_XinShouBuffRequest = 21004;
		 public const ushort G2C_XinShouBuffResponse = 21005;
		 public const ushort C2G_LoadSceneCompleteRequest = 21006;
		 public const ushort G2C_LoadSceneCompleteResponse = 21007;
		 public const ushort G2C_AttackDistance_notice = 21008;
		 public const ushort C2G_SelectSingleTargetRequest = 21009;
		 public const ushort G2C_SelectSingleTargetResponse = 21010;
		 public const ushort G2C_KillResult_notice = 21011;
		 public const ushort C2G_GetPlayerSecondaryAttribute = 21012;
		 public const ushort G2C_GetPlayerSecondaryAttribute = 21013;
	}
}
