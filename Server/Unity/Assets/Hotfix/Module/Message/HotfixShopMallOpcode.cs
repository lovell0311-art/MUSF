using ETModel;
using CustomFrameWork.Component;
namespace ETHotfix
{
// Opcode = 38000
//打开商城界面
	[Message(HotfixShopMallOpcode.C2G_OpenShopMallRequest)]
	public partial class C2G_OpenShopMallRequest : IActorRequest {}

//0:一般商城1:元宝商城
//1=buff类2=消耗类3=宠物坐骑类4=特殊类
	[Message(HotfixShopMallOpcode.G2C_OpenShopMallResponse)]
	public partial class G2C_OpenShopMallResponse : IActorResponse {}

//购买
	[Message(HotfixShopMallOpcode.C2G_ShopMallBuyItemRequest)]
	public partial class C2G_ShopMallBuyItemRequest : IActorRequest {}

//0:一般商城1:元宝商城
	[Message(HotfixShopMallOpcode.G2C_ShopMallBuyItemResponse)]
	public partial class G2C_ShopMallBuyItemResponse : IActorResponse {}

//累计充值领奖
	[Message(HotfixShopMallOpcode.C2G_ShopMallReceiveRequest)]
	public partial class C2G_ShopMallReceiveRequest : IActorRequest {}

//1:充值6元2:充值38元3:充值68元4:充值198元5:充值288元6:七天领奖
	[Message(HotfixShopMallOpcode.G2C_ShopMallReceiveResponse)]
	public partial class G2C_ShopMallReceiveResponse : IActorResponse {}

//原地复活
	[Message(HotfixShopMallOpcode.C2G_ResurrectionInSituRequest)]
	public partial class C2G_ResurrectionInSituRequest : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_ResurrectionInSituResponse)]
	public partial class G2C_ResurrectionInSituResponse : IActorResponse {}

//安全区复活
	[Message(HotfixShopMallOpcode.C2G_SafeAreaResurrectionRequest)]
	public partial class C2G_SafeAreaResurrectionRequest : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_SafeAreaResurrectionResponse)]
	public partial class G2C_SafeAreaResurrectionResponse : IActorResponse {}

//获取玩家充值信息
	[Message(HotfixShopMallOpcode.C2G_PlayerShopInfoRequest)]
	public partial class C2G_PlayerShopInfoRequest : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_PlayerShopInfoResponse)]
	public partial class G2C_PlayerShopInfoResponse : IActorResponse {}

	[Message(HotfixShopMallOpcode.G2C_PlayerShopInfoUpdataMessage)]
	public partial class G2C_PlayerShopInfoUpdataMessage : IActorMessage {}

//首充Dictionary<int, int> <档次，金额>
//远程打开
	[Message(HotfixShopMallOpcode.C2G_RemoteOpenRequest)]
	public partial class C2G_RemoteOpenRequest : IActorRequest {}

//0:商店1:仓库
	[Message(HotfixShopMallOpcode.G2C_RemoteOpenResponse)]
	public partial class G2C_RemoteOpenResponse : IActorResponse {}

//0:商店1:仓库
//充值
	[Message(HotfixShopMallOpcode.C2G_RechargeRequest)]
	public partial class C2G_RechargeRequest : IActorRequest {}

//充值Type
	[Message(HotfixShopMallOpcode.G2C_RechargeResponse)]
	public partial class G2C_RechargeResponse : IActorResponse {}

//扩展为额外充值信息
	[Message(HotfixShopMallOpcode.G2C_SevenDaysToRechargeMessage)]
	public partial class G2C_SevenDaysToRechargeMessage : IActorMessage {}

//Dic<string,(int,bool)> 日期key，int值，bool领奖状态
//DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
//string =  dateTime.ToString
//创建订单
	[Message(HotfixShopMallOpcode.C2G_CreateAnOrderRequest)]
	public partial class C2G_CreateAnOrderRequest : IActorRequest {}

