using ETModel;
namespace ETHotfix
{
// Opcode = 35000
//打开宠物界面
	[Message(HotfixPetsOpcode.C2G_OpenPetsInterfaceRequest)]
	public partial class C2G_OpenPetsInterfaceRequest : IActorRequest {}

//打开宠物界面 返回
	[Message(HotfixPetsOpcode.G2C_OpenPetsInterfaceResponse)]
	public partial class G2C_OpenPetsInterfaceResponse : IActorResponse {}

//获取宠物信息
	[Message(HotfixPetsOpcode.C2G_GetPetsInfoRequest)]
	public partial class C2G_GetPetsInfoRequest : IActorRequest {}

//获取宠物信息 返回
	[Message(HotfixPetsOpcode.G2C_GetPetsInfoResponse)]
	public partial class G2C_GetPetsInfoResponse : IActorResponse {}

//打开经验道具使用界面
	[Message(HotfixPetsOpcode.C2G_OpenItemInterfaceRequest)]
	public partial class C2G_OpenItemInterfaceRequest : IActorRequest {}

//打开经验道具使用界面 返回
	[Message(HotfixPetsOpcode.G2C_OpenItemInterfaceResponse)]
	public partial class G2C_OpenItemInterfaceResponse : IActorResponse {}

//使用道具增加经验
	[Message(HotfixPetsOpcode.C2G_UseItemAddExpRequest)]
	public partial class C2G_UseItemAddExpRequest : IActorRequest {}

//使用道具增加经验 返回
	[Message(HotfixPetsOpcode.G2C_UseItemAddExpResponse)]
	public partial class G2C_UseItemAddExpResponse : IActorResponse {}

//增加属性点
	[Message(HotfixPetsOpcode.C2G_AddAttributePointRequest)]
	public partial class C2G_AddAttributePointRequest : IActorRequest {}

//增加属性点 返回
	[Message(HotfixPetsOpcode.G2C_AddAttributePointResponse)]
	public partial class G2C_AddAttributePointResponse : IActorResponse {}

//放生
	[Message(HotfixPetsOpcode.C2G_PetsReleaseRequest)]
	public partial class C2G_PetsReleaseRequest : IActorRequest {}

//放生 返回
	[Message(HotfixPetsOpcode.G2C_PetsReleaseResponse)]
	public partial class G2C_PetsReleaseResponse : IActorResponse {}

//出战
	[Message(HotfixPetsOpcode.C2G_PetsGoToWarRequest)]
	public partial class C2G_PetsGoToWarRequest : IActorRequest {}

//出战 返回
	[Message(HotfixPetsOpcode.G2C_PetsGoToWarResponse)]
	public partial class G2C_PetsGoToWarResponse : IActorResponse {}

//休息
	[Message(HotfixPetsOpcode.C2G_PetsRestRequest)]
	public partial class C2G_PetsRestRequest : IActorRequest {}

//休息 返回
	[Message(HotfixPetsOpcode.G2C_PetsRestResponse)]
	public partial class G2C_PetsRestResponse : IActorResponse {}

//技能
	[Message(HotfixPetsOpcode.C2G_OpenPetsSkillRequest)]
	public partial class C2G_OpenPetsSkillRequest : IActorRequest {}

// 技能 返回
	[Message(HotfixPetsOpcode.G2C_OpenPetsSkillResponse)]
	public partial class G2C_OpenPetsSkillResponse : IActorResponse {}

//技能学习
	[Message(HotfixPetsOpcode.C2G_PetsLearnSkillRequest)]
	public partial class C2G_PetsLearnSkillRequest : IActorRequest {}

//技能学习 返回
	[Message(HotfixPetsOpcode.G2C_PetsLearnSkillResponse)]
	public partial class G2C_PetsLearnSkillResponse : IActorResponse {}

//技能使用
	[Message(HotfixPetsOpcode.C2G_UsePetsSkillRequest)]
	public partial class C2G_UsePetsSkillRequest : IActorRequest {}

//技能使用 返回
	[Message(HotfixPetsOpcode.G2C_UsePetsSkillResponse)]
	public partial class G2C_UsePetsSkillResponse : IActorResponse {}

//技能取消
	[Message(HotfixPetsOpcode.C2G_PetsSkillCancelUseRequest)]
	public partial class C2G_PetsSkillCancelUseRequest : IActorRequest {}

//技能取消 返回
	[Message(HotfixPetsOpcode.G2C_PetsSkillCancelUseResponse)]
	public partial class G2C_PetsSkillCancelUseResponse : IActorResponse {}

//获取新宠物
	[Message(HotfixPetsOpcode.C2G_InsertPetsRequest)]
	public partial class C2G_InsertPetsRequest : IActorRequest {}

// 获取新宠物  返回
	[Message(HotfixPetsOpcode.G2C_InsertPetsResponse)]
	public partial class G2C_InsertPetsResponse : IActorResponse {}

// 增送宠物  通知
	[Message(HotfixPetsOpcode.G2C_InsertPetsMessage)]
	public partial class G2C_InsertPetsMessage : IActorMessage {}

// 属性变化
	[Message(HotfixPetsOpcode.C2G_AttributeChangeRequest)]
	public partial class C2G_AttributeChangeRequest : IActorRequest {}

