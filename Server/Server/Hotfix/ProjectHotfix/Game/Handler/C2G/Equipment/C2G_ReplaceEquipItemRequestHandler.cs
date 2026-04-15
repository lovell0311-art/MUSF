using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ReplaceEquipItemRequestHandler : AMActorRpcHandler<C2G_ReplaceEquipItemRequest, G2C_ReplaceEquipItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ReplaceEquipItemRequest b_Request, G2C_ReplaceEquipItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ReplaceEquipItemRequest b_Request, G2C_ReplaceEquipItemResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
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
            if (equipComponent != null && backpackComponent != null)
            {
                //检测背包是否有物品
                Item targetItem = backpackComponent.GetItemByUID(b_Request.ItemUUID);
                if (targetItem == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(705);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("获取背包物品失败!");
                    b_Reply(b_Response);
                    return true;
                }

                // TODO 物品状态限制 - 穿戴
                if(targetItem.GetProp(EItemValue.IsUsing) != 0)
                {
                    // 使用中的物品无法穿戴
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3122);
                    b_Reply(b_Response);
                    return false;
                }
                if (targetItem.GetProp(EItemValue.IsLocking) != 0)
                {
                    // 锁定的物品无法穿戴
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3123);
                    b_Reply(b_Response);
                    return false;
                }

                if (!equipComponent.CheckEquipRule(targetItem, (EquipPosition)b_Request.EquipPosition))
                {
                    // 穿戴失败!不满足穿戴条件
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(707);
                    b_Reply(b_Response);
                    return true;
                }

                Item oldItem = equipComponent.GetEquipItemByPosition(b_Request.EquipPosition);
                if(oldItem == null)
                {
                    // 直接装备，无需替换
                    // 穿上装备
                    if (!equipComponent.EquipItemFromBackpack(targetItem, (EquipPosition)b_Request.EquipPosition, "替换装备"))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(707);
                        b_Reply(b_Response);
                        return true;
                    }
                    if ((EquipPosition)b_Request.EquipPosition == EquipPosition.Pet)
                    {
                        var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
                        gameplayer.PetsTurnEquipmentChuan(targetItem.ItemUID, targetItem.ConfigData.PetsId, targetItem.ConfigData.Name).Coroutine();
                    }
                }
                else
                {
                    // 替换装备
                    int posX = targetItem.data.posX;
                    int posY = targetItem.data.posY;

                    using ItemsBoxStatus itemBox = backpackComponent.mItemBox.Clone();
                    itemBox.RemoveItem(
                        targetItem.ConfigData.X,
                        targetItem.ConfigData.Y,
                        posX,
                        posY);

                    if(!itemBox.CheckStatus(
                        oldItem.ConfigData.X,
                        oldItem.ConfigData.Y,
                        posX,
                        posY))
                    {
                        // 穿戴失败，原位置不可放置卸下的装备
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(773);
                        b_Reply(b_Response);
                        return true;
                    }
                    equipComponent.UnloadEquipItem((EquipPosition)b_Request.EquipPosition, "替换装备2");
                    if (!equipComponent.EquipItemFromBackpack(targetItem, (EquipPosition)b_Request.EquipPosition, "替换装备3"))
                    {
                        // 穿戴失败!不满足穿戴条件
                        // 不应该走到这里
                        Log.Error($"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 替换装备3失败 ({targetItem.ToLogString()})");
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(707);
                        b_Reply(b_Response);
                        return true;
                    }
                    if ((EquipPosition)b_Request.EquipPosition == EquipPosition.Pet)
                    {
                        var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
                        gameplayer.PetsTurnEquipmentChuan(targetItem.ItemUID, targetItem.ConfigData.PetsId, targetItem.ConfigData.Name).Coroutine();
                    }
                    // 将旧装备还给玩家
                    if (!backpackComponent.AddItem(oldItem,posX,posY,"替换装备4"))
                    {
                        // 卸下失败!背包当前位置不可放置卸下的装备
                        // 不应该走到这
                        Log.Error($"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 替换装备4失败 ({targetItem.ToLogString()})");
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(771);
                        b_Reply(b_Response);
                        return true;
                    }
                    if ((EquipPosition)b_Request.EquipPosition == EquipPosition.Pet)
                    {
                        var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
                        gameplayer.PetsTurnEquipmentTuo(oldItem.ItemUID, oldItem.ConfigData.PetsId);
                    }
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}
