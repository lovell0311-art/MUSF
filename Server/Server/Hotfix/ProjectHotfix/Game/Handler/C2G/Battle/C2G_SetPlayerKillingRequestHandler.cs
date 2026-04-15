using ETModel;
using System;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using UnityEngine;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SetPlayerKillingRequestHandler : AMActorRpcHandler<C2G_SetPlayerKillingRequest, G2C_SetPlayerKillingResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SetPlayerKillingRequest b_Request, G2C_SetPlayerKillingResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_SetPlayerKillingRequest b_Request, G2C_SetPlayerKillingResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家不存在!");
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

            if (b_Request.Model < (int)E_PKModel.Peace || b_Request.Model > (int)E_PKModel.Friend)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(415);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据异常!");
                b_Reply(b_Response);
                return false;
            }

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();

            if (mGamePlayer.Data.Level < 50)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(438);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据异常!");
                b_Reply(b_Response);
                return false;
            }
            var ServerInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>();
            var info = ServerInfo?.GetStartUpInfo(OptionComponent.Options.AppType, OptionComponent.Options.AppId);
            if (info.IsPVP == 1 && (E_PKModel)b_Request.Model != E_PKModel.Friend)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(202);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据异常!");
                b_Reply(b_Response);
                return false;
            }
            else if (info.IsPVP == 2 && (E_PKModel)b_Request.Model != E_PKModel.Peace)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(202);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据异常!");
                b_Reply(b_Response);
                return false;
            }
            mGamePlayer._PKModel = (E_PKModel)b_Request.Model;

            if(mGamePlayer.Pets != null && !mGamePlayer.Pets.IsDeath) 
            {
                mGamePlayer.Pets.Enemy = null;
                mGamePlayer.Pets.TargetEnemy = null;
            }
            if (mGamePlayer.Summoned != null && !mGamePlayer.Summoned.IsDeath)
            {
                mGamePlayer.Summoned.Enemy = null;
                mGamePlayer.Summoned.TargetEnemy = null;
            }

                b_Reply(b_Response);
            return true;
        }
    }
}