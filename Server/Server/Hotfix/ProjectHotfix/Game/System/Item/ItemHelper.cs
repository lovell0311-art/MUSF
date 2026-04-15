using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETHotfix.ItemUpdateProp;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace ETHotfix
{
    /// <summary>
    /// 物品类型、种类、属性 判断，都写在这里，方便统一管理 Have/Is
    /// </summary>
    public static partial class ItemHelper
    {
        #region Have
        /// <summary>
        /// 有卓越属性
        /// </summary>
        public static bool HaveExcellentOption(this Item self)
        {
            return self.data.ExcellentEntry.Count > 0;
        }
        /// <summary>
        /// 有套装属性
        /// </summary>
        public static bool HaveSetOption(this Item self)
        {
            return self.GetProp(EItemValue.SetId) > 0;
        }
        /// <summary>
        /// 有380属性
        /// </summary>
        public static bool Have380Option(this Item self)
        {
            return false;
        }
        /// <summary>
        /// 有镶嵌孔洞
        /// </summary>
        public static bool HaveEnableSocket(this Item self)
        {
            return self.GetProp(EItemValue.FluoreSlotCount) > 0;
        }
        /// <summary>
        /// 有耐久
        /// </summary>
        public static bool HaveDurability(this Item self)
        {
            if (self.GetProp(EItemValue.Durability) == 0 && self.GetProp(EItemValue.DurabilityMax) == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 有幸运属性
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool HaveLuckyAttr(this Item self)
        {
            if (self.GetProp(EItemValue.LuckyEquip) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  有技能
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool HaveSkill(this Item self)
        {
            return self.GetProp(EItemValue.SkillId) > 0;
        }

        #endregion

        #region Is

        /// <summary>
        /// 是翅膀
        /// </summary>
        public static bool IsWing(this Item self)
        {
            return self.Type == EItemType.Wing;
        }
        /// <summary>
        /// 是宠物
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsPets(this Item self)
        {
            switch (self.Type)
            {
                case EItemType.Pets:
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 是一代翅膀
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsFirstWing(this Item self)
        {
            return self.ConfigData.WingLevel == 10;
        }

        /// <summary>
        /// 是二代翅膀
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsSecondWing(this Item self)
        {
            return self.ConfigData.WingLevel == 20;
        }

        /// <summary>
        /// 是三代翅膀
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsThirdWing(this Item self)
        {
            return self.ConfigData.WingLevel == 30;
        }

        /// <summary>
        /// 是四代翅膀
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsFourthWing(this Item self)
        {
            return self.ConfigData.WingLevel == 40;
        }

        /// <summary>
        /// 是武器
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsWeapon(this Item self)
        {
            switch (self.Type)
            {
                case EItemType.Swords:
                case EItemType.Axes:
                case EItemType.Maces:
                case EItemType.Bows:
                case EItemType.Crossbows:
                case EItemType.Spears:
                case EItemType.Staffs:
                case EItemType.MagicBook:
                case EItemType.Scepter:
                case EItemType.RuneWand:
                case EItemType.FistBlade:
                case EItemType.MagicSword:
                case EItemType.ShortSword:
                case EItemType.MagicGun:
                case EItemType.Flag:
                case EItemType.Necklace:
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 是饰品
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsAccessory(this Item self)
        {
            switch (self.Type)
            {
                case EItemType.Flag:
                case EItemType.Dangler:
                case EItemType.Rings:
                case EItemType.Necklace:
                case EItemType.Bracelet:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 是防具
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsArmor(this Item self)
        {
            switch (self.Type)
            {
                case EItemType.Shields:
                case EItemType.Helms:
                case EItemType.Armors:
                case EItemType.Pants:
                case EItemType.Gloves:
                case EItemType.Boots:
                case EItemType.Bracelet:
                case EItemType.Rings:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 是盾牌
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsShield(this Item self)
        {
            switch (self.Type)
            {
                case EItemType.Shields:
                    return true;
            }
            return false;
        }


        /// <summary>
        /// 是幸运装备
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsLuckyEquip(this Item self)
        {
            return self.HaveLuckyAttr();
        }


        /// <summary>
        /// 是镶嵌装备
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsSocketEquip(this Item self)
        {
            return self.HaveEnableSocket();
        }
        #endregion

        #region Get

        /// <summary>
        /// 获取物品品质,仅用于前端显示地面物品名
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int GetQuality(this Item self)
        {
            int ret = 0;
            if (self.HaveSkill())
            {
                ret |= 1;
            }
            if (self.HaveLuckyAttr())
            {
                ret |= 1 << 2;
            }
            if (self.HaveExcellentOption())
            {
                ret |= 1 << 3;
            }
            if (self.HaveSetOption())
            {
                ret |= 1 << 4;
            }
            if (self.HaveEnableSocket())
            {
                ret |= 1 << 5;
            }
            return ret;
        }

        /// <summary>
        /// 获取装备对应职业的技能id
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerTypeId">职业id</param>
        /// <returns></returns>
        public static int GetEquipSkillId(this Item self, E_GameOccupation playerTypeId)
        {
            int skillId = self.GetProp(EItemValue.SkillId);
            switch (playerTypeId)
            {
                case E_GameOccupation.Swordsman:
                    switch (skillId)
                    {
                        case 123: skillId = 123; break;
                    }
                    break;
                case E_GameOccupation.Archer:
                    switch (skillId)
                    {
                        case 123: skillId = 221; break;
                    }
                    break;
                case E_GameOccupation.Spell:
                    switch (skillId)
                    {
                        case 123: skillId = 11; break;
                    }
                    break;
                case E_GameOccupation.Spellsword:
                    switch (skillId)
                    {
                        case 102: skillId = 314; break;
                        case 103: skillId = 315; break;
                        case 104: skillId = 316; break;
                        case 105: skillId = 317; break;
                        case 106: skillId = 318; break;
                        case 107: skillId = 319; break;
                        case 123: skillId = 330; break;
                        default:
                            break;
                    }
                    break;
                case E_GameOccupation.Holyteacher:
                    switch (skillId)
                    {
                        case 102: skillId = 401; break;
                        case 103: skillId = 402; break;
                        case 104: skillId = 403; break;
                        case 105: skillId = 404; break;
                        case 106: skillId = 405; break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return skillId;
        }

        /// <summary>普通装备</summary>
        const string NormalItemNameColor = "#f5f5f5";
        /// <summary>卓越属性</summary>
        const string ExcellenceItemColor = "#38b641";
        /// <summary>套装</summary>
        public const string SetItemColor = "#56abda";
        public static string GetClientName(this Item self)
        {
            string lev = self.GetProp(EItemValue.Level) > 0 ? " +" + self.GetProp(EItemValue.Level) : "";
            string itemName;
            if (self.GetProp(EItemValue.SetId) != 0)
            {
                // 套装
                string setName = "未知的";
                ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
                if (readConfig.GetJson<SetItem_TypeConfigJson>().JsonDic.TryGetValue(self.GetProp(EItemValue.SetId), out SetItem_TypeConfig setConfig))
                {
                    setName = setConfig.SetName;
                }
                if(self.HaveExcellentOption())
                {
                    itemName = $"<color={SetItemColor}>卓越的 {setName} {self.ConfigData.Name}{lev}</color>";
                }
                else
                {
                    itemName = $"<color={SetItemColor}>{setName} {self.ConfigData.Name}{lev}</color>";
                }
            }
            else if (self.HaveExcellentOption())
            {
                // 卓越
                itemName = $"<color={ExcellenceItemColor}>卓越的 {self.ConfigData.Name}{lev}</color>";
            }
            else if(self.ConfigID == 320294)
            {
                // 金币
                itemName = $"<color=c0c000>{self.GetProp(EItemValue.Quantity)} 金币</color>";
            }
            else
            {
                // 普通装备
                itemName = $"<color={NormalItemNameColor}>{self.ConfigData.Name}{lev}</color>";
            }
            return itemName;
        }
        #endregion

        #region Can
        /// <summary>
        /// 可以使用物品
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gamePlayer"></param>
        /// <returns></returns>
        public static bool CanUse(this Item self, GamePlayer gamePlayer)
        {
            if (self.ConfigData.UseRole.Count != 0)
            {
                if (!self.ConfigData.UseRole.TryGetValue((int)gamePlayer.GameHeroType, out var level) || level < 0 || level > gamePlayer.Data.OccupationLevel)
                {
                    return false;
                }
            }

            if (self.ConfigData.ReqLvl > gamePlayer.Data.Level) { return false; }
            if (self.ConfigData.ReqAgi > gamePlayer.GetNumerial(E_GameProperty.Property_Agility)) { return false; }
            if (self.ConfigData.ReqCom > gamePlayer.GetNumerial(E_GameProperty.Property_Command)) { return false; }
            if (self.ConfigData.ReqEne > gamePlayer.GetNumerial(E_GameProperty.Property_Willpower)) { return false; }
            if (self.ConfigData.ReqStr > gamePlayer.GetNumerial(E_GameProperty.Property_Strength)) { return false; }
            if (self.ConfigData.ReqVit > gamePlayer.GetNumerial(E_GameProperty.Property_BoneGas)) { return false; }

            return true;
        }

        /// <summary>
        /// 可以有卓越属性
        /// </summary>
        public static bool CanHaveExcellentOption(this Item self)
        {
            // TODO 临时代码 下次更新去掉
            if (self.Type == EItemType.Guard) return true;
            return (self.ConfigData.QualityAttr & 8) == 8;
        }

        /// <summary>
        /// 可以有套装属性
        /// </summary>
        public static bool CanHaveSetOption(this Item self)
        {
            return (self.ConfigData.QualityAttr & 16) == 16;
        }

        /// <summary>
        /// 可以有镶嵌孔洞
        /// </summary>
        public static bool CanHaveEnableSocket(this Item self)
        {
            return (self.ConfigData.QualityAttr & 32) == 32;
        }

        /// <summary>
        /// 可以有幸运属性
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool CanHaveLuckyAttr(this Item self)
        {
            return (self.ConfigData.QualityAttr & 4) == 4;
        }

        /// <summary>
        ///  可以有技能
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool CanHaveSkill(this Item self)
        {
            return (self.ConfigData.QualityAttr & 1) == 1 && self.ConfigData.Skill != 0;
        }


        public static bool CanStrengthen(this Item self)
        {
            switch ((EquipPosition)self.ConfigData.Slot)
            {
                case EquipPosition.Weapon:
                case EquipPosition.Shield:
                case EquipPosition.Helmet:
                case EquipPosition.Armor:
                case EquipPosition.Leggings:
                case EquipPosition.HandGuard:
                case EquipPosition.Boots:
                case EquipPosition.Wing:
                case EquipPosition.Necklace:
                case EquipPosition.Bracelet:
                case EquipPosition.Guard:
                case EquipPosition.Flag:
                case EquipPosition.Pet:
                    return true;
                case EquipPosition.LeftRing:
                case EquipPosition.RightRing:
                    //if (self.ConfigData.IsTransRing != 0) return false;
                    return true;
                default:
                    break;
            }

            return false;
        }

        #endregion
    }
}
