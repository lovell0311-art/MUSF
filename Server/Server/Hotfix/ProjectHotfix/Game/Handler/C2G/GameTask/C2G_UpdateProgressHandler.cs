
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_UpdateTaskProgressHandler : AMActorRpcHandler<C2G_UpdateTaskProgress,
        G2C_UpdateTaskProgress>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_UpdateTaskProgress b_Request, G2C_UpdateTaskProgress b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_UpdateTaskProgress b_Request, G2C_UpdateTaskProgress b_Response, Action<IMessage> b_Reply)
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
            var gameTask = tasksComponent.GetTask(b_Request.TaskId);
            if(gameTask == null)
            {
                // 任务不存在
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2004);
                return true;
            }

            // 发布 ClientMsgC2G_UpdateProgress 事件
            ETModel.EventType.ClientMsgC2G_UpdateTaskProgress.Instance.player = mPlayer;
            ETModel.EventType.ClientMsgC2G_UpdateTaskProgress.Instance.request = b_Request;
            Root.EventSystem.OnRun("ClientMsgC2G_UpdateTaskProgress", ETModel.EventType.ClientMsgC2G_UpdateTaskProgress.Instance);

            b_Reply(b_Response);
            return true;

        }
    }
}