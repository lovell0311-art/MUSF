using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_UnloadEquipItemRequestHandler : AMActorRpcHandler<C2G_UnloadEquipItemRequest, G2C_UnloadEquipItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_UnloadEquipItemRequest b_Request, G2C_UnloadEquipItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_UnloadEquipItemRequest b_Request, G2C_UnloadEquipItemResponse b_Response, Action<IMessage> b_Reply)
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
            if (equipComponent == null || backpackComponent == null)
            {
                // 装备组件或背包组件不存在
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(770);
                b_Reply(b_Response);
                return false;
            } 
            //检查装备栏当前位置是否有装备,
            Item targetItem = equipComponent.GetEquipItemByPosition(b_Request.EquipPosition);
            if (targetItem != null)
            {
                //卸下装备
                if (!equipComponent.UnloadEquipItemToBackpack((EquipPosition)b_Request.EquipPosition, b_Request.PosInBackpackX, b_Request.PosInBackpackY,"卸下装备"))
                {
                    // 卸下失败!背包当前位置不可放置卸下的装备
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(771);
                }
            }
            else
            {
                // 卸下失败，当前位置没有装备
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(772);
            }
            if ((EquipPosition)b_Request.EquipPosition == EquipPosition.Pet)
            {
                var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
                gameplayer.PetsTurnEquipmentTuo(targetItem.ItemUID, targetItem.ConfigData.PetsId);
            }
            b_Reply(b_Response);
            return true;
        }
    }
}