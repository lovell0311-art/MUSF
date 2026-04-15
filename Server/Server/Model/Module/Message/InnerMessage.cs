using ETModel;
using System.Collections.Generic;
using CustomFrameWork.Component;
using MongoDB.Bson;
namespace ETModel
{
	[Message(InnerOpcode.DBSaveRequest)]
	public partial class DBSaveRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public ComponentWithId Component { get; set; }

	}

	[Message(InnerOpcode.DBSaveBatchResponse)]
	public partial class DBSaveBatchResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.DBSaveBatchRequest)]
	public partial class DBSaveBatchRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public List<ComponentWithId> Components = new List<ComponentWithId>();

	}

	[Message(InnerOpcode.DBSaveResponse)]
	public partial class DBSaveResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.DBQueryRequest)]
	public partial class DBQueryRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Id { get; set; }

		public string CollectionName { get; set; }

	}

	[Message(InnerOpcode.DBQueryResponse)]
	public partial class DBQueryResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public ComponentWithId Component { get; set; }

	}

	[Message(InnerOpcode.DBQueryBatchRequest)]
	public partial class DBQueryBatchRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public List<long> IdList = new List<long>();

	}

	[Message(InnerOpcode.DBQueryBatchResponse)]
	public partial class DBQueryBatchResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public List<ComponentWithId> Components = new List<ComponentWithId>();

	}

	[Message(InnerOpcode.DBQueryJsonRequest)]
	public partial class DBQueryJsonRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public string Json { get; set; }

		public int Count { get; set; }

	}

	[Message(InnerOpcode.DBQueryJsonResponse)]
	public partial class DBQueryJsonResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public List<ComponentWithId> Components = new List<ComponentWithId>();

	}

	[Message(InnerOpcode.DBUpdateOneRequest)]
	public partial class DBUpdateOneRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public BsonDocument Filter { get; set; }

		public BsonDocument Updates { get; set; }

	}

	[Message(InnerOpcode.DBUpdateOneResponse)]
	public partial class DBUpdateOneResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.DBAggregateRequest)]
	public partial class DBAggregateRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public List<BsonDocument> PipeLine = new List<BsonDocument>();

	}

	[Message(InnerOpcode.DBAggregateResponse)]
	public partial class DBAggregateResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public List<BsonDocument> Result = new List<BsonDocument>();

	}

	[Message(InnerOpcode.ObjectAddRequest)]
	public partial class ObjectAddRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Key { get; set; }

		public long InstanceId { get; set; }

	}

	[Message(InnerOpcode.ObjectAddResponse)]
	public partial class ObjectAddResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.ObjectRemoveRequest)]
	public partial class ObjectRemoveRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Key { get; set; }

	}

	[Message(InnerOpcode.ObjectRemoveResponse)]
	public partial class ObjectRemoveResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.ObjectLockRequest)]
	public partial class ObjectLockRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Key { get; set; }

		public long InstanceId { get; set; }

		public int Time { get; set; }

	}

	[Message(InnerOpcode.ObjectLockResponse)]
	public partial class ObjectLockResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.ObjectUnLockRequest)]
	public partial class ObjectUnLockRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Key { get; set; }

		public long OldInstanceId { get; set; }

		public long InstanceId { get; set; }

	}

	[Message(InnerOpcode.ObjectUnLockResponse)]
	public partial class ObjectUnLockResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.ObjectGetRequest)]
	public partial class ObjectGetRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Key { get; set; }

	}

	[Message(InnerOpcode.ObjectGetResponse)]
	public partial class ObjectGetResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public long InstanceId { get; set; }

	}

	[Message(InnerOpcode.R2G_GetLoginKey)]
	public partial class R2G_GetLoginKey: IRequest
	{
		public int RpcId { get; set; }

		public string Account { get; set; }

	}

	[Message(InnerOpcode.G2R_GetLoginKey)]
	public partial class G2R_GetLoginKey: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public long Key { get; set; }

	}

	[Message(InnerOpcode.G2M_CreateUnit)]
	public partial class G2M_CreateUnit: IRequest
	{
		public int RpcId { get; set; }

		public long PlayerId { get; set; }

		public long GateSessionId { get; set; }

	}

	[Message(InnerOpcode.M2G_CreateUnit)]
	public partial class M2G_CreateUnit: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

// 自己的unit id
// 自己的unit id
		public long UnitId { get; set; }

