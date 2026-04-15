//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using ETModel;

//namespace ETHotfix
//{



//    public static partial class GamePlayerSystem
//    {


//        private static int Numerial_master(this GamePlayer b_Component, E_GameProperty b_GameProperty, bool b_HasTemporary = true)
//        {
//            int mResult = 0;

//            // 大师
//            #region 大师
//            switch (b_GameProperty)
//            {
//                #region 大师 力量 智力 体力 敏捷 统率
//                case E_GameProperty.Property_Strength:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseStrength18, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseStrength118, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseStrength218, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseStrength318, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseStrength419, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseStrength518, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.Property_Willpower:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseWillpower15, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseWillpower115, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseWillpower215, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseWillpower315, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseWillpower416, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseWillpower515, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.Property_Agility:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseAgility17, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseAgility117, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseAgility217, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseAgility317, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseAgility418, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseAgility517, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.Property_BoneGas:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseBoneGas16;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseBoneGas116;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseBoneGas216;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseBoneGas316;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseAgility418;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.Property_Command:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseCommand413;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                #endregion
//                case E_GameProperty.PROP_HP:
//                    break;
//                case E_GameProperty.PROP_MP:
//                    break;
//                case E_GameProperty.PROP_SD:
//                    break;
//                case E_GameProperty.PROP_AG:
//                    break;
//                case E_GameProperty.PROP_HP_MAX:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax33, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax42, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax131, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax140, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax234, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax243, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.AddHpMax239, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax336, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax347, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax433, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax442, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax531, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax540, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.PROP_MP_MAX:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMagicPowerMax36, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMagicPowerMax134, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMagicPowerMax238, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_MpMax_263, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMagicPowerMax339, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMagicPowerMax436, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.PROP_SD_MAX:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseSDMax3, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseSDMax103, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseSDMax203, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseSDMax303, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseSDMax403, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseSDMax503, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.PROP_AG_MAX:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseAGMax39;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseAGMax137;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseAGMax241;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseAGMax343;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.IncreaseAGMax439;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.Injury_HP:
//                    break;
//                case E_GameProperty.Injury_SD:
//                    break;
//                case E_GameProperty.MaxAtteck:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {

//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    // 物理攻击力提高
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Skill_WeaponMaster127))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Skill_WeaponMaster127];
//                                        mResult += mMasteryValue;
//                                    }
//                                    // 最大伤害值 大师增益
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseMaxAttack160))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseMaxAttack160];
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 暴风之翼攻击强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Equipment_Attack_BaoFengZhiYi_120))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 暴风之翼
//                                            if (mWingEquipment.ConfigID == 220035)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Equipment_Attack_BaoFengZhiYi_120];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ShuangShouJian_Strengthening144))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手剑
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ShuangShouJian_Strengthening144];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DanShouJian_Strengthening145))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手剑
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening145];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 锤
//                                            if (mShieldWeaponEquipment.Type == EItemType.Swords)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening145];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Hammers_Strengthening146))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 锤
//                                            if (mWeaponEquipment.Type == EItemType.Maces)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Hammers_Strengthening146];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 锤
//                                            if (mShieldWeaponEquipment.Type == EItemType.Maces)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Hammers_Strengthening146];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Spear_Strengthening147))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 矛
//                                            if (mWeaponEquipment.Type == EItemType.Spears)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Spear_Strengthening147];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 矛
//                                            if (mShieldWeaponEquipment.Type == EItemType.Spears)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Spear_Strengthening147];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.PVPShuangShouJian_Strengthening148))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手剑
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.PVPShuangShouJian_Strengthening148];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }



