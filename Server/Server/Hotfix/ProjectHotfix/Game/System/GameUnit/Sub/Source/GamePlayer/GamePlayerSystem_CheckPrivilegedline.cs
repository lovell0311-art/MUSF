using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using ETModel;
using ETModel.EventType;
using SharpCompress.Common;
using System.Security.Cryptography;
using TencentCloud.Bri.V20190328.Models;
using System.Runtime.InteropServices;

namespace ETHotfix
{
    /// <summary>
    /// 玩家上线准备完成
    /// </summary>
    [EventMethod("PlayerReadyComplete")]
    public class PlayerPrivilegedline_Init : ITEventMethodOnRun<PlayerReadyComplete>
    {
        public void OnRun(PlayerReadyComplete args)
        {
            var gamePlayer = args.player.GetCustomComponent<GamePlayer>();
            var ServerInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>();
            var info = ServerInfo?.GetStartUpInfo(OptionComponent.Options.AppType, OptionComponent.Options.AppId);
            if (info.IsVIP == 1)
            {
                gamePlayer.AddCustomComponent<CheckPrivilegedLineComponent>();
            }

        }
    }
    [FriendOf(typeof(CheckPrivilegedLineComponent))]
    [Timer(TimerType.CheckPrivilegedLine)]
    public class CheckPrivilegedLineTimer : ATimer<CheckPrivilegedLineComponent>
    {
        public override void Run(CheckPrivilegedLineComponent self)
        {
            var gamePlayer = self.Parent.Player.GetCustomComponent<PlayerShopMallComponent>();
            var ServerInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>();
            var info = ServerInfo?.GetStartUpInfo(OptionComponent.Options.AppType, OptionComponent.Options.AppId);
            if (info.IsVIP == 1)
            {
                if (self.Parent.Player.GetCustomComponent<GamePlayer>().Data.Level >= 30)
                {
                    if (!gamePlayer.GetPlayerShopState(DeviationType.MinMonthlyCard))
                    {
                        if (!gamePlayer.GetPlayerShopState(DeviationType.MaxMonthlyCard))
                            AccountHelper.Kick(self.Parent.Player.UserId, "月卡到期").Coroutine();
                    }
                }
            }
        }
    }
    [FriendOf(typeof(CheckPrivilegedLineComponent))]
    [EventMethod(typeof(CheckPrivilegedLineComponent), EventSystemType.INIT)]
    public class CheckPrivilegedLineOnInit : ITEventMethodOnInit<CheckPrivilegedLineComponent>
    {
        public void OnInit(CheckPrivilegedLineComponent self)
        {
            self.timerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000 * 30, TimerType.CheckPrivilegedLine, self);
        }
    }
    [FriendOf(typeof(CheckPrivilegedLineComponent))]
    [EventMethod(typeof(CheckPrivilegedLineComponent), EventSystemType.DISPOSE)]
    public class CheckPrivilegedLineOnDispose : ITEventMethodOnDispose<CheckPrivilegedLineComponent>
    {
        public override void OnDispose(CheckPrivilegedLineComponent self)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref self.timerId);
        }
    }
}