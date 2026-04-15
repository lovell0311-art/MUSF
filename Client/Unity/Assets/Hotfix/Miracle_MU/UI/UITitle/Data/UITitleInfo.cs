using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public class UITitleInfo
    {
        /// <summary>
        /// 称号ID
        /// </summary>
        public long TitleId;
        /// <summary>
        /// 资源名
        /// </summary>
        public string TitleAssetsName;
        /// <summary>
        /// 称号描述
        /// </summary>
        public string Describe;
        /// <summary>
        /// 获取途径
        /// </summary>
        public string GetWay;
        /// <summary>
        /// 称号附加属性
        /// </summary>
        public string Attribute;
        /// <summary>
        /// 0是未获取，1是未使用，2是正在使用
        /// </summary>
        public int UseInfo;
        /// <summary>
        /// 开始时间
        /// </summary>
        public long BingTime = -1;
        /// <summary>
        /// 结束时间，0就是永久
        /// </summary>
        public long EndTime = -1;
        /// <summary>
        /// 模型
        /// </summary>
        public GameObject titleModle;
    }
    public static class TitleManager
    {
        /// <summary>
        /// 当前使用称号
        /// </summary>
        public static int useID;
        /// <summary>
        /// 当前已有称号
        /// </summary>
        public static List<UITitleInfo> allTitles = new List<UITitleInfo>();
    }
}
