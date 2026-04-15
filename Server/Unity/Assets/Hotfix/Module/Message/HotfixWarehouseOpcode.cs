using ETModel;
using CustomFrameWork.Component;
namespace ETHotfix
{
// Opcode = 28000
//=========================仓库============================
// 移动仓库物品的位置 推送G2C_MoveWarehouseItem_notice
	[Message(HotfixWarehouseOpcode.C2G_MoveWarehouseItem)]
	public partial class C2G_MoveWarehouseItem : IActorRequest {}

// 移动仓库物品的位置  返回
	[Message(HotfixWarehouseOpcode.G2C_MoveWarehouseItem)]
	public partial class G2C_MoveWarehouseItem : IActorResponse {}

// 从背包添加到仓库  推送G2C_AddWarehouseItem_notice
	[Message(HotfixWarehouseOpcode.C2G_AddWarehouseItem)]
	public partial class C2G_AddWarehouseItem : IActorRequest {}

// 从背包添加到仓库  返回
	[Message(HotfixWarehouseOpcode.G2C_AddWarehouseItem)]
	public partial class G2C_AddWarehouseItem : IActorResponse {}

// 仓库物品移动到背包  推送G2C_ItemsLeaveBackpack_notice
	[Message(HotfixWarehouseOpcode.C2G_DelWarehouseItem)]
	public partial class C2G_DelWarehouseItem : IActorRequest {}

// 仓库物品移动到背包  返回
	[Message(HotfixWarehouseOpcode.G2C_DelWarehouseItem)]
	public partial class G2C_DelWarehouseItem : IActorResponse {}

// 放进金币到仓库  推送G2C_BackpackGoldChange_notice和G2C_WarehouseGoldChange_notice
	[Message(HotfixWarehouseOpcode.C2G_AddGoldToWarehouse)]
	public partial class C2G_AddGoldToWarehouse : IActorRequest {}

// 放进金币到仓库  返回
	[Message(HotfixWarehouseOpcode.G2C_AddGoldToWarehouse)]
	public partial class G2C_AddGoldToWarehouse : IActorResponse {}

// 从仓库中取出金币  推送G2C_BackpackGoldChange_notice和G2C_WarehouseGoldChange_notice
	[Message(HotfixWarehouseOpcode.C2G_TackOutGoldInWarehouse)]
	public partial class C2G_TackOutGoldInWarehouse : IActorRequest {}

// 从仓库中取出金币  返回
	[Message(HotfixWarehouseOpcode.G2C_TackOutGoldInWarehouse)]
	public partial class G2C_TackOutGoldInWarehouse : IActorResponse {}

//============================推送==============================
//推送客户端仓库中有物品进入 选择角色进入游戏场景时会推送玩家仓库里现有物品
	[Message(HotfixWarehouseOpcode.G2C_AddWarehouseItem_notice)]
	public partial class G2C_AddWarehouseItem_notice : IActorMessage {}

//推送客户端仓库中有物品离开
	[Message(HotfixWarehouseOpcode.G2C_DelWarehouseItem_notice)]
	public partial class G2C_DelWarehouseItem_notice : IActorMessage {}

//推送客户端仓库中有物品位置变动
	[Message(HotfixWarehouseOpcode.G2C_MoveWarehouseItem_notice)]
	public partial class G2C_MoveWarehouseItem_notice : IActorMessage {}

//推送仓库扩建
	[Message(HotfixWarehouseOpcode.G2C_WarehouseExtension_notice)]
	public partial class G2C_WarehouseExtension_notice : IActorMessage {}

//广播玩家背包金币数值变动  选择角色进入游戏场景时或者金币变动时会推送
	[Message(HotfixWarehouseOpcode.G2C_WarehouseGoldChange_notice)]
	public partial class G2C_WarehouseGoldChange_notice : IActorMessage {}

}
namespace ETHotfix
{
	public static partial class HotfixWarehouseOpcode
	{
		 public const ushort C2G_MoveWarehouseItem = 28001;
		 public const ushort G2C_MoveWarehouseItem = 28002;
		 public const ushort C2G_AddWarehouseItem = 28003;
		 public const ushort G2C_AddWarehouseItem = 28004;
		 public const ushort C2G_DelWarehouseItem = 28005;
		 public const ushort G2C_DelWarehouseItem = 28006;
		 public const ushort C2G_AddGoldToWarehouse = 28007;
		 public const ushort G2C_AddGoldToWarehouse = 28008;
		 public const ushort C2G_TackOutGoldInWarehouse = 28009;
		 public const ushort G2C_TackOutGoldInWarehouse = 28010;
		 public const ushort G2C_AddWarehouseItem_notice = 28011;
		 public const ushort G2C_DelWarehouseItem_notice = 28012;
		 public const ushort G2C_MoveWarehouseItem_notice = 28013;
		 public const ushort G2C_WarehouseExtension_notice = 28014;
		 public const ushort G2C_WarehouseGoldChange_notice = 28015;
	}
}
