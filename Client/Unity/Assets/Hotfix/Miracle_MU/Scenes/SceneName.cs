using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public enum SceneName
    {
        /// <summary>选择角色</summary>
        ChooseRole = 0,
        /// <summary>勇者大陆</summary>
        YongZheDaLu = 1,
        /// <summary>仙踪林</summary>
        XianZongLin = 2,
        /// <summary>幻术园</summary>
        HuanShuYuan = 3,
        /// <summary>冰风谷</summary>
        BingFengGu = 4,
        /// <summary>地下城</summary>
        DiXiaCheng = 5,
        /// <summary>亚特兰蒂斯</summary>
        YaTeLanDiSi = 6,
        /// <summary>失落之塔</summary>
        ShiLuoZhiTa = 7,
        /// <summary>死亡沙漠</summary>
        SiWangShaMo = 8,
        /// <summary>幽暗森林</summary>
        YouAnSenLin = 9,
        /// <summary>天空之城</summary>
        TianKongZhiCheng = 10,
        /// <summary>狼魂要塞</summary>
        LangHunYaoSai = 11,
        /// <summary>坎特鲁废墟</summary>
        KanTeLuFeiXu = 12,
        /// <summary>坎特鲁遗址</summary>
        KanTeLuYiZhi = 13,
        /// <summary>冰霜之城</summary>
        BingShuangZhiCheng = 14,
        /// <summary>冰霜之城-孵化魔地</summary>
        FuHuaMoDi = 15,
        /// <summary>安宁池</summary>
        AnNingChi = 16,
        /// <summary>无名岛</summary>
        WuMingDao = 17,
        /// <summary>菲利亚</summary>
        Feiliya = 18,

        /// <summary>血色城堡</summary>
        XueSeChengBao = 100,
        /// <summary>恶魔广场</summary>
        EMoGuangChang = 101, 
        /// <summary>古战场</summary>
        GuZhanChang = 102,
        /// <summary>古战场</summary>
        kalima_map = 103,
        /// <summary>藏宝图</summary>b
        cangbaotu = 110,
        /// <summary>试炼之地</summary>
        ShiLianZhiDi = 111,
        /// <summary>古战场2</summary>
        GuZhanChang2 = 112,

    }
    public static class SceneNameExtension 
    {

        /// <summary>
        /// 获取场景名字
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static string GetSceneName(this SceneName sceneName)
        {
            return sceneName switch
            {
                SceneName.ChooseRole => "选择角色",
                SceneName.YongZheDaLu => "勇者大陆",
                SceneName.XianZongLin => "仙踪林",
                SceneName.HuanShuYuan => "幻术园",
                SceneName.BingFengGu => "冰风谷",
                SceneName.DiXiaCheng => "地下城",
                SceneName.YaTeLanDiSi => "亚特兰蒂斯",
                SceneName.ShiLuoZhiTa => "失落之塔",
                SceneName.YouAnSenLin => "幽暗森林",
                SceneName.SiWangShaMo => "死亡沙漠",
                SceneName.TianKongZhiCheng => "天空之城",
                SceneName.LangHunYaoSai => "狼魂要塞",
                SceneName.KanTeLuFeiXu => "坎特鲁废墟",
                SceneName.KanTeLuYiZhi => "坎特鲁遗址",
                SceneName.BingShuangZhiCheng => "冰霜之城",
                SceneName.FuHuaMoDi => "冰霜之城-孵化魔地",
                SceneName.AnNingChi => "安宁池",
                SceneName.XueSeChengBao => "血色城堡",
                SceneName.EMoGuangChang => "恶魔广场",
                SceneName.GuZhanChang => "古战场",
                SceneName.kalima_map => "卡利玛神庙",
                SceneName.cangbaotu => "藏宝图",
                SceneName.WuMingDao => "囚禁之岛",
                SceneName.Feiliya => "菲利亚",
                SceneName.ShiLianZhiDi => "试炼之地",
                SceneName.GuZhanChang2 => "古战场2",
                _ => string.Empty,
            };
        }
        /// <summary>
        /// 字符串转Enum
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string str) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), str);
        }
        /// <summary>
        /// Int整形 转Enum
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <typeparam name="A">整形</typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnum<T,A>(this A str)
        {
            return (T)Enum.Parse(typeof(T),str.ToString());
        }

        /// <summary>
        /// 枚举转字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string EnumToString<T>(this T name) where T : Enum
        {
            return Enum.GetName(typeof(T), name);
        } 
       
    }
}