	[Message(HotfixPetsOpcode.G2C_AttributeChangeResponse)]
	public partial class G2C_AttributeChangeResponse : IActorResponse {}

// 属性变化  通知
	[Message(HotfixPetsOpcode.G2C_AttributeChangeMessage)]
	public partial class G2C_AttributeChangeMessage : IActorMessage {}

//卓越信息
	[Message(HotfixPetsOpcode.C2G_GetPetsExcellentInfoRequest)]
	public partial class C2G_GetPetsExcellentInfoRequest : IActorRequest {}

	[Message(HotfixPetsOpcode.G2C_GetPetsExcellentInfoResponse)]
	public partial class G2C_GetPetsExcellentInfoResponse : IActorResponse {}

//宠物复活
	[Message(HotfixPetsOpcode.C2G_PetsResurrectionRequest)]
	public partial class C2G_PetsResurrectionRequest : IActorRequest {}

	[Message(HotfixPetsOpcode.G2C_PetsResurrectionResponse)]
	public partial class G2C_PetsResurrectionResponse : IActorResponse {}

//宠物洗点
	[Message(HotfixPetsOpcode.C2G_PetsWashASpotRequest)]
	public partial class C2G_PetsWashASpotRequest : IActorRequest {}

	[Message(HotfixPetsOpcode.C2G_PetsWashASpotResponse)]
	public partial class C2G_PetsWashASpotResponse : IActorResponse {}

	[Message(HotfixPetsOpcode.C2G_PetcLevelUpdataMessage)]
	public partial class C2G_PetcLevelUpdataMessage : IActorMessage {}

	[Message(HotfixPetsOpcode.C2G_PetsPackBackRequest)]
	public partial class C2G_PetsPackBackRequest : IActorRequest {}

	[Message(HotfixPetsOpcode.G2C_PetsPackBackResponse)]
	public partial class G2C_PetsPackBackResponse : IActorResponse {}

	[Message(HotfixPetsOpcode.C2G_OpenPetsEnhanceRequest)]
	public partial class C2G_OpenPetsEnhanceRequest : IActorRequest {}

	[Message(HotfixPetsOpcode.G2C_OpenPetsEnhanceResponse)]
	public partial class G2C_OpenPetsEnhanceResponse : IActorResponse {}

	[Message(HotfixPetsOpcode.C2G_PetsEnhanceRequest)]
	public partial class C2G_PetsEnhanceRequest : IActorRequest {}

	[Message(HotfixPetsOpcode.G2C_PetsEnhanceResponse)]
	public partial class G2C_PetsEnhanceResponse : IActorResponse {}

	[Message(HotfixPetsOpcode.PetsInfo)]
	public partial class PetsInfo {}

	[Message(HotfixPetsOpcode.PetsItem)]
	public partial class PetsItem {}

	[Message(HotfixPetsOpcode.PetsRankInfo)]
	public partial class PetsRankInfo {}

//****************宠物改版****************
//打开新宠物界面
	[Message(HotfixPetsOpcode.C2G_OpenNewPetsUI)]
	public partial class C2G_OpenNewPetsUI : IActorRequest {}

	[Message(HotfixPetsOpcode.G2C_OpenNewPetsUI)]
	public partial class G2C_OpenNewPetsUI : IActorResponse {}

