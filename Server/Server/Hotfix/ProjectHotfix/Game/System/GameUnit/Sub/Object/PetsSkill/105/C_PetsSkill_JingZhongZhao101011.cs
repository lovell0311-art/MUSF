
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CustomFrameWork;
using ETModel;
using UnityEngine;
using static ETModel.CombatSource;

namespace ETHotfix
{
    /// <summary>
    /// 月神祝福
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_Pets_SKILL_Id.JinZhongZhao)]
    public partial class C_PetsSkill_JinZhongZhao101011 : C_HeroSkillSource
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
    /// 月神祝福
    /// </summary>
    public partial class C_PetsSkill_JinZhongZhao101011
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (!(Config is Pets_SkillConfig mConfig))
            {
                //b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(425);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("技能配置数据异常!");
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
                                    //b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(408);
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
                return false;
            }

            if (mConfig.skillType == 2) return true;

            return TryUseByUseStandard(b_Attacker, b_Response);
        }
        public override bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent)
        {
            if (!(Config is Pets_SkillConfig mConfig))
            {
                return false;
            }
            //角色buff在DataAddPropertyBufferGotoMap增加这里不处理
            //宠物buff在DataAddPropertyBuffer增加这里不在处理

            if (b_BattleComponent == null)
            {
                return false;
            }

            if ((b_Attacker as Pets).dBPetsData.PetsUseState == 0)
            {
                return false;
            }

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

                    switch (b_CombatSource.Identity)
                    {
                        case E_Identity.Hero:
                            {
                                if (((b_CombatSource as GamePlayer).Pets).dBPetsData.PetsUseState == 0)
                                {
                                    return CombatSource.E_SyncTimerTaskResult.Dispose;
                                }
                            }
                            break;
                        case E_Identity.Pet:
                            {
                                if ((b_CombatSource as Pets).dBPetsData.PetsUseState == 0)
                                {
                                    return CombatSource.E_SyncTimerTaskResult.Dispose;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var hp_Curse) == false)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }

                    if (hp_Curse.CacheDatas.TryGetValue(0, out var hpCacheDatas) == false)
                    {
                        hp_Curse.CacheDatas[0] = new CombatSource.C_CombatUnitStatsSource();
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

            b_Attacker.AddHealthState(E_BattleSkillStats.FangYuHuZhao, 5, 0, 0, mCreateFunc, b_BattleComponent);
            b_Attacker.UpdateHealthState();
            {
                if (b_Attacker.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var hp_Curse))
                {
                    if (hp_Curse.CacheDatas.TryGetValue(0, out var hpCacheDatas) == false)
                    {
                        hp_Curse.CacheDatas[0] = new C_CombatUnitStatsSource();
                    }
                    if (hpCacheDatas.CacheData == null) hpCacheDatas.CacheData = new Dictionary<int, int>();

                    var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    mMax = (int)(mMax * 0.1f);
                    hpCacheDatas.CacheData[0] = mMax;
                }
            }


            (b_Attacker as Pets).GamePlayer.AddHealthState(E_BattleSkillStats.FangYuHuZhao, 5, 0, 0, mCreateFunc, b_BattleComponent);
            (b_Attacker as Pets).GamePlayer.UpdateHealthState();
            {
                if ((b_Attacker as Pets).GamePlayer.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var hp_Curse))
                {
                    if (hp_Curse.CacheDatas.TryGetValue(0, out var hpCacheDatas) == false)
                    {
                        hp_Curse.CacheDatas[0] = new C_CombatUnitStatsSource();
                    }
                    if (hpCacheDatas.CacheData == null) hpCacheDatas.CacheData = new Dictionary<int, int>();

                    var mMax = (b_Attacker as Pets).GamePlayer.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    mMax = (int)(mMax * 0.1f);
                    hpCacheDatas.CacheData[0] = mMax;
                }
            }
            return true;
        }
    }
}
