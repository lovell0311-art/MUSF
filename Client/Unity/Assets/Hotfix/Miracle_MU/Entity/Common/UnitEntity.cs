using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Threading;

namespace ETHotfix
{

    /// <summary>
    /// 公共实体
    /// </summary
    [HideInInspector]//在层级面板中隐藏
    public class UnitEntity : Entity
    {
        /// <summary>
        /// 延时销毁Obj的时间
        /// 死亡动画播方结束后 隐藏放入对象池
        /// </summary>
        public int DelayTime = 0;

        /// <summary>
        /// 实体 对应的模型Obj
        /// </summary>
        public GameObject Game_Object;
        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead = false;

        //实体所拥有的技能
        public List<int> OwnSkills = new List<int>();
        /// <summary>
        /// 实体属性
        /// </summary>
        public UnitEnityProperty Property { get; private set; } = new UnitEnityProperty();

        /// <summary>
        /// 是否减速
        /// </summary>
        public bool IsSlowDown = false;

        /// <summary>
        /// 实体当前做格子坐标
        /// </summary>
        public AstarNode CurrentNodePos
        {
            get
            {
                if (currentNodePos != null)
                {
                    return currentNodePos;
                }
                //return AstarComponent.Instance.GetNode(currentNodePos.x, currentNodePos.z); 
                else
                    return null;
            }
            set
            {
                currentNodePos = value;
                lasttNodePos = value;
            }
        }

        private AstarNode currentNodePos;

        public AstarNode lasttNodePos;

        public Vector3 CurrentPos => AstarComponent.Instance.GetVectory3(currentNodePos);

      
        /// <summary>
        /// 位置
        /// </summary>
        public virtual Vector3 Position
        {
            get
            {
                if (this.Game_Object == null)
                {
                   
                    return CurrentPos;
                }
                else
                {
                    return Game_Object.transform.position;
                }
            }
            set
            {

                if (this.Game_Object != null)
                    Game_Object.transform.position = value;
            }
        }
        /// <summary>
        /// 旋转
        /// </summary>
        public virtual Quaternion Rotation
        {
            get
            {
                return Game_Object.transform.rotation;
            }
            set
            {
                if (Game_Object != null)
                    Game_Object.transform.rotation = value;
            }
        }



        /// <summary>
        /// 被击方法
        /// 该技能有被击特效的话 显示被击特效
        /// </summary>
        /// <param name="attacker">使用技能 的实体</param>
        /// <param name="hitSkillConfigId">技能的configID</param>
        public virtual void Hit(UnitEntity attacker, long hitSkillConfigId)
        {
            //Log.DebugGreen($"被技能：{hitSkillConfigId} 击中  attacker==null {attacker == null}");

            //受击对象朝向攻击目标
            //this.Game_Object.transform.LookAt(attacker.Position);
            //显示被击 特效 音效
            if (this is MonsterEntity monsterEntity && !monsterEntity.IsDead)
            {
                monsterEntity.GetComponent<AnimatorComponent>()?.HitEffect();
            }

            if (attacker is PetEntity pet)//宠物
            {
                if ((GlobalDataManager.IsHideRole == false) && pet.Id!= PetArchiveInfoManager.Instance.petId)
                {
                    //关闭其他玩家的特效
                    return;
                }

                GameObject Oet_hitEffect = null;
                if (hitSkillConfigId == 0)//普攻 并且被击特效不为空
                {
                    Oet_hitEffect = ResourcesComponent.Instance.LoadEffectObject(pet.NorMalAttackEffectResName.StringToAB(), pet.NorMalAttackEffectResName);
                }
                else
                {
                    //技能 被击特效
                    if (ConfigComponent.Instance.GetItem<Pets_SkillConfig>((int)hitSkillConfigId) is Pets_SkillConfig pets_Skill && pets_Skill != null)
                    {
                        Oet_hitEffect = ResourcesComponent.Instance.LoadEffectObject(pets_Skill.HitEffect.StringToAB(), pets_Skill.HitEffect);
                    }
                }
                if (Oet_hitEffect == null) return;
                if (Oet_hitEffect.GetComponent<ResourcesRecycle>() is null)
                {
                    Oet_hitEffect.AddComponent<ResourcesRecycle>();
                }
                //被击特效的位置 为当前实体的位置
                Oet_hitEffect.transform.localPosition = this.Game_Object.transform.position + Vector3.up;
                Oet_hitEffect.transform.localRotation = this.Game_Object.transform.rotation;
                return;
            }


            ((int)hitSkillConfigId).GetSkillInfos__Out(out SkillInfos skillInfos);

            if ((GlobalDataManager.IsHideRole == false) && attacker.Id != UnitEntityComponent.Instance.LocaRoleUUID)
            {
                //关闭其他玩家的特效
                return;
            }

            if (skillInfos == null || string.IsNullOrEmpty(skillInfos.HitEffect)) return;//配置表信息不存在 或者 被击打特效为空 直接返回

            
            GameObject hitEffect = ResourcesComponent.Instance.LoadEffectObject(skillInfos.HitEffect.StringToAB(), skillInfos.HitEffect);

            if (hitEffect.GetComponent<ResourcesRecycle>() is null)
            {
                hitEffect.AddComponent<ResourcesRecycle>();
            }
            //被击特效的位置 为当前实体的位置
            hitEffect.transform.localPosition = this.Game_Object.transform.position + Vector3.up;
            hitEffect.transform.localRotation = this.Game_Object.transform.rotation;

        }
        /// <summary>
        /// 实体死亡
        /// </summary>
        public virtual void Dead()
        {

            IsDead = true;

        }



        /// <summary>
        /// 释放掉
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
             base.Dispose();


            if (this.Game_Object != null)
            {

                if (this is RoleEntity roleEntity)
                {
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(roleEntity.Game_Object.transform.parent.gameObject, roleEntity.Game_Object.transform.parent.gameObject.name.StringToAB());
                   // ResourcesComponent.Instance.RecycleGameObject(roleEntity.Game_Object.transform.parent.gameObject);
                }
                else if (this is PetEntity)
                {
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(this.Game_Object, this.Game_Object.name.StringToAB());
                    //ResourcesComponent.Instance.RecycleGameObject(this.Game_Object);
                }
                else if (this is KnapsackItemEntity knapsackItemEntity)
                {
                    if (knapsackItemEntity.item_Info.Id == 320294||knapsackItemEntity.item_Info.Id== 320316 || KnapsackItemsManager.MedicineHpIdList.Contains(knapsackItemEntity.item_Info.Id)|| KnapsackItemsManager.MedicineMpIdList.Contains(knapsackItemEntity.item_Info.Id))
                    {
                        //金币、血瓶、蓝瓶回收
                        ResourcesComponent.Instance.RecycleGameObject(this.Game_Object);
                    }
                    else
                    {
                        ResourcesComponent.Instance.DestoryGameObjectImmediate(this.Game_Object, this.Game_Object.name.StringToAB());
                        //ResourcesComponent.Instance.RecycleGameObject(this.Game_Object);
                    }
                    
                }
                else
                {
                    ResourcesComponent.Instance.RecycleGameObject(this.Game_Object, DelayTime);
                }
                this.Game_Object = null;

            }
         
         //   base.Dispose();
        }
    }
}