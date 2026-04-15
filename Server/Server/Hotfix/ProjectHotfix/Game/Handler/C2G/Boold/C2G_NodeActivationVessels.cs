using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_NodeActivationVesselsHandler : AMActorRpcHandler<C2G_NodeActivationVessels, G2C_NodeActivationVessels>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_NodeActivationVessels b_Request, G2C_NodeActivationVessels b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_NodeActivationVessels b_Request, G2C_NodeActivationVessels b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mServerArea.GameAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0 || b_Request.BloodId == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                b_Reply(b_Response);
                return false;
            }

            var mPlayerBloodAwakening = mPlayer.GetCustomComponent<PlayerBloodAwakeningComponent>();
            if (mPlayerBloodAwakening != null)
            {
                var ItemList = mPlayerBloodAwakening.CheckItem(b_Request.BloodId, b_Request.Node, 2);
                if (ItemList != null)
                {
                    int ErrorId = mPlayerBloodAwakening.ActivateAttribute(b_Request.BloodId,b_Request.RingId,b_Request.Node);
                    if (ErrorId == 3810)
                    {
                        if (!mPlayerBloodAwakening.UseItem(ItemList))
                        {
                            Log.PLog("血脉觉醒", $"玩家:{mPlayer.GameUserId}血脉觉醒节点激活消耗异常");
                        }
                    }
                    b_Response.Error = ErrorId;
                    b_Response.BloodInfo = mPlayerBloodAwakening.GetBloodSendInfo(b_Request.BloodId);
                    b_Reply(b_Response);
                    return true;
                }
                b_Response.Error = 1609;
                b_Reply(b_Response);
                return true;
            }
            b_Response.Error = 3803;
            b_Reply(b_Response);
            return true;
        }
    }
}