//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    // 物理攻击力提高
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Skill_WeaponMaster233))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Skill_WeaponMaster233];
//                                        mResult += mMasteryValue;
//                                    }
//                                    // 最大伤害值 大师增益
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseMaxAttack266))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseMaxAttack266];
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 时空之翼攻击强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Equipment_Attack_ShiKongZhiYi_221))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 时空之翼
//                                            if (mWingEquipment.ConfigID == 220033)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Equipment_Attack_ShiKongZhiYi_221];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    var mBattleMasteryKey = E_BattleMasteryState.Arch_Strengthening254;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 矛
//                                            if (mWeaponEquipment.Type == EItemType.Bows)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    mBattleMasteryKey = E_BattleMasteryState.Crossbow_Master258;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 怒类装备时对目标的攻击力追加增加{0:F}。
//                                            if (mWeaponEquipment.Type == EItemType.Crossbows)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    // 物理攻击力提高
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.WeaponMaster332))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.WeaponMaster332];
//                                        mResult += mMasteryValue;
//                                    }
//                                    // 最大伤害值 大师增益
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseMaxAttack368))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseMaxAttack368];
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 毁灭之翼攻击强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Equipment_Attack_HuiMieZhiYi_322))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 破灭之翼
//                                            if (mWingEquipment.ConfigID == 220032)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Equipment_Attack_HuiMieZhiYi_322];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    // 双手剑强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ShuangShouJian_Strengthening353))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手剑
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ShuangShouJian_Strengthening353];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    // 单手剑强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DanShouJian_Strengthening354))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手剑
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening354];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手剑
//                                            if (mShieldWeaponEquipment.Type == EItemType.Swords && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening354];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }

//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    // 物理攻击力提高
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_WeaponMaster431, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    // 最大伤害值 大师增益
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMaxAttack462, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 帝王披风攻击强化
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Attack_DiWangPiFeng422, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 帝王披风
//                                            if (mWingEquipment.ConfigID == 220031)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    // 权杖强化
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.QuanZhang_Strengthen445, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手剑
//                                            if (mWeaponEquipment.Type == EItemType.Scepter)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    // 统率攻击力 增加
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseAttackByCommand451, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            if (mWeaponEquipment.Type == EItemType.Scepter)
//                                            {
//                                                int mProperty_CommandValue = b_Component.GetNumerial(E_GameProperty.Property_Command);
//                                                // 装备权杖时统率属性每 38.56 攻击力增加1
//                                                mResult += mProperty_CommandValue / mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.MinAtteck:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {

//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    // 物理攻击力提高
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_WeaponMaster127, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    // 最小伤害值 大师增益
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMinAttack157, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 暴风之翼攻击强化
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Attack_BaoFengZhiYi_120, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 暴风之翼
//                                            if (mWingEquipment.ConfigID == 220035)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }

//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.ShuangShouJian_Strengthening144, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手剑强化
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.DanShouJian_Strengthening145, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手剑强化
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手剑强化
//                                            if (mShieldWeaponEquipment.Type == EItemType.Swords && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Hammers_Strengthening146, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 锤类强化
//                                            if (mWeaponEquipment.Type == EItemType.Maces)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 锤类强化
//                                            if (mShieldWeaponEquipment.Type == EItemType.Maces)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Spear_Strengthening147, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 矛类强化
//                                            if (mWeaponEquipment.Type == EItemType.Spears)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 矛类强化
//                                            if (mShieldWeaponEquipment.Type == EItemType.Spears)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    // 物理攻击力提高
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_WeaponMaster233, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    // 最小伤害值 大师增益
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMinAttack264, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 时空之翼攻击强化
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Attack_ShiKongZhiYi_221, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 时空之翼
//                                            if (mWingEquipment.ConfigID == 220033)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }

//                                    var mBattleMasteryKey = E_BattleMasteryState.Arch_Strengthening254;
//                                    if (b_Component.BattleMasteryDic.TryGetValue(mBattleMasteryKey, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 弓
//                                            if (mWeaponEquipment.Type == EItemType.Bows)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    mBattleMasteryKey = E_BattleMasteryState.Crossbow_Master258;
//                                    if (b_Component.BattleMasteryDic.TryGetValue(mBattleMasteryKey, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 怒类装备时对目标的攻击力追加增加{0:F}。
//                                            if (mWeaponEquipment.Type == EItemType.Crossbows)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    // 物理攻击力提高
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.WeaponMaster332, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                    // 最大伤害值 大师增益
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMinAttack365, out mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 毁灭之翼攻击强化
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Attack_HuiMieZhiYi_322, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 破灭之翼
//                                            if (mWingEquipment.ConfigID == 220032)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    // 双手剑强化
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.ShuangShouJian_Strengthening353, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手剑
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    // 单手剑强化
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.DanShouJian_Strengthening354, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手剑
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手剑
//                                            if (mShieldWeaponEquipment.Type == EItemType.Swords && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }

