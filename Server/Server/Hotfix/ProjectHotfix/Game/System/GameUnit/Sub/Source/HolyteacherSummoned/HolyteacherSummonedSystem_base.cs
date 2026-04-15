using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{



    public static partial class HolyteacherSummonedSystem
    {
        public static void SetConfig(this HolyteacherSummoned b_Component, Enemy_InfoConfig b_Config, Item b_Item)
        {
            b_Component.Identity = E_Identity.HolyteacherSummoned;
            b_Component.Config = b_Config;
            b_Component.Item = b_Item;

            if (b_Component.UnitData == null) b_Component.UnitData = new DBPlayerUnitData();

            var mCacheDatas = CustomFrameWork.Root.MainFactory.GetCustomComponent<PropertyCreateBuilder>().CacheDatas;
            foreach (var item in mCacheDatas)
            {
                switch ((E_GameProperty)item.Key)
                {
                    case E_GameProperty.MinAtteck:
                    case E_GameProperty.MaxAtteck:
                        {
                            var mResult = CustomFrameWork.Root.CreateBuilder.GetInstance<C_PropertyNumerial>(item.Value);
                            b_Component.GamePropertyNumerialDic[(E_GameProperty)item.Key] = mResult;
                        }
                        break;
                    default:
                        break;
                }
            }

            b_Component.GetNumerialFunc = (E_GameProperty b_GameProperty) =>
            {
                return b_Component.GetNumerial(b_GameProperty);
            };
        }


        public static void AfterAwake(this HolyteacherSummoned b_Component)
        {
            b_Component.UnitData.Hp = 255;

            b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = 255;

            var mCommand = b_Component.GamePlayer.GetNumerial(E_GameProperty.Property_Command);
            var mLevel = b_Component.Item.GetProp(EItemValue.MountsLevel);

            switch (b_Component.Config.Id)
            {
                case 594:
                    {
                        int mMinAtteck = 60, mMaxAtteck = 80, mAttackSpeed = 3;
                        b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = mMinAtteck + mLevel * 15 + mCommand / 8;
                        b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = mMaxAtteck + mLevel * 15 + mCommand / 4;
                        b_Component.GamePropertyDic[E_GameProperty.TianYingMinAtteck] = mMinAtteck + mLevel * 15 + mCommand / 8 * 2 / 3;
                        b_Component.GamePropertyDic[E_GameProperty.TianYingMaxAtteck] = mMaxAtteck + mLevel * 15 + mCommand / 4 * 2 / 3;

                        b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = mAttackSpeed + (int)(mLevel * 0.8f) + mCommand / 50;

                        b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = 20 + mLevel * 16;
                    }
                    break;
                default:
                    {
                        int mMinAtteck = 40, mMaxAtteck = 50, mAttackSpeed = 2;
                        b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = mMinAtteck + mLevel * 15 + mCommand / 8;
                        b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = mMaxAtteck + mLevel * 15 + mCommand / 4;
                        b_Component.GamePropertyDic[E_GameProperty.TianYingMinAtteck] = mMinAtteck + mLevel * 15 + mCommand / 8 * 2 / 3;
                        b_Component.GamePropertyDic[E_GameProperty.TianYingMaxAtteck] = mMaxAtteck + mLevel * 15 + mCommand / 4 * 2 / 3;

                        b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = mAttackSpeed + (int)(mLevel * 0.8f) + mCommand / 50;

                        b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = 10 + mLevel * 16;
                    }
                    break;
            }

            b_Component.GamePropertyDic[E_GameProperty.AttackDistance] = 6;

            b_Component.GamePropertyDic[E_GameProperty.InjuryValueRate_3] = 0;
            b_Component.GamePropertyDic[E_GameProperty.InjuryValueRate_2] = 0;
            b_Component.GamePropertyDic[E_GameProperty.LucklyAttackRate] = 0;
            b_Component.GamePropertyDic[E_GameProperty.ExcellentAttackRate] = 0;


            b_Component.DataAddPropertyBuffer();
        }
        public static void DataUpdate(this HolyteacherSummoned b_Component)
        {


        }
        public static void DataAddPropertyBuffer(this HolyteacherSummoned b_Component)
        {

        }
    }
}