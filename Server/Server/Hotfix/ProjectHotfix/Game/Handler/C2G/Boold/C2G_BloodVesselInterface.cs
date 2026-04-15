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
    public class C2G_BloodVesselInterfaceHandler : AMActorRpcHandler<C2G_BloodVesselInterface, G2C_BloodVesselInterface>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BloodVesselInterface b_Request, G2C_BloodVesselInterface b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_BloodVesselInterface b_Request, G2C_BloodVesselInterface b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mServerArea.GameAreaId, b_Request.ActorId);
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

            var mPlayerBloodAwakening = mPlayer.GetCustomComponent<PlayerBloodAwakeningComponent>();
            if (mPlayerBloodAwakening != null)
            {
                if (mPlayerBloodAwakening.BloodAwakeningInfo.Count != 0)
                {
                    var List = mPlayerBloodAwakening.BloodAwakeningInfo.Keys.ToList();
                    b_Response.BloodIdList.AddRange(List);
                }
                b_Reply(b_Response);
                return true;
            }
            b_Response.Error = 3803;
            b_Reply(b_Response);
            return true;
        }
    }
}