//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.MaxMagicAtteck:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    // 魔法精通
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.MagicMaster32))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.MagicMaster32];
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseMaxMagic61))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseMaxMagic61];
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 时空之翼攻击强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Equipment_Attack_ShiKongZhiYi_21))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 时空之翼
//                                            if (mWingEquipment.ConfigID == 220034)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Equipment_Attack_ShiKongZhiYi_21];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DanShouJian_Strengthening47))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手杖
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening47];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手杖
//                                            if (mShieldWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening47];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ShuangShouJian_Strengthening48))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手杖
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ShuangShouJian_Strengthening48];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Shield_Strengthening49))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 盾牌
//                                            if (mShieldWeaponEquipment.Type == EItemType.Shields)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Shield_Strengthening49];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {

//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {

//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    // 魔法精通
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.MagicMaster335))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.MagicMaster335];
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseMaxMagic367))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseMaxMagic367];
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 毁灭之翼攻击强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Equipment_Attack_HuiMieZhiYi_322))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 破灭之翼
//                                            if (mWingEquipment.ConfigID == 220032)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Equipment_Attack_HuiMieZhiYi_322];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DanShouJian_Strengthening355))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手杖
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening355];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手杖
//                                            if (mShieldWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening355];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ShuangShouJian_Strengthening356))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手杖
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ShuangShouJian_Strengthening356];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.MinMagicAtteck:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    // 魔法精通
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.MagicMaster32))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.MagicMaster32];
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseMinMagic58))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseMinMagic58];
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 时空之翼攻击强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Equipment_Attack_ShiKongZhiYi_21))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 时空之翼
//                                            if (mWingEquipment.ConfigID == 220034)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Equipment_Attack_ShiKongZhiYi_21];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DanShouJian_Strengthening47))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手杖
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening47];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手杖
//                                            if (mShieldWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening47];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ShuangShouJian_Strengthening48))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手杖
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ShuangShouJian_Strengthening48];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Shield_Strengthening49))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 盾牌
//                                            if (mShieldWeaponEquipment.Type == EItemType.Shields)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Shield_Strengthening49];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {

//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {

//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    // 魔法精通
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.MagicMaster335))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.MagicMaster335];
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseMinMagic364))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseMinMagic364];
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 毁灭之翼攻击强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Equipment_Attack_HuiMieZhiYi_322))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 破灭之翼
//                                            if (mWingEquipment.ConfigID == 220032)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Equipment_Attack_HuiMieZhiYi_322];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DanShouJian_Strengthening355))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手杖
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening355];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手杖
//                                            if (mShieldWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Strengthening355];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ShuangShouJian_Strengthening356))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手杖
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ShuangShouJian_Strengthening356];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.MaxElementAtteck:
//                    break;
//                case E_GameProperty.MinElementAtteck:
//                    break;
//                case E_GameProperty.AtteckSuccessRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseAtteckSuccessRate24))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseAtteckSuccessRate24];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseAtteckSuccessRate122))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseAtteckSuccessRate122];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseAtteckSuccessRate224))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseAtteckSuccessRate224];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseAtteckSuccessRate325))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseAtteckSuccessRate325];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.PVPAtteckSuccessRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.PVPAtteckSuccessRate46))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.PVPAtteckSuccessRate46];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.PVPAtteckSuccessRate143))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.PVPAtteckSuccessRate143];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.PVPAtteckSuccessRate253))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.PVPAtteckSuccessRate253];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.PVPAtteckSuccessRate352))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.PVPAtteckSuccessRate352];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.PVPAtteckSuccessRate443))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.PVPAtteckSuccessRate443];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.PVPAtteckSuccessRate542))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.PVPAtteckSuccessRate542];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ElementAtteckSuccessRate:
//                    break;
//                case E_GameProperty.PVPElementAtteckSuccessRate:
//                    break;
//                case E_GameProperty.DefenseRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Shield_Master52))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 盾牌
//                                            if (mShieldWeaponEquipment.Type == EItemType.Shields)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Shield_Master52];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }

