using CustomFrameWork.Baseic;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    public enum CopyType
    {
        None = 0,
        DemonSquae = 1,
        RedCastle = 2,
        //AncientBattlefield =3,
        CangBaoTu = 4,
    }
   
    public class BatteCopyManagerComponent : TCustomComponent<C_ServerArea>
    {
        /// <summary>
        /// 副本MapID列表
        /// </summary>
        public static List<int> BattleCopyMapIDList = new List<int>() { 100, 101, 103, 104, 105, 106, 107, 108, 109, 110 };
        /// <summary>
        /// 恶魔广场挑战次数上限
        /// </summary>
        public int demonSquaeNum;

        /// <summary>
        /// 血色城堡挑战次数上限
        /// </summary>
        public int redCastleNum;
        /// <summary>
        /// 古战场次数预留
        /// </summary>
        //public int AncientBattlefield;
        public readonly Dictionary<int, BattleCopyComponent> battleCopyMap = new Dictionary<int, BattleCopyComponent>();
        /// <summary>
        /// 试炼塔
        /// </summary>
        public Dictionary<long, MapComponent> TrialTowerList = new Dictionary<long, MapComponent>();
        public int GameAreaId { get; set; }

        /*public override void Awake(int b_AreaId)
        {
            GameAreaId = b_AreaId;
        }*/
    }
}
