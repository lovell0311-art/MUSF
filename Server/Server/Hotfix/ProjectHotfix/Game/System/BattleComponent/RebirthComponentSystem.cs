using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using ETModel.ET;

namespace ETHotfix
{
    [FriendOf(typeof(RebirthComponent))]
    [Timer(TimerType.CombatSourceRebirth)]
    public class RebirthTimer : ATimer<RebirthComponent>
    {
        public override void Run(RebirthComponent self)
        {
            CombatSource unit = self.Parent;
            if (unit.IsDeath == false) return;  // 单位已经活了



            switch (unit.Identity)
            {
                case E_Identity.Hero:
                    if (((GamePlayer)unit).Player.OnlineStatus == EOnlineStatus.Offline) return;
                    RebirthHelper.Rebirth((GamePlayer)unit,"正常复活");
                    break;
                case E_Identity.Enemy:
                    RebirthHelper.Rebirth((Enemy)unit);
                    break;
                case E_Identity.Pet:
                    GamePlayer gamePlayer = ((Pets)unit).GamePlayer;
                    if (gamePlayer == null) return;
                    if (gamePlayer.Player.OnlineStatus == EOnlineStatus.Offline) return;
                    RebirthHelper.Rebirth((Pets)unit);
                    break;
                case E_Identity.Summoned:
                case E_Identity.HolyteacherSummoned:
                default:
                    Log.Error($"没有实现复活方法的单位类型: {unit.Identity}");
                    break;
            }
        }
    }

    [FriendOf(typeof(RebirthComponent))]
    [EventMethod(typeof(RebirthComponent), EventSystemType.INIT)]
    public class RebirthComponentEventOnInit : ITEventMethodOnInit<RebirthComponent>
    {
        public void OnInit(RebirthComponent self)
        {
            CombatSource unit = self.Parent;
            if (unit.CurrentMap != null)
            {
                self.map = unit.CurrentMap;
                self.mapInstanceId = unit.CurrentMap.Id;
                self.deathPosX = unit.UnitData.X;
                self.deathPosY = unit.UnitData.Y;
            }
            else
            {
                self.map = null;
                self.mapInstanceId = 0;
                self.deathPosX = 0;
                self.deathPosY = 0;
            }
            
            self.timerId = TimerComponent.Instance.NewOnceTimer(unit.DeathSleepTime, TimerType.CombatSourceRebirth, self);
        }
    }

    [FriendOf(typeof(RebirthComponent))]
    [EventMethod(typeof(RebirthComponent), EventSystemType.DISPOSE)]
    public class RebirthComponentEventOnDispose : ITEventMethodOnDispose<RebirthComponent>
    {
        public override void OnDispose(RebirthComponent self)
        {
            self.map = null;
            self.mapInstanceId = 0;
            self.deathPosX = 0;
            self.deathPosY = 0;
            TimerComponent.Instance.Remove(ref self.timerId);
        }
    }

    [FriendOf(typeof(RebirthComponent))]
    public static class RebirthComponentSystem
    {


    }
}
