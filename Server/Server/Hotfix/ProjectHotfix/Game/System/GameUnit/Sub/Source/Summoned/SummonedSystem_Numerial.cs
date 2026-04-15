using System;
using System.Threading.Tasks;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static partial class SummonedSystem
    {
        public static int GetNumerial(this Summoned b_Component, E_GameProperty b_GameProperty, bool b_HasTemporary = true)
        {
            if (b_GameProperty.IsChangeTypeEnemy())
            {
                return b_Component.GetNumerialSpecial(b_GameProperty);
            }
            return b_Component.ResetNumerial(b_GameProperty, b_HasTemporary);

            if (b_Component.ChangePropertyDic.TryGetValue(b_GameProperty, out var mChangeProperty))
            {
                if (mChangeProperty.Item1)
                {
                    return mChangeProperty.Item2;
                }
            }
            else
            {
                mChangeProperty = b_Component.ChangePropertyDic[b_GameProperty] = (false, 0);
            }

            int mResult = b_Component.ResetNumerial(b_GameProperty, b_HasTemporary);

            mChangeProperty.Item1 = true;
            mChangeProperty.Item2 = mResult;
            return mResult;
        }

		public static int ResetNumerial(this Summoned b_Component, E_GameProperty b_GameProperty, bool b_HasTemporary = true)
        {
            int mResult = 0;

            //基础
            if (b_Component.GamePropertyDic.TryGetValue(b_GameProperty, out var mGamePropertyValue))
            {
                mResult += mGamePropertyValue;
            }

            if (b_Component.GamePropertyNumerialDic.TryGetValue(b_GameProperty, out var GamePropertyNumerial))
            {
                mResult += GamePropertyNumerial.Run(b_Component, b_HasTemporary);
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

        public static int GetNumerialSpecial(this Summoned b_Component, E_GameProperty b_GameProperty)
        {
            int mResult = 0;

            switch (b_GameProperty)
            {
                case E_GameProperty.PROP_HP:
                    {
                        mResult = b_Component.UnitData.Hp;
                        var mHPMax = b_Component.GetNumerial(E_GameProperty.PROP_HP_MAX);
                        if (mResult > mHPMax)
                        {
                            mResult = b_Component.UnitData.Hp = mHPMax;
                        }
                    }
                    break;
                case E_GameProperty.PROP_MP:
                    {
                        mResult = b_Component.UnitData.Mp;
                        var mMPMax = b_Component.GetNumerial(E_GameProperty.PROP_MP_MAX);
                        if (mResult > mMPMax)
                        {
                            mResult = b_Component.UnitData.Mp = mMPMax;
                        }
                    }
                    break;
                case E_GameProperty.PROP_SD:
                    {
                        mResult = b_Component.UnitData.SD;
                        var mSDMax = b_Component.GetNumerial(E_GameProperty.PROP_SD_MAX);
                        if (mResult > mSDMax)
                        {
                            mResult = b_Component.UnitData.SD = mSDMax;
                        }
                    }
                    break;
                case E_GameProperty.PROP_AG:
                    {
                        mResult = b_Component.UnitData.AG;
                        var mAGMax = b_Component.GetNumerial(E_GameProperty.PROP_AG_MAX);
                        if (mResult > mAGMax)
                        {
                            mResult = b_Component.UnitData.AG = mAGMax;
                        }
                    }
                    break;
                case E_GameProperty.Level:
                    {
                        mResult = b_Component.Config.Lvl;
                    }
                    break;
                default:
                    break;
            }

            return mResult;
        }
    }
}