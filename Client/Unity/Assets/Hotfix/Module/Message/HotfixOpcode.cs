using ETModel;
namespace ETHotfix
{
// Opcode = 10000
// 根据GameUserId获取角色信息 通常在其他角色进入视野时请求  成功会推送一系列信息通知
	[Message(HotfixOpcode.C2G_GetPlayerInfoByGameUserIdRequest)]
	public partial class C2G_GetPlayerInfoByGameUserIdRequest : IActorRequest {}

// 根据GameUserId获取角色信息  返回
	[Message(HotfixOpcode.G2C_GetPlayerInfoByGameUserIdResponse)]
	public partial class G2C_GetPlayerInfoByGameUserIdResponse : IActorResponse {}

//--------属性结构-------------
	[Message(HotfixOpcode.Struct_Property)]
	public partial class Struct_Property {}

//--------物品属性词条结构---------
	[Message(HotfixOpcode.Struct_AttrEntry)]
	public partial class Struct_AttrEntry {}

// 根据昵称获取角色GameUserId 角色不存在或者不在线时会返回对应错误码
	[Message(HotfixOpcode.C2G_GetPlayerInfoByNickName)]
	public partial class C2G_GetPlayerInfoByNickName : IActorRequest {}

// 根据昵称获取角色GameUserId  返回
	[Message(HotfixOpcode.G2C_GetPlayerInfoByNickName)]
	public partial class G2C_GetPlayerInfoByNickName : IActorResponse {}

//=========================背包============================
// Opcode = 11100
//---------背包中的物品状态-------------
	[Message(HotfixOpcode.Struct_ItemInBackpack_Status)]
	public partial class Struct_ItemInBackpack_Status {}

// 背包所属玩家ID
// 物品唯一标识
// 物品配置表ID
// 物品类型ID
// 物品在背包中的位置
// 物品占用的格子
// 物品数量
// 物品等级
// 物品所有的属性
	[Message(HotfixOpcode.Struct_ItemAllProperty)]
	public partial class Struct_ItemAllProperty {}

// 移动背包物品的位置 推送G2C_ItemsLocationChangeBackpack_notice
	[Message(HotfixOpcode.C2G_MoveBackpackItemRequest)]
	public partial class C2G_MoveBackpackItemRequest : IActorRequest {}

// 移动背包物品的位置  返回
	[Message(HotfixOpcode.G2C_MoveBackpackItemResponse)]
	public partial class G2C_MoveBackpackItemResponse : IActorResponse {}

// 背包添加新的物品(测试用)  推送G2C_ItemsIntoBackpack_notice
	[Message(HotfixOpcode.C2G_AddBackpackItemRequest)]
	public partial class C2G_AddBackpackItemRequest : IActorRequest {}

// 自定义属性方法
// ItemRandomlyAddExcAttr_3		随机添加3条卓越属性
// 背包添加新的物品(测试用)  返回
	[Message(HotfixOpcode.G2C_AddBackpackItemResponse)]
	public partial class G2C_AddBackpackItemResponse : IActorResponse {}

// 丢弃背包物品  推送G2C_ItemsLeaveBackpack_notice
	[Message(HotfixOpcode.C2G_DelBackpackItemRequest)]
	public partial class C2G_DelBackpackItemRequest : IActorRequest {}

// 丢弃到指定位置 距离小于 10
// 丢弃背包物品  返回
	[Message(HotfixOpcode.G2C_DelBackpackItemResponse)]
	public partial class G2C_DelBackpackItemResponse : IActorResponse {}

//推送客户端背包中有物品进入 选择角色进入游戏场景时会推送玩家背包里现有物品
	[Message(HotfixOpcode.G2C_ItemsIntoBackpack_notice)]
	public partial class G2C_ItemsIntoBackpack_notice : IActorMessage {}

//推送客户端背包中有物品离开
	[Message(HotfixOpcode.G2C_ItemsLeaveBackpack_notice)]
	public partial class G2C_ItemsLeaveBackpack_notice : IActorMessage {}

//推送客户端背包中有物品位置变动
	[Message(HotfixOpcode.G2C_ItemsLocationChangeBackpack_notice)]
	public partial class G2C_ItemsLocationChangeBackpack_notice : IActorMessage {}

//玩家使用背包中的物品
	[Message(HotfixOpcode.C2G_PlayerUseItemInTheBackpack)]
	public partial class C2G_PlayerUseItemInTheBackpack : IActorRequest {}

//返回 玩家使用背包中的物品
	[Message(HotfixOpcode.G2C_PlayerUseItemInTheBackpack)]
	public partial class G2C_PlayerUseItemInTheBackpack : IActorResponse {}

//广播物品属性变动
	[Message(HotfixOpcode.G2C_ItemsPropChange_notice)]
	public partial class G2C_ItemsPropChange_notice : IActorMessage {}

// 广播物品属性词条变动
	[Message(HotfixOpcode.G2C_ItemsAttrEntryChange_notice)]
	public partial class G2C_ItemsAttrEntryChange_notice : IActorMessage {}

//广播玩家背包金币数值变动
	[Message(HotfixOpcode.G2C_BackpackGoldChange_notice)]
	public partial class G2C_BackpackGoldChange_notice : IActorMessage {}

