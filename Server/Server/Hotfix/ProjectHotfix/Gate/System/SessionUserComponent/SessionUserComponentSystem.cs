using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class SessionUserComponentDestroySystem : DestroySystem<SessionGateUserComponent>
    {
        public override void Destroy(SessionGateUserComponent self)
        {
            // 发送断线消息
            //ActorLocationSender actorLocationSender = Game.Scene.GetComponent<ActorLocationSenderComponent>().Get(self.User.UnitId);
            //actorLocationSender.Send(new G2M_SessionDisconnect());
            GateUser gateUser = Root.MainFactory.GetCustomComponent<GateUserComponent>().GetUserByUserId(self.UserId);
            if(gateUser != null)
            {
                gateUser.KickGateUser().Coroutine();
            }
            //             if (self.UserId != 0)
            //             {
            //                 var ipEndPoint = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(self.GameServerId);
            //                 //var ipEndPoint = StartConfigComponent.Instance.GetInnerAddress(self.GameServerId);
            //                 Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
            //                 targetSession.Send(new G2Game_SessionDisconnect() { ActorId = self.GameUserId,UserId = self.UserId, GameAreaId = (int)((uint)self.GameAreaId << 16 | (uint)self.GameAreaLineId) });
            // 
            //                 Root.MainFactory.GetCustomComponent<GateUserComponent>().RemoveUserByUserId(self.UserId);
            //                 var mGatePlayer = Root.MainFactory.GetCustomComponent<GatePlayerComponent>().Remove(self.GameUserId);
            //                 if (mGatePlayer != null)
            //                 {
            //                     mGatePlayer.Dispose();
            //                 }
            // 
            //                 //ActorMessageSender actorMessageSender = Game.Scene.GetComponent<ActorMessageSenderComponent>().Get(self.GameUserId);
            //                 //actorMessageSender.Send(new G2Game_SessionDisconnect());
            //                 //Game.Scene.GetComponent<ETModel.GateUserComponent>()?.Remove(0, self.UserId);
            //             }
            //             else
            //             {
            //                 //ActorMessageSender actorMessageSender = Game.Scene.GetComponent<ActorMessageSenderComponent>().Get(self.UserId);
            //                 //actorMessageSender.Send(new G2Game_SessionDisconnect());
            //                 //Game.Scene.GetComponent<ETModel.GateUserComponent>()?.Remove(0, self.UserId);
            //             }
        }
    }
}