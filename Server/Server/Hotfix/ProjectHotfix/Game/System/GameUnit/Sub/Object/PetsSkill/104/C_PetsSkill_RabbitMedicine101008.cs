
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CustomFrameWork;
using ETModel;
using TencentCloud.Youmall.V20180228.Models;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 玉兔捣药
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_Pets_SKILL_Id.RabbitMedicine)]
    public partial class C_PetsSkill_RabbitMedicine101008 : C_HeroSkillSource
    {
        public override void AfterAwake()
        { // 只调用一次  
            IsDataHasError = false;
            DataUpdate();
        }
        public override void DataUpdate()
        {  //数据变化 更新变更数据 
            if (IsDataHasError) return;
            IsDataHasError = true;

            if (!(Config is Pets_SkillConfig mConfig))
            {
                return;
            }

            MP = mConfig.ConsumeDic[1];
            if (mConfig.ConsumeDic.TryGetValue(2, out var mAG))
            {
                AG = mAG;
            }

            CoolTime = mConfig.CoolTime;

            
            

            NextAttackTime = 0;
            IsDataHasError = false;
        }
    }

    /// <summary>
    /// 玉兔捣药
    /// </summary>
    public partial class C_PetsSkill_RabbitMedicine101008
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (!(Config is Pets_SkillConfig mConfig))
            {
                return false;
            }

            var mGamePlayer = b_Attacker as Pets;
            if (mGamePlayer != null)
            {
                var mKeys = mConfig.UseStandardDic.Keys.ToArray();
                for (int i = 0, len = mKeys.Length; i < len; i++)
                {
                    int key = mKeys[i];
                    int value = mConfig.UseStandardDic[key];

                    switch (key)
                    {
                        case 1:
                            {  // 等级
                                if (mGamePlayer.dBPetsData.PetsLevel < value)
                                {
                                    return false;
                                }
                            }
                            break;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        /*case 6:
                            {
                                var mPropertyValue = mGamePlayer.GetNumerial((E_GameProperty)(key - 1));
                                if (mPropertyValue < value)
                                {
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(409);
                                    return false;
                                }
                            }
                            break;*/
                        default:
                            break;
                    }
                }
            }
            return true;
        }
        public override bool TryUse(CombatSource b_Attacker, CombatSource b_BeAttacker, C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            if (!(Config is Pets_SkillConfig mConfig))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(425);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("技能配置数据异常!");
                return false;
            }

            if (mConfig.skillType == 2) return true;

            if (b_Attacker.GetNumerialFunc(E_GameProperty.PROP_MP) < MP)
            {
                return false;
            }

            Vector2 mSelfPos = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker).Vector2Pos;
            if (Vector2.Distance(mSelfPos, b_Cell.Vector2Pos) > mConfig.Distance)
            {
                return false;
            }

            switch (b_BeAttacker.Identity)
            {
                case E_Identity.Hero:
                    {
                        var mTryUseResult = b_Attacker.TryUseByPlayerKilling(b_BeAttacker as GamePlayer, b_Response);
                        if (mTryUseResult == false)
                        {
                            return false;
                        }
                    }
                    break;
                case E_Identity.Enemy:
                    break;
                case E_Identity.Npc:
                    break;
                case E_Identity.Pet:
                    {
                        var mTryUseResult = b_Attacker.TryUseByPlayerKilling(b_BeAttacker as Pets, b_Response);
                        if (mTryUseResult == false)
                        {
                            return false;
                        }
                    }
                    break;
                case E_Identity.Summoned:
                    {
                        var mTryUseResult = b_Attacker.TryUseByPlayerKilling(b_BeAttacker as Summoned, b_Response);
                        if (mTryUseResult == false)
                        {
                            return false;
                        }
                    }
                    break;
                case E_Identity.HolyteacherSummoned:
                    break;
                default:
                    break;
            }

            return TryUseByUseStandard(b_Attacker, b_Response);
        }
        public override bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent)
        {
            if (!(Config is Pets_SkillConfig mConfig))
            {
                return false;
            }
            //BattleComponent.Log($"使用技能 <<{mConfig.Name}>> 使用宠物为:{b_Attacker.InstanceId}", false);

            Pets pets = b_Attacker as Pets;

            CombatSource mBeAttacker = b_BeAttacker;

            int mAttackTime = (int)(b_Attacker.GetAttackSpeed(true));//mConfig.SkillInterval;

            G2C_AttackStart_notice mAttackStartNotice = new G2C_AttackStart_notice();
            mAttackStartNotice.AttackSource = b_Attacker.InstanceId;
            mAttackStartNotice.AttackTarget = mBeAttacker.InstanceId;
            mAttackStartNotice.AttackType = Id;
            mAttackStartNotice.Ticks = b_BattleComponent.CurrentTimeTick + mAttackTime + 100;
            mAttackStartNotice.MpValue = pets.dBPetsData.PetsMP - this.MP;
            mAttackStartNotice.AG = 0;
            b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackStartNotice);

            b_Attacker.IsAttacking = true;
            b_Attacker.AttackTime = b_BattleComponent.CurrentTimeTick + mAttackTime;

            Action<long, long, long> mSyncAction = (b_CombatRoundId, b_AttackerId, b_BeAttackerId) =>
            {
                //if (b_Attacker.CombatRoundId != b_CombatRoundId) return;

                if (b_Attacker.InstanceId != b_AttackerId || b_Attacker.IsDeath || b_Attacker.IsDisposeable || b_Attacker.UnitData.Index != b_BattleComponent.Parent.MapId)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 6;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);
                    return;
                }
                if (mBeAttacker.InstanceId != b_BeAttackerId || mBeAttacker.IsDeath || mBeAttacker.IsDisposeable || mBeAttacker.UnitData.Index != b_BattleComponent.Parent.MapId)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 6;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);
                    return;
                }
                C_FindTheWay2D b_Cell = b_BattleComponent.Parent.GetFindTheWay2D(mBeAttacker);
                // 距离变化了 超过技能范围则取消技能效果
                if (Vector2.Distance(b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker).Vector2Pos, b_Cell.Vector2Pos) > mConfig.Distance)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 6;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);

                    //b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
                    return;
                }
                int mMinAtteck = 0; //+ b_Attacker.GetNumerialFunc(E_GameProperty.MinAtteck);
                int mMaxAtteck = 0; //+ b_Attacker.GetNumerialFunc(E_GameProperty.MaxAtteck);
                if ((b_Attacker as Pets).GamePlayer != null)
                {
                    {
                        //switch ((E_GameOccupation)(b_Attacker as Pets).GamePlayer.Data.PlayerTypeId)
                        //{
                        //    case E_GameOccupation.Archer:
                        //    case E_GameOccupation.Swordsman:
                        //    case E_GameOccupation.GrowLancer:
                        //    case E_GameOccupation.Combat:
                        //        mMinAtteck = (b_Attacker as Pets).GamePlayer.GetNumerialFunc(E_GameProperty.MinAtteck);
                        //        mMaxAtteck = (b_Attacker as Pets).GamePlayer.GetNumerialFunc(E_GameProperty.MaxAtteck);
                        //        break;
                        //    case E_GameOccupation.Spell:
                        //    case E_GameOccupation.Spellsword:
                        //        mMinAtteck = (b_Attacker as Pets).GamePlayer.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);
                        //        mMaxAtteck = (b_Attacker as Pets).GamePlayer.GetNumerialFunc(E_GameProperty.MinMagicAtteck);
                        //        break;
                        //    case E_GameOccupation.Holyteacher:
                        //    case E_GameOccupation.SummonWarlock:
                        //        { 
                        //        }
                        //        break;
                    }
                    int AMi = (b_Attacker as Pets).GamePlayer.GetNumerialFunc(E_GameProperty.MinMagicAtteck);
                    int AMa = (b_Attacker as Pets).GamePlayer.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);
                    int BMi = (b_Attacker as Pets).GamePlayer.GetNumerialFunc(E_GameProperty.MinAtteck);
                    int BMa = (b_Attacker as Pets).GamePlayer.GetNumerialFunc(E_GameProperty.MaxAtteck);
                    if (AMi < BMi && AMa < BMa)
                    {
                        mMaxAtteck = BMa;
                        mMinAtteck = BMi;
                    }
                    else
                    {
                        mMaxAtteck = AMa;
                        mMinAtteck = AMi;
                    }

                    var EC = (b_Attacker as Pets).GamePlayer.Player.GetCustomComponent<EquipmentComponent>();
                    var PetItem = EC.GetEquipItemByPosition(EquipPosition.Pet);
                    if (PetItem != null)
                    {
                        switch (PetItem.ConfigID)
                        {
                            case 350009:
                            case 350010:
                            case 350013:
                            case 350014:
                            case 350030:
                                mMinAtteck *= 2;
                                mMaxAtteck *= 2;
                                break;
                            case 350016:
                            case 350017:
                            case 350020:
                            case 350021:
                            case 350031:
                                mMinAtteck *= 3;
                                mMaxAtteck *= 3;
                                break;
                            case 350023:
                            case 350024:
                            case 350027:
                            case 350028:
                            case 350032:
                                mMinAtteck *= 4;
                                mMaxAtteck *= 4;
                                break;
                            default:
                                mMinAtteck *= 1;
                                mMaxAtteck *= 1;
                                break;
                        }
                    }
                    mMinAtteck += mConfig.OtherDataDic[1];
                    mMaxAtteck += mConfig.OtherDataDic[1];
                }
                bool IsHit = false;
                switch (mBeAttacker.Identity)
                {
                    case E_Identity.Hero:
                        {
                            IsHit = b_Attacker.PetsIsHitPvP(mBeAttacker, b_BattleComponent, true);
                        }
                        break;
                    default:
                        {
                            IsHit = b_Attacker.IsHitPvE(mBeAttacker, b_BattleComponent, true);
                            IsHit = true;
                        }
                        break;
                }
                if (IsHit == false)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 1;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);

                    //b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
                    return;
                }
                int mVirtualHurtValue;
                E_GameProperty b_SpecialAttack = b_Attacker.AttackSpecial();
                if (b_SpecialAttack == E_GameProperty.LucklyAttackRate)
                {
                    mVirtualHurtValue = mMaxAtteck + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedLuckyStrokeAttack) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                }
                else
                {
                    mVirtualHurtValue = Help_RandomHelper.Range(mMinAtteck, mMaxAtteck) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                    if (b_SpecialAttack == E_GameProperty.ExcellentAttackRate)
                        mVirtualHurtValue += b_Attacker.GetNumerialFunc(E_GameProperty.EmbedGreatShotAttack);
                }

                switch (mBeAttacker.Identity)
                {
                    case E_Identity.Enemy:
                        (mBeAttacker as Enemy).InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, b_SpecialAttack, Id, mVirtualHurtValue, b_BattleComponent);
                        break;
                    case E_Identity.Summoned:
                        (mBeAttacker as Summoned).InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, b_SpecialAttack, Id, mVirtualHurtValue, b_BattleComponent);
                        break;
                    case E_Identity.Pet:
                        (mBeAttacker as Pets).InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, b_SpecialAttack, Id, mVirtualHurtValue, b_BattleComponent);
                        break;
                    case E_Identity.Hero:
                        (mBeAttacker as GamePlayer).InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, b_SpecialAttack, Id, mVirtualHurtValue, b_BattleComponent);
                        break;
                    default:
                        break;
                }

                //b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            };
            b_BattleComponent.WaitSync(mConfig.DamageWait, b_Attacker.InstanceId, mBeAttacker.InstanceId, mSyncAction);
            b_Attacker.CombatRoundId = 0;
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            return true;
        }
    }
}