	[Message(HotfixOpcode.ItemPositionInBackpack)]
	public partial class ItemPositionInBackpack {}

// 整理背包
	[Message(HotfixOpcode.C2G_OrganizeBackpack)]
	public partial class C2G_OrganizeBackpack : IActorRequest {}

	[Message(HotfixOpcode.G2C_OrganizeBackpack)]
	public partial class G2C_OrganizeBackpack : IActorResponse {}

//=========================NPC商店=========================
// Opcode = 11150
//获取商店NPC的全部物品
	[Message(HotfixOpcode.C2G_GetShopNPCItems)]
	public partial class C2G_GetShopNPCItems : IActorRequest {}

//获取商店NPC的全部物品  返回
	[Message(HotfixOpcode.G2C_GetShopNPCItems)]
	public partial class G2C_GetShopNPCItems : IActorResponse {}

//玩家购买NPC商店的物品到背包 购买成功会推送物品进入背包
	[Message(HotfixOpcode.C2G_BuyItemFromNPCShop)]
	public partial class C2G_BuyItemFromNPCShop : IActorRequest {}

//玩家购买NPC商店的物品到背包  返回
	[Message(HotfixOpcode.G2C_BuyItemFromNPCShop)]
	public partial class G2C_BuyItemFromNPCShop : IActorResponse {}

//将背包中的物品卖给商店
	[Message(HotfixOpcode.C2G_SellingItemToNPCShop)]
	public partial class C2G_SellingItemToNPCShop : IActorRequest {}

//将背包中的物品卖给商店  返回
	[Message(HotfixOpcode.G2C_SellingItemToNPCShop)]
	public partial class G2C_SellingItemToNPCShop : IActorResponse {}

//=========================装备============================
// Opcode = 11200
// 穿戴装备物品  成功会推送G2C_ItemsLeaveBackpack_notice更新背包物品
	[Message(HotfixOpcode.C2G_EquipItemRequest)]
	public partial class C2G_EquipItemRequest : IActorRequest {}

// 穿戴装备物品  返回
	[Message(HotfixOpcode.G2C_EquipItemResponse)]
	public partial class G2C_EquipItemResponse : IActorResponse {}

// 卸下装备物品  成功会推送G2C_ItemsIntoBackpack_notice更新背包物品
	[Message(HotfixOpcode.C2G_UnloadEquipItemRequest)]
	public partial class C2G_UnloadEquipItemRequest : IActorRequest {}

// 卸下装备物品  返回
	[Message(HotfixOpcode.G2C_UnloadEquipItemResponse)]
	public partial class G2C_UnloadEquipItemResponse : IActorResponse {}

//推送实体穿戴装备(周边一定范围玩家也广播)
	[Message(HotfixOpcode.G2C_UnitEquipLoad_notice)]
	public partial class G2C_UnitEquipLoad_notice : IActorMessage {}

//推送实体卸下装备(周边一定范围玩家也广播)
	[Message(HotfixOpcode.G2C_UnitEquipUnload_notice)]
	public partial class G2C_UnitEquipUnload_notice : IActorMessage {}

//---------卡槽中的物品状态-------------
	[Message(HotfixOpcode.Struct_ItemInSlot_Status)]
	public partial class Struct_ItemInSlot_Status {}

// 维修装备 请求
	[Message(HotfixOpcode.C2G_RepairEquipItemRequest)]
	public partial class C2G_RepairEquipItemRequest : IActorRequest {}

// 维修价格 > 0 的装备
// 在npc处维修需要传下面的参数，检查距离 10 格内的npc,且配置表中 NpcType 为 3
// 维修装备 返回
	[Message(HotfixOpcode.G2C_RepairEquipItemResponse)]
	public partial class G2C_RepairEquipItemResponse : IActorResponse {}

// 替换装备物品  返回
	[Message(HotfixOpcode.C2G_ReplaceEquipItemRequest)]
	public partial class C2G_ReplaceEquipItemRequest : IActorRequest {}

// 替换装备物品  返回
	[Message(HotfixOpcode.G2C_ReplaceEquipItemResponse)]
	public partial class G2C_ReplaceEquipItemResponse : IActorResponse {}

// 获取 装备栏物品属性
	[Message(HotfixOpcode.C2G_GetEquipItemAllPropRequest)]
	public partial class C2G_GetEquipItemAllPropRequest : IActorRequest {}

