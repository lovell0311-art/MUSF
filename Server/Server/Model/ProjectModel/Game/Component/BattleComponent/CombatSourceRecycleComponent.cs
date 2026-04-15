using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Collections.Generic;


namespace ETModel
{
    [PrivateObject]
    public class CombatSourceRecycleComponent : TCustomComponent<MainFactory>
    {
        public static CombatSourceRecycleComponent Instance;

        /// <summary>
        /// 真死，要清理的单位
        /// </summary>
        public Queue<CombatSource> reallyDeathUnit = new Queue<CombatSource>();


        public void Destroy(CombatSource combatSource)
        {
            if (combatSource.IsDisposeable)
            {
                Log.Error($"{combatSource.InstanceId} CombatSource 重复销毁");
                return;
            }

            if(combatSource.isReallyDeath == false)
            {
                combatSource.isReallyDeath = true;
                reallyDeathUnit.Enqueue(combatSource);
            }
        }
    }
}