//参照 PlayerShopQuotaType
	[Message(HotfixShopMallOpcode.C2G_CreateAnOrderResponse)]
	public partial class C2G_CreateAnOrderResponse : IActorResponse {}

	[Message(HotfixShopMallOpcode.GM2C_PayReturnRequest)]
	public partial class GM2C_PayReturnRequest : IActorRequest {}

	[Message(HotfixShopMallOpcode.C2GM_PayReturnResponse)]
	public partial class C2GM_PayReturnResponse : IActorResponse {}

	[Message(HotfixShopMallOpcode.C2G_QuickPurchaseRequest)]
	public partial class C2G_QuickPurchaseRequest : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_QuickPurchaseResponse)]
	public partial class G2C_QuickPurchaseResponse : IActorResponse {}

	[Message(HotfixShopMallOpcode.ShopProp)]
	public partial class ShopProp {}

	[Message(HotfixShopMallOpcode.PlayerShop)]
	public partial class PlayerShop {}

	[Message(HotfixShopMallOpcode.ItemInBackpack_Status)]
	public partial class ItemInBackpack_Status {}

// 背包所属玩家ID
// 物品唯一标识
// 物品配置表ID
// 物品类型ID
// 物品在背包中的位置
// 物品占用的格子
// 物品数量
// 物品等级
// 物品所有的属性
	[Message(HotfixShopMallOpcode.ItemAllProperty)]
	public partial class ItemAllProperty {}

//--------属性结构-------------
	[Message(HotfixShopMallOpcode.Property)]
	public partial class Property {}

//--------物品属性词条结构---------
	[Message(HotfixShopMallOpcode.AttrEntry)]
	public partial class AttrEntry {}

//创建订单
	[Message(HotfixShopMallOpcode.C2G_DouYinCreateAnOrderRequest)]
	public partial class C2G_DouYinCreateAnOrderRequest : IActorRequest {}

//参照 PlayerShopQuotaType
	[Message(HotfixShopMallOpcode.C2G_DouYinCreateAnOrderResponse)]
	public partial class C2G_DouYinCreateAnOrderResponse : IActorResponse {}

//藏宝阁
//上架物品
	[Message(HotfixShopMallOpcode.C2G_listingTreasureHouse)]
	public partial class C2G_listingTreasureHouse : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_listingTreasureHouse)]
	public partial class G2C_listingTreasureHouse : IActorResponse {}

//打开藏宝阁
	[Message(HotfixShopMallOpcode.C2G_OpenTreasureHouse)]
	public partial class C2G_OpenTreasureHouse : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_OpenTreasureHouse)]
	public partial class G2C_OpenTreasureHouse : IActorResponse {}

//收索道具
	[Message(HotfixShopMallOpcode.C2G_SearchForItems)]
	public partial class C2G_SearchForItems : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_SearchForItems)]
	public partial class G2C_SearchForItems : IActorResponse {}

//查看下一页
	[Message(HotfixShopMallOpcode.C2G_NextPage)]
	public partial class C2G_NextPage : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_NextPage)]
	public partial class G2C_NextPage : IActorResponse {}

//查看道具信息
	[Message(HotfixShopMallOpcode.C2G_GetTreasureHouseItemInfo)]
	public partial class C2G_GetTreasureHouseItemInfo : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_GetTreasureHouseItemInfo)]
	public partial class G2C_GetTreasureHouseItemInfo : IActorResponse {}

//购买道具
	[Message(HotfixShopMallOpcode.C2G_BuyTreasureHouseItemInfo)]
	public partial class C2G_BuyTreasureHouseItemInfo : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_BuyTreasureHouseItemInfo)]
	public partial class G2C_BuyTreasureHouseItemInfo : IActorResponse {}

//打开寄售
	[Message(HotfixShopMallOpcode.C2G_OpenConsign)]
	public partial class C2G_OpenConsign : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_OpenConsign)]
	public partial class G2C_OpenConsign : IActorResponse {}

//下架物品
	[Message(HotfixShopMallOpcode.C2G_RemovedItems)]
	public partial class C2G_RemovedItems : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_RemovedItems)]
	public partial class G2C_RemovedItems : IActorResponse {}