// 所有的unit
// 所有的unit
		public List<UnitInfo> Units = new List<UnitInfo>();

	}

	[Message(InnerOpcode.G2M_SessionDisconnect)]
	public partial class G2M_SessionDisconnect: IActorMessage
	{
		public int RpcId { get; set; }

		public long ActorId { get; set; }

	}

	[Message(InnerOpcode.DBGetCountRequest)]
	public partial class DBGetCountRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public string Json { get; set; }

	}

	[Message(InnerOpcode.DBGetCountResponse)]
	public partial class DBGetCountResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public long RowCount { get; set; }

	}

	[Message(InnerOpcode.DBIncRequest)]
	public partial class DBIncRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public string Json { get; set; }

	}

	[Message(InnerOpcode.DBIncResponse)]
	public partial class DBIncResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public ComponentWithId Component { get; set; }

	}

//网关服通知匹配服 请求游戏区服信息
	[Message(InnerOpcode.G2M_GetGameAreaInfoMessage)]
	public partial class G2M_GetGameAreaInfoMessage: IRequest
	{
		public int RpcId { get; set; }

		public int GetGameAreaPage { get; set; }

		public long UserId { get; set; }

	}

//网关服通知匹配服 请求游戏区服信息  返回
	[Message(InnerOpcode.M2G_GetGameAreaInfoMessage)]
	public partial class M2G_GetGameAreaInfoMessage: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public List<G2C_GameAreaInfoMessage2> GameAreaInfos = new List<G2C_GameAreaInfoMessage2>();

	}

// 游戏区服信息
	[Message(InnerOpcode.G2C_GameAreaInfoMessage2)]
	public partial class G2C_GameAreaInfoMessage2: IMessage
	{
		public int GameAreaId { get; set; }

		public string GameAreaNickName { get; set; }

		public int GameAreaType { get; set; }

		public int IsGameAreaState { get; set; }

	}

//网关服通知匹配服 请求游戏区服线路信息
	[Message(InnerOpcode.G2M_GetGameAreaLineInfoMessage)]
	public partial class G2M_GetGameAreaLineInfoMessage: IRequest
	{
		public int RpcId { get; set; }

		public int GetGameAreaPage { get; set; }

		public long UserId { get; set; }

		public int AreaId { get; set; }

	}

//网关服通知匹配服 请求游戏区服线路信息  返回
	[Message(InnerOpcode.M2G_GetGameAreaLineInfoMessage)]
	public partial class M2G_GetGameAreaLineInfoMessage: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public List<G2C_GameAreaInfoMessage2> GameAreaInfos = new List<G2C_GameAreaInfoMessage2>();

	}

//网关服通知匹配服 请求匹配游戏区服线路信息
	[Message(InnerOpcode.G2M_EnterGameGetAreaLineInfoMessage)]
	public partial class G2M_EnterGameGetAreaLineInfoMessage: IRequest
	{
		public int RpcId { get; set; }

		public long UserId { get; set; }

		public int AreaId { get; set; }

		public int AreaLineId { get; set; }

	}

//网关服通知匹配服 请求匹配游戏区服线路信息  返回
	[Message(InnerOpcode.M2G_EnterGameGetAreaLineInfoMessage)]
	public partial class M2G_EnterGameGetAreaLineInfoMessage: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public int GameServerId { get; set; }

	}

//网关服通知匹配服 进区区服
	[Message(InnerOpcode.G2Game_EnterGameAreaMessage)]
	public partial class G2Game_EnterGameAreaMessage: IRequest
	{
		public int RpcId { get; set; }

		public long UserId { get; set; }

		public int AreaId { get; set; }

		public int AreaLineId { get; set; }

		public int GateServerID { get; set; }

		public string ChannelId { get; set; }

		public string ConnectIp { get; set; }

	}

//网关服通知匹配服 进区区服  返回
	[Message(InnerOpcode.Game2G_EnterGameAreaMessage)]
	public partial class Game2G_EnterGameAreaMessage: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public long ResActorId { get; set; }

		public List<long> GameUserIds = new List<long>();

		public List<long> GameOccupation = new List<long>();

	}

// 断开GateUser
	[Message(InnerOpcode.S2Gate_DisconnectGateUser)]
	public partial class S2Gate_DisconnectGateUser: IRequest
	{
		public int RpcId { get; set; }

		public long UserId { get; set; }

		public int DisconnectType { get; set; }

		public long BanTillTime { get; set; }

		public string Reason { get; set; }

	}

	[Message(InnerOpcode.Gate2S_DisconnectGateUser)]
	public partial class Gate2S_DisconnectGateUser: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

