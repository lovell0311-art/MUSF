
using UnityEngine;
using ETModel;
namespace ETHotfix
{

    [ObjectSystem]
    public class SummonEntityAwake : AwakeSystem<SummonEntity, GameObject,EnemyConfig_InfoConfig>
    {
        public override void Awake(SummonEntity self, GameObject gameObject, EnemyConfig_InfoConfig enemyConfig_)
        {
            self.Awake(gameObject,enemyConfig_);
        }
    }
    /// <summary>
    /// ’ŸªΩ ﬁ µÃÂ
    /// </summary>
    public class SummonEntity : MonsterEntity
    {
        
        public override void Dead()
        {
           
            base.Dead();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
        }
    }
}