	[Message(HotfixOpcode.C2G_GetEquipItemAllPropResponse)]
	public partial class C2G_GetEquipItemAllPropResponse : IActorResponse {}

// 玩家关闭了装备界面
// 只有左手装备武器时，会将武器移到右手
	[Message(HotfixOpcode.C2G_CloseEquipUIRequest)]
	public partial class C2G_CloseEquipUIRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_CloseEquipUIResponse)]
	public partial class G2C_CloseEquipUIResponse : IActorResponse {}

//=======================合成====================
// Opcode = 11300
// -----------合并、升级背包中单个物品-------------
// 如两个 药水 数量未到上限，可以进行合并(暂未实现)
// 武器、防具等升级+1~+9
// 武器、防具追加属性
// 武器、防具等再生属性添加、进化
	[Message(HotfixOpcode.C2G_MergeSingleItems)]
	public partial class C2G_MergeSingleItems : IActorRequest {}

// 合并、升级背包中单个物品  返回
	[Message(HotfixOpcode.G2C_MergeSingleItems)]
	public partial class G2C_MergeSingleItems : IActorResponse {}

// 拆分、分堆物品功能   成功会推送拆分物品进入背包和物品数量变化
	[Message(HotfixOpcode.C2G_SplitItems)]
	public partial class C2G_SplitItems : IActorRequest {}

// 拆分、分堆物品功能  返回
	[Message(HotfixOpcode.G2C_SplitItems)]
	public partial class G2C_SplitItems : IActorResponse {}

// 利用合成公式合成物品
	[Message(HotfixOpcode.C2G_ItemsSynthesis)]
	public partial class C2G_ItemsSynthesis : IActorRequest {}

// 利用合成公式合成物品  返回
	[Message(HotfixOpcode.G2C_ItemsSynthesis)]
	public partial class G2C_ItemsSynthesis : IActorResponse {}

// 合成结果
// true.合成成功
// false.合成失败(合成异常请看Error错误码，这里只表示正常情况下的合成失败)
//背包物品到临时空间
	[Message(HotfixOpcode.C2G_MoveBackpackItemToCacheSpace)]
	public partial class C2G_MoveBackpackItemToCacheSpace : IActorRequest {}

// 需要移到缓存中的物品
//背包物品到临时空间 返回
	[Message(HotfixOpcode.G2C_MoveBackpackItemToCacheSpace)]
	public partial class G2C_MoveBackpackItemToCacheSpace : IActorResponse {}

//临时空间到背包物品
	[Message(HotfixOpcode.C2G_MoveCacheSpaceItemToBackpack)]
	public partial class C2G_MoveCacheSpaceItemToBackpack : IActorRequest {}

// 需要移到背包中的物品
// 将物品移到背包中的坐标
//临时空间到背包物品 返回
	[Message(HotfixOpcode.G2C_MoveCacheSpaceItemToBackpack)]
	public partial class G2C_MoveCacheSpaceItemToBackpack : IActorResponse {}

//=============================技能===========================
// Opcode = 11500
// 获取已经学习的技能列表
	[Message(HotfixOpcode.C2G_OpenSkillGroupRequest)]
	public partial class C2G_OpenSkillGroupRequest : IActorRequest {}

// 获取已经学习的技能列表 返回
	[Message(HotfixOpcode.G2C_OpenSkillGroupResponse)]
	public partial class G2C_OpenSkillGroupResponse : IActorResponse {}

// 获取已经学习的技能列表 通知
	[Message(HotfixOpcode.G2C_OpenSkillGroup_notice)]
	public partial class G2C_OpenSkillGroup_notice : IActorMessage {}

// 学习的技能 通知
	[Message(HotfixOpcode.G2C_StudySkillSingle_notice)]
	public partial class G2C_StudySkillSingle_notice : IActorMessage {}

// 禁用技能 通知
	[Message(HotfixOpcode.G2C_DisabledSkillSingle_notice)]
	public partial class G2C_DisabledSkillSingle_notice : IActorMessage {}

//=============================大师===========================
// Opcode = 11600
// 属性种类
	[Message(HotfixOpcode.G2C_HotfixKVData)]
	public partial class G2C_HotfixKVData : IMessage {}

