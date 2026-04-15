using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 宠物· 动画 组件
    /// </summary>
    public partial class AnimatorComponent
    {
        public PetEntity petEntity;
       
        GameSetInfo petGameSetInfo;
        int petSpaceTime;//待机音效间隔时间

        public UnitEntity targetEntity;//目标实体
        public Pets_SkillConfig pets_SkillConfig;//宠物技能配置表
        public void Init_Pet(PetEntity petEntity)
        {
            gameSetInfo = LocalDataJsonComponent.Instance.gameSetInfo;
            this.petEntity = petEntity;
            if (!petEntity.Game_Object.TryGetComponent(out AnimationEventProxy animationEventProxy))
            {
                animationEventProxy = petEntity.Game_Object.AddComponent<AnimationEventProxy>();
            }
            petEntity.Game_Object.GetComponent<AnimationEventProxy>().Effect_action = petSkillEffect;//注册 怪物 使用技能 释放特效方法
            petEntity.Game_Object.GetComponent<AnimationEventProxy>().Effect_Action_Str = PetSkillEffect;//注册 怪物 使用技能 释放特效方法
            petEntity.Game_Object.GetComponent<AnimationEventProxy>().Monster_DeadEffect_action = PetDeadEffect;//注册 怪物 使用技能 释放特效方法

            petGameSetInfo = LocalDataJsonComponent.Instance.gameSetInfo;
            petSpaceTime = RandomHelper.RandomNumber(3, 8);

        }

        public void petSkillEffect()
        {
        }
        /// <summary>
        /// 显示 被击动画
        /// </summary>
        public void PetHitEffect()
        {
            SetTrigger(MotionType.Hit);
        }

        /// <summary>
        /// 播放 宠物 攻击音效
        /// </summary>
        public void PlayPetAttackSound()
        {
            //if (string.IsNullOrEmpty(petEntity.ConfigInfo.Sound_Attack)) return;
            //SoundComponent.Instance.PlayClip(petEntity.ConfigInfo.Sound_Attack, petEntity.Position);
        }

        Vector3 targetEntityPos = Vector3.zero;
        /// <summary>
        /// 宠物攻击特效
        /// </summary>
        /// <param name="effectName">特效名字</param>
        public void PetSkillEffect(string effectName)
        {
            GameObject petHitEffect = ResourcesComponent.Instance.LoadEffectObject(effectName.StringToAB(), effectName);
            if (petEntity == null) return;
            petHitEffect.transform.position = petEntity.CurrentPos + Vector3.up*2;

            if(targetEntityPos == null) return;
            petHitEffect.transform.LookAt(targetEntityPos + Vector3.up);
            if (petHitEffect.GetComponent<PetSkillTrack>() is PetSkillTrack petSkill)
                petSkill.Target = targetEntityPos;
        }
        public void PetDeadEffect(string effectName)
        {
           // Log.Debug("------播放死亡特效");
            GameObject petHitEffect = ResourcesComponent.Instance.LoadEffectObject(effectName.StringToAB(), effectName);
            petHitEffect.transform.position = petEntity.CurrentPos;
            petHitEffect.transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// 宠物 使用技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="UnitUUID">目标 实体的UUID</param>
        public void UsePetSkill(int skillId = 0,long UnitUUID = 0)
        {

            if ((gameSetInfo.CloseEffect || GlobalDataManager.IsHideRole == false) && petEntity.RoleId != UnitEntityComponent.Instance.LocaRoleUUID)
            {
                //关闭其他玩家的特效
                return;
            }


            targetEntity = UnitEntityComponent.Instance.Get<UnitEntity>(UnitUUID);
            if (targetEntity == null) return;
            //Log.DebugGreen($"Entitys是否为null{targetEntity == null}");
            pets_SkillConfig = ConfigComponent.Instance.GetItem<Pets_SkillConfig>((int)skillId);
            targetEntityPos = targetEntity.Position;
            petEntity.Game_Object.transform.LookAt(targetEntity.Position);
            SetTrigger(skillId==0?MotionType.Attack: MotionType.Skill);

            if (pets_SkillConfig != null && !string.IsNullOrEmpty(pets_SkillConfig.AttackEffect))
            {
               
                GameObject petAttackEffect = ResourcesComponent.Instance.LoadEffectObject(pets_SkillConfig.AttackEffect.StringToAB(), pets_SkillConfig.AttackEffect);

                if (petAttackEffect == null) return;
                //0：无（被动技能没有目标）1：自身 2：目标 3：自身向目标移动
                if (pets_SkillConfig.SkillTarget == 2)
                {
                    petAttackEffect.gameObject.transform.position = targetEntity.Game_Object.transform.position;
                    petAttackEffect.transform.localScale = Vector3.one;
                    return;
                }
                petAttackEffect.gameObject.transform.position = petEntity.Game_Object.transform.position;
                petAttackEffect.transform.localScale = Vector3.one;
                petAttackEffect.transform.LookAt(targetEntity.Position);
                if (petAttackEffect.GetComponent<PetSkillTrack>() != null)
                {
                    petAttackEffect.GetComponent<PetSkillTrack>().Target = targetEntity.Game_Object.transform.position;
                }
            }
        }
        public void petDead() 
        {
            //if(!string.IsNullOrEmpty(petEntity.ConfigInfo.Sound_Dead))
            //SoundComponent.Instance.PlayClip(petEntity.ConfigInfo.Sound_Dead, petEntity.Position);//播放死亡音效
            SetTrigger(MotionType.Dead);//播放死亡动画 
        }

        /// <summary>
        /// 播放待机音效
        /// </summary>
        //public void UpdateIdleSound() 
        //{
        //    if (petGameSetInfo !=null&& petGameSetInfo.CloseSound) return; ///关闭音效
        //    if (Time.deltaTime % spaceTime == 0) //间隔 spaceTime 播放一次 待机音效
        //    {
        //        SoundComponent.Instance.PlayClip(monsterEntity.ConfigInfo.Sound_Idle,monsterEntity.Position);
        //    }
        //}

    }
}