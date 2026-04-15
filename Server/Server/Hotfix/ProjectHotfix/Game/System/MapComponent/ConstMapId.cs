using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using TencentCloud.Vod.V20180717.Models;
using System.Collections.Generic;
using System.Linq;

namespace ETHotfix
{
    public static class ConstMapId
    {
        /// <summary>
        /// 勇者大陆
        /// </summary>
        public const int YongZheDaLu = 1;
        /// <summary>
        /// 仙踪林
        /// </summary>
        public const int XianZongLin = 2;
        /// <summary>
        /// 幻术园
        /// </summary>
        public const int HuanShuYuan = 3;
        /// <summary>
        /// 天空之城
        /// </summary>
        public const int TianKongZhiCheng = 10;
        /// <summary>
        /// 古战场
        /// </summary>
        public const int GuZhanChang = 102;
        /// <summary>
        /// 囚禁之岛
        /// </summary>
        public const int QiuJinZhiDao = 17;
        public const string ListVlaue = "[8,9,10,11,12,13,14,15,16,17,102,112]";
        public static List<int> GetListValue()
        {
            return Help_JsonSerializeHelper.DeSerialize<List<int>>(ListVlaue);
        }
    }
}
