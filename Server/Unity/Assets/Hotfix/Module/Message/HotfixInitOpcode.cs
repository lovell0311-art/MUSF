using ETModel;
using CustomFrameWork.Component;
namespace ETHotfix
{
// Opcode = 30000
	[Message(HotfixInitOpcode.G2C_TestHotfixMessage)]
	public partial class G2C_TestHotfixMessage : IMessage {}

//=========================注册账号 登录账号============================
// 注册账号 登录账号
	[Message(HotfixInitOpcode.C2R_RegisterOrLoginRequest)]
	public partial class C2R_RegisterOrLoginRequest : IRequest {}

// 注册账号 登录账号 返回
	[Message(HotfixInitOpcode.R2C_RegisterOrLoginResponse)]
	public partial class R2C_RegisterOrLoginResponse : IResponse {}

// 封禁到什么时候 时间戳 毫秒
// 封禁原因
//=========================登录网关============================
// 登录网关
	[Message(HotfixInitOpcode.C2G_LoginGateRequest)]
	public partial class C2G_LoginGateRequest : IRequest {}

// 登录网关 返回
	[Message(HotfixInitOpcode.G2C_LoginGateResponse)]
	public partial class G2C_LoginGateResponse : IResponse {}

//=========================申请验证码============================
// 申请验证码
	[Message(HotfixInitOpcode.C2R_LoginSystemSMSApplyCodeRequest)]
	public partial class C2R_LoginSystemSMSApplyCodeRequest : IRequest {}

// 申请验证码 返回
	[Message(HotfixInitOpcode.R2C_LoginSystemSMSApplyCodeResponse)]
	public partial class R2C_LoginSystemSMSApplyCodeResponse : IResponse {}

//=========================请求游戏区服信息============================
// 请求游戏区服信息
	[Message(HotfixInitOpcode.C2G_LoginSystemGetServerInfoRequest)]
	public partial class C2G_LoginSystemGetServerInfoRequest : IRequest {}

// 请求游戏区服信息  返回
	[Message(HotfixInitOpcode.G2C_LoginSystemGetServerInfoResponse)]
	public partial class G2C_LoginSystemGetServerInfoResponse : IResponse {}

// 游戏区服信息 结构体
	[Message(HotfixInitOpcode.G2C_LoginSystemServerInfoMessage)]
	public partial class G2C_LoginSystemServerInfoMessage : IMessage {}

//=========================请求游戏区服线路信息============================
// 请求游戏区服线路信息
	[Message(HotfixInitOpcode.C2G_LoginSystemGetServerLineInfoRequest)]
	public partial class C2G_LoginSystemGetServerLineInfoRequest : IRequest {}

// 请求游戏区服线路信息  返回
	[Message(HotfixInitOpcode.G2C_LoginSystemGetServerLineInfoResponse)]
	public partial class G2C_LoginSystemGetServerLineInfoResponse : IResponse {}

	[Message(HotfixInitOpcode.G2R_GetServerLineInfoRequest)]
	public partial class G2R_GetServerLineInfoRequest : IActorRequest {}

	[Message(HotfixInitOpcode.R2G_GetServerLineInfoResponse)]
	public partial class R2G_GetServerLineInfoResponse : IActorResponse {}

// 游戏区服信息 结构体
	[Message(HotfixInitOpcode.G2C_LoginSystemServerLineInfoMessage)]
	public partial class G2C_LoginSystemServerLineInfoMessage : IMessage {}

	[Message(HotfixInitOpcode.GM2R_UpdateArwaInfo)]
	public partial class GM2R_UpdateArwaInfo : IActorMessage {}

