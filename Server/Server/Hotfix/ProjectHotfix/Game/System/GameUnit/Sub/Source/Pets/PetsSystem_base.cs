using System;
using System.Threading.Tasks;
using ETModel;
using System.Collections.Generic;
using CustomFrameWork.Component;
using CustomFrameWork;

namespace ETHotfix
{



    public static partial class PetsSystem
    {
        public static void SetConfig(this Pets b_Component, DBPetsData dBPetsData, Pets_InfoConfig b_Config)
        {
            b_Component.Identity = E_Identity.Pet;
            b_Component.Config = b_Config;

            b_Component.dBPetsData = dBPetsData;
            if (b_Component.UnitData == null) b_Component.UnitData = new DBPlayerUnitData();

            b_Component.GetNumerialFunc = (E_GameProperty b_GameProperty) =>
            {
                return b_Component.GetNumerial(b_GameProperty);
            };
        }


        public static void AfterAwake(this Pets b_Component)
        {
            b_Component.GamePropertyDic[E_GameProperty.ReplyHpRate] = 100;
            b_Component.GamePropertyDic[E_GameProperty.ReplySDRate] = 100;
            b_Component.GamePropertyDic[E_GameProperty.ReplyMpRate] = 300;
            b_Component.GamePropertyDic[E_GameProperty.ReplyAGRate] = 300;

            b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = 0;
            b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = 0;

            b_Component.GamePropertyDic[E_GameProperty.ExcellentAttackRate] = 0;
            b_Component.GamePropertyDic[E_GameProperty.AttackBonus] = 0;
            b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = 0;
            b_Component.GamePropertyDic[E_GameProperty.KillMonsterReplyHp_8] = 0;
            b_Component.GamePropertyDic[E_GameProperty.KillMonsterReplyMp_8] = 0;
            b_Component.GamePropertyDic[E_GameProperty.HealthBonus] = 0;
            b_Component.GamePropertyDic[E_GameProperty.MagicBonus] = 0;
            b_Component.GamePropertyDic[E_GameProperty.InjuryValueRate_Reduce] = 0;
            b_Component.GamePropertyDic[E_GameProperty.BackInjuryRate] = 0;
            b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = 0;
            b_Component.GamePropertyDic[E_GameProperty.AddGoldCoinRate_Increase] = 0;


        }
        public static void DataUpdate(this Pets b_Component)
        {


        }
        public static void UpDataEnhanceA(this Pets b_Component, int OldEnhanceLv = 0)
        {
            if (b_Component != null)
            {
                int NewEnhanceLv = b_Component.dBPetsData.EnhanceLv;
                //{ 如果数值复杂复杂需要调用配置表
                //  var EnhanceAttribute = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_EnhanceAttributeConfigJson>().JsonDic;
                //  var Info = EnhanceAttribute[NewEnhanceLv];
                //  int ValueA = Info.EnhanceAttack;
                //  int ValueB = Info.EnhanceHP;
                //  int ValueC = Info.EnhanceDEF;
                //}
                if (b_Component.Config.AttackType == 1)//魔法
                {
                    b_Component.dBPetsData.PintAdd = (int)(b_Component.dBPetsData.PetsPINT * (NewEnhanceLv * 0.01f));
                }
                else if (b_Component.Config.AttackType == 0)//物理
                {
                    if (b_Component.GamePropertyDic.TryGetValue(E_GameProperty.AttackBonus, out var Value))
                    {
                        b_Component.GamePropertyDic[E_GameProperty.AttackBonus] -= OldEnhanceLv;
                        b_Component.GamePropertyDic[E_GameProperty.AttackBonus] += NewEnhanceLv;
                    }
                    else
                        b_Component.GamePropertyDic[E_GameProperty.AttackBonus] = NewEnhanceLv;

                    b_Component.ChangeNumerialType(E_GameProperty.AttackBonus);
                }

                if (b_Component.GamePropertyDic.TryGetValue(E_GameProperty.DefenseBonus, out var Value1))
                {
                    b_Component.GamePropertyDic[E_GameProperty.DefenseBonus] -= OldEnhanceLv;
                    b_Component.GamePropertyDic[E_GameProperty.DefenseBonus] += NewEnhanceLv;
                }
                else
                    b_Component.GamePropertyDic[E_GameProperty.DefenseBonus] = NewEnhanceLv;

                b_Component.ChangeNumerialType(E_GameProperty.DefenseBonus);

                if (b_Component.GamePropertyDic.TryGetValue(E_GameProperty.HealthBonus, out var Value2))
                {
                    b_Component.GamePropertyDic[E_GameProperty.HealthBonus] -= OldEnhanceLv;
                    b_Component.GamePropertyDic[E_GameProperty.HealthBonus] += NewEnhanceLv;
                }
                else
                    b_Component.GamePropertyDic[E_GameProperty.HealthBonus] = NewEnhanceLv;

                b_Component.ChangeNumerialType(E_GameProperty.HealthBonus);
            }
        }

