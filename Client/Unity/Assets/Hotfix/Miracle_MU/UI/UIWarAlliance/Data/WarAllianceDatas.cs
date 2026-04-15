using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 战盟数据 
    /// </summary>
    public class WarAllianceDatas
    {
        public static List<WarInfo> WarLists = new List<WarInfo>();//战盟 列表
        /// <summary>
        /// 战盟成员
        /// </summary>
        public static List<WarMemberInfo> WarMemberList = new List<WarMemberInfo>();
        public static bool IsJoinWar = false;
        /// <summary>
        /// 清理 战盟数据
        /// </summary>
        public static void Clear()
        {
            WarLists.Clear();
            WarMemberList.Clear();
        }

    }
}