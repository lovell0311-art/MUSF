using ETModel;

namespace ETHotfix
{
    [SessionMessageDispatcher(SessionMessageDispatcherType.ServerInner)]
    public class InnerMessageDispatcher : IMessageDispatcher
    {
        public void Dispatch(Session session, ushort opcode, object message)
        {
            Game.Scene.GetComponent<MessageDispatcherComponent>().RunMessageAsync(session, new MessageInfo(opcode, message));
            return;


            //// 收到actor消息,放入actor队列
            //switch (message)
            //{
            //    case IActorRequest iActorRequest:
            //        {
            //            HandleIActorRequest(session, iActorRequest);
            //        }
            //        break;

            //    case IActorMessage iactorMessage:
            //        {
            //            HandleIActorMessage(session, iactorMessage);
            //        }
            //        break;
            //    default:
            //        {
            //            Game.Scene.GetComponent<MessageDispatcherComponent>().Handle(session, new MessageInfo(opcode, message));
            //        }
            //        break;
            //}
        }

        //private async void HandleIActorRequest(Session session, IActorRequest message)
        //{
        //    using (await CoroutineLockComponent.Instance.Wait(message.ActorId))
        //    {
        //        Entity entity = (Entity)Game.EventSystem.Get(message.ActorId);
        //        if (entity == null)
        //        {
        //            Log.Warning($"not found actor: {message}");
        //            ActorResponse response = new ActorResponse
        //            {
        //                Error = ErrorCode.ERR_NotFoundActor,
        //                RpcId = message.RpcId
        //            };
        //            session.Reply(response);
        //            return;
        //        }

        //        MailBoxComponent mailBoxComponent = entity.GetComponent<MailBoxComponent>();
        //        if (mailBoxComponent == null)
        //        {
        //            ActorResponse response = new ActorResponse
        //            {
        //                Error = ErrorCode.ERR_ActorNoMailBoxComponent,
        //                RpcId = message.RpcId
        //            };
        //            session.Reply(response);
        //            Log.Error($"actor not add MailBoxComponent: {entity.GetType().Name} {message}");
        //            return;
        //        }

        //        await mailBoxComponent.Add(session, message);
        //    }
        //}

        //private async void HandleIActorMessage(Session session, IActorMessage message)
        //{
        //    using (await CoroutineLockComponent.Instance.Wait(message.ActorId))
        //    {
        //        Entity entity = (Entity)Game.EventSystem.Get(message.ActorId);
        //        if (entity == null)
        //        {
        //            Log.Error($"not found actor: {message}");
        //            return;
        //        }

        //        MailBoxComponent mailBoxComponent = entity.GetComponent<MailBoxComponent>();
        //        if (mailBoxComponent == null)
        //        {
        //            Log.Error($"actor not add MailBoxComponent: {entity.GetType().Name} {message}");
        //            return;
        //        }
        //        await mailBoxComponent.Add(session, message);
        //    }
        //}
    }
}
