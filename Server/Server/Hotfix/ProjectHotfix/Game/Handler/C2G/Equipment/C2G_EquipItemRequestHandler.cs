using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_EquipItemRequestHandler : AMActorRpcHandler<C2G_EquipItemRequest, G2C_EquipItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_EquipItemRequest b_Request, G2C_EquipItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_EquipItemRequest b_Request, G2C_EquipItemResponse b_Response, Action<IMessage> b_Reply)
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


                //检查装备栏当前位置是否有装备
                //int position = targetItem.GetConfigData().Slot; 去掉，位置不一定是配置表位置
                Item oldItem = equipComponent.GetEquipItemByPosition(b_Request.EquipPosition);
                if (oldItem != null)
                {
                    //提示卸下装备
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(706);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("穿戴失败，请先卸下当前部位装备!");
                    b_Reply(b_Response);
                    return true;
                }
                //穿上装备
                if (!equipComponent.EquipItemFromBackpack(targetItem, (EquipPosition)b_Request.EquipPosition,"穿戴装备"))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(707);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("穿戴失败!不满足穿戴条件");
                }
                if ((EquipPosition)b_Request.EquipPosition == EquipPosition.Pet)
                {
                    var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
                    gameplayer.PetsTurnEquipmentChuan(targetItem.ItemUID, targetItem.ConfigData.PetsId, targetItem.ConfigData.Name).Coroutine();
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}
