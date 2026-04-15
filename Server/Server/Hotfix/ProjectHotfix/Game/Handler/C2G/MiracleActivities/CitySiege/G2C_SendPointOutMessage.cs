using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class G2C_SendPointOutMessageHandler : AMHandler<G2C_SendPointOutMessage>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, G2C_SendPointOutMessage b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(G2C_SendPointOutMessage b_Request)
        {
            var PlayerList = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllPlayers();
            if (PlayerList == null) return false;

            G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
            g2C_SendPointOutMessage.Status = b_Request.Status;
            g2C_SendPointOutMessage.Pointout = b_Request.Pointout;
            g2C_SendPointOutMessage.PlayerName = b_Request.PlayerName;
            g2C_SendPointOutMessage.WarName = b_Request.WarName;
            g2C_SendPointOutMessage.Time = b_Request.Time;
            g2C_SendPointOutMessage.TitleName = b_Request.TitleName;
            g2C_SendPointOutMessage.PlayerId = b_Request.PlayerId;
            foreach (var player in PlayerList)
            {
                foreach (var player2 in player.Value)
                {
                    player2.Value.Send(g2C_SendPointOutMessage);
                }
            }
            return true;
        }
    }
}