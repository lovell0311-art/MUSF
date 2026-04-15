using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{



    public static partial class GamePlayerSystem
    {

        public static void AwakeProperty(this GamePlayer b_Component, CreateRole_InfoConfig b_Config)
        {
            b_Component.Identity = E_Identity.Hero;
            b_Component.Config = b_Config;
            b_Component.GameHeroType = (E_GameOccupation)b_Component.Data.PlayerTypeId;

            b_Component.GameHeroSexType = (E_GameHeroSexType)b_Config.Sex;

            var mCacheDatas = Root.MainFactory.GetCustomComponent<PropertyCreateBuilder>().CacheDatas;
            foreach (var item in mCacheDatas)
            {
                var mResult = Root.CreateBuilder.GetInstance<C_PropertyNumerial>(item.Value);
                b_Component.GamePropertyNumerialDic[(E_GameProperty)item.Key] = mResult;
            }
        }
        public static void DataUpdateProperty(this GamePlayer b_Component)
        {
            if (b_Component.UnitData.Hp <= 0)
            {
                b_Component.IsDeath = true;
            }

            if (b_Component.GamePropertyDic.Count > 0)
            {
                b_Component.GamePropertyDic.Clear();
            }

            b_Component.GamePropertyDic[E_GameProperty.ReplyHpRate] = 100;
            b_Component.GamePropertyDic[E_GameProperty.ReplySDRate] = 100;
            b_Component.GamePropertyDic[E_GameProperty.ReplyMpRate] = 300;
            b_Component.GamePropertyDic[E_GameProperty.ReplyAGRate] = 300;

            var mStrength = b_Component.GetNumerial(E_GameProperty.Property_Strength);
            var mWillpower = b_Component.GetNumerial(E_GameProperty.Property_Willpower);
            var mAgility = b_Component.GetNumerial(E_GameProperty.Property_Agility);
            var mBoneGas = b_Component.GetNumerial(E_GameProperty.Property_BoneGas);
            var mCommand = b_Component.GetNumerial(E_GameProperty.Property_Command);
            var mLevel = b_Component.Data.Level;

            switch (b_Component.GameHeroType)
            {
                case E_GameOccupation.Spell:
                    {
                        b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = mStrength / 8;
                        b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.MinMagicAtteck] = mWillpower / 6;
                        b_Component.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = mWillpower / 3;

                        b_Component.GamePropertyDic[E_GameProperty.MinElementAtteck] = mWillpower / 9;
                        b_Component.GamePropertyDic[E_GameProperty.MaxElementAtteck] = mWillpower / 6;

                        b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = mLevel * 3 + (int)(mAgility * 4);
                        b_Component.GamePropertyDic[E_GameProperty.ElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = mAgility / 10;

                        b_Component.GamePropertyDic[E_GameProperty.Defense] = mAgility / 4;
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefense] = mAgility / 4;
                        b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = mAgility / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = mLevel * 2 + (int)(mAgility / 4);
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefenseRate] = mAgility / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementDefenseRate] = mAgility / 3;

                        b_Component.GamePropertyDic[E_GameProperty.PROP_AG_MAX] = (int)(mStrength * 0.2f) + (int)(mWillpower * 0.2f) + (int)(mAgility * 0.4f) + mBoneGas / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_SD_MAX] = (int)(mLevel * 1.2f) + b_Component.GamePropertyDic[E_GameProperty.Defense] + (int)((mStrength + mWillpower + mAgility + mBoneGas) * 1.2f);


                        b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = 60 + (mBoneGas - 15) * 2 + mLevel * 1;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = 60 + (int)((mWillpower - 30) * 2) + (int)(mLevel * 2);
                    }
                    break;
                case E_GameOccupation.Swordsman:
                    {
                        b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = mStrength / 6 /*+ mWillpower / 8*/;
                        b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = mStrength / 4 /*+ mWillpower / 6*/;

                        b_Component.GamePropertyDic[E_GameProperty.MinMagicAtteck] = 0;
                        b_Component.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = 0;

                        b_Component.GamePropertyDic[E_GameProperty.MinElementAtteck] = mStrength / 6;
                        b_Component.GamePropertyDic[E_GameProperty.MaxElementAtteck] = mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = mLevel * 3 + (int)(mAgility * 4.5f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = mAgility / 15;

                        b_Component.GamePropertyDic[E_GameProperty.Defense] = mAgility / 3;
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefense] = mAgility / 3;
                        b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = mAgility / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = mLevel * 2 + (int)(mAgility * 0.5f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefenseRate] = mAgility / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementDefenseRate] = mAgility / 3;

                        b_Component.GamePropertyDic[E_GameProperty.PROP_AG_MAX] = (int)(mStrength * 0.15f) + (int)(mWillpower * 1) + (int)(mAgility * 0.2f) + (int)(mBoneGas * 0.3f);
                        b_Component.GamePropertyDic[E_GameProperty.PROP_SD_MAX] = (int)(mLevel * 1.2f) + b_Component.GamePropertyDic[E_GameProperty.Defense] + (int)((mStrength + mWillpower + mAgility + mBoneGas) * 1.2f);
                        b_Component.GamePropertyDic[E_GameProperty.SkillAddition] = b_Component.Config.SkillAddition + (int)(mWillpower * 0.1f);


                        b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = 110 + (mBoneGas - 25) * 3 + mLevel * 2;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = 20 + (mWillpower - 10) * 1 + (int)(mLevel * 0.5f);
                    }
                    break;
                case E_GameOccupation.Archer:
                    {
                        b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = mStrength / 6 + mAgility / 6;
                        b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = mStrength / 3 + mAgility / 3;

                        b_Component.GamePropertyDic[E_GameProperty.MinMagicAtteck] = 0;
                        b_Component.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = 0;

                        b_Component.GamePropertyDic[E_GameProperty.MinElementAtteck] = mStrength / 14 + mAgility / 8;
                        b_Component.GamePropertyDic[E_GameProperty.MaxElementAtteck] = mStrength / 8 + mAgility / 4;

                        b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = mLevel * 3 + (int)(mAgility * 0.6f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = mAgility / 30;

                        b_Component.GamePropertyDic[E_GameProperty.Defense] = mAgility / 10;
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefense] = mAgility / 10;
                        b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = mAgility / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = mLevel * 2 + (int)(mAgility * 0.1f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefenseRate] = mAgility / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementDefenseRate] = mAgility / 4;

                        b_Component.GamePropertyDic[E_GameProperty.PROP_AG_MAX] = (int)(mStrength / 3) + (int)(mWillpower * 0.2f) + (int)(mAgility * 0.2f) + mBoneGas / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_SD_MAX] = (int)(mLevel * 1.2f) + b_Component.GamePropertyDic[E_GameProperty.Defense] + (int)((mStrength + mWillpower + mAgility + mBoneGas) * 1);


                        b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = 80 + (mBoneGas - 20) * 2 + mLevel * 1;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = 30 + (int)((mWillpower - 15) * 1.5f) + (int)(mLevel * 1.5f);
                    }
                    break;
                case E_GameOccupation.Spellsword:
                    {
                        b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = mStrength / 4 + mWillpower / 8;
                        b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = mStrength / 3 + mWillpower / 6;

                        b_Component.GamePropertyDic[E_GameProperty.MinMagicAtteck] = mWillpower / 7;
                        b_Component.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = mWillpower / 3;

                        b_Component.GamePropertyDic[E_GameProperty.MinElementAtteck] = mStrength / 10 + mWillpower / 14;
                        b_Component.GamePropertyDic[E_GameProperty.MaxElementAtteck] = mStrength / 6 + mWillpower / 8;

                        b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = mLevel * 3 + (int)(mAgility * 3.5f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = mAgility / 15;

                        b_Component.GamePropertyDic[E_GameProperty.Defense] = mAgility / 4;
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefense] = mAgility / 5;
                        b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = mAgility / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = mLevel * 2 + mAgility / 4;
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefenseRate] = mAgility / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementDefenseRate] = mAgility / 3;

                        b_Component.GamePropertyDic[E_GameProperty.PROP_AG_MAX] = (int)(mStrength * 0.2f) + (int)(mWillpower * 0.15f) + (int)(mAgility * 0.25f) + (int)(mBoneGas / 3);
                        b_Component.GamePropertyDic[E_GameProperty.PROP_SD_MAX] = (int)(mLevel * 1.2f) + b_Component.GamePropertyDic[E_GameProperty.Defense] + (int)((mStrength + mWillpower + mAgility + mBoneGas) * 1);
                        b_Component.GamePropertyDic[E_GameProperty.SkillAddition] = b_Component.Config.SkillAddition;

                        b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = 110 + (mBoneGas - 26) * 2 + mLevel;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = 60 + (mWillpower - 26) * 2 + mLevel;
                    }
                    break;
                case E_GameOccupation.SummonWarlock:
                    {
                        b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = mStrength / 8;
                        b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.MinMagicAtteck] = mWillpower / 8;
                        b_Component.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = mWillpower / 3;

                        b_Component.GamePropertyDic[E_GameProperty.MinDamnationAtteck] = mWillpower / 8;
                        b_Component.GamePropertyDic[E_GameProperty.MaxDamnationAtteck] = mWillpower / 3;

                        //self.GamePropertyDic[E_GameProperty.MinElementAtteck] = self.Agility / 10;
                        //self.GamePropertyDic[E_GameProperty.MaxElementAtteck] = self.Agility / 6;

                        b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = mLevel * 3 + (int)(mAgility * 3.5f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = mAgility / 15;

                        b_Component.GamePropertyDic[E_GameProperty.Defense] = mAgility / 3;
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefense] = mAgility / 3;
                        b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = mAgility / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = mLevel * 2 + (int)(mAgility * 0.5f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefenseRate] = mAgility / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementDefenseRate] = mAgility / 4;

                        b_Component.GamePropertyDic[E_GameProperty.PROP_AG_MAX] = (int)(mStrength * 0.2f) + (int)(mWillpower * 0.15f) + mAgility / 3 + mBoneGas / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_SD_MAX] = (int)(mLevel * 1.2f) + b_Component.GamePropertyDic[E_GameProperty.Defense] + (int)((mStrength + mWillpower + mAgility + mBoneGas) * 1);

                        b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = 70 + (mBoneGas - 18) * 2 + mLevel * 1;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = 40 + (int)((mWillpower - 23) * 1.7f) + (int)(mLevel * 1.5f);
                    }
                    break;
                case E_GameOccupation.Holyteacher:
                    {
                        b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = mStrength / 6 + mWillpower / 12;
                        b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = mStrength / 4 + mWillpower / 8;

                        //self.GamePropertyDic[E_GameProperty.MinMagicAtteck] = self.Strength / 8 + self.Willpower / 9;
                        //self.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = self.Strength / 8 + self.Willpower / 4;

                        b_Component.GamePropertyDic[E_GameProperty.MinElementAtteck] = mStrength / 10 + mWillpower / 14;
                        b_Component.GamePropertyDic[E_GameProperty.MaxElementAtteck] = mStrength / 6 + mWillpower / 10;

                        b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 2.5f) + mStrength / 6 + mCommand / 10;
                        b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = mLevel * 3 + (int)(mAgility * 4f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = mAgility / 10;

                        b_Component.GamePropertyDic[E_GameProperty.Defense] = mAgility / 6;
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefense] = mAgility / 7;
                        b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = mAgility / 6;
                        b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = mLevel * 2 + (int)(mAgility * 0.5f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefenseRate] = mAgility / 7;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementDefenseRate] = mAgility / 7;

                        b_Component.GamePropertyDic[E_GameProperty.PROP_AG_MAX] = (int)(mStrength / 3) + (int)(mWillpower * 0.15f) + mAgility / 5 + mBoneGas / 10 + mCommand / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_SD_MAX] = (int)(mLevel * 1.2f) + b_Component.GamePropertyDic[E_GameProperty.Defense] + (int)((mStrength + mWillpower + mAgility + mBoneGas + mCommand) * 1f);
                        b_Component.GamePropertyDic[E_GameProperty.SkillAddition] = b_Component.Config.SkillAddition + (int)(mWillpower * 0.05f);

                        b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = 90 + (mBoneGas - 20) * 2 + (int)(mLevel * 1.5f);
                        b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = 40 + (int)((mWillpower - 15) * 1.5f) + (int)(mLevel * 1.0f);
                    }
                    break;
                case E_GameOccupation.Combat:
                    {
                        b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = mStrength / 7 + mBoneGas / 15;
                        b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = mStrength / 5 + mBoneGas / 12;
                        b_Component.GamePropertyDic[E_GameProperty.AdvanceAttackPower] = mBoneGas / 10;
                        b_Component.GamePropertyDic[E_GameProperty.RangeAttack] = mWillpower / 7;
                        b_Component.GamePropertyDic[E_GameProperty.SacredBeast] = mAgility / 6;
                        //self.GamePropertyDic[E_GameProperty.MinMagicAtteck] = self.Strength / 8 + self.Willpower / 9;
                        //self.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = self.Strength / 8 + self.Willpower / 4;

                        b_Component.GamePropertyDic[E_GameProperty.MinElementAtteck] = mStrength / 10 + mBoneGas / 20;
                        b_Component.GamePropertyDic[E_GameProperty.MaxElementAtteck] = mStrength / 6 + mBoneGas / 15;

                        b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = mLevel * 3 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = mLevel * 3 + (int)(mAgility * 3.5f) + mWillpower / 5;
                        b_Component.GamePropertyDic[E_GameProperty.ElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = mAgility / 8;

                        b_Component.GamePropertyDic[E_GameProperty.Defense] = mAgility / 8;
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefense] = mAgility / 8;
                        b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = mAgility / 10;
                        b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = mLevel * 3 + (int)(mAgility * 0.1f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefenseRate] = mAgility / 8;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementDefenseRate] = mAgility / 3;

                        b_Component.GamePropertyDic[E_GameProperty.PROP_AG_MAX] = (int)(mStrength * 0.15f) + (int)(mWillpower * 1f) + mAgility / 5 + mBoneGas / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_SD_MAX] = (int)(mLevel * 1.2f) + b_Component.GamePropertyDic[E_GameProperty.Defense] + (int)((mStrength + mWillpower + mAgility + mBoneGas) * 1.2f);


                        b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = 110 + (mBoneGas - 25) * 2 + (int)(mLevel * 1.3f);
                        b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = 40 + (int)((mWillpower - 20) * 1.3f) + (int)(mLevel * 1.0f);
                    }
                    break;
                case E_GameOccupation.GrowLancer:
                    {
                        b_Component.GamePropertyDic[E_GameProperty.MinAtteck] = mStrength / 8 + mBoneGas / 10;
                        b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] = mStrength / 4 + mBoneGas / 6;
                        b_Component.GamePropertyDic[E_GameProperty.DreamRiderPenalize] = mStrength / 10;
                        b_Component.GamePropertyDic[E_GameProperty.DreamRiderIrritate] = mAgility / 10;

                        //self.GamePropertyDic[E_GameProperty.MinMagicAtteck] = self.Strength / 8 + self.Willpower / 9;
                        //self.GamePropertyDic[E_GameProperty.MaxMagicAtteck] = self.Strength / 8 + self.Willpower / 4;

                        b_Component.GamePropertyDic[E_GameProperty.MinElementAtteck] = mStrength / 4 + mAgility / 1;
                        b_Component.GamePropertyDic[E_GameProperty.MaxElementAtteck] = mStrength / 4 + mAgility / 1;

                        b_Component.GamePropertyDic[E_GameProperty.AtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.25f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPAtteckSuccessRate] = mLevel * 3 + (int)(mAgility * 2);
                        b_Component.GamePropertyDic[E_GameProperty.ElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementAtteckSuccessRate] = mLevel * 5 + (int)(mAgility * 1.5f) + mStrength / 4;

                        b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] = mAgility / 20;

                        b_Component.GamePropertyDic[E_GameProperty.Defense] = mAgility / 7;
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefense] = mAgility / 8;
                        b_Component.GamePropertyDic[E_GameProperty.DefenseRate] = mAgility / 4;
                        b_Component.GamePropertyDic[E_GameProperty.PVPDefenseRate] = mLevel * 2 + (int)(mAgility * 0.2f);
                        b_Component.GamePropertyDic[E_GameProperty.ElementDefenseRate] = mAgility / 8;
                        b_Component.GamePropertyDic[E_GameProperty.PVPElementDefenseRate] = mAgility / 4;

                        b_Component.GamePropertyDic[E_GameProperty.PROP_AG_MAX] = (int)(mStrength * 0.15f) + (int)(mWillpower * 1f) + mAgility / 5 + mBoneGas / 3;
                        b_Component.GamePropertyDic[E_GameProperty.PROP_SD_MAX] = (int)(mLevel * 1.2f) + b_Component.GamePropertyDic[E_GameProperty.Defense] + (int)((mStrength + mWillpower + mAgility + mBoneGas) * 1.2f);


                        b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] = 110 + (mBoneGas - 25) * 1 + (int)(mLevel * 2);
                        b_Component.GamePropertyDic[E_GameProperty.PROP_MP_MAX] = 40 + (int)((mWillpower - 24) * 1) + (int)(mLevel * 1);
                    }
                    break;
                default:
                    break;
            }

            b_Component.UnitData.Mp = b_Component.GetNumerial(E_GameProperty.PROP_MP_MAX);
            b_Component.UnitData.Hp = b_Component.GetNumerial(E_GameProperty.PROP_HP_MAX);
            b_Component.UnitData.AG = b_Component.GetNumerial(E_GameProperty.PROP_AG_MAX);
            b_Component.UnitData.SD = b_Component.GetNumerial(E_GameProperty.PROP_SD_MAX);
        }

        public static void DataAddPropertyBuffer(this GamePlayer b_Component)
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

                    G2C_ChangeValue_notice mChangeValue = null;

                    var mHp = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP);
                    var mHpMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    if (mHp < mHpMax)
                    {
                        var mReplyHpRate = b_CombatSource.GetNumerialFunc(E_GameProperty.ReplyHpRate);
                        var mReplyHp = mHpMax * mReplyHpRate * 0.0001f;
                        if (mReplyHp < 1) mReplyHp = 1;
                        mReplyHp+= b_CombatSource.GetNumerialFunc(E_GameProperty.ReplyHp);
                        b_CombatSource.UnitData.Hp += (int)mReplyHp;
                        if (b_CombatSource.UnitData.Hp > mHpMax) b_CombatSource.UnitData.Hp = mHpMax;

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
                        if (mReplySD < 1) mReplySD = 1;
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
                        if (b_CombatSource.Identity == E_Identity.Hero)
                        {
                            mChangeValue.GameUserId = b_CombatSource.InstanceId;
                            (b_CombatSource as GamePlayer).Player.Send(mChangeValue);
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

                    G2C_ChangeValue_notice mChangeValue = null;

                    var mMP = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP);
                    var mMpMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                    if (mMP < mMpMax)
                    {
                        var mReplyMpRate = b_CombatSource.GetNumerialFunc(E_GameProperty.ReplyMpRate);
                        var mReplyMp = mMpMax * mReplyMpRate * 0.0001f;
                        if (mReplyMp < 1) mReplyMp = 1;
                        mReplyMp += b_CombatSource.GetNumerialFunc(E_GameProperty.ReplyMp);
                        b_CombatSource.UnitData.Mp += (int)mReplyMp;
                        if (b_CombatSource.UnitData.Mp > mMpMax) b_CombatSource.UnitData.Mp = mMpMax;

                        if (mChangeValue == null) mChangeValue = new G2C_ChangeValue_notice();
                        G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.PROP_MP;
                        mChildChangeValue.Value = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP);
                        mChangeValue.Info.Add(mChildChangeValue);
                    }

                    if (mChangeValue != null)
                    {
                        if (b_CombatSource.Identity == E_Identity.Hero)
                        {
                            mChangeValue.GameUserId = b_CombatSource.InstanceId;
                            (b_CombatSource as GamePlayer).Player.Send(mChangeValue);
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
                mBattleSyncTimer.SyncWaitTime = 2 * 1000;
                mBattleSyncTimer.NextWaitTime = CustomFrameWork.Help_TimeHelper.GetNow() + mBattleSyncTimer.SyncWaitTime;
                mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                {
                    if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    if (b_CombatSource.IsDeath) return CombatSource.E_SyncTimerTaskResult.AutoNextRound;

                    G2C_ChangeValue_notice mChangeValue = null;

                    var mAG = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_AG);
                    var mAGMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_AG_MAX);
                    if (mAG < mAGMax)
                    {
                        var mReplyAGRate = b_CombatSource.GetNumerialFunc(E_GameProperty.ReplyAGRate);
                        var mReplyAG = mAGMax * mReplyAGRate * 0.0001f;
                        if (mReplyAG < 1) mReplyAG = 1;
                        mReplyAG += b_CombatSource.GetNumerialFunc(E_GameProperty.EmbedAGRecoveryUp);
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
                        if (b_CombatSource.Identity == E_Identity.Hero)
                        {
                            mChangeValue.GameUserId = b_CombatSource.InstanceId;
                            (b_CombatSource as GamePlayer).Player.Send(mChangeValue);
                        }
                    }

                    return CombatSource.E_SyncTimerTaskResult.AutoNextRound;
                };
                b_Component.AddTask(mBattleSyncTimer);
            }

            if (b_Component.UnitData.PkPoint > 0)
            {
                var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                //mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateUnitId(b_ZoneId);
                mBattleSyncTimer.SyncWaitTime = 60 * 1000;
                mBattleSyncTimer.NextWaitTime = CustomFrameWork.Help_TimeHelper.GetNow() + mBattleSyncTimer.SyncWaitTime;
                mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                {
                    if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    if (b_CombatSource.IsDeath) return CombatSource.E_SyncTimerTaskResult.AutoNextRound;

                    if (b_CombatSource.UnitData.PkPoint <= 0) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    b_CombatSource.UnitData.PkPoint -= (int)120;
                    if (b_CombatSource.UnitData.PkPoint < 0) b_CombatSource.UnitData.PkPoint = 0;

                    G2C_ChangeValue_notice mChangeValue = new G2C_ChangeValue_notice();
                    G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                    mChildChangeValue.Key = (int)E_GameProperty.PkNumber;
                    mChildChangeValue.Value = b_CombatSource.GetNumerialFunc(E_GameProperty.PkNumber);
                    mChangeValue.Info.Add(mChildChangeValue);

                    mChangeValue.GameUserId = b_CombatSource.InstanceId;
                    (b_CombatSource as GamePlayer).Player.Send(mChangeValue);

                    if (b_CombatSource.UnitData.PkPoint <= 0) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    return CombatSource.E_SyncTimerTaskResult.AutoNextRound;
                };
                b_Component.AddTask(mBattleSyncTimer);
            }
        }
        public static void DataAddPropertyBufferGotoMap(this GamePlayer b_Component, BattleComponent b_BattleComponent)
        {
            b_Component.DataAddPropertyBuffer();

            {
                //if (b_Component.Pets != null && b_Component.Pets.dBPetsData.ConfigID == 105)
                //{
                //    // 拉宠物buff
                //    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                //    {
                //        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                //        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.DeathRemoveTask;
                //        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                //        mBattleSyncTimer.SyncWaitTime = 2 * 60 * 1000;
                //        mBattleSyncTimer.NextWaitTime = CustomFrameWork.Help_TimeHelper.GetNow() + mBattleSyncTimer.SyncWaitTime;

                //        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                //        {
                //            if (b_CombatSource.IsDisposeable) return;

                //            b_CombatSource.RemoveHealthState(E_BattleSkillStats.FangYuHuZhao, b_BattleComponent);
                //            b_CombatSource.UpdateHealthState();
                //        };
                //        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                //        {
                //            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                //            Pets pets = (b_CombatSource as GamePlayer).Pets;
                //            if (pets == null || (pets != null && (b_CombatSource as GamePlayer).Pets.dBPetsData.ConfigID != 105))
                //            {
                //                return CombatSource.E_SyncTimerTaskResult.Dispose;
                //            }

                //            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var mCurrentHealthStats) == false)
                //            {
                //                return CombatSource.E_SyncTimerTaskResult.Dispose;
                //            }

                //            if (mCurrentHealthStats.CacheDatas.TryGetValue(0, out var hpCacheDatas) == false)
                //            {
                //                mCurrentHealthStats.CacheDatas[0] = new CombatSource.C_CombatUnitStatsSource();
                //            }
                //            if (hpCacheDatas.CacheData == null) hpCacheDatas.CacheData = new Dictionary<int, int>();

                //            var mMax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                //            mMax = (int)(mMax * 0.1f);

                //            hpCacheDatas.CacheData.TryGetValue(0, out var mTempValue);
                //            if (mTempValue != mMax)
                //            {
                //                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                //                mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;
                //                G2C_BattleKVData mData = new G2C_BattleKVData();
                //                mData.Key = (int)E_GameProperty.FangYuHuZhao;
                //                mData.Value = mMax;
                //                mChangeValueMessage.Info.Add(mData);

                //                b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                //            }
                //            hpCacheDatas.CacheData[0] = mMax;

                //            b_TimerTask.NextWaitTime = b_TimerTask.NextWaitTime + b_TimerTask.SyncWaitTime;
                //            b_CombatSource.AddTask(b_TimerTask);

                //            return CombatSource.E_SyncTimerTaskResult.NextRound;
                //        };
                //        return mBattleSyncTimer;
                //    };
                //    if (b_Component.Pets.dBPetsData.PetsUseState != 0)
                //    {
                //        b_Component.AddHealthState(E_BattleSkillStats.FangYuHuZhao, 0, 0, 0, mCreateFunc, b_BattleComponent);
                //        b_Component.UpdateHealthState();
                //        if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var hp_Curse))
                //        {
                //            if (hp_Curse.CacheDatas.TryGetValue(0, out var hpCacheDatas) == false)
                //            {
                //                hp_Curse.CacheDatas[0] = new CombatSource.C_CombatUnitStatsSource();
                //            }
                //            if (hpCacheDatas.CacheData == null) hpCacheDatas.CacheData = new Dictionary<int, int>();

                //            var mMax = b_Component.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                //            mMax = (int)(mMax * 0.1f);
                //            hpCacheDatas.CacheData[0] = mMax;
                //        }
                //    }
                //}

                if (b_Component.Data.DBBufflist.Count > 0)
                {
                    var mCurrentTick = b_BattleComponent.CurrentTimeTick;

                    var mKeylist = b_Component.Data.DBBufflist.Keys.ToArray();
                    var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Item_ConsumablesConfigJson>().JsonDic;
                    for (int i = 0, len = mKeylist.Length; i < len; i++)
                    {
                        var mKey = mKeylist[i];
                        var mBuffTick = b_Component.Data.DBBufflist[mKey];

                        if (mKey == (int)E_BattleSkillStats.XinShouBuff)
                        {
                            if (b_Component.Data.Level > 150) continue;

                            b_Component.AddHealthState(E_BattleSkillStats.XinShouBuff, 0, 0, 0, null, b_BattleComponent);
                            b_Component.UpdateHealthState();

                            continue;
                        }

                        if (mBuffTick <= mCurrentTick) continue;

                        if (mJsonDic.TryGetValue(mKey, out var mConfig) == false) continue;

                        switch ((E_BattleSkillStats)mKey)
                        {
                            case E_BattleSkillStats.Use310018:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310018, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310018, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310018, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310019:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310019, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310019, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310019, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310020:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310020, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310020, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310020, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310021:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310021, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310021, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310021, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310022:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310022, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();

                                            var mHpmax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                                            if (b_CombatSource.UnitData.Hp > mHpmax)
                                            {
                                                b_CombatSource.UnitData.Hp = mHpmax;
                                            }

                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310022, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310022, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();

                                    b_Component.UnitData.Hp += mConfig.Value;
                                    var mCurrentTempHpmax = b_Component.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                                    if (b_Component.UnitData.Hp > mCurrentTempHpmax)
                                    {
                                        b_Component.UnitData.Hp = mCurrentTempHpmax;
                                    }

                                    void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                    {
                                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                        mBattleKVData.Key = (int)b_GameProperty;
                                        mBattleKVData.Value = b_Component.GetNumerial(b_GameProperty);
                                        b_ChangeValue_notice.Info.Add(mBattleKVData);
                                    }

                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;

                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                                    b_BattleComponent.Parent.SendNotice(b_Component, mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310023:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310023, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();

                                            var mHpmax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                                            if (b_CombatSource.UnitData.Mp > mHpmax)
                                            {
                                                b_CombatSource.UnitData.Mp = mHpmax;
                                            }

                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP_MAX);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310023, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310023, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();

                                    b_Component.UnitData.Mp += mConfig.Value;
                                    var mCurrentTempHpmax = b_Component.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                                    if (b_Component.UnitData.Mp > mCurrentTempHpmax)
                                    {
                                        b_Component.UnitData.Mp = mCurrentTempHpmax;
                                    }

                                    void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                    {
                                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                        mBattleKVData.Key = (int)b_GameProperty;
                                        mBattleKVData.Value = b_Component.GetNumerial(b_GameProperty);
                                        b_ChangeValue_notice.Info.Add(mBattleKVData);
                                    }

                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;

                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP);
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP_MAX);

                                    b_BattleComponent.Parent.SendNotice(b_Component, mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310024:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310024, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310024, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310024, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310025:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310025, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310025, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310025, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310026:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310026, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();

                                            var mHpmax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                                            if (b_CombatSource.UnitData.Hp > mHpmax)
                                            {
                                                b_CombatSource.UnitData.Hp = mHpmax;
                                            }

                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310026, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310026, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();

                                    void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                    {
                                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                        mBattleKVData.Key = (int)b_GameProperty;
                                        mBattleKVData.Value = b_Component.GetNumerial(b_GameProperty);
                                        b_ChangeValue_notice.Info.Add(mBattleKVData);
                                    }

                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;

                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                                    b_BattleComponent.Parent.SendNotice(b_Component, mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310027:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310027, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310027, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310027, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310028:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310028, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310028, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310028, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.UseYingHuaJiu310029:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.UseYingHuaJiu310029, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();

                                            var mHpmax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                                            if (b_CombatSource.UnitData.Mp > mHpmax)
                                            {
                                                b_CombatSource.UnitData.Mp = mHpmax;
                                            }

                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP_MAX);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaJiu310029, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.UseYingHuaJiu310029, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();

                                    b_Component.UnitData.Mp += mConfig.Value;
                                    var mCurrentTempHpmax = b_Component.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                                    if (b_Component.UnitData.Mp > mCurrentTempHpmax)
                                    {
                                        b_Component.UnitData.Mp = mCurrentTempHpmax;
                                    }

                                    void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                    {
                                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                        mBattleKVData.Key = (int)b_GameProperty;
                                        mBattleKVData.Value = b_Component.GetNumerial(b_GameProperty);
                                        b_ChangeValue_notice.Info.Add(mBattleKVData);
                                    }

                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;

                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP);
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP_MAX);

                                    b_BattleComponent.Parent.SendNotice(b_Component, mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310031:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310031, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310031, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310031, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310032:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310032, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310032, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310032, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310034:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310034, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();

                                            var mHpmax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                                            if (b_CombatSource.UnitData.Hp > mHpmax)
                                            {
                                                b_CombatSource.UnitData.Hp = mHpmax;
                                            }

                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310034, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310034, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();

                                    if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310034, out var mTempBuffer))
                                    {
                                        if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                        {
                                            if (mTempBufferData.CacheData == null)
                                            {
                                                mTempBufferData.CacheData = new Dictionary<int, int>();
                                            }
                                            mTempBufferData.CacheData[(int)E_GameProperty.PROP_HP_MAX] = 100;
                                            mTempBufferData.CacheData[(int)E_GameProperty.ReplyHpRate] = 300;
                                        }
                                    }
                                }
                                break;
                            case E_BattleSkillStats.UseGuangZhiZhuFu310059:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.UseGuangZhiZhuFu310059, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseGuangZhiZhuFu310059, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.UseGuangZhiZhuFu310059, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.UseGuangZhiZhuFu310060:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.UseGuangZhiZhuFu310060, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseGuangZhiZhuFu310060, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.UseGuangZhiZhuFu310060, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.UseGuangZhiZhuFu310061:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.UseGuangZhiZhuFu310061, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseGuangZhiZhuFu310061, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.UseGuangZhiZhuFu310061, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.UseYingHuaBing310062:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaBing310062, out var hp_Curse))
                                            {
                                                b_CombatSource.RemoveHealthState(E_BattleSkillStats.UseYingHuaBing310062, b_BattleComponent);
                                                b_CombatSource.UpdateHealthState();

                                                var mHpmax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                                                if (b_CombatSource.UnitData.Hp > mHpmax)
                                                {
                                                    b_CombatSource.UnitData.Hp = mHpmax;
                                                }

                                                void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                                {
                                                    G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                    mBattleKVData.Key = (int)b_GameProperty;
                                                    mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                    b_ChangeValue_notice.Info.Add(mBattleKVData);
                                                }
                                                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                                mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                                                b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                            }
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaBing310062, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.UseYingHuaBing310062, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();

                                    b_Component.UnitData.Hp += mConfig.Value;
                                    var mCurrentTempHpmax = b_Component.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                                    if (b_Component.UnitData.Hp > mCurrentTempHpmax)
                                    {
                                        b_Component.UnitData.Hp = mCurrentTempHpmax;
                                    }

                                    void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                    {
                                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                        mBattleKVData.Key = (int)b_GameProperty;
                                        mBattleKVData.Value = b_Component.GetNumerial(b_GameProperty);
                                        b_ChangeValue_notice.Info.Add(mBattleKVData);
                                    }

                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;

                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                                    b_BattleComponent.Parent.SendNotice(b_Component, mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.UseYingHuaHuaBan310063:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.UseYingHuaHuaBan310063, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaHuaBan310063, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.UseYingHuaHuaBan310063, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310069:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310069, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310069, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310069, 100, mBuffTick - mCurrentTick, 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310070:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310070, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310070, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310070, 100, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310103:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310103, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310103, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310103, mConfig.Value, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310114:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310114, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310114, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310114, 100, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310116:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;
                                            if (!b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310116, out var hp_Curse)) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310116, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinAtteck);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxAtteck);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinMagicAtteck);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxMagicAtteck);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinDamnationAtteck);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxDamnationAtteck);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310116, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310116, mConfig.Value, mConfig.Value2, 0, mCreateFunc, b_BattleComponent);
                                    //b_Component.GamePropertyDic[E_GameProperty.MinAtteck] += mConfig.Value;
                                    //b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] += mConfig.Value;
                                    //b_Component.GamePropertyDic[E_GameProperty.MinMagicAtteck] += mConfig.Value;
                                    //b_Component.GamePropertyDic[E_GameProperty.MaxMagicAtteck] += mConfig.Value;
                                    //b_Component.GamePropertyDic[E_GameProperty.MinDamnationAtteck] += mConfig.Value;
                                    //b_Component.GamePropertyDic[E_GameProperty.MaxDamnationAtteck] += mConfig.Value;
                                    b_Component.UpdateHealthState();

                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;
                                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MinAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MinAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MaxAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MaxAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MinMagicAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MinMagicAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MaxMagicAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MinDamnationAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MinDamnationAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MaxDamnationAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MaxDamnationAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    b_Component.Player.Send(mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310117:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;
                                            if (!b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310117, out var hp_Curse)) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310117, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();

                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.Defense);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310117, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310117, mConfig.Value, mConfig.Value2, 0, mCreateFunc, b_BattleComponent);
                                    //b_Component.GamePropertyDic[E_GameProperty.Defense] += mConfig.Value;
                                    b_Component.UpdateHealthState();

                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;
                                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.Defense;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.Defense); 
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                   
                                    b_Component.Player.Send(mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310118:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;
                                            if (!b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310118, out var hp_Curse)) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310118, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();

                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310118, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310118, mConfig.Value, mConfig.Value2, 0, mCreateFunc, b_BattleComponent);
                                    //b_Component.GamePropertyDic[E_GameProperty.PROP_HP_MAX] += mConfig.Value;
                                    b_Component.UpdateHealthState();

                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;
                                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.PROP_HP_MAX;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                                    mChangeValueMessage.Info.Add(mPropertyData);

                                    b_Component.Player.Send(mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310119:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;
                                            if (!b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310119, out var hp_Curse)) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310119, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();

                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.InjuryValueRate_Increase);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310119, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310119, mConfig.Value, mConfig.Value2, 0, mCreateFunc, b_BattleComponent);
                                    //b_Component.GamePropertyDic[E_GameProperty.InjuryValueRate_Increase] += mConfig.Value;
                                    b_Component.UpdateHealthState();

                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;
                                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.InjuryValueRate_Increase;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.InjuryValueRate_Increase);
                                    mChangeValueMessage.Info.Add(mPropertyData);

                                    b_Component.Player.Send(mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310120:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;
                                            if (!b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310120, out var hp_Curse)) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310120, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.HurtValueAbsorbRate);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310120, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310120, mConfig.Value, mConfig.Value2, 0, mCreateFunc, b_BattleComponent);
                                    //b_Component.GamePropertyDic[E_GameProperty.HurtValueAbsorbRate] += mConfig.Value;
                                    b_Component.UpdateHealthState();

                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;
                                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.HurtValueAbsorbRate;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.HurtValueAbsorbRate);
                                    mChangeValueMessage.Info.Add(mPropertyData);

                                    b_Component.Player.Send(mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310127:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310127, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.AttackSpeed);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310127, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };
                                    int AttackSpeed = b_Component.GetNumerialFunc(E_GameProperty.AttackSpeed);
                                    AttackSpeed = AttackSpeed * mConfig.Value / 100;
                                    b_Component.AddHealthState(E_BattleSkillStats.Use310127, AttackSpeed, (mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310128:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310128, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinAtteck);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxAtteck);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinMagicAtteck);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxMagicAtteck);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinDamnationAtteck);
                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxDamnationAtteck);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310128, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310128, mConfig.Value, (mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;
                                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MinAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MinAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MaxAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MaxAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MinMagicAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MinMagicAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MaxMagicAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MinDamnationAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MinDamnationAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.MaxDamnationAtteck;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.MaxDamnationAtteck);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    b_Component.Player.Send(mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310129:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310129, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310129, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };
                                    b_Component.AddHealthState(E_BattleSkillStats.Use310129, mConfig.Value, (mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;
                                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.PROP_HP_MAX;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();

                                    b_Component.Player.Send(mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310130:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310130, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.Defense);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310130, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };
                                    b_Component.AddHealthState(E_BattleSkillStats.Use310130, mConfig.Value, (mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;
                                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.Defense;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.Defense);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();

                                    b_Component.Player.Send(mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310131:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310131, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP_MAX);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310131, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };
                                    b_Component.AddHealthState(E_BattleSkillStats.Use310131, mConfig.Value, (mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;
                                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.PROP_MP_MAX;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();

                                    b_Component.Player.Send(mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310132:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310132, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                                            {
                                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                                mBattleKVData.Key = (int)b_GameProperty;
                                                mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                                            }
                                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                            mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                                            b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310132, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };
                                    b_Component.AddHealthState(E_BattleSkillStats.Use310132, mConfig.Value, (mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                    mChangeValueMessage.GameUserId = b_Component.InstanceId;
                                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                                    mPropertyData.Key = (int)E_GameProperty.PROP_SD_MAX;
                                    mPropertyData.Value = b_Component.GetNumerialFunc(E_GameProperty.PROP_SD_MAX);
                                    mChangeValueMessage.Info.Add(mPropertyData);
                                    mPropertyData = new G2C_BattleKVData();

                                    b_Component.Player.Send(mChangeValueMessage);
                                }
                                break;
                            case E_BattleSkillStats.Use310133:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310133, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310133, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310133, mConfig.Value * 100, (mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310134:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310134, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310134, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310134, mConfig.Value * 100, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310135:
                                {
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310135, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310135, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310135, mConfig.Value * 100, (int)(mBuffTick - mCurrentTick), 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
                                break;
                            case E_BattleSkillStats.Use310157:
                                {
                                    if (b_Component.Data.ReincarnateCnt > 1) break;
                                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                                    {
                                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                                        mBattleSyncTimer.SyncWaitTime = (int)(mBuffTick - mCurrentTick);
                                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                                        mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return;

                                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310157, b_BattleComponent);
                                            b_CombatSource.UpdateHealthState();
                                        };
                                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                                        {
                                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310157, out var mCurrentHealthStats) == false)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }

                                            if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                                            {
                                                return CombatSource.E_SyncTimerTaskResult.Dispose;
                                            }
                                            else
                                            {
                                                b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                                                b_CombatSource.AddTask(b_TimerTask);
                                            }
                                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                                        };
                                        return mBattleSyncTimer;
                                    };

                                    b_Component.AddHealthState(E_BattleSkillStats.Use310157, mConfig.Value, mBuffTick - mCurrentTick, 0, mCreateFunc, b_BattleComponent);
                                    b_Component.UpdateHealthState();
                                }
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