using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_CloseEquipUIRequestHandler : AMActorRpcHandler<C2G_CloseEquipUIRequest, G2C_CloseEquipUIResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_CloseEquipUIRequest b_Request, G2C_CloseEquipUIResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_CloseEquipUIRequest b_Request, G2C_CloseEquipUIResponse b_Response, Action<IMessage> b_Reply)
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
            EquipmentComponent equipmentCom = mPlayer.GetCustomComponent<EquipmentComponent>();

            Item weapon = equipmentCom.GetEquipItemByPosition(EquipPosition.Weapon);
            if (weapon != null)
            {
                // 不需要替换
                b_Reply(b_Response);
                return true;
            }
            Item shield = equipmentCom.GetEquipItemByPosition(EquipPosition.Shield);
            if (shield == null)
            {
                // 不需要替换
                b_Reply(b_Response);
                return true;
            }
            if(shield.ConfigData.Slot != (int)EquipPosition.Weapon)
            {
                // 不需要替换
                b_Reply(b_Response);
                return true;
            }
            // TODO 开始将左手的武器换到右手
            equipmentCom.UnloadEquipItem(EquipPosition.Shield, "副手武器切换到主手");
            equipmentCom.EquipItem(shield, EquipPosition.Weapon, "副手武器切换到主手");
            b_Reply(b_Response);
            return true;
        }
    }
}