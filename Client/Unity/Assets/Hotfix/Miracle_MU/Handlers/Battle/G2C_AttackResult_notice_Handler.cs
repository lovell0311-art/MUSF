using UnityEngine;
using ETModel;
using UnityEngine.Profiling;
using System.Runtime.CompilerServices;

namespace ETHotfix
{
    /// <summary>
    /// 攻击返回通知
    /// </summary>
    [MessageHandler]
    public class G2C_AttackResult_notice_Handler : AMHandler<G2C_AttackResult_notice>
    {
        protected override void Run(ETModel.Session session, G2C_AttackResult_notice message)
        {
            // Log.DebugWhtie($"{JsonHelper.ToJson(message)}");
            UnitEntity unitEntity = UnitEntityComponent.Instance.Get<UnitEntity>(message.AttackTarget);
            if (unitEntity == null) return;
            UnitEntity AtkSourceUnitEntity = UnitEntityComponent.Instance.Get<UnitEntity>(message.AttackSource);

            /*if (AtkSourceUnitEntity is RoleEntity && AtkSourceUnitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                if (RoleOnHookComponent.Instance.IsOnHooking) RoleOnHookComponent.Instance.IsAttack = false;
                else if (UIMainComponent.Instance.IsCanUse) UIMainComponent.Instance.IsCanUse = false;
            }*/
            
            //自己被其他玩家攻击
            if (message.AttackTarget == UnitEntityComponent.Instance.LocaRoleUUID && AtkSourceUnitEntity is RoleEntity role)
            {
                UIMainComponent.Instance.ShowPkState();
                //第一次被击 或者 第二次反击
                if (GlobalDataManager.curBeatUUID != role.Id || (GlobalDataManager.IsBeatBack == false && GlobalDataManager.curBeatUUID == role.Id))
                {
                   // Log.DebugGreen($"可以主动反击 攻击者：{message.AttackSource} 被击者：{message.AttackTarget} 本地玩家：{UnitEntityComponent.Instance.LocaRoleUUID}");
                    GlobalDataManager.IsBeatBack = true;
                    GlobalDataManager.BeatBackTimer = Time.time + 60;
                    GlobalDataManager.curBeatUUID = role.Id;
                    ClickSelectUnitEntityComponent.Instance.curSelectUnit = role;
                }
            }
           
            //TODO message.HurtValueType 6 距离过超出攻击范围、怪物已经死亡）、（自身死亡） 终止技能释放（攻击）
            if (message.HurtValueType == 6)
            {
               // Log.DebugRed("距离过超出攻击范围、怪物已经死亡）、（自身死亡） 终止技能释放（攻击）");
                if (RoleOnHookComponent.Instance.IsOnHooking)
                {
                    RoleOnHookComponent.Instance.curAttackEntity = null;
                }
                else
                {
                    UIMainComponent.Instance.curSkillEntity = null;
                }
                return;
            }

            //更新每秒伤害
            if (message.AttackSource==UnitEntityComponent.Instance.LocaRoleUUID)
            {
                UIMainComponent.Instance?.ChangeDemageCount(message.HurtValue);
            }


           // Log.DebugBrown($"伤害来源：{message.AttackSource} 被击者：{message.AttackTarget} 本地玩家：{UnitEntityComponent.Instance.LocaRoleUUID}");
            UnitEntityHitTextComponent unitEntityHitText = unitEntity.GetComponent<UnitEntityHitTextComponent>();

            if (unitEntityHitText == null) return;

          
          
            // Profiler.BeginSample("Miss");
            if (message.HurtValue == 0 || message.HurtValueType == 1)
            {

                unitEntityHitText.SetColor(GetHitColor(1));//为1表示护盾抵消伤害
            }
            else
            {
                unitEntityHitText.SetColor(GetHitColor((int)message.HurtValueType));

            }

            if (message.HurtValueSource == 122)//连击
            {
                unitEntityHitText.ShowHitText(message.HurtValue.ToString(), Vector3.one * 3f);
            }
            switch (message.HurtValueType)
            {
                case 1:
                    {
                        unitEntityHitText.ShowHitText("Miss", Vector3.one);
                        return;
                    }
                case 2:
                case 3:
                case 4:
                case 5:
                    {
                        unitEntityHitText.ShowHitText(message.HurtValue.ToString(), Vector3.one * 5f);
                    }
                    break;
                case 7:
                    {
                        unitEntityHitText.ShowHitText(message.HurtValue.ToString(), Vector3.one);
                        return;
                    }
                case 8:
                    {
                        unitEntityHitText.ShowHitText(message.HurtValue.ToString(), Vector3.one);
                        return;
                    } 
                case 9:
                    {
                        unitEntityHitText.ShowHitText(message.HurtValue.ToString(), Vector3.one);
                        return;
                    }
                case 0:
                    unitEntityHitText.ShowHitText(message.HurtValue.ToString(), Vector3.one);
                    break;
                default:
                    {
                        unitEntityHitText.ShowHitText(message.HurtValue.ToString(), Vector3.one);
                    }
                    break;
            }

          //  Profiler.EndSample();


            //改变当前实体的血条
            if (unitEntity is RoleEntity roleEntity)
            {
                if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                {

                    roleEntity.Property.ChangeProperValue(E_GameProperty.PROP_HP, message.HpValue);

                    UIMainComponent.Instance?.ChangeRoleHp(Maxvalue: message.HpMaxValue, value: message.HpValue);

                    if (message.SD != 0 && message.SDMaxValue != 0)
                    {
                        roleEntity.Property.ChangeProperValue(E_GameProperty.PROP_SD, message.SD);
                        roleEntity.Property.ChangeProperValue(E_GameProperty.PROP_SD_MAX, message.SDMaxValue);
                        UIMainComponent.Instance?.ChangeSD();
                    }

                    if (SiegeWarfareData.SiegeWarfareIsStart && SiegeWarfareData.currole != null && SiegeWarfareData.currole.Game_Object != null)
                    {
                        if(roleEntity.Game_Object.GetInstanceID() == SiegeWarfareData.currole.Game_Object.GetInstanceID())
                            TriggerEvents.Instance.RoleActionLevea?.Invoke(roleEntity.Game_Object.transform);
                    }
                }
                else
                {
                    //其他玩家 被Pk时的血条
                    if (GlobalDataManager.BattleModel == E_BattleType.Whole || ClickSelectUnitEntityComponent.Instance.curSelectUnit is RoleEntity)
                    {
                        roleEntity.GetComponent<UIUnitEntityHpBarComponent>().ChangeHp(Maxvalue: message.HpMaxValue, value: message.HpValue);
                    }
                }
                if (message.HpValue == 0)
                {
                    roleEntity.Dead();
                }
                //else
                //{
                //    if (SiegeWarfareData.currole?.Game_Object.GetInstanceID() == roleEntity.Game_Object.GetInstanceID())
                //    {
                //        SiegeWarfareComponent.Instance.LeveaThrone(roleEntity.Game_Object).Coroutine();
                //    }
                //}
            }
            else if (unitEntity is SummonEntity summonEntity)
            {
                if (UnitEntityComponent.Instance.LocalRole.summonEntity != null && UnitEntityComponent.Instance.LocalRole.summonEntity.Id == unitEntity.Id && message.HpValue <= 0)
                {
                    UnitEntityComponent.Instance.LocalRole.summonEntity = null;
                }
                summonEntity.GetComponent<UIUnitEntityHpBarComponent>().ChangeHp(Maxvalue: message.HpMaxValue, value: message.HpValue);
              
            }
            else if (unitEntity is MonsterEntity monsterEntity)
            {
                if (monsterEntity.MonsterType == 1 || monsterEntity.MonsterType == 6)
                {
                    UIMainComponent.Instance?.ChangeBossHp((float)message.HpValue / message.HpMaxValue);
                    if (message.HpValue <= 0)
                    {
                        

                        monsterEntity.Dead();
                    }
                }
                else
                {
                    if (message.HpValue <= 0)
                    {
                       
                        //更新每秒杀怪
                        if (message.AttackSource == UnitEntityComponent.Instance.LocaRoleUUID)
                        {
                            UIMainComponent.Instance?.ChangeKillMonsterCount(1);
                        }
                    }
                    monsterEntity.GetComponent<UIUnitEntityHpBarComponent>().ChangeHp(Maxvalue: message.HpMaxValue, value: message.HpValue);
                }
               
            }
            else if (unitEntity is PetEntity petEntity)
            {

                
                if (petEntity.RoleId == RoleArchiveInfoManager.Instance.LoadRoleUUID)
                {
                    //  Log.DebugBrown("打印数据" + petEntity.RoleId + ":::" + RoleArchiveInfoManager.Instance.LoadRoleUUID);
                    //   petEntity.GetComponent<UIUnitEntityHpBarComponent>().ChangeHp( message.HpValue, message.HpMaxValue);
                    //更新血量
                    petEntity.GetComponent<UIUnitEntityHpBarComponent>().PetHp(Maxvalue: message.HpValue, value: message.HpMaxValue);
                    Log.DebugBrown("数据HpValue" + message.HpValue + "::" + message.HpMaxValue);
                    UIMainComponent.Instance?.SetPetHpValue(message.HpValue, petEntity.MaxHp);
                    if (UIComponent.Instance?.Get(UIType.UIPet)?.GetComponent<UIPetComponent>() != null)
                    {
                       // Log.DebugBrown("数据变化"+ message.HpValue);
                        UIComponent.Instance.Get(UIType.UIPet).GetComponent<UIPetComponent>()?.RealTimeUpdatePetHp(message.AttackTarget, message.HpValue, message.HpMaxValue);
                    }
                    
                    if (message.HpValue <= 0)
                    {
                        UIComponent.Instance.Get(UIType.UIPet)?.GetComponent<UIPetComponent>().HindYaoPingObj();
                        UIComponent.Instance.Remove(UIType.UIPet);
                        if (UIMainComponent.Instance.DeadTxt.gameObject != null)
                            UIMainComponent.Instance?.DeadTxt.gameObject.SetActive(true);
                        petEntity.Dead();
                    }

                }
                else
                {
                    if (message.HpValue <= 0)
                    {
                        petEntity.Dead();
                    }

                }
            }
            else if (unitEntity is NPCEntity npc)
            {
                npc.Dispose();
            }
        }
        /// <summary>
        /// 获取被击伤害的颜色
        /// </summary>
        /// <param name="hitType">伤害类型</param>
        /// <returns></returns>
        //public string GetHitColor(int hitType) => hitType switch
        //{

