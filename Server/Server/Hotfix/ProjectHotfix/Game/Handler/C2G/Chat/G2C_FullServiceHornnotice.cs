using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class G2C_FullServiceHornnoticeHandler : AMHandler<G2C_FullServiceHornnotice>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, G2C_FullServiceHornnotice b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(G2C_FullServiceHornnotice b_Request)
        {
            var PlayerList = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllPlayers();
            if (PlayerList == null) return false;

            G2C_FullServiceHornnotice g2C_FullServiceHornnotice = new G2C_FullServiceHornnotice();
            g2C_FullServiceHornnotice.SendGameUserId = b_Request.SendGameUserId;
            g2C_FullServiceHornnotice.SendUserName = b_Request.SendUserName;
            g2C_FullServiceHornnotice.SendTime = CustomFrameWork.TimeHelper.ClientNowSeconds();
            g2C_FullServiceHornnotice.MessageInfo = b_Request.MessageInfo;
            g2C_FullServiceHornnotice.LineId = b_Request.LineId;
            foreach (var player in PlayerList)
            {
                foreach (var player2 in player.Value)
                {
                    player2.Value.Send(g2C_FullServiceHornnotice);
                }
            }
            return true;
        }
    }
}