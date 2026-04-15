using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    public static partial class GameUserSystem
    {
        /// <summary>
        /// 用户从Gate进程下线
        /// </summary>
        /// <param name="self"></param>
        public static async Task OfflineFromGate(this GameUser self, DisconnectType disconnectType = DisconnectType.RepeatLogin)
        {
            if (self.GateServerId != -1)
            {
                var mStartUpInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(self.GateServerId);

                Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mStartUpInfo.ServerInnerIP);
                if (targetSession == null && targetSession.IsDisposed)
                {
                    Log.Error($"玩家的链接失效 {self.UserId} :{self.GateServerId} {mStartUpInfo.ServerInnerIP}");
                }
                await targetSession.Call(new S2Gate_DisconnectGateUser()
                {
                    UserId = self.UserId,
                    DisconnectType = (int)disconnectType
                });
            }
        }
    }
}