	[Message(HotfixInitOpcode.G2All_UpdateCommandInfo)]
	public partial class G2All_UpdateCommandInfo : IActorMessage {}

//=========================请求进入游戏区服============================
// 请求进入游戏区服
	[Message(HotfixInitOpcode.C2G_LoginSystemEnterGameAreaMessage)]
	public partial class C2G_LoginSystemEnterGameAreaMessage : IRequest {}

// 请求进入游戏区服  返回
	[Message(HotfixInitOpcode.G2C_LoginSystemEnterGameAreaMessage)]
	public partial class G2C_LoginSystemEnterGameAreaMessage : IResponse {}

//=========================获取角色信息============================
// 获取角色信息
	[Message(HotfixInitOpcode.C2G_LoginSystemGetGamePlayerInfoRequest)]
	public partial class C2G_LoginSystemGetGamePlayerInfoRequest : IActorRequest {}

// 获取角色信息  返回
	[Message(HotfixInitOpcode.G2C_LoginSystemGetGamePlayerInfoResponse)]
	public partial class G2C_LoginSystemGetGamePlayerInfoResponse : IActorResponse {}

// 角色信息 结构体
	[Message(HotfixInitOpcode.G2C_LoginSystemGetGamePlayerInfoMessage)]
	public partial class G2C_LoginSystemGetGamePlayerInfoMessage : IMessage {}

//---------卡槽中的物品状态-------------
	[Message(HotfixInitOpcode.G2C_LoginSystemEquipItemMessage)]
	public partial class G2C_LoginSystemEquipItemMessage : IMessage {}

//=========================新建角色 删除 选择角色============================
// 新建角色
	[Message(HotfixInitOpcode.C2G_LoginSystemCreateGamePlayerRequest)]
	public partial class C2G_LoginSystemCreateGamePlayerRequest : IActorRequest {}

// 新建角色  返回
	[Message(HotfixInitOpcode.G2C_LoginSystemCreateGamePlayerResponse)]
	public partial class G2C_LoginSystemCreateGamePlayerResponse : IActorResponse {}

// 删除角色
	[Message(HotfixInitOpcode.C2G_LoginSystemDeleteGamePlayerRequest)]
	public partial class C2G_LoginSystemDeleteGamePlayerRequest : IActorRequest {}

// 删除角色  返回
	[Message(HotfixInitOpcode.G2C_LoginSystemDeleteGamePlayerResponse)]
	public partial class G2C_LoginSystemDeleteGamePlayerResponse : IActorResponse {}

// 选择角色开始游戏
	[Message(HotfixInitOpcode.C2G_StartGameGamePlayerRequest)]
	public partial class C2G_StartGameGamePlayerRequest : IActorRequest {}

// 选择角色开始游戏  返回
	[Message(HotfixInitOpcode.G2C_StartGameGamePlayerResponse)]
	public partial class G2C_StartGameGamePlayerResponse : IActorResponse {}

//=========================准备就绪============================
// 准备就绪
	[Message(HotfixInitOpcode.C2G_ReadyRequest)]
	public partial class C2G_ReadyRequest : IActorRequest {}

// 准备就绪  返回
	[Message(HotfixInitOpcode.G2C_ReadyResponse)]
	public partial class G2C_ReadyResponse : IActorResponse {}

//=========================准备就绪后 属性通知============================
	[Message(HotfixInitOpcode.G2C_InitProperty_notice)]
	public partial class G2C_InitProperty_notice : IActorMessage {}

// GM
	[Message(HotfixInitOpcode.C2G_GMRequest)]
	public partial class C2G_GMRequest : IActorRequest {}

// GM  返回
	[Message(HotfixInitOpcode.G2C_GMResponse)]
	public partial class G2C_GMResponse : IActorResponse {}

// 从其他设备登录账号
	[Message(HotfixInitOpcode.Gate2C_LoginFromOtherDevices)]
	public partial class Gate2C_LoginFromOtherDevices : IActorMessage {}

// 下线角色 恢复到刚 LoginGate 状态
	[Message(HotfixInitOpcode.C2Gate_KickRole)]
	public partial class C2Gate_KickRole : IActorRequest {}

// 下线角色 返回
	[Message(HotfixInitOpcode.Gate2C_KickRole)]
	public partial class Gate2C_KickRole : IActorResponse {}

// 断开连接，服务器将不会等待客户端重连
// 玩家关闭游戏时请求
	[Message(HotfixInitOpcode.C2Gate_Disconnect)]
	public partial class C2Gate_Disconnect : IActorMessage {}

// 重新进入游戏，服务器会将session切换成断线前的
// 服务器会将断线期间的数据包重新发送
	[Message(HotfixInitOpcode.C2Gate_Reconnect)]
	public partial class C2Gate_Reconnect : IActorRequest {}

	[Message(HotfixInitOpcode.Gate2C_Reconnect)]
	public partial class Gate2C_Reconnect : IActorResponse {}

// 用于重连 gate 的 key
// 客户端收到后，需要保存。（用于下次重连）
	[Message(HotfixInitOpcode.Gate2C_GateSessionKeyChange)]
	public partial class Gate2C_GateSessionKeyChange : IActorMessage {}

	[Message(HotfixInitOpcode.C2R_ResetPasswordByPhoneNumberRequest)]
	public partial class C2R_ResetPasswordByPhoneNumberRequest : IRequest {}

