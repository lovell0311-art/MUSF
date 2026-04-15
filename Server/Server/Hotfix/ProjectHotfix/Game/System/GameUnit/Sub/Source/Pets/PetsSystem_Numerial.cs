using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ETModel;
using TencentCloud.Mrs.V20200910.Models;
using CustomFrameWork;

namespace ETHotfix
{



    public static partial class PetsSystem
    {
        public static int GetNumerial(this Pets b_Component, E_GameProperty b_GameProperty, bool b_HasTemporary = true)
        {
            if (b_GameProperty.IsChangeTypePets())
            {
                return b_Component.GetNumerialSpecial(b_GameProperty);
            }
            return b_Component.ResetNumerialSource(b_GameProperty, b_HasTemporary);

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

            int mResult = b_Component.ResetNumerialSource(b_GameProperty, b_HasTemporary);

            mChangeProperty.Item1 = true;
            mChangeProperty.Item2 = mResult;
            return mResult;
        }

      
        public static int GetNumerialSpecial(this Pets b_Component, E_GameProperty b_GameProperty)
        {
            int mResult = 0;

            switch (b_GameProperty)
            {
                case E_GameProperty.PROP_HP:
                    {
                        mResult = b_Component.dBPetsData.PetsHP;
                    }
                    break;
                case E_GameProperty.PROP_MP:
                    {
                        mResult = b_Component.dBPetsData.PetsMP;
                    }
                    break;
                case E_GameProperty.PROP_SD:
                    {
                        mResult = b_Component.UnitData.SD;
                    }
                    break;
                case E_GameProperty.PROP_AG:
                    {
                        mResult = b_Component.UnitData.AG;
                    }
                    break;
                case E_GameProperty.Level:
                    {
                        mResult = b_Component.dBPetsData.PetsLevel;
                    }
                    break;
                default:
                    break;
            }
            return mResult;
        }
    }
}