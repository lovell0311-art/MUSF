using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETHotfix
{

    /// <summary>
    /// 游戏区/服信息
    /// </summary>
    public class ServerZoneInfo
    {
        private int zoneId = 0;
        private string zonename = "";
        private int zoneType = (int)E_LineStateType.NONE;
        private int zoneState = (int)E_LineState.CLOSE;

        /// <summary>
        /// 游戏区/服ID
        /// </summary>
        public int ZoneId { get => zoneId; set => zoneId = value; }
        /// <summary>
        /// 游戏区/服状态
        /// 0->正常
        /// 1->爆满
        /// 2->新服
        /// </summary>
        public int ZoneType { get => zoneType; set => zoneType = value; }
        /// <summary>
        /// 游戏区/服名字
        /// </summary>
        public string Zonename { get => zonename; set => zonename = value; }
        /// <summary>
        /// 游戏区/服状态
        /// 0->关闭
        /// 1->开启
        /// </summary>
        public int ZoneState { get => zoneState; set => zoneState = value; }
    }

    /// <summary>
    /// 线路信息
    /// </summary>
    public class LineInfo 
    {
        public int GameAreaId;//区服ID
        public string GameAreaNickName;//线路名
        public int GameAreaType;//线路状态
        public int IsGameAreaState;//线路是否开启
    }


    public class AccouncInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id;
        /// <summary>
        /// 1：公告 2：活动
        /// </summary>
        public int Type;
        /// <summary>
        /// 公告名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 公告内容
        /// </summary>
        public string Content;
    }
}