	[Message(HotfixInitOpcode.R2C_ResetPasswordByPhoneNumberResponse)]
	public partial class R2C_ResetPasswordByPhoneNumberResponse : IResponse {}

// 服务器关闭时间
	[Message(HotfixInitOpcode.Game2C_ServerShutdownTime)]
	public partial class Game2C_ServerShutdownTime : IActorMessage {}

// 服务器关闭
	[Message(HotfixInitOpcode.Gate2C_ServerShutdown)]
	public partial class Gate2C_ServerShutdown : IActorMessage {}

//SDK登录验证
	[Message(HotfixInitOpcode.C2R_XYSDKvalidatelogonRequest)]
	public partial class C2R_XYSDKvalidatelogonRequest : IRequest {}

	[Message(HotfixInitOpcode.R2C_XYSDKvalidatelogonResponse)]
	public partial class R2C_XYSDKvalidatelogonResponse : IResponse {}

// 封禁到什么时候 时间戳 毫秒
// 封禁原因
// 被服务器踢下线
	[Message(HotfixInitOpcode.Gate2C_KickOffline)]
	public partial class Gate2C_KickOffline : IActorMessage {}

// 下线原因
// 0.顶号
// 1.服务器关闭
// 2.gm 踢下线
// 3.封号
// 封禁到什么时候 时间戳 毫秒
// 被踢下线的原因 （2、3 才有数据）
	[Message(HotfixInitOpcode.C2G_Ping)]
	public partial class C2G_Ping : IMessage {}

//腾讯实名认证
	[Message(HotfixInitOpcode.C2G_NeedIdCardInspectMessage)]
	public partial class C2G_NeedIdCardInspectMessage : IMessage {}

//身份验证
	[Message(HotfixInitOpcode.C2G_IdCardInspectRequest)]
	public partial class C2G_IdCardInspectRequest : IRequest {}

//身份验证 返回
	[Message(HotfixInitOpcode.G2C_IdCardInspectResponse)]
	public partial class G2C_IdCardInspectResponse : IResponse {}

//中宣部实名认证
	[Message(HotfixInitOpcode.C2G_RealNameAuthenticationRequest)]
	public partial class C2G_RealNameAuthenticationRequest : IRequest {}

	[Message(HotfixInitOpcode.G2C_RealNameAuthenticationResponse)]
	public partial class G2C_RealNameAuthenticationResponse : IResponse {}

//抖音SDK登录验证
	[Message(HotfixInitOpcode.C2R_DouYinSDKLogonRequest)]
	public partial class C2R_DouYinSDKLogonRequest : IRequest {}

//抖音SDK登录验证
	[Message(HotfixInitOpcode.R2C_DouYinSDKLogonResponse)]
	public partial class R2C_DouYinSDKLogonResponse : IResponse {}

// 封禁到什么时候 时间戳 毫秒
// 封禁原因
	[Message(HotfixInitOpcode.C2R_ShouQSDKLogonRequest)]
	public partial class C2R_ShouQSDKLogonRequest : IRequest {}

	[Message(HotfixInitOpcode.R2C_ShouQSDKLogonResponse)]
	public partial class R2C_ShouQSDKLogonResponse : IResponse {}

// 封禁到什么时候 时间戳 毫秒
// 封禁原因
	[Message(HotfixInitOpcode.C2R_GetLastLoginToTheRegion)]
	public partial class C2R_GetLastLoginToTheRegion : IRequest {}

//第三方返回的Uid,游戏方是账号
//第三方类型1:恺英2:抖音3:手Q--其他所有类型都用游戏方ID
	[Message(HotfixInitOpcode.R2C_GetLastLoginToTheRegion)]
	public partial class R2C_GetLastLoginToTheRegion : IResponse {}

	[Message(HotfixInitOpcode.C2R_HaXiSDKLogonRequest)]
	public partial class C2R_HaXiSDKLogonRequest : IRequest {}

