using System;
using ETModel;
using System.Threading.Tasks;
namespace ETHotfix
{
    /// <summary>
    /// gate session类型的Mailbox，收到的actor消息直接转发给客户端
    /// </summary>
    [MailboxHandler(AppType.Gate, MailboxType.GateSession)]
    public class MailboxGateSessionHandler : IMailboxHandler
    {
        public async Task Handle(Session session, Entity entity, object actorMessage)
        {
            try
            {
                switch (actorMessage)
                {
                    case IActorMessage iActorMessage:
                        {
                            Session clientSession = entity as Session;
                            iActorMessage.ActorId = 0;
                            clientSession.Send(iActorMessage);
                        }
                        break;
                    default:
                        {
                            Log.Error("MailboxGateSession Not Handler Message:" + actorMessage.GetType().Name);
                            await Game.Scene.GetComponent<ActorMessageDispatcherComponent>().Handle(
                                          entity, new ActorMessageInfo() { Session = session, Message = actorMessage });
                        }
                        break;
                }

                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