//清除角色缓存
	[Message(HotfixShopMallOpcode.C2G_DeletePlayerTreasureHouse)]
	public partial class C2G_DeletePlayerTreasureHouse : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_DeletePlayerTreasureHouse)]
	public partial class G2C_DeletePlayerTreasureHouse : IActorResponse {}

	[Message(HotfixShopMallOpcode.TreasureHouseItemInfo)]
	public partial class TreasureHouseItemInfo {}

	[Message(HotfixShopMallOpcode.TransactionRecord)]
	public partial class TransactionRecord {}

// =========================
// 直冲礼包协议
// =========================
// Opcode = 38500
// 物品基础信息
	[Message(HotfixShopMallOpcode.ItemBaseInfo)]
	public partial class ItemBaseInfo {}

// 礼包信息
	[Message(HotfixShopMallOpcode.RecGiftBundleInfo)]
	public partial class RecGiftBundleInfo {}

// 获取直冲礼包
	[Message(HotfixShopMallOpcode.C2G_GetDirectRechargeGiftBundle)]
	public partial class C2G_GetDirectRechargeGiftBundle : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_GetDirectRechargeGiftBundle)]
	public partial class G2C_GetDirectRechargeGiftBundle : IActorResponse {}

// 获取礼包信息
	[Message(HotfixShopMallOpcode.C2G_GetGiftBundleItemInfo)]
	public partial class C2G_GetGiftBundleItemInfo : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_GetGiftBundleItemInfo)]
	public partial class G2C_GetGiftBundleItemInfo : IActorResponse {}

// 购买直冲礼包
	[Message(HotfixShopMallOpcode.C2G_TestBuyRechargeGiftBundle)]
	public partial class C2G_TestBuyRechargeGiftBundle : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_TestBuyRechargeGiftBundle)]
	public partial class G2C_TestBuyRechargeGiftBundle : IActorResponse {}

// =========================
// 抽奖协议
// =========================
// Opcode = 38550
// 抽奖奖品信息
	[Message(HotfixShopMallOpcode.LotteryGiftInfo)]
	public partial class LotteryGiftInfo {}

	[Message(HotfixShopMallOpcode.LotteryLog)]
	public partial class LotteryLog {}

// 打开抽奖界面
	[Message(HotfixShopMallOpcode.C2G_OpenLotteryWin)]
	public partial class C2G_OpenLotteryWin : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_OpenLotteryWin)]
	public partial class G2C_OpenLotteryWin : IActorResponse {}

// 开始抽奖
	[Message(HotfixShopMallOpcode.C2G_StartLottery)]
	public partial class C2G_StartLottery : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_StartLottery)]
	public partial class G2C_StartLottery : IActorResponse {}

// ◄─StartLogId        EndLogId─►
//       │                 │
//       ▼                 ▼
//────────────────────────────────►
// 获取历史抽奖日志
	[Message(HotfixShopMallOpcode.C2G_GetHistoryLotteryLog)]
	public partial class C2G_GetHistoryLotteryLog : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_GetHistoryLotteryLog)]
	public partial class G2C_GetHistoryLotteryLog : IActorResponse {}

// 获取抽奖日志
	[Message(HotfixShopMallOpcode.C2G_GetLotteryLog)]
	public partial class C2G_GetLotteryLog : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_GetLotteryLog)]
	public partial class G2C_GetLotteryLog : IActorResponse {}

// =========================
// 累计充值协议
// =========================
// Opcode = 38600
// 打开累计充值界面
	[Message(HotfixShopMallOpcode.C2G_OpenCumulativeRecharge)]
	public partial class C2G_OpenCumulativeRecharge : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_OpenCumulativeRecharge)]
	public partial class G2C_OpenCumulativeRecharge : IActorResponse {}

