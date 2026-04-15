using ETModel;
namespace ETHotfix
{
// Opcode = 37000
//新邮件通知
	[Message(HotfxMailOpcode.G2C_ServerSendMailMessage)]
	public partial class G2C_ServerSendMailMessage : IActorMessage {}

//Mailinfo Mail =1;
//点击确认以读取
	[Message(HotfxMailOpcode.C2G_ReadMailRequest)]
	public partial class C2G_ReadMailRequest : IActorRequest {}

	[Message(HotfxMailOpcode.G2C_ReadMailResponse)]
	public partial class G2C_ReadMailResponse : IActorResponse {}

//领取道具
	[Message(HotfxMailOpcode.C2G_MailReceiveItemRequest)]
	public partial class C2G_MailReceiveItemRequest : IActorRequest {}

	[Message(HotfxMailOpcode.G2C_MailReceiveItemResponse)]
	public partial class G2C_MailReceiveItemResponse : IActorResponse {}

//删除邮件
	[Message(HotfxMailOpcode.C2G_MailDeleteItemRequest)]
	public partial class C2G_MailDeleteItemRequest : IActorRequest {}

	[Message(HotfxMailOpcode.G2C_MailDeleteItemResponse)]
	public partial class G2C_MailDeleteItemResponse : IActorResponse {}

//打开邮件
	[Message(HotfxMailOpcode.C2G_OpenMailRequest)]
	public partial class C2G_OpenMailRequest : IActorRequest {}

	[Message(HotfxMailOpcode.G2C_OpenMailResponse)]
	public partial class G2C_OpenMailResponse : IActorResponse {}

	[Message(HotfxMailOpcode.G2G_SendFullServiceMailMessage)]
	public partial class G2G_SendFullServiceMailMessage : IActorMessage {}

	[Message(HotfxMailOpcode.G2C_LoadMaillUnreadMessage)]
	public partial class G2C_LoadMaillUnreadMessage : IActorMessage {}

	[Message(HotfxMailOpcode.Mailinfo)]
	public partial class Mailinfo {}

	[Message(HotfxMailOpcode.Iteminfo)]
	public partial class Iteminfo {}

}
namespace ETHotfix
{
	public static partial class HotfxMailOpcode
	{
		 public const ushort G2C_ServerSendMailMessage = 37001;
		 public const ushort C2G_ReadMailRequest = 37002;
		 public const ushort G2C_ReadMailResponse = 37003;
		 public const ushort C2G_MailReceiveItemRequest = 37004;
		 public const ushort G2C_MailReceiveItemResponse = 37005;
		 public const ushort C2G_MailDeleteItemRequest = 37006;
		 public const ushort G2C_MailDeleteItemResponse = 37007;
		 public const ushort C2G_OpenMailRequest = 37008;
		 public const ushort G2C_OpenMailResponse = 37009;
		 public const ushort G2G_SendFullServiceMailMessage = 37010;
		 public const ushort G2C_LoadMaillUnreadMessage = 37011;
		 public const ushort Mailinfo = 37012;
		 public const ushort Iteminfo = 37013;
	}
}
