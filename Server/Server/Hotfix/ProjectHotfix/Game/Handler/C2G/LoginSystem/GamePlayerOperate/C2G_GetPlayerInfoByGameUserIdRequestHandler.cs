using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetPlayerInfoByGameUserIdRequestHandler : AMActorRpcHandler<C2G_GetPlayerInfoByGameUserIdRequest, G2C_GetPlayerInfoByGameUserIdResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetPlayerInfoByGameUserIdRequest b_Request, G2C_GetPlayerInfoByGameUserIdResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetPlayerInfoByGameUserIdRequest b_Request, G2C_GetPlayerInfoByGameUserIdResponse b_Response, Action<IMessage> b_Reply)
        {
            //GameUser mGameUser = Root.MainFactory.GetCustomComponent<GameUserComponent>().GetPlayer(b_Request.ActorId);
            //if (mGameUser == null)
            //{
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(120);
            //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
            //    b_Reply(b_Response);
            //    return true;
            //}
            //// 区服判断
            //if (mGameUser.GameAreaId <= 0)
            //{
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(121);
            //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
            //    b_Reply(b_Response);
            //    return true;
            //}

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

            //推送目标玩家信息
            Player tPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.GameUserId);
            if (tPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(300);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("视野内没有玩家信息!");
                b_Reply(b_Response);
                return false;
            }

            //装备
            EquipmentComponent equipComponent = tPlayer.GetCustomComponent<EquipmentComponent>();
            if (equipComponent != null)
            {
                equipComponent.NotifyAllEquipToPlayer(mPlayer);
            }

            b_Reply(b_Response);
            return true;
        }
    }
}