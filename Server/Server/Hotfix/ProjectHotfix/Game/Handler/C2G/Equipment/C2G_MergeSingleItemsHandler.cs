using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_MergeSingleItemsHandler : AMActorRpcHandler<C2G_MergeSingleItems, G2C_MergeSingleItems>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MergeSingleItems b_Request, G2C_MergeSingleItems b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_MergeSingleItems b_Request, G2C_MergeSingleItems b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            var equipComponent = mPlayer.GetCustomComponent<EquipmentComponent>();
            var backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            //if (equipComponent != null && backpackComponent != null)
            {
                //检测背包是否有物品
                Item dragItem = backpackComponent.GetItemByUID(b_Request.ItemUUID);
                Item targetItem = backpackComponent.GetItemByUID(b_Request.TargetItemUUID);
                if (dragItem == null || targetItem == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(705);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("获取背包物品失败!");
                    b_Reply(b_Response);
                    return true;
                }
                if (dragItem.ConfigID == (int)EItemStrengthen.Wing_ResetCard && targetItem.Type == EItemType.Wing)
                {// 翅膀重置卡
                    switch (targetItem.ConfigData.WingLevel)
                    {
                        case 40://1-5
                            {
                                GenerationWingSynthesis.InitWingsProp_Rand(targetItem, 100, 100, 1, 5);
                                targetItem.SendAllPropertyData(mPlayer);
                                targetItem.SendAllEntryAttr(mPlayer);
                                targetItem.SaveDB(mPlayer);
                                backpackComponent.UseItem(dragItem.ItemUID, $"翅膀重置卡 targetItem.UID={targetItem.ItemUID}");
                            }
                            break;
                        case 30:
                        case 25:
                        case 20://1
                            {
                                GenerationWingSynthesis.InitWingsProp_Rand(targetItem, 100, 100, 1, 1);
                                targetItem.SendAllPropertyData(mPlayer);
                                targetItem.SendAllEntryAttr(mPlayer);
                                targetItem.SaveDB(mPlayer);
                                backpackComponent.UseItem(dragItem.ItemUID, $"翅膀重置卡 targetItem.UID={targetItem.ItemUID}");
                            }
                            break;
                        default:
                            //参数错误，0,1代翅膀不能强化
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(774);
                            b_Reply(b_Response);
                            return false;
                            break;
                    }
                }
                else if(dragItem.ConfigID == (int)EItemStrengthen.LUCKY_GEMS && (targetItem.IsEquipment() || targetItem.Type == EItemType.Pets))
                {
                    if(!targetItem.CanHaveLuckyAttr())
                    {
                        // 该物品无法添加幸运属性
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(731);
                        b_Reply(b_Response);
                        return true;
                    }
                    if (targetItem.HaveLuckyAttr())
                    {
                        // 该物品已拥有幸运属性
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(732);
                        b_Reply(b_Response);
                        return true;
                    }
                    // 添加幸运属性
                    if(RandomHelper.RandomNumber(0,100) < ConstItem.LuckyGemsSuccessPct)
                    {
                        mPlayer.PLog($"幸运宝石 添加幸运属性成功 ({dragItem.ToLogString()}) => ({targetItem.ToLogString()})");
                        mPlayer.GetCustomComponent<GamePlayer>()?.SendItem(6, targetItem, true);
                        targetItem.SetProp(EItemValue.LuckyEquip, 1, mPlayer);
                    }
                    else
                    {
                        mPlayer.PLog($"幸运宝石 添加幸运属性失败 ({dragItem.ToLogString()}) => ({targetItem.ToLogString()})");
                        mPlayer.GetCustomComponent<GamePlayer>()?.SendItem(6, targetItem, false);
                    }
                    backpackComponent.UseItem(dragItem.ItemUID, $"添加幸运属性 targetItem={targetItem.ToLogString()}",1);
                }
                else if (dragItem.ConfigID == (int)EItemStrengthen.EXC_GEMS && (targetItem.IsEquipment() || targetItem.Type == EItemType.Pets))
                {
                    if (!targetItem.CanHaveExcellentOption())
                    {
                        // 该物品无法添加卓越属性
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(733);
                        b_Reply(b_Response);
                        return true;
                    }
                    if (targetItem.HaveExcellentOption())
                    {
                        // 该物品已拥有卓越属性
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(734);
                        b_Reply(b_Response);
                        return true;
                    }
                    // 添加卓越属性
                    if (RandomHelper.RandomNumber(0, 100) < ConstItem.ExcGemsSuccessPct)
                    {
                        mPlayer.PLog($"卓越宝石 添加卓越属性成功 ({dragItem.ToLogString()}) => ({targetItem.ToLogString()})");
                        mPlayer.GetCustomComponent<GamePlayer>()?.SendItem(4, targetItem, true);
                        ExcAttrEntryManagerComponent excAttrEntryManager =  Root.MainFactory.GetCustomComponent<ExcAttrEntryManagerComponent>();
                        //if (targetItem.Type == EItemType.Pets)
                        //{
                        //    // 宠物
                        //    if (!excAttrEntryManager.TryGetPetsAttrEntry(out var selector)) Log.Error("宠物添加卓越属性失败");
                        //    if(!selector.TryGetValue(out int entryId)) Log.Error("宠物添加卓越属性失败2");
                        //    targetItem.data.ExcellentEntry.Add(entryId);

                        //    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                        //    var dBProxy2 = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                        //    var PetList = await dBProxy2.Query<DBPetsData>(p => p.GameUserId == 0 && p.PetsId == targetItem.ItemUID && p.IsDisabled != 1);
                        //    DBPetsData dBPetsData = null;
                        //    if (PetList != null && PetList.Count > 0)
                        //    {
                        //        dBPetsData = PetList[0] as DBPetsData;
                        //        dBPetsData.Excellent = Help_JsonSerializeHelper.Serialize(targetItem.data.ExcellentEntry);
                        //        await dBProxy2.Save(dBPetsData);
                        //    }
                        //    else
                        //    {
                        //        Log.Error($"宠物添加卓越属性失败3");
                        //    }
                        //}

                        //else
                        {
                            // 装备
                            if (!excAttrEntryManager.TryGetSelectorByItem(targetItem,out var selector)) Log.Error("装备添加卓越属性失败");
                            targetItem.data.ExcellentEntry.Clear();
                            var newSelector = new RandomSelector<int>(selector);
                            excAttrEntryManager.FlagExcAttrEntryCount.TryGetValue(out int count);
                            if (count < 3) count = 3;

                            while (count > 0)
                            {
                                if (newSelector.TryGetValueAndRemove(out var entryId))
                                {
                                    if (targetItem.data.ExcellentEntry.Add(entryId))
                                    {
                                        --count;
                                    }
                                }
                                else
                                {
                                    // 词条取空了
                                    break;
                                }
                            }
                        }
                        targetItem.UpdateProp();
                        targetItem.SendAllPropertyData(mPlayer);
                        targetItem.SendAllEntryAttr(mPlayer);
                        targetItem.SaveDB(mPlayer);
                    }
                    else
                    {
                        mPlayer.PLog($"卓越宝石 添加卓越属性失败 ({dragItem.ToLogString()}) => ({targetItem.ToLogString()})");
                        mPlayer.GetCustomComponent<GamePlayer>()?.SendItem(4, targetItem, false);
                    }

                    backpackComponent.UseItem(dragItem.ItemUID, $"添加卓越属性 targetItem={targetItem.ToLogString()}", 1);
                }
                else if (dragItem.ConfigID == (int)EItemStrengthen.UNBIND_GEMS && targetItem.ConfigID != dragItem.ConfigID)
                {
                    // 解绑宝石
                    if (targetItem.GetProp(EItemValue.IsBind) == 0)
                    {
                        // 物品无需解绑
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(735);
                        b_Reply(b_Response);
                        return true;
                    }
                    targetItem.SetProp(EItemValue.IsBind, 0, mPlayer);
                    backpackComponent.UseItem(dragItem.ItemUID, $"解绑装备 targetItem={targetItem.ToLogString()}", 1);
                }
                else if (targetItem.ConfigData.Slot > 0)
                {
                    //进入装备强化逻辑
                    int result = StrengthenItemSystem.Strengthen(targetItem, dragItem, mPlayer, out string message);
                    if (result != ErrorCodeHotfix.ERR_Success)
                    {
                        b_Response.Error = result;
                        b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage(message);
                    }
                }
                else if (dragItem.ConfigID == targetItem.ConfigID &&
                    dragItem.GetProp(EItemValue.Level) == targetItem.GetProp(EItemValue.Level) &&
                    dragItem.GetProp(EItemValue.IsBind) == targetItem.GetProp(EItemValue.IsBind) &&
                    targetItem.ConfigData.StackSize > 1)
                {

                    //进入物品合并逻辑
                    int dragItemCount = dragItem.GetProp(EItemValue.Quantity);
                    int targetItemCount = targetItem.GetProp(EItemValue.Quantity);
                    int stackSize = dragItem.ConfigData.StackSize;
                    //主物品若已达到最大组数，则不合并
                    if (targetItemCount == stackSize)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(711);
                        //b_Response.Message = "物品已达到最大组数";
                        b_Reply(b_Response);
                        return true;
                    }
                    //主物品合并后,副物品数量足够则不消失
                    if (dragItemCount + targetItemCount > stackSize)
                    {
                        backpackComponent.SetItemQuantity(targetItem, stackSize);
                        backpackComponent.UseItem(dragItem.ItemUID, $"合并到物品 targetItem.UID={targetItem.ItemUID}", stackSize - targetItemCount);
                    }
                    else
                    {
                        backpackComponent.SetItemQuantity(targetItem, dragItemCount + targetItemCount);
                        backpackComponent.UseItem(dragItem.ItemUID, $"合并到物品 targetItem.UID={targetItem.ItemUID}", dragItemCount);
                    }
                }
                else
                {
                    // 参数错误，物品无法合并
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(730);
                    b_Reply(b_Response);
                    return false;
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}