using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{

    /// <summary>
    /// 颜色管理类
    /// </summary>
    public class ColorTools
    {
        public static Color YellowColor = new Color(1, 208 / 255f, 0);
        public static Color BlueColor = new Color(134 / 255f, 152 / 255f, 231 / 255f);
        public static Color GreenColor = new Color(152 / 255f, 1, 0);
        public static Color GlodYellowColor = new Color(1, 1, 0);
        public static Color ZiSeColor = new Color(222 / 255f, 108 / 255f, 215 / 255f);
        public static Color MinRedColor = new Color(1, 120 / 255f, 120 / 255f, 1);
        public static Color RedColor = Color.red;

        #region 套装颜色
        /// <summary>套装标题 黄色</summary>
        public const string Suit_Title = "#937612";
        /// <summary>套装无效 灰色</summary>
        public const string Suit_Invalid = "#595655";
        /// <summary>套装装备生效 绿色</summary>
        public const string Suit_Equip_Effective = "#01e700";
        /// <summary>套装属性生效 蓝色</summary>
        public const string Suit_Atr_Effective = "#658dc9";
        #endregion
        #region 装备名字颜色
        /// <summary>普通装备</summary>
        public const string NormalItemNameColor = "#f5f5f5";
        /// <summary>卓越属性</summary>
        public const string ExcellenceItemColor = "#38b641";
        /// <summary>幸运属性</summary>
        public const string LuckyItemColor = "#8698E7";
        /// <summary>380属性</summary>
        public const string ATTR380ItemColor = "#8698E7";
        /// <summary>再生属性</summary>
        public const string ZaiShengItemColor = "#ba9512";
        /// <summary>能使用的装备</summary>
        public const string CanUserItemColor = "#98FF00";
        /// <summary>不能使用的装备</summary>
        public const string CanNotUserItemColor = "#F23118";
        /// <summary>不能穿戴的装备</summary>
        public const string CanNotWareItemColor = "#F23118";
        /// <summary>套装</summary>
        public const string NotActiveItemColor = "#808A87"; 
        /// <summary>绑定</summary>
        public const string IsBindItemColor = "#b000a7";
       
        #endregion


        /// <summary>普通角色名字颜色</summary>
        public static Color RoleNameNormalColor = new Color(1, 220 / 255f, 0);

        /// <summary>NPC名字颜色</summary>
        public static Color NPCNameColor = new Color(147 / 255f, 250 / 255f, 236 / 255f);

        /// <summary>宠物名字颜色</summary>
        public static Color PetNameColor = new Color(0, 250 / 255f, 154 / 255f);

        /// <summary>二阶宠物名字颜色</summary>
        public static Color PetSecondNameColor = new Color(255 / 255f, 192 / 255f, 203 / 255f);

        /// <summary>三阶宠物名字颜色</summary>
        public static Color PetThirdNameColor = new Color(255f, 0f, 255f);

        /// <summary>怪物名字颜色</summary>
        public static Color MonsterNameColor = new Color(1f, 1f, 1f);

        /// <summary>物品名字颜色</summary>
        public static Color ItemNameColor = new Color(1f, 1f, 1f);


        /// <summary>套装物品名字颜色</summary>
        public static Color ItemSetNameColor = new Color(1, 208 / 255f, 0);

        /// <summary>召唤物名字颜色</summary>
        public static Color SummonNameColor = new Color(1f, 1f, 1f);

        /// <summary>
        /// 十六进制转换为color
        /// </summary>
        /// <param name="colorStr">十六进制的色值</param>
        /// <returns>Color</returns>
        public static Color GetColorHtmlString(string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out Color color);
            return color;
        }
        /// <summary>
        /// color转换为十六进制
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>颜色 十六进制值</returns>

        public static string GetColorHtmlString(Color color)
        {

            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }

        
    }
}