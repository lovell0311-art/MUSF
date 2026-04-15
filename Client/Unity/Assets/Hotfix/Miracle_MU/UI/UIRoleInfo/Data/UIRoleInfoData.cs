using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ETHotfix
{
    public class RoleAttribute
    {
        /// <summary>
        /// 力量
        /// </summary>
        public string Property_Strength { get; set; }
        /// <summary>
        /// 攻击力
        /// </summary>
        public string Property_Atteck { get; set; }
        /// <summary>
        /// 攻击成功率
        /// </summary>
        public string AtteckSuccessRate { get; set; }
        /// <summary>
        /// PVP攻击率
        /// </summary>
        public string PVPSuccessRate { get; set; }
        /// <summary>
        /// 敏捷
        /// </summary>
        public string Property_Agility { get; set; }
        /// <summary>
        /// 防御力
        /// </summary>
        public string DefensivePower { get; set; }
        /// <summary>
        /// 攻击速率
        /// </summary>
        public string AttackRate { get; set; }
        /// <summary>
        /// 防御率
        /// </summary>
        public string DefenseRate { get; set; }
        /// <summary>
        /// PVP防御力
        /// </summary>
        public string PVPDefenseRate { get; set; }
        /// <summary>
        /// 体力
        /// </summary>
        public string Property_PhysicalStrength { get; set; }
        /// <summary>
        /// 智力
        /// </summary>
        public string Property_Intelligence { get; set; }
        /// <summary>
        /// 技能攻击力%
        /// </summary>
        public string SkillAttack { get; set; }
        /// <summary>
        /// 魔力
        /// </summary>
        public string Property_Magic { get; set; }
    }
    /// <summary>
    /// 推荐加点
    /// </summary>
    public class RecommendAddPoint
    {
        public int One { get; set; }
        public int Two { get; set; }
        public int Three { get; set; }
        public int Four { get; set; }
    }
    public static class UIRoleInfoData
    {
        public static RoleEntity roleEntity = UnitEntityComponent.Instance.LocalRole;
        public static RecommendAddPoint GetRoleRecommendAddPoint(int CanUsePoint)
        {
            RecommendAddPoint addPoint = new RecommendAddPoint();
            if (CanUsePoint <= 3 && CanUsePoint >= 0)
            {
                addPoint.One = CanUsePoint;
                addPoint.Three = 0;
                addPoint.Two = 0;
            }
            else if (CanUsePoint == 4)
            {
                addPoint.One = CanUsePoint - 1;
                addPoint.Three = 1;
                addPoint.Two = 0;
            }
            else if (CanUsePoint >= 5)
            {
                addPoint.One = (int)Math.Ceiling((float)CanUsePoint * 3 / 5);
                addPoint.Three = (int)Math.Floor((float)CanUsePoint * 1 / 5);
                addPoint.Two = CanUsePoint - addPoint.One - addPoint.Three;
            }
            addPoint.Four = 0;
            return addPoint;
        }
        public static Dictionary<string, int> RecommendkeyValues = new Dictionary<string, int>();
        public static void RecommendAddPointInit()
        {
            GetRoleProperties().Coroutine();

        }
        public static void RecommendAddPointPetInit(PetAttributeSystem petAttribute, int petsLVpoint, out Dictionary<string, int> recommendkeyValue)
        {
            RecommendAddPoint recommendAdd = GetRoleRecommendAddPoint(petsLVpoint);
            recommendkeyValue = new Dictionary<string, int>();
            switch (petAttribute)
            {
                case PetAttributeSystem.Magic:
                    recommendkeyValue["intell"] = recommendAdd.One;
                    recommendkeyValue["agile"] = recommendAdd.Two;
                    recommendkeyValue["PhysicalStrength"] = recommendAdd.Three;
                    break;
                case PetAttributeSystem.Physics:
                    recommendkeyValue["power"] = recommendAdd.One;
                    recommendkeyValue["agile"] = recommendAdd.Two;
                    recommendkeyValue["PhysicalStrength"] = recommendAdd.Three;
                    break;
                default:
                    break;
            }
        }

        static async ETVoid GetRoleProperties()
        {
            G2C_PlayerPropertyResponse c2G_Player = (G2C_PlayerPropertyResponse)await SessionComponent.Instance.Session.Call(new C2G_PlayerPropertyRequest { SelectId = 0 });
            if (c2G_Player.Error != 0)
            {
                Log.DebugRed($"获取玩家属性报错：{c2G_Player.Error.GetTipInfo()}");
            }
            else
            {
                // UIRoleInfoInfo.ZhuanShengCount = c2G_Player.Reincarnate;
                foreach (G2C_BattleKVData item in c2G_Player.Info)
                {
                    // Log.DebugBrown($"玩家属性：{item.Key} -> {item.Value}");
                    UnitEntityComponent.Instance.LocalRole.Property.Set(item);
                    if ((E_GameProperty)item.Key == E_GameProperty.FreePoint)
                    {
                        // Log.DebugGreen($"等级点数：{item.Value}");
                        UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.FreePoint, item.Value);
                        UIMainComponent.Instance.SetArributeRedDot(item.Value > 0);
                        RecommendAddPoint recommendAdd = GetRoleRecommendAddPoint((int)UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.FreePoint));
                        switch (UnitEntityComponent.Instance.LocalRole.RoleType)
                        {
                            case E_RoleType.Magician:
                                RecommendkeyValues["Intell"] = recommendAdd.One;
                                RecommendkeyValues["Agile"] = recommendAdd.Two;
                                RecommendkeyValues["PhyStrength"] = recommendAdd.Three;
                                RecommendkeyValues["Strength"] = recommendAdd.Four;
                                break;
                            case E_RoleType.Swordsman:
                                RecommendkeyValues["Strength"] = recommendAdd.One;
                                RecommendkeyValues["Agile"] = recommendAdd.Two;
                                RecommendkeyValues["PhyStrength"] = recommendAdd.Three;
                                RecommendkeyValues["Intell"] = recommendAdd.Four;
                                break;
                            case E_RoleType.Archer:
                                RecommendkeyValues["Agile"] = recommendAdd.One;
                                RecommendkeyValues["Strength"] = recommendAdd.Two;
                                RecommendkeyValues["PhyStrength"] = recommendAdd.Three;
                                RecommendkeyValues["Intell"] = recommendAdd.Four;
                                break;
                            default:
                                break;
                        }
                        return;
                    }
                }
            }


        }
        public static void GetAttribute(RecommendAddPoint recommendAdd, out RoleAttribute roleAttribute)
        {
            roleAttribute = new RoleAttribute();

            switch (roleEntity.RoleType)
            {
                case E_RoleType.Magician:
                case E_RoleType.Magicswordsman:
                case E_RoleType.Summoner:
                    //推荐加点的点数
                    int Property_Intelligence_Magician = recommendAdd.One;
                    int Property_Agility_Magician = recommendAdd.Two;
                    int Property_PhysicalStrength_Magician = recommendAdd.Three;
                    int Property_Strength_Magician = recommendAdd.Four;

                    roleAttribute.Property_Strength = $"{Property_Strength_Magician}";//--------------力量
                    roleAttribute.Property_Agility = $"{Property_Agility_Magician}";//--------------敏捷
                    roleAttribute.Property_PhysicalStrength = $"{Property_PhysicalStrength_Magician}";//--------------体力
                    roleAttribute.Property_Intelligence = $"{Property_Intelligence_Magician}";//--------------智力

                    long Property_Atteck_After_Magician = roleEntity.Property.GetProperValue(E_GameProperty.Property_Strength) + Property_Strength_Magician;//加点后的力量
                    long Property_Agility_After_Magician = roleEntity.Property.GetProperValue(E_GameProperty.Property_Agility) + Property_Agility_Magician;//加点后的敏捷
                    long AtteckSuccessRate_After_Magician = (int)(roleEntity.Level * 5 + Property_Agility_After_Magician * 1.5f + Property_Atteck_After_Magician / 4);//加点后的成功率
                    long AtteckSuccessRate_Magician = (int)(roleEntity.Level * 5 + (Property_Agility_After_Magician - Property_Agility_Magician) * 1.5f + (Property_Atteck_After_Magician - Property_Strength_Magician) / 4);//加点前的成功率

                    long PVPSuccessRateAfter_Magician = (int)(roleEntity.Level * 3 + Property_Agility_After_Magician * 4);//加点后的PVP攻击率
                    long PVPSuccessRate_Magician = (int)(roleEntity.Level * 3 + (Property_Agility_After_Magician - Property_Agility_Magician) * 4);//加点前的PVP攻击率

                    long MaxProperty_MagicAfter_Magician = (long)(1.5 + (Property_Intelligence_Magician + roleEntity.Property.GetProperValue(E_GameProperty.Property_Willpower)) / 4);
                    long MaxProperty_Magic_Magician = (long)(1.5 + roleEntity.Property.GetProperValue(E_GameProperty.Property_Willpower) / 4);

                    long MinProperty_MagicAfter_Magician = (Property_Intelligence_Magician + roleEntity.Property.GetProperValue(E_GameProperty.Property_Willpower)) / 9;
                    long MinProperty_Magic_Magician = roleEntity.Property.GetProperValue(E_GameProperty.Property_Willpower) / 9;

                    roleAttribute.Property_Atteck = $" +{(Property_Atteck_After_Magician / 8) - (roleEntity.Property.GetProperValue(E_GameProperty.Property_Strength) / 8)} ~ {(Property_Atteck_After_Magician / 4) - (roleEntity.Property.GetProperValue(E_GameProperty.Property_Strength) / 4)}";//--------------攻击力
                    roleAttribute.AtteckSuccessRate = $" +{AtteckSuccessRate_After_Magician - AtteckSuccessRate_Magician}";//--------------攻击成功率
                    roleAttribute.PVPSuccessRate = $" +{PVPSuccessRateAfter_Magician - PVPSuccessRate_Magician}";//--------------PVP攻击率
                    roleAttribute.DefensivePower = $" +{(Property_Agility_After_Magician / 4) - ((Property_Agility_After_Magician - Property_Agility_Magician) / 4)}";//--------------防御力
                    roleAttribute.AttackRate = $" +{(Property_Agility_After_Magician / 10) - ((Property_Agility_After_Magician - Property_Agility_Magician) / 10)}";//--------------攻击速度
                    roleAttribute.DefenseRate = $" +{(Property_Agility_After_Magician / 3) - ((Property_Agility_After_Magician - Property_Agility_Magician) / 3)}";//--------------防御率+
                    roleAttribute.PVPDefenseRate = $" +{(roleEntity.Level * 2 + Property_Agility_After_Magician / 4) - (roleEntity.Level * 2 + (Property_Agility_After_Magician - Property_Agility_Magician) / 4)}";//--------------PVP防御率
                    roleAttribute.Property_Magic = $" +{(MinProperty_MagicAfter_Magician - MinProperty_Magic_Magician)} ~ {MaxProperty_MagicAfter_Magician - MaxProperty_Magic_Magician}";//--------------PVP防御率
                    break;
                case E_RoleType.Swordsman:
                case E_RoleType.Holymentor:
                case E_RoleType.Gladiator:
                case E_RoleType.GrowLancer:
                    //推荐加点的点数
                    int Property_Strength_Swordsman = recommendAdd.One;
                    int Property_Agility_Swordsman = recommendAdd.Two;
                    int Property_PhysicalStrength_Swordsman = recommendAdd.Three;
                    int Property_Intelligence_Swordsman = recommendAdd.Four;

                    roleAttribute.Property_Strength = $"{Property_Strength_Swordsman}";//--------------力量
                    roleAttribute.Property_Agility = $"{Property_Agility_Swordsman}";//--------------敏捷
                    roleAttribute.Property_PhysicalStrength = $"{Property_PhysicalStrength_Swordsman}";//--------------体力
                    roleAttribute.Property_Intelligence = $"{Property_Intelligence_Swordsman}";//--------------智力

                    long Property_Atteck_After_Swordsman = roleEntity.Property.GetProperValue(E_GameProperty.Property_Strength) + Property_Strength_Swordsman;//加点后的力量
                    long Property_Agility_After_Swordsman = roleEntity.Property.GetProperValue(E_GameProperty.Property_Agility) + Property_Agility_Swordsman;//加点后的敏捷
                    long AtteckSuccessRate_After_Swordsman = (int)(roleEntity.Level * 5 + Property_Agility_After_Swordsman * 1.5f + Property_Atteck_After_Swordsman / 4);//加点后的成功率
                    long AtteckSuccessRate_Swordsman = (int)(roleEntity.Level * 5 + (Property_Agility_After_Swordsman - Property_Agility_Swordsman) * 1.5f + (Property_Atteck_After_Swordsman - Property_Strength_Swordsman) / 4);//加点前的成功率

                    long PVPSuccessRateAfter_Swordsman = (int)(roleEntity.Level * 3 + Property_Agility_After_Swordsman * 4.5);//加点后的PVP攻击率
                    long PVPSuccessRate_Swordsman = (int)(roleEntity.Level * 3 + (Property_Agility_After_Swordsman - Property_Agility_Swordsman) * 4.5);//加点前的PVP攻击率

                    roleAttribute.Property_Atteck = $" +{(Property_Atteck_After_Swordsman / 6) - (roleEntity.Property.GetProperValue(E_GameProperty.Property_Strength) / 6)} ~ {(Property_Atteck_After_Swordsman / 4) - (roleEntity.Property.GetProperValue(E_GameProperty.Property_Strength) / 4)}";//--------------攻击力
                    roleAttribute.AtteckSuccessRate = $" +{AtteckSuccessRate_After_Swordsman - AtteckSuccessRate_Swordsman}";//--------------攻击成功率
                    roleAttribute.PVPSuccessRate = $" +{PVPSuccessRateAfter_Swordsman - PVPSuccessRate_Swordsman}";//--------------PVP攻击率
                    roleAttribute.DefensivePower = $" +{(Property_Agility_After_Swordsman / 3) - ((Property_Agility_After_Swordsman - Property_Agility_Swordsman) / 3)}";//--------------防御力
                    roleAttribute.AttackRate = $" +{(Property_Agility_After_Swordsman / 15) - ((Property_Agility_After_Swordsman - Property_Agility_Swordsman) / 15)}";//--------------攻击速度
                    roleAttribute.DefenseRate = $" +{(Property_Agility_After_Swordsman / 3) - ((Property_Agility_After_Swordsman - Property_Agility_Swordsman) / 3)}";//--------------防御率+
                    roleAttribute.PVPDefenseRate = $" +{(roleEntity.Level * 2 + Property_Agility_After_Swordsman / 2) - (roleEntity.Level * 2 + (Property_Agility_After_Swordsman - Property_Agility_Swordsman) / 2)}";//--------------PVP防御率
                    roleAttribute.SkillAttack = $" +{Property_Intelligence_Swordsman / 10}%";//--------------PVP防御率
                    break;
                case E_RoleType.Archer:
                    //推荐加点的点数
                    int Property_Agility_Archer = recommendAdd.One;
                    int Property_Strength_Archer = recommendAdd.Two;
                    int Property_PhysicalStrength_Archer = recommendAdd.Three;
                    int Property_Intelligence_Archer = recommendAdd.Four;

                    roleAttribute.Property_Strength = $"{Property_Strength_Archer}";//--------------力量
                    roleAttribute.Property_Agility = $"{Property_Agility_Archer}";//--------------敏捷
                    roleAttribute.Property_PhysicalStrength = $"{Property_PhysicalStrength_Archer}";//--------------体力
                    roleAttribute.Property_Intelligence = $"{Property_Intelligence_Archer}";//--------------智力

                    long Property_Atteck_After_Archer = roleEntity.Property.GetProperValue(E_GameProperty.Property_Strength) + Property_Strength_Archer;//加点后的力量
                    long Property_Agility_After_Archer = roleEntity.Property.GetProperValue(E_GameProperty.Property_Agility) + Property_Agility_Archer;//加点后的敏捷
                    long AtteckSuccessRate_After_Archer = (int)(roleEntity.Level * 5 + Property_Agility_After_Archer * 1.5f + Property_Atteck_After_Archer / 4);//加点后的成功率
                    long AtteckSuccessRate_Archer = (int)(roleEntity.Level * 5 + (Property_Agility_After_Archer - Property_Agility_Archer) * 1.5f + (Property_Atteck_After_Archer - Property_Strength_Archer) / 4);//加点前的成功率

                    long PVPSuccessRateAfter_Archer = (int)(roleEntity.Level * 3 + Property_Agility_After_Archer * 0.6f);//加点后的PVP攻击率
                    long PVPSuccessRate_Archer = (int)(roleEntity.Level * 3 + (Property_Agility_After_Archer - Property_Agility_Archer) * 0.6f);//加点前的PVP攻击率

                    if (IsGongNu())
                    {
                        roleAttribute.Property_Atteck = $" +{(int)(Property_Atteck_After_Archer / 14 + Property_Agility_After_Archer / 8) - (int)((Property_Atteck_After_Archer - Property_Strength_Archer) / 14 + (Property_Agility_After_Archer - Property_Agility_Archer) / 8)} ~ " +
                       $"{(int)(Property_Atteck_After_Archer / 8 + Property_Agility_After_Archer / 4) - (int)((Property_Atteck_After_Archer - Property_Strength_Archer) / 8 + (Property_Agility_After_Archer - Property_Agility_Archer) / 4)}";//--------------攻击力
                    }
                    else
                    {
                        roleAttribute.Property_Atteck = $" +{(int)(Property_Atteck_After_Archer / 7 + Property_Agility_After_Archer / 7) - (int)((Property_Atteck_After_Archer - Property_Strength_Archer) / 7 + (Property_Agility_After_Archer - Property_Agility_Archer) / 7)} ~ " +
                       $"{(int)(Property_Atteck_After_Archer / 4 + Property_Agility_After_Archer / 4) - (int)((Property_Atteck_After_Archer - Property_Strength_Archer) / 4 + (Property_Agility_After_Archer - Property_Agility_Archer) / 4)}";//--------------攻击力
                    }

                    roleAttribute.AtteckSuccessRate = $" +{AtteckSuccessRate_After_Archer - AtteckSuccessRate_Archer}";//--------------攻击成功率
                    roleAttribute.PVPSuccessRate = $" +{PVPSuccessRateAfter_Archer - PVPSuccessRate_Archer}";//--------------PVP攻击率
                    roleAttribute.DefensivePower = $" +{(Property_Agility_After_Archer / 10) - ((Property_Agility_After_Archer - Property_Agility_Archer) / 10)}";//--------------防御力
                    roleAttribute.AttackRate = $" +{(Property_Agility_After_Archer / 50) - ((Property_Agility_After_Archer - Property_Agility_Archer) / 50)}";//--------------攻击速度
                    roleAttribute.DefenseRate = $" +{(Property_Agility_After_Archer / 4) - ((Property_Agility_After_Archer - Property_Agility_Archer) / 4)}";//--------------防御率
                    roleAttribute.PVPDefenseRate = $" +{(roleEntity.Level * 2 + Property_Agility_After_Archer / 10) - (roleEntity.Level * 2 + (Property_Agility_After_Archer - Property_Agility_Archer) / 10)}";//--------------PVP防御率
                    break;
                default:
                    break;
            }

            //弓箭手是否装备了弓弩
            bool IsGongNu()
            {
                List<KnapsackDataItem> knapsackData = roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic.Values.ToList();
                for (int i = 0, length = knapsackData.ToList().Count; i < length; i++)
                {
                    if (knapsackData[i].ItemType == (int)E_ItemType.Bows || knapsackData[i].ItemType == (int)E_ItemType.Crossbows)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }

}
