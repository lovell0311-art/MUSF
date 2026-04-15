
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [GameMasterCommandLine("修改属性")]
    public class GMCommandLine_修改属性 : C_GameMasterCommandLine<Player, IResponse>
    {
        public override async Task Run(Player b_Player, List<int> b_Parameter, IResponse b_Response)
        {
            if (b_Parameter.Count < 2)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();

            E_GameProperty mGameProperty = (E_GameProperty)b_Parameter[0];
            var mParameter = b_Parameter[1];

            switch (mGameProperty)
            {
                case E_GameProperty.PROP_HP:
                    {
                        mGamePlayer.UnitData.Hp = mParameter;

                        G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                        mChangeValue_notice.GameUserId = mGamePlayer.InstanceId;
                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                        mBattleKVData.Key = (int)mGameProperty;
                        mBattleKVData.Value = mGamePlayer.GetNumerial(mGameProperty);
                        mChangeValue_notice.Info.Add(mBattleKVData);
                        b_Player.Send(mChangeValue_notice);
                    }
                    break;
                case E_GameProperty.PROP_MP:
                    {
                        mGamePlayer.UnitData.Mp = mParameter;

                        G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                        mChangeValue_notice.GameUserId = mGamePlayer.InstanceId;
                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                        mBattleKVData.Key = (int)mGameProperty;
                        mBattleKVData.Value = mGamePlayer.GetNumerial(mGameProperty);
                        mChangeValue_notice.Info.Add(mBattleKVData);
                        b_Player.Send(mChangeValue_notice);
                    }
                    break;
                case E_GameProperty.PROP_SD:
                    {
                        mGamePlayer.UnitData.SD = mParameter;

                        G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                        mChangeValue_notice.GameUserId = mGamePlayer.InstanceId;
                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                        mBattleKVData.Key = (int)mGameProperty;
                        mBattleKVData.Value = mGamePlayer.GetNumerial(mGameProperty);
                        mChangeValue_notice.Info.Add(mBattleKVData);
                        b_Player.Send(mChangeValue_notice);
                    }
                    break;
                case E_GameProperty.PROP_AG:
                    {
                        mGamePlayer.UnitData.AG = mParameter;

                        G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                        mChangeValue_notice.GameUserId = mGamePlayer.InstanceId;
                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                        mBattleKVData.Key = (int)mGameProperty;
                        mBattleKVData.Value = mGamePlayer.GetNumerial(mGameProperty);
                        mChangeValue_notice.Info.Add(mBattleKVData);
                        b_Player.Send(mChangeValue_notice);
                    }
                    break;
                case E_GameProperty.Property_Strength:
                case E_GameProperty.Property_Willpower:
                case E_GameProperty.Property_Agility:
                case E_GameProperty.Property_BoneGas:
                case E_GameProperty.Property_Command:
                case E_GameProperty.PROP_HP_MAX:
                case E_GameProperty.PROP_MP_MAX:
                case E_GameProperty.PROP_SD_MAX:
                case E_GameProperty.PROP_AG_MAX:
                case E_GameProperty.Injury_HP:
                case E_GameProperty.Injury_SD:
                case E_GameProperty.MaxAtteck:
                case E_GameProperty.MinAtteck:
                case E_GameProperty.MaxMagicAtteck:
                case E_GameProperty.MinMagicAtteck:
                case E_GameProperty.MaxElementAtteck:
                case E_GameProperty.MinElementAtteck:
                case E_GameProperty.AtteckSuccessRate:
                case E_GameProperty.PVPAtteckSuccessRate:
                case E_GameProperty.ElementAtteckSuccessRate:
                case E_GameProperty.PVPElementAtteckSuccessRate:
                case E_GameProperty.DefenseRate:
                case E_GameProperty.ElementDefenseRate:
                case E_GameProperty.PVPDefenseRate:
                case E_GameProperty.PVPElementDefenseRate:
                case E_GameProperty.Defense:
                case E_GameProperty.ElementDefense:
                case E_GameProperty.AttackSpeed:
                case E_GameProperty.MoveSpeed:
                case E_GameProperty.MoveSpeed_Increase:
                case E_GameProperty.MoveSpeed_Reduce:
                case E_GameProperty.AttackDistance:
                case E_GameProperty.SkillAddition:
                case E_GameProperty.PVPAttack:
                case E_GameProperty.PVPDefense:
                case E_GameProperty.PVEAttack:
                case E_GameProperty.PVEDefense:
                case E_GameProperty.ReallyDefense:
                case E_GameProperty.ReplyHp:
                case E_GameProperty.ReplyMp:
                case E_GameProperty.ReplyAG:
                case E_GameProperty.ReplySD:
                case E_GameProperty.ReplyHpRate:
                case E_GameProperty.ReplyMpRate:
                case E_GameProperty.ReplyAGRate:
                case E_GameProperty.ReplySDRate:
                case E_GameProperty.KillEnemyReplyHpRate:
                case E_GameProperty.KillEnemyReplyMpRate:
                case E_GameProperty.KillEnemyReplyAGRate:
                case E_GameProperty.KillEnemyReplySDRate:
                case E_GameProperty.ReplyAllHpRate:
                case E_GameProperty.ReplyAllMpRate:
                case E_GameProperty.ReplyAllAGRate:
                case E_GameProperty.ReplyAllSdRate:
                case E_GameProperty.Injury_ReplyAllHpRate:
                case E_GameProperty.Injury_ReplyAllMpRate:
                case E_GameProperty.Attack_ReplyAllSdRate:
                case E_GameProperty.HpAbsorbRate:
                case E_GameProperty.SdAbsorbRate:
                case E_GameProperty.MpConsumeRate_Reduce:
                case E_GameProperty.AgConsumeRate_Reduce:
                case E_GameProperty.AttackIgnoreDefenseRate:
                case E_GameProperty.ReboundRate:
                case E_GameProperty.LucklyAttackHurtValueIncrease:
                case E_GameProperty.ExcellentAttackHurtValueIncrease:
                case E_GameProperty.InjuryValueRate_Increase:
                case E_GameProperty.InjuryValueRate_Reduce:
                case E_GameProperty.InjuryValue_Reduce:
                case E_GameProperty.BackInjuryRate:
                case E_GameProperty.HurtValueAbsorbRate:
                case E_GameProperty.HitSdRate:
                case E_GameProperty.AttackSdRate:
                case E_GameProperty.SDAttackIgnoreRate:
                case E_GameProperty.ShacklesRate:
                case E_GameProperty.ShacklesResistanceRate:
                case E_GameProperty.ShieldHurtAbsorb:
                case E_GameProperty.DefenseShieldRate:
                case E_GameProperty.AddGoldCoinRate_Increase:
                case E_GameProperty.MagicRate_Increase:
                case E_GameProperty.GridBlockRate:
                case E_GameProperty.GuardShieldRate:
                case E_GameProperty.AttackBonus:
                case E_GameProperty.DefenseBonus:
                case E_GameProperty.AttackSpeedBonus:
                case E_GameProperty.DefenseRateBonus:
                case E_GameProperty.HealthBonus:
                case E_GameProperty.MagicBonus:
                case E_GameProperty.SkillAttack:
                case E_GameProperty.MaxDamnationAtteck:
                case E_GameProperty.MinDamnationAtteck:
                case E_GameProperty.DamnationRate_Increase:
                case E_GameProperty.NullAttack:
                case E_GameProperty.InjuryValueRate_2:
                case E_GameProperty.InjuryValueRate_3:
                case E_GameProperty.LucklyAttackRate:
                case E_GameProperty.ExcellentAttackRate:
                case E_GameProperty.IceResistance:
                case E_GameProperty.CurseResistance:
                case E_GameProperty.FireResistance:
                case E_GameProperty.ThunderResistance:
                case E_GameProperty.Level:
                case E_GameProperty.OccupationLevel:
                case E_GameProperty.FreePoint:
                case E_GameProperty.Exprience:
                case E_GameProperty.ExprienceDrop:
                    {
                        mGamePlayer.GamePropertyDic[mGameProperty] = mParameter;

                        G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                        mChangeValue_notice.GameUserId = mGamePlayer.InstanceId;
                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                        mBattleKVData.Key = (int)mGameProperty;
                        mBattleKVData.Value = mGamePlayer.GetNumerial(mGameProperty);
                        mChangeValue_notice.Info.Add(mBattleKVData);
                        b_Player.Send(mChangeValue_notice);
                    }
                    break;
                default:
                    {
                        b_Response.Error = 99;
                        b_Response.Message = "参数属性无实现";
                        return;
                    }
                    break;
            }
        }
    }
}