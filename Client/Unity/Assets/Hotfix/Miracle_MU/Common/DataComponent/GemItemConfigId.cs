using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 宝石物品
    /// </summary>
    public static class GemItemConfigId
    {
        /// <summary>祝福宝石</summary>
        public static int BLESSING_GEMS = 280003;
        /// <summary>灵魂宝石</summary>
        public static int SOUL_GEMS = 280004;
        /// <summary>生命宝石</summary>
        public static int ANIMA_GEMS = 280005;
        /// <summary>玛雅之石</summary>
        public static int MAYA_GEMS = 280001;
        /// <summary>创造宝石</summary>
        public static int CREATE_GEMS = 280006;
        /// <summary>守护宝石</summary>
        public static int GUARDIAN_GEMS = 280007;
        /// <summary>再生原石</summary>
        public static int ORECYCLED_GEMS = 280002;
        /// <summary>再生宝石</summary>
        public static int RECYCLED_GEMS = 280011;
        /// <summary>初级进化宝石</summary>
        public static int ELEMENTARY_EVOLUTION_GEMS = 280012;
        /// <summary>高级进化宝石</summary>
        public static int ADVANCED_EVOLUTION_GEMS = 280013;
        /// <summary>低级魔晶石</summary>
        public static int LOW_LEVEL_MOJING_STONE = 280017;
        /// <summary>中级魔晶石</summary>
        public static int MIDDLE_LEVEL_MOJING_STONE = 280018;
        /// <summary>高级魔晶石</summary>
        public static int High_LEVEL_MOJING_STONE = 280019;  
        /// <summary>魔晶石</summary>
        public static int LEVEL_MOJING_STONE = 280020; 
        /// <summary>光之石</summary>
        public static int GUANG_ZHI_SHI = 270001; 
        /// <summary>光之石强化符文</summary>
        public static int GUANG_ZHI_SHI_QIANGHUA_FUWEN = 320302;
        /// <summary>荧之石</summary>
        public static Dictionary<long,string> YING_ZHI_SHI_DIC=new Dictionary<long, string>() 
        {
            {270002,"荧之石(火)" },
            {270003,"荧之石(水)" },
            {270004,"荧之石(冰)" },
            {270005,"荧之石(风)" },
            {270006,"荧之石(雷)" },
            {270007,"荧之石(土)" },
        };
        /// <summary>荧光宝石</summary>
        public static Dictionary<long, string> YING_GUANG_GEM_DIC = new Dictionary<long, string>()
        {
            {270008,"荧光宝石(火)" },
            {270009,"荧光宝石(水)" },
            {270010,"荧光宝石(冰)" },
            {270011,"荧光宝石(风)" },
            {270012,"荧光宝石(雷)" },
            {270013,"荧光宝石(土)" },
        };


    }
}
