using ETModel;
using UnityEngine;


namespace ETHotfix
{
    [ObjectSystem]
    public class MonsterEntityAwake : AwakeSystem<MonsterEntity, GameObject, EnemyConfig_InfoConfig>
    {
        public override void Awake(MonsterEntity self, GameObject gameObject, EnemyConfig_InfoConfig enemyConfig_InfoConfig)
        {
            self.Awake(gameObject, enemyConfig_InfoConfig);
        }
    }

    /// <summary>
    /// 怪物实体
    /// </summary>
    public class MonsterEntity : UnitEntity
    {
     
        public string Sound_Attack;//攻击音效
        public string Sound_Dead;//死亡音效
        public string Sound_Idle;//死亡音效
        public string AttackEffect;//死亡音效
        public int monsterConfigId;//怪物配置表ID
        public int MoveSpeed;//移动时间
        public int MonsterType;//怪物类型
        public string MonsterName;//怪物名字
       
        readonly string Men = "TransformPoint_KunDunFuBen";

       

        public void Awake(GameObject gameObject, EnemyConfig_InfoConfig enemyConfig_)
        {

            IsDead = false;
            this.Game_Object = gameObject;
            this.DelayTime = 0;//2秒后回收

          

            Sound_Attack =enemyConfig_.Sound_Attack;
            Sound_Dead = enemyConfig_.Sound_Dead;
            monsterConfigId = (int)enemyConfig_.Id;
            MoveSpeed = enemyConfig_.MoSpeed;
            MonsterType = enemyConfig_.Monster_Type;
            MonsterName = enemyConfig_.Name;
            AttackEffect = enemyConfig_.AttackEffect;

        }

        /// <summary>
        /// 怪物被击时
        /// </summary>
        /// <param name="attacker">攻击实体</param>
        /// <param name="hitSkillConfigId">技能配置表iD</param>
        public override void Hit(UnitEntity attacker, long hitSkillConfigId)
        {
            base.Hit(attacker, hitSkillConfigId);
        }


        public override void Dead()
        {
            base.Dead();

            if (this.MonsterType == 1 || this.MonsterType == 6)
            {
                UIMainComponent.Instance?.HideBossHp();
                //UIMainComponent.Instance?.HideDamagedRank();
            }

            if (this.monsterConfigId == 197)//门
            {
                for (int i = 0,len = this.Game_Object.transform.childCount; i < len; i++)
                {
                    this.Game_Object.transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
                }
                this.DelayTime = 4*1000;
            }
            else if (this.MonsterName == "昆顿")//昆顿
            {
                TransformPointManager.Instance.AddPoint(CurrentNodePos.AreaPosX, CurrentNodePos.AreaPosY);
                GameObject attackEffect = ResourcesComponent.Instance.LoadEffectObject(Men.StringToAB(), Men);
                attackEffect.transform.position = AstarComponent.Instance.GetVectory3(CurrentNodePos.AreaPosX, CurrentNodePos.AreaPosY);
            }
            else
            {
                if (this.monsterConfigId != 546)//天鹰
                {
                    this.GetComponent<AnimatorComponent>().Dead();
                }
                // 无需停止移动，组件Dispose后，移动会停止
                //this.GetComponent<UnitEntityMoveComponent>().moveTcs = null;
                    this.DelayTime = 2 * 1000;
                
            }
           // Log.DebugGreen($"怪物 {ConfigInfo.Name} 死亡 {this.Id} {this.GetComponent<UIUnitEntityHpBarComponent>()?.nameTxt.text} 所在区域：Index:{CurrentNodePos.AreaIndex} -> {CurrentNodePos.AreaPosX}:{CurrentNodePos.AreaPosY}");
            this.Dispose();
        }


        public override void Dispose()
        {
            if (this.IsDisposed)
                return;

            base.Dispose();
            if (MonsterType == 1 || MonsterType == 6)
            {
                UIMainComponent.Instance?.HideBossHp();
            }

        }
    }

}