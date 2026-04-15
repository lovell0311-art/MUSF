using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 技能模块
    /// </summary>
    public partial class UIMainComponent
    {

        private Transform Skills;//技能
        Skillconfiguration skillconfiguration;

        private bool IsInitSkillSlot;

        public Dictionary<int, UISkillBtnEntity> AllskillBtnsDic;// 卡槽技能字典 key:卡槽id value :技能实体

        public Dictionary<int,int> DaShiSkillAtr=new Dictionary<int, int>();//大师技能 修改了技能攻击范围

        public UISkillBtnEntity attackEntity;
        public Transform attack;
       
        /// <summary>
        /// 是否可以使用技能
        /// </summary>
        public bool IsCanUse = true;

        public UISkillBtnEntity curSkillEntity;

        public RoleMoveControlComponent controlComponent;
        public int ExTime = 0;
        /// <summary>
        /// 挂机时 不能使用的技能
        /// </summary>
        public int OnExTime()
        {
            if (ExTime != 0)
            {
                return ExTime;
            }
            return 0;
        }
        /// <summary>
        /// 挂机时 不能使用的技能
        /// </summary>
        public List<string> onhookNoUseSkillList = new List<string>()
        {
            "瞬间移动",
        };
        public void Init_Skill()
        {
          
            Skills = ReferenceCollector_Main.GetGameObject("Skills").transform;//技能卡槽
            ReferenceCollector collector = Skills.GetReferenceCollector();
            attack = collector.GetButton("AttackBtn").transform;
            IsInitSkillSlot = true;

            SoundComponent.Instance.LoadSkillAudioRefrence("Audio_Skill".StringToAB(), "Audio_Skill").Coroutine();//加载技能音效

            AllskillBtnsDic = new Dictionary<int, UISkillBtnEntity>();
            InitNormalAttack();
            LoadSkills();

            controlComponent = roleEntity.GetComponent<RoleMoveControlComponent>();

            CountDownAction += () => 
            {
                if (RoleOnHookComponent.Instance.IsOnHooking==false&& curSkillEntity!=null&& controlComponent.IsNavigation)
                {
                    if (curSkillEntity.curAttackEntity != null && UnitEntityComponent.Instance.CurAttackEntityIsDieOrLeveView(curSkillEntity.curAttackEntity) == false)
                    {
                        //当前目标已经被其他玩家 击杀
                        curSkillEntity.curAttackEntity = null;
                    }
                  
                    if (curSkillEntity.curAttackEntity != null && PositionHelper.Distance(curSkillEntity.curAttackEntity.CurrentNodePos, curSkillEntity.RoleEntity.CurrentNodePos) <= curSkillEntity.SkillAttackRange)
                    {
                       controlComponent.UnitEntityPathComponent.StopMove();
                        
                        // self.RequestAttackUnitEntity(self.curAttackEntity).Coroutine();
                        //curSkillEntity.RequestAttackUnitEntity().Coroutine();
                        curSkillEntity.RequestAttackUnit(curSkillEntity.curAttackEntity, ref UIMainComponent.Instance.IsCanUse);
                    }

                   
                }
            
                if (IsCanUse) return;
                if (TimeHelper.Now() > GlobalDataManager.AttackSpaceTime)
                {
                   
                    IsCanUse = true;
                }
            };

            //PK 反击

            CountDownAction += () => 
            {
                if (GlobalDataManager.IsBeatBack)
                {
                    if (Time.time > GlobalDataManager.BeatBackTimer)
                    {
                        GlobalDataManager.IsBeatBack=false;
                    }
                }
            };
            collector.GetButton("monster").onClick.AddSingleListener(() => 
            {
               var HookEffect= ResourcesComponent.Instance.LoadEffectObject("Effect_OnHookBackpos".StringToAB(), "Effect_OnHookBackpos");
                HookEffect.transform.position = roleEntity.Position + Vector3.up * .5F;
                HookEffect.SetActive(true);
                UnitEntityComponent.Instance.GetNearMonster(); 
            });
            collector.GetButton("role").onClick.AddSingleListener(() =>
            {
                var HookEffect = ResourcesComponent.Instance.LoadEffectObject("Effect_OnHookBackpos".StringToAB(), "Effect_OnHookBackpos");
                HookEffect.transform.position = roleEntity.Position + Vector3.up * .5F;
                HookEffect.SetActive(true);
                UnitEntityComponent.Instance.GetNearRole();
            });

            DaShiSkillAtr.Clear();
        }

        /// <summary>
        /// 初始化普攻
        /// </summary>
        public void InitNormalAttack() 
        {
          attackEntity= ComponentFactory.Create<UISkillBtnEntity, Transform>(attack);
        }
        /// <summary>
        /// 加载技能
        /// </summary>
        public void LoadSkills()
        {
            skillconfiguration = LocalDataJsonComponent.Instance.LoadData<Skillconfiguration>(roleEntity.LocalSkillFillName) ?? new Skillconfiguration();
            ClearskillBtnsDic(skillconfiguration);
       
            for (int i = 0; i < 7; i++)
            {
                Transform skill = Skills.Find($"Skill_{i}");
                int btnIndex = i;
                int configId;
                UISkillBtnEntity btnEntity;
                var skillkey = $"{btnIndex}";
              //  skill.gameObject.SetActive(false);
                if (IsInitSkillSlot)//初始化技能卡槽
                {
                    
                    if (skillconfiguration.SKilDic.ContainsKey(skillkey))//技能卡槽配置 中包含 该卡槽
                    {
                        configId = skillconfiguration.SKilDic[skillkey];
                        btnEntity = ComponentFactory.Create<UISkillBtnEntity, int, Transform>(configId, skill);
                        btnEntity.slotId = btnIndex;
                        AllskillBtnsDic[btnIndex] = btnEntity;
                    //    skill.gameObject.SetActive(true);
                    }
                    else
                    {
                        //skill.GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.skillIconAtlasName, "+");
                    //    skill.gameObject.SetActive(false);
                    }
                }
                else
                {
                    //卡槽发生变动
                    if (skillconfiguration.SKilDic.ContainsKey(skillkey))
                    {
                        //该卡槽 有技能
                        if (AllskillBtnsDic.ContainsKey(i))
                        {
                          //  skill.gameObject.SetActive(true);
                            //判断该卡槽的技能是否发生改变、
                            if (AllskillBtnsDic[i].skill_ConfigID != skillconfiguration.SKilDic[skillkey])
                            {
                                configId = skillconfiguration.SKilDic[skillkey];
                                btnEntity = ComponentFactory.Create<UISkillBtnEntity, int, Transform>(configId, skill);
                                btnEntity.slotId = btnIndex;
                                AllskillBtnsDic[btnIndex] = btnEntity;
                            }
                        }
                        else
                        {
                          //  skill.gameObject.SetActive(false);
                            //卡槽新加的技能
                            configId = skillconfiguration.SKilDic[skillkey];
                            btnEntity = ComponentFactory.Create<UISkillBtnEntity, int, Transform>(configId, skill);
                            btnEntity.slotId = btnIndex;
                            AllskillBtnsDic[btnIndex] = btnEntity;
                        }

                    }

                }
            }
            IsInitSkillSlot = false;
        
        }
        private void ClearskillBtnsDic(Skillconfiguration skillconfiguration)
        {
            var dic = skillconfiguration.SKilDic;
            var removelist = new List<int>();//需要移除的技能集合
          
            for (int i = 0,length= AllskillBtnsDic.Count; i < length; i++)
            {
                int slot = AllskillBtnsDic.ElementAt(i).Key;//卡槽索引
                if (!dic.ContainsKey(slot.ToString()))//本地缓存技能卡槽json中 不包含 slot卡槽 则该卡槽的技能已经卸载了
                {
                    AllskillBtnsDic.ElementAt(i).Value.Dispose();
                    removelist.Add(slot);
                    
                }
                //卡槽的技能发生变动时 
                else  
                {
                    if (AllskillBtnsDic.ElementAt(i).Value.skill_ConfigID != dic[slot.ToString()])
                    {
                        AllskillBtnsDic.ElementAt(i).Value.Dispose();
                        removelist.Add(slot);
                    }
                }
            }
            //清除技能
            
            foreach (var item in removelist)
            {
               
                AllskillBtnsDic.Remove(item);
            }

        }
        /// <summary>
        /// 更改Buffer冷却时间
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="time">秒</param>
        public void ChangeBufferCoolTime(int skillId,float time)
        {
            Log.DebugBrown("buff" + skillId + "时间" + time);
            if (skillId == 47925)
            {
                ExTime = (int)time;
            }
            if (AllskillBtnsDic == null) return;
            foreach (var skill in AllskillBtnsDic.Values)
            {
                if (skill.skill_ConfigID == skillId)
                {
                    skill.nextUseTime = Time.time + time*.8f;
                  
                }
            }
        }

        /// <summary>
        /// 修改技能的攻击范围（大师使用）
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="range"></param>
        public void ChangeSkllAttackRange(int skillId,int range) 
        {
            DaShiSkillAtr[skillId] = range;//缓存

            foreach (var skill in AllskillBtnsDic.Values)
            {
                if (skill.skill_ConfigID == skillId)
                {
                    skill.SkillAttackRange = range;
                }
            }
        }
      
        
        public void SkillDispos() 
        {
            /*ClearskillBtnsDic(new Skillconfiguration());*/
            for (int i = 0, length = AllskillBtnsDic.Count; i < length; i++)
            {
                AllskillBtnsDic.ElementAt(i).Value.Dispose();
            }
            AllskillBtnsDic.Clear();
            skillconfiguration = new Skillconfiguration();

                attackEntity.Dispose();
        }

       

    }
}
