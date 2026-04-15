using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ETHotfix
{
    [MessageHandler(AppType.AllServer)]
    public class GM2S_RunCodeHandler : AMRpcHandler<GM2S_RunCode, S2GM_RunCode>
    {
        protected override async Task<bool> CodeAsync(Session session, GM2S_RunCode b_Request, S2GM_RunCode b_Response, Action<IMessage> b_Reply)
        {
            ReplComponent repl = session.GetComponent<ReplComponent>();
            if (repl == null)
            {
                repl = session.AddComponent<ReplComponent>();
            }
            else
            {
                repl.CancellationTokenSource?.Cancel();
                repl.CancellationTokenSource = null;
            }

            await repl.RunCode(b_Request.Code);

            b_Response.Return = repl.PrintContext.ToString();
            session.RemoveComponent<ReplComponent>();
            b_Reply(b_Response);
            return true;
        }
    }
}