//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {

//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Shield_Master259))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 盾牌
//                                            if (mWeaponEquipment.Type == EItemType.Shields)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Shield_Master259];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 盾牌
//                                            if (mShieldWeaponEquipment.Type == EItemType.Shields)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Shield_Master259];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }

//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {

//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ElementDefenseRate:
//                    break;
//                case E_GameProperty.PVPDefenseRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.PVPDefenseRate2, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.PVPDefenseRate102, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.PVPDefenseRate202, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.PVPDefenseRate302, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.PVPDefenseRate402, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.PVPDefenseRate502, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.PVPElementDefenseRate:
//                    break;
//                case E_GameProperty.Defense:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseDefense8))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseDefense8];
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseElementDefense9))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseElementDefense9];
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 时空之翼防御强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Equipment_Defense_ShiKongZhiYi_19))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 时空之翼
//                                            if (mWingEquipment.ConfigID == 220034)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Equipment_Defense_ShiKongZhiYi_19];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    // 整套防御力增加
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.SpecialDefenseRate113))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();

//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseDefense108))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseDefense108];
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 暴风之翼防御强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Equipment_Defense_BaoFengZhiYi_119))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 暴风之翼
//                                            if (mWingEquipment.ConfigID == 220035)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Equipment_Defense_BaoFengZhiYi_119];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }
//                                    // 整套防御力增加
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.SpecialDefenseRate113))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();

//                                    }

//                                    // 在身上有技能buffer
//                                    if (b_Component.HealthStatsDic.ContainsKey(E_BattleSkillStats.JiQiangDeXinNian114))
//                                    {
//                                        // 坚强的信念强化
//                                        if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Skill_JianQiangDeXinNian125))
//                                        {
//                                            int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Skill_JianQiangDeXinNian125];
//                                            mResult += mMasteryValue;
//                                        }
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseDefense208))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseDefense208];
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 时空之翼防御强化
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Equipment_Defense_ShiKongZhiYi_219))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 时空之翼
//                                            if (mWingEquipment.ConfigID == 220033)
//                                            {
//                                                int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Equipment_Defense_ShiKongZhiYi_219];
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Shield_Strengthening256))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 盾牌强化
//                                            if (mWeaponEquipment.Type == EItemType.Shields)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Shield_Strengthening256];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 盾牌强化
//                                            if (mShieldWeaponEquipment.Type == EItemType.Shields)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Shield_Strengthening256];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    // 整套防御力增加
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.SpecialDefenseRate113))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();

//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseDefense308, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }

//                                    // 毁灭之翼防御强化
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Defense_HuiMieZhiYi_319, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 破灭之翼
//                                            if (mWingEquipment.ConfigID == 220032)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }

//                                    // 整套防御力增加
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.SpecialDefenseRate113))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();

//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseDefense408, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }

//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Defense_DiWangPiFeng420, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 帝王披风
//                                            if (mWingEquipment.ConfigID == 220031)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }


//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseDefense508, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }

//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Defense_DiWangPiFeng420, out mMasteryValue))
//                                    {
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
//                                        {
//                                            // 帝王披风
//                                            if (mWingEquipment.ConfigID == 220030)
//                                            {
//                                                mResult += mMasteryValue;
//                                            }
//                                        }
//                                    }


