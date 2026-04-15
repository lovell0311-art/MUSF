using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{

    public static partial class CombatSourceSystem
    {
        public static void Start(this CombatSource self)
        {


        }
        /// <summary>
        /// 获取主人(宠物，召唤兽，以及自己)
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static GamePlayer GetMaster(this CombatSource self)
        {
            switch (self.Identity)
            {
                case E_Identity.Enemy: return null;
                case E_Identity.Summoned: return (self as Summoned).GamePlayer;
                case E_Identity.HolyteacherSummoned: return (self as HolyteacherSummoned).GamePlayer;
                case E_Identity.Pet: return (self as Pets).GamePlayer;
                case E_Identity.Hero: return self as GamePlayer;
                default: return null;
            }
        }


        public static void ChangeNumerialType(this CombatSource b_Component, E_GameProperty b_GameProperty)
        {
            if (b_Component.ChangePropertyDic.TryGetValue(b_GameProperty, out var mChangeProperty) == false)
            {
                mChangeProperty = b_Component.ChangePropertyDic[b_GameProperty] = (false, 0);
            }

            mChangeProperty.Item1 = false;
        }

        public static int ResetNumerialSource(this CombatSource b_Component, E_GameProperty b_GameProperty, bool b_HasTemporary = true)
        {
            int mResult = 0;

            if (b_Component.GamePropertyDic.TryGetValue(b_GameProperty, out var mGamePropertyValue))
            {
                mResult += mGamePropertyValue;
            }
            // 装备属性应用
            //if (b_Component.EquipPropertyDic.TryGetValue(b_GameProperty, out int mEquipPropertyValue))
            //{
            //    mResult += mEquipPropertyValue;
            //}

            // buff
            if (b_HasTemporary)
            {
                // buff
                #region buff
                switch (b_GameProperty)
                {
                    case E_GameProperty.Property_Strength:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310024, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.Property_Willpower:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310027, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.Property_Agility:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310025, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.Property_BoneGas:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310026, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.Property_Command:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310028, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.PROP_HP:
                        break;
                    case E_GameProperty.PROP_MP:
                        break;
                    case E_GameProperty.PROP_SD:
                        break;
                    case E_GameProperty.PROP_AG:
                        break;
                    case E_GameProperty.PROP_HP_MAX:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaBing310062, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }

                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShengMingZhiGuang110, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310022, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310034, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult += mTempValue;
                                    }
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult -= mTempValue;
                                        if (mResult < 0) mResult = 0;
                                    }
                                }

                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310118, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.DouShenMing608, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.PROP_MP_MAX:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaJiu310029, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310023, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.PROP_SD_MAX:
                        break;
                    case E_GameProperty.PROP_AG_MAX:
                        break;
                    case E_GameProperty.Injury_HP:
                        break;
                    case E_GameProperty.Injury_SD:
                        break;
                    case E_GameProperty.MaxAtteck:
                        {
                            // 在身上有技能buffer
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.JiGuDeBiHu115, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }

                                // 坚固的庇护精通
                                if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Skill_JianQiangDeBiHu_Master133))
                                {
                                    int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Skill_JianQiangDeBiHu_Master133];
                                    mResult += mMasteryValue;
                                }
                            }

                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ZhanShenZhiLi204, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.XuRuoZhen510, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult -= mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult += mTempValue;
                                    }
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310020, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaHuaBan310063, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310116, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangNu709, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.DouShenPo607, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.MinAtteck:
                        {
                            // 在身上有技能buffer
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.JiGuDeBiHu115, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }

                                // 坚固的庇护精通
                                if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Skill_JianQiangDeBiHu_Master133))
                                {
                                    int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Skill_JianQiangDeBiHu_Master133];
                                    mResult += mMasteryValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ZhanShenZhiLi204, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.XuRuoZhen510, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult -= mTempBufferValue;
                                }
                            }

                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult += mTempValue;
                                    }
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310020, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaHuaBan310063, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310116, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangNu709, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.DouShenPo607, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.MaxMagicAtteck:
                        {
                            // 在身上有技能buffer
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.JiGuDeBiHu115, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }

                                // 坚固的庇护精通
                                if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Skill_JianQiangDeBiHu_Master133))
                                {
                                    int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Skill_JianQiangDeBiHu_Master133];
                                    mResult += mMasteryValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310116, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ZhanShenZhiLi204, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult += mTempValue;
                                    }
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310021, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaHuaBan310063, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.MinMagicAtteck:
                        {
                            // 在身上有技能buffer
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.FaShenFuTi23, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310116, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            // 在身上有技能buffer
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.JiGuDeBiHu115, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }

                                // 坚固的庇护精通
                                if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Skill_JianQiangDeBiHu_Master133))
                                {
                                    int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Skill_JianQiangDeBiHu_Master133];
                                    mResult += mMasteryValue;
                                }
                            }

                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ZhanShenZhiLi204, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult += mTempValue;
                                    }
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310021, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaHuaBan310063, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.MaxElementAtteck:
                        break;
                    case E_GameProperty.MinElementAtteck:
                        break;
                    case E_GameProperty.AtteckSuccessRate:
                        break;
                    case E_GameProperty.PVPAtteckSuccessRate:
                        break;
                    case E_GameProperty.ElementAtteckSuccessRate:
                        break;
                    case E_GameProperty.PVPElementAtteckSuccessRate:
                        break;
                    case E_GameProperty.DefenseRate:
                        break;
                    case E_GameProperty.ElementDefenseRate:
                        break;
                    case E_GameProperty.PVPDefenseRate:
                        break;
                    case E_GameProperty.PVPElementDefenseRate:
                        break;
                    case E_GameProperty.Defense:
                        {
                            
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShouHuZhiGuang203, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.PoYuZhen511, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult -= mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310019, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult -= mTempValue;
                                        if (mResult < 0) mResult = 0;
                                    }
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310117, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangNu709, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult -= mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.DouShenYu609, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.ElementDefense:
                        break;
                    case E_GameProperty.AttackSpeed:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310018, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310127, out var mTempBuffer1))
                            {
                                if (mTempBuffer1.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.MoveSpeed:
                        break;
                    case E_GameProperty.MoveSpeed_Increase:
                        break;
                    case E_GameProperty.MoveSpeed_Reduce:
                        break;
                    case E_GameProperty.AttackDistance:
                        break;
                    case E_GameProperty.SkillAddition:
                        break;
                    case E_GameProperty.PVPAttack:
                        break;
                    case E_GameProperty.PVPDefense:
                        break;
                    case E_GameProperty.PVEAttack:
                        break;
                    case E_GameProperty.PVEDefense:
                        break;
                    case E_GameProperty.ReallyDefense:
                        break;
                    case E_GameProperty.ReplyHp:
                        break;
                    case E_GameProperty.ReplyMp:
                        break;
                    case E_GameProperty.ReplyAG:
                        break;
                    case E_GameProperty.ReplySD:
                        break;
                    case E_GameProperty.ReplyHpRate:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310034, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult += mTempValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameProperty.ReplyMpRate:
                        break;
                    case E_GameProperty.ReplyAGRate:
                        break;
                    case E_GameProperty.ReplySDRate:
                        break;
                    case E_GameProperty.KillEnemyReplyHpRate:
                        break;
                    case E_GameProperty.KillEnemyReplyMpRate:
                        break;
                    case E_GameProperty.KillEnemyReplyAGRate:
                        break;
                    case E_GameProperty.KillEnemyReplySDRate:
                        break;
                    case E_GameProperty.ReplyAllHpRate:
                        break;
                    case E_GameProperty.ReplyAllMpRate:
                        break;
                    case E_GameProperty.ReplyAllAGRate:
                        break;
                    case E_GameProperty.ReplyAllSdRate:
                        break;
                    case E_GameProperty.Injury_ReplyAllHpRate:
                        break;
                    case E_GameProperty.Injury_ReplyAllMpRate:
                        break;
                    case E_GameProperty.Attack_ReplyAllSdRate:
                        break;
                    case E_GameProperty.HpAbsorbRate:
                        break;
                    case E_GameProperty.SdAbsorbRate:
                        break;
                    case E_GameProperty.MpConsumeRate_Reduce:
                        break;
                    case E_GameProperty.AgConsumeRate_Reduce:
                        break;
                    case E_GameProperty.AttackIgnoreDefenseRate:
                        break;
                    case E_GameProperty.ReboundRate:
                        break;
                    case E_GameProperty.LucklyAttackHurtValueIncrease:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ZhiMingShengYin410, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310031, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.ExcellentAttackHurtValueIncrease:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310032, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.InjuryValueRate_Increase:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310119, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.InjuryValueRate_Reduce:
                        break;
                    case E_GameProperty.InjuryValue_Reduce:
                        break;
                    case E_GameProperty.BackInjuryRate:
                        {
                          
                        }
                        break;
                    case E_GameProperty.HurtValueAbsorbRate:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310120, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.HitSdRate:
                        break;
                    case E_GameProperty.AttackSdRate:
                        break;
                    case E_GameProperty.SDAttackIgnoreRate:
                        break;
                    case E_GameProperty.ShacklesRate:
                        break;
                    case E_GameProperty.ShacklesResistanceRate:
                        break;
                    case E_GameProperty.ShieldHurtAbsorb:
                        break;
                    case E_GameProperty.DefenseShieldRate:
                        break;
                    case E_GameProperty.AddGoldCoinRate_Increase:
                        break;
                    case E_GameProperty.MagicRate_Increase:
                        break;
                    case E_GameProperty.GridBlockRate:
                        break;
                    case E_GameProperty.GuardShieldRate:
                        break;
                    case E_GameProperty.AttackBonus:
                        break;
                    case E_GameProperty.DefenseBonus:
                        break;
                    case E_GameProperty.AttackSpeedBonus:
                        break;
                    case E_GameProperty.DefenseRateBonus:
                        break;
                    case E_GameProperty.HealthBonus:
                        break;
                    case E_GameProperty.MagicBonus:
                        break;
                    case E_GameProperty.SkillAttack:
                        break;
                    case E_GameProperty.MaxDamnationAtteck:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310021, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaHuaBan310063, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310116, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.MinDamnationAtteck:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310021, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaHuaBan310063, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310116, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.DamnationRate_Increase:
                        break;
                    case E_GameProperty.PROP_HP_MAXPct:
                        {

                        }
                        break;
                    case E_GameProperty.PROP_MP_MAXPct:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult += mTempValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameProperty.PROP_SD_MAXPct:
                        break;
                    case E_GameProperty.PROP_AG_MAXPct:
                        break;
                    case E_GameProperty.MaxAtteckPct:
                        break;
                    case E_GameProperty.MinAtteckPct:
                        break;
                    case E_GameProperty.MaxMagicAtteckPct:
                        break;
                    case E_GameProperty.MinMagicAtteckPct:
                        break;
                    case E_GameProperty.MaxElementAtteckPct:
                        break;
                    case E_GameProperty.MinElementAtteckPct:
                        break;
                    case E_GameProperty.AtteckSuccessRatePct:
                        break;
                    case E_GameProperty.PVPAtteckSuccessRatePct:
                        break;
                    case E_GameProperty.ElementAtteckSuccessRatePct:
                        break;
                    case E_GameProperty.PVPElementAtteckSuccessRatePct:
                        break;
                    case E_GameProperty.DefenseRatePct:
                        break;
                    case E_GameProperty.ElementDefenseRatePct:
                        break;
                    case E_GameProperty.PVPDefenseRatePct:
                        break;
                    case E_GameProperty.PVPElementDefenseRatePct:
                        break;
                    case E_GameProperty.DefensePct:
                        {

                        }
                        break;
                    case E_GameProperty.ElementDefensePct:
                        break;
                    case E_GameProperty.MaxDamnationAtteckPct:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult += mTempValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameProperty.MinDamnationAtteckPct:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    if (mTempBufferData.CacheData.TryGetValue((int)b_GameProperty, out var mTempValue))
                                    {
                                        mResult += mTempValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameProperty.KillMonsterReplyHp_8:
                        break;
                    case E_GameProperty.KillMonsterReplyMp_8:
                        break;
                    case E_GameProperty.DamageAbsPct_Guard:
                        break;
                    case E_GameProperty.DamageAbsPct_Mounts:
                        break;
                    case E_GameProperty.DamageAbsPct_Wing:
                        break;
                    case E_GameProperty.DamageAbsPct_Pets:
                        break;
                    case E_GameProperty.MpsDownDur1:
                        break;
                    case E_GameProperty.MpsDownDur2:
                        break;
                    case E_GameProperty.MpsDownDur3:
                        break;
                    case E_GameProperty.MpsPetDurDownSpeed:
                        break;
                    case E_GameProperty.NullAttack:
                        break;
                    case E_GameProperty.InjuryValueRate_2:
                        break;
                    case E_GameProperty.InjuryValueRate_3:
                        break;
                    case E_GameProperty.LucklyAttackRate:
                        {

                        }
                        break;
                    case E_GameProperty.ExcellentAttackRate:
                        break;
                    case E_GameProperty.IceResistance:
                        break;
                    case E_GameProperty.CurseResistance:
                        break;
                    case E_GameProperty.FireResistance:
                        break;
                    case E_GameProperty.ThunderResistance:
                        break;
                    case E_GameProperty.Level:
                        break;
                    case E_GameProperty.OccupationLevel:
                        break;
                    case E_GameProperty.UnionName:
                        break;
                    case E_GameProperty.FreePoint:
                        break;
                    case E_GameProperty.Exprience:
                        break;
                    case E_GameProperty.ExprienceDrop:
                        break;
                    case E_GameProperty.ServerTime:
                        break;
                    case E_GameProperty.GoldCoin:
                        break;
                    case E_GameProperty.GoldCoinChange:
                        break;
                    case E_GameProperty.ExperienceBonus:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseGuangZhiZhuFu310059, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            else if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseGuangZhiZhuFu310061, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                            else if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseGuangZhiZhuFu310060, out mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.ExplosionRate:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310140, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.StrengthValue;
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    case E_GameProperty.GoldCoinMarkup:
                        break;
                    case E_GameProperty.MiracleCoin:
                        break;
                    case E_GameProperty.MiracleChange:
                        break;
                    case E_GameProperty.YuanbaoCoin:
                        break;
                    case E_GameProperty.YuanbaoChange:
                        break;
                    case E_GameProperty.PlayerKillingMedel:
                        break;
                    case E_GameProperty.PkNumber:
                        break;
                    case E_GameProperty.GamePropertyMax:
                        break;
                    case E_GameProperty.FangYuHuZhao:
                        {
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var mTempBuffer))
                            {
                                if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                {
                                    var mTempBufferValue = mTempBufferData.CacheData[0];
                                    mResult += mTempBufferValue;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                #endregion
            }

            switch (b_Component.Identity)
            {
                case E_Identity.Hero:
                    break;
                case E_Identity.Enemy:
                    break;
                case E_Identity.Npc:
                    break;
                case E_Identity.Pet:
                    break;
                case E_Identity.Summoned:
                    break;
                case E_Identity.HolyteacherSummoned:
                    {
                        // 二级属性应用
                        switch (b_GameProperty)
                        {
                            case E_GameProperty.MaxAtteckPct:
                            case E_GameProperty.MinAtteckPct:
                                {
                                    HolyteacherSummoned mInstance = b_Component as HolyteacherSummoned;
                                    if (mInstance != null)
                                    {
                                        int pct = mInstance.GamePlayer.GetNumerial(E_GameProperty.AttackBonus2);
                                        if (mResult != 0 && pct != 0)
                                        {
                                            pct += 100;
                                            mResult = (int)(mResult * pct * 0.01f);
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }

            // 限制 上限或者后计算
            // 修正
            switch (b_GameProperty)
            {
                case E_GameProperty.MinAtteck:
                    {
                        var mMaxAtteck = b_Component.GetNumerialFunc(E_GameProperty.MaxAtteck);
                        if (mResult > mMaxAtteck)
                        {
                            mResult = mMaxAtteck;
                        }
                    }
                    break;
                case E_GameProperty.MinMagicAtteck:
                    {
                        var mMaxMagicAtteck = b_Component.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);
                        if (mResult > mMaxMagicAtteck)
                        {
                            mResult = mMaxMagicAtteck;
                        }
                    }
                    break;
                case E_GameProperty.MinElementAtteck:
                    {
                        var mMaxElementAtteck = b_Component.GetNumerialFunc(E_GameProperty.MaxElementAtteck);
                        if (mResult > mMaxElementAtteck)
                        {
                            mResult = mMaxElementAtteck;
                        }
                    }
                    break;
                case E_GameProperty.Defense:
                    {
                        // 在身上有技能buffer 魔剑士技能玄月斩 1-90%防御力
                        if (b_Component.HealthStatsDic.ContainsKey(E_BattleSkillStats.XuanYueZhan321))
                        {
                            int mTempBufferValue = 0;
                            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.XuanYueZhan321, out var mTempBuffer))
                            {
                                mTempBufferValue = 90;
                                //if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                //{
                                //    mTempBufferValue = mTempBufferData.StrengthValue;
                                //}
                            }

                            // 玄月斩精通 玄月斩技能使用时防御力减少效果提高{0:F}%
                            if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Skill_XuanYueZhanMaster337))
                            {
                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Skill_XuanYueZhanMaster337];
                                mTempBufferValue += mMasteryValue;
                            }

                            mResult = (int)(mResult * (100 - mTempBufferValue) * 0.01f);
                        }
                    }
                    break;
                default:
                    break;
            }



            return mResult;
        }
    }
}