// 大师
	[Message(HotfixOpcode.C2G_OpenBattleMasterRequest)]
	public partial class C2G_OpenBattleMasterRequest : IActorRequest {}

// 大师 返回
	[Message(HotfixOpcode.G2C_OpenBattleMasterResponse)]
	public partial class G2C_OpenBattleMasterResponse : IActorResponse {}

// 大师加点
	[Message(HotfixOpcode.C2G_BattleMasterUpdateLevelRequest)]
	public partial class C2G_BattleMasterUpdateLevelRequest : IActorRequest {}

// 大师加点 返回
	[Message(HotfixOpcode.G2C_BattleMasterUpdateLevelResponse)]
	public partial class G2C_BattleMasterUpdateLevelResponse : IActorResponse {}

//===========================镶嵌==================================
// Opcode = 11700
// 荧之石合成
	[Message(HotfixOpcode.C2G_FluoreStoneCompose)]
	public partial class C2G_FluoreStoneCompose : IActorRequest {}

// 荧之石合成  返回  成功后返回荧之石进入背包
	[Message(HotfixOpcode.G2C_FluoreStoneCompose)]
	public partial class G2C_FluoreStoneCompose : IActorResponse {}

// 荧光宝石合成
	[Message(HotfixOpcode.C2G_FluoreGemsCompose)]
	public partial class C2G_FluoreGemsCompose : IActorRequest {}

// 荧光宝石合成  返回  成功后返回荧光宝石进入背包
	[Message(HotfixOpcode.G2C_FluoreGemsCompose)]
	public partial class G2C_FluoreGemsCompose : IActorResponse {}

// 荧光宝石镶嵌
	[Message(HotfixOpcode.C2G_FluoreGemsSet)]
	public partial class C2G_FluoreGemsSet : IActorRequest {}

// 荧光宝石镶嵌  返回  成功后返回装备属性改动消息
	[Message(HotfixOpcode.G2C_FluoreGemsSet)]
	public partial class G2C_FluoreGemsSet : IActorResponse {}

// 荧光宝石提取
	[Message(HotfixOpcode.C2G_FluoreGemsRecover)]
	public partial class C2G_FluoreGemsRecover : IActorRequest {}

// 荧光宝石提取  返回  成功后返回装备属性改动消息和荧光宝石进入背包
	[Message(HotfixOpcode.G2C_FluoreGemsRecover)]
	public partial class G2C_FluoreGemsRecover : IActorResponse {}

// 荧光宝石强化
	[Message(HotfixOpcode.C2G_FluoreGemsStrengthen)]
	public partial class C2G_FluoreGemsStrengthen : IActorRequest {}

// 荧光宝石提取  返回  成功后返回荧光宝石属性改动消息
	[Message(HotfixOpcode.G2C_FluoreGemsStrengthen)]
	public partial class G2C_FluoreGemsStrengthen : IActorResponse {}

//===========================摆摊==================================
// 摆摊物品结构体
	[Message(HotfixOpcode.C2G_BaiTanItemMessage)]
	public partial class C2G_BaiTanItemMessage : IMessage {}

// 摆摊结构体
	[Message(HotfixOpcode.C2G_BaiTanInfoMessage)]
	public partial class C2G_BaiTanInfoMessage : IMessage {}

// 摆摊
	[Message(HotfixOpcode.C2G_BaiTanRequest)]
	public partial class C2G_BaiTanRequest : IActorRequest {}

// 摆摊 返回
	[Message(HotfixOpcode.G2C_BaiTanResponse)]
	public partial class G2C_BaiTanResponse : IActorResponse {}

// 摆摊设置名字
	[Message(HotfixOpcode.C2G_BaiTanSetNameRequest)]
	public partial class C2G_BaiTanSetNameRequest : IActorRequest {}

// 摆摊设置名字 返回
	[Message(HotfixOpcode.G2C_BaiTanSetNameResponse)]
	public partial class G2C_BaiTanSetNameResponse : IActorResponse {}

// 摆摊添加物品
	[Message(HotfixOpcode.C2G_BaiTanAddItemRequest)]
	public partial class C2G_BaiTanAddItemRequest : IActorRequest {}

// 摆摊添加物品 返回
	[Message(HotfixOpcode.G2C_BaiTanAddItemResponse)]
	public partial class G2C_BaiTanAddItemResponse : IActorResponse {}

// 摆摊移除物品
	[Message(HotfixOpcode.C2G_BaiTanRemoveItemRequest)]
	public partial class C2G_BaiTanRemoveItemRequest : IActorRequest {}