// 下线玩家
	[Message(InnerOpcode.S2Game_RequestExitGame)]
	public partial class S2Game_RequestExitGame: IRequest
	{
		public int RpcId { get; set; }

		public long UserId { get; set; }

	}

	[Message(InnerOpcode.Game2S_RequestExitGame)]
	public partial class Game2S_RequestExitGame: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

// ================== 登录中心服 ==================
// 登录账号 游戏登录服
	[Message(InnerOpcode.Realm2LoginCenter_LoginAccount)]
	public partial class Realm2LoginCenter_LoginAccount: IRequest
	{
		public int RpcId { get; set; }

		public long UserId { get; set; }

	}

	[Message(InnerOpcode.LoginCenter2Realm_LoginAccount)]
	public partial class LoginCenter2Realm_LoginAccount: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public int GateServerId { get; set; }

	}

// 添加记录 并 登录
	[Message(InnerOpcode.Gate2LoginCenter_AddRecordAndLogin)]
	public partial class Gate2LoginCenter_AddRecordAndLogin: IRequest
	{
		public int RpcId { get; set; }

		public long UserId { get; set; }

		public int GateServerId { get; set; }

		public string ConnectIp { get; set; }

	}

	[Message(InnerOpcode.LoginCenter2Gate_AddRecordAndLogin)]
	public partial class LoginCenter2Gate_AddRecordAndLogin: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public int GateServerId { get; set; }

	}

// 设置登录信息
	[Message(InnerOpcode.Gate2LoginCenter_SetLoginRecord)]
	public partial class Gate2LoginCenter_SetLoginRecord: IRequest
	{
		public int RpcId { get; set; }

		public long UserId { get; set; }

		public int GateServerId { get; set; }

		public long GameUserId { get; set; }

		public int GameServerId { get; set; }

	}

	[Message(InnerOpcode.LoginCenter2Gate_SetLoginRecord)]
	public partial class LoginCenter2Gate_SetLoginRecord: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

// 移除登录信息
	[Message(InnerOpcode.LoginCenter2Gate_RemoveLoginRecord)]
	public partial class LoginCenter2Gate_RemoveLoginRecord: IRequest
	{
		public int RpcId { get; set; }

		public long UserId { get; set; }

		public int GateServerId { get; set; }

	}

	[Message(InnerOpcode.Gate2LoginCenter_RemoveLoginRecord)]
	public partial class Gate2LoginCenter_RemoveLoginRecord: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

// 获取登录记录
	[Message(InnerOpcode.S2LoginCenter_GetLoginRecord)]
	public partial class S2LoginCenter_GetLoginRecord: IRequest
	{
		public int RpcId { get; set; }

		public long UserId { get; set; }

	}

	[Message(InnerOpcode.LoginCenter2S_GetLoginRecord)]
	public partial class LoginCenter2S_GetLoginRecord: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public long UserId { get; set; }

		public int GateServerId { get; set; }

		public long GameUserId { get; set; }

		public int GameServerId { get; set; }

	}

// ==========================================
// GM
// ==========================================
// 发送邮件
	[Message(InnerOpcode.GM2Game_SendMail)]
	public partial class GM2Game_SendMail: IRequest
	{
		public int RpcId { get; set; }

		public int ZoneId { get; set; }

		public long UserId { get; set; }

		public long GameUserId { get; set; }

		public string Name { get; set; }

		public string Content { get; set; }

		public string MailItemsBson { get; set; }

	}

	[Message(InnerOpcode.Game2GM_SendMail)]
	public partial class Game2GM_SendMail: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

// 获取游戏服状态
	[Message(InnerOpcode.GM2Game_GetGameServerStatus)]
	public partial class GM2Game_GetGameServerStatus: IRequest
	{
		public int RpcId { get; set; }

	}

	[Message(InnerOpcode.Game2GM_GetGameServerStatus)]
	public partial class Game2GM_GetGameServerStatus: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public int OnlineCount { get; set; }

		public int EnterGameCount { get; set; }

	}

// 运行代码
	[Message(InnerOpcode.GM2S_RunCode)]
	public partial class GM2S_RunCode: IRequest
	{
		public int RpcId { get; set; }

		public string Code { get; set; }

	}

	[Message(InnerOpcode.S2GM_RunCode)]
	public partial class S2GM_RunCode: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public string Return { get; set; }

	}

// 更新账号Identity
	[Message(InnerOpcode.GM2Game_UpdateAccountIdentity)]
	public partial class GM2Game_UpdateAccountIdentity: IRequest
	{
		public int RpcId { get; set; }

		public long GameUserId { get; set; }

	}

	[Message(InnerOpcode.Game2GM_UpdateAccountIdentity)]
	public partial class Game2GM_UpdateAccountIdentity: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

}
