using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// 技能状态
    /// </summary>
    public enum E_BattleSkillStats
    {
        /// <summary>
        /// 无敌 不收伤害
        /// </summary>
        WuDi = 1,

        /// <summary>
        /// 新手buff
        /// </summary>
        XinShouBuff = 2,

        /// <summary>
        /// 双倍经验
        /// </summary>
        ShuangBeiJingYan = 3,


        /// <summary>
        /// 冰封
        /// </summary>
        BingFeng = 7,
        /// <summary>
        /// 毒咒
        /// </summary>
        Curse = 8,
        /// <summary>
        /// 守护之魂
        /// </summary>
        ShouHuZhiHun = 14,
        /// <summary>
        ///防御护罩
        /// </summary>
        FangYuHuZhao = 15,
        /// <summary>
        /// 法神附体
        /// </summary>
        FaShenFuTi23 = 23,




        /// <summary>
        /// 圣盾防御
        /// </summary>
        HolyShieldDefense102 = 102,
        /// <summary>
        /// 生命之光
        /// </summary>
        ShengMingZhiGuang110 = 110,
        /// <summary>
        /// 剑之愤怒
        /// </summary>
        JianZhiFenNu112 = 112,

        /// <summary>
        /// 坚强的信念
        /// </summary>
        JiQiangDeXinNian114 = 114,
        /// <summary>
        /// 坚固的庇护
        /// </summary>
        JiGuDeBiHu115 = 115,


        /// <summary>
        /// 弓箭手 - 守护之光
        /// </summary>
        ShouHuZhiGuang203 = 203,
        /// <summary>
        /// 弓箭手 - 战神之力
        /// </summary>
        ZhanShenZhiLi204 = 204,

        /// <summary>
        /// 弓箭手 - 冰封箭
        /// </summary>
        BingFengJian214 = 14 + 200,
        /// <summary>
        /// 弓箭手 - 无影箭
        /// </summary>
        WuYingJian216 = 16 + 200,

        /// <summary>
        /// 祝福
        /// </summary>
        ZhuFu224 = 24 + 200,
        /// <summary>
        /// 毒箭
        /// </summary>
        Curse225 = 25 + 200,
        /// <summary>
        /// 闪避
        /// </summary>
        ShanBi228 = 28 + 200,

        /// <summary>
        /// 玄月斩
        /// </summary>
        XuanYueZhan321 = 21 + 300,
        /// <summary>
        /// 地牢术
        /// </summary>
        DiLaoShu326 = 26 + 300,

        /// <summary>
        /// 致命圣印
        /// </summary>
        ZhiMingShengYin410 = 10 + 400,

        /// <summary>
        /// 昏睡术
        /// </summary>
        HunShuiShu502 = 2 + 500,
        /// <summary>
        /// 爆裂击
        /// </summary>
        LieBaoJi503 = 3 + 500,
        /// <summary>
        /// 安魂弥撒
        /// </summary>
        AnHunMiSha505 = 5 + 500,
        /// <summary>
        /// 伤害反射
        /// </summary>
        ShangHaiFanShe506 = 6 + 500,
        /// <summary>
        /// 狂暴术
        /// </summary>
        KuangBaoShu509 = 9 + 500,
        /// <summary>
        /// 虚弱阵
        /// </summary>
        XuRuoZhen510 = 10 + 500,

        /// <summary>
        /// 破御阵
        /// </summary>
        PoYuZhen511 = 11 + 500,
        /// <summary>
        /// 烈袭
        /// </summary>
        LieXi522 = 22 + 500,
        /// <summary>
        /// 聚灵
        /// </summary>
        JuLing524 = 24 + 500,
        /// <summary>
        /// 影煞
        /// </summary>
        YingSha523 = 23 + 500,
        /// <summary>
        /// 蒙眼煞
        /// </summary>
        MengYanSha526 = 26 + 500,


        /// <summary>
        /// 幽冥青狼拳
        /// </summary>
        YouMingQingLangQuan601 = 1 + 600,
        /// <summary>
        /// 斗气爆裂拳
        /// </summary>
        DouQiBaoLieQuan602 = 2 + 600,
        /// <summary>
        /// 斗神破
        /// </summary>
        DouShenPo607 = 7 + 600,
        /// <summary>
        /// 斗神命
        /// </summary>
        DouShenMing608 = 8 + 600,
        /// <summary>
        /// 斗神-御
        /// </summary>
        DouShenYu609 = 9 + 600,

        /// <summary>
        /// 神圣气旋
        /// </summary>
        ShenShengQiXuan611 = 11 + 600,
        /// <summary>
        /// 鲜血咆哮
        /// </summary>
        XianXuePaoXiao616 = 16 + 600,


        /// <summary>
        /// 惩戒之盾
        /// </summary>
        ChengJieZhiDun703 = 3 + 700,
        /// <summary>
        /// 黑曜石
        /// </summary>
        HeiYaoShi704 = 4 + 700,
        /// <summary>
        /// 狂怒
        /// </summary>
        KuangNu709 = 9 + 700,

        /// <summary>
        /// 破御圣言
        /// </summary>
        PoYuShengYan713 = 13 + 700,


        /// <summary>
        /// 加速卷轴
        /// </summary>
        Use310018 = 18 + 310000,
        /// <summary>
        /// 防御卷轴
        /// </summary>
        Use310019 = 19 + 310000,
        /// <summary>
        /// 愤怒卷轴
        /// </summary>
        Use310020 = 20 + 310000,
        /// <summary>
        /// 魔力卷轴
        /// </summary>
        Use310021 = 21 + 310000,
        /// <summary>
        /// 体力卷轴
        /// </summary>
        Use310022 = 22 + 310000,
        /// <summary>
        /// 魔法卷轴
        /// </summary>
        Use310023 = 23 + 310000,
        /// <summary>
        /// 力量圣水
        /// </summary>
        Use310024 = 24 + 310000,
        /// <summary>
        /// 敏捷圣水
        /// </summary>
        Use310025 = 25 + 310000,
        /// <summary>
        /// 体力圣水
        /// </summary>
        Use310026 = 26 + 310000,
        /// <summary>
        /// 智力圣水
        /// </summary>
        Use310027 = 27 + 310000,
        /// <summary>
        /// 统率圣水
        /// </summary>
        Use310028 = 28 + 310000,
        /// <summary>
        /// 樱花酒
        /// </summary>
        UseYingHuaJiu310029 = 29 + 310000,
        /// <summary>
        /// 幸运一击卷轴
        /// </summary>
        Use310031 = 31 + 310000,
        /// <summary>
        /// 卓越一击卷轴
        /// </summary>
        Use310032 = 32 + 310000,
        /// <summary>
        /// 恢复卷轴
        /// </summary>
        Use310034 = 34 + 310000,

        /// <summary>
        /// 光之祝福（高级）
        /// </summary>
        UseGuangZhiZhuFu310059 = 59 + 310000,
        /// <summary>
        /// 光之祝福（低级）
        /// </summary>
        UseGuangZhiZhuFu310060 = 60 + 310000,
        /// <summary>
        /// 光之祝福（中级）
        /// </summary>
        UseGuangZhiZhuFu310061 = 61 + 310000,
        /// <summary>
        /// 樱花饼
        /// </summary>
        UseYingHuaBing310062 = 62 + 310000,
        /// <summary>
        /// 樱花花瓣
        /// </summary>
        UseYingHuaHuaBan310063 = 63 + 310000,

        /// <summary>
        /// 经验印章
        /// </summary>
        Use310069 = 69 + 310000,
        /// <summary>
        /// 经验印章
        /// </summary>
        Use310070 = 70 + 310000,

        /// <summary>
        /// 
        /// </summary>
        Use310103 = 103 + 310000,

        Use310114 = 114 + 310000,
        /// <summary>
        /// 攻击力提升
        /// </summary>
        Use310116 = 116 + 310000, 
        /// <summary>
        /// 防御力提升
        /// </summary>
        Use310117 = 117 + 310000,
        /// <summary>
        /// 生命提升
        /// </summary>
        Use310118 = 118 + 310000,
        /// <summary>
        /// 伤害提升
        /// </summary>
        Use310119 = 119 + 310000,
        /// <summary>
        /// 伤害吸收提升
        /// </summary>
        Use310120 = 120 + 310000,
        /// <summary>
        /// 攻速增加+30%卷轴
        /// </summary>
        Use310127 = 127 + 310000,
        /// <summary>
        /// 攻击力增加+10%卷轴
        /// </summary>
        Use310128,
        /// <summary>
        /// 生命力增加+10%卷轴
        /// </summary>
        Use310129,
        /// <summary>
        /// 防御力增加+10%卷轴
        /// </summary>
        Use310130,
        /// <summary>
        /// 魔力增加+10%卷轴
        /// </summary>
        Use310131,
        /// <summary>
        /// SD防御增加+10%卷轴
        /// </summary>
        Use310132,
        /// <summary>
        /// 生命力恢复卷轴
        /// </summary>
        Use310133,
        /// <summary>
        /// 魔力恢复卷轴
        /// </summary>
        Use310134,
        /// <summary>
        /// AG恢复卷轴
        /// </summary>
        Use310135,
        Use310140 = 140 + 310000,
        /// <summary>
        /// 倍数经验
        /// </summary>
        Use310157 = 310000 + 157,
    }
}