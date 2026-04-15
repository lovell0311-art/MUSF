using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 角色扩展类
    /// </summary>
    public static class RoleExtend
    {
        /// <summary>
        /// 获取对应的模型资源名
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string GetRoleResName(this E_RoleType self) => (self) switch
        {
            E_RoleType.Magician => "Role_Magician",
            E_RoleType.Swordsman => "Role_Swordsman",
            E_RoleType.Archer => "Role_Archer",
            E_RoleType.Magicswordsman => "Role_Magicswordsman",
            E_RoleType.Holymentor => "Role_Holymentor",
            E_RoleType.Summoner => "Role_Summoner",
            E_RoleType.Gladiator => "Role_Gladiator",
            E_RoleType.GrowLancer => "Role_GrowLancer",
            _ => string.Empty
        };


        /// <summary>
        /// 根据角色类型与转职等级 获取对应的称号
        /// </summary>
        /// <param name="self"></param>
        /// <param name="classLevel">转职等级</param>
        /// <returns></returns>
        public static string GetRoleName(this E_RoleType self, int classLevel) => self switch
        {

            E_RoleType.Magician => classLevel == 0 ? "魔法师" : ((classLevel == 1 || classLevel == 2) ? "魔导师" : (classLevel == 3 ? "神导师" : "大法师")),//转职线路-> 魔法师-》魔导师（150级）-》神导师（400级）-》大法师（800级）
            E_RoleType.Swordsman => classLevel == 0 ? "剑士" : ((classLevel == 1 || classLevel == 2) ? "骑士" : (classLevel == 3 ? "神骑士" : "圣殿武士")),//转职线路-> 剑士-》骑士（150级）-》神骑士（400级）-》圣殿武士（800级）
            E_RoleType.Archer => classLevel == 0 ? "弓箭手" : ((classLevel == 1 || classLevel == 2) ? "圣射手" : (classLevel == 3 ? "神射手" : "精灵游侠")),
            E_RoleType.Magicswordsman => (classLevel == 0 || classLevel == 1 || classLevel == 2) ? "魔剑士" : (classLevel == 3 ? "剑圣" : "魔骑士"),
            E_RoleType.Holymentor => (classLevel == 0 || classLevel == 1 || classLevel == 2) ? "圣导师" : (classLevel == 3 ? "祭祀" : "大领主"),
            E_RoleType.Summoner => classLevel == 0 ? "召唤术师" : ((classLevel == 1 || classLevel == 2) ? "召唤导师" : (classLevel == 3 ? "召唤巫师" : "大召唤师")),
            E_RoleType.Gladiator => (classLevel == 0 || classLevel == 1 || classLevel == 2) ? "格斗家" : (classLevel == 3 ? "格斗大师" : "格斗宗师"),
            E_RoleType.GrowLancer => (classLevel == 0 || classLevel == 1 || classLevel == 2) ? "梦幻骑士" : (classLevel == 3 ? "魅影骑士" : "光辉骑士"),
            _ => string.Empty

        };
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="type">配置表中的使用专职等级</param>
        /// <returns></returns>
        public static string GetItemUserName(this E_RoleType self, int type) => self switch 
        {
            E_RoleType.Magician => type == 0 ? "魔法师" : (type <= 2 ? "魔导师" : (type == 3 ? "神导师、" : "大法师")),
            E_RoleType.Swordsman => type == 0 ? "剑士" : (type <= 2 ? "骑士" : (type == 3 ? "神骑士" : "圣殿武士")),
            E_RoleType.Archer => type == 0 ? "弓箭手" : (type <= 2 ? "圣射手" : (type == 3 ? "神射手" : "精灵游侠")),
            E_RoleType.Magicswordsman => type <= 2 ? "魔剑士" : (type == 3 ? "剑圣" : "魔骑士"),
            E_RoleType.Holymentor => type <= 2 ? "圣导师" : (type == 3 ? "祭祀" : "大领主"),
            E_RoleType.Summoner => type == 0 ? "召唤术师" : (type <= 2 ? "召唤导师" : (type == 3 ? "召唤巫师" : "大召唤师")),
            E_RoleType.Gladiator => type <= 3 ? "格斗家" : (type == 3 ? "格斗大师" : "格斗宗师"),
            E_RoleType.GrowLancer => type <= 2 ? "梦幻骑士" : (type == 3 ? "魅影骑士" : "光辉骑士"),
            // E_RoleType.Runemage => type == 0 ? "符文法师" : (type <= 2 ? "秘咒法师" : (type == 3 ? "奥术大师" : "灵魂咏者")),
            // E_RoleType.StrongWind => type == 0 ? "疾风" : (type <= 2 ? "逐影" : (type == 3 ? "碎梦" : "斩灵")),
            // E_RoleType.Gunners => type == 0 ? "火枪手" : (type <= 2 ? "狙击手" : (type == 3 ? "枪术大师" : "枪神")),
            //  E_RoleType.WhiteMagician => type == 0 ? "白发魔女" : (type <= 2 ? "光导师" : (type == 3 ? "闪耀导师" : "光明法师")),
            //E_RoleType.WomanMagician => type == 0 ? "女法师" : (type <= 2 ? "战争法师" : (type == 3 ? "大女法师" : "迷雾法师")),
            _ => string.Empty
        };

        /// <summary>
        /// 根据角色类型 判断是否是男性角色
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsMan(this E_RoleType self)
        {
            switch (self)
            {
                case E_RoleType.Magician:
                case E_RoleType.Swordsman:
                case E_RoleType.Magicswordsman:
                case E_RoleType.Holymentor:
                case E_RoleType.Gladiator:
                    return true;
                case E_RoleType.Summoner:
                case E_RoleType.Archer:
                case E_RoleType.GrowLancer:
                    return false;
            }
            return false;
        }
        /// <summary>
        /// 根据角色类型获取对应的时装
        /// </summary>
        /// <param name="self"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetFashionAssetName(this E_RoleType self, Fashion_InfoConfig config) => self switch 
        {
            E_RoleType.Magician=> config.DW,
            E_RoleType.Swordsman => config.DK,
            E_RoleType.Archer => config.ELF,
            E_RoleType.Magicswordsman => config.MG,
            E_RoleType.Holymentor => config.DL,
            E_RoleType.Summoner => config.SUM,
            E_RoleType.Gladiator => config.MONK,
            E_RoleType.GrowLancer => config.DREAM,
            E_RoleType.Runemage =>null,
            E_RoleType.StrongWind => null,
            E_RoleType.Gunners => null,
            E_RoleType.WhiteMagician => null,
            E_RoleType.WomanMagician => null,
            _=> string.Empty


        };
    }

}
