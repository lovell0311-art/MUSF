using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static class EquipmentComponentSystem_Numerial
    {

        /// <summary>
        /// 武器耐久掉落
        /// </summary>
        /// <param name="self"></param>
        /// <param name="targetUnit"></param>
        public static void WeaponDurDown(this EquipmentComponent self, CombatSource targetUnit)
        {
            Item right = self.GetEquipItemByPosition(EquipPosition.Weapon);
            Item left = self.GetEquipItemByPosition(EquipPosition.Shield);
            Item target = null;
            #region 选择一把武器
            if (left == null || left.IsWeapon())
            {
                if (right != null) target = right;
            }
            else
            {
                if (right == null)
                {
                    target = left;
                }
                else
                {
                    if (RandomHelper.RandomNumber(0, 2) == 0)
                    {
                        target = right;
                    }
                    else
                    {
                        target = left;
                    }
                }
            }
            #endregion
            if (target == null) return;
            GamePlayer gamePlayer = self.mPlayer.GetCustomComponent<GamePlayer>();
            int defance;
            bool durChanged = false;
            switch (target.Type)
            {
                case EItemType.Swords:
                case EItemType.Axes:
                case EItemType.Maces:
                case EItemType.Scepter:
                case EItemType.FistBlade:
                case EItemType.MagicSword:
                case EItemType.ShortSword:
                    {
                        // 普通武器
                        defance = targetUnit.GetNumerialFunc(E_GameProperty.Defense);
                        durChanged = target.NormalWeaponDurabilityDown(defance, gamePlayer);
                    }
                    break;
                case EItemType.Bows:
                case EItemType.Crossbows:
                    {
                        // 弓弩
                        defance = targetUnit.GetNumerialFunc(E_GameProperty.Defense);
                        durChanged = target.BowWeaponDurabilityDown(defance, gamePlayer);
                    }
                    break;
                case EItemType.Staffs:
                case EItemType.MagicBook:
                case EItemType.RuneWand:
                case EItemType.MagicGun:
                    {
                        // 魔杖
                        defance = targetUnit.GetNumerialFunc(E_GameProperty.Defense);
                        durChanged = target.StaffWeaponDurabilityDown(defance, gamePlayer);
                    }
                    break;
                default:
                    break;
            }
            if (durChanged)
            {
                // 耐久变了，需要更新装备属性
                self.ApplyEquipProp();
                // TODO 通知玩家新属性
                // ...
            }
        }

        /// <summary>
        /// 防具随机掉落耐久
        /// </summary>
        /// <param name="self"></param>
        /// <param name="attackUnit"></param>
        public static void ArmorRandomDurDown(this EquipmentComponent self, CombatSource attackUnit)
        {
            EquipPosition itemPos = (EquipPosition)RandomHelper.RandomNumber(2, 7);
            Item target = self.GetEquipItemByPosition(itemPos);
            if (target == null) return;

            if (itemPos == EquipPosition.Shield)
            {
                if (target.Type != EItemType.Shields)
                {
                    // 左手拿的不是防具
                    target = null;
                }
            }
            if (target != null)
            {
                GamePlayer gamePlayer = self.mPlayer.GetCustomComponent<GamePlayer>();
                int minAttackDamage = attackUnit.GetNumerialFunc(E_GameProperty.MinAtteck);
                if (target.ArmorDurabilityDown(minAttackDamage, gamePlayer))
                {
                    // 物品耐久变动
                    self.ApplyEquipProp();
                    // TODO 通知玩家，属性变动
                }
            }
        }



    }
}
