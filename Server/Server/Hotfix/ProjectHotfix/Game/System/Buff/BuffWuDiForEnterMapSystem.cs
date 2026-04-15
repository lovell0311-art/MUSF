using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using ETModel.ET;

namespace ETHotfix
{
    [Timer(TimerType.Buff_WuDiForEnterMap)]
    public class Buff_WuDiForEnterMapTimer : ATimer<BuffWuDiForEnterMap>
    {
        public override void Run(BuffWuDiForEnterMap self)
        {
            self.Parent.RemoveCustomComponent<BuffWuDiForEnterMap>();
        }
    }

    [FriendOf(typeof(BuffWuDiForEnterMap))]
    [EventMethod(typeof(BuffWuDiForEnterMap), EventSystemType.INIT)]
    public class BuffWuDiForEnterMapEventOnInit : ITEventMethodOnInit<BuffWuDiForEnterMap>
    {
        public void OnInit(BuffWuDiForEnterMap self)
        {
            int time = 10000;
            self.timerId = TimerComponent.Instance.NewRepeatedTimer(time, TimerType.Buff_WuDiForEnterMap, self);

            // 添加无敌状态
            self.combatSource = self.Parent;
            CombatSource combatSource = self.combatSource;
            self.battleComponent = combatSource.CurrentMap.GetCustomComponent<BattleComponent>();
            combatSource.AddHealthState(E_BattleSkillStats.WuDi, 0, time, 0, null, self.battleComponent);
            combatSource.UpdateHealthState();
        }
    }

    [FriendOf(typeof(BuffWuDiForEnterMap))]
    [EventMethod(typeof(BuffWuDiForEnterMap), EventSystemType.DISPOSE)]
    public class BuffWuDiForEnterMapEventOnDispose : ITEventMethodOnDispose<BuffWuDiForEnterMap>
    {
        public override void OnDispose(BuffWuDiForEnterMap self)
        {
            TimerComponent.Instance.Remove(ref self.timerId);

            // 移除无敌状态
            CombatSource combatSource = self.combatSource;
            combatSource.RemoveHealthState(E_BattleSkillStats.WuDi, self.battleComponent);
            combatSource.UpdateHealthState();
            self.combatSource = null;
            self.battleComponent = null;
        }
    }

    [EventMethod("CombatSourceEnterOrSwitchMap")]
    public class CombatSourceEnterOrSwitchMap_AddWuDiBuff : ITEventMethodOnRun<ETModel.EventType.CombatSourceEnterOrSwitchMap>
    {
        public void OnRun(ETModel.EventType.CombatSourceEnterOrSwitchMap args)
        {
            if (args.combatSource.Identity != E_Identity.Hero) return;

            // 玩家进入或切换地图
            GamePlayer gamePlayer = (GamePlayer)args.combatSource;

            // 添加新的无敌buff 5秒
            gamePlayer.RemoveCustomComponent<BuffWuDiForEnterMap>();
            gamePlayer.AddCustomComponent<BuffWuDiForEnterMap>();
        }
    }



    public static class BuffWuDiForEnterMapSystem
    {
    }
}