//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ElementDefense:
//                    break;
//                case E_GameProperty.AttackSpeed:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DanShouJian_Master50))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手杖精通
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Master50];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手杖精通
//                                            if (mShieldWeaponEquipment.Type == EItemType.Staffs && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Master50];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DanShouJian_master149))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手剑精通
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_master149];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手剑精通
//                                            if (mShieldWeaponEquipment.Type == EItemType.Swords && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_master149];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Arch_Master257))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 弓类精通
//                                            if (mWeaponEquipment.Type == EItemType.Bows)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Arch_Master257];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 弓类精通
//                                            if (mShieldWeaponEquipment.Type == EItemType.Bows)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Arch_Master257];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DanShouJian_master358))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手剑精通
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_master358];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手剑精通
//                                            if (mShieldWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_master358];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DanShouJian_Master359))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 单手杖精通
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Master359];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 单手杖精通
//                                            if (mShieldWeaponEquipment.Type == EItemType.Staffs && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DanShouJian_Master359];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.MoveSpeed:
//                    break;
//                case E_GameProperty.MoveSpeed_Increase:
//                    break;
//                case E_GameProperty.MoveSpeed_Reduce:
//                    break;
//                case E_GameProperty.AttackDistance:
//                    break;
//                case E_GameProperty.SkillAddition:
//                    break;
//                case E_GameProperty.PVPAttack:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ShuangShouJian_Master51))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手杖
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ShuangShouJian_Master51];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    var mBattleMasteryKey = E_BattleMasteryState.Crossbow_Strengthening255;
//                                    if (b_Component.BattleMasteryDic.ContainsKey(mBattleMasteryKey))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 怒类装备时对目标的攻击力追加增加{0:F}。
//                                            if (mWeaponEquipment.Type == EItemType.Crossbows)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[mBattleMasteryKey];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.PVPDefense:
//                    break;
//                case E_GameProperty.PVEAttack:
//                    break;
//                case E_GameProperty.PVEDefense:
//                    break;
//                case E_GameProperty.ReallyDefense:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_22, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_121, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_222, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_323, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_423, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_521, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ReplyHp:
//                    break;
//                case E_GameProperty.ReplyMp:
//                    break;
//                case E_GameProperty.ReplyAG:
//                    break;
//                case E_GameProperty.ReplySD:
//                    break;
//                case E_GameProperty.ReplyHpRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp7, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp107, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp207, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp307, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp407, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp507, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ReplyMpRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryMagic4, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryMagic104, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryMagic204, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryMagic304, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryMagic404, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryMagic504, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ReplyAGRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryAG10, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryAG110, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryAG210, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryAG310, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryAG410, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryAG510, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ReplySDRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoverySD6, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoverySD106, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoverySD206, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoverySD306, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoverySD406, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoverySD506, out var mMasteryValue))
//                                    {
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.KillEnemyReplyHpRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryHp56, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryHp155, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryHp262, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoverySD362, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoverySD454, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoverySD548, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.KillEnemyReplyMpRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryMp59, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryMp158, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryMp265, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryMp366, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryMp459, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryMp553, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.KillEnemyReplyAGRate:
//                    {
//                    }
//                    break;
//                case E_GameProperty.KillEnemyReplySDRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoverySD55, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoverySD154, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoverySD261, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoverySD362, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoverySD454, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoverySD548, out var mBattleMasteryValue))
//                                    {
//                                        mResult += mBattleMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ReplyAllHpRate:
//                    break;
//                case E_GameProperty.ReplyAllMpRate:
//                    break;
//                case E_GameProperty.ReplyAllAGRate:
//                    break;
//                case E_GameProperty.ReplyAllSdRate:
//                    break;
//                case E_GameProperty.Injury_ReplyAllHpRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Injure_RecoveryAllHp64))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Injure_RecoveryAllHp64];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Injure_RecoveryAllHp163))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Injure_RecoveryAllHp163];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Injure_RecoveryAllHp269))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Injure_RecoveryAllHp269];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Injure_RecoveryAllHp371))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Injure_RecoveryAllHp371];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.Injury_ReplyAllMpRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Injure_RecoveryAllMp63))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Injure_RecoveryAllMp63];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Injure_RecoveryAllMp162))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Injure_RecoveryAllMp162];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Injure_RecoveryAllMp268))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Injure_RecoveryAllMp268];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Injure_RecoveryAllMp370))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Injure_RecoveryAllMp370];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.Attack_ReplyAllSdRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Attack_RecoveryAllSD68))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Attack_RecoveryAllSD68];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Attack_RecoveryAllSD167))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Attack_RecoveryAllSD167];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Attack_RecoveryAllSD273))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Attack_RecoveryAllSD273];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Attack_RecoveryAllSD374))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Attack_RecoveryAllSD374];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.HpAbsorbRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.AttackAbsorbHp65))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.AttackAbsorbHp65];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.AttackAbsorbHp164))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.AttackAbsorbHp164];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.AttackAbsorbHp270))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.AttackAbsorbHp270];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.AttackAbsorbHp372))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.AttackAbsorbHp372];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.SdAbsorbRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Attack_RecoverySD70))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Attack_RecoverySD70];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Attack_RecoverySD169))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Attack_RecoverySD169];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Attack_RecoverySD275))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Attack_RecoverySD275];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Attack_RecoverySD376))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Attack_RecoverySD376];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.MpConsumeRate_Reduce:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ReduceConsumeMp54))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ReduceConsumeMp54];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ReduceConsumeMp153))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ReduceConsumeMp153];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ReduceConsumeMp260))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ReduceConsumeMp260];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ReduceConsumeMp361))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ReduceConsumeMp361];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.AgConsumeRate_Reduce:
//                    break;
//                case E_GameProperty.AttackIgnoreDefenseRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IgnoreDefenseRate71))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IgnoreDefenseRate71];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IgnoreDefenseRate170))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IgnoreDefenseRate170];
//                                        mResult += mMasteryValue;
//                                    }

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Hammers_master150))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 矛
//                                            if (mWeaponEquipment.Type == EItemType.Maces)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Hammers_master150];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 矛
//                                            if (mShieldWeaponEquipment.Type == EItemType.Maces)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Hammers_master150];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }


