using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{



    public static partial class EnemySystem
    {
        public static void SetConfig(this Enemy b_Component, Enemy_InfoConfig b_Config)
        {
            b_Component.Identity = E_Identity.Enemy;
            b_Component.Config = b_Config;

            if (b_Component.UnitData == null) b_Component.UnitData = new DBPlayerUnitData();

            b_Component.GetNumerialFunc = (E_GameProperty b_GameProperty) =>
            {
                return b_Component.GetNumerial(b_GameProperty);
            };
        }


        public static void AfterAwake(this Enemy b_Component)
        {
            b_Component.UnitData.Hp = b_Component.Config.HP;
            b_Component.UnitData.Mp = b_Component.Config.MP;
            b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = b_Component.Config.HP;
            b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = b_Component.Config.MP;

            b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = b_Component.Config.DmgMin;
            b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = b_Component.Config.DmgMax;

            b_Component.GamePropertyDic[E_GameProperty.Defense] = b_Component.Config.Def;

            b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = b_Component.Config.AttRate;
            b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = b_Component.Config.BloRate;

            b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = b_Component.Config.AtSpeed;
            b_Component.GamePropertyDic[E_GameProperty.MoveSpeed] = b_Component.Config.MoSpeed;
            b_Component.GamePropertyDic[E_GameProperty.AttackDistance] = b_Component.Config.AR;
            b_Component.GamePropertyDic[E_GameProperty.Level] = b_Component.Config.Lvl;


            b_Component.GamePropertyDic[E_GameProperty.InjuryValueRate_3] = 0;
            b_Component.GamePropertyDic[E_GameProperty.InjuryValueRate_2] = 0;
            b_Component.GamePropertyDic[E_GameProperty.LucklyAttackRate] = 0;
            b_Component.GamePropertyDic[E_GameProperty.ExcellentAttackRate] = 0;

            b_Component.GamePropertyDic[E_GameProperty.CurseResistance] = b_Component.Config.POI;
            b_Component.GamePropertyDic[E_GameProperty.FireResistance] = b_Component.Config.FIR;
            b_Component.GamePropertyDic[E_GameProperty.IceResistance] = b_Component.Config.ICE;
            b_Component.GamePropertyDic[E_GameProperty.ThunderResistance] = b_Component.Config.LIG;
            b_Component.GamePropertyDic[E_GameProperty.ShacklesResistanceRate] = b_Component.Config.LIG;

            if (b_Component.Config.Monster_Type == 1)
            {
                b_Component.GamePropertyDic[E_GameProperty.AttackIgnoreAbsorbRate] = 500;
                b_Component.GamePropertyDic[E_GameProperty.IgnoreAbsorbRate] = 3000;
            } 
			if (b_Component.Config.Monster_Type == 6)
            {
                b_Component.GamePropertyDic[E_GameProperty.AttackIgnoreAbsorbRate] = 500;
                b_Component.GamePropertyDic[E_GameProperty.IgnoreAbsorbRate] = 3000;
            }

            b_Component.DataAddPropertyBuffer();
        }
        public static void DataUpdate(this Enemy b_Component)
        {


        }
        public static void DataAddPropertyBuffer(this Enemy b_Component)
        {
            b_Component.SyncTaskTimerInit();
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

                    if (b_TimeTick > b_CombatSource.LastAttackTime * 1000 + 10000)
                    {
                        G2C_ChangeValue_notice mChangeValue = null;

                        var mHp = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP);
                        var mHpMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                        if (mHp < mHpMax)
                        {
                            b_CombatSource.UnitData.Hp = mHpMax;

                            if (mChangeValue == null) mChangeValue = new G2C_ChangeValue_notice();
                            G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                            mChildChangeValue.Key = (int)E_GameProperty.PROP_HP;
                            mChildChangeValue.Value = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP);
                            mChangeValue.Info.Add(mChildChangeValue);
                        }

                        if (mChangeValue != null)
                        {
                            if (b_CombatSource.Identity == E_Identity.Enemy)
                            {
                                mChangeValue.GameUserId = b_CombatSource.InstanceId;
                                C_FindTheWay2D b_Source = b_BattleComponent.Parent.GetFindTheWay2D(b_CombatSource);
                                b_BattleComponent.Parent.SendNotice(b_Source, mChangeValue);
                            }
                        }
                    }

                    return CombatSource.E_SyncTimerTaskResult.AutoNextRound;
                };
                b_Component.AddTask(mBattleSyncTimer);
            }
        }
    }
}