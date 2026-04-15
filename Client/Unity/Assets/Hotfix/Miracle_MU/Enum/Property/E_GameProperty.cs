using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    /// <summary>
    /// 鏈嶅姟鍣ㄥ崗璁疄浣撳睘鎬D
    /// 
    /// </summary>
    public enum E_GameProperty : int
    {

        /// <summary>鍔涢噺</summary>
        Property_Strength = 1,
        /// <summary>鎰忓織 鏅哄姏</summary>
        Property_Willpower = 2,
        /// <summary>鏁忔嵎</summary>
        Property_Agility = 3,
        /// <summary>浣撳姏</summary>
        Property_BoneGas = 4,
        /// <summary>缁熺巼</summary>
        Property_Command = 5,

        /// <summary>褰撳墠 鐢熷懡鍊?/summary>
        PROP_HP = 10,
        /// <summary>褰撳墠 榄旀硶鍊?/summary>
        PROP_MP = 11,
        /// <summary>褰撳墠 SD</summary>
        PROP_SD = 12,
        /// <summary>褰撳墠 AG</summary>
        PROP_AG = 13,
        /// <summary>鏈€澶?鐢熷懡鍊?/summary>
        PROP_HP_MAX = 14,
        /// <summary>鏈€澶?榄旀硶鍊?/summary>
        PROP_MP_MAX = 15,
        /// <summary>鏈€澶?Sd</summary>
        PROP_SD_MAX = 16,
        /// <summary>鏈€澶?AG</summary>
        PROP_AG_MAX = 17,
        Injury_HP = 18,
        Injury_SD = 19,


        /// <summary>鏈€澶ф敾鍑诲姏</summary>
        MaxAtteck = 21,
        /// <summary>鏈€灏忔敾鍑诲姏</summary>
        MinAtteck = 22,
        /// <summary>鏈€澶ч瓟娉曟敾鍑伙紙鏈€澶ч瓟鍔涳級</summary>
        MaxMagicAtteck = 23,
        /// <summary>鏈€灏忛瓟娉曟敾鍑伙紙鏈€灏忛瓟鍔涳級</summary>
        MinMagicAtteck = 24,
        /// <summary> 鏈€澶у厓绱犳敾鍑?鐣欑┖)</summary>
        MaxElementAtteck = 25,
        /// <summary>鏈€灏忓厓绱犳敾鍑?鐣欑┖)</summary>
        MinElementAtteck = 26,
        /// <summary>鏀诲嚮鎴愬姛鐜?/summary>
        AtteckSuccessRate = 27,
        /// <summary>
        /// PVP鏀诲嚮鎴愬姛鐜?
        /// </summary>
        PVPAtteckSuccessRate = 28,
        /// <summary>鍏冪礌鏀诲嚮鎴愬姛鐜?/summary>
        ElementAtteckSuccessRate = 29,
        /// <summary>PVP鍏冪礌鏀诲嚮鎴愬姛鐜?/summary>
        PVPElementAtteckSuccessRate = 30,
        /// <summary>闃插尽鐜?/summary>
        DefenseRate = 31,
        /// <summary>鍏冪礌闃插尽鐜?/summary>
        ElementDefenseRate = 32,
        /// <summary>PVP闃插尽鐜?/summary>
        PVPDefenseRate = 33,
        /// <summary>PVP鍏冪礌闃插尽鐜?/summary>
        PVPElementDefenseRate = 34,
        /// <summary>闃插尽</summary>
        Defense = 35,
        /// <summary>鍏冪礌闃插尽</summary>
        ElementDefense = 36,
        /// <summary>鏀婚€?/summary>
        AttackSpeed = 37,
        /// <summary>绉婚€?/summary>
        MoveSpeed = 38,
        /// <summary>绉婚€熷閲?/summary>
        MoveSpeed_Increase = 39,
        /// <summary>绉婚€熷噺閲?/summary>
        MoveSpeed_Reduce = 40,

        /// <summary>鏀诲嚮璺濈</summary>
        AttackDistance = 41,
        /// <summary>鎶€鑳戒激瀹冲骞?/summary>
        SkillAddition = 42,

        /// <summary>pvp 闄勫姞鏀诲嚮</summary>
        PVPAttack = 43,
        /// <summary>pvp 闄勫姞闃插尽</summary>
        PVPDefense = 44,
        /// <summary>pve 闄勫姞鏀诲嚮</summary>
        PVEAttack = 45,
        /// <summary>pve 闄勫姞闃插尽</summary>
        PVEDefense = 46,
        /// <summary>鐪熷疄闃插尽</summary>
        ReallyDefense = 47,

        /// <summary>鐢熷懡鑷姩鎭㈠閲?7s</summary>
        ReplyHp = 50,
        /// <summary>榄斿姏鑷姩鎭㈠閲?3s</summary>
        ReplyMp = 51,
        /// <summary>AG鑷姩鎭㈠閲?3s</summary>
        ReplyAG = 52,
        /// <summary>SD鑷姩鎭㈠閲?7s</summary>
        ReplySD = 53,

        /// <summary>鐢熷懡鎭㈠鍑犵巼</summary>
        ReplyHpRate = 54,
        /// <summary>榄斿姏鎭㈠鍑犵巼</summary>
        ReplyMpRate = 55,
        /// <summary>AG鎭㈠鍑犵巼</summary>
        ReplyAGRate = 56,
        /// <summary>SD鎭㈠鍑犵巼</summary>
        ReplySDRate = 57,

        /// <summary>鍑绘潃鎬墿鐢熷懡鑷姩鎭㈠閲?/summary>
        KillEnemyReplyHp = 58,
        KillEnemyReplyHpRate = KillEnemyReplyHp,
        /// <summary>鍑绘潃鎬墿榄旀硶鍊艰嚜鍔ㄦ仮澶嶉噺</summary>
        KillEnemyReplyMp = 59,
        KillEnemyReplyMpRate = KillEnemyReplyMp,
        /// <summary>鍑绘潃鎬墿AG鑷姩鎭㈠閲?/summary>
        KillEnemyReplyAG = 60,
        KillEnemyReplyAGRate = KillEnemyReplyAG,
        /// <summary>鍑绘潃鎬墿SD鑷姩鎭㈠閲?/summary>
        KillEnemyReplySD = 61,

        KillEnemyReplySDRate = KillEnemyReplySD,
        ReplyAllHpRate = 62,
        ReplyAllMpRate = 63,
        ReplyAllAGRate = 64,
        ReplyAllSdRate = 65,

        /// <summary>鍙楀埌浼ゅ鏃?Hp瀹屽叏鎭㈠鍑犵巼</summary>
        Injury_ReplyAllHpRate = 66,
        /// <summary>鍙楀埌浼ゅ鏃?Hp瀹屽叏鎭㈠鍑犵巼</summary>
        Injury_ReplyAllMpRate = 67,
        /// <summary>鏀诲嚮鏃?SD瀹屽叏鎭㈠鍑犵巼</summary>
        Attack_ReplyAllSdRate = 68,

        /// <summary>
        /// 鐢熷懡鍔涘惛鏀堕噺
        /// 鏀诲嚮鏁屼汉鏃舵瘡娆℃敾鍑绘垚鍔燂紝浠?0%鐨勬鐜囩敓鍛藉姏鎭㈠{0:F}銆?
        /// </summary>
        HpAbsorbRate = 80,
        /// <summary>
        /// sd鍚告敹閲?
        /// 鏀诲嚮鏁屼汉鏃舵瘡娆℃敾鍑绘垚鍔燂紝浠?0%鐨勬鐜噑d鎭㈠{0:F}銆?
        /// </summary>
        SdAbsorbRate = 81,

        /// <summary>榄旀硶鍊间娇鐢ㄥ噺灏戠巼</summary>
        MpConsumeRate_Reduce = 82,
        /// <summary>AG浣跨敤鍑忓皯鐜?/summary>
        AgConsumeRate_Reduce = 83,

        /// <summary>鏀诲嚮鏃舵棤瑙嗛槻寰℃鐜?/summary>
        AttackIgnoreDefenseRate = 84,
        /// <summary>鍙嶅脊鍑犵巼</summary>
        ReboundRate = 85,
        /// <summary>骞歌繍涓€鍑讳激瀹冲鍔犻噺</summary>
        LucklyAttackHurtValueIncrease = 86,
        /// <summary>鍗撹秺涓€鍑讳激瀹冲鍔犻噺</summary>
        ExcellentAttackHurtValueIncrease = 87,
        /// <summary>浼ゅ鎻愰珮鐜?/summary>
        InjuryValueRate_Increase = 88,
        /// <summary>浼ゅ鍑忓皯鐜?/summary>
        InjuryValueRate_Reduce = 89,
        /// <summary>浼ゅ鍑忓皯閲?/summary>
        InjuryValue_Reduce = 90,
        /// <summary>浼ゅ鍙嶅皠鐜?/summary>
        BackInjuryRate = 91,
        /// <summary>浼ゅ鍚告敹鐜?/summary>
        HurtValueAbsorbRate = 92,

        /// <summary>琚嚮鏃禨d姣旂巼</summary>
        HitSdRate = 93,
        /// <summary>鏀诲嚮鏃禨d姣旂巼</summary>
        AttackSdRate = 94,
        /// <summary>鏀诲嚮鏃舵棤瑙哠d姒傜巼</summary>
        SDAttackIgnoreRate = 95,
        /// <summary>鏉熺細鍑犵巼</summary>
        ShacklesRate = 96,
        /// <summary>鏉熺細鎶垫姉鍑犵巼</summary>
        ShacklesResistanceRate = 97,
        /// <summary>鐩剧墝浼ゅ鍚告敹閲?/summary>
        ShieldHurtAbsorb = 98,
        /// <summary>闃茬浘鍑犵巼</summary>
        DefenseShieldRate = 99,
        /// <summary>鑾峰緱閲戝竵澧炲姞鐜?/summary>   
        AddGoldCoinRate_Increase = 100,
        /// <summary>榄旀潠榄斿姏鎻愬崌鐧惧垎姣?/summary>
        MagicRate_Increase = 101,
        /// <summary>鏍兼尅鍑犵巼</summary>
        GridBlockRate = 102,
        /// <summary>瀹堟姢鐩惧嚑鐜?/summary>
        GuardShieldRate = 103,
        /// <summary>
        /// 瀹犵墿閮ㄥ垎灞炴€?鏀诲嚮鍔犳垚鐧惧垎姣?
        /// </summary>
        AttackBonus = 104,
        /// <summary>
        /// 闃插尽鍔犳垚鐧惧垎姣?
        /// </summary>
        DefenseBonus = 105,
        /// <summary>
        /// 鏀婚€熷姞鎴愮櫨鍒嗘瘮
        /// </summary>
        AttackSpeedBonus = 106,
        /// <summary>
        /// 闃插尽鐜囧姞鎴愮櫨鍒嗘瘮
        /// </summary>
        DefenseRateBonus = 107,
        /// <summary>
        /// 鐢熷懡鍊煎姞鎴愮櫨鍒嗘瘮
        /// </summary>
        HealthBonus = 108,
        /// <summary>
        /// 瀹犵墿钃濋噺鍔犳垚鐧惧垎姣?
        /// </summary>
        MagicBonus = 109,
        /// <summary>
        /// 鎶€鑳芥敾鍑诲姏
        /// </summary>
        SkillAttack = 110,
        /// <summary>
        /// 瀹犵墿浼ゅ鍚告敹锛岀櫨鍒嗘瘮
        /// </summary>
        PetsDamageAbsorption,
        /// <summary>
        /// 鏈€澶ц瘏鍜掓敾鍑?
        /// </summary>
        MaxDamnationAtteck,
        /// <summary>
        /// 鏈€灏忚瘏鍜掓敾鍑?
        /// </summary>
        MinDamnationAtteck,
        /// <summary>涔﹁瘏鍜掓彁鍗囩櫨鍒嗘瘮</summary>
        DamnationRate_Increase,


        ///////////////////////////鐧惧垎姣斿睘鎬?2绾у睘鎬?////////////////////
        PROP_HP_MAXPct,
        PROP_MP_MAXPct,
        PROP_SD_MAXPct,
        PROP_AG_MAXPct,
        MaxAtteckPct,
        MinAtteckPct,
        MaxMagicAtteckPct,
        MinMagicAtteckPct,
        MaxElementAtteckPct,
        MinElementAtteckPct,
        AtteckSuccessRatePct,
        PVPAtteckSuccessRatePct,
        ElementAtteckSuccessRatePct,
        PVPElementAtteckSuccessRatePct,
        DefenseRatePct,
        ElementDefenseRatePct,
        PVPDefenseRatePct,
        PVPElementDefenseRatePct,
        DefensePct,
        ElementDefensePct,
        MaxDamnationAtteckPct,
        MinDamnationAtteckPct,
        ///////////////////////////鐧惧垎姣斿睘鎬?////////////////////


        ///////////////////////////鐗╁搧璇嶆潯灞炴€?////////////////////
        /// <summary>鍑绘潃鎬墿鎭㈠鑷繁鏈€澶х敓鍛?8 HpMax/8*Value</summary>
        KillMonsterReplyHp_8,
        /// <summary>鍑绘潃鎬墿鎭㈠鑷繁鏈€澶ч瓟娉曞€?8 MpMax/8*Value</summary>
        KillMonsterReplyMp_8,
        /// <summary>
        /// 浼ゅ鍚告敹鐧惧垎姣?瀹堟姢
        /// </summary>
        DamageAbsPct_Guard,
        /// <summary>
        /// 浼ゅ鍚告敹鐧惧垎姣?鍧愰獞
        /// </summary>
        DamageAbsPct_Mounts,
        /// <summary>
        /// 浼ゅ鍚告敹鐧惧垎姣?缈呰唨
        /// </summary>
        DamageAbsPct_Wing,
        ///////////////////////////鐗╁搧璇嶆潯灞炴€?////////////////////


        ///////////////////////////澶у笀灞炴€?////////////////////
        /// <summary>
        /// 瑁呭涓殑姝﹀櫒鍜岄槻鍏疯€愪箙涓嬮檷閫熷害鍑忔參 鐧惧垎姣?
        /// </summary>
        MpsDownDur1,
        /// <summary>
        /// 瑁呭涓殑棣栭グ锛堥」閾撅紝鎴掓寚锛夎€愪箙涓嬮檷閫熷害鍑忔參 鐧惧垎姣?
        /// </summary>
        MpsDownDur2,
        /// <summary>
        /// 锛堝皬鎭堕瓟銆佸皬澶╀娇銆佸吔瑙掋€佸僵浜戝吔銆佺値鐙煎吔涔嬭锛夎€愪箙涓嬮檷閫熷害鍑忔參 鐧惧垎姣?
        /// </summary>
        MpsDownDur3,
        /// <summary>
        /// 瀹犵墿(榛戠帇椹€佸ぉ楣?鐨勭敓鍛藉姏鍑忓皯閫熷害 鐧惧垎姣?
        /// </summary>
        MpsPetDurDownSpeed,
        /// <summary>
        /// 鏁村闃插尽鍔涘鍔?
        /// </summary>
        SpecialDefenseRate,
        ///////////////////////////澶у笀灞炴€?////////////////////





        ///////////////////////////鏁堟灉鍙婂睘鎬?////////////////////
        /// <summary>鍟ラ兘娌¤Е鍙?/summary>
        NullAttack,
        /// <summary>鍙屽€嶄激瀹虫鐜?/summary>
        InjuryValueRate_2,
        /// <summary>涓夊€嶄激瀹虫鐜?/summary>
        InjuryValueRate_3,
        /// <summary>骞歌繍涓€鍑绘鐜? 鏈€澶т激瀹?/summary>
        LucklyAttackRate,
        /// <summary>鍗撹秺涓€鍑绘鐜? 1.3鍊?/summary>
        ExcellentAttackRate,
        ///////////////////////////鏁堟灉鍙婂睘鎬?////////////////////
        ///
        /// <summary>
        /// 鍐版姉鎬?
        /// </summary>
        IceResistance,
        /// <summary>
        /// 姣掓姉鎬?
        /// </summary>
        CurseResistance,
        /// <summary>
        /// 鐏姉鎬?
        /// </summary>
        FireResistance,
        /// <summary>
        /// 闆锋姉鎬?
        /// </summary>
        ThunderResistance,

        /// <summary>
        /// 绛夌骇
        /// </summary>
        Level = 200,
        /// <summary>
        /// 杞亴绛夌骇
        /// </summary>
        OccupationLevel = 201,
        /// <summary>
        /// 鑱旂洘鍚嶅瓧
        /// </summary>
        UnionName = 202,
        /// <summary>
        /// 鑷敱鐐规暟
        /// </summary>
        FreePoint = 203,
        /// <summary>
        /// 褰撳墠缁忛獙
        /// </summary>
        Exprience = 204,
        /// <summary>
        /// 鑾峰彇缁忛獙
        /// </summary>
        ExprienceDrop = 205,
        /// <summary>
        /// 鏈嶅姟鍣?鏃堕棿鎴?
        /// </summary>
        ServerTime = 206,
        /// <summary>
        /// 閲戝竵
        /// </summary>
        GoldCoin = 207,
        /// <summary>
        /// 閲戝竵鍙樺寲鍊?
        /// </summary>
        GoldCoinChange = 208,
        /// <summary>
        /// 缁忛獙鍔犳垚鐧惧垎姣?
        /// </summary>
        ExperienceBonus = 209,
        /// <summary>
        /// 鐖嗙巼鍔犳垚鐧惧垎姣?
        /// </summary>
        ExplosionRate = 230,
        /// <summary>
        /// 閬撳叿鍥炴敹閲戝竵鍔犳垚鐧惧垎姣?
        /// </summary>
        GoldCoinMarkup = 231,
        /// <summary>
        /// 濂囪抗甯?
        /// </summary>
        MiracleCoin = 232,
        /// <summary>
        /// 濂囪抗甯佸彉鍖栧€?
        /// </summary>
        MiracleChange = 233,
        /// <summary>
        /// 榄旀櫠鏁伴噺
        /// </summary>
        MoJing = 234,
        /// <summary>
        /// 鐟炲竵鍙樺寲鍊?
        /// </summary>
        MoJingChange = 235,
        /// <summary>
        /// pk妯″紡 0:鍜屽钩 1:鍏ㄤ綋 2:鍙嬫柟
        /// </summary>
        PlayerKillingMedel = 236,
        /// <summary>
        /// pk 鐐规暟
        /// </summary>
        PkNumber = 237,
        /// <summary>
        /// pk 鐐规暟
        /// </summary>
        Pet_FangYuHuZhao = 240,

        IgnoreAbsorbRate = 272,
        AttackIgnoreAbsorbRate = 273,
        /// <summary>
        /// 鏍兼枟瀹惰繎鎴樻敾鍑诲姏
        /// </summary>
        AdvanceAttackPower = 274,
        /// <summary>
        /// 鑼冨洿鏀诲嚮鍔?
        /// </summary>
        RangeAttack,
        /// <summary>
        /// 鏍兼枟瀹剁鍏芥敾鍑诲姏
        /// </summary>
        SacredBeast,

        /// <summary>
        /// 姊﹀够楠戝＋鎯╁鏀诲嚮鍔?
        /// </summary>
        DreamRiderPenalize,
        /// <summary>
        /// 姊﹀够楠戝＋婵€鎬掓敾鍑诲姏
        /// </summary>
        DreamRiderIrritate,
        /// <summary>
        /// 瀹犵墿浼ゅ鍚告敹
        /// </summary>
        DamageAbsPct_Pets = 279,
        PetsDamageAbsorptionNew = DamageAbsPct_Pets,
        DisregardHarmReductionPct = 280,
        AllianceScoreChange = 281,
        GamePropertyMax = 282,
        /*
         * 瀹㈡埛绔?鑷畾涔夌殑灞炴€?
         */
        /// <summary>
        /// 绱鍏呭€奸搴?
        /// </summary>
        AccumulatedRecharge = 10002,
       
        /// <summary>
        /// 鍘熷湴澶嶆椿cd鍒版湡鏃堕棿
        /// </summary>
        InSituCd = 10003,
        /// <summary>鏀诲嚮闂撮殧鏃堕棿</summary>
        AttackSpaceTime = 10004,
        //鍓婂急瀵规柟鍑忎激
        weaken = DisregardHarmReductionPct,
        //鎴樼洘绉垎
        AllianceIntegral = AllianceScoreChange,


    }



}


