using System;
using System.Threading.Tasks;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static partial class HolyteacherSummonedSystem
    {
        public static int GetNumerial(this HolyteacherSummoned b_Component, E_GameProperty b_GameProperty, bool b_HasTemporary = true)
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
        public static int ResetNumerial(this HolyteacherSummoned b_Component, E_GameProperty b_GameProperty, bool b_HasTemporary = true)
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

            return mResult;
        }

        public static int GetNumerialSpecial(this HolyteacherSummoned b_Component, E_GameProperty b_GameProperty)
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
                        mResult = b_Component.Item.GetProp(EItemValue.MountsLevel);
                    }
                    break;
                default:
                    break;
            }
            return mResult;
        }
    }
}