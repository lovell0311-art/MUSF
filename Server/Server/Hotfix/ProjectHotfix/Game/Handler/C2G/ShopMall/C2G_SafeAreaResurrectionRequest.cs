using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Mrs.V20200910.Models;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SafeAreaResurrectionRequestHandler : AMActorRpcHandler<C2G_SafeAreaResurrectionRequest, G2C_SafeAreaResurrectionResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SafeAreaResurrectionRequest b_Request, G2C_SafeAreaResurrectionResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SafeAreaResurrectionRequest b_Request, G2C_SafeAreaResurrectionResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_AccountNotExists;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }

            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (gameplayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            gameplayer.DeathSleepTime = 0;
            b_Reply(b_Response);
            return true;
        }
    }
}