using ETModel;
using UnityEngine.Profiling;

namespace ETHotfix
{
   
    /// <summary>
    /// 实体伤害通知
    /// </summary>
    [MessageHandler]
    public class G2C_AttackStart_notice_Handler : AMHandler<G2C_AttackStart_notice>
    {
        protected override void Run(ETModel.Session session, G2C_AttackStart_notice message)
        {

           // Log.DebugBrown("攻击返回通知===>G2C_AttackStart_notice" + JsonHelper.ToJson(message));
            UnitEntity unitEntity = UnitEntityComponent.Instance.Get<UnitEntity>(message.AttackSource);
            if (unitEntity == null) return;
            //怪物使用技能 攻击玩家
            if (unitEntity is MonsterEntity monster)
            {
                if (monster.IsDead) return;
               
                switch (monster.MonsterType)
                {
                    case 1:
                        //是boss 显示特效（BOSS技能 1玩家坐标点释放 2怪物坐标点释放） 
                        // message.AttackType 怪物技能ID  message.AttackTarget 技能目标点
                        monster.GetComponent<AnimatorComponent>()?.BossUseSkill(message.AttackTarget, message.AttackType, message.TimeTick, message.Pos);
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    default:
                       //  Log.DebugBrown($"怪物攻击玩家：怪物技能ID:{(int)message.AttackType}");
                       
                        monster.GetComponent<AnimatorComponent>()?.UseSkill(message.AttackTarget, (int)message.AttackType);
                      
                        
                        //Log.DebugGreen($"怪物攻速：{message.Ticks}  毫秒");
                        break;
                }
            }
            //玩家 攻击怪物
            else if (unitEntity is RoleEntity roleEntity)
            {
                //设置玩家动画的速度
                //60000 / (50 + (240 - 50) * Math.Clamp(b_CombatSource.GetNumerialFunc(E_GameProperty.AttackSpeed), 0, 280) / 280)=》  Min:25 Max:12001  1050

                //roleEntity.GetComponent<AnimatorComponent>().SetAttackSpeed(((float)1000 / (message.Ticks - (TimeHelper.Now() + GlobalDataManager.ServerTime))));

                roleEntity.GetComponent<AnimatorComponent>()?.SetAttackSpeed(((float)1000 / (message.Ticks - TimeHelper.Now())));

                if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    if (RoleOnHookComponent.Instance.IsOnHooking) RoleOnHookComponent.Instance.IsAttack = false;
                    else if (UIMainComponent.Instance.IsCanUse) UIMainComponent.Instance.IsCanUse = false;

                   // GlobalDataManager.AttackSpaceTime = (message.Ticks - (TimeHelper.Now() + GlobalDataManager.ServerTime)) + TimeHelper.Now();

                    GlobalDataManager.AttackSpaceTime = message.Ticks;
                
                }
              
                roleEntity.GetComponent<RoleSkillComponent>()?.UseSkill(message.AttackType, message.AttackTarget);


                if (message.AttackType != 0 && roleEntity == UnitEntityComponent.Instance.LocalRole)//本地玩家 更新MP
                {
                    roleEntity.Property.ChangeProperValue(E_GameProperty.PROP_MP, message.MpValue);
                    roleEntity.Property.ChangeProperValue(E_GameProperty.PROP_AG, message.AG);

                    UIMainComponent.Instance.ChangeRoleMp(message.MpValue);
                    UIMainComponent.Instance.ChangeAG();
                }
                // Log.DebugGreen($"显示被击：{message.AttackTarget != UnitEntityComponent.Instance.LocaRoleUUID}");
                UnitEntity unitEntityMonster = UnitEntityComponent.Instance.Get<UnitEntity>(message.AttackTarget);
                //被击对象 显示被击特效
                if (message.AttackTarget != UnitEntityComponent.Instance.LocaRoleUUID && unitEntityMonster != null && !unitEntityMonster.IsDead) //被击对象 不是玩家自己 才显示被击特效
                {
                    UnitEntityComponent.Instance.Get<UnitEntity>(message.AttackTarget).Hit(roleEntity, message.AttackType);
                }
            }
            //宠物攻击怪物
            else if (unitEntity is PetEntity petEntity)
             {
                // Log.DebugGreen($"宠物攻击怪物 :{(int)message.AttackType} message.AttackTarget:{message.AttackTarget}");
                 if (UnitEntityComponent.Instance.Get<UnitEntity>(message.AttackTarget) == null || !petEntity.Game_Object.activeSelf) return;
                 petEntity.SetHitTarget(message.AttackTarget, (int)message.AttackType);
                 petEntity.MasterHit();

                  // Game.EventManager.BroadcastEvent(EventTypeId.Hit);
                 Game.EventCenter.EventTrigger(EventTypeId.Hit);

                 petEntity.GetComponent<AnimatorComponent>().UsePetSkill((int)message.AttackType, message.AttackTarget);
                if (petEntity.RoleId == RoleArchiveInfoManager.Instance.LoadRoleUUID && (int)message.AttackType != 0)
                {
                    // 更新蓝量.
                    if (UIMainComponent.Instance != null)
                    {
                        UIMainComponent.Instance.SetPetMpValue(message.MpValue, petEntity.MaxMp);
                    }

                    if (UIComponent.Instance.Get(UIType.UIPet) != null)
                    {
                        UIComponent.Instance.Get(UIType.UIPet).GetComponent<UIPetComponent>().RealTimeUpdatePetMp(message.AttackSource, message.MpValue);
                    }
                }
            }
             //召唤兽攻击怪物或者玩家
             else
            if (unitEntity is SummonEntity summonEntity)
            {
                Log.DebugBrown("召唤兽 攻击怪物或玩家");
                summonEntity.GetComponent<AnimatorComponent>().UseSkill(message.AttackTarget);
            }
          
        }
    }
}