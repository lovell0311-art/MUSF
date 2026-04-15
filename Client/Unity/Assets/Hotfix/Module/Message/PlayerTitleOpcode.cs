using ETModel;
namespace ETHotfix
{
// Opcode = 42000
//称号通知
	[Message(PlayerTitleOpcode.G2C_ServerSendTitleMessage)]
	public partial class G2C_ServerSendTitleMessage : IActorMessage {}

//称号修改
	[Message(PlayerTitleOpcode.C2G_SetPlayerTitleRequest)]
	public partial class C2G_SetPlayerTitleRequest : IActorRequest {}

	[Message(PlayerTitleOpcode.G2C_SetPlayerTitleResponse)]
	public partial class G2C_SetPlayerTitleResponse : IActorResponse {}

//称号删除
	[Message(PlayerTitleOpcode.C2G_DelPlayerTitleRequest)]
	public partial class C2G_DelPlayerTitleRequest : IActorRequest {}

	[Message(PlayerTitleOpcode.G2C_DelPlayerTitleResponse)]
	public partial class G2C_DelPlayerTitleResponse : IActorResponse {}

//获取排行
	[Message(PlayerTitleOpcode.C2G_GetRankInfoRequest)]
	public partial class C2G_GetRankInfoRequest : IActorRequest {}

	[Message(PlayerTitleOpcode.G2C_GetRankInfoResponse)]
	public partial class G2C_GetRankInfoResponse : IActorResponse {}

//客户端不需要
	[Message(PlayerTitleOpcode.G2M_GetRankInfoRequest)]
	public partial class G2M_GetRankInfoRequest : IActorRequest {}

	[Message(PlayerTitleOpcode.M2G_GetRankInfoResponse)]
	public partial class M2G_GetRankInfoResponse : IActorResponse {}

//触发排行
	[Message(PlayerTitleOpcode.G2M_EnterRankingRequest)]
	public partial class G2M_EnterRankingRequest : IActorRequest {}

	[Message(PlayerTitleOpcode.M2G_EnterRankingResponse)]
	public partial class M2G_EnterRankingResponse : IActorResponse {}

//排行榜结算通知
	[Message(PlayerTitleOpcode.M2G_SettlementRankingMessage)]
	public partial class M2G_SettlementRankingMessage : IActorMessage {}

	[Message(PlayerTitleOpcode.G2C_SendFullServiceMessage)]
	public partial class G2C_SendFullServiceMessage : IActorMessage {}

	[Message(PlayerTitleOpcode.Title_Status)]
	public partial class Title_Status {}

	[Message(PlayerTitleOpcode.Rank_status)]
	public partial class Rank_status {}

}
namespace ETHotfix
{
	public static partial class PlayerTitleOpcode
	{
		 public const ushort G2C_ServerSendTitleMessage = 42001;
		 public const ushort C2G_SetPlayerTitleRequest = 42002;
		 public const ushort G2C_SetPlayerTitleResponse = 42003;
		 public const ushort C2G_DelPlayerTitleRequest = 42004;
		 public const ushort G2C_DelPlayerTitleResponse = 42005;
		 public const ushort C2G_GetRankInfoRequest = 42006;
		 public const ushort G2C_GetRankInfoResponse = 42007;
		 public const ushort G2M_GetRankInfoRequest = 42008;
		 public const ushort M2G_GetRankInfoResponse = 42009;
		 public const ushort G2M_EnterRankingRequest = 42010;
		 public const ushort M2G_EnterRankingResponse = 42011;
		 public const ushort M2G_SettlementRankingMessage = 42012;
		 public const ushort G2C_SendFullServiceMessage = 42013;
		 public const ushort Title_Status = 42014;
		 public const ushort Rank_status = 42015;
	}
}
