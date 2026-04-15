using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public class BattleMasterInfos
    {
        public long Id { get; set; }
        ///<summary>名字 </summary>
        public string Name;
        ///<summary>介绍 </summary>
        public string Describe;
        ///<summary>等级 </summary>
        public int LayerLevel;
        ///<summary>前置技能 </summary>
        public List<int> FrontIds = new List<int>();
        ///<summary>上一阶技能 </summary>
        public List<int> LastIds = new List<int>();
        ///<summary>升级消耗 </summary>
        public int Consume;
        ///<summary>附加数值 </summary>
        public string OtherData;
    }
}
