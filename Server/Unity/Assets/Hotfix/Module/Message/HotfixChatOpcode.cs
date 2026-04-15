using ETModel;
using CustomFrameWork.Component;
namespace ETHotfix
{
// Opcode = 22000
// 请求 进入聊天房间
	[Message(HotfixChatOpcode.C2G_EnterChatRoom)]
	public partial class C2G_EnterChatRoom : IActorRequest {}

// 返回 进入聊天房间
	[Message(HotfixChatOpcode.G2C_EnterChatRoom)]
	public partial class G2C_EnterChatRoom : IActorResponse {}

// 请求 离开聊天房间
	[Message(HotfixChatOpcode.C2G_LeaveChatRoom)]
	public partial class C2G_LeaveChatRoom : IActorRequest {}

// 返回 离开聊天房间
	[Message(HotfixChatOpcode.G2C_LeaveChatRoom)]
	public partial class G2C_LeaveChatRoom : IActorResponse {}

// 请求 发送消息到聊天房间
	[Message(HotfixChatOpcode.C2G_SendChatMessageToChatRoom)]
	public partial class C2G_SendChatMessageToChatRoom : IActorRequest {}

// 返回 发送消息到聊天房间
	[Message(HotfixChatOpcode.G2C_SendChatMessageToChatRoom)]
	public partial class G2C_SendChatMessageToChatRoom : IActorResponse {}

// 请求 私聊聊天消息
	[Message(HotfixChatOpcode.C2G_SendChatMessageToPlayer)]
	public partial class C2G_SendChatMessageToPlayer : IActorRequest {}

// 返回 私聊聊天消息
	[Message(HotfixChatOpcode.G2C_SendChatMessageToPlayer)]
	public partial class G2C_SendChatMessageToPlayer : IActorResponse {}

// 请求 发送消息到附近
	[Message(HotfixChatOpcode.C2G_SendChatMessageToNearby)]
	public partial class C2G_SendChatMessageToNearby : IActorRequest {}

// 返回 发送消息到附近
	[Message(HotfixChatOpcode.G2C_SendChatMessageToNearby)]
	public partial class G2C_SendChatMessageToNearby : IActorResponse {}

//通知 消息到达聊天房间
	[Message(HotfixChatOpcode.G2C_MessageInChatRoom_notice)]
	public partial class G2C_MessageInChatRoom_notice : IActorMessage {}

//通知 收到一条私聊消息
	[Message(HotfixChatOpcode.ReceivePlayerChatMessage_notice)]
	public partial class ReceivePlayerChatMessage_notice : IActorMessage {}

//通知 来自附近的聊天消息
	[Message(HotfixChatOpcode.ChatMessageFromNearby_notice)]
	public partial class ChatMessageFromNearby_notice : IActorMessage {}

	[Message(HotfixChatOpcode.C2G_FullServiceHornRequest)]
	public partial class C2G_FullServiceHornRequest : IActorRequest {}

	[Message(HotfixChatOpcode.G2C_FullServiceHornResponse)]
	public partial class G2C_FullServiceHornResponse : IActorResponse {}

	[Message(HotfixChatOpcode.G2C_FullServiceHornnotice)]
	public partial class G2C_FullServiceHornnotice : IActorMessage {}

	[Message(HotfixChatOpcode.C2G_BagShareRequest)]
	public partial class C2G_BagShareRequest : IActorRequest {}

	[Message(HotfixChatOpcode.G2C_BagShareResponse)]
	public partial class G2C_BagShareResponse : IActorResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixChatOpcode
	{
		 public const ushort C2G_EnterChatRoom = 22001;
		 public const ushort G2C_EnterChatRoom = 22002;
		 public const ushort C2G_LeaveChatRoom = 22003;
		 public const ushort G2C_LeaveChatRoom = 22004;
		 public const ushort C2G_SendChatMessageToChatRoom = 22005;
		 public const ushort G2C_SendChatMessageToChatRoom = 22006;
		 public const ushort C2G_SendChatMessageToPlayer = 22007;
		 public const ushort G2C_SendChatMessageToPlayer = 22008;
		 public const ushort C2G_SendChatMessageToNearby = 22009;
		 public const ushort G2C_SendChatMessageToNearby = 22010;
		 public const ushort G2C_MessageInChatRoom_notice = 22011;
		 public const ushort ReceivePlayerChatMessage_notice = 22012;
		 public const ushort ChatMessageFromNearby_notice = 22013;
		 public const ushort C2G_FullServiceHornRequest = 22014;
		 public const ushort G2C_FullServiceHornResponse = 22015;
		 public const ushort G2C_FullServiceHornnotice = 22016;
		 public const ushort C2G_BagShareRequest = 22017;
		 public const ushort G2C_BagShareResponse = 22018;
	}
}