//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IgnoreDefenseRate276))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IgnoreDefenseRate276];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IgnoreDefenseRate377))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IgnoreDefenseRate377];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ReboundRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.BackInjureRate14))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.BackInjureRate14];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.BackInjureRate114))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.BackInjureRate114];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.BackInjureRate214))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.BackInjureRate214];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.BackInjureRate314))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.BackInjureRate314];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.BackInjureRate415))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.BackInjureRate415];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.BackInjureRate514))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.BackInjureRate514];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.LucklyAttackHurtValueIncrease:
//                    {

//                    }
//                    break;
//                case E_GameProperty.ExcellentAttackHurtValueIncrease:
//                    break;
//                case E_GameProperty.InjuryValueRate_Increase:
//                    break;
//                case E_GameProperty.InjuryValueRate_Reduce:
//                    break;
//                case E_GameProperty.InjuryValue_Reduce:
//                    break;
//                case E_GameProperty.BackInjuryRate:
//                    break;
//                case E_GameProperty.HurtValueAbsorbRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.HurtValueAbsorbRate230, out var mMasteryValue))
//                                    {// 
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.HitSdRate:
//                    break;
//                case E_GameProperty.AttackSdRate:
//                    break;
//                case E_GameProperty.SDAttackIgnoreRate:
//                    break;
//                case E_GameProperty.ShacklesRate:
//                    break;
//                case E_GameProperty.ShacklesResistanceRate:
//                    break;
//                case E_GameProperty.ShieldHurtAbsorb:
//                    break;
//                case E_GameProperty.DefenseShieldRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DefensiveShield_23))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DefensiveShield_23];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {

//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DefensiveShield_223))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DefensiveShield_223];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DefensiveShield_324))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DefensiveShield_324];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.AddGoldCoinRate_Increase:
//                    break;
//                case E_GameProperty.MagicRate_Increase:
//                    break;
//                case E_GameProperty.GridBlockRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.WuQiGridBlock320))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.WuQiGridBlock320];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.GuardShieldRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.GuardianShield20))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.GuardianShield20];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {

