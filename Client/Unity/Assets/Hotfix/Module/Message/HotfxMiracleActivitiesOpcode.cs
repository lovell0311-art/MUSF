using ETModel;
namespace ETHotfix
{
// Opcode = 39000
//获取相应活动的数据
	[Message(HotfxMiracleActivitiesOpcode.C2G_OpenMiracleActivitiesRequest)]
	public partial class C2G_OpenMiracleActivitiesRequest : IActorRequest {}

//活动ID
//获取相应活动的数据 返回
	[Message(HotfxMiracleActivitiesOpcode.G2C_OpenMiracleActivitiesResponse)]
	public partial class G2C_OpenMiracleActivitiesResponse : IActorResponse {}

//冲级活动领奖
	[Message(HotfxMiracleActivitiesOpcode.C2G_RushGradeActivityReceiveRequest)]
	public partial class C2G_RushGradeActivityReceiveRequest : IActorRequest {}

//活动ID
//奖励ID
//冲级活动领奖 返回
	[Message(HotfxMiracleActivitiesOpcode.G2C_RushGradeActivityReceiveResponse)]
	public partial class G2C_RushGradeActivityReceiveResponse : IActorResponse {}

//攻城战通知
	[Message(HotfxMiracleActivitiesOpcode.G2C_SendPointOutMessage)]
	public partial class G2C_SendPointOutMessage : IActorMessage {}

//座上城主位
	[Message(HotfxMiracleActivitiesOpcode.C2G_TakeAThroneRequest)]
	public partial class C2G_TakeAThroneRequest : IActorRequest {}

//座上城主位 返回
	[Message(HotfxMiracleActivitiesOpcode.G2C_TakeAThroneResponse)]
	public partial class G2C_TakeAThroneResponse : IActorResponse {}

//离开座位
	[Message(HotfxMiracleActivitiesOpcode.C2G_LeaveYourSeatRequest)]
	public partial class C2G_LeaveYourSeatRequest : IActorRequest {}

//离开座位 返回
	[Message(HotfxMiracleActivitiesOpcode.G2C_LeaveYourSeatResponse)]
	public partial class G2C_LeaveYourSeatResponse : IActorResponse {}

//怪物数量
	[Message(HotfxMiracleActivitiesOpcode.C2G_NewYearMobCntRequest)]
	public partial class C2G_NewYearMobCntRequest : IActorRequest {}

//怪物数量 返回
	[Message(HotfxMiracleActivitiesOpcode.G2C_NewYearMobCntResponse)]
	public partial class G2C_NewYearMobCntResponse : IActorResponse {}

	[Message(HotfxMiracleActivitiesOpcode.struct_MiracleActivities)]
	public partial class struct_MiracleActivities {}

	[Message(HotfxMiracleActivitiesOpcode.Struct_MobCnt)]
	public partial class Struct_MobCnt {}

//签到
	[Message(HotfxMiracleActivitiesOpcode.C2G_NationalDaySignin)]
	public partial class C2G_NationalDaySignin : IActorRequest {}

	[Message(HotfxMiracleActivitiesOpcode.G2C_NationalDaySignin)]
	public partial class G2C_NationalDaySignin : IActorResponse {}

//打开推广界面
	[Message(HotfxMiracleActivitiesOpcode.C2G_OpenInvitationInterface)]
	public partial class C2G_OpenInvitationInterface : IActorRequest {}

	[Message(HotfxMiracleActivitiesOpcode.G2C_OpenInvitationInterface)]
	public partial class G2C_OpenInvitationInterface : IActorResponse {}

	[Message(HotfxMiracleActivitiesOpcode.C2G_PromotionAward)]
	public partial class C2G_PromotionAward : IActorRequest {}

	[Message(HotfxMiracleActivitiesOpcode.G2C_PromotionAward)]
	public partial class G2C_PromotionAward : IActorResponse {}

	[Message(HotfxMiracleActivitiesOpcode.InvitationInfo)]
	public partial class InvitationInfo {}

//五一抽奖
	[Message(HotfxMiracleActivitiesOpcode.C2G_MayDayLuckyDraw)]
	public partial class C2G_MayDayLuckyDraw : IActorRequest {}

	[Message(HotfxMiracleActivitiesOpcode.G2C_MayDayLuckyDraw)]
	public partial class G2C_MayDayLuckyDraw : IActorResponse {}

}
namespace ETHotfix
{
	public static partial class HotfxMiracleActivitiesOpcode
	{
		 public const ushort C2G_OpenMiracleActivitiesRequest = 39001;
		 public const ushort G2C_OpenMiracleActivitiesResponse = 39002;
		 public const ushort C2G_RushGradeActivityReceiveRequest = 39003;
		 public const ushort G2C_RushGradeActivityReceiveResponse = 39004;
		 public const ushort G2C_SendPointOutMessage = 39005;
		 public const ushort C2G_TakeAThroneRequest = 39006;
		 public const ushort G2C_TakeAThroneResponse = 39007;
		 public const ushort C2G_LeaveYourSeatRequest = 39008;
		 public const ushort G2C_LeaveYourSeatResponse = 39009;
		 public const ushort C2G_NewYearMobCntRequest = 39010;
		 public const ushort G2C_NewYearMobCntResponse = 39011;
		 public const ushort struct_MiracleActivities = 39012;
		 public const ushort Struct_MobCnt = 39013;
		 public const ushort C2G_NationalDaySignin = 39014;
		 public const ushort G2C_NationalDaySignin = 39015;
		 public const ushort C2G_OpenInvitationInterface = 39016;
		 public const ushort G2C_OpenInvitationInterface = 39017;
		 public const ushort C2G_PromotionAward = 39018;
		 public const ushort G2C_PromotionAward = 39019;
		 public const ushort InvitationInfo = 39020;
		 public const ushort C2G_MayDayLuckyDraw = 39021;
		 public const ushort G2C_MayDayLuckyDraw = 39022;
	}
}