// 摆摊移除物品 返回
	[Message(HotfixOpcode.G2C_BaiTanRemoveItemResponse)]
	public partial class G2C_BaiTanRemoveItemResponse : IActorResponse {}

// 摆摊结果 通知 同时发送邮件
	[Message(HotfixOpcode.G2C_BaiTanResult_notice)]
	public partial class G2C_BaiTanResult_notice : IActorMessage {}

// 附近摆摊数据
	[Message(HotfixOpcode.G2C_BaiTanInstance_notice)]
	public partial class G2C_BaiTanInstance_notice : IActorMessage {}

//修改价格 位置
	[Message(HotfixOpcode.C2G_BaiTanChangeDataRequest)]
	public partial class C2G_BaiTanChangeDataRequest : IActorRequest {}

// 修改价格 位置 返回
	[Message(HotfixOpcode.G2C_BaiTanChangeDataResponse)]
	public partial class G2C_BaiTanChangeDataResponse : IActorResponse {}

// 看一看
	[Message(HotfixOpcode.C2G_BaiTanLookLookRequest)]
	public partial class C2G_BaiTanLookLookRequest : IActorRequest {}

// 看一看 返回
	[Message(HotfixOpcode.G2C_BaiTanLookLookResponse)]
	public partial class G2C_BaiTanLookLookResponse : IActorResponse {}

// 购买摊位物品
	[Message(HotfixOpcode.C2G_BaiTanBuyItemRequest)]
	public partial class C2G_BaiTanBuyItemRequest : IActorRequest {}

// 购买摊位物品 返回
	[Message(HotfixOpcode.G2C_BaiTanBuyItemResponse)]
	public partial class G2C_BaiTanBuyItemResponse : IActorResponse {}

// 取消摊位
	[Message(HotfixOpcode.C2G_BaiTanCloseRequest)]
	public partial class C2G_BaiTanCloseRequest : IActorRequest {}

// 取消摊位 返回
	[Message(HotfixOpcode.G2C_BaiTanCloseResponse)]
	public partial class G2C_BaiTanCloseResponse : IActorResponse {}

// 取消摊位 通知
	[Message(HotfixOpcode.G2C_BaiTanClose_notice)]
	public partial class G2C_BaiTanClose_notice : IActorMessage {}

// 开始摊位
	[Message(HotfixOpcode.C2G_BaiTanOpenRequest)]
	public partial class C2G_BaiTanOpenRequest : IActorRequest {}

// 开始摊位 返回
	[Message(HotfixOpcode.G2C_BaiTanOpenResponse)]
	public partial class G2C_BaiTanOpenResponse : IActorResponse {}

//===========================摆摊==================================
// 玩家退出通知 通知
	[Message(HotfixOpcode.G2C_PlayerSessionDisconnect_notice)]
	public partial class G2C_PlayerSessionDisconnect_notice : IActorMessage {}

// Opcode = 11800
// 加载完成
	[Message(HotfixOpcode.G2C_LoadingComplete)]
	public partial class G2C_LoadingComplete : IActorMessage {}

	[Message(HotfixOpcode.C2G_TaskTransferRequest)]
	public partial class C2G_TaskTransferRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_TaskTransferResponse)]
	public partial class G2C_TaskTransferResponse : IActorResponse {}

	[Message(HotfixOpcode.C2G_OpenTheSpecialTreasureChestRequest)]
	public partial class C2G_OpenTheSpecialTreasureChestRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_OpenTheSpecialTreasureChestResponse)]
	public partial class G2C_OpenTheSpecialTreasureChestResponse : IActorResponse {}

//使用兑换码
	[Message(HotfixOpcode.C2G_UseRedemptionCodeRequest)]
	public partial class C2G_UseRedemptionCodeRequest : IActorRequest {}

//使用兑换码 返回
	[Message(HotfixOpcode.G2C_UseRedemptionCodeResponse)]
	public partial class G2C_UseRedemptionCodeResponse : IActorResponse {}

	[Message(HotfixOpcode.G2C_BagSharenotice)]
	public partial class G2C_BagSharenotice : IActorMessage {}

	[Message(HotfixOpcode.C2G_BagShareGetInfoRequest)]
	public partial class C2G_BagShareGetInfoRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_BagShareGetInfoResponse)]
	public partial class G2C_BagShareGetInfoResponse : IActorResponse {}

	[Message(HotfixOpcode.G2C_SendTreasureHouseItemInfo)]
	public partial class G2C_SendTreasureHouseItemInfo : IActorMessage {}