//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.GuardianShield220))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.GuardianShield220];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.GuardianShield321))
//                                    {
//                                        int mValue = b_Component.BattleMasteryDic[E_BattleMasteryState.GuardianShield321];
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.AttackBonus:
//                    break;
//                case E_GameProperty.DefenseBonus:
//                    break;
//                case E_GameProperty.AttackSpeedBonus:
//                    break;
//                case E_GameProperty.DefenseRateBonus:
//                    break;
//                case E_GameProperty.HealthBonus:
//                    break;
//                case E_GameProperty.MagicBonus:
//                    break;
//                case E_GameProperty.SkillAttack:
//                    break;
//                case E_GameProperty.MaxDamnationAtteck:
//                    break;
//                case E_GameProperty.MinDamnationAtteck:
//                    break;
//                case E_GameProperty.DamnationRate_Increase:
//                    break;
//                case E_GameProperty.PROP_HP_MAXPct:
//                    break;
//                case E_GameProperty.PROP_MP_MAXPct:
//                    break;
//                case E_GameProperty.PROP_SD_MAXPct:
//                    break;
//                case E_GameProperty.PROP_AG_MAXPct:
//                    break;
//                case E_GameProperty.MaxAtteckPct:
//                    break;
//                case E_GameProperty.MinAtteckPct:
//                    break;
//                case E_GameProperty.MaxMagicAtteckPct:
//                    break;
//                case E_GameProperty.MinMagicAtteckPct:
//                    break;
//                case E_GameProperty.MaxElementAtteckPct:
//                    break;
//                case E_GameProperty.MinElementAtteckPct:
//                    break;
//                case E_GameProperty.AtteckSuccessRatePct:
//                    break;
//                case E_GameProperty.PVPAtteckSuccessRatePct:
//                    break;
//                case E_GameProperty.ElementAtteckSuccessRatePct:
//                    break;
//                case E_GameProperty.PVPElementAtteckSuccessRatePct:
//                    break;
//                case E_GameProperty.DefenseRatePct:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DefenseRate12))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DefenseRate12];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DefenseRate112))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DefenseRate112];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DefenseRate212))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DefenseRate212];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DefenseRate312))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DefenseRate312];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DefenseRate412))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DefenseRate412];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.DefenseRate512))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.DefenseRate512];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ElementDefenseRatePct:
//                    break;
//                case E_GameProperty.PVPDefenseRatePct:
//                    break;
//                case E_GameProperty.PVPElementDefenseRatePct:
//                    break;
//                case E_GameProperty.DefensePct:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseElementDefense9, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseElementDefense109, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseElementDefense209, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseElementDefense309, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseElementDefense409, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseElementDefense509, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ElementDefensePct:
//                    break;
//                case E_GameProperty.MaxDamnationAtteckPct:
//                    break;
//                case E_GameProperty.MinDamnationAtteckPct:
//                    break;
//                case E_GameProperty.KillMonsterReplyHp_8:
//                    break;
//                case E_GameProperty.KillMonsterReplyMp_8:
//                    break;
//                case E_GameProperty.DamageAbsPct_Guard:
//                    break;
//                case E_GameProperty.DamageAbsPct_Mounts:
//                    break;
//                case E_GameProperty.DamageAbsPct_Wing:
//                    break;
//                case E_GameProperty.MpsDownDur1:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable1_1, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable1_101, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable1_201, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable1_301, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable1_401, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable1_501, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.MpsDownDur2:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable2_5, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable2_105, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable2_205, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable2_305, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable2_405, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.MpsDownDur3:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable3_11, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable3_111, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable3_211, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable3_311, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Reduce_Durable3_411, out var mValue))
//                                    {
//                                        mResult += mValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.MpsPetDurDownSpeed:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Holyteacher:
//                                if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.PetsDurableStrengthen457, out var mValue))
//                                {
//                                    mResult += mValue;
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.SpecialDefenseRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.SpecialDefenseRate13))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.SpecialDefenseRate13];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.SpecialDefenseRate113))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.SpecialDefenseRate113];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.SpecialDefenseRate213))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.SpecialDefenseRate213];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.SpecialDefenseRate313))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.SpecialDefenseRate313];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.SpecialDefenseRate414))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.SpecialDefenseRate414];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.SpecialDefenseRate513))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.SpecialDefenseRate513];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.NullAttack:
//                    break;
//                case E_GameProperty.InjuryValueRate_2:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseShuangBeiYiJiRate69))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseShuangBeiYiJiRate69];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseShuangBeiYiJiRate168))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseShuangBeiYiJiRate168];
//                                        mResult += mMasteryValue;
//                                    }