        /// <summary>
        /// 宠物更新属性 Type{1:等级2:体力3:力量4:敏捷5:智力}其他类型属性都计算
        /// </summary>
        /// <param name="b_Component"></param>
        public static void DataUpdateProperty(this Pets b_Component, int Type = 0)
        {
            if (b_Component != null)
            {

                if (b_Component.dBPetsData.PetsHP == 0)
                    b_Component.IsDeath = false;

                float AValue_LV = b_Component.dBPetsData.PetsLevel;
                float BValue_PB = b_Component.dBPetsData.PetsPSTR;
                float CValue_PS = b_Component.dBPetsData.PetsSTR;
                float DValue_PW = b_Component.dBPetsData.PetsPINT + b_Component.dBPetsData.PintAdd;
                float EValue_PA = b_Component.dBPetsData.PetsDEX;
                if (b_Component.GameHeroSexType == E_GameHeroSexType.BOY)
                {
                    switch (Type)
                    {
                        case 1:
                            b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = (int)((33 + AValue_LV * 4 + BValue_PB * 6) * (1 + b_Component.GetNumerialFunc(E_GameProperty.HealthBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = (int)((10 + (AValue_LV - 1) * 0.5 + DValue_PW * 1) * (1 + b_Component.GetNumerialFunc(E_GameProperty.MagicBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = (int)(AValue_LV * 5 + EValue_PA * 1.5 + CValue_PS / 4);
                            b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = (int)(AValue_LV * 2 + EValue_PA / 2);
                            b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = (int)((AValue_LV * 3) + (EValue_PA / 4.5f));
                            break;
                        case 2:
                            b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = (int)((33 + AValue_LV * 4 + BValue_PB * 6) * (1 + b_Component.GetNumerialFunc(E_GameProperty.HealthBonus) * 0.01f));
                            break;
                        case 3:
                            b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = (int)(CValue_PS / 2 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = (int)(CValue_PS / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = (int)(AValue_LV * 5 + EValue_PA * 1.5 + CValue_PS / 4);
                            break;
                        case 4:
                            b_Component.GamePropertyDic[E_GameProperty.Defense] = (int)(EValue_PA / 2 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = (int)(AValue_LV * 5 + EValue_PA * 1.5 + CValue_PS / 4);
                            b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = (int)(EValue_PA / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseRateBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = (int)(EValue_PA / 15 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackSpeedBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = (int)(AValue_LV * 2 + EValue_PA / 2);
                            b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = (int)((AValue_LV * 3) + (EValue_PA / 4.5f));
                            break;
                        case 5:
                            b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = (int)((10 + (AValue_LV - 1) * 0.5 + DValue_PW * 1) * (1 + b_Component.GetNumerialFunc(E_GameProperty.MagicBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.SkillAttack] = (int)(2 + DValue_PW * 0.001f);
                            break;
                        default:
                            b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = (int)((33 + AValue_LV * 4 + BValue_PB * 6) * (1 + b_Component.GetNumerialFunc(E_GameProperty.HealthBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = (int)((10 + (AValue_LV - 1) * 0.5 + DValue_PW * 1) * (1 + b_Component.GetNumerialFunc(E_GameProperty.MagicBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = (int)(CValue_PS / 2 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = (int)(CValue_PS / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.Defense] = (int)(EValue_PA / 2 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = (int)(AValue_LV * 5 + EValue_PA * 1.5 + CValue_PS / 4);
                            b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = (int)(EValue_PA / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseRateBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = (int)(EValue_PA / 15 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackSpeedBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = (int)(AValue_LV * 2 + EValue_PA / 2);
                            b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = (int)((AValue_LV * 3) + (EValue_PA / 4.5f));
                            b_Component.GamePropertyDic[E_GameProperty.SkillAttack] = (int)(2 + DValue_PW * 0.001f);
                            break;
                    }
                }
                else
                {
                    switch (Type)
                    {
                        case 1:
                            b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = (int)((44 + AValue_LV * 3 + BValue_PB * 4) * (1 + b_Component.GetNumerialFunc(E_GameProperty.HealthBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = (int)(((AValue_LV - 1) * 2 + DValue_PW * 2) * (1 + b_Component.GetNumerialFunc(E_GameProperty.MagicBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = (int)(AValue_LV * 5 + EValue_PA * 1.5 + CValue_PS / 4);
                            b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = (int)(AValue_LV * 2 + EValue_PA / 4);
                            b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = (int)((AValue_LV * 2) + (EValue_PA / 4));
                            break;
                        case 2:
                            b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = (int)((44 + AValue_LV * 3 + BValue_PB * 4) * (1 + b_Component.GetNumerialFunc(E_GameProperty.HealthBonus) * 0.01f));
                            break;
                        case 3:
                            b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = (int)(CValue_PS / 2 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = (int)(CValue_PS / 4 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = (int)(AValue_LV * 5 + EValue_PA * 1.5 + CValue_PS / 4);
                            b_Component.GamePropertyDic[E_GameProperty.SkillAttack] = (int)(2 + CValue_PS * 0.001f);
                            break;
                        case 4:
                            b_Component.GamePropertyDic[E_GameProperty.Defense] = (int)(EValue_PA / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = (int)(AValue_LV * 5 + EValue_PA * 1.5 + CValue_PS / 4);
                            b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = (int)(EValue_PA / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseRateBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = (int)(EValue_PA / 10 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackSpeedBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = (int)(AValue_LV * 2 + EValue_PA / 4);
                            b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = (int)((AValue_LV * 2) + (EValue_PA / 4));
                            break;
                        case 5:
                            b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = (int)(((AValue_LV - 1) * 2 + DValue_PW * 2) * (1 + b_Component.GetNumerialFunc(E_GameProperty.MagicBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = (int)(DValue_PW / 2);
                            b_Component.GamePropertyDic[E_GameProperty.MinMagicAtteck] = (int)(DValue_PW / 4);
                            break;
                        default:
                            b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = (int)((44 + AValue_LV * 3 + BValue_PB * 4) * (1 + b_Component.GetNumerialFunc(E_GameProperty.HealthBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = (int)(((AValue_LV - 1) * 2 + DValue_PW * 2) * (1 + b_Component.GetNumerialFunc(E_GameProperty.MagicBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = (int)(CValue_PS / 2 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = (int)(CValue_PS / 4 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.Defense] = (int)(EValue_PA / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = (int)(AValue_LV * 5 + EValue_PA * 1.5 + CValue_PS / 4);
                            b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = (int)(EValue_PA / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseRateBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = (int)(EValue_PA / 10 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackSpeedBonus) * 0.01f));
                            b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = (int)(AValue_LV * 2 + EValue_PA / 4);
                            b_Component.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = (int)(DValue_PW / 2);
                            b_Component.GamePropertyDic[E_GameProperty.MinMagicAtteck] = (int)(DValue_PW / 4);
                            b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = (int)((AValue_LV * 2) + (EValue_PA / 4));
                            b_Component.GamePropertyDic[E_GameProperty.SkillAttack] = (int)(2 + CValue_PS * 0.001f);
                            break;
                    }
                }
                /*if (b_Component.GameHeroSexType == E_GameHeroSexType.BOY)
                {
                    b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = (int)((33 + AValue_LV * 2 + BValue_PB * 3) * (1 + b_Component.GetNumerialFunc(E_GameProperty.HealthBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = (int)((10 + (AValue_LV - 1) * 0.5 + DValue_PW * 1) * (1 + b_Component.GetNumerialFunc(E_GameProperty.MagicBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = (int)(CValue_PS / 4 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = (int)(CValue_PS / 6 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.Defense] = (int)(EValue_PA / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = (int)(AValue_LV * 5 + EValue_PA * 1.5 + CValue_PS / 4);
                    b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = (int)(EValue_PA / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseRateBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = (int)(EValue_PA / 15 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackSpeedBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = (AValue_LV * 2) + (EValue_PA / 2);
                    b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = (int)((AValue_LV * 3) + (EValue_PA / 4.5f));
                    b_Component.GamePropertyDic[E_GameProperty.SkillAttack] = (int)(2 + DValue_PW * 0.001f);
                }
                else
                {
                    b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = (int)((44 + AValue_LV * 1 + BValue_PB * 1) * (1 + b_Component.GetNumerialFunc(E_GameProperty.HealthBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = (int)(((AValue_LV - 1) * 2 + DValue_PW * 2) * (1 + b_Component.GetNumerialFunc(E_GameProperty.MagicBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = (int)(CValue_PS / 4 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = (int)(CValue_PS / 8 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.Defense] = (int)(EValue_PA / 4 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = (int)(AValue_LV * 5 + EValue_PA * 1.5 + CValue_PS / 4);
                    b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = (int)(EValue_PA / 3 * (1 + b_Component.GetNumerialFunc(E_GameProperty.DefenseRateBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = (int)(EValue_PA / 10 * (1 + b_Component.GetNumerialFunc(E_GameProperty.AttackSpeedBonus) * 0.01f));
                    b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = (AValue_LV * 2) + (EValue_PA / 4);
                    b_Component.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = (int)(DValue_PW / 4);
                    b_Component.GamePropertyDic[E_GameProperty.MinMagicAtteck] = (int)(DValue_PW / 9);
                    b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = (int)((AValue_LV * 2) + (EValue_PA / 4));
                }*/
            }
        }
        public static void DataAddPropertyBuffer(this Pets b_Component)
        {
            b_Component.SyncTaskTimerInit();
            {
                var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                //mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateUnitId(b_ZoneId);
                mBattleSyncTimer.SyncWaitTime = 7 * 1000;
                mBattleSyncTimer.NextWaitTime = CustomFrameWork.Help_TimeHelper.GetNow() + mBattleSyncTimer.SyncWaitTime;
                mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                {
                    if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    if (b_CombatSource.IsDeath) return CombatSource.E_SyncTimerTaskResult.AutoNextRound;
                    var b_CombatSourceAS = b_CombatSource as Pets;
                    G2C_ChangeValue_notice mChangeValue = null;

                    var mHp = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP);
                    var mHpMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    if (mHp < mHpMax)
                    {
                        var mReplyHpRate = b_CombatSource.GetNumerialFunc(E_GameProperty.ReplyHpRate);
                        var mReplyHp = mHpMax * mReplyHpRate * 0.0001f;
                        b_CombatSourceAS.dBPetsData.PetsHP += (int)mReplyHp;
                        if (b_CombatSourceAS.dBPetsData.PetsHP >= mHpMax) b_CombatSourceAS.dBPetsData.PetsHP = mHpMax;

                        if (mChangeValue == null) mChangeValue = new G2C_ChangeValue_notice();
                        G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.PROP_HP;
                        mChildChangeValue.Value = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP);
                        mChangeValue.Info.Add(mChildChangeValue);
                    }

                    var mSD = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_SD);
                    var mSDMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_SD_MAX);
                    if (mSD < mSDMax)
                    {
                        var mReplySDRate = b_CombatSource.GetNumerialFunc(E_GameProperty.ReplySDRate);
                        var mReplySD = mSDMax * mReplySDRate * 0.0001f;
                        b_CombatSource.UnitData.SD += (int)mReplySD;
                        if (b_CombatSource.UnitData.SD > mSDMax) b_CombatSource.UnitData.SD = mSDMax;

                        if (mChangeValue == null) mChangeValue = new G2C_ChangeValue_notice();
                        G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.PROP_SD;
                        mChildChangeValue.Value = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_SD);
                        mChangeValue.Info.Add(mChildChangeValue);
                    }

                    if (mChangeValue != null)
                    {
                        if (b_CombatSource.Identity == E_Identity.Pet)
                        {
                            mChangeValue.GameUserId = b_CombatSource.InstanceId;
                            (b_CombatSource as Pets).GamePlayer.Player.Send(mChangeValue);
                        }
                    }

                    return CombatSource.E_SyncTimerTaskResult.AutoNextRound;
                };
                b_Component.AddTask(mBattleSyncTimer);
            }
            {
                var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                //mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateUnitId(b_ZoneId);
                mBattleSyncTimer.SyncWaitTime = 3 * 1000;
                mBattleSyncTimer.NextWaitTime = CustomFrameWork.Help_TimeHelper.GetNow() + mBattleSyncTimer.SyncWaitTime;
                mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                {
                    if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    if (b_CombatSource.IsDeath) return CombatSource.E_SyncTimerTaskResult.AutoNextRound;
                    var b_CombatSourceAS = b_CombatSource as Pets;
                    G2C_ChangeValue_notice mChangeValue = null;

                    var mMP = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP);
                    var mMpMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                    if (mMP < mMpMax)
                    {
                        var mReplyMpRate = b_CombatSource.GetNumerialFunc(E_GameProperty.ReplyMpRate);
                        var mReplyMp = mMpMax * mReplyMpRate * 0.0001f;
                        b_CombatSourceAS.dBPetsData.PetsMP += (int)mReplyMp;
                        if (b_CombatSourceAS.dBPetsData.PetsMP >= mMpMax) b_CombatSourceAS.dBPetsData.PetsMP = mMpMax;

                        if (mChangeValue == null) mChangeValue = new G2C_ChangeValue_notice();
                        G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.PROP_MP;
                        mChildChangeValue.Value = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP);
                        mChangeValue.Info.Add(mChildChangeValue);
                    }

                    var mAG = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_AG);
                    var mAGMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_AG_MAX);
                    if (mAG < mAGMax)
                    {
                        var mReplyAGRate = b_CombatSource.GetNumerialFunc(E_GameProperty.ReplyAGRate);
                        var mReplyAG = mAGMax * mReplyAGRate * 0.0001f;
                        b_CombatSource.UnitData.AG += (int)mReplyAG;
                        if (b_CombatSource.UnitData.AG > mAGMax) b_CombatSource.UnitData.AG = mAGMax;

                        if (mChangeValue == null) mChangeValue = new G2C_ChangeValue_notice();
                        G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.PROP_AG;
                        mChildChangeValue.Value = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_AG);
                        mChangeValue.Info.Add(mChildChangeValue);
                    }

                    if (mChangeValue != null)
                    {
                        if (b_CombatSource.Identity == E_Identity.Pet)
                        {
                            mChangeValue.GameUserId = b_CombatSource.InstanceId;
                            (b_CombatSource as Pets).GamePlayer.Player.Send(mChangeValue);
                        }
                    }

                    return CombatSource.E_SyncTimerTaskResult.AutoNextRound;
                };
                b_Component.AddTask(mBattleSyncTimer);
            }
        }
        public static void DataAddPropertyBufferGotoMap(this Pets b_Component, BattleComponent b_BattleComponent)
        {
            b_Component.DataAddPropertyBuffer();

            if (b_Component.dBPetsData.ConfigID == 105)
            {
                Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                {
                    var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                    mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.DeathRemoveTask;
                    mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                    mBattleSyncTimer.SyncWaitTime = 2 * 60 * 1000;
                    mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                    mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                    {
                        if (b_CombatSource.IsDisposeable) return;

                        b_CombatSource.RemoveHealthState(E_BattleSkillStats.FangYuHuZhao, b_BattleComponent);
                        b_CombatSource.UpdateHealthState();
                    };
                    mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                    {
                        if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                        if ((b_CombatSource as Pets).dBPetsData.PetsUseState == 0)
                        {
                            return CombatSource.E_SyncTimerTaskResult.Dispose;
                        }

                        if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var mCurrentHealthStats) == false)
                        {
                            return CombatSource.E_SyncTimerTaskResult.Dispose;
                        }

                        if (mCurrentHealthStats.CacheDatas.TryGetValue(0, out var hpCacheDatas) == false)
                        {
                            mCurrentHealthStats.CacheDatas[0] = new CombatSource.C_CombatUnitStatsSource();
                        }
                        if (hpCacheDatas.CacheData == null) hpCacheDatas.CacheData = new Dictionary<int, int>();

                        var mMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                        mMax = (int)(mMax * 0.1f);

                        hpCacheDatas.CacheData.TryGetValue(0, out var mTempValue);
                        if (mTempValue != mMax)
                        {
                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;
                            G2C_BattleKVData mData = new G2C_BattleKVData();
                            mData.Key = (int)E_GameProperty.FangYuHuZhao;
                            mData.Value = mMax;
                            mChangeValueMessage.Info.Add(mData);

                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                        }
                        hpCacheDatas.CacheData[0] = mMax;

                        b_TimerTask.NextWaitTime = b_TimerTask.NextWaitTime + b_TimerTask.SyncWaitTime;
                        b_CombatSource.AddTask(b_TimerTask);

                        return CombatSource.E_SyncTimerTaskResult.NextRound;
                    };
                    return mBattleSyncTimer;
                };
                b_Component.AddHealthState(E_BattleSkillStats.FangYuHuZhao, 0, 0, 0, mCreateFunc, b_BattleComponent);
                b_Component.UpdateHealthState();
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var hp_Curse))
                {
                    if (hp_Curse.CacheDatas.TryGetValue(0, out var hpCacheDatas) == false)
                    {
                        hp_Curse.CacheDatas[0] = new CombatSource.C_CombatUnitStatsSource();
                    }
                    if (hpCacheDatas.CacheData == null) hpCacheDatas.CacheData = new Dictionary<int, int>();

                    var mMax = b_Component.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    mMax = (int)(mMax * 0.1f);
                    hpCacheDatas.CacheData[0] = mMax;
                }
            }
        }
    }
}