using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class BattleHeroSkillConstData
    {
        public const int SpellId = 0;
        public const int SwordsmanId = 100;
        public const int ArcherId = 200;
        public const int SpellswordId = 300;
        public const int HolyteacherId = 400;
        public const int SummonWarlockId = 500;
        public const int CombatId = 600;
        public const int DreamKnightId = 700;

        public const int EnemyId = 10000;
    }
    /// <summary>
    /// 技能 1-1000
    /// </summary>
    public enum E_HeroSkillId
    {
        #region 魔法师技能
        /// <summary>
        /// 能量球
        /// </summary>
        NengLiangQiu = 1 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 火球 fireball
        /// </summary>
        FIREBALL = 2 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 真空波
        /// </summary>
        ZhenKongBo = 3 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 掌心雷
        /// </summary>
        ZHANG_XIN_LEI = 4 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 瞬间移动
        /// </summary>
        ShunJianYiDong = 5 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 陨石 Meteorite
        /// </summary>
        METEORITE = 6 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 冰封
        /// </summary>
        BingFeng = 7 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 毒咒  Curse
        /// </summary>
        Curse = 8 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 火龙 fiery dragon
        /// </summary>
        FieryDragon = 9 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 龙卷风
        /// </summary>
        LongJuanFeng = 10 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 黑龙波
        /// </summary>
        HeiLongBo = 11 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 地狱火
        /// </summary>
        DiYuHuo = 12 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 极光
        /// </summary>
        JiGuang = 13 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 守护之魂
        /// </summary>
        ShouHuZhiHun = 14 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 爆炎
        /// </summary>
        BaoYan = 15 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 毁灭烈焰
        /// </summary>
        HuiMieLieYan = 16 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 回旋刃
        /// </summary>
        HuiXuanRen = 17 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 小挪移
        /// </summary>
        XiaoNuoYi = 18 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 暴风雪
        /// </summary>
        BaoFengXue = 19 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 毒炎
        /// </summary>
        DuYan = 20 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 星辰一怒
        /// </summary>
        XingCenYiNu = 21 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 炎狼闪电链
        /// </summary>
        YanLangShanDianLian22 = 22 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 法神附体
        /// </summary>
        FaShenFuTi = 23 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 陨星雨
        /// </summary>
        YunXingYu = 24 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 流星石
        /// </summary>
        LiuXingShi = 25 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 幽魂
        /// </summary>
        YouHun = 26 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 连击
        /// </summary>
        LianJi27 = 27 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 地牢术
        /// </summary>
        DiLaoShu28 = 28 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 一露斩
        /// </summary>
        YiLuZhan29 = 29 + BattleHeroSkillConstData.SpellId,
        /// <summary>
        /// 漫天陨石雨
        /// </summary>
        YunXingYu30 = 30 + BattleHeroSkillConstData.SpellId,

        #endregion

        #region 剑士技能
        /// <summary>
        /// 钻云枪 Cloud drill gun
        /// </summary>
        CloudDrillGun = 1 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 圣盾防御 Holy Shield defense
        /// </summary>
        Holy_Shield_Defense = 2 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 地裂斩 Ground fissure chop
        /// </summary>
        GroundFissureChop = 3 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 牙突刺
        /// </summary>
        YaTuCi = 4 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 升龙击
        /// </summary>
        ShengLongJi = 5 + BattleHeroSkillConstData.SwordsmanId,

        /// <summary>
        /// 旋风斩
        /// </summary>
        XuanFengZhan = 6 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 天地十字剑
        /// </summary>
        TianDiShiZhiJian = 7 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 霹雳回旋斩
        /// </summary>
        XuanFengHuiXuanZhan = 8 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 流星焰
        /// </summary>
        LiuXingYan = 9 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 生命之光
        /// </summary>
        ShengMingZhiGuang = 10 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 半月斩
        /// </summary>
        BanYueZhan = 11 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 剑之愤怒
        /// </summary>
        JianZhiFenNu = 12 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 逆流
        /// </summary>
        NiLiu = 13 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 坚强的信念
        /// </summary>
        JiQiangDeXinNian = 14 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 坚固的庇护
        /// </summary>
        JiGuDeBiHu = 15 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 袭风刺
        /// </summary>
        XiFengCi = 16 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 雷霆裂闪
        /// </summary>
        LeiTingLieShan = 17 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 炎狼闪电链
        /// </summary>
        YanLangShanDianLian118 = 18 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 致命一击
        /// </summary>
        ZhiMingYiJi = 19 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 强袭
        /// </summary>
        QiangXi = 20 + BattleHeroSkillConstData.SwordsmanId,


        /// <summary>
        /// 连击
        /// </summary>
        LianJi122 = 22 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 天雷闪闪
        /// </summary>
        TianLeiShanShan123 = 23 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 突袭
        /// </summary>
        TuXi124 = 24 + BattleHeroSkillConstData.SwordsmanId,
        /// <summary>
        /// 血腥风暴
        /// </summary>
        XueXingFengBao125 = 25 + BattleHeroSkillConstData.SwordsmanId,


        JianYu126 = 26 + BattleHeroSkillConstData.SwordsmanId,


        #endregion

        #region 弓箭手技能

        /// <summary>
        /// 多重箭
        /// </summary>
        DuoChongJian = 1 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 治疗
        /// </summary>
        ZhiLiao = 2 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 守护之光
        /// </summary>
        ShouHuZhiGuang = 3 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 战神之力
        /// </summary>
        ZhanShenZhiLi = 4 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 穿透箭
        /// </summary>
        ChuanTouJian = 5 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 天堂之箭
        /// </summary>
        TianTangZhiJian = 6 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 召唤 哥布林
        /// </summary>
        Call_GeBuLin = 7 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 召唤 石巨人
        /// </summary>
        Call_ShiJuRen = 8 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 召唤 暗杀者
        /// </summary>
        Call_AnShaZhe = 9 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 召唤 雪人王
        /// </summary>
        Call_XueRenWang = 10 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 召唤 暗黑骑士
        /// </summary>
        Call_AnHeiQiShi = 11 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 召唤 守护兽巴里
        /// </summary>
        Call_ShouHuShouBaLi = 12 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 召唤 黄金斗士
        /// </summary>
        Call_HuangJinDouShi = 13 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 冰封箭
        /// </summary>
        BingFengJian = 14 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 炎狼闪电链
        /// </summary>
        YanLangShanDianLian215 = 15 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 无影箭
        /// </summary>
        WuYingJian = 16 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 防护值恢复
        /// </summary>
        FangHuZhiHuiFu = 17 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 五重箭
        /// </summary>
        WuChongJian = 18 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 聚击
        /// </summary>
        JuJi = 19 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 连击
        /// </summary>
        LianJi220 = 20 + BattleHeroSkillConstData.ArcherId,


        /// <summary>
        /// 双重箭
        /// </summary>
        ShaungChongJian221 = 21 + BattleHeroSkillConstData.ArcherId,


        /// <summary>
        /// 祝福
        /// </summary>
        ZhuFu224 = 24 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 毒箭
        /// </summary>
        Curse225 = 25 + BattleHeroSkillConstData.ArcherId,


        /// <summary>
        /// 召唤 萨迪洛斯
        /// </summary>
        Call_226 = 26 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 幻影移行
        /// </summary>
        ShunJianYiDong227 = 27 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 闪避
        /// </summary>
        ShanBi228 = 28 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 三重箭
        /// </summary>
        SanChongJian229 = 29 + BattleHeroSkillConstData.ArcherId,
        /// <summary>
        /// 箭雨
        /// </summary>
        JianYu230 = 30 + BattleHeroSkillConstData.ArcherId,

        #endregion

        #region 魔剑士技能
        /// <summary>
        /// 毒咒  Curse
        /// </summary>
        Curse_301 = 1 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 陨石 Meteorite
        /// </summary>
        Meteorite302 = 2 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 掌心雷
        /// </summary>
        ZhangXinLei303 = 3 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 火球 fireball
        /// </summary>
        Fireball304 = 4 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 火龙
        /// </summary>
        FieryDragon305 = 5 + BattleHeroSkillConstData.SpellswordId,


        /// <summary>
        /// 冰封
        /// </summary>
        BingFeng306 = 6 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 龙卷风
        /// </summary>
        LongJuanFeng307 = 7 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 黑龙波
        /// </summary>
        HeiLongBo308 = 8 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 地狱火
        /// </summary>
        DiYuHuo309 = 9 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 真空波
        /// </summary>
        ZhenKongBo310 = 10 + BattleHeroSkillConstData.SpellswordId,


        /// <summary>
        /// 极光
        /// </summary>
        JiGuang311 = 11 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 爆炎
        /// </summary>
        BaoYan312 = 12 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 毁灭烈焰
        /// </summary>
        HuiMieLieYan313 = 13 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 圣盾防御
        /// </summary>
        Holy_Shield_Defense314 = 14 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 地裂斩 Ground fissure chop
        /// </summary>
        GroundFissureChop315 = 15 + BattleHeroSkillConstData.SpellswordId,


        /// <summary>
        /// 牙突刺
        /// </summary>
        YaTuCi316 = 16 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 升龙击
        /// </summary>
        ShengLongJi317 = 17 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 旋风斩
        /// </summary>
        XuanFengZhan318 = 18 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 天地十字剑
        /// </summary>
        TianDiShiZhiJian319 = 19 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 霹雳回旋斩
        /// </summary>
        XuanFengHuiXuanZhan320 = 20 + BattleHeroSkillConstData.SpellswordId,


        /// <summary>
        /// 玄月斩
        /// </summary>
        XuanYueZhan321 = 21 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 炎狼闪电链
        /// </summary>
        YanLangShanDianLian322 = 22 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 火剑袭
        /// </summary>
        HuoJianXi323 = 23 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 闪电轰顶
        /// </summary>
        ShanDianHongDing324 = 24 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 血腥风暴
        /// </summary>
        XueXingFegBao325 = 25 + BattleHeroSkillConstData.SpellswordId,

        /// <summary>
        /// 地牢术
        /// </summary>
        DiLaoShu326 = 26 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 天雷闪
        /// </summary>
        TianLeiShan327 = 27 + BattleHeroSkillConstData.SpellswordId,

        /// <summary>
        /// 连击
        /// </summary>
        LianJi329 = 29 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 天雷闪闪
        /// </summary>
        TianLeiShanShan330 = 30 + BattleHeroSkillConstData.SpellswordId,



        /// <summary>
        /// 陨星雨
        /// </summary>
        YunXingYu331 = 31 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 火血
        /// </summary>
        HuoXue332 = 32 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 冰封血
        /// </summary>
        BingFenXue333 = 33 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 冰封血
        /// </summary>
        AnBaoYan334 = 34 + BattleHeroSkillConstData.SpellswordId,

        /// <summary>
        /// 剑雨
        /// </summary>
        JianYu335 = 35 + BattleHeroSkillConstData.SpellswordId,
        /// <summary>
        /// 陨石雨
        /// </summary>
        YunXingYu336 = 36 + BattleHeroSkillConstData.SpellswordId,


        #endregion

        #region 圣导师技能
        /// <summary>
        /// 圣盾防御
        /// </summary>
        HolyShieldDefense401 = 1 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 地裂斩
        /// </summary>
        GroundFissureChop402 = 2 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 牙突刺
        /// </summary>
        YaTuCi403 = 3 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 升龙击
        /// </summary>
        ShengLongJi404 = 4 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 旋风斩
        /// </summary>
        XuanFengZhan405 = 5 + BattleHeroSkillConstData.HolyteacherId,

        /// <summary>
        /// 冲击
        /// </summary>
        ChongJi406 = 6 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 星云火链
        /// </summary>
        XingYunHuoLian407 = 7 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 地裂
        /// </summary>
        DiLie408 = 8 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 星云召唤
        /// </summary>
        XingYunZhaoHuan409 = 9 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 致命圣印
        /// </summary>
        ZhiMingShengYin410 = 10 + BattleHeroSkillConstData.HolyteacherId,

        /// <summary>
        /// 圣极光
        /// </summary>
        ShengJiGuang411 = 11 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 炎狼闪电链
        /// </summary>
        YanLangShanDianLian412 = 12 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 火舞旋风
        /// </summary>
        HuoWuXuanFeng413 = 13 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 黑暗之力
        /// </summary>
        HeiAnZhiLi414 = 14 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 风魂
        /// </summary>
        FengHun415 = 15 + BattleHeroSkillConstData.HolyteacherId,


        /// <summary>
        /// 大天使的意志
        /// </summary>
        DaTianShiDeYiZhi416 = 16 + BattleHeroSkillConstData.HolyteacherId,
        /// <summary>
        /// 连击
        /// </summary>
        LianJi417 = 17 + BattleHeroSkillConstData.HolyteacherId,

        /// <summary>
        /// 黑色天空
        /// </summary>
        HeiSeTianKong419 = 19 + BattleHeroSkillConstData.HolyteacherId,

        #endregion

        #region 召唤术士技能

        /// <summary>
        /// 摄魂咒
        /// </summary>
        SheHunZhou501 = 1 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 昏睡术
        /// </summary>
        HunShuiShu502 = 2 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 爆裂击
        /// </summary>
        LieBaoJi503 = 3 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 链雷咒
        /// </summary>
        LianLeiZhou504 = 4 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 安魂弥撒
        /// </summary>
        AnHunMiSha505 = 5 + BattleHeroSkillConstData.SummonWarlockId,

        /// <summary>
        /// 伤害反射
        /// </summary>
        ShangHaiFanShe506 = 6 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 幻兽
        /// </summary>
        HuanShou507 = 7 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 烈光闪
        /// </summary>
        LieGuangShan508 = 8 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 狂暴术
        /// </summary>
        KuangBaoShu509 = 9 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 虚弱阵
        /// </summary>
        XuRuoZhen510 = 10 + BattleHeroSkillConstData.SummonWarlockId,

        /// <summary>
        /// 破御阵
        /// </summary>
        PoYuZhen511 = 11 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 黑暗镇魂曲
        /// </summary>
        HeiAnZhenHunQu512 = 12 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 恶灵召唤
        /// </summary>
        ELingZhaoHuan513 = 13 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 地狱野兽
        /// </summary>
        DiYuYeShou514 = 14 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 水幻兽
        /// </summary>
        ShuiHuanShou515 = 15 + BattleHeroSkillConstData.SummonWarlockId,


        /// <summary>
        /// 回旋刃
        /// </summary>
        HuiXuanRen516 = 16 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 火球
        /// </summary>
        HuoQiu517 = 17 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 真空波
        /// </summary>
        ZhenKongBo518 = 18 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 陨石
        /// </summary>
        YunShi519 = 19 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 冰封
        /// </summary>
        BingFeng520 = 20 + BattleHeroSkillConstData.SummonWarlockId,


        /// <summary>
        /// 炎狼闪电链
        /// </summary>
        YanLangShanDianLian521 = 21 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 烈袭
        /// </summary>
        LieXi522 = 22 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 影煞
        /// </summary>
        YingSha523 = 23 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 聚灵
        /// </summary>
        JuLing524 = 24 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 连击
        /// </summary>
        LianJi525 = 25 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 蒙眼煞
        /// </summary>
        MengYanSha526 = 26 + BattleHeroSkillConstData.SummonWarlockId,
        /// <summary>
        /// 诱惑魅影
        /// </summary>
        YouHuoMeiYing527 = 27 + BattleHeroSkillConstData.SummonWarlockId,


        #endregion


        #region 格斗家
        /// <summary>
        /// 幽冥青狼拳
        /// </summary>
        YouMingQingLangQuan601 = 1 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 斗气爆裂拳
        /// </summary>
        DouQiBaoLieQuan602 = 2 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 回旋踢
        /// </summary>
        HuiXuanTi603 = 3 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 幽冥光速拳
        /// </summary>
        YouMingGuangSuQuan604 = 4 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 炎龙拳
        /// </summary>
        YanLongQuan605 = 5 + BattleHeroSkillConstData.CombatId,

        /// <summary>
        /// 嗜血之龙
        /// </summary>
        ShiXueZhiLong606 = 6 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 斗神-破
        /// </summary>
        DouShenPo607 = 7 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 斗神-命
        /// </summary>
        DouShenMing608 = 8 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 斗神-御
        /// </summary>
        DouShenYu609 = 9 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 冲锋
        /// </summary>
        ChongFeng610 = 10 + BattleHeroSkillConstData.CombatId,

        /// <summary>
        /// 神圣气旋
        /// </summary>
        ShenShengQiXuan611 = 11 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 灵魂勾拳
        /// </summary>
        LingHunGouQuan612 = 12 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 火凤凰气旋
        /// </summary>
        HuoFengHuangQiXuan613 = 13 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 大天使的意志
        /// </summary>
        DaTianShiYiZhi614 = 14 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 连击
        /// </summary>
        LianJi615 = 15 + BattleHeroSkillConstData.CombatId,

        /// <summary>
        /// 鲜血咆哮
        /// </summary>
        XianXuePaoXiao616 = 16 + BattleHeroSkillConstData.CombatId,
        /// <summary>
        /// 地裂斩
        /// </summary>
        GroundFissureChop617 = 17 + BattleHeroSkillConstData.CombatId,

        JianYu618 = 18 + BattleHeroSkillConstData.CombatId,

        #endregion

        #region 梦幻骑士
        /// <summary>
        /// 牙突刺
        /// </summary>
        YaTuCi701 = 1 + BattleHeroSkillConstData.DreamKnightId,
        /// <summary>
        /// 旋龙刺
        /// </summary>
        XuanLongCi702 = 2 + BattleHeroSkillConstData.DreamKnightId,
        /// <summary>
        /// 惩戒之盾
        /// </summary>
        ChengJieZhiDun703 = 3 + BattleHeroSkillConstData.DreamKnightId,
        /// <summary>
        /// 黑曜石
        /// </summary>
        HeiYaoShi704 = 4 + BattleHeroSkillConstData.DreamKnightId,
        /// <summary>
        /// 幻龙破
        /// </summary>
        HuanLongPo705 = 5 + BattleHeroSkillConstData.DreamKnightId,

        /// <summary>
        /// 猛冲
        /// </summary>
        MengChong706 = 6 + BattleHeroSkillConstData.DreamKnightId,
        /// <summary>
        /// 回旋穿刺
        /// </summary>
        HuiXuanChuanCi707 = 7 + BattleHeroSkillConstData.DreamKnightId,
        /// <summary>
        /// 飓风刺
        /// </summary>
        JuFengCi708 = 8 + BattleHeroSkillConstData.DreamKnightId,
        /// <summary>
        /// 狂怒
        /// </summary>
        KuangNu709 = 9 + BattleHeroSkillConstData.DreamKnightId,
        /// <summary>
        /// 炎舞
        /// </summary>
        YanWu710 = 10 + BattleHeroSkillConstData.DreamKnightId,


        /// <summary>
        /// 破御圣言
        /// </summary>
        PoYuShengYan713 = 13 + BattleHeroSkillConstData.DreamKnightId,

        JianYu714 = 14 + BattleHeroSkillConstData.DreamKnightId,

        #endregion


        #region 怪物技能
        /// <summary>
        /// 火球 fireball
        /// </summary>
        Fireball10001 = 1 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 冰封
        /// </summary>
        BingFeng10002 = 2 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 能量球
        /// </summary>
        NengLiangQiu10003 = 3 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 毒咒  Curse
        /// </summary>
        Curse10004 = 4 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 真空波
        /// </summary>
        ZhenKongBo10005 = 5 + BattleHeroSkillConstData.EnemyId,

        /// <summary>
        /// 陨石 Meteorite
        /// </summary>
        Meteorite10006 = 6 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 回旋刃
        /// </summary>
        HuiXuanRen10007 = 7 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 掌心雷
        /// </summary>
        ZhangXinLei10008 = 8 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 恶魔的咆哮(怪物)
        /// </summary>
        EMoDePaoXiao10009 = 9 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 暴风雪
        /// </summary>
        BaoFengXue10010 = 10 + BattleHeroSkillConstData.EnemyId,

        /// <summary>
        /// 天雷闪
        /// </summary>
        TianLeiShan10011 = 11 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 地裂
        /// </summary>
        DiLie10012 = 12 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 霹雳回旋斩
        /// </summary>
        XuanFengHuiXuanZhan10013 = 13 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 烈光闪
        /// </summary>
        LieGuangShan10014 = 14 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 五重箭
        /// </summary>
        WuChongJian10015 = 15 + BattleHeroSkillConstData.EnemyId,

        /// <summary>
        /// 冰霜术
        /// </summary>
        BossBingShuangShu10016 = 16 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 火焰术
        /// </summary>
        BossHuoYanShu10017 = 17 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 神龙摆尾
        /// </summary>
        BossShenLongBaiWei10018 = 18 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 毁灭烈焰
        /// </summary>
        BossHuiMieLieYan10019 = 19 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 蜘蛛腿
        /// </summary>
        BossZhiZhuTui10020 = 20 + BattleHeroSkillConstData.EnemyId,

        /// <summary>
        /// 昆顿火焰术
        /// </summary>
        BossKunDunHuoYanShu10021 = 21 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 天鹰技能1
        /// </summary>
        TianYing10022 = 22 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 天鹰技能2
        /// </summary>
        TianYing10023 = 23 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 冰封雨
        /// </summary>
        BingFengYu10024 = 24 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 破御阵
        /// </summary>
        PoYuZhen10025 = 25 + BattleHeroSkillConstData.EnemyId,

        /// <summary>
        /// 冰霜术
        /// </summary>
        BingShuangShu10026 = 26 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 毁灭烈焰
        /// </summary>
        HuiMieLieYan10027 = 27 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 地裂斩
        /// </summary>
        GroundFissureChop10028 = 28 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 火焰术
        /// </summary>
        HuoYanSu10029 = 29 + BattleHeroSkillConstData.EnemyId,
        /// <summary>
        /// 致命一击(猴王)
        /// </summary>
        ZhiMingYiJi10030 = 30 + BattleHeroSkillConstData.EnemyId,


        /// <summary>
        /// 霹雳回旋斩
        /// </summary>
        XuanFengHuiXuanZhan10031 = 31 + BattleHeroSkillConstData.EnemyId,

        #endregion


        /// <summary>
        /// MAX
        /// </summary>
        MAX = 1000,
    }
}
