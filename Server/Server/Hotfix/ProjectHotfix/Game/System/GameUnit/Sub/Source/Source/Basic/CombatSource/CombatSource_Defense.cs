using System;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using ETModel;
using TencentCloud.Ocr.V20181119.Models;
using UnityEngine;

namespace ETHotfix
{
    public static partial class CombatSourceSystem
    {
        public static (int, int) Defense(this CombatSource b_BeAttacker, CombatSource b_Attacker,
             E_BattleHurtAttackType b_AttackType,
             E_BattleHurtType b_BattleHurtType,
             bool b_IgnoreDefense,
             int b_HurtTypeId,
             E_GameProperty b_SpecialAttack,
             int b_InjureValue,
             BattleComponent b_BattleComponent,
             bool b_CanDefense = true)
        {
            if (b_BeAttacker.HealthStatsDic.ContainsKey(E_BattleSkillStats.WuDi))
            {
                return (0, 0);
            }

            // 玩家等级小于50 免疫伤害
            if (b_Attacker.Identity != E_Identity.Enemy)
            {
                if (b_BeAttacker.Identity != E_Identity.Enemy)
                {
                    var mAttackerUserId = b_Attacker.GetPlayerInstance();
                    var mAttackerFanJiIdlist = b_Attacker.GetFanJiIdlist();

                    var mBeAttackerUserId = b_BeAttacker.GetPlayerInstance();
                    var mBeAttackerFanJiIdlist = b_BeAttacker.GetFanJiIdlist();
                    if (mAttackerFanJiIdlist.TryGetValue(mBeAttackerUserId, out var mBeAttackerlastAttackTime) == false)
                    {
                        var mThisAttackTime = CustomFrameWork.Help_TimeHelper.GetNowSecond();
                        if (mBeAttackerFanJiIdlist.TryGetValue(mAttackerUserId, out var mAttackerlastAttackTime) == false)
                        {
                            mBeAttackerFanJiIdlist[mAttackerUserId] = mThisAttackTime;
                        }
                        else if (mAttackerlastAttackTime + 60 < mThisAttackTime)
                        {
                            mBeAttackerFanJiIdlist[mAttackerUserId] = mThisAttackTime;
                        }
                    }
                    else
                    {
                        var mThisAttackTime = CustomFrameWork.Help_TimeHelper.GetNowSecond();
                        if (mBeAttackerlastAttackTime + 60 < mThisAttackTime)
                        {
                            if (mBeAttackerFanJiIdlist.TryGetValue(mAttackerUserId, out var mAttackerlastAttackTime) == false)
                            {
                                mBeAttackerFanJiIdlist[mAttackerUserId] = mThisAttackTime;
                            }
                            else if (mAttackerlastAttackTime + 60 < mThisAttackTime)
                            {
                                mBeAttackerFanJiIdlist[mAttackerUserId] = mThisAttackTime;
                            }
                        }
                    }
                }

                switch (b_BeAttacker.Identity)
                {
                    case E_Identity.Enemy:
                        break;
                    case E_Identity.Summoned:
                        {
                            var mSummoned = b_BeAttacker as Summoned;

                            if (mSummoned.GamePlayer.Data.Level <= 50)
                            {
                                return (0, 0);
                            }
                        }
                        break;
                    case E_Identity.Pet:
                        {
                            var mPet = b_BeAttacker as Pets;

                            if (mPet.GamePlayer.Data.Level <= 50)
                            {
                                return (0, 0);
                            }
                        }
                        break;
                    case E_Identity.Hero:
                        {
                            var mGamePlayer = b_BeAttacker as GamePlayer;

                            if (mGamePlayer.Data.Level <= 50)
                            {
                                return (0, 0);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            if (b_BeAttacker.Identity != E_Identity.Enemy)
            {
                switch (b_Attacker.Identity)
                {
                    case E_Identity.Enemy:
                        break;
                    case E_Identity.Summoned:
                        {
                            var mSummoned = b_Attacker as Summoned;

                            if (mSummoned.GamePlayer.Data.Level <= 50)
                            {
                                return (0, 0);
                            }
                        }
                        break;
                    case E_Identity.Pet:
                        {
                            var mPet = b_Attacker as Pets;

                            if (mPet.GamePlayer.Data.Level <= 50)
                            {
                                return (0, 0);
                            }
                        }
                        break;
                    case E_Identity.Hero:
                        {
                            var mGamePlayer = b_Attacker as GamePlayer;

                            if (mGamePlayer.Data.Level <= 50)
                            {
                                return (0, 0);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            int mRealInjureValue = b_InjureValue;
            int mRealInjureSDValue = 0;

            {
                if (b_Attacker.Identity == E_Identity.Hero)
                {
                    if (b_BeAttacker.Identity == E_Identity.Hero)
                    {
                        mRealInjureValue += b_Attacker.GetNumerialFunc(E_GameProperty.PVPAttack);
                    }
                    else
                    {
                        mRealInjureValue += b_Attacker.GetNumerialFunc(E_GameProperty.PVEAttack);
                    }
                }
                else
                {
                    mRealInjureValue += b_Attacker.GetNumerialFunc(E_GameProperty.PVEAttack);
                }

                if (b_Attacker.Identity == E_Identity.Hero)
                {
                    #region 攻击回复
                    G2C_ChangeValue_notice AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                    {
                        if (b_ChangeValue_notice == null) b_ChangeValue_notice = new G2C_ChangeValue_notice();

                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                        mBattleKVData.Key = (int)b_GameProperty;
                        mBattleKVData.Value = b_Attacker.GetNumerialFunc(b_GameProperty);
                        b_ChangeValue_notice.Info.Add(mBattleKVData);

                        return b_ChangeValue_notice;
                    }
                    G2C_ChangeValue_notice mChangeValue_notice = null;

                    var mReplyValueRate = b_Attacker.GetNumerialFunc(E_GameProperty.HpAbsorbRate);
                    if (mReplyValueRate > 0)
                    {
                        var mRate = Help_RandomHelper.Range(0, 100);
                        if (mRate < 50)
                        {
                            b_Attacker.UnitData.Hp += mReplyValueRate;
                            var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                            if (b_Attacker.UnitData.Hp > mMax) b_Attacker.UnitData.Hp = mMax;
                            mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_HP);
                        }
                    }

                    mReplyValueRate = b_Attacker.GetNumerialFunc(E_GameProperty.SdAbsorbRate);
                    if (mReplyValueRate > 0)
                    {
                        var mRate = Help_RandomHelper.Range(0, 100);
                        if (mRate < 50)
                        {
                            b_Attacker.UnitData.SD += mReplyValueRate;
                            var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_SD_MAX);
                            if (b_Attacker.UnitData.SD > mMax) b_Attacker.UnitData.SD = mMax;
                            mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_SD);
                        }
                    }
                    mReplyValueRate = b_Attacker.GetNumerialFunc(E_GameProperty.Attack_ReplyAllSdRate);
                    if (mReplyValueRate > 0)
                    {
                        var mRate = Help_RandomHelper.Range(0, 100);
                        if (mRate < mReplyValueRate)
                        {
                            var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_SD_MAX);
                            if (b_Attacker.UnitData.SD != mMax)
                            {
                                b_Attacker.UnitData.SD = mMax;
                                mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_SD);
                            }
                        }
                    }
                    if (mChangeValue_notice != null)
                    {
                        mChangeValue_notice.GameUserId = b_Attacker.InstanceId;
                        b_BattleComponent.Parent.SendNotice(b_Attacker, mChangeValue_notice);
                    }
                    #endregion

                    var mGamePlayer = b_Attacker as GamePlayer;
                    if (mGamePlayer != null)
                    {
                        switch ((E_GameOccupation)mGamePlayer.Data.PlayerTypeId)
                        {
                            case E_GameOccupation.Spell:
                                {
                                    if (mGamePlayer.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MagicField67, out var mMasteryValue))
                                    {
                                        C_FindTheWay2D mAttackerCell = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker);
                                        C_FindTheWay2D mBeAttackerCell = b_BattleComponent.Parent.GetFindTheWay2D(b_BeAttacker);
                                        if (mBeAttackerCell != null && mAttackerCell != null)
                                        {
                                            if (10 * Vector2.Distance(mAttackerCell.Vector2Pos, mBeAttackerCell.Vector2Pos) > 40)
                                            {
                                                mRealInjureValue += mMasteryValue;
                                            }
                                        }
                                    }
                                }
                                break;
                            case E_GameOccupation.Archer:
                                {
                                    if (mGamePlayer.BattleMasteryDic.TryGetValue(E_BattleMasteryState.ShenSheShou272, out var mMasteryValue))
                                    {
                                        C_FindTheWay2D mAttackerCell = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker);
                                        C_FindTheWay2D mBeAttackerCell = b_BattleComponent.Parent.GetFindTheWay2D(b_BeAttacker);
                                        if (mBeAttackerCell != null && mAttackerCell != null)
                                        {
                                            if (10 * Vector2.Distance(mAttackerCell.Vector2Pos, mBeAttackerCell.Vector2Pos) > 40)
                                            {

                                                mRealInjureValue += mMasteryValue;
                                            }
                                        }
                                    }
                                }
                                break;
                            case E_GameOccupation.SummonWarlock:
                                {
                                    if (mGamePlayer.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_ZuZhouDeTongKu_Master562, out var mMasteryValue))
                                    {
                                        C_FindTheWay2D mAttackerCell = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker);
                                        C_FindTheWay2D mBeAttackerCell = b_BattleComponent.Parent.GetFindTheWay2D(b_BeAttacker);
                                        if (mBeAttackerCell != null && mAttackerCell != null)
                                        {
                                            if (10 * Vector2.Distance(mAttackerCell.Vector2Pos, mBeAttackerCell.Vector2Pos) > 40)
                                            {
                                                mRealInjureValue += mMasteryValue;
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            {// 攻击时无视防御概率 最大为80%
                bool mIgnoreDefense = b_IgnoreDefense;
                int mAttackIgnoreDefenseRate = b_Attacker.GetNumerialFunc(E_GameProperty.AttackIgnoreDefenseRate);
                if (mAttackIgnoreDefenseRate > 0)
                {
                    //mAttackIgnoreDefenseRate *= 100;
                    if (mAttackIgnoreDefenseRate > 8000) mAttackIgnoreDefenseRate = 8000;

                    int mRandomResult = CustomFrameWork.Help_RandomHelper.Range(0, 10000);
                    if (mRandomResult <= mAttackIgnoreDefenseRate)
                    {
                        mIgnoreDefense = true;
                    }
                }

                if (mIgnoreDefense == false)
                {// 防御属性减免伤害
                    int mDefenseValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.Defense);
                    if (mDefenseValue > 0)
                    {
                        switch (b_Attacker.Identity)
                        {
                            case E_Identity.Enemy:
                            case E_Identity.Summoned:
                                {
                                    mDefenseValue = mDefenseValue / 2;
                                }
                                break;
                            default:
                                break;
                        }

                        mRealInjureValue -= mDefenseValue;
                        if (mRealInjureValue <= 0) mRealInjureValue = 1;
                    }
                }
            }

            { // 防御属性减免伤害
                int mDefenseValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.ReallyDefense);
                if (mDefenseValue > 0)
                {
                    mRealInjureValue -= mDefenseValue;
                    if (mRealInjureValue <= 0) mRealInjureValue = 1;
                }
            }

            switch (b_Attacker.Identity)
            {
                case E_Identity.Enemy:
                case E_Identity.Summoned:
                case E_Identity.Pet:
                case E_Identity.Hero:
                    {//pvp
                        // 职业技能伤害增幅
                        if (b_AttackType == E_BattleHurtAttackType.SKILL)
                        {
                            switch (b_Attacker.GameHeroType)
                            {
                                case E_GameOccupation.Swordsman:
                                    {
                                        int mSkillAdditionValue = b_Attacker.GetNumerialFunc(E_GameProperty.SkillAddition);
                                        if (mSkillAdditionValue > 0)
                                        {
                                            mRealInjureValue = (int)(mRealInjureValue * mSkillAdditionValue * 0.01f);
                                            if (mRealInjureValue < 1)
                                            {
                                                mRealInjureValue = 1;
                                            }
                                        }
                                    }
                                    break;
                                case E_GameOccupation.Spellsword:
                                    {
                                        switch (b_HurtTypeId)
                                        {
                                            case 303:
                                            case 315:
                                            case 316:
                                            case 317:
                                            case 318:
                                            case 319:
                                            case 320:
                                            case 322:
                                            case 323:
                                            case 327:
                                            case 330:
                                                {
                                                    int mSkillAdditionValue = b_Attacker.GetNumerialFunc(E_GameProperty.SkillAddition);
                                                    if (mSkillAdditionValue > 0)
                                                    {
                                                        mRealInjureValue = (int)(mRealInjureValue * mSkillAdditionValue * 0.01f);
                                                        if (mRealInjureValue < 1)
                                                        {
                                                            mRealInjureValue = 1;
                                                        }
                                                    }
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                case E_GameOccupation.Holyteacher:
                                    {
                                        int mSkillAdditionValue = b_Attacker.GetNumerialFunc(E_GameProperty.SkillAddition);
                                        if (mSkillAdditionValue > 0)
                                        {
                                            mRealInjureValue = (int)(mRealInjureValue * mSkillAdditionValue * 0.01f);
                                            if (mRealInjureValue < 1)
                                            {
                                                mRealInjureValue = 1;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            switch (b_SpecialAttack)
            {
                case E_GameProperty.InjuryValueRate_2:
                    {
                        mRealInjureValue *= 2;
                    }
                    break;
                case E_GameProperty.InjuryValueRate_3:
                    {
                        mRealInjureValue *= 3;
                    }
                    break;
                case E_GameProperty.ExcellentAttackRate:
                    {
                        mRealInjureValue = (int)(mRealInjureValue * 1.2f);

                        {// 卓越一击伤害增加量
                            int mDefenseValue = b_Attacker.GetNumerialFunc(E_GameProperty.ExcellentAttackHurtValueIncrease);
                            if (mDefenseValue > 0) mRealInjureValue += mDefenseValue;
                        }
                    }
                    break;
                case E_GameProperty.LucklyAttackRate:
                    {
                        {// 幸运一击伤害增加量
                            int mDefenseValue = b_Attacker.GetNumerialFunc(E_GameProperty.LucklyAttackHurtValueIncrease);
                            if (mDefenseValue > 0) mRealInjureValue += mDefenseValue;
                        }
                    }
                    break;
                default:
                    break;
            }


            // TODO 耐久计算
            {
                if (b_BeAttacker.Identity == E_Identity.Hero)
                {
                    GamePlayer gamePlayer = b_BeAttacker as GamePlayer;
                    EquipmentComponent equipCmpt = gamePlayer.Player.GetCustomComponent<EquipmentComponent>();
                    Item guard = equipCmpt.GetEquipItemByPosition(EquipPosition.Guard);
                    if (guard != null)
                    {
                        // 守护
                        if (guard.SpriteDamage(mRealInjureValue, gamePlayer))
                        {
                            if (guard.GetProp(EItemValue.Durability) <= 0)
                            {
                                equipCmpt.UnloadEquipItem(EquipPosition.Guard, "耐久降为0")?.DisposeDB("耐久降为0");
                            }
                            equipCmpt.ApplyEquipProp();
                        }
                    }
                    Item mounts = equipCmpt.GetEquipItemByPosition(EquipPosition.Mounts);
                    if (mounts != null)
                    {
                        // 坐骑
                        if (mounts.MountsDamage(mRealInjureValue, gamePlayer))
                        {
                            if (mounts.GetProp(EItemValue.Durability) <= 0)
                            {
                                // 坐骑死亡
                                // TODO 卸载坐骑，重新应用玩家属性
                                equipCmpt.UnloadEquipItemToBackpack(EquipPosition.Mounts, 0, 0, "坐骑死亡，主动卸下坐骑");
                                mounts.SetProp(EItemValue.IsUsing, 0, gamePlayer.Player);
                                equipCmpt.ApplyEquipProp();
                            }
                        }
                    }

                    // 防具
                    equipCmpt.ArmorRandomDurDown(b_Attacker);
                }
                if (b_Attacker.Identity == E_Identity.Hero)
                {
                    // 武器
                    GamePlayer gamePlayer = b_Attacker as GamePlayer;
                    EquipmentComponent equipCmpt = gamePlayer.Player.GetCustomComponent<EquipmentComponent>();
                    equipCmpt.WeaponDurDown(b_BeAttacker);
                }
            }

            if (mRealInjureValue <= 0) mRealInjureValue = 1;

            {// 万分比
                { // 伤害提高率 
                    int mIncreaseValue = b_Attacker.GetNumerialFunc(E_GameProperty.InjuryValueRate_Increase);
                    if (mIncreaseValue > 0)
                    {
                        int mBufferValue = 10000 + mIncreaseValue;
                        if (mBufferValue <= 0) mBufferValue = 1;
                        mRealInjureValue = (int)(mRealInjureValue * (mBufferValue * 0.0001f));
                    }
                }
                { // 伤害减少率
                    int mReduceValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.InjuryValueRate_Reduce);
                    int mDhr = b_Attacker.GetNumerialFunc(E_GameProperty.DisregardHarmReductionPct);
                    if (mDhr > 0) mReduceValue -= mDhr;

                    if (mReduceValue > 0)
                    {
                        int mBufferValue = 10000 - mReduceValue;
                        if (mBufferValue <= 0) mBufferValue = 1;
                        mRealInjureValue = (int)(mRealInjureValue * (mBufferValue * 0.0001f));
                    }
                }

                {// 攻击时无视防御概率 最大为80%
                    bool mIgnoreDefense = false;
                    int mAttackIgnoreDefenseRate = b_Attacker.GetNumerialFunc(E_GameProperty.AttackIgnoreAbsorbRate);
                    if (mAttackIgnoreDefenseRate > 0)
                    {
                        //mAttackIgnoreDefenseRate *= 100;
                        if (mAttackIgnoreDefenseRate > 8000) mAttackIgnoreDefenseRate = 8000;

                        int mRandomResult = CustomFrameWork.Help_RandomHelper.Range(0, 10000);
                        if (mRandomResult <= mAttackIgnoreDefenseRate)
                        {
                            mIgnoreDefense = true;
                        }
                    }

                    if (mIgnoreDefense == false)
                    {// 防御属性减免伤害
                        int mIgnoreAbsorbRate = b_Attacker.GetNumerialFunc(E_GameProperty.IgnoreAbsorbRate);
                        { // 伤害吸收率
                            int mReduceValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.HurtValueAbsorbRate);
                            if (mReduceValue > 0 && mIgnoreAbsorbRate > 0)
                            {
                                if (mIgnoreAbsorbRate > mReduceValue)
                                {
                                    mIgnoreAbsorbRate -= mReduceValue;
                                    mReduceValue = 0;
                                }
                                else
                                {
                                    mReduceValue -= mIgnoreAbsorbRate;
                                    mIgnoreAbsorbRate = 0;
                                }
                            }
                            if (mReduceValue > 0)
                            {
                                int mBufferValue = 10000 - mReduceValue;
                                if (mBufferValue <= 0) mBufferValue = 1;
                                mRealInjureValue = (int)(mRealInjureValue * (mBufferValue * 0.0001f));
                            }
                        }
                        {// 宠物伤害吸收率
                            int mReduceValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.DamageAbsPct_Pets) * 100;
                            if (mReduceValue > 0 && mIgnoreAbsorbRate > 0)
                            {
                                if (mIgnoreAbsorbRate > mReduceValue)
                                {
                                    mIgnoreAbsorbRate -= mReduceValue;
                                    mReduceValue = 0;
                                }
                                else
                                {
                                    mReduceValue -= mIgnoreAbsorbRate;
                                    mIgnoreAbsorbRate = 0;
                                }
                            }
                            if (mReduceValue > 0)
                            {
                                int mBufferValue = 10000 - mReduceValue;
                                if (mBufferValue <= 0) mBufferValue = 1;
                                mRealInjureValue = (int)(mRealInjureValue * (mBufferValue * 0.0001f));
                            }
                        }
                        {// 守护伤害吸收率
                            int mReduceValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.DamageAbsPct_Guard) * 100;
                            if (mReduceValue > 0 && mIgnoreAbsorbRate > 0)
                            {
                                if (mIgnoreAbsorbRate > mReduceValue)
                                {
                                    mIgnoreAbsorbRate -= mReduceValue;
                                    mReduceValue = 0;
                                }
                                else
                                {
                                    mReduceValue -= mIgnoreAbsorbRate;
                                    mIgnoreAbsorbRate = 0;
                                }
                            }
                            if (mReduceValue > 0)
                            {
                                int mBufferValue = 10000 - mReduceValue;
                                if (mBufferValue <= 0) mBufferValue = 1;
                                mRealInjureValue = (int)(mRealInjureValue * (mBufferValue * 0.0001f));
                            }
                        }
                        {// 翅膀伤害吸收率
                            int mReduceValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.DamageAbsPct_Wing) * 100;
                            if (mReduceValue > 0 && mIgnoreAbsorbRate > 0)
                            {
                                if (mIgnoreAbsorbRate > mReduceValue)
                                {
                                    mIgnoreAbsorbRate -= mReduceValue;
                                    mReduceValue = 0;
                                }
                                else
                                {
                                    mReduceValue -= mIgnoreAbsorbRate;
                                    mIgnoreAbsorbRate = 0;
                                }
                            }
                            if (mReduceValue > 0)
                            {
                                int mBufferValue = 10000 - mReduceValue;
                                if (mBufferValue <= 0) mBufferValue = 1;
                                mRealInjureValue = (int)(mRealInjureValue * (mBufferValue * 0.0001f));
                            }
                        }
                        {// 宠物伤害吸收率
                            int mReduceValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.DamageAbsPct_Pets) * 100;
                            if (mReduceValue > 0 && mIgnoreAbsorbRate > 0)
                            {
                                if (mIgnoreAbsorbRate > mReduceValue)
                                {
                                    mIgnoreAbsorbRate -= mReduceValue;
                                    mReduceValue = 0;
                                }
                                else
                                {
                                    mReduceValue -= mIgnoreAbsorbRate;
                                    mIgnoreAbsorbRate = 0;
                                }
                            }
                            if (mReduceValue > 0)
                            {
                                int mBufferValue = 10000 - mReduceValue;
                                if (mBufferValue <= 0) mBufferValue = 1;
                                mRealInjureValue = (int)(mRealInjureValue * (mBufferValue * 0.0001f));
                            }
                        }
                        {// 坐骑伤害吸收率
                            int mReduceValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.DamageAbsPct_Mounts) * 100;
                            if (mReduceValue > 0 && mIgnoreAbsorbRate > 0)
                            {
                                if (mIgnoreAbsorbRate > mReduceValue)
                                {
                                    mIgnoreAbsorbRate -= mReduceValue;
                                    mReduceValue = 0;
                                }
                                else
                                {
                                    mReduceValue -= mIgnoreAbsorbRate;
                                    mIgnoreAbsorbRate = 0;
                                }
                            }
                            if (mReduceValue > 0)
                            {
                                int mBufferValue = 10000 - mReduceValue;
                                if (mBufferValue <= 0) mBufferValue = 1;
                                mRealInjureValue = (int)(mRealInjureValue * (mBufferValue * 0.0001f));
                            }
                        }
                        { // 技能伤害吸收率
                            if (b_BeAttacker.HealthStatsDic.TryGetValue(E_BattleSkillStats.HolyShieldDefense102, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    int mBufferValue = 100 - mTempBufferData.StrengthValue;
                                    if (mBufferValue <= 0) mBufferValue = 1;
                                    mRealInjureValue = (int)(mRealInjureValue * (mBufferValue * 0.01f));
                                }
                            }
                            // 这个技能万分比
                            if (b_BeAttacker.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShouHuZhiHun, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    int mBufferValue = 10000 - mTempBufferData.StrengthValue;
                                    if (mBufferValue <= 0) mBufferValue = 1;
                                    mRealInjureValue = (int)(mRealInjureValue * (mBufferValue * 0.0001f));
                                }
                                { // 守护之魂扣篮
                                    switch (b_BeAttacker.Identity)
                                    {
                                        case E_Identity.Hero:
                                            {
                                                if (b_BeAttacker.UnitData.Mp > 0)
                                                {
                                                    b_BeAttacker.UnitData.Mp = (int)(b_BeAttacker.UnitData.Mp * 0.98f);

                                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                                    mChangeValueMessage.GameUserId = b_BeAttacker.InstanceId;

                                                    G2C_BattleKVData mData = new G2C_BattleKVData();
                                                    mData.Key = (int)E_GameProperty.PROP_MP;
                                                    mData.Value = b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_MP);
                                                    mChangeValueMessage.Info.Add(mData);

                                                    (b_BeAttacker as GamePlayer).Player.Send(mChangeValueMessage);
                                                }
                                            }
                                            break;
                                        case E_Identity.Pet:
                                            {
                                                if (b_BeAttacker.UnitData.Mp > 0)
                                                {
                                                    b_BeAttacker.UnitData.Mp = (int)(b_BeAttacker.UnitData.Mp * 0.98f);

                                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                                    mChangeValueMessage.GameUserId = b_BeAttacker.InstanceId;

                                                    G2C_BattleKVData mData = new G2C_BattleKVData();
                                                    mData.Key = (int)E_GameProperty.PROP_MP;
                                                    mData.Value = b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_MP);
                                                    mChangeValueMessage.Info.Add(mData);

                                                    (b_BeAttacker as Pets).GamePlayer.Player.Send(mChangeValueMessage);
                                                }
                                            }
                                            break;
                                        case E_Identity.Summoned:
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            { // 伤害减少量
                int mDefenseValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.InjuryValue_Reduce);
                if (mDefenseValue > 0)
                {
                    mRealInjureValue -= mDefenseValue;
                    if (mRealInjureValue <= 0) mRealInjureValue = 1;
                }
            }

            if (b_Attacker.Identity == E_Identity.Hero && b_BeAttacker.Identity == E_Identity.Hero)
            { // PvP时受到攻击，以SD 90 %, HP 10 % 比率消耗。
                bool mIgnoreDefense = false;
                int mAttackIgnoreSDRate = b_Attacker.GetNumerialFunc(E_GameProperty.SDAttackIgnoreRate);
                if (mAttackIgnoreSDRate > 0)
                {
                    //mAttackIgnoreSDRate *= 100;
                    if (mAttackIgnoreSDRate > 8000) mAttackIgnoreSDRate = 8000;

                    int mRandomResult = CustomFrameWork.Help_RandomHelper.Range(0, mAttackIgnoreSDRate);
                    if (mRandomResult <= mAttackIgnoreSDRate)
                    {
                        mIgnoreDefense = true;
                    }
                }

                if (mIgnoreDefense == false)
                {// SD比率
                    int mAttackSdRate = b_Attacker.GetNumerialFunc(E_GameProperty.AttackSdRate);
                    int mHitSdRate = b_BeAttacker.GetNumerialFunc(E_GameProperty.HitSdRate);

                    int mSdRate = 9000 + mHitSdRate - mAttackSdRate;
                    int mHpRate = 10000 - mSdRate;
                    if (mSdRate < 0) mSdRate = 0;
                    if (mHpRate < 0) mHpRate = 0;
                    if (mSdRate > 10000) mSdRate = 10000;
                    if (mHpRate > 10000) mHpRate = 10000;

                    int mRealInjureValueSource = mRealInjureValue;

                    mRealInjureSDValue = (int)(mRealInjureValueSource * mSdRate * 0.0001f);
                    if (b_BeAttacker.UnitData.SD < mRealInjureSDValue)
                    {
                        mRealInjureSDValue = b_BeAttacker.UnitData.SD;
                    }

                    mRealInjureValue = mRealInjureValueSource - mRealInjureSDValue;
                }
            }

            if (mRealInjureValue < 1)
            {
                mRealInjureValue = 1;
            }

            {// 防御护罩 抵扣伤害值
                if (b_BeAttacker.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        if (mTempBufferData.CacheData.TryGetValue(0, out var mTempBufferValue))
                        {
                            if (mTempBufferValue > 0)
                            {
                                if (mTempBufferValue >= mRealInjureValue)
                                {
                                    mTempBufferValue -= mRealInjureValue;
                                    mRealInjureValue = 0;
                                    mTempBufferData.CacheData[0] = mTempBufferValue;
                                }
                                else
                                {
                                    mRealInjureValue -= mTempBufferValue;
                                    mTempBufferValue = 0;
                                    mTempBufferData.CacheData[0] = mTempBufferValue;
                                }

                                if (mTempBufferValue == 0)
                                {
                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_BeAttacker.InstanceId;
                                    G2C_BattleKVData mData = new G2C_BattleKVData();
                                    mData.Key = (int)E_GameProperty.FangYuHuZhao;
                                    mData.Value = mTempBufferValue;
                                    mChangeValueMessage.Info.Add(mData);

                                    b_BattleComponent.Parent.SendNotice(b_BeAttacker, mChangeValueMessage);
                                }
                            }
                        }
                    }
                }
            }

            if (b_BeAttacker.HealthStatsDic.ContainsKey(E_BattleSkillStats.HunShuiShu502))
            {
                b_BeAttacker.RemoveHealthState(E_BattleSkillStats.HunShuiShu502, b_BattleComponent, true);
                b_BeAttacker.UpdateHealthState();
            }

            return (mRealInjureValue, mRealInjureSDValue);
        }
    }
}