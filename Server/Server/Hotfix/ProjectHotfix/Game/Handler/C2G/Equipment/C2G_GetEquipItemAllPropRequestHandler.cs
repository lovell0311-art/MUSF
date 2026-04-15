using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetEquipItemAllPropRequestHandler : AMActorRpcHandler<C2G_GetEquipItemAllPropRequest, C2G_GetEquipItemAllPropResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetEquipItemAllPropRequest b_Request, C2G_GetEquipItemAllPropResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetEquipItemAllPropRequest b_Request, C2G_GetEquipItemAllPropResponse b_Response, Action<IMessage> b_Reply)
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
            GamePlayer gamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            GamePlayer targetGamePlayer = null;
            foreach(var cell in gamePlayer.CurrentCell.AroundFieldArray)
            {
                if (cell.FieldPlayerDic.TryGetValue(b_Request.GameUserId, out targetGamePlayer)) break;
            }

            if(targetGamePlayer == null)
            {
                // 查看的装备所属玩家已离开
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(776);
                b_Reply(b_Response);
                return false;
            }
            EquipmentComponent targetEquipmentCom = targetGamePlayer.Player.GetCustomComponent<EquipmentComponent>();
            Item targetItem = targetEquipmentCom.GetEquipItemByPosition((EquipPosition)b_Request.EquipPosition);
            if(targetItem == null)
            {
                // 查看的装备已脱下
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(777);
                b_Reply(b_Response);
                return false;
            }

            b_Response.AllProperty = targetItem.ToItemAllProperty();
            b_Reply(b_Response);
            return true;
        }
    }
}