
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ReceiveTaskRewardHandler : AMActorRpcHandler<C2G_ReceiveTaskReward,
        G2C_ReceiveTaskReward>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ReceiveTaskReward b_Request, G2C_ReceiveTaskReward b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ReceiveTaskReward b_Request, G2C_ReceiveTaskReward b_Response, Action<IMessage> b_Reply)
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
            var tasksComponent = mPlayer.GetCustomComponent<GameTasksComponent>();
            if(!tasksComponent.ReceiveTaskReward(b_Request.TaskId,out var err))
            {
                // 领取任务奖励失败
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(err);
                b_Reply(b_Response);
                return true;
            }
            // 领取成功
            b_Reply(b_Response);
            return true;

        }
    }
}