//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.Spear_master151))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 矛
//                                            if (mWeaponEquipment.Type == EItemType.Spears)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Spear_master151];
//                                            }
//                                        }
//                                        if (mMasteryValue == 0 && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
//                                        {
//                                            // 矛
//                                            if (mShieldWeaponEquipment.Type == EItemType.Spears)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.Spear_master151];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }


//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseShuangBeiYiJiRate274))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseShuangBeiYiJiRate274];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseShuangBeiYiJiRate274))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseShuangBeiYiJiRate274];
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ShuangShouJian_Strengthening357))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手剑精通
//                                            if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ShuangShouJian_Strengthening357];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.ShuangShouJian_Master360))
//                                    {
//                                        int mMasteryValue = 0;
//                                        var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
//                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
//                                        {
//                                            // 双手杖精通
//                                            if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 1)
//                                            {
//                                                mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.ShuangShouJian_Master360];
//                                            }
//                                        }
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.InjuryValueRate_3:
//                    break;
//                case E_GameProperty.LucklyAttackRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseXingYunYiJiRate62))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseXingYunYiJiRate62];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseXingYunYiJiRate161))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseXingYunYiJiRate161];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseXingYunYiJiRate267))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseXingYunYiJiRate267];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseXingYunYiJiRate369))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseXingYunYiJiRate369];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseXingYunYiJiRate463))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseXingYunYiJiRate463];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseXingYunYiJiRate557))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseXingYunYiJiRate557];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.ExcellentAttackRate:
//                    {
//                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
//                        {
//                            case E_GameOccupation.Spell:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseZuoYueYiJiRate66))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseZuoYueYiJiRate66];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Swordsman:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseZuoYueYiJiRate165))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseZuoYueYiJiRate165];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Archer:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseZuoYueYiJiRate271))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseZuoYueYiJiRate271];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Spellsword:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseZuoYueYiJiRate373))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseZuoYueYiJiRate373];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.Holyteacher:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseZuoYueYiJiRate467))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseZuoYueYiJiRate467];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            case E_GameOccupation.SummonWarlock:
//                                {
//                                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.IncreaseZuoYueYiJiRate561))
//                                    {
//                                        int mMasteryValue = b_Component.BattleMasteryDic[E_BattleMasteryState.IncreaseZuoYueYiJiRate561];
//                                        mResult += mMasteryValue;
//                                    }
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                case E_GameProperty.IceResistance:
//                    break;
//                case E_GameProperty.CurseResistance:
//                    break;
//                case E_GameProperty.FireResistance:
//                    break;
//                case E_GameProperty.ThunderResistance:
//                    break;
//                case E_GameProperty.Level:
//                    break;
//                case E_GameProperty.OccupationLevel:
//                    break;
//                case E_GameProperty.UnionName:
//                    break;
//                case E_GameProperty.FreePoint:
//                    break;
//                case E_GameProperty.Exprience:
//                    break;
//                case E_GameProperty.ExprienceDrop:
//                    break;
//                case E_GameProperty.ServerTime:
//                    break;
//                case E_GameProperty.GoldCoin:
//                    break;
//                case E_GameProperty.GoldCoinChange:
//                    break;
//                case E_GameProperty.ExperienceBonus:
//                    break;
//                case E_GameProperty.ExplosionRate:
//                    break;
//                case E_GameProperty.GoldCoinMarkup:
//                    break;
//                case E_GameProperty.MiracleCoin:
//                    break;
//                case E_GameProperty.MiracleChange:
//                    break;
//                case E_GameProperty.YuanbaoCoin:
//                    break;
//                case E_GameProperty.YuanbaoChange:
//                    break;
//                case E_GameProperty.PlayerKillingMedel:
//                    break;
//                case E_GameProperty.PkNumber:
//                    break;
//                case E_GameProperty.GamePropertyMax:
//                    break;

//                default:
//                    break;
//            }
//            #endregion

//            return mResult;
//        }
//    }
//}