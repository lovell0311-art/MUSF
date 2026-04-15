using ETModel;
using CustomFrameWork.Component;
namespace ETHotfix
{
// Opcode = 31000
//申请好友列表
	[Message(FriendSystemOpcode.C2G_OpenFriendsinterfaceRequest)]
	public partial class C2G_OpenFriendsinterfaceRequest : IActorRequest {}

//申请好友列表	返回
	[Message(FriendSystemOpcode.G2C_OpenFriendsinterfaceResponse)]
	public partial class G2C_OpenFriendsinterfaceResponse : IActorResponse {}

//搜索好友
	[Message(FriendSystemOpcode.C2G_SearchForFriendsRequest)]
	public partial class C2G_SearchForFriendsRequest : IActorRequest {}

//搜索好友	返回
	[Message(FriendSystemOpcode.G2C_SearchForFriendsResponse)]
	public partial class G2C_SearchForFriendsResponse : IActorResponse {}

//是否在线 0:在线	1:离线
//添加好友
	[Message(FriendSystemOpcode.C2G_AddFriendsRequest)]
	public partial class C2G_AddFriendsRequest : IActorRequest {}

//添加好友 返回
	[Message(FriendSystemOpcode.G2C_AddFriendsResponse)]
	public partial class G2C_AddFriendsResponse : IActorResponse {}

//添加好友通知对方
	[Message(FriendSystemOpcode.G2C_AddFriendsnotice_notice)]
	public partial class G2C_AddFriendsnotice_notice : IActorMessage {}

//同意或者拒绝 添加好友
	[Message(FriendSystemOpcode.C2G_AgreeToAddFriendRequest)]
	public partial class C2G_AgreeToAddFriendRequest : IActorRequest {}

//Type=0 拒绝	Type=1 同意
//同意或者拒绝 添加好友返回
	[Message(FriendSystemOpcode.G2C_AgreeToAddFriendResponse)]
	public partial class G2C_AgreeToAddFriendResponse : IActorResponse {}

//同意或者拒绝 通知好友
	[Message(FriendSystemOpcode.G2C_AgreeToAddFriend_notice)]
	public partial class G2C_AgreeToAddFriend_notice : IActorMessage {}

	[Message(FriendSystemOpcode.Struct_Friends)]
	public partial class Struct_Friends {}

//ID
//类型(0:默认 1:拉黑 2:仇人 3:申请 4:好友)
//角色名
//是否在线 0:在线	1:离线
//拉黑或者被击杀时间
//职业类型
//战盟职务
//专职次数
//删除或者拉黑好友
	[Message(FriendSystemOpcode.C2G_DeleteOrBlockFriendRequest)]
	public partial class C2G_DeleteOrBlockFriendRequest : IActorRequest {}

//ID
//好友列表类型
//操作类型 0：删除好友  1；拉黑好友
//删除或者拉黑好友	返回
	[Message(FriendSystemOpcode.G2C_DeleteOrBlockFriendResponse)]
	public partial class G2C_DeleteOrBlockFriendResponse : IActorResponse {}

//好友聊天
	[Message(FriendSystemOpcode.C2G_FriendChatRequest)]
	public partial class C2G_FriendChatRequest : IActorRequest {}

//ID
//信息内容长度MAX50
//好友聊天 返回
	[Message(FriendSystemOpcode.G2C_FriendChatResponse)]
	public partial class G2C_FriendChatResponse : IActorResponse {}

//好友聊天 通知
	[Message(FriendSystemOpcode.G2C_FriendChat_notice)]
	public partial class G2C_FriendChat_notice : IActorMessage {}

//信息内容长度MAX50
//角色ID
//发送时间
//好友传送
	[Message(FriendSystemOpcode.C2G_FriendDeliveryRequest)]
	public partial class C2G_FriendDeliveryRequest : IActorRequest {}

//ID
//好友传送 返回
	[Message(FriendSystemOpcode.G2C_FriendDeliveryResponse)]
	public partial class G2C_FriendDeliveryResponse : IActorResponse {}

//好友推荐
	[Message(FriendSystemOpcode.C2G_FriendRecommendRequest)]
	public partial class C2G_FriendRecommendRequest : IActorRequest {}

//好友推荐 返回
	[Message(FriendSystemOpcode.G2C_FriendRecommendResponse)]
	public partial class G2C_FriendRecommendResponse : IActorResponse {}

//返回长度十个
//获取位置信息
	[Message(FriendSystemOpcode.C2G_FriendPositionRequest)]
	public partial class C2G_FriendPositionRequest : IActorRequest {}

//获取位置信息 返回
	[Message(FriendSystemOpcode.G2C_FriendPositionResponse)]
	public partial class G2C_FriendPositionResponse : IActorResponse {}

//获取位置信息 通知
	[Message(FriendSystemOpcode.G2C_FriendPositionMessage)]
	public partial class G2C_FriendPositionMessage : IActorMessage {}

//坐标
//地图ID
//发送者ID
}
namespace ETHotfix
{
	public static partial class FriendSystemOpcode
	{
		 public const ushort C2G_OpenFriendsinterfaceRequest = 31001;
		 public const ushort G2C_OpenFriendsinterfaceResponse = 31002;
		 public const ushort C2G_SearchForFriendsRequest = 31003;
		 public const ushort G2C_SearchForFriendsResponse = 31004;
		 public const ushort C2G_AddFriendsRequest = 31005;
		 public const ushort G2C_AddFriendsResponse = 31006;
		 public const ushort G2C_AddFriendsnotice_notice = 31007;
		 public const ushort C2G_AgreeToAddFriendRequest = 31008;
		 public const ushort G2C_AgreeToAddFriendResponse = 31009;
		 public const ushort G2C_AgreeToAddFriend_notice = 31010;
		 public const ushort Struct_Friends = 31011;
		 public const ushort C2G_DeleteOrBlockFriendRequest = 31012;
		 public const ushort G2C_DeleteOrBlockFriendResponse = 31013;
		 public const ushort C2G_FriendChatRequest = 31014;
		 public const ushort G2C_FriendChatResponse = 31015;
		 public const ushort G2C_FriendChat_notice = 31016;
		 public const ushort C2G_FriendDeliveryRequest = 31017;
		 public const ushort G2C_FriendDeliveryResponse = 31018;
		 public const ushort C2G_FriendRecommendRequest = 31019;
		 public const ushort G2C_FriendRecommendResponse = 31020;
		 public const ushort C2G_FriendPositionRequest = 31021;
		 public const ushort G2C_FriendPositionResponse = 31022;
		 public const ushort G2C_FriendPositionMessage = 31023;
	}
}
