using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Linq;

namespace ETHotfix
{
#if DEVELOP
    [MessageHandler(AppType.Game)]
    public class C2G_GMRequestHandler : AMActorRpcHandler<C2G_GMRequest, G2C_GMResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GMRequest b_Request, G2C_GMResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_GMRequest b_Request, G2C_GMResponse b_Response, Action<IMessage> b_Reply)
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

            if (Root.MainFactory.GetCustomComponent<GameMasterComponent>().GMCommandLineDic.TryGetValue(b_Request.Command, out var mGmType) == false)
            {
                b_Response.Error = 99;
                b_Response.Message = "命令不存在";
                b_Reply(b_Response);
                return true;
            }

            var mGmCommand = Root.CreateBuilder.GetInstance<C_GameMasterCommandLine<Player, IResponse>>(mGmType);
            await mGmCommand.Run(mPlayer, b_Request.Parameter.ToList(), b_Response);
            mGmCommand.Dispose();

            b_Reply(b_Response);
            return true;
        }
    }
#endif
}