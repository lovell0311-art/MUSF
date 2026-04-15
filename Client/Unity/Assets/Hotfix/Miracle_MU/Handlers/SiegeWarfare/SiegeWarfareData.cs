using ETModel;

namespace ETHotfix
{
    public static class SiegeWarfareData
    {
        /// <summary>
        /// 当前宝座是否有玩家
        /// </summary>
        public static bool HavePlayer = false;
        /// <summary>
        /// 当前宝座上的玩家
        /// </summary>
        public static RoleEntity currole = null;
        /// <summary>
        /// 当前宝座上的玩家ID
        /// </summary>
        public static long CurroleId = 0;
        /// <summary>
        /// 攻城战是否开启
        /// </summary>
        public static bool SiegeWarfareIsStart = false;
    }
   
}