	[Message(HotfixPetsOpcode.NewPetsInfo)]
	public partial class NewPetsInfo {}

//****************************************
//进阶
	[Message(HotfixPetsOpcode.C2G_PetProgressionRequest)]
	public partial class C2G_PetProgressionRequest : IActorRequest {}

	[Message(HotfixPetsOpcode.G2C_PetProgressionResponse)]
	public partial class G2C_PetProgressionResponse : IActorResponse {}

//精炼
	[Message(HotfixPetsOpcode.C2G_PetRefiningRequest)]
	public partial class C2G_PetRefiningRequest : IActorRequest {}

	[Message(HotfixPetsOpcode.G2C_PetRefiningResponse)]
	public partial class G2C_PetRefiningResponse : IActorResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixPetsOpcode
	{
		 public const ushort C2G_OpenPetsInterfaceRequest = 35001;
		 public const ushort G2C_OpenPetsInterfaceResponse = 35002;
		 public const ushort C2G_GetPetsInfoRequest = 35003;
		 public const ushort G2C_GetPetsInfoResponse = 35004;
		 public const ushort C2G_OpenItemInterfaceRequest = 35005;
		 public const ushort G2C_OpenItemInterfaceResponse = 35006;
		 public const ushort C2G_UseItemAddExpRequest = 35007;
		 public const ushort G2C_UseItemAddExpResponse = 35008;
		 public const ushort C2G_AddAttributePointRequest = 35009;
		 public const ushort G2C_AddAttributePointResponse = 35010;
		 public const ushort C2G_PetsReleaseRequest = 35011;
		 public const ushort G2C_PetsReleaseResponse = 35012;
		 public const ushort C2G_PetsGoToWarRequest = 35013;
		 public const ushort G2C_PetsGoToWarResponse = 35014;
		 public const ushort C2G_PetsRestRequest = 35015;
		 public const ushort G2C_PetsRestResponse = 35016;
		 public const ushort C2G_OpenPetsSkillRequest = 35017;
		 public const ushort G2C_OpenPetsSkillResponse = 35018;
		 public const ushort C2G_PetsLearnSkillRequest = 35019;
		 public const ushort G2C_PetsLearnSkillResponse = 35020;
		 public const ushort C2G_UsePetsSkillRequest = 35021;
		 public const ushort G2C_UsePetsSkillResponse = 35022;
		 public const ushort C2G_PetsSkillCancelUseRequest = 35023;
		 public const ushort G2C_PetsSkillCancelUseResponse = 35024;
		 public const ushort C2G_InsertPetsRequest = 35025;
		 public const ushort G2C_InsertPetsResponse = 35026;
		 public const ushort G2C_InsertPetsMessage = 35027;
		 public const ushort C2G_AttributeChangeRequest = 35028;
		 public const ushort G2C_AttributeChangeResponse = 35029;
		 public const ushort G2C_AttributeChangeMessage = 35030;
		 public const ushort C2G_GetPetsExcellentInfoRequest = 35031;
		 public const ushort G2C_GetPetsExcellentInfoResponse = 35032;
		 public const ushort C2G_PetsResurrectionRequest = 35033;
		 public const ushort G2C_PetsResurrectionResponse = 35034;
		 public const ushort C2G_PetsWashASpotRequest = 35035;
		 public const ushort C2G_PetsWashASpotResponse = 35036;
		 public const ushort C2G_PetcLevelUpdataMessage = 35037;
		 public const ushort C2G_PetsPackBackRequest = 35038;
		 public const ushort G2C_PetsPackBackResponse = 35039;
		 public const ushort C2G_OpenPetsEnhanceRequest = 35040;
		 public const ushort G2C_OpenPetsEnhanceResponse = 35041;
		 public const ushort C2G_PetsEnhanceRequest = 35042;
		 public const ushort G2C_PetsEnhanceResponse = 35043;
		 public const ushort PetsInfo = 35044;
		 public const ushort PetsItem = 35045;
		 public const ushort PetsRankInfo = 35046;
		 public const ushort C2G_OpenNewPetsUI = 35047;
		 public const ushort G2C_OpenNewPetsUI = 35048;
		 public const ushort NewPetsInfo = 35049;
		 public const ushort C2G_PetProgressionRequest = 35050;
		 public const ushort G2C_PetProgressionResponse = 35051;
		 public const ushort C2G_PetRefiningRequest = 35052;
		 public const ushort G2C_PetRefiningResponse = 35053;
	}
}