        //    0 => "#FA6401",//普通伤害
        //    1 => "#FFFFFF",//Miss
        //    2 => "#0191FF",//幸运一击
        //    3 => "#44D7B6",//卓越一击
        //    4 => "#8698e7",//双倍伤害
        //    5 => "#ffff00",//三倍伤害
        //    100 => "#32C5FF",//无视防御
        //    101 => "#FFFFFF",//Miss
        //    102 => "#0191FF",//幸运一击
        //    103 => "#44D7B6",//卓越一击
        //    104 => "#8698e7",//双倍伤害
        //    105 => "#ffff00",//三倍伤害
        //    _ => "#ff0004"
        //}; 
        /// <summary>
        /// 获取被击伤害的颜色
        /// </summary>
        /// <param name="hitType">伤害类型</param>
        /// <returns></returns>
        public string GetHitColor(int hitType) => hitType switch
        {

            0 => "#FF9900",//普通伤害
            1 => "#FFFFFF",//Miss
            2 => "#0099FF",//幸运一击
            3 => "#00FF99",//卓越一击
            4 => "#DE6CD7",//双倍伤害
            5 => "#ffff00",//三倍伤害
            100 => "#00FFFF",//无视防御
            101 => "#FFFFFF",//Miss
            102 => "#0099FF",//幸运一击
            103 => "#00FF99",//卓越一击
            104 => "#DE6CD7",//双倍伤害
            105 => "#ffff00",//三倍伤害
            _ => "#ff0004"
        };
    }

}