//改名卡
	[Message(HotfixOpcode.C2G_BagChangeNameCardRequest)]
	public partial class C2G_BagChangeNameCardRequest : IActorRequest {}

//改名卡
	[Message(HotfixOpcode.G2C_BagChangeNameCardResponse)]
	public partial class G2C_BagChangeNameCardResponse : IActorResponse {}

	[Message(HotfixOpcode.C2G_NewSynthesis)]
	public partial class C2G_NewSynthesis : IActorRequest {}

	[Message(HotfixOpcode.G2C_NewSynthesis)]
	public partial class G2C_NewSynthesis : IActorResponse {}

	[Message(HotfixOpcode.NewSynthesisItemInfo)]
	public partial class NewSynthesisItemInfo {}

	[Message(HotfixOpcode.C2G_OpenFashion)]
	public partial class C2G_OpenFashion : IActorRequest {}

	[Message(HotfixOpcode.G2C_OpenFashion)]
	public partial class G2C_OpenFashion : IActorResponse {}

	[Message(HotfixOpcode.C2G_UseFashion)]
	public partial class C2G_UseFashion : IActorRequest {}

	[Message(HotfixOpcode.G2C_UseFashion)]
	public partial class G2C_UseFashion : IActorResponse {}

	[Message(HotfixOpcode.C2G_FashionEnhanceOrAdd)]
	public partial class C2G_FashionEnhanceOrAdd : IActorRequest {}

	[Message(HotfixOpcode.G2C_FashionEnhanceOrAdd)]
	public partial class G2C_FashionEnhanceOrAdd : IActorResponse {}

	[Message(HotfixOpcode.Fashion_Status)]
	public partial class Fashion_Status {}

	[Message(HotfixOpcode.C2G_RenewItemRequest)]
	public partial class C2G_RenewItemRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_RenewItemRequest)]
	public partial class G2C_RenewItemRequest : IActorResponse {}

//坐骑协议 打开面板
	[Message(HotfixOpcode.C2G_OpenTheMountPanelRequest)]
	public partial class C2G_OpenTheMountPanelRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_OpenTheMountPanelResponse)]
	public partial class G2C_OpenTheMountPanelResponse : IActorResponse {}

// Dictionary<MountID, ConfigId>
//获取坐骑信息 内容在G2C_MountInfo通知
	[Message(HotfixOpcode.C2G_GetMountInfoRequest)]
	public partial class C2G_GetMountInfoRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_GetMountInfoResponse)]
	public partial class G2C_GetMountInfoResponse : IActorResponse {}

//坐骑强化
	[Message(HotfixOpcode.C2G_FortifiedMountRequest)]
	public partial class C2G_FortifiedMountRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_FortifiedMountResponse)]
	public partial class G2C_FortifiedMountResponse : IActorResponse {}

//坐骑进阶
	[Message(HotfixOpcode.C2G_AdvancedMountRequest)]
	public partial class C2G_AdvancedMountRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_AdvancedMountResponse)]
	public partial class G2C_AdvancedMountResponse : IActorResponse {}

//坐骑乘骑
	[Message(HotfixOpcode.C2G_UseMountRequest)]
	public partial class C2G_UseMountRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_UseMountResponse)]
	public partial class G2C_UseMountResponse : IActorResponse {}

//坐骑召回
	[Message(HotfixOpcode.C2G_RecallMountRequest)]
	public partial class C2G_RecallMountRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_RecallMountResponse)]
	public partial class G2C_RecallMountResponse : IActorResponse {}

