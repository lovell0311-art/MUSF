using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    public enum E_PKLevel
    {
        /// <summary>
        /// 白名
        /// </summary>
        White = 0,
        /// <summary>
        /// 黄名
        /// </summary>
        Yellow = 21600,
        /// <summary>
        /// 红名
        /// </summary>
        Red = 43200,
        /// <summary>
        /// 深红
        /// </summary>
        Crimson = 9999999
    }


    public enum E_PKModel
    {
        /// <summary>
        /// 和平
        /// </summary>
        Peace = 0,
        /// <summary>
        /// 全体
        /// </summary>
        AllBoddy = 1,
        /// <summary>
        /// 友方
        /// </summary>
        Friend = 2,
    }
}