using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using ETModel.ET;

namespace ETHotfix
{
    [FriendOf(typeof(UpdateFrameComponent))]
    [Timer(TimerType.CombatSourceUpdateFrame)]
    public class CombatSourceUpdateFrameTimer : ATimer<UpdateFrameComponent>
    {
        public override void Run(UpdateFrameComponent self)
        {
            CombatSource unit = self.Parent;
            switch (unit.Identity)
            {
                case E_Identity.Hero:
                    self.battleComponent.LateUpdateLogic((GamePlayer)unit, unit.CurrentMap);
                    self.battleComponent?.UpdateLogic((GamePlayer)unit, unit.CurrentMap);
                    break;
                case E_Identity.Enemy:
                    self.battleComponent.LateUpdateLogic((Enemy)unit,TimerComponent.Instance.TimeNow, unit.CurrentMap);
                    self.battleComponent?.UpdateLogic((Enemy)unit, TimerComponent.Instance.TimeNow, unit.CurrentMap);
                    break;
                case E_Identity.Pet:
                    self.battleComponent.LateUpdateLogic((Pets)unit, TimerComponent.Instance.TimeNow, unit.CurrentMap);
                    self.battleComponent?.UpdateLogic((Pets)unit, TimerComponent.Instance.TimeNow, unit.CurrentMap);
                    break;
                case E_Identity.Summoned:
                    self.battleComponent.LateUpdateLogic((Summoned)unit, TimerComponent.Instance.TimeNow, unit.CurrentMap);
                    self.battleComponent?.UpdateLogic((Summoned)unit, TimerComponent.Instance.TimeNow, unit.CurrentMap);
                    break;
                case E_Identity.HolyteacherSummoned:
                    self.battleComponent.LateUpdateLogic((HolyteacherSummoned)unit, TimerComponent.Instance.TimeNow, unit.CurrentMap);
                    self.battleComponent?.UpdateLogic((HolyteacherSummoned)unit, TimerComponent.Instance.TimeNow, unit.CurrentMap);
                    break;
                default:
                    break;
            }
        }
    }

    [FriendOf(typeof(UpdateFrameComponent))]
    [EventMethod(typeof(UpdateFrameComponent), EventSystemType.INIT)]
    public class UpdateFrameComponentEventOnInit : ITEventMethodOnInit<UpdateFrameComponent>
    {
        public void OnInit(UpdateFrameComponent self)
        {
            self.battleComponent = self.Parent.CurrentMap.GetCustomComponent<BattleComponent>();
            if(self.battleComponent == null)
            {
                Log.Error("self.battleComponent == null");
                return;
            }
            self.timerId = TimerComponent.Instance.NewRepeatedTimer(20, TimerType.CombatSourceUpdateFrame, self);
        }
    }

    [FriendOf(typeof(UpdateFrameComponent))]
    [EventMethod(typeof(UpdateFrameComponent), EventSystemType.DISPOSE)]
    public class UpdateFrameComponentEventOnDispose : ITEventMethodOnDispose<UpdateFrameComponent>
    {
        public override void OnDispose(UpdateFrameComponent self)
        {
            self.battleComponent = null;
            TimerComponent.Instance.Remove(ref self.timerId);
        }
    }

    public static class UpdateFrameComponentSystem
    {

    }
}
