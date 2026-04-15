using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class R2G_GetLoginKeyHandler : AMRpcHandler<R2G_GetLoginKey, G2R_GetLoginKey>
    {
        protected override async Task<bool> CodeAsync(Session session, R2G_GetLoginKey b_Request, G2R_GetLoginKey b_Response, Action<IMessage> b_Reply)
        {
            long key = Help_UniqueValueHelper.GetServerUniqueValue(); //RandomHelper.RandInt64();

            GateSessionKeyComponent mComponent = Root.MainFactory.GetCustomComponent<GateSessionKeyComponent>();
            if (mComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(108);
                b_Reply(b_Response);
                return true;
            }

            while (mComponent.Get(key) != null)
            {
                await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(20);

                Log.Warning($"重新获取一个Key:{key}  Account:{b_Request.Account}");
                key = Help_UniqueValueHelper.GetServerUniqueValue();
            }

            mComponent.Add(b_Request.Account, key);
            b_Response.Key = key;

            b_Reply(b_Response);
            return true;
        }
    }
}