	[Message(HotfixInitOpcode.R2C_HaXiSDKLogonResponse)]
	public partial class R2C_HaXiSDKLogonResponse : IResponse {}

// 封禁到什么时候 时间戳 毫秒
// 封禁原因
}
namespace ETHotfix
{
	public static partial class HotfixInitOpcode
	{
		 public const ushort G2C_TestHotfixMessage = 30001;
		 public const ushort C2R_RegisterOrLoginRequest = 30002;
		 public const ushort R2C_RegisterOrLoginResponse = 30003;
		 public const ushort C2G_LoginGateRequest = 30004;
		 public const ushort G2C_LoginGateResponse = 30005;
		 public const ushort C2R_LoginSystemSMSApplyCodeRequest = 30006;
		 public const ushort R2C_LoginSystemSMSApplyCodeResponse = 30007;
		 public const ushort C2G_LoginSystemGetServerInfoRequest = 30008;
		 public const ushort G2C_LoginSystemGetServerInfoResponse = 30009;
		 public const ushort G2C_LoginSystemServerInfoMessage = 30010;
		 public const ushort C2G_LoginSystemGetServerLineInfoRequest = 30011;
		 public const ushort G2C_LoginSystemGetServerLineInfoResponse = 30012;
		 public const ushort G2R_GetServerLineInfoRequest = 30013;
		 public const ushort R2G_GetServerLineInfoResponse = 30014;
		 public const ushort G2C_LoginSystemServerLineInfoMessage = 30015;
		 public const ushort GM2R_UpdateArwaInfo = 30016;
		 public const ushort G2All_UpdateCommandInfo = 30017;
		 public const ushort C2G_LoginSystemEnterGameAreaMessage = 30018;
		 public const ushort G2C_LoginSystemEnterGameAreaMessage = 30019;
		 public const ushort C2G_LoginSystemGetGamePlayerInfoRequest = 30020;
		 public const ushort G2C_LoginSystemGetGamePlayerInfoResponse = 30021;
		 public const ushort G2C_LoginSystemGetGamePlayerInfoMessage = 30022;
		 public const ushort G2C_LoginSystemEquipItemMessage = 30023;
		 public const ushort C2G_LoginSystemCreateGamePlayerRequest = 30024;
		 public const ushort G2C_LoginSystemCreateGamePlayerResponse = 30025;
		 public const ushort C2G_LoginSystemDeleteGamePlayerRequest = 30026;
		 public const ushort G2C_LoginSystemDeleteGamePlayerResponse = 30027;
		 public const ushort C2G_StartGameGamePlayerRequest = 30028;
		 public const ushort G2C_StartGameGamePlayerResponse = 30029;
		 public const ushort C2G_ReadyRequest = 30030;
		 public const ushort G2C_ReadyResponse = 30031;
		 public const ushort G2C_InitProperty_notice = 30032;
		 public const ushort C2G_GMRequest = 30033;
		 public const ushort G2C_GMResponse = 30034;
		 public const ushort Gate2C_LoginFromOtherDevices = 30035;
		 public const ushort C2Gate_KickRole = 30036;
		 public const ushort Gate2C_KickRole = 30037;
		 public const ushort C2Gate_Disconnect = 30038;
		 public const ushort C2Gate_Reconnect = 30039;
		 public const ushort Gate2C_Reconnect = 30040;
		 public const ushort Gate2C_GateSessionKeyChange = 30041;
		 public const ushort C2R_ResetPasswordByPhoneNumberRequest = 30042;
		 public const ushort R2C_ResetPasswordByPhoneNumberResponse = 30043;
		 public const ushort Game2C_ServerShutdownTime = 30044;
		 public const ushort Gate2C_ServerShutdown = 30045;
		 public const ushort C2R_XYSDKvalidatelogonRequest = 30046;
		 public const ushort R2C_XYSDKvalidatelogonResponse = 30047;
		 public const ushort Gate2C_KickOffline = 30048;
		 public const ushort C2G_Ping = 30049;
		 public const ushort C2G_NeedIdCardInspectMessage = 30050;
		 public const ushort C2G_IdCardInspectRequest = 30051;
		 public const ushort G2C_IdCardInspectResponse = 30052;
		 public const ushort C2G_RealNameAuthenticationRequest = 30053;
		 public const ushort G2C_RealNameAuthenticationResponse = 30054;
		 public const ushort C2R_DouYinSDKLogonRequest = 30055;
		 public const ushort R2C_DouYinSDKLogonResponse = 30056;
		 public const ushort C2R_ShouQSDKLogonRequest = 30057;
		 public const ushort R2C_ShouQSDKLogonResponse = 30058;
		 public const ushort C2R_GetLastLoginToTheRegion = 30059;
		 public const ushort R2C_GetLastLoginToTheRegion = 30060;
		 public const ushort C2R_HaXiSDKLogonRequest = 30061;
		 public const ushort R2C_HaXiSDKLogonResponse = 30062;
	}
}
