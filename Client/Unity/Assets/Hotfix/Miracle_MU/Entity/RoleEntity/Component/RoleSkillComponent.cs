using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{

    [ObjectSystem]
    public class RoleSkillComponentAwake : AwakeSystem<RoleSkillComponent>
    {
        public override void Awake(RoleSkillComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class RoleSkillComponentStart : StartSystem<RoleSkillComponent>
    {
        public override void Start(RoleSkillComponent self)
        {
            self.equipmentComponent = self.roleEntity.GetComponent<RoleEquipmentComponent>();
        }
    }
    /// <summary>
    /// 玩家技能组件
    /// </summary>
    public class RoleSkillComponent : Component
    {

        public AnimatorComponent animatorComponent;
        public RoleEntity roleEntity;
        public AnimationEventProxy AnimationEvent;
        SkillInfos Skill_Info;
        public UnitEntity hitunitEntity;//被击实体
        public RoleEquipmentComponent equipmentComponent;
        public  GameSetInfo gameSetInfo;
        
        int attackState = 0;
        public void Awake() 
        {
            roleEntity = GetParent<RoleEntity>();
            animatorComponent = roleEntity.GetComponent<AnimatorComponent>();
            AnimationEvent = roleEntity.Game_Object.GetComponent<AnimationEventProxy>();

            Skill_Info = new SkillInfos();
            if (roleEntity==UnitEntityComponent.Instance.LocalRole)//本地玩家 添加走路音效
            AnimationEvent.Sound_action = () => { SoundComponent.Instance.PlaySkill("zoulu"); };//走路音效
            //添加特效 回调函数
            AnimationEvent.Effect_action = SkillEffect;
            AnimationEvent.Effect_Action_Str = NormalAttackEffect;
           
            gameSetInfo = LocalDataJsonComponent.Instance.gameSetInfo;
        }
        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skillConfig">技能配置表ID</param>
        /// <param name="hitunitUUID">被击实体的UUID</param>
        public void UseSkill(long skillConfig, long hitunitUUID = 0) 
        {
            hitunitEntity = null;
            hitunitEntity = UnitEntityComponent.Instance.Get<UnitEntity>(hitunitUUID);
            
            if (hitunitEntity != null&& hitunitEntity.Game_Object!=null)
            {
                Animator animator = hitunitEntity.Game_Object.GetComponent<Animator>();
                if(animator != null && roleEntity.IsDead == false)
                {
                    var pos = hitunitEntity.Game_Object.transform.position;
                    pos.y = this.roleEntity.Game_Object.transform.parent.position.y;
                   // this.roleEntity.Game_Object.transform.parent.LookAt(pos);
                    this.roleEntity.GetComponent<TurnComponent>().Turn(pos);
                   // this.roleEntity.roleTrs.LookAt(pos);
                }
            }
            if (skillConfig is 0)//普攻
            {
                //普攻
                PlayAttackSkillAnimation();
                PlaySkillAudio(roleEntity.RoleType!=E_RoleType.Archer?"pugong": "gongjianshoupugong");
            }
            else
            {
                skillConfig.ToInt32().GetSkillInfos__Ref(ref Skill_Info);
                if (Skill_Info == null) return;
                PlaySkillAnimation();
                if(Skill_Info.SoundName.Length==0) return;
                PlaySkillAudio(Skill_Info.SoundName);
            }
            //if(roleEntity.Id == RoleArchiveInfoManager.Instance.LoadRoleUUID)
            //    UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>().EnterSafeArea(false);
            //if(roleEntity.Id == RoleArchiveInfoManager.Instance.LoadRoleUUID)


            if (SceneComponent.Instance.CurrentSceneIndex == (int)SceneName.YaTeLanDiSi)
            {
                roleEntity.GetComponent<RoleEquipmentComponent>().EnterSafeArea(false);
                AnimatorCondition.IsSwing = false;
            }
        }



        /// <summary>
        /// 播放技能动画
        /// </summary>
        public void PlaySkillAnimation()
        {
            if (animatorComponent == null)
            {
                animatorComponent = roleEntity.GetComponent<AnimatorComponent>();
            }
            if (string.IsNullOrEmpty(Skill_Info.AnimatorTriggerIndex)) return;
            animatorComponent.SetTrigger(Skill_Info.AnimatorTriggerIndex);
            animatorComponent.SetBoolValue(MotionType.IsSwim, false);
        }
        /// <summary>
        /// 播放普攻动画
        /// </summary>
        /// <param name="skillConfig"></param>
        /// <param name="hitunitUUID"></param>
        public void PlayAttackSkillAnimation(int hitunitUUID = 0)
        {
            if (animatorComponent == null)
            {
                animatorComponent = roleEntity.GetComponent<AnimatorComponent>();
            }
            
            animatorComponent.SetTrigger(MotionType.Attack);
            animatorComponent.SetBoolValue(MotionType.IsSwim, false);
            animatorComponent.SetBoolValue(MotionType.IsMove, false);

            if (roleEntity.RoleType == E_RoleType.Archer) return;

            if (animatorComponent.Animator.GetBool(MotionType.IsLeftWeapon))
            {
                if (attackState > 1)
                {
                    attackState = 0;
                }
             
                animatorComponent.SetTrigger($"Attack_{attackState++}");
                if (attackState > 1)
                {
                    attackState = 0;
                }
            }
           else if (animatorComponent.Animator.GetBool(MotionType.IsDoubleWeapon))
            {
            
                animatorComponent.SetTrigger($"Attack_{attackState++}");
                if (attackState > 2)
                {
                    attackState = 0;
                }
            }

        }
        /// <summary>
        /// 弓箭手 普攻特效
        /// </summary>
        /// <param name="effectResName"></param>
        private void NormalAttackEffect(string effectResName)
        {
            GameObject effect = ResourcesComponent.Instance.LoadEffectObject(effectResName.StringToAB(), effectResName);
            if (effect == null || roleEntity == null) return;
            effect.transform.localPosition = roleEntity.Position;
            effect.transform.localRotation = roleEntity.Game_Object.transform.rotation;
        }
        /// <summary>
        /// 技能特效
        /// </summary>
        private void SkillEffect()
        {
            if ((gameSetInfo.CloseEffect||GlobalDataManager.IsHideRole==false) && roleEntity != UnitEntityComponent.Instance.LocalRole)
            {
                //关闭其他玩家的特效
                return;
            }

           // Log.DebugGreen($"技能类型->{Skill_Info.skillType}");
            //if (Skill_Info.skillType == 2) return;//辅助技能
          
            //Log.DebugBrown($"技能：{Skill_Info.Name} 特效：{Skill_Info.AttackEffect}");
            if (string.IsNullOrEmpty(Skill_Info.AttackEffect))
            {
                //牙突刺(yatuci (1))、升龙击(Effect_ShengLongJi)、旋风斩(xuanfengzhan (1))、天地十字剑()、
              
                    if (equipmentComponent.roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Weapon, out GameObject weapon))
                    {
                        switch (Skill_Info.Name)
                        {
                            case "牙突刺":
                                if (weapon.transform.Find("World/yatuci") is Transform yatuci)
                                {
                                    if (yatuci.GetComponent<WaitTimeHide>() is null)
                                    {
                                        
                                        yatuci.gameObject.AddComponent<WaitTimeHide>().time = .5f;
                                    }
                                    yatuci.gameObject.SetActive(true);
                                }
                                break;
                            case "升龙击":
                                if (weapon.transform.Find("World/Effect_ShengLongJi") is Transform shenglongji)
                                {
                                    if (shenglongji.GetComponent<WaitTimeHide>() is null)
                                    {
                                        shenglongji.gameObject.AddComponent<WaitTimeHide>().time = .5f;
                                    }
                                    shenglongji.gameObject.SetActive(true);
                                }
                                break;
                            case "旋风斩":
                                if (weapon.transform.Find("World/xuanfengzhan") is Transform xuanfengzhan)
                                {
                                    if (xuanfengzhan.GetComponent<WaitTimeHide>() is null)
                                    {
                                        xuanfengzhan.gameObject.AddComponent<WaitTimeHide>().time = .8f;
                                    }
                                    xuanfengzhan.gameObject.SetActive(true);
                                }
                                break;
                            case "天地十字剑":
                                if (weapon.transform.Find("World/tiandishizijian") is Transform tiandishizijian)
                                {
                                    if (tiandishizijian.GetComponent<WaitTimeHide>() is null)
                                    {
                                        tiandishizijian.gameObject.AddComponent<WaitTimeHide>().time = .5f;
                                    }
                                    tiandishizijian.gameObject.SetActive(true);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                return;
            }
            string effectResName = Skill_Info.AttackEffect;
            //霹雳回旋斩(特殊处理)
            if (Skill_Info.Name.Contains("霹雳回旋斩"))
            {
                equipmentComponent ??= roleEntity.GetComponent<RoleEquipmentComponent>();
                if (equipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Weapon))//装备了武器
                {
                    effectResName=$"Effect_PiLiHuiXuan_{equipmentComponent.roleCurWare_EquipDic[E_Grid_Type.Weapon].name.Split('_')[1]}";
                }
            }
            //加载技能特效

            GameObject effect=null;
            if (roleEntity.RoleType == E_RoleType.Archer)
            {
                if (equipmentComponent.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Weapon, out KnapsackDataItem dataItem)&& dataItem.ItemType == (int)E_ItemType.Crossbows && this.Skill_Info.Name.Contains("多重箭"))
                {
                    effect = ResourcesComponent.Instance.LoadEffectObject("Effect_GongJian_Attack_3_Nu".StringToAB(), "Effect_GongJian_Attack_3_Nu");
                }
                else
                {
                    effect = ResourcesComponent.Instance.LoadEffectObject(effectResName.StringToAB(), effectResName);
                }
            }
            else
            {
                effect = ResourcesComponent.Instance.LoadEffectObject(effectResName.StringToAB(), effectResName);
            }
            if (effect == null)
            {
              //  Log.DebugRed($"特效为空：{effectResName}");
                return;
            }
            else
            {
                if (effect.GetComponent<ResourcesRecycle>() is null)
                {
                    effect.AddComponent<ResourcesRecycle>();
                }
            }
            //技能特效是否 带LineRenderer组件
            if (!effect.TryGetComponent<LineRenderer>(out var lineRenderer))
            {
                //特效位置就 在玩家原地
                effect.transform.localPosition = roleEntity.Position;
                effect.transform.localRotation = roleEntity.Game_Object.transform.rotation;
             //   Log.DebugGreen($"roleEntity.Position:{roleEntity.Position}");

            }
            else
            {
                //Log.DebugGreen($"SkillEffect：{hitunitEntity == null}");
                if (hitunitEntity != null&& hitunitEntity.Game_Object!=null)
                {
                  
                    //roleEntity.Game_Object.transform.LookAt(hitunitEntity.Position);
                    effect.transform.localRotation = roleEntity.Game_Object.transform.rotation;
                    lineRenderer.SetPosition(0, roleEntity.Position + Vector3.up * 4.5f);//起始点
                    lineRenderer.SetPosition(1, hitunitEntity.Position + Vector3.up * 2f);//目标点 为被击实体
                }
                else
                {
                    effect.transform.localRotation=roleEntity.Game_Object.transform.rotation;
                    lineRenderer.SetPosition(0, roleEntity.Position + Vector3.up * 4.5f);//起始点
                    lineRenderer.SetPosition(1, roleEntity.Position + roleEntity.Game_Object.transform.forward*Skill_Info.Distance + Vector3.up * 2f);//目标点为 玩家正前方
                }
            }

        }

        /// <summary>
        /// 播放技能音效
        /// </summary>
        /// <param name="audioName">技能音效名</param>
        public void PlaySkillAudio(string audioName)
        {
            if (string.IsNullOrEmpty(audioName)) return;
            SoundComponent.Instance.PlaySkill(audioName);
        }


        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            hitunitEntity = null;
            equipmentComponent = null;
            animatorComponent = null;
            gameSetInfo = null;
            if (AnimationEvent.Effect_action != null)
            {
                AnimationEvent.Effect_action = null;
            }
            if (AnimationEvent.Sound_action != null)
            {
                AnimationEvent.Sound_action = null;
            }
            AnimationEvent = null;
          
        }

    }
}