using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.SocialPlatforms;

namespace ETHotfix
{
    [ObjectSystem]
    public class AnimatorComponentAwake : AwakeSystem<AnimatorComponent>
    {
        public override void Awake(AnimatorComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class AnimatorComponentStart : StartSystem<AnimatorComponent>
    {
        public override void Start(AnimatorComponent self)
        {
            self.Start();
        
        }
    }
    [ObjectSystem]
    public class AnimatorComponentUpdate : UpdateSystem<AnimatorComponent>
    {
        public override void Update(AnimatorComponent self)
        {
            if(self.unitEntity is MonsterEntity)
            self.UpdateIdleSound();
        }
    }
    /// <summary>
    /// 动画控制组件
    /// </summary>
    public partial class AnimatorComponent : Component
    {

        public Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();

        public HashSet<string> Parameter = new HashSet<string>();

        public List<Animator> subAnimators = new List<Animator>();//装备动画
        public MotionType motionType;

        public float montionSpeed;

        public bool isStop;
        public float stopSpeed;

        public Animator Animator;

        public UnitEntity unitEntity;
        float curSpeed;

        public void Awake()
        {
            unitEntity = this.GetParent<UnitEntity>();
            Animator animator = unitEntity.Game_Object.GetComponent<Animator>()??unitEntity.Game_Object.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                if (unitEntity is SummonEntity)
                {
                    animator = unitEntity.Game_Object.GetComponentInChildren<Animator>();
                }
                if(animator==null)
                return;
            }
            if (unitEntity is RoleEntity roleEntity)
            {
                AssetBundleComponent.Instance.LoadBundle($"Animator_{roleEntity.RoleType.GetRoleResName()}".StringToAB());
                animator.runtimeAnimatorController = (RuntimeAnimatorController)AssetBundleComponent.Instance.GetAsset($"Animator_{roleEntity.RoleType.GetRoleResName()}".StringToAB(), $"Animator_{roleEntity.RoleType.GetRoleResName()}");
               
            }
            else if (unitEntity is SummonEntity summon)
            {
               
                Init_Summon(summon);
            }
            else if (unitEntity is MonsterEntity monsterEntity)
            {
                Init_Monster(monsterEntity);
            }
           
            else if (unitEntity is PetEntity petEntity)
            {
                Init_Pet(petEntity);
            }
            if (animator.runtimeAnimatorController == null)
            {
                return;
            }
            if (animator.runtimeAnimatorController.animationClips == null) return;
            this.Animator = animator;

            foreach (AnimationClip animationClip in animator.runtimeAnimatorController.animationClips)
            {
                animationClips[animationClip.name] = animationClip;
                // Log.DebugGreen($"animationClip.name:{animationClip.name}");
            }
            foreach (AnimatorControllerParameter animatorControllerParameter in animator.parameters)
            {
                Parameter.Add(animatorControllerParameter.name);
                // Log.DebugGreen($"animatorControllerParameter.name:{animatorControllerParameter.name}");
            }

        }


        public void Start()
        {
            if (unitEntity is RoleEntity roleEntity)
            {
                if (SiegeWarfareData.SiegeWarfareIsStart && SiegeWarfareData.CurroleId == roleEntity.Id)
                {
                    SiegeWarfareData.currole = roleEntity;
                    SiegeWarfareData.currole.Game_Object = roleEntity.Game_Object;
                    //SetBoolValue("SiegeSitDown", true);
                    //  SiegeWarfareData.currole.Game_Object.transform.parent.eulerAngles = new Vector3(0, 90, 0);
                    //SiegeWarfareData.currole.roleTrs.eulerAngles = new Vector3(0, 90, 0);
                }
                else
                {
                    SetBoolValue(MotionType.IsMan.ToString(), roleEntity.RoleType.IsMan());
                    SetIntValue(MotionType.RoleIndex.ToString(), (int)roleEntity.RoleType);
                }
            }
        }
        /// <summary>
        /// 转换条件是否存在
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool HasParameter(string parameter)
        {
            try
            {
                return this.Parameter.Contains(parameter);
            }
            catch (Exception e )
            {

                return false;
            }
            
        }

        public void PlayInTime(MotionType motionType, float time)
        {
            AnimationClip animationClip;
            if (!animationClips.TryGetValue(motionType.ToString(), out animationClip))
            {
                Log.DebugRed($"找不到该动作：{motionType}");
            }
            float motionSpeed = animationClip.length / time;
            if (motionSpeed < 0.01f || motionSpeed > 1000f)
            {
                Log.DebugRed($"motionSpeed数值异常，{motionSpeed} 此动作跳过");
                return;
            }
            this.motionType = motionType;
            this.montionSpeed = motionSpeed;
        }
        public void Play(MotionType motionType, float motionSpeed = 1f)
        {
            if (!this.HasParameter(motionType.ToString()))
            {
                return;
            }
            this.motionType = motionType;
            this.montionSpeed = motionSpeed;
        }
        /// <summary>
        /// 获取动画时间
        /// </summary>
        /// <param name="motionType"></param>
        /// <returns></returns>
        public float AnimationTime(MotionType motionType)
        {
            AnimationClip animationClip;
            if (!this.animationClips.TryGetValue(motionType.ToString(), out animationClip))
            {
                throw new Exception($"找不到该动作: {motionType}");
            }
            return animationClip.length;
        }

        public void PauseAnimator()
        {
            if (this.isStop)
            {
                return;
            }
            this.isStop = true;

            if (this.Animator == null)
            {
                return;
            }
            this.stopSpeed = this.Animator.speed;
            this.Animator.speed = 0;
            for (int i = 0, length = subAnimators.Count; i < length; i++)
            {
                subAnimators[i].speed = 0;
            }
        }

        public void RunAnimator()
        {
            if (!this.isStop)
                return;
            this.isStop = false;
            if (this.Animator == null)
            {
                return;
            }
            this.Animator.speed = this.stopSpeed;
            for (int i = 0, length = subAnimators.Count; i < length; i++)
            {
                subAnimators[i].speed = this.stopSpeed;
            }
        }

        public void SetBoolValue(string name, bool state)
        {
            if (!this.HasParameter(name))
                return;
            //if (name == "IsMove" || name == "Attack" || name == "Hit")
            //{
            //    if (GetBoolParameterValue("SiegeSitDown"))
            //        this.Animator.SetBool("SiegeSitDown", false);
            //}
            this.Animator.SetBool(name, state);
            try
            {
                for (int i = 0, length = subAnimators.Count; i < length; i++)
                {
                    if (subAnimators[i] == null) continue;
                  //  if (IsCanSet(subAnimators[i], name))
                        subAnimators[i].SetBool(name, state);
                }
            }
            catch (Exception e)
            {

                Log.DebugGreen(e.ToString());
            }
        }

        public void SetFloatValue(string name, float value)
        {
            if (!this.HasParameter(name))
                return;
            this.Animator.SetFloat(name, value);
            for (int i = 0, length = subAnimators.Count; i < length; i++)
            {
                if (subAnimators[i] == null) continue;
                //if (IsCanSet(subAnimators[i], name))
                    subAnimators[i].SetFloat(name, value);
            }
        }

        public void SetIntValue(string name, int value)
        {
            if (!this.HasParameter(name))
                return;
            this.Animator.SetInteger(name, value);
            for (int i = 0, length = subAnimators.Count; i < length; i++)
            {
                if (subAnimators[i] == null) continue;
              //  if (IsCanSet(subAnimators[i], name))
                    subAnimators[i].SetInteger(name, value);
            }
        }

        public void SetTrigger(string name)
        {
            if (!this.HasParameter(name))
                return;
            this.Animator.SetTrigger(name);
            for (int i = 0, length = subAnimators.Count; i < length; i++)
            {
                if (subAnimators[i] == null) continue;
                //if (IsCanSet(subAnimators[i], name))
                    subAnimators[i].SetTrigger(name);
            }
        }
        public void SetAnimtorSpeed(float speed)
        {

            this.stopSpeed = this.Animator.speed;
            this.Animator.speed = speed;
            for (int i = 0, length=subAnimators.Count; i < length; i++)
            {
                if (subAnimators[i] == null) continue;
                subAnimators[i].speed = speed;
            }
          
        }
        /// <summary>
        /// 设置动画攻击速度
        /// </summary>
        /// <param name="speed"></param>
        public void SetAttackSpeed(float speed)
        {
            if (curSpeed == speed) return;
            if (speed <= 0)
            {
                speed = 1;
            }
            float changeSpeed = speed - speed / 10;
            this.Animator.SetFloat("AttackSpeed", changeSpeed);
            //if (speed <= 0)
            //{
            //    speed = 2;
            //}
            //Log.DebugGreen($"攻击速度 {speed} 修改后的 {changeSpeed}");
            //if(speed >= 2)
            //    this.Animator.SetFloat("AttackSpeed", speed / 2);
            //else
            //    this.Animator.SetFloat("AttackSpeed", speed);
            for (int i = 0, length = subAnimators.Count; i < length; i++)
            {
                if (subAnimators[i] == null) continue;
                subAnimators[i].SetFloat("AttackSpeed", changeSpeed);
            }
        }
        public void ResetAnimatorSpeed()
        {
            this.Animator.speed = this.stopSpeed;
            for (int i = 0, length = subAnimators.Count; i < length; i++)
            {
                if (subAnimators[i] == null) continue;
                subAnimators[i].speed = this.stopSpeed;
            }
        }

        public bool GetBoolParameterValue(string name)
        {
            if (!this.HasParameter(name))
                return false;
            else
                return Animator.GetBool(name);
        }

        public void AddSubAnimator(Animator anim)
        {
            if (anim == null || this.subAnimators.Contains(anim)) return;
            this.subAnimators.Add(anim);

        }
        public void RemoveSubAnimator(Animator anim)
        {
            if (anim == null) return;
            if (this.subAnimators.Contains(anim))
                this.subAnimators.Remove(anim);
        }
        public bool IsCanSet(Animator animator, string value)
        {
            foreach (AnimatorControllerParameter animatorControllerParameter in animator.parameters)
            {
                if (value == animatorControllerParameter.name)
                    return true;
            }
            return false;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            if (unitEntity is RoleEntity role)
            {
                AssetBundleComponent.Instance.UnloadBundle($"Animator_{role.RoleType.GetRoleResName()}".StringToAB());
            }
            this.animationClips.Clear();
            this.Parameter.Clear();
            this.Animator = null;
            this.subAnimators.Clear();
            this.unitEntity = null;
            if(Proxy!=null)
            Proxy.Effect_Action_Str = null;

            
        }

    }
}
