using UnityEngine;
using ETModel;
using UnityEngine.Playables;
using UnityEngine.Profiling;
using System.Collections.Generic;

namespace ETHotfix
{
    [ObjectSystem]
    public class PetEntityAwake : AwakeSystem<PetEntity, GameObject, Pets_InfoConfig>
    {
        public override void Awake(PetEntity self, GameObject gameObject, Pets_InfoConfig pet_InfoConfig)
        {
            self.Awake(gameObject, pet_InfoConfig);
        }
    }

    [ObjectSystem]
    public class PetEntity1Awake : AwakeSystem<PetEntity, GameObject>
    {
        public override void Awake(PetEntity self, GameObject gameObject)
        {
            self.Awake(gameObject);
        }
    }

    /// <summary>
    /// 宠物实体
    /// </summary>
    public class PetEntity : UnitEntity
    {
        public int configId;
        public int MoSpeed;

        public string NorMalAttackEffectResName;

        public long RoleId;
        public long MaxHp;
        public long MaxMp;
       
        Dictionary<int, Pets_SkillConfig> pets_SkillConfig;
        Pets_SkillConfig pet_SkillInfo;
        public void Awake(GameObject gameObject, Pets_InfoConfig pet_InfoConfig)
        {
            
            this.Game_Object = gameObject;
            this.DelayTime = 0;//0秒后回收
           
            configId = (int)pet_InfoConfig.Id;
            MoSpeed= pet_InfoConfig.MoSpeed;

            NorMalAttackEffectResName = GetNorMalAttackEffectResName();

            pets_SkillConfig =new Dictionary<int, Pets_SkillConfig>();
        } 
        public void Awake(GameObject gameObject)
        {
            
            this.Game_Object = gameObject;
            this.DelayTime = 0;//0秒后回收
           
          
        }
        /// <summary>
        /// 获取普攻被击特效
        /// </summary>
        /// <returns></returns>
        public string GetNorMalAttackEffectResName() /*=> configId switch */
        {
            if (configId == 100)
            {
                return "Pet_Rabbit_Attack_Hit";
            }
            else if (configId == 101) { return "Pet_Dragon_Attack_Hit"; }
            else if (configId == 102) { return "Pet_Monkey_Attack_Hit"; }
            else if (configId == 104) { return "Pet_Rabbit_Attack_Hit"; }
            else return "Pet_Rabbit_Attack_Hit";
           
            /* 100=> "Pet_Rabbit_Attack_Hit",
             101=> "Pet_Dragon_Attack_Hit",
             102=> "Pet_Monkey_Attack_Hit",
             104=> "Pet_Rabbit_Attack_Hit",
                _=>string.Empty*/
        }
        private long monsterUUID;
        private int skillID;
        public void SetHitTarget(long targetUid, int skillId)
        {
            monsterUUID = targetUid;
            skillID = skillId;
        }
        public void MasterHit() 
        {
           
            //if (petUUID != monsterUUID) return;
            UnitEntity entity = UnitEntityComponent.Instance?.Get<UnitEntity>(monsterUUID);

            Transform entityPos =null;
            if (entity == null)
            {
                entityPos = this.Game_Object.transform;
                if(entityPos != null)
                    entityPos.position += Vector3.forward * 2;
            }
            else
            {
                if(entity.Game_Object != null)
                    entityPos = entity.Game_Object.transform;
            }
        
            GameObject Oet_hitEffect = null;
           
            if (skillID == 0)//普攻 并且被击特效不为空
            {
               
                Oet_hitEffect = ResourcesComponent.Instance.LoadEffectObject(NorMalAttackEffectResName.StringToAB(), NorMalAttackEffectResName);
                
            }
            else
            {
                //技能 被击特效
                if (pets_SkillConfig.TryGetValue(skillID, out pet_SkillInfo)==false)
                {
                    pet_SkillInfo = ConfigComponent.Instance.GetItem<Pets_SkillConfig>(skillID);
                    if (pet_SkillInfo == null)
                    {
                        return;
                    }
                    pets_SkillConfig[skillID]=pet_SkillInfo;
                }
                if (!string.IsNullOrEmpty(pet_SkillInfo.HitEffect))
                {
                    Oet_hitEffect = ResourcesComponent.Instance.LoadEffectObject(pet_SkillInfo.HitEffect.StringToAB(), pet_SkillInfo.HitEffect);
                }
               
            }
          
            if (Oet_hitEffect == null) return;

            //显示被击 特效 音效
            if (entity is MonsterEntity monsterEntity)
            {
              //  Profiler.BeginSample("MonsterEntityHitEffect");
                monsterEntity.GetComponent<AnimatorComponent>().HitEffect();
               // Profiler.EndSample();
            }
            //被击特效的位置 为当前实体的位置
            if(entityPos == null) return;
            Oet_hitEffect.transform.localPosition = entityPos.position + Vector3.up;
            Oet_hitEffect.transform.localRotation = entityPos.rotation;
            
            return;

        }

        /// <summary>
        /// 宠物被击时
        /// </summary>
        /// <param name="attacker">攻击实体</param>
        /// <param name="hitSkillConfigId">技能配置表iD</param>
        /// <param name="islook"></param>
        public override void Hit(UnitEntity attacker, long hitSkillConfigId)
        {
            base.Hit(attacker, hitSkillConfigId);
        }

        public override void Dead()
        { 

            if(configId == 103)
            {
                Game_Object.GetComponent<PlayableDirector>().enabled = true;
            }
            else
            {
                this.GetComponent<AnimatorComponent>().petDead();
            }
            
            base.Dead();
            TimerComponent.Instance.RegisterTimeCallBack(GetNorMalAttackTime()*1000 + 500, () => 
            {
                this.DelayTime = 0;

                this.Dispose();

            });
            int GetNorMalAttackTime() => configId switch
            {
                100 => 4,
                101 => 2,
                102 => 0,
                103 => 4,
                104 => 4,
                _ => 0
            };
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            this.Game_Object?.SetActive(false);
            base.Dispose();
          
        }
    }

}