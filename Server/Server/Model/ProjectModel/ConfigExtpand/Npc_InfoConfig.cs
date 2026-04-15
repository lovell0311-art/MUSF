using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// Npc.xlsx-npc
    /// </summary>
    public partial class Npc_InfoConfig
    {

        /// <summary>
        /// 附加信息
        /// </summary>
        [JsonIgnore] public Dictionary<string, string> OtherDataDic { get; set; }
        [JsonIgnore] public List<int> EquipDataList { get; set; }
        public override void InitExpand()
        {
            OtherDataDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(OtherData);
            EquipDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(EquipData); 
        }
    }
}