//坐骑召回背包
	[Message(HotfixOpcode.C2G_RecallMountBackpackRequest)]
	public partial class C2G_RecallMountBackpackRequest : IActorRequest {}

	[Message(HotfixOpcode.G2C_RecallMountBackpackResponse)]
	public partial class G2C_RecallMountBackpackResponse : IActorResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixOpcode
	{
		 public const ushort C2G_GetPlayerInfoByGameUserIdRequest = 10001;
		 public const ushort G2C_GetPlayerInfoByGameUserIdResponse = 10002;
		 public const ushort Struct_Property = 10003;
		 public const ushort Struct_AttrEntry = 10004;
		 public const ushort C2G_GetPlayerInfoByNickName = 10005;
		 public const ushort G2C_GetPlayerInfoByNickName = 10006;
		 public const ushort Struct_ItemInBackpack_Status = 11101;
		 public const ushort Struct_ItemAllProperty = 11102;
		 public const ushort C2G_MoveBackpackItemRequest = 11103;
		 public const ushort G2C_MoveBackpackItemResponse = 11104;
		 public const ushort C2G_AddBackpackItemRequest = 11105;
		 public const ushort G2C_AddBackpackItemResponse = 11106;
		 public const ushort C2G_DelBackpackItemRequest = 11107;
		 public const ushort G2C_DelBackpackItemResponse = 11108;
		 public const ushort G2C_ItemsIntoBackpack_notice = 11109;
		 public const ushort G2C_ItemsLeaveBackpack_notice = 11110;
		 public const ushort G2C_ItemsLocationChangeBackpack_notice = 11111;
		 public const ushort C2G_PlayerUseItemInTheBackpack = 11112;
		 public const ushort G2C_PlayerUseItemInTheBackpack = 11113;
		 public const ushort G2C_ItemsPropChange_notice = 11114;
		 public const ushort G2C_ItemsAttrEntryChange_notice = 11115;
		 public const ushort G2C_BackpackGoldChange_notice = 11116;
		 public const ushort ItemPositionInBackpack = 11117;
		 public const ushort C2G_OrganizeBackpack = 11118;
		 public const ushort G2C_OrganizeBackpack = 11119;
		 public const ushort C2G_GetShopNPCItems = 11151;
		 public const ushort G2C_GetShopNPCItems = 11152;
		 public const ushort C2G_BuyItemFromNPCShop = 11153;
		 public const ushort G2C_BuyItemFromNPCShop = 11154;
		 public const ushort C2G_SellingItemToNPCShop = 11155;
		 public const ushort G2C_SellingItemToNPCShop = 11156;
		 public const ushort C2G_EquipItemRequest = 11201;
		 public const ushort G2C_EquipItemResponse = 11202;
		 public const ushort C2G_UnloadEquipItemRequest = 11203;
		 public const ushort G2C_UnloadEquipItemResponse = 11204;
		 public const ushort G2C_UnitEquipLoad_notice = 11205;
		 public const ushort G2C_UnitEquipUnload_notice = 11206;
		 public const ushort Struct_ItemInSlot_Status = 11207;
		 public const ushort C2G_RepairEquipItemRequest = 11208;
		 public const ushort G2C_RepairEquipItemResponse = 11209;
		 public const ushort C2G_ReplaceEquipItemRequest = 11210;
		 public const ushort G2C_ReplaceEquipItemResponse = 11211;
		 public const ushort C2G_GetEquipItemAllPropRequest = 11212;
		 public const ushort C2G_GetEquipItemAllPropResponse = 11213;
		 public const ushort C2G_CloseEquipUIRequest = 11214;
		 public const ushort G2C_CloseEquipUIResponse = 11215;
		 public const ushort C2G_MergeSingleItems = 11301;
		 public const ushort G2C_MergeSingleItems = 11302;
		 public const ushort C2G_SplitItems = 11303;
		 public const ushort G2C_SplitItems = 11304;
		 public const ushort C2G_ItemsSynthesis = 11305;
		 public const ushort G2C_ItemsSynthesis = 11306;
		 public const ushort C2G_MoveBackpackItemToCacheSpace = 11307;
		 public const ushort G2C_MoveBackpackItemToCacheSpace = 11308;
		 public const ushort C2G_MoveCacheSpaceItemToBackpack = 11309;
		 public const ushort G2C_MoveCacheSpaceItemToBackpack = 11310;
		 public const ushort C2G_OpenSkillGroupRequest = 11501;
		 public const ushort G2C_OpenSkillGroupResponse = 11502;
		 public const ushort G2C_OpenSkillGroup_notice = 11503;
		 public const ushort G2C_StudySkillSingle_notice = 11504;
		 public const ushort G2C_DisabledSkillSingle_notice = 11505;
		 public const ushort G2C_HotfixKVData = 11601;
		 public const ushort C2G_OpenBattleMasterRequest = 11602;
		 public const ushort G2C_OpenBattleMasterResponse = 11603;
		 public const ushort C2G_BattleMasterUpdateLevelRequest = 11604;
		 public const ushort G2C_BattleMasterUpdateLevelResponse = 11605;
		 public const ushort C2G_FluoreStoneCompose = 11701;
		 public const ushort G2C_FluoreStoneCompose = 11702;
		 public const ushort C2G_FluoreGemsCompose = 11703;
		 public const ushort G2C_FluoreGemsCompose = 11704;
		 public const ushort C2G_FluoreGemsSet = 11705;
		 public const ushort G2C_FluoreGemsSet = 11706;
		 public const ushort C2G_FluoreGemsRecover = 11707;
		 public const ushort G2C_FluoreGemsRecover = 11708;
		 public const ushort C2G_FluoreGemsStrengthen = 11709;
		 public const ushort G2C_FluoreGemsStrengthen = 11710;
		 public const ushort C2G_BaiTanItemMessage = 11711;
		 public const ushort C2G_BaiTanInfoMessage = 11712;
		 public const ushort C2G_BaiTanRequest = 11713;
		 public const ushort G2C_BaiTanResponse = 11714;
		 public const ushort C2G_BaiTanSetNameRequest = 11715;
		 public const ushort G2C_BaiTanSetNameResponse = 11716;
		 public const ushort C2G_BaiTanAddItemRequest = 11717;
		 public const ushort G2C_BaiTanAddItemResponse = 11718;
		 public const ushort C2G_BaiTanRemoveItemRequest = 11719;
		 public const ushort G2C_BaiTanRemoveItemResponse = 11720;
		 public const ushort G2C_BaiTanResult_notice = 11721;
		 public const ushort G2C_BaiTanInstance_notice = 11722;
		 public const ushort C2G_BaiTanChangeDataRequest = 11723;
		 public const ushort G2C_BaiTanChangeDataResponse = 11724;
		 public const ushort C2G_BaiTanLookLookRequest = 11725;
		 public const ushort G2C_BaiTanLookLookResponse = 11726;
		 public const ushort C2G_BaiTanBuyItemRequest = 11727;
		 public const ushort G2C_BaiTanBuyItemResponse = 11728;
		 public const ushort C2G_BaiTanCloseRequest = 11729;
		 public const ushort G2C_BaiTanCloseResponse = 11730;
		 public const ushort G2C_BaiTanClose_notice = 11731;
		 public const ushort C2G_BaiTanOpenRequest = 11732;
		 public const ushort G2C_BaiTanOpenResponse = 11733;
		 public const ushort G2C_PlayerSessionDisconnect_notice = 11734;
		 public const ushort G2C_LoadingComplete = 11801;
		 public const ushort C2G_TaskTransferRequest = 11802;
		 public const ushort G2C_TaskTransferResponse = 11803;
		 public const ushort C2G_OpenTheSpecialTreasureChestRequest = 11804;
		 public const ushort G2C_OpenTheSpecialTreasureChestResponse = 11805;
		 public const ushort C2G_UseRedemptionCodeRequest = 11806;
		 public const ushort G2C_UseRedemptionCodeResponse = 11807;
		 public const ushort G2C_BagSharenotice = 11808;
		 public const ushort C2G_BagShareGetInfoRequest = 11809;
		 public const ushort G2C_BagShareGetInfoResponse = 11810;
		 public const ushort G2C_SendTreasureHouseItemInfo = 11811;
		 public const ushort C2G_BagChangeNameCardRequest = 11812;
		 public const ushort G2C_BagChangeNameCardResponse = 11813;
		 public const ushort C2G_NewSynthesis = 11814;
		 public const ushort G2C_NewSynthesis = 11815;
		 public const ushort NewSynthesisItemInfo = 11816;
		 public const ushort C2G_OpenFashion = 11817;
		 public const ushort G2C_OpenFashion = 11818;
		 public const ushort C2G_UseFashion = 11819;
		 public const ushort G2C_UseFashion = 11820;
		 public const ushort C2G_FashionEnhanceOrAdd = 11821;
		 public const ushort G2C_FashionEnhanceOrAdd = 11822;
		 public const ushort Fashion_Status = 11823;
		 public const ushort C2G_RenewItemRequest = 11824;
		 public const ushort G2C_RenewItemRequest = 11825;
		 public const ushort C2G_OpenTheMountPanelRequest = 11826;
		 public const ushort G2C_OpenTheMountPanelResponse = 11827;
		 public const ushort C2G_GetMountInfoRequest = 11828;
		 public const ushort G2C_GetMountInfoResponse = 11829;
		 public const ushort C2G_FortifiedMountRequest = 11830;
		 public const ushort G2C_FortifiedMountResponse = 11831;
		 public const ushort C2G_AdvancedMountRequest = 11832;
		 public const ushort G2C_AdvancedMountResponse = 11833;
		 public const ushort C2G_UseMountRequest = 11834;
		 public const ushort G2C_UseMountResponse = 11835;
		 public const ushort C2G_RecallMountRequest = 11836;
		 public const ushort G2C_RecallMountResponse = 11837;
		 public const ushort C2G_RecallMountBackpackRequest = 11838;
		 public const ushort G2C_RecallMountBackpackResponse = 11839;
	}
}