// 领取累计充值礼包
	[Message(HotfixShopMallOpcode.C2G_ReceiveRechargeGiftPack)]
	public partial class C2G_ReceiveRechargeGiftPack : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_ReceiveRechargeGiftPack)]
	public partial class G2C_ReceiveRechargeGiftPack : IActorResponse {}

	[Message(HotfixShopMallOpcode.C2G_ShouQCreateAnOrder)]
	public partial class C2G_ShouQCreateAnOrder : IActorRequest {}

//参照 PlayerShopQuotaType
	[Message(HotfixShopMallOpcode.G2C_ShouQCreateAnOrder)]
	public partial class G2C_ShouQCreateAnOrder : IActorResponse {}

	[Message(HotfixShopMallOpcode.C2G_MyPayTopUp)]
	public partial class C2G_MyPayTopUp : IActorRequest {}

//参照 PlayerShopQuotaType
	[Message(HotfixShopMallOpcode.G2C_MyPayTopUp)]
	public partial class G2C_MyPayTopUp : IActorResponse {}

	[Message(HotfixShopMallOpcode.C2G_MyV4PayTopUp)]
	public partial class C2G_MyV4PayTopUp : IActorRequest {}

//参照 PlayerShopQuotaType
	[Message(HotfixShopMallOpcode.G2C_MyV4PayTopUp)]
	public partial class G2C_MyV4PayTopUp : IActorResponse {}

	[Message(HotfixShopMallOpcode.C2G_ClearingGiftKit)]
	public partial class C2G_ClearingGiftKit : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_ClearingGiftKit)]
	public partial class G2C_ClearingGiftKit : IActorResponse {}

	[Message(HotfixShopMallOpcode.C2G_MagicBoxCreateOrder)]
	public partial class C2G_MagicBoxCreateOrder : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_MagicBoxCreateOrder)]
	public partial class G2C_MagicBoxCreateOrder : IActorResponse {}

	[Message(HotfixShopMallOpcode.C2G_BuyYourOwnGiftPackRequest)]
	public partial class C2G_BuyYourOwnGiftPackRequest : IActorRequest {}

	[Message(HotfixShopMallOpcode.G2C_BuyYourOwnGiftPackResponse)]
	public partial class G2C_BuyYourOwnGiftPackResponse : IActorResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixShopMallOpcode
	{
		 public const ushort C2G_OpenShopMallRequest = 38001;
		 public const ushort G2C_OpenShopMallResponse = 38002;
		 public const ushort C2G_ShopMallBuyItemRequest = 38003;
		 public const ushort G2C_ShopMallBuyItemResponse = 38004;
		 public const ushort C2G_ShopMallReceiveRequest = 38005;
		 public const ushort G2C_ShopMallReceiveResponse = 38006;
		 public const ushort C2G_ResurrectionInSituRequest = 38007;
		 public const ushort G2C_ResurrectionInSituResponse = 38008;
		 public const ushort C2G_SafeAreaResurrectionRequest = 38009;
		 public const ushort G2C_SafeAreaResurrectionResponse = 38010;
		 public const ushort C2G_PlayerShopInfoRequest = 38011;
		 public const ushort G2C_PlayerShopInfoResponse = 38012;
		 public const ushort G2C_PlayerShopInfoUpdataMessage = 38013;
		 public const ushort C2G_RemoteOpenRequest = 38014;
		 public const ushort G2C_RemoteOpenResponse = 38015;
		 public const ushort C2G_RechargeRequest = 38016;
		 public const ushort G2C_RechargeResponse = 38017;
		 public const ushort G2C_SevenDaysToRechargeMessage = 38018;
		 public const ushort C2G_CreateAnOrderRequest = 38019;
		 public const ushort C2G_CreateAnOrderResponse = 38020;
		 public const ushort GM2C_PayReturnRequest = 38021;
		 public const ushort C2GM_PayReturnResponse = 38022;
		 public const ushort C2G_QuickPurchaseRequest = 38023;
		 public const ushort G2C_QuickPurchaseResponse = 38024;
		 public const ushort ShopProp = 38025;
		 public const ushort PlayerShop = 38026;
		 public const ushort ItemInBackpack_Status = 38027;
		 public const ushort ItemAllProperty = 38028;
		 public const ushort Property = 38029;
		 public const ushort AttrEntry = 38030;
		 public const ushort C2G_DouYinCreateAnOrderRequest = 38031;
		 public const ushort C2G_DouYinCreateAnOrderResponse = 38032;
		 public const ushort C2G_listingTreasureHouse = 38033;
		 public const ushort G2C_listingTreasureHouse = 38034;
		 public const ushort C2G_OpenTreasureHouse = 38035;
		 public const ushort G2C_OpenTreasureHouse = 38036;
		 public const ushort C2G_SearchForItems = 38037;
		 public const ushort G2C_SearchForItems = 38038;
		 public const ushort C2G_NextPage = 38039;
		 public const ushort G2C_NextPage = 38040;
		 public const ushort C2G_GetTreasureHouseItemInfo = 38041;
		 public const ushort G2C_GetTreasureHouseItemInfo = 38042;
		 public const ushort C2G_BuyTreasureHouseItemInfo = 38043;
		 public const ushort G2C_BuyTreasureHouseItemInfo = 38044;
		 public const ushort C2G_OpenConsign = 38045;
		 public const ushort G2C_OpenConsign = 38046;
		 public const ushort C2G_RemovedItems = 38047;
		 public const ushort G2C_RemovedItems = 38048;
		 public const ushort C2G_DeletePlayerTreasureHouse = 38049;
		 public const ushort G2C_DeletePlayerTreasureHouse = 38050;
		 public const ushort TreasureHouseItemInfo = 38051;
		 public const ushort TransactionRecord = 38052;
		 public const ushort ItemBaseInfo = 38501;
		 public const ushort RecGiftBundleInfo = 38502;
		 public const ushort C2G_GetDirectRechargeGiftBundle = 38503;
		 public const ushort G2C_GetDirectRechargeGiftBundle = 38504;
		 public const ushort C2G_GetGiftBundleItemInfo = 38505;
		 public const ushort G2C_GetGiftBundleItemInfo = 38506;
		 public const ushort C2G_TestBuyRechargeGiftBundle = 38507;
		 public const ushort G2C_TestBuyRechargeGiftBundle = 38508;
		 public const ushort LotteryGiftInfo = 38551;
		 public const ushort LotteryLog = 38552;
		 public const ushort C2G_OpenLotteryWin = 38553;
		 public const ushort G2C_OpenLotteryWin = 38554;
		 public const ushort C2G_StartLottery = 38555;
		 public const ushort G2C_StartLottery = 38556;
		 public const ushort C2G_GetHistoryLotteryLog = 38557;
		 public const ushort G2C_GetHistoryLotteryLog = 38558;
		 public const ushort C2G_GetLotteryLog = 38559;
		 public const ushort G2C_GetLotteryLog = 38560;
		 public const ushort C2G_OpenCumulativeRecharge = 38601;
		 public const ushort G2C_OpenCumulativeRecharge = 38602;
		 public const ushort C2G_ReceiveRechargeGiftPack = 38603;
		 public const ushort G2C_ReceiveRechargeGiftPack = 38604;
		 public const ushort C2G_ShouQCreateAnOrder = 38605;
		 public const ushort G2C_ShouQCreateAnOrder = 38606;
		 public const ushort C2G_MyPayTopUp = 38607;
		 public const ushort G2C_MyPayTopUp = 38608;
		 public const ushort C2G_MyV4PayTopUp = 38609;
		 public const ushort G2C_MyV4PayTopUp = 38610;
		 public const ushort C2G_ClearingGiftKit = 38611;
		 public const ushort G2C_ClearingGiftKit = 38612;
		 public const ushort C2G_MagicBoxCreateOrder = 38613;
		 public const ushort G2C_MagicBoxCreateOrder = 38614;
		 public const ushort C2G_BuyYourOwnGiftPackRequest = 38615;
		 public const ushort G2C_BuyYourOwnGiftPackResponse = 38616;
	}
}
