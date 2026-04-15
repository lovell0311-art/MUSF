using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETHotfix
{
    [FriendOf(typeof(CombatSourceRecycleComponent))]
    [EventMethod(typeof(CombatSourceRecycleComponent), EventSystemType.INIT)]
    public class CombatSourceRecycleComponentEventOnInit : ITEventMethodOnInit<CombatSourceRecycleComponent>
    {
        public void OnInit(CombatSourceRecycleComponent self)
        {
            CombatSourceRecycleComponent.Instance = self;
        }
    }

    [FriendOf(typeof(CombatSourceRecycleComponent))]
    [EventMethod(typeof(CombatSourceRecycleComponent), EventSystemType.DISPOSE)]
    public class CombatSourceRecycleComponentEventOnDispose : ITEventMethodOnDispose<CombatSourceRecycleComponent>
    {
        public override void OnDispose(CombatSourceRecycleComponent self)
        {
            CombatSourceRecycleComponent.Instance = null;
            self.reallyDeathUnit.Clear();
        }
    }


    [FriendOf(typeof(CombatSourceRecycleComponent))]
    public static class CombatSourceRecycleComponentSystem
    {
        /// <summary>
        /// 清理真死单位
        /// </summary>
        public static void ClearCombatSource(this CombatSourceRecycleComponent self)
        {
            while (self.reallyDeathUnit.Count > 0)
            {
                CombatSource combatSource = self.reallyDeathUnit.Dequeue();
                if (combatSource.IsDisposeable == true) continue;
                if (combatSource.IsReallyDeath == false) continue;  // 取消被清理了
                MapComponent currentMap = combatSource.CurrentMap;
                switch (combatSource.Identity)
                {
                    case E_Identity.Enemy:
                        {
                            currentMap?.Leave(combatSource);
                            // 回收
                            combatSource.Dispose();
                        }
                        break;
                    case E_Identity.Summoned:
                        {
                            currentMap?.Leave(combatSource);

                            G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                            mAttackResultNotice.AttackTarget = combatSource.InstanceId;
                            mAttackResultNotice.HpValue = 0;

                            currentMap?.SendNotice(combatSource, mAttackResultNotice);

                            // 回收
                            combatSource.Dispose();
                        }
                        break;
                    case E_Identity.HolyteacherSummoned:
                        {
                            currentMap?.Leave(combatSource);

                            G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                            mAttackResultNotice.AttackTarget = combatSource.InstanceId;
                            mAttackResultNotice.HpValue = 0;

                            currentMap?.SendNotice(combatSource, mAttackResultNotice);

                            // 回收
                            combatSource.Dispose();
                        }
                        break;
                    default:
                        Log.Error($"没实现的清理类型。instanceId:{combatSource.Identity} Identity:{combatSource.Identity}");
                        break;

                }

            }
